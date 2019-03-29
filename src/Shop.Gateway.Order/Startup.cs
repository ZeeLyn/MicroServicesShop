using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Uragano.Consul;
using Uragano.Core;
using Uragano.Logging.Exceptionless;

namespace Shop.Gateway.Order
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ctx =>
                {
                    var firstError = ctx.ModelState.Keys.SelectMany(k => ctx.ModelState[k].Errors).Select(e => e.ErrorMessage).LastOrDefault();
                    return new BadRequestObjectResult(firstError);
                };
            });
            services.AddUragano(Configuration, builder =>
            {
                builder.AddClient();
                builder.AddConsul();
                builder.AddExceptionlessLogger();
                //if (HostingEnvironment.IsProduction())
                //builder.AddCircuitBreaker();
            });
            services.AddJwtBearerAuthorize();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
