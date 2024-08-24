using EnglishStudy.DTO;
using EnglishStudy.Entity;

namespace EnglishStudy.Service {
    public interface IPassageService {

        /// <summary>
        /// 上传题目文件
        /// </summary>
        /// <param name="file">excel文件</param>
        /// <returns>是否解析成功</returns>
        public Task<bool> AddFile(IFormFile file,string title);

        /// <summary>
        /// 通过文章id获取阅读理解文章内容
        /// </summary>
        /// <param name="id">文章id</param>
        /// <returns>文章</returns>
        public PassageDTO GetPassageById(int id);

        /// <summary>
        /// 通过文章id删除阅读理解文章，设置删除标志位为1
        /// </summary>
        /// <param name="id">文章id</param>
        /// <returns>true表示删除成功</returns>
        public bool DeletePassageById(int id);

        /// <summary>
        /// 将删除的阅读理解文章恢复
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public bool RecoverPassageById(int id);

        /// <summary>
        /// 修改阅读理解文章内容
        /// </summary>
        /// <param name="passageDTO">passageDTO类</param>
        /// <returns>true表示修改成功</returns>
        public bool UpdatePassage(PassageDTO passageDTO);

        /// <summary>
        /// 批量添加阅读理解文章
        /// </summary>
        /// <param name="passageList">文章集合</param>
        /// <returns>成功添加的数量</returns>
        public int AddPassageList(List<Passage> passageList);

        /// <summary>
        /// 分页查询阅读理解的文章
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns>查询到的数据</returns>
        public PassagePage GetPassageByPageSize(int page = 1, int size = 10);

        /// <summary>
        /// 管理员分页查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <param name="delete">0表示未删除</param>
        /// <returns>查询到的数据</returns>
        public PassagePage AdminGetPassageByPageSize(int page = 1, int size = 10, int delete = 0);

        /// <summary>
        /// 获取阅读理解文章总数
        /// </summary>
        /// <returns>文章的总数</returns>
        public int GetPassageTotal();
    }
}
