using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransitDemo.Contracts;

namespace MassTransitDemo.Consumer
{
    public class CreateDemoEntityConsumer : IConsumer<CreateDemoEntity>
    {
        public Task Consume(ConsumeContext<CreateDemoEntity> context)
        {
            Console.WriteLine($"CreateDemoEntity command received: {context.Message.Name}");

            //here comes the Create DemoEntity implementation....

            //throw new Exception("failed");
            //throw new CreateDemoEntityException("failed");

            //return Task.CompletedTask;

            Console.WriteLine("Sending Event DemoEntityCreated");
            return context.Publish(
                new DemoEntityCreated
                {
                    Name = context.Message.Name,
                    Created = DateTimeOffset.Now
                });
        }
    }
}