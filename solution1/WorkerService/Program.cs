using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WorkerService.Order;

namespace WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddMassTransit(p =>
                    {
                        p.AddConsumer<OrderAggregate>();
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
                    AppContext.SetSwitch("MassTransit.EnableActivityPropagation",true);
                    services.AddMassTransitHostedService();
                    services.AddOpenTelemetryTracing(builder =>
                        builder
                            .AddSource("*")
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("workerservice"))
                            .AddOtlpExporter(options => options.Endpoint = new Uri("http://collector:4317"))
                            .AddConsoleExporter()
                    );
                    // services.AddHostedService<Worker>();
                });
    }
}