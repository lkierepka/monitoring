using System.Diagnostics;
using OpenTelemetry;

namespace WebApi
{
    public class Processor:BaseProcessor<Activity>
    {
        public override void OnStart(Activity data)
        {
            base.OnStart(data);
        }

        public override void OnEnd(Activity data)
        {
            base.OnEnd(data);
        }
    }
}