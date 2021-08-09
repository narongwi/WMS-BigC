using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
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

namespace snaps.wms.api.administrator
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
            services.AddSingleton<AppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);

            services.AddSingleton<accountService>();
            services.AddSingleton<roleService>();
            services.AddSingleton<policyService>();
            services.AddSingleton<configService>();

            services.AddSingleton<SystemBinaryService>();
            services.AddSingleton<mapstorageService>();
            services.AddSingleton<admdeviceService>(); 
            services.AddSingleton<admbarcodeService>();
            services.AddSingleton<admthpartyService>();
            services.AddSingleton<admproductService>();
            services.AddSingleton<admwarehouseService>();
            services.AddSingleton<admdepotService>();
            services.AddSingleton<admparameterService>();

            services.AddSingleton<zoneprepService>();
            services.AddSingleton<LOVService>();
            services.AddSingleton<binaryService>();
            services.AddSingleton<shareprepService>();

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            //Confug CORS
            string[] allowOrigins =
                Configuration.GetSection("AllowedOrigins").Get<string[]>();

            services.AddCors(options => options.AddPolicy(name: "wmsCors",
                builder => {
                    builder.WithOrigins(allowOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(2520));
                }));

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
                x.Authority = Configuration["AllowAuthority"];
                x.Configuration = new OpenIdConnectConfiguration();
            });

            // configure DI for application services
            services.AddScoped<IaccountService, accountService>();
            services.AddScoped<IroleService, roleService>();
            services.AddScoped<IroleService, roleService>();
            services.AddScoped<IpolicyService, policyService>();
            services.AddScoped<IconfigService, configService>();

            services.AddScoped<ISystemBinaryService, SystemBinaryService>();
            services.AddScoped<ImapstorageService, mapstorageService>();

            services.AddScoped<IadmdeviceService, admdeviceService>();
            services.AddScoped<IadmbarcodeService, admbarcodeService>();
            services.AddScoped<IadmthpartyService, admthpartyService>();
            services.AddScoped<IadmproductService, admproductService>();
            services.AddScoped<IadmwarehouseService, admwarehouseService>();
            services.AddScoped<IadmdepotService, admdepotService>();
            services.AddScoped<IadmparameterService, admparameterService>();

            services.AddScoped<IzoneprepService, zoneprepService>();
            services.AddScoped<ILOVService, LOVService>();
            services.AddScoped<IbinaryService, binaryService>();
            services.AddScoped<IshareprepService, shareprepService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            // CORS
            app.UseCors("wmsCors");

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication(); //JWT
            app.UseAuthorization();  

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            logger.LogInformation("Snaps service api of administrator module started " + DateTime.Now + " url: " + Configuration["AppSettings:ApiUrl"]);
        }
    }
}
