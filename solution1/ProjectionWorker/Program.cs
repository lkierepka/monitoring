using System;
using Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProjectionWorker.Config;
using ProjectionWorker.Order;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using IIdGenerator = Common.IIdGenerator;

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
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("projection"))
                            .AddOtlpExporter(options => options.Endpoint = new Uri("http://collector:4317"))
                            .AddMongoDBInstrumentation()
                            .AddMassTransitInstrumentation()
                            .AddConsoleExporter());
                    services.Configure<MongoConfiguration>(hostContext.Configuration.GetSection("Mongo"));
                    services.AddSingleton<IMongoConfiguration>(provider =>
                        provider.GetRequiredService<IOptions<MongoConfiguration>>().Value);
                    services.AddSingleton<IOrderRepository, OrderRepository>();
                    services.AddSingleton<IIdGenerator, SequentialIdGenerator>();
                    services.AddSingleton<IMongoClient>(provider =>
                    {
                        var mongoUrl = provider.GetRequiredService<IOptions<MongoConfiguration>>().Value
                            .ConnectionString;
                        var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(mongoUrl));
                        clientSettings.ClusterConfigurator =
                            cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
                        return new MongoClient(clientSettings);
                    });
                    BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
                })
                .UseSerilog((context, configuration) =>
                    configuration
                        .MinimumLevel.Information()
                        .WriteTo.Elasticsearch(
                            new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200"))
                            {
                                AutoRegisterTemplate = true,
                                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                                FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate)
                            })
                        .WriteTo.Console());
    }
}