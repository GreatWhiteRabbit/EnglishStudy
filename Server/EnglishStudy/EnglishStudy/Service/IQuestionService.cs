using EnglishStudy.DTO;
using EnglishStudy.Entity;

namespace EnglishStudy.Service {
    public interface IQuestionService {

        /// <summary>
        /// 通过passageId获取question集合
        /// </summary>
        /// <param name="id">passageId</param>
        /// <returns>question集合</returns>
        public List<QuestionDTO> GetQuestionListByPassageId(int id);

        /// <summary>
        /// 根据阅读理解id获取答案列表
        /// </summary>
        /// <param name="PassageId">passageId</param>
        /// <returns>答案列表</returns>
        public List<AnwserDTO> GetAnwserByPassageId(int PassageId);

        /// <summary>
        /// 根据questionId形式删除题目
        /// </summary>
        /// <param name="id">questionId</param>
        /// <returns>true表示删除成功</returns>
        public bool DeleteById(int id);

        /// <summary>
        /// 添加题目集合
        /// </summary>
        /// <param name="questionList">题目集合</param>
        /// <returns>成功添加的数量</returns>
        public int AddQuestionList(List<UpdateQuestionDTO> questionList);

        /// <summary>
        /// 修改题目
        /// </summary>
        /// <param name="question">修改后的题目</param>
        /// <returns>true表示修改成功</returns>
        public bool UpdateQuestion(UpdateQuestionDTO updateQuestionDTO);

        /// <summary>
        /// 将被删除的题目恢复
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>true表示操作成功</returns>
        public bool RecoverQuestion(int id);

    }
}
