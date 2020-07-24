using System;

namespace MassTransitDemo.Contracts
{
    public class DemoEntityCreated
    {
        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}
