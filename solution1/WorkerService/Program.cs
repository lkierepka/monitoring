using System;
using Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using WorkerService.Order;

namespace WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<WorkerDbContext>();
                db.Database.Migrate();
            }

            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<PostgresConfiguration>(context.Configuration.GetSection("Postgres"));
                    services.AddDbContext<WorkerDbContext>((provider, builder) =>
                        builder.UseNpgsql(
                            provider.GetRequiredService<IOptions<PostgresConfiguration>>().Value.ConnectionString));
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
                    services.AddMassTransitHostedService();
                    services.AddOpenTelemetryTracing(builder =>
                        builder
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("workerservice"))
                            .AddOtlpExporter(options => options.Endpoint = new Uri("http://collector:4317"))
                            .AddMassTransitInstrumentation()
                            .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
                            .AddConsoleExporter()
                    );
                    services.AddSingleton<IIdGenerator, SequentialIdGenerator>();
                })
                .UseSerilog((context, configuration) =>
                    configuration
                        .MinimumLevel.Information()
                        .WriteTo.Elasticsearch(
                            new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200"))
                            {
                                AutoRegisterTemplate = true,
                                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                                FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                                EmitEventFailure = EmitEventFailureHandling.RaiseCallback,
                            })
                        .WriteTo.Console());
    }
}