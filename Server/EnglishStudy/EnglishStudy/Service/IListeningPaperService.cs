using EnglishStudy.DTO;
using EnglishStudy.DTO.MyListening;
using EnglishStudy.Entity;

namespace EnglishStudy.Service {
    public interface IListeningPaperService {

        /// <summary>
        /// 添加听力服务
        /// </summary>
        /// <param name="files">文件，包含音频和实体</param>
        /// <param name="PaperTitle">试题题目</param>
        /// <returns>题目编号</returns>
        public  Task<int> AddListeningPaper(List<IFormFile> files, string PaperTitle);

        /// <summary>
        /// 根据id获取听力
        /// </summary>
        /// <param name="listeningPaperId">试题id</param>
        /// <returns>听力试题内容</returns>
        public ListeningPaper GetByListeningPaperId(int listeningPaperId);

        /// <summary>
        /// 根据试题id删除试题
        /// </summary>
        /// <param name="listeningPaperId">试题id</param>
        /// <returns>true表示删除成功</returns>
        public bool DeleteByListeningPaperId(int listeningPaperId);

        /// <summary>
        /// 获取听力试题总数
        /// </summary>
        /// <returns></returns>
        public int GetTotal();

        /// <summary>
        /// 根据页码和页面大小获取分页数据
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns>页面数据</returns>
        public ListeningPaperPage GetByPageSize(int page = 1, int size= 10);

        /// <summary>
        /// 上传听力音频服务
        /// </summary>
        /// <param name="file"听力文件></param>
        /// <returns>听力音频的地址</returns>
        public Task<string> uploadAudio(IFormFile file);


        /// <summary>
        /// 上传听力试题接口
        /// </summary>
        /// <param name="listening">听力试题</param>
        /// <returns>上传成功的id</returns>
        public int addSingle(ListeningPaper listening);

        /// <summary>
        /// 修改paper的标题或者听力音频的路径
        /// </summary>
        /// <param name="listeningPaper"></param>
        /// <returns>true表示修改成功，false表示修改失败</returns>
        public bool updateTitleOrAudio(ListeningPaperDTO listeningPaper);

        /// <summary>
        /// 根据questionId删除question
        /// </summary>
        /// <param name="questionId">id</param>
        /// <param name="listeningPaperId">id</param>
        /// <param name="PartId">id</param>
        /// <returns>true表示删除成功</returns>
        public bool deleteQuestionById(int listeningPaperId,int PartId,int questionId);

        /// <summary>
        /// 根据partId删除part
        /// </summary>
        /// <param name="partId">id</param>
        /// <param name="listeningPaperId">id</param>
        /// <returns>true表示删除成功</returns>
        public bool deletePartById(int listeningPaperId, int partId);


        /// <summary>
        /// 根据id修改question
        /// </summary>
        /// <param name="listeningPaperId">id</param>
        /// <param name="partId">id</param>
        /// <param name="questionId">id</param>
        /// <param name="listeingQuestionDTO">要修改的值</param>
        /// <returns></returns>
        public bool updateQuestionById(int listeningPaperId, 
            int partId, int questionId, ListeingQuestionDTO listeingQuestionDTO);

        /// <summary>
        /// 根据修改part的标题和听力原文
        /// </summary>
        /// <param name="listeningPaperId">试题id</param>
        /// <param name="partId">partId</param>
        /// <param name="updatePartDTO">要更新的数据</param>
        /// <returns></returns>
        public bool updatePartById(int listeningPaperId, int partId, UpdatePartDTO updatePartDTO);
    
        
        /// <summary>
        /// 添加part服务
        /// </summary>
        /// <param name="addPartDTO">part中的数据</param>
        /// <returns>true表示添加成功</returns>
        public bool addPart(AddPartDTO addPartDTO);

        /// <summary>
        /// 添加听力题目
        /// </summary>
        /// <param name="listeningPaperId">听力试题id</param>
        /// <param name="listeningQuestion">添加的听力题目</param>
        /// <returns>true表示添加成功</returns>
        public bool addQuestion(int listeningPaperId, ListeningQuestion listeningQuestion);
    }
}
