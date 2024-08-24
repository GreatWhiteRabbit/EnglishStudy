using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers
{
    [ApiController]
    [Route("word")]
    public class WordController :ControllerBase {
       
        private  Result result = new Result();

        private IWordService wordService;

       

        public WordController(IWordService wordService) {
            this.wordService = wordService;
        }

        /// <summary>
        /// 获取单词类型的总数
        /// </summary>
        /// <param name="type">四六级单词</param>
        /// <returns>单词总数</returns>
        [Route("sum")]
        [HttpGet]
        public Result WordSum([FromQuery]int type = 1) {
             int count = wordService.WordSum(type);  
            return result.Ok(count);
        }

        /// <summary>
        /// 获取首字母的四六级单词的列表
        /// </summary>
        /// <param name="type">四六级单词</param>
        /// <param name="init">首字母</param>
        /// <param name="page">第几页</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        [Route("initlist")]
        [HttpGet]
        public  Result InitWordList([FromQuery]int type = 1, [FromQuery] string init = "a",
            [FromQuery] int page = 1, [FromQuery] int size = 20) {
             Task<WordPage> task = wordService.InitWordList(type, init, page, size);
            return result.Ok(task.Result);
        }

        /// <summary>
        /// 将单词表中的元素以hash的形式存储到Redis中
        /// </summary>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [Route("hash")]
        [HttpGet]
        public Result PushIntoHash() {
            bool success = wordService.PushAllWordIntoHash();
            if(success) {
                return result.Ok();
            }
            else {
                return result.failed(Utils.StatusCode.ServerError, "服务器内部错误");
            }
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [Authorize("Admin")]
        [HttpGet("all/{type}/{page}/{size}")]
        public Result GetAll(int type = 1, int page = 1, int size = 20) {
            var list = wordService.GetWordList(type, page, size);
            return result.Ok(list);
        }

        /// <summary>
        /// 添加单词集合接口
        /// </summary>
        /// <param name="wordList">单词集合</param>
        /// <param name="type">单词类型</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("addlist/{type}")]
        public Result AddWord([FromBody] MyWordList words,int type = 1) {
            if (words.wordList.Count == 0)
                return result.Ok(-1);
            var count = wordService.AddWord(words.wordList,type);
            return result.Ok(count);
        }

        /// <summary>
        /// 上传文件接口
        /// </summary>
        /// <param name="file">excel文件</param>
        /// <param name="type">单词类型</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("addfile/{type}")]
        public async Task<Result> AddWordFile(IFormFile file, int type = 1) {
            var count = await wordService.AddWord(file,type);
            return result.Ok(count);
        }

        /// <summary>
        /// 更新单词接口
        /// </summary>
        /// <param name="wordList">要更新的单词集合</param>
        /// <param name="type">单词类型</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("update/{type}")]
        public Result UpdateWord([FromBody] MyWordList words, int type = 1) {
            if (words.wordList.Count == 0)
                return result.Ok(-1);
            var count = wordService.UpdateWord(words.wordList, type);
            return result.Ok(count);
        }

        /// <summary>
        /// 删除单词接口
        /// </summary>
        /// <param name="wordList">删除单词集合</param>
        /// <param name="type">单词类型</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("delete/{type}")]
        public Result DeleteWrod([FromBody] MyWordList words, int type ) { 
            if(words.wordList.Count == 0)
                return result.Ok(-1);
            var count = wordService.DeleteWord(words.wordList,type);
            return result.Ok(count);
        }

        /// <summary>
        /// 根据关键字模糊匹配查询
        /// </summary>
        /// <param name="words">单词</param>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        [HttpGet("search")]
        public Result SearchByWords([FromQuery(Name = "keywords")]string keywords,
            [FromQuery(Name ="page")] int page = 1, [FromQuery(Name ="size")]int size = 20) {
            return result.Ok(wordService.SearchByKeywords(keywords,page,size));
        }

        /// <summary>
        /// 根据关键字模糊匹配查询
        /// </summary>
        /// <param name="words">单词</param>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        [HttpGet("adminsearch")]
        [Authorize(MyConstant.Admin)]
        public Result AdminSearchByWords([FromQuery(Name = "keywords")] string keywords,
            [FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "size")] int size = 20) {
            return result.Ok(wordService.AdminSearchByKeywords(keywords, page, size));
        }



    }
}
