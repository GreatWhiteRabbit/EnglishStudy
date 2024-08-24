using EnglishStudy.DTO.MyMessage;
using EnglishStudy.Entity;
using EnglishStudy.Utils;
using Microsoft.EntityFrameworkCore;

namespace EnglishStudy.Service.ServiceImpl {
    public class MessageServiceImpl : IMessageService {

        public MyDbContext dbContext;

        public MessageServiceImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public List<Message> AdminGetMessageList(int page, int size) {
            // TODO 还未测试
            // 管理员获取所有的系统消息
            var result = dbContext.Messages
                .OrderByDescending(item => item.Time)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
            return result;
        }

        public async Task<bool> DeleteMessage(int messageId) {
            // TODO 还未测试
            // 删除系统消息的时候，同时删除消息接收表中的相对应的数据

            // 从数据库中查找数据
            var result = dbContext.Messages
                .Where(item => item.MessageId == messageId)
                .SingleOrDefault();
            if (result == null) {
                return false;
            }
            // 从receive表中查找与messageId相关的数据
            var receiveList = await dbContext.Receives
                .Where(item => item.MessageId == messageId)
                .ToListAsync();
            // 删除数据
            dbContext.Messages.Remove(result);
            dbContext.Receives.RemoveRange(receiveList);
            int count = dbContext.SaveChanges();
            // 如果事务没有执行成功，返回false
            if (count == 0) {
                return false;
            }
            return true;

         
        }

        public bool PublishMessage(string title, string content) {
            // TODO 还没有测试
            // 管理员发布系统消息
            Message message = new Message();
            message.Title = title;
            message.content = content;
            message.Time = DateTime.Now;
            dbContext.Messages.Add(message);
            int count = dbContext.SaveChanges();
            if (count != 0) {
                return true;
            }
            return false;
        }

        public async Task<bool> PullMessage() {
            // TODO 还没有经过测试
            /* 从消息表中拉取消息到接收表中，可以管理员手动拉取任务和
             * 定时从消息表中拉取数据到接收表中，定时的时间根据情况而定
             * 可以使用backgroundService来设置定时任务，具体选用什么作为
             * 定时任务的框架，到时候而定，快过年了，暂时不想写这些玩意儿
            */
            // 从user表中获取所有的用户的id
            List<int> userIdList = new List<int>();
                userIdList =  dbContext.Users
               .Select(item => item.UserId)
               .ToList();
       
            // 从消息表中获取所有的消息，为什么要获取所有的消息呢，因为有些
            // 用户是新注册的，所以获取不到注册之前的消息，因此需要获取所有的消息
            List<int> messageIdList = new List<int>();
            
                messageIdList =  dbContext.Messages
                .Select(item => item.MessageId)
                .ToList();
        
            // 等待userTask和messageTask执行完之后，执行后续的任务
         
            List<Receive> receiceList = new List<Receive>();
            // 从receive表中查找userId和messageId，如果不存在则插入
            for (int i = 0; i < userIdList.Count(); i++) {
                for (int j = 0; j < messageIdList.Count(); j++) {
                    int userId = userIdList[i];
                    int messageId = messageIdList[j];
                    var result = dbContext.Receives
                        .Where(item => item.UserId == userId && item.MessageId == messageId)
                        .SingleOrDefault();
                    // 不存在，从数据库中插入数据
                    if (result == null) {
                        Receive receive = new Receive();
                        receive.MessageId = messageId;
                        receive.UserId = userId;
                        receive.Read = 0;
                        receive.Time = DateTime.Now;
                        receiceList.Add(receive);
                    }
                }
            }
            // 将list插入到数据库中
            await dbContext.Receives.AddRangeAsync(receiceList);
            int count = dbContext.SaveChanges();
            if (count != 0) {
                return true;
            }
            return false;
        }

        public bool ReadMessage(int UserId, int ReceiveId) {
        
            /*从数据库中拉取了数据之后用户不一定会马上读取数据，所以
             * 如果用户自己读取了消息，那么就将已读标记设置为已读
             * 
            */
            var result = dbContext.Receives
               .Where(item => item.UserId == UserId && item.ReceiveId == ReceiveId)
               .SingleOrDefault();
            if (result == null) { return false; }
            // 设置为已读，同时设置已读时间
            result.Read = 1;
            result.Time = DateTime.Now;
            int count = dbContext.SaveChanges();
            if(count == 0) {
                return false;
            }
            return true;
        }

        public List<MessageDTO> UserGetMessage(int userId, int page, int size) {
            // TODO 还未测试
            /* 用户从数据库中获取系统消息，这里的话应该要使用
             * websocket进行通信了，而不是使用普通的http请求，
             * 可以使用微软自己的SignalR进行通信，如果到时候实在
             * 写不出来SignalR的话，那么到时候再使用http请求也行
             */

            // 查询数据，先从receive表中查找数据，然后联合message表查找数据
            var result = (from newReceiveTable in (
             from receive in dbContext.Receives
             where receive.UserId == userId
             orderby receive.MessageId descending
             select new {
                 receive.UserId,
                 receive.MessageId,
                 receive.ReceiveId,
                 receive.Read
             })
             .Skip((page - 1) * size)
             .Take(size)
             join message in dbContext.Messages
             on newReceiveTable.MessageId equals message.MessageId
             select new {
                 newReceiveTable.MessageId,
                 newReceiveTable.UserId,
                 newReceiveTable.ReceiveId,
                 newReceiveTable.Read,
                 message.Title,
                 message.content,
                 message.Time
             }
             ).ToList();
            List<MessageDTO> messageDTOList = new List<MessageDTO>();
            for(int i = 0; i < result.Count(); i++) {
                MessageDTO messageDTO = new MessageDTO();
                messageDTO.Title = result[i].Title;
                messageDTO.Content = result[i].content;
                messageDTO.Time = result[i].Time;
                messageDTO.MessageId = result[i].MessageId;
                messageDTO.UserId = result[i].UserId;
                messageDTO.ReceiveId = result[i].ReceiveId;
                messageDTO.Read = result[i].Read;
                messageDTOList.Add(messageDTO);
            }
            return messageDTOList;
         
        }

        public int UserGetUnreadMessageCount(int userId) {
            // TODO 还未测试
            // 查找所有未读的消息
            var count = dbContext.Receives
                .Where(item => item.UserId == userId
                && item.Read == 0)
                .Count();
            return count;
           
        }

        public bool SetJobTime(int Hour, int Minute) {
            string time = Hour + ":" + Minute;
            RedisHelper redisHelper = new RedisHelper();
            bool success = redisHelper.setString(MyConstant.WorkTime, time);
            return success;
        }

        public string GetTime() {
            string defaultTime = "12:00";
            try {
                RedisHelper redisHelper = new RedisHelper();
                defaultTime =  redisHelper.getStringObject<string>(MyConstant.WorkTime);
                return defaultTime;
            } catch (Exception ex) {
                Console.WriteLine("获取定时任务时间失败");
                Console.WriteLine(ex.Message);
            }
            return defaultTime;
        }
    }
}
