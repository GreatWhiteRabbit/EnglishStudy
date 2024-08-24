using EnglishStudy.DTO;
using EnglishStudy.Entity.ChildEntity;

namespace EnglishStudy.Service {
    public interface IListeningPaperRecordService {

        /// <summary>
        /// 添加听力做题记录服务
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="ListeningPaperId">听力试题id</param>
        /// <param name="Accuracy">准确率</param>
        /// <param name="List">答题列表集合</param>
        /// <returns>做题记录id</returns>
        public int AddListeningPaperRecord(int UserId, int ListeningPaperId,Double Accuracy, List<AnswerDetail> List);


        /// <summary>
        /// 根据id批量删除记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="RecordIdList">记录id集合</param>
        /// <returns>成功删除的个数</returns>
        public int DeleteBatch(int UserId, List<int> RecordIdList);

        /// <summary>
        /// 根据page和size获取做题记录分页数据
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns>做题记录分页数据</returns>
        public ListeningPaperRecordDTOPage GetByPageSize(int UserId, int page, int size);

        /// <summary>
        /// 根据记录id获取数据
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="RecordId">记录id</param>
        /// <returns>做题记录数据</returns>
        public ListeningPaperRecordDTO GetByRecordId(int UserId, int RecordId);

        /// <summary>
        /// 根据用户id和听力试题id判断是否已经做了题目
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="ListeningPaperId">听力试题id</param>
        /// <returns>记录id</returns>
        public int ExistRecord(int UserId, int ListeningPaperId);

        /// <summary>
        /// 根据用户id获取做题总数
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <returns>做题总数</returns>
        public int GetTotal(int UserId);

        /// <summary>
        /// 根据用户id和paperId获取记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="paperId">paperId</param>
        /// <returns>做题记录</returns>
        public ListeningPaperRecordDTO getByUserIdAndPaperId(int UserId, int paperId);

        /// <summary>
        /// 获取近size次准确率
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<Double> GetAccuracyList(int UserId, int size = 10);
    }
}
