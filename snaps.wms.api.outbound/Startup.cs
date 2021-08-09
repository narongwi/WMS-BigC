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
namespace snaps.wms.api.outbound
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.AddSingleton<AppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);
 
            services.AddSingleton<ouborderService>();
            services.AddSingleton<ouprepService>();
            services.AddSingleton<ourouteService>();
            services.AddSingleton<ouhanderlingunitService>();


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
            services.AddScoped<IouborderService, ouborderService>();
            services.AddScoped<IouprepService, ouprepService>();
            services.AddScoped<IourouteService, ourouteService>();
            services.AddScoped<IouhanderlingunitService, ouhanderlingunitService>();
            services.AddScoped<IoutboundparameterService, ouboundparameterService>();


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            //Confug CORS
            app.UseCors("wmsCors");

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication(); //JWT
            app.UseAuthorization();  

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            logger.LogInformation("Snaps service api of Outbound module started " + DateTime.Now + " url: " + Configuration["AppSettings:ApiUrl"]);
        }
    }
}
