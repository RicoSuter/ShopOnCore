using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ShopOnCore.Web;
using Xunit.Sdk;

namespace ShopOnCore.Integration.Tests
{
    public abstract class IntegrationTestsBase
    {
        private TestServer _server;

        public void InitializeTestServer()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(ConfigureTestServerServices));

            HttpClient = _server.CreateClient();
        }

        public void InitializeRemoteServer()
        {
            HttpClient = new HttpClient { BaseAddress = new Uri("http://localhost:51914/") };
        }

        public HttpClient HttpClient { get; private set; }

        protected abstract void ConfigureTestServerServices(IServiceCollection serviceCollection);

        protected async Task<T> AwaitAsync<T>(Func<Task<T>> action, Predicate<T> check, TimeSpan timeout)
        {
            var now = DateTime.UtcNow;
            while (true)
            {
                var result = await action();
                if (check(result))
                {
                    return result;
                }

                if (DateTime.UtcNow - now > timeout)
                {
                    throw new XunitException("Criterium could not be met before timing out.");
                }
            }
        }
    }
}