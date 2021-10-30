using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace School.Member.Api
{
    public class Startup
    {
        private readonly DatabaseConfig dbConfig = new ();
        private readonly KeycloakConfig keycloakConfig = new ();


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.Bind("Database", dbConfig);
            Configuration.Bind("Keycloak", keycloakConfig);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {                        
            services.AddControllers()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    opt.JsonSerializerOptions.IgnoreNullValues = true;
                    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            services.AddLogging();
            services.AddDbContext<MemberDbContext>(opt => {
                opt.UseNpgsql(dbConfig.ConnectionString)
                    .EnableSensitiveDataLogging(true);
            });
            
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = keycloakConfig.Authority;
                    options.Audience = keycloakConfig.Audience;
                    options.RequireHttpsMetadata = false;
                });

            services.AddAutoMapper(cfg => 
            {
                cfg.AddProfile(new PupilProfile());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
            });
            services.AddScoped<IPupilService, PupilService>();
            services.AddSingleton<KeycloakConfig>(keycloakConfig);
            services.AddHttpClient<IKeycloakService, KeycloakService>(client =>
            {
                client.BaseAddress = new Uri(keycloakConfig.BaseUrl);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
