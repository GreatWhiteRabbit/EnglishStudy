using EnglishStudy.Service;
using Quartz;

namespace EnglishStudy.Quartz {

    /// <summary>
    /// 定时推送系统消息的Job
    /// </summary>
    public class PullMessageJob : IJob {

        private  IMessageService messageService;
       
        public PullMessageJob(IMessageService messageService) {
            this.messageService = messageService;
        }

        public async Task Execute(IJobExecutionContext context) {
            // 执行推送任务
            Console.WriteLine("开始执行推送任务");
           await messageService.PullMessage();
        }
    }
}
