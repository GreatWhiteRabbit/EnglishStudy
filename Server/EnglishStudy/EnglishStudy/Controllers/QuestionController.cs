using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers {

    [ApiController]
    [Route("question")]
    public class QuestionController {
        private IQuestionService questionService;

        private Result result = new Result(); 

        public QuestionController(IQuestionService questionService) {
            this.questionService = questionService;
        }


        [HttpGet("passage/{id}")]
        /// <summary>
        /// 通过passageId获取question集合
        /// </summary>
        /// <param name="id">passageId</param>
        /// <returns></returns>
        public Result GetQuestionListByPassageId(int id) {
            var list = questionService.GetQuestionListByPassageId(id);
            if(list.Count == 0) {
                return result.failed(StatusCode.NotFound, "出错啦");
            }
            return result.Ok(list);
        }

        /// <summary>
        /// 根据id删除题目
        /// </summary>
        /// <param name="id">题目id</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("{id}")]
        public Result DeleteById(int id) {
            bool delete = questionService.DeleteById(id);
            if (delete) return result.Ok();
            return result.failed(StatusCode.BadRequest, "删除失败");
        }

        /// <summary>
        /// 添加题目集合
        /// </summary>
        /// <param name="questionList">题目集合</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("add")]
        public Result AddQuestionList([FromBody] MyQuestionDTOList questionList) {
            int count = questionService.AddQuestionList(questionList.questionList);
            return result.Ok(count);
        }

        /// <summary>
        /// 更新题目
        /// </summary>
        /// <param name="question">题目</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("update")]
        public Result UpdateQuestion([FromBody]UpdateQuestionDTO updateQuestionDTO) {
            bool update = questionService.UpdateQuestion(updateQuestionDTO);
            if(update) return result.Ok();
            return result.failed(StatusCode.BadRequest, "更新失败");
        }

        /// <summary>
        /// 根据ID恢复题目
        /// </summary>
        /// <param name="id">题目id</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("recover/{id}")]
        public Result RecoverQuestion(int id) {
            bool recover = questionService.RecoverQuestion(id);
            if(recover) return result.Ok();
            return result.failed(StatusCode.BadRequest, "恢复失败");
        }


        /// <summary>
        /// 获取答案列表
        /// </summary>
        /// <param name="PassageId"></param>
        /// <returns></returns>
        [HttpGet("answer/{passageId}")]
        public Result GetAnwserByPassageId(int PassageId) {
            return result.Ok(questionService.GetAnwserByPassageId(PassageId));
        }
    }
}
