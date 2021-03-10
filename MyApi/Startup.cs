using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MyApi
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
            services.AddControllers();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        //IdentityServer��ַ
                        options.Authority = "https://localhost:5001";
                        //��ӦApiResource��Name
                        options.Audience = "api";
                        //��ʹ��https
                        options.RequireHttpsMetadata = false;
                        IdentityModelEventSource.ShowPII = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            //�Ƿ���Կ��֤
                            ValidateIssuerSigningKey = true,
                            //�Ƿ���֤������
                            ValidateIssuer = true,
                            //�Ƿ���֤����
                            ValidateAudience = true,

                            //�Ƿ���֤����ʱ��
                            RequireExpirationTime = true,
                            ValidateLifetime = true
                        };
                    });
            services.AddAuthorization(options =>
            {
                //���ڲ�����Ȩ
                options.AddPolicy("WeatherPolicy", builder =>
                {
                    //�ͻ���Scope�а���api1.weather.scope���ܷ���
                    builder.RequireScope("api.weather.scope");
                });
                //���ڲ�����Ȩ
                options.AddPolicy("TestPolicy", builder =>
                {
                    //�ͻ���Scope�а���api1.test.scope���ܷ���
                    builder.RequireScope("api.test.scope");
                });
            });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("default");

            //�����֤
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
