using System.ComponentModel;

namespace EnglishStudy.Utils {

    /// <summary>
    /// 常量
    /// </summary>
    public  class MyConstant {

        /// <summary>
        /// 四级单词总数
        /// </summary>
        public static readonly string CET_4_Word_Num = "CET_4_Word_Num";

        /// <summary>
        /// 四级词汇代号
        /// </summary>
        public static readonly int CET_4_Word_Type = 1;

        /// <summary>
        /// 六级单词总数
        /// </summary>
        public static readonly string CET_6_Word_Num = "CET_6_Word_Num";

        /// <summary>
        /// 六级词汇代号
        /// </summary>
        public static readonly int CET_6_Word_Type = 2;

        /// <summary>
        /// 考研词汇
        /// </summary>
        public static readonly string GRE_Word_Num = "GRE_Word_Num";

        /// <summary>
        /// 考研词汇代号
        /// </summary>
        public static readonly int GRE_Word_Type = 3;

        /// <summary>
        /// 将单词以hash的形式存储到Redis中，Word_Hash + 1或2或3，方便修改单词
        /// </summary>
        public static readonly string Word_Hash = "Word_Hash";


        /// <summary>
        /// 将单词按照首字母存储在不同的list中
        /// </summary>
        public static readonly string Word_List_Init = "Word_List_Init";

        /// <summary>
        /// 账号不存在
        /// </summary>
        public static int AccountNotExist = 1;

        /// <summary>
        /// 账号或者密码错误
        /// </summary>
        public static int AccountOrPasswordError = 2;
        
        /// <summary>
        /// 邮箱已经被注册
        /// </summary>
        public static int EmailAlreadyRegistered = 3;

        /// <summary>
        /// 登录成功
        /// </summary>
        public static int LoginSuccess = 4;

        /// <summary>
        /// 验证码错误
        /// </summary>
        public static int ChaptcharError = 5;

        /// <summary>
        /// 密码不满足规则
        /// </summary>
        public static int PasswordRuleError = 6;

        /// <summary>
        /// 邮箱格式错误
        /// </summary>
        public static int EmailRuleError = 7;

        /// <summary>
        /// 存储文章
        /// </summary>
        public static string Passage = "Passage";

        /// <summary>
        /// 存储题目列表
        /// </summary>
        public static string QuestionList = "QuestionList";

        /// <summary>
        /// 答案列表
        /// </summary>
        public static string AnswerList = "AnserList";

        /// <summary>
        /// 用于对passage的分页查询
        /// </summary>
        // 由于之前的分页查询全部是使用list来完成的，list存在一个非常严重的缺点
        // 就是如果要做分页查询，需要先将数据全部导入到list中，然后才能分页
        // 不能适合增删的情形，所以以后得分页查询准备采用sortset完成
        public static string PassageSortSet = "PassageSortSet";

        /// <summary>
        /// 阅读理解文章的总数
        /// </summary>
        public static string PassageTotal = "PassageTotal";

        /// <summary>
        /// 每日英语sortedset
        /// </summary>
        public static string EveryDayEnglishSortSet = "EveryDayEnglishSortSet";

        /// <summary>
        /// 每日英语string存储
        /// </summary>
        public static string EveryDayEnglishString = "EveryDayEnglishString";

        /// <summary>
        /// 每日英语的篇数
        /// </summary>
        public static string EveryDayEnglishTotal = "EveryDayEnglishTotal";

        /// <summary>
        /// 用户每日英语阅读记录sortset结构
        /// </summary>
        public static string UserEveryDayEnglishRecordSortSet = "UserEveryDayEnglishRecordSortSet";

        /// <summary>
        /// 用户每日英语阅读总篇数
        /// </summary>
        public static string UserEveryDayEnglishRecordTotal = "UserEveryDayEnglishRecordTotal";

        /// <summary>
        /// 用户完成阅读理解记录sortset结构
        /// </summary>
        public static string UserPassageRecordTotal = "UserPassageRecordTotal";

        /// <summary>
        /// 用户阅读理解记录总篇数
        /// </summary>
        public static string UserPassageRecordSortSet = "UserPassageRecordSortSet";

        /// <summary>
        /// 听力试题sortset结构
        /// </summary>
        public static string ListeningPaperSortSet = "ListeningPaperSortSet";

        /// <summary>
        /// 听力试题总数
        /// </summary>
        public static string ListeningPaperTotal = "ListeningPaperTotal";

        /// <summary>
        /// 用户听力做题记录sortset集合
        /// </summary>
        public static string UserListeningPaperRecordSortSet = "UserListeningPaperRecordSortSet";

        /// <summary>
        /// 用户听力做题记录总数
        /// </summary>
        public static string UserListingPaperRecordTotal = "UserListingPaperRecordTotal";

        /// <summary>
        /// 管理员
        /// </summary>
        public const  string Admin = "Admin";

        /// <summary>
        /// 普通用户
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// 管理员和普通用户
        /// </summary>
        public const string UserOrAdmin = "UserOrAdmin";


        /// <summary>
        /// 定时任务执行时间
        /// </summary>
        public static string WorkTime = "WorkTime";

        /// <summary>
        /// 资源提取密码
        /// </summary>
        public const string ResourceCode = "ResourceCode";
    }
}
    