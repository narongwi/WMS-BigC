using System;
using Serilog;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace snaps.wms.api.externalsource
{
    public class Program
    {
        public static void Main(string[] args) {  CreateHostBuilder(args).Build().Run(); }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, builder) => { builder.AddFile("Logs/Snaps.wms.externalsource-{Date}.logx"); })
            .ConfigureWebHostDefaults(webBuilder => { 
                webBuilder.UseStartup<Startup>() 
                .UseUrls("http://192.168.2.35:4920"); })
            .UseSerilog((hostingContext, loggerConfig) => loggerConfig.ReadFrom .Configuration(hostingContext.Configuration) );
    }
}
