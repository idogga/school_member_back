using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using System.IO;
using Serilog;
using Serilog.Context;
using Microsoft.Extensions.Logging;

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
                    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
                cfg.AddProfile(new TeachersProfile());
            });
            AddSwaggerGen(services);
            services.AddScoped<IPupilService, PupilService>();
            services.AddScoped<ITeachersService, TeachersService>();
            services.AddSingleton<KeycloakConfig>(keycloakConfig);
            services.AddHttpClient<IKeycloakService, KeycloakService>(client =>
            {
                client.BaseAddress = new Uri(keycloakConfig.BaseUrl);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            loggerFactory.AddSerilog();
            
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "People");
            });

            app.UseCors(options =>
            {
                options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });

            app.UseForwardedHeaders();

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseSerilogRequestLogging();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void AddSwaggerGen(IServiceCollection services)
        {
            var oidcUrl = $"{keycloakConfig.BaseUrl}auth/realms/{keycloakConfig.Realm}/protocol/openid-connect/";

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "People", Version = "v1" });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        ClientCredentials = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri($"{oidcUrl}auth"),
                            TokenUrl = new Uri($"{oidcUrl}token"),
                        },
                    },
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // https://github.com/domaindrivendev/Swashbuckle.AspNetCore#systemtextjson-stj-vs-newtonsoft
            // services.AddSwaggerGenNewtonsoftSupport();
        }

    }
}
