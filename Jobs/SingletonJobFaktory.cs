using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace Basket.Jobs
{
    public class SingletonJobFaktory : IJobFactory
    {
        #region Ctor with Service Provider
        private readonly IServiceProvider _serviceProvider;

        public SingletonJobFaktory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        #endregion

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            throw new NotImplementedException();
        }
    }
}
