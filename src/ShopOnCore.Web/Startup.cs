using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ShopOnCore.Common.Messaging;
using ShopOnCore.Common.Messaging.ServiceBus;
using ShopOnCore.Orders.Contract;
using ShopOnCore.Orders.Services;

namespace ShopOnCore.Web
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.TryAddScoped<IOrdersService, OrdersService>();
            services.AddMessageSender(provider =>
            {
                return new ServiceBusMessageSender<CreateOrderMessage>(
                    Configuration.GetSection("ServiceBus")["ConnectionString"],
                    Configuration.GetSection("ServiceBus")["EntityPath"]);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
