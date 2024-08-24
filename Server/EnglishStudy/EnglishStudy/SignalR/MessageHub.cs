using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EnglishStudy.SignalR {

    // 推送消息的hub
    public class MessageHub : Hub{

        private IMessageService messageService;

        public MessageHub(IMessageService messageService) {
            this.messageService = messageService;
        }

        [Authorize(MyConstant.UserOrAdmin)]
        public async Task SendMessage(int userId) {
            Console.WriteLine("log");
            // 获取未读消息条数
            int count = messageService.UserGetUnreadMessageCount(userId);
            
            await Clients.All.SendAsync("sys_msg_count", count);   
        }
    }
}
