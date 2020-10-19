using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OKexTime.Context;
using OKexTime.Services;
using Serilog;

namespace OKexTime
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(
            //        builder =>
            //        {
            //            builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            //        });
            //});
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("172.18.0.1"));

            });
            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing())
                .ConfigureApiBehaviorOptions(options => options.SuppressMapClientErrors = true);
            services.AddDbContext<OkexContext>(p => p.UseNpgsql(Configuration.GetValue<string>("PsqlAuth")));
            services.AddHostedService<GetAccountHistory>(container =>
            {
                var loggerFactory = container.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<GetAccountHistory>();


                var optionsBuilder = new DbContextOptionsBuilder<OkexContext>();
                optionsBuilder.UseNpgsql(Configuration.GetValue<string>("PsqlAuth"));
                return new GetAccountHistory(logger, Configuration,
                    new OkexContext(optionsBuilder.Options));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }
            app.UseSerilogRequestLogging();
            //app.UseCors();
            //app.UseMiddleware<DecryptRequest>();
            app.UseMiddleware<ForbidMethods>();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
