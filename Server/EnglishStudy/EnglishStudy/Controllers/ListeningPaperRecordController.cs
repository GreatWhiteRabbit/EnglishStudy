using EnglishStudy.DTO;
using EnglishStudy.Entity.ChildEntity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers {
    [ApiController]
    [Route("paperRecord")]
    public class ListeningPaperRecordController {
        private IListeningPaperRecordService listeningPaperRecordService;

        private Result result = new Result();

        public ListeningPaperRecordController(IListeningPaperRecordService listeningPaperRecordService) {
            this.listeningPaperRecordService = listeningPaperRecordService;
        }

        /// <summary>
        /// 添加做题记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="ListeningPaperId">听力试题id</param>
        /// <param name="Accuracy">准确率</param>
        /// <param name="List">答题情况</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("add/{UserId}/{ListeningPaperId}/{accuracy}")]
        public Result AddListeningPaperRecord(int UserId, int ListeningPaperId, double Accuracy, [FromBody]AnswerDetailList List) {
           int id = listeningPaperRecordService.AddListeningPaperRecord(UserId, ListeningPaperId, Accuracy, List.List);
            if (id == 0) return result.failed(StatusCode.ServerError, "出错啦");
            return result.Ok(id);
        }

        /// <summary>
        /// 根据用户id和recordid获取数据
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="RecordId">记录id</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("{UserId}/{RecordId}")]
        public Result GetByRecordId(int UserId, int RecordId) {
           var dto = listeningPaperRecordService.GetByRecordId(UserId, RecordId);
           if(dto == null) {
                return result.failed(StatusCode.NotFound, "没有该记录");
            }
            return result.Ok(dto);
        }


        /// <summary>
        /// 根据page和size获取分页数据
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("page/{UserId}/{page}/{size}")]
        public Result GetByPageSize(int UserId, int page = 1, int size = 10) {
            var dtoPage = listeningPaperRecordService.GetByPageSize(UserId, page, size);
            return result.Ok(dtoPage);
        }

        /// <summary>
        /// 根据id批量删除数据
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="list">记录id集合</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("delete/{UserId}")]
        public Result DeleteBatch(int UserId, [FromBody] IdList list) {
            int count = listeningPaperRecordService.DeleteBatch(UserId, list.list);
            return result.Ok(count);
        }
        /// <summary>
        /// 根据用户id和听力试题id获取做题记录
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PaperId"></param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)]
        [HttpGet("paper/{UserId}/{PaperId}")]
        public Result GetByUserIdAndPaperId(int UserId, int PaperId) {
            var record = listeningPaperRecordService.getByUserIdAndPaperId(UserId, PaperId); 
            if(record == null) {
                return result.failed(StatusCode.NotFound, "没有做题记录");
            }
            return result.Ok(record);
        }

        /// <summary>
        /// 获取近size的准确率
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("accuracylist/{UserId}/{size}")]
        public Result GetAccuracyList(int UserId, int size = 10) {
            return result.Ok(listeningPaperRecordService.GetAccuracyList(UserId, size));
        }
    }
}
