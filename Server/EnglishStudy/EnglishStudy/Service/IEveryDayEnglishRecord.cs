using EnglishStudy.DTO;

namespace EnglishStudy.Service {
    public interface IEveryDayEnglishRecord {

        /// <summary>
        /// 根据id集合删除记录
        /// </summary>
        /// <param name="IdList">id集合</param>
        /// <returns>成功删除的个数</returns>
        public int Delete(int UserId, List<int> IdList);

        /// <summary>
        /// 添加阅读记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="EverydayId">每日英语id</param>
        /// <returns>是否添加成功</returns>
        public bool Add(int UserId, int EverydayId);

        /// <summary>
        /// 分页获取每日英语阅读记录
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns>分页数据</returns>
        public EveryDayEnglishRecordDTOPage GetByPageSize(int UserId, int page = 1, int size = 10);

        /// <summary>
        /// 获取用户每日英语阅读总篇数
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <returns>阅读总篇数</returns>
        public int GetTotal(int UserId);

        /// <summary>
        /// 更新每日英语阅读记录时间
        /// </summary>
        /// <param name="RecordId">阅读记录ID</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateTime(int UserId ,int RecordId);

        /// <summary>
        /// 根据用户id和每日英语id获取recordId
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="EverydayId">每日英语id</param>
        /// <returns>0表示没有，非0表示存在</returns>
        public int getRecordIdByUserIdAndEverydayId(int UserId, int EverydayId);
    }
}
