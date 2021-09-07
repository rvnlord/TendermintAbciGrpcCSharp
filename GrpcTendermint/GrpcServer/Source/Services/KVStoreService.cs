using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using GrpcServer.Source.Common.Converters;
using GrpcServer.Source.Common.Extensions;
using GrpcServer.Source.Models;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tendermint.Abci;

namespace GrpcServer.Source.Services
{
    public class KVStoreService : ABCIApplication.ABCIApplicationBase
    {
        private readonly IConfiguration _conf;
        private readonly ILogger<KVStoreService> _logger;
        private static LiteDatabase _env;
        private static ILiteCollection<KV> _store;
        
        public LiteDatabase Env => _env ??= new LiteDatabase(_conf.GetConnectionString("LiteDb"));
        public ILiteCollection<KV> Store => _store ??= Env.GetCollection<KV>("kvs");

        public KVStoreService(IConfiguration conf, ILogger<KVStoreService> logger)
        {
            _conf = conf;
            _logger = logger;
        }
        
        public override Task<ResponseEcho> Echo(RequestEcho request, ServerCallContext context)
        {
            var echo = new ResponseEcho { Message = $"Validator is Running: {DateTime.Now:dd-MM-yyyy HH:mm}" };
            return Task.FromResult(echo).LogAsync(_logger, $"Echo Status: {echo.Message}");
        }

        public override Task<ResponseInfo> Info(RequestInfo request, ServerCallContext context)
        {
            var info = new ResponseInfo { Data = $"Version: {request.Version}, Abci Version: {request.AbciVersion}, P2P Version: {request.P2PVersion}, Block Versions: {request.BlockVersion}" };
            return Task.FromResult(info).LogAsync(_logger, $"Info Status: {info.Data}");
        }

        public override Task<ResponseInitChain> InitChain(RequestInitChain request, ServerCallContext context)
        {
            Store.EnsureIndex(x => x.Key, true);
            return Task.FromResult(new ResponseInitChain()).LogAsync(_logger, "InitChain Status: Success");
        }

        public override Task<ResponseBeginBlock> BeginBlock(RequestBeginBlock request, ServerCallContext context)
        {
            Env.BeginTrans();
            return Task.FromResult(new ResponseBeginBlock()).LogAsync(_logger, "Begin Block Status: Success");
        }

        public override Task<ResponseDeliverTx> DeliverTx(RequestDeliverTx request, ServerCallContext context)
        {
            var (code, kv) = Validate(request.Tx);
            if (code == 0)
                Store.Insert(kv);
            else if (code == 3)
                Store.Update(kv);
            return Task.FromResult(new ResponseDeliverTx { Code = code.In<uint>(0, 3) ? 0 : code }).LogAsync(_logger, $"Delivery Status: {code switch { 0 => "Success", 1 => "Invalid Data", 2 => "Already Exists", 3 => "Already Exists with Different Value", _ => "Failure" }}");
        }

        public override Task<ResponseCommit> Commit(RequestCommit request, ServerCallContext context)
        {
            Env.Commit(); // timed out waiting for tx to be included in a block
            return Task.FromResult(new ResponseCommit { Data = ByteString.CopyFrom(new byte[8]) }).LogAsync(_logger, "Commit Status: Success");
        }

        public override Task<ResponseCheckTx> CheckTx(RequestCheckTx request, ServerCallContext context)
        {
            var (code, _) = Validate(request.Tx);
            return Task.FromResult(new ResponseCheckTx { Code = code.In<uint>(0, 3) ? 0 : code, GasWanted = 1 }).LogAsync(_logger, $"CheckTx Status: {code switch { 0 => "Success", 1 => "Invalid Data", 2 => "Already Exists", 3 => "Already Exists with Different Value", _ => "Failure" }}");
        }

        public override Task<ResponseEndBlock> EndBlock(RequestEndBlock request, ServerCallContext context)
        {
            return Task.FromResult(new ResponseEndBlock());
        }
        
        public override Task<ResponseQuery> Query(RequestQuery request, ServerCallContext context)
        {
            var k = request.Data.ToBase64();
            var v = Store.FindOne(x => x.Key == k)?.Value;
            var resp = new ResponseQuery();
            if (v == null)
                resp.Log = $"There is no value for \"{k}\" key";
            else 
            {
                resp.Log = "KVP:";
                resp.Key = ByteString.FromBase64(k);
                resp.Value = ByteString.FromBase64(v);
            }

            return Task.FromResult(resp).LogAsync(_logger, $"Query Status: {resp.Log} {resp.Key.ToStringUtf8()}{(resp.Key == ByteString.Empty || resp.Value == ByteString.Empty ? "" : "=")}{resp.Value.ToStringUtf8()}");
        }

        private (uint, KV) Validate(ByteString tx) 
        {
            var kv = tx.ToStringUtf8().Split('=').Select(kv => kv.UTF8ToBase64()).ToKV();
            if (kv.Key.IsNullOrWhiteSpace() || kv.Value.IsNullOrWhiteSpace())
                return (1, kv); // Invalid data
            
            var stored = Store.FindOne(x => x.Key == kv.Key)?.Value;
            if (stored == null) 
                return (0, kv); // Not in the Db Yet
            if (stored == kv.Value)
                return (2, kv); // Already Exists with the Same Value
            return (3, kv); // Already Exists with different Value
        }
    }
}
