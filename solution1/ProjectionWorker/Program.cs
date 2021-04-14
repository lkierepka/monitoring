using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProjectionWorker.Order;

namespace ProjectionWorker
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
                    services.AddMassTransit(p =>
                    {
                        p.AddConsumer<OrderConsumer>();
                        p.UsingRabbitMq((context, configurator) =>
                        {
                            configurator.Host("rabbit", hostConfigurator =>
                            {
                                hostConfigurator.Username("guest");
                                hostConfigurator.Password("guest");
                            });
                            configurator.ConfigureEndpoints(context);
                        });
                    });
                    services.AddMassTransitHostedService();
                    services.AddOpenTelemetryTracing(builder =>
                        builder
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("workerservice"))
                            .AddOtlpExporter(options => options.Endpoint = new Uri("http://collector:4317"))
                            .AddMassTransitInstrumentation());
                });
    }
}