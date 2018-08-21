using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NCrontab;

namespace Inspector
{
    public class InspectorService :  IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;

        // Every one minute;
        private const string _scheduleExpression = "0 * * * * *";

        public InspectorService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            var parseOptions = new CrontabSchedule.ParseOptions() {IncludingSeconds = true};
            _schedule = CrontabSchedule.Parse(_scheduleExpression, parseOptions);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            do
            {
                var nowDateTime = DateTime.Now;
                if (nowDateTime > _nextRun)
                {
                    Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            } while (true);
        }

        private void Process()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var modelContext = scope.ServiceProvider.GetRequiredService<ModelContext>();
                var inspectingLink = modelContext.InspectionLink;
                if (!inspectingLink.Any())
                    return;

                var pageSource = GetPageSourceCode(inspectingLink.First().InspectionLink);
                var lastFixedState = modelContext.SourceStates.LastOrDefault();

                if (lastFixedState != null && lastFixedState.SourceCode == pageSource)
                    return;

                modelContext.SourceStates.Add(new SourceState(pageSource));
                modelContext.SaveChanges();
            }
        }

        private static string GetPageSourceCode(string inspectingLink)
        {
            var request = WebRequest.CreateHttp(inspectingLink);
            var response = request.GetResponse();
            string pageSource;
            using (var streamReader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                pageSource = streamReader.ReadToEnd();
            }

            return pageSource;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
