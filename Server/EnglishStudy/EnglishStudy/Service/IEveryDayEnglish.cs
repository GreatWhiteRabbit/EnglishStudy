using EnglishStudy.DTO;
using EnglishStudy.Entity;

namespace EnglishStudy.Service {
    public interface IEveryDayEnglish {

        /// <summary>
        /// 上传数据文件，文件excel文件
        /// </summary>
        /// <param name="file">上传的文件</param>
        /// <returns>成功添加的数据</returns>
        public Task<int> UploadFile(IFormFile file);

        /// <summary>
        /// 根据id集合删除数据
        /// </summary>
        /// <param name="idList">id集合</param>
        /// <returns>成功删除的个数</returns>
        public int Delete(List<int> idList);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="everyDayEnglish">数据</param>
        /// <returns>是否修改成功</returns>
        public bool Update(EveryDayEnglish everyDayEnglish);

        /// <summary>
        /// 通过id获取每日英语
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EveryDayEnglish GetById(int id);

        /// <summary>
        /// 通过页码和页面大小获取每日英语
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns>页面数据</returns>
        public EveryDayEnglishPage GetByPageSize(int page = 1, int size = 10);

        /// <summary>
        /// 管理员获取分页数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <param name="delete">0表示未删除</param>
        /// <returns>页面数据</returns>
        public EveryDayEnglishPage AdminGetByPageSize(int page = 1, int size = 10, int delete = 0);

        /// <summary>
        /// 获取所有没有被删除的每日英语
        /// </summary>
        /// <returns>返回每日英语总数</returns>
        public int GetTotal();

        /// <summary>
        /// 恢复被删除的每日英语
        /// </summary>
        /// <param name="IdList">整数集合</param>
        /// <returns>成功恢复的个数</returns>
        public int Recover(List<int> IdList);
    }
}
