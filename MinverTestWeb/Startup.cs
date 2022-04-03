using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MinverTestLib;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MinverTestWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var apiKey = Configuration.GetValue<string>("ApiKey") ?? "1234567890";
            services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                .AddApiKeyInHeader(options =>
                {
                    options.Realm = "Minver API";
                    options.KeyName = "X-API-KEY";
                    options.Events = new ApiKeyEvents
                    {
                        OnValidateKey = (context) =>
                        {
                            if (string.Compare(context.ApiKey, apiKey, true) != 0)
                                context.NoResult();
                            else
                            {
                                var claims = new[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier, "0", ClaimValueTypes.Integer32, context.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name, "Api Key User", ClaimValueTypes.String, context.Options.ClaimsIssuer)
                                };
                                context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                                context.Success();
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MinverTestWeb", Version = "v1" });
            });
            services.AddScoped<WeatherForecastService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MinverTestWeb v1"));
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization();
            });
        }
    }
}
