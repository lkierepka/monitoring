using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Common
{
    public class ElasticsearchEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (Activity.Current != null)
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ElasticApmTraceId", Activity.Current.TraceId));
        }
    }
}