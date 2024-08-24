using EnglishStudy.DTO;
using EnglishStudy.DTO.MyListening;
using EnglishStudy.Entity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers {
    [ApiController]
    [Route("paper")]
    public class ListeningPaperController {

        private IListeningPaperService listeningPaperService;

        private Result result = new Result();

        public ListeningPaperController(IListeningPaperService listeningPaperService) {
            this.listeningPaperService = listeningPaperService;
        }

        /// <summary>
        /// 添加听力试题
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="title">试题题目</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("add/{title}")]
        public async Task<Result> AddListeningPaper(List<IFormFile> files, string title) {
            int id = await listeningPaperService.AddListeningPaper(files, title);
            return result.Ok(id);
        }

        /// <summary>
        /// 根据id获取听力试题数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Result GetByListeningPaperId(int id) {
            var item = listeningPaperService.GetByListeningPaperId(id);
            if (item == null) return result.failed(StatusCode.NotFound, "数据丢失啦");
            return result.Ok(item);
        }

        /// <summary>
        /// 根据页码和页面大小获取数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        [HttpGet("list/{page}/{size}")]
        public Result GetByPageSize(int page, int size) {
            return result.Ok(listeningPaperService.GetByPageSize(page, size));
        }

        /// <summary>
        /// 根据id删除数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("delete/{id}")]
        public Result DeleteByListeningPaperId(int id) {
            var success = listeningPaperService.DeleteByListeningPaperId(id);
            return result.Ok(success);
        }


        /// <summary>
        /// 上传听力音频接口
        /// </summary>
        /// <param name="file">听力音频</param>
        /// <returns></returns>
        [HttpPost("upload")]
        [Authorize(MyConstant.Admin)]
        public async Task<Result> uploadAudio(IFormFile file) {
            string url = await listeningPaperService.uploadAudio(file);
            return result.Ok(url);
        }

        /// <summary>
        /// 上传单个听力试题
        /// </summary>
        /// <param name="listeningPaper">听力试题</param>
        /// <returns></returns>
        [HttpPost("add/single")]
        [Authorize(MyConstant.Admin)]
        // 这里为了方便前端上传数据，设置成List更加容易获取且不会报错
        public Result addSingle([FromBody] ListeningPaperList list) {
            int id = listeningPaperService.addSingle(list.list[0]);
            if(id == -1) {
                return result.failed(StatusCode.ServerError, "数据错误");
            }
            return result.Ok(id);
        }

        /// <summary>
        /// 修改paper的标题或者听力音频的路径
        /// </summary>
        /// <param name="listeningPaperDTO">listeningPaperDTO对象</param>
        /// <returns></returns>
        [HttpPost("update/paper")]
        [Authorize(MyConstant.Admin)]
        public Result updateTitleOrAudio([FromBody] ListeningPaperDTO listeningPaperDTO) {
            var success = listeningPaperService.updateTitleOrAudio(listeningPaperDTO);
            return result.Ok(success);
        }

        /// <summary>
        /// 根据ID删除题目
        /// </summary>
        /// <param name="listeningPaperId">试题id</param>
        /// <param name="PartId">partId</param>
        /// <param name="questionId">题目id</param>
        /// <returns></returns>
        [HttpPost("delete/question/{listeningPaperId}/{PartId}/{questionId}")]
        [Authorize (MyConstant.Admin)]
        public Result deleteQuestionById(int listeningPaperId, int PartId, int questionId) {
            var success = listeningPaperService.deleteQuestionById(listeningPaperId, PartId, questionId);
            return result.Ok(success);
        }

        /// <summary>
        /// 根据Id删除part
        /// </summary>
        /// <param name="listeningPaperId">试题id</param>
        /// <param name="partId">partId</param>
        /// <returns></returns>
        [HttpPost("delete/part/{listeningPaperId}/{partId}")]
        public Result deletePartById(int listeningPaperId, int partId) {
            var success = listeningPaperService.deletePartById(listeningPaperId, partId);
            return result.Ok(success);
        }

        /// <summary>
        /// 根据id修改问题
        /// </summary>
        /// <param name="listeningPaperId">试题id</param>
        /// <param name="partId">partId</param>
        /// <param name="questionId">问题id</param>
        /// <param name="listeingQuestionDTO">要修改的问题内容</param>
        /// <returns></returns>
        [HttpPost("update/question/{listeningPaperId}/{partId}/{questionId}")]
        [Authorize(MyConstant.Admin)]
        public Result updateQuestionById(int listeningPaperId,
            int partId, int questionId,[FromBody] ListeingQuestionDTO listeingQuestionDTO) {
            var success = listeningPaperService.updateQuestionById(listeningPaperId, partId, questionId, listeingQuestionDTO);
            return result.Ok(success);
        }

        /// <summary>
        /// 根据id修改part的内容
        /// </summary>
        /// <param name="listeningPaperId">试题id</param>
        /// <param name="partId">partId</param>
        /// <param name="updatePartDTO">要修改的内容</param>
        /// <returns></returns>
        [HttpPost("update/part/{listeningPaperId}/{partId}")]
        [Authorize(MyConstant.Admin)]
        public Result updatePartById(int listeningPaperId, int partId,[FromBody] UpdatePartDTO updatePartDTO) {
            var success = listeningPaperService.updatePartById(listeningPaperId, partId, updatePartDTO);
            return result.Ok(success);
        }

        /// <summary>
        /// 添加part
        /// </summary>
        /// <param name="addPartDTO">要添加的part的内容</param>
        /// <returns></returns>
        [HttpPost("add/part")]
        [Authorize(MyConstant.Admin)]
        public Result addPart([FromBody] AddPartDTO addPartDTO) {
            var success = listeningPaperService.addPart(addPartDTO);
            return result.Ok(success);
        }

        [HttpPost("add/question/{listeningPaperId}")]
        [Authorize(MyConstant.Admin)]
        public Result addQuestion(int listeningPaperId,[FromBody] ListeningQuestion listeningQuestion) {
            var success = listeningPaperService.addQuestion(listeningPaperId, listeningQuestion);
            return result.Ok(success);
        }

    }
}
