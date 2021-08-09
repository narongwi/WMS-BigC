using System;
using Serilog;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace snaps.wms.api.administrator
{
    public class Program
    {
        public static void Main(string[] args) { CreateHostBuilder(args).Build().Run(); }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, builder) => {builder.AddFile("Logs/Snaps.wms.administrator-{Date}.logx"); })
            .ConfigureWebHostDefaults(webBuilder => { 
                webBuilder.UseStartup<Startup>(); 
            })
            .UseSerilog((hostingContext, loggerConfig) => loggerConfig.ReadFrom .Configuration(hostingContext.Configuration) );
    }
}
