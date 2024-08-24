using EnglishStudy.DTO;
using EnglishStudy.Entity.ChildEntity;

namespace EnglishStudy.Service {
    public interface IWordRecordService {

        /// <summary>
        /// 添加记忆单词的记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="Type">单词类型</param>
        /// <param name="LastId">最后一个单词的id</param>
        /// <param name="wordRecordDetail">记忆的单词列表</param>
        /// <returns>记录id</returns>
        public int AddRecord(int UserId, int Type, int LastId, WordRecordDetail wordRecordDetail);

        /// <summary>
        /// 获取近size次单词记录的数量集合
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="Type">单词类型</param>
        /// <param name="size">size</param>
        /// <returns>total集合</returns>
        public List<int> GetTotalList(int UserId, int Type,int size = 10);

        /// <summary>
        /// 获取本次需要记忆的单词集合
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="type">单词类型</param>
        /// <param name="size">size</param>
        /// <returns>返回单词集合</returns>
        public List<WordRecordDTO> GetWordList(int UserId, int type, int size = 20);

        /// <summary>
        /// 获取用户上一次记忆的单词
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="type">单词类型</param>
        /// <returns>返回单词集合</returns>
        public List<WordDTO> GetLastRecord(int UserId, int type);

        /// <summary>
        /// 将lastId设置为0，重新开始记忆
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="type">单词类型</param>
        /// <returns>true表示修改成功</returns>
        public bool UpdateLastId(int UserId, int type);

        /// <summary>
        /// 获取用户记忆的所有单词
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="type">单词类型</param>
        /// <returns></returns>
        public WordPage getAllWord(int UserId, int type, int page, int size);
    }
}
