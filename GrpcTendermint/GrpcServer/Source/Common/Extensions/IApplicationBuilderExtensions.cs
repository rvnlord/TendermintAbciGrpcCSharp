using System.Diagnostics;
using Microsoft.AspNetCore.Builder;

namespace GrpcServer.Source.Common.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void UseTendermint(this IApplicationBuilder app)
        {
            var tendermint = Process.Start(new ProcessStartInfo
            {
                FileName = @"Tendermint\tendermint.exe",
                Arguments = "init validator --home=Tendermint"
            });
            tendermint?.WaitForExit();

            Process.Start(new ProcessStartInfo
            {
                FileName = @"Tendermint\tendermint.exe",
                Arguments = @"node --abci grpc --home=Tendermint"
            });
        }
    }
}
