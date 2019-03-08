using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.JwtAuthorize;
using Uragano.Consul;
using Uragano.Core;

namespace Shop.Gateway.Identity
{
    public class Startup
    {
        private IHostingEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment evn)
        {
            Configuration = configuration;
            HostingEnvironment = evn;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiJwtAuthorize((context) => true);
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidateModelStateFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddUragano(Configuration, builder =>
            {
                builder.AddClient();
                builder.AddConsul();
                //if (HostingEnvironment.IsProduction())
                //builder.AddCircuitBreaker();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
