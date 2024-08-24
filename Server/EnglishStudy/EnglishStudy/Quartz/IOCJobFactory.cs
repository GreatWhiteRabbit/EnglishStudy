using Quartz;
using Quartz.Spi;

namespace EnglishStudy.Quartz {
    public class IOCJobFactory : IJobFactory{

        private readonly IServiceProvider serviceProvider;

        public IOCJobFactory(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) {
            return serviceProvider.CreateScope().ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            // return serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job) {
           var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
