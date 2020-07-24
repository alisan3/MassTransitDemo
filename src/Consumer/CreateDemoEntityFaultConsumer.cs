using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransitDemo.Contracts;
using Newtonsoft.Json;

namespace MassTransitDemo.Consumer
{
    public class CreateDemoEntityFaultConsumer : IConsumer<Fault<CreateDemoEntity>>
    {
        public Task Consume(ConsumeContext<Fault<CreateDemoEntity>> context)
        {
            Console.WriteLine($"CreateDemoEntity Fault received: {context.Message.Message.Name}");

            //do fault handling here

            return context.Publish(
                new DemoEntityCreationFailed
                {
                    Name = context.Message.Message.Name,
                    ErrorMsg = JsonConvert.SerializeObject(context.Message.Exceptions)
                });
        }
    }
}