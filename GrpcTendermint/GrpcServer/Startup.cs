using System;
using System.Diagnostics;
using GrpcServer.Source.Common.Extensions;
using GrpcServer.Source.Models;
using GrpcServer.Source.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GrpcServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddDbContextPool<KvDbContext>(o => o.UseSqlite(Configuration.GetConnectionString("DBCS")));
            services.AddKVStoreCache();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseEndpoints(e =>
            {
                e.MapGrpcService<KVStoreService>();
                e.MapGet("/", async context => await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client"));
            });
            
            app.UseTendermint();
        }
    }
}
