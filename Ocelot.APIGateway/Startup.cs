using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

namespace Ocelot.APIGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "Gateway API", Version = "v1", Description = "# gateway api..." });
            });

            services.AddControllers();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication("orderService", options =>
                {
                    options.Authority = "http://ids.kai:5001";//��Ȩ���ĵ�ַ
                    options.ApiName = "orderApi";
                    options.SupportedTokens = SupportedTokens.Both;
                    options.ApiSecret = "orderApi secret";
                    options.RequireHttpsMetadata = false;
                })
                .AddIdentityServerAuthentication("productService", options =>
                {
                    options.Authority = "http://ids.kai:5001";//��Ȩ���ĵ�ַ
                    options.ApiName = "productApi";
                    options.SupportedTokens = SupportedTokens.Both;
                    options.ApiSecret = "productApi secret";
                    options.RequireHttpsMetadata = false;
                });
            //���ocelot����
            services.AddOcelot()
                    //���consul֧��
                    .AddConsul()
                    //��ӻ���
                    .AddCacheManager(x =>
                    {
                        x.WithDictionaryHandle();
                    })
                    //���Polly
                    .AddPolly();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/order/swagger/v1/swagger.json", "Order API V1");
                c.SwaggerEndpoint("/product/swagger/v1/swagger.json", "Product API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            //����Ocelot�м��
            app.UseOcelot().Wait();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            IdentityModelEventSource.ShowPII = true;
        }
    }
}
