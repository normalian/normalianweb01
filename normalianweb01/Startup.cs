using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace normalianweb01
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
            services.AddRazorPages();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            app.Use(async (context, next) =>
            {

                // Create a user on current thread from provided header
                if (context.Request.Headers.ContainsKey("X-MS-CLIENT-PRINCIPAL-ID"))
                {
                    // Read headers from Azure
                    var azureAppServicePrincipalIdHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"][0];
                    var azureAppServicePrincipalNameHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"][0];

                    // Create claims id
                    var claims = new Claim[] {
                        new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", azureAppServicePrincipalIdHeader),
                        new System.Security.Claims.Claim("name", azureAppServicePrincipalNameHeader)
                    };

                    // Set user in current context as claims principal
                    var identity = new GenericIdentity(azureAppServicePrincipalIdHeader);
                    identity.AddClaims(claims);

                    // Set current thread user to identity
                    context.User = new GenericPrincipal(identity, null);
                };

                await next.Invoke();
            });
        }
    }
}
