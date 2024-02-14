using Quartz.Spi;
using Quartz;

namespace Basket.Jobs
{
    public class QuartzHostdService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<JobScheduler> _jobSchedulers;
        public QuartzHostdService(ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IEnumerable<JobScheduler> jobSchedulers)
        {
            _schedulerFactory = schedulerFactory;
            _jobSchedulers = jobSchedulers;
            _jobFactory = jobFactory;
        }
        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;

            foreach (var jobSchedule in _jobSchedulers)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
            }

            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
        }

        private static IJobDetail CreateJob(JobScheduler schedule)
        {
            var jobType = schedule._JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)
                .Build();
        }

        private static ITrigger CreateTrigger(JobScheduler schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule._JobType.FullName}.trigger")
                .WithCronSchedule(schedule._CronExperssion)
                .WithDescription(schedule._CronExperssion)
                .Build();
        }
    }
}
