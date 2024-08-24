using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Quartz {
    [ApiController]
    [Route("quartz")]
    public class QuartzController {
        private QuartzFactory quartzFactory;

        public QuartzController(QuartzFactory quartzFactory) {
            this.quartzFactory = quartzFactory;
        }

        [HttpPost("start")]
        public async Task<bool> StartQuartz() {
            // 开启定时任务
            try {
                await quartzFactory.Start();
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        [HttpPost("stop")]
        public bool StopQuartz() {
            // 关闭定时任务
            try {
                quartzFactory.Stop();
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
