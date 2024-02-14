namespace Basket.Jobs
{
    public class JobScheduler
    {
        public Type _JobType { get; set; }
        public string _CronExperssion { get; set; }

        public JobScheduler(Type JobType, string CronExperssion)
        {
            _CronExperssion = CronExperssion;
            _JobType = JobType;
        }

        
    }
}
