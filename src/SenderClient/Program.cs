using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.Context;
using MassTransitDemo.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransitDemo.SenderClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", false, true);
            var config = builder.Build();

            var services = new ServiceCollection();
            services.AddMassTransit(s =>
            {
                s.AddBus(provider => Bus.Factory.CreateUsingAzureServiceBus(cfg =>
                {
                    cfg.ConfigureEndpoints(provider);
                    cfg.Host(config.GetValue<string>(
                        "MassTransitDemo:MassTransit:ConnectionString"));

                    EndpointConvention.Map<CreateDemoEntity>(
                        new Uri($"queue:{config.GetValue<string>("MassTransitDemo:MassTransit:SendQueueName")}"));
                }));
            });

            var serviceProvider = services.BuildServiceProvider();
            var bus = serviceProvider.GetService<IBus>();

            await bus.Send(new CreateDemoEntity { Name = "NewDemoEntity" });

            Console.WriteLine("Command CreateDemoEntity Send");
        }
    }
}
