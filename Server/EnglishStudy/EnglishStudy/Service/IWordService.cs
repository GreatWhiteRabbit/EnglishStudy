using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Utils;

namespace EnglishStudy.Service
{
    public interface IWordService {

        /// <summary>
        /// 获取词汇的总数
        /// </summary>
        /// <param name="type">默认值为1，四级词汇</param>
        /// <returns>词汇总数</returns>
        public int WordSum(int type = 1);

        /// <summary>
        /// 获取首字母的四六级单词的列表
        /// </summary>
        /// <param name="type">四六级单词</param>
        /// <param name="init">首字母</param>
        /// <returns></returns>
        public Task<WordPage> InitWordList(int type = 1, string init = "a",int page = 1,int size = 20);

        /// <summary>
        /// 将MySQL中的单词以hash的形式存储到Redis中
        /// </summary>
        /// <param name="type">单词的类型，默认值为1</param>
        /// <returns></returns>
        public bool PushAllWordIntoHash();

        /// <summary>
        /// 添加单词
        /// </summary>
        /// <param name="wordList">单词列表</param>
        /// <param name="type">单词类型</param>
        /// <returns>成功添加的个数</returns>
        public int AddWord(List<Word> wordList, int type = 1);

        /// <summary>
        /// 添加单词
        /// </summary>
        /// <param name="file">存储单词的excel文件</param>
        /// <param name="type">单词类型</param>
        /// <returns>成功添加的个数</returns>
        public Task<int> AddWord(IFormFile file, int type = 1);

        /// <summary>
        /// 删除单词
        /// </summary>
        /// <param name="wordList">要删除的单词列表</param>
        /// <param name="type">单词类型</param>
        /// <returns>删除成功的个数</returns>
        public int DeleteWord(List<Word> wordList, int type = 1);

        /// <summary>
        /// 修改单词
        /// </summary>
        /// <param name="wordList">单词列表</param>
        /// <param name="type">单词类型</param>
        /// <returns></returns>
        public int UpdateWord(List<Word> wordList, int type = 1);

        /// <summary>
        /// 管理员获取单词列表
        /// </summary>
        /// <param name="type">单词类型</param>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns>单词总数</returns>
        public List<Word> GetWordList(int type = 1, int page = 1, int size = 20);

        /// <summary>
        /// 根据关键字模糊搜素单词列表
        /// </summary>
        /// <param name="keywords">关键字</param>
        /// <returns>单词列表</returns>
        public List<WordDTO> SearchByKeywords(string keywords, int page = 1, int size = 20);


        public List<Word> AdminSearchByKeywords(string keywords, int page = 1, int size = 20);
    }
}
