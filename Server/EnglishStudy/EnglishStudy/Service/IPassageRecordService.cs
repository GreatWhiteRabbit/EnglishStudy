using EnglishStudy.DTO;
using EnglishStudy.Entity.ChildEntity;

namespace EnglishStudy.Service {
    public interface IPassageRecordService {

        /// <summary>
        /// 添加做题记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="PassageId">阅读理解id</param>
        /// <param name="answerDetailList">答题情况列表</param>
        /// <returns>是否添加成功</returns>
        public bool AddRecord(int UserId, int PassageId, List<AnswerDetail> answerDetailList);

        /// <summary>
        /// 批量删除做题记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="RecordIdList">做题记录id集合</param>
        /// <returns>成功删除的个数</returns>
        public int DeleteBatch(int UserId, List<int> RecordIdList);

        /// <summary>
        /// 分页获取做题记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="page">页面</param>
        /// <param name="size">页码</param>
        public PassageRecordDTOPage GetRecordByPageSize(int UserId, int page, int size);

        /// <summary>
        /// 根据阅读理解id获取做题情况
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="PassageId">做题记录id</param>
        public PassageRecordDTO GetRecordByPassageId(int UserId, int PassageId);

        /// <summary>
        /// 获取用户的阅读理解做题记录总数
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <returns>总数</returns>
        public int GetTotal(int UserId);

        /// <summary>
        /// 更新准确率
        /// </summary>
        /// <param name="PassageId">文章id</param>
        /// <param name="Accuracy">准确率</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateAccuracy(int UserId, int PassageId, Double Accuracy);

        /// <summary>
        /// 获取近size此准确率
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="size">size</param>
        /// <returns>准确率集合</returns>
        public List<Double> GetAccuracyList(int UserId, int size);
    }
}
