using EnglishStudy.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.SS.Formula.Functions;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace EnglishStudy.Quartz {

    /// <summary>
    /// 任务调度类
    /// </summary>
    public class QuartzFactory {

    

        private RedisHelper redisHelper = new RedisHelper();

        // 默认每天12点执行任务
        private int Hour = 12;
        private int Minute = 0;

        private ISchedulerFactory schedulerFactory;
        private IScheduler scheduler;
        private IJobFactory jobFactory;

        public QuartzFactory(ISchedulerFactory schedulerFactory,IJobFactory jobFactory) {
            this.schedulerFactory = schedulerFactory;
            this.jobFactory = jobFactory;
        }

        public async Task<string> Start() {
            scheduler = await schedulerFactory.GetScheduler();
            scheduler.JobFactory = jobFactory;

            await scheduler.Start();

            // 获取时间
            try {
                string time = redisHelper.getStringObject<string>(MyConstant.WorkTime);
                string[] times = time.Split(":");
                Hour = int.Parse(times[0]);
                Minute = int.Parse(times[1]);
            }
            catch (Exception ex) {
                Console.WriteLine("获取时间失败");
            }
                // 创建触发器
                var trigger = TriggerBuilder.Create()
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(Hour, Minute))
                    .Build();

                // 创建任务
                var pullMessageJobDetail = JobBuilder.Create<PullMessageJob>()
                    .WithIdentity("pullMessageJob", "pullMessage")
                    .Build();

                await scheduler.ScheduleJob(pullMessageJobDetail, trigger);
                Console.WriteLine("定时任务执行完成");
                return await Task.FromResult("定时任务执行成功");
        }

        public void Stop() {
            if (scheduler == null) {
                return;
            }
            if (scheduler.Shutdown(waitForJobsToComplete: true).Wait(30000))
                scheduler = null;
            Console.WriteLine("定时任务停止");

        }
    }
}
