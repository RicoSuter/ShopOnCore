using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NSwag.AspNetCore;
using ShopOnCore.Common.Messaging.Rabbit;
using ShopOnCore.Orders.Contract;
using ShopOnCore.Orders.Services;
using ShopOnCore.Common.Messaging.Extensions;

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
            services.AddSwagger();

            // Register the orders service which handles the data access 
            // We use TryAddScoped so that we can replace the service in tests
            services.TryAddScoped<IOrdersService, OrdersService>();

            // Register the message sender to send order request to a queue (injected into the orders controller)
            // AddMessageSender is a custom method implemented in ShopOnCore.Common
            services.AddMessageSender(provider => new RabbitMessageSender<CreateOrderMessage>(
                new RabbitConfiguration { Host = "rabbit_queue" }));
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

            // Register Swagger UI provided by http://nswag.org
            app.UseSwaggerUi3WithApiExplorer();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
