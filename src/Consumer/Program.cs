using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using GreenPipes;
using Microsoft.Extensions.Configuration;

namespace MassTransitDemo.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {

                    var builder = new ConfigurationBuilder()
                        .AddJsonFile($"appsettings.json", false, true);
                    var config = builder.Build();

                    services.AddMassTransit(s =>
                    {
                        s.AddConsumer<CreateDemoEntityConsumer>();
                        s.AddConsumer<CreateDemoEntityFaultConsumer>();

                        s.AddBus(provider => Bus.Factory.CreateUsingAzureServiceBus(cfg =>
                        {
                            cfg.Host(config.GetValue<string>("MassTransitDemo:MassTransit:ConnectionString"));

                            cfg.ReceiveEndpoint(config.GetValue<string>(
                                "MassTransitDemo:MassTransit:ReceiveQueueName"), e =>
                            {
                                e.ConfigureConsumer<CreateDemoEntityConsumer>(provider, e =>
                                {
                                    e.UseMessageRetry(r =>
                                    {
                                        r.Incremental(3, TimeSpan.FromSeconds(1),
                                            TimeSpan.FromSeconds(3));
                                        r.Ignore<CreateDemoEntityException>();
                                    });
                                });

                                e.ConfigureConsumer<CreateDemoEntityFaultConsumer>(provider);
                            });
                        }));
                    });

                    services.AddHostedService<Worker>();
                });
    }
}
