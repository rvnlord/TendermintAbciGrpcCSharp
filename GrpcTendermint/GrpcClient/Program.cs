using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Tendermint.Abci;

namespace GrpcClient
{
    public class Program
    {
        public static async Task Main()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:26658");
            var client = new ABCIApplication.ABCIApplicationClient(channel);

            string echo = null;
            while (echo == null)
            {
                try
                {
                    echo = client.Echo(new RequestEcho()).Message;
                }
                catch (Exception ex) when (ex is HttpRequestException or RpcException)
                {
                    await Task.Delay(1000);
                    System.Console.WriteLine("Server not Ready, retrying...");
                }
            }
            
            var info = await client.InfoAsync(new RequestInfo { Version = "v0.0.test", AbciVersion = "v0.1.test", BlockVersion = 0, P2PVersion = 1 });
            var initChain = await client.InitChainAsync(new RequestInitChain());
            var beginBlock = await client.BeginBlockAsync(new RequestBeginBlock());
            var deliver = await client.DeliverTxAsync(new RequestDeliverTx { Tx = ByteString.CopyFromUtf8("tendermint=rocks") });
            var commit = await client.CommitAsync(new RequestCommit());
            var checkTx = await client.CheckTxAsync(new RequestCheckTx { Tx = ByteString.CopyFromUtf8("tendermint=rocks") });
            var query = await client.QueryAsync(new RequestQuery { Data = ByteString.CopyFromUtf8("tendermint") });

            Console.WriteLine($"Echo Status: {echo}");
            Console.WriteLine($"Info Status: {info.Data}");
            Console.WriteLine($"InitChain Status: {(initChain != null ? "Success" : "Failure")}");
            Console.WriteLine($"Begin Block Status: {(beginBlock != null ? "Success" : "Failure")}");
            Console.WriteLine($"Delivery Status: {deliver.Code switch { 0 => "Success", 1 => "Invalid Data", 2 => "Already Exists with the Same Value", 3 => "Already Exists with Different Value", _ => "Failure" }}");
            Console.WriteLine($"Commit Status: {(commit.Data.ToByteArray().SequenceEqual(new byte[8]) ? "Success" : "Failure")}");
            Console.WriteLine($"CheckTx Status: {checkTx.Code switch { 0 => "Success", 1 => "Invalid Data", 2 => "Already Exists with the Same Value", 3 => "Already Exists with Different Value", _ => "Failure" }}");
            Console.WriteLine($"Query Status: {query.Log} {query.Key.ToStringUtf8()}{(query.Key == ByteString.Empty || query.Value == ByteString.Empty ? "" : "=")}{query.Value.ToStringUtf8()}");

            System.Console.ReadKey();
        }
    }
}
