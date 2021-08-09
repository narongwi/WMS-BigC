using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Snaps.Helpers;
using Snaps.WMS.Services;
using Snaps.WMS.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace snaps.wms.api.externalsource
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.Configure<FormOptions>(o => { 
            o.ValueLengthLimit = int.MaxValue;
            o.MultipartBodyLengthLimit = int.MaxValue;
            o.MemoryBufferThreshold = int.MaxValue;
            });
            services.AddSingleton<AppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);
            services.AddSingleton<exsBarcodeService>();
            services.AddSingleton<exsTHPartyService>();
            services.AddSingleton<exsInboundService>();
            services.AddSingleton<exsOutboundService>();
            services.AddSingleton<exsProductService>();
            services.AddSingleton<exsPrepService>();
            services.AddSingleton<exsLocupService>();
            services.AddSingleton<exsLocdwService>();
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            // configure jwt authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"]
                };
                x.Audience = Configuration["JWT:Key"];
                x.Authority = "http://localhost:4100/";
                x.Configuration = new OpenIdConnectConfiguration();
            });

            // configure DI for application services
            services.AddScoped<IexsBarcodeService, exsBarcodeService>();
            services.AddScoped<IexsTHPartyService, exsTHPartyService>();
            services.AddScoped<IexsInboundService, exsInboundService>();
            services.AddScoped<IexsOutboundService, exsOutboundService>();
            services.AddScoped<IexsProductService, exsProductService>();
            services.AddScoped<IexsPrepService, exsPrepService>();
            services.AddScoped<IexsLocupService, exsLocupService>();
            services.AddScoped<IexsLocdwService, exsLocdwService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            //Confug CORS
            app.UseCors(
                options => 
                options.WithOrigins("http://localhost:4200","http://192.168.2.35:4200","http://bgcdwmsasn-ap01:4200")
                .AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
            );
            app.UseRouting();
            app.UseAuthentication(); //JWT
            app.UseAuthorization();  
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            logger.LogInformation("Snaps service api of external source module started " + DateTime.Now + " url: " + Configuration["AppSettings:ApiUrl"]);
        }
    }
}
