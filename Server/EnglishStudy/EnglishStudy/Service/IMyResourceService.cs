using EnglishStudy.DTO.Resource;
using EnglishStudy.Entity;


namespace EnglishStudy.Service {
    public interface IMyResourceService {

        /// <summary>
        /// 添加资源服务
        /// </summary>
        /// <param name="resource">资源对象</param>
        /// <returns>是否添加成功</returns>
        public bool AddResource(Resource resource);

        /// <summary>
        /// 上传资源封面
        /// </summary>
        /// <param name="file">图片</param>
        /// <returns>图片地址</returns>
        public Task<string> UploadImage(IFormFile file);

        /// <summary>
        /// 上传资源
        /// </summary>
        /// <param name="file">资源</param>
        /// <returns>资源地址</returns>
        public Task<string> UploadResource(IFormFile file);

        /// <summary>
        /// 管理员分页获取资源
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        public ResourceDTOPage AdminGetResource(int page = 1, int size = 10);

        /// <summary>
        /// 用户获取分页资源
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        public ResourceDTOPage GetResource(int page =1 , int size = 10);

        /// <summary>
        /// 修改提取码
        /// </summary>
        /// <param name="code">提取码</param>
        /// <returns>是否修改成功</returns>
        public bool UpdateCode(string code);

        /// <summary>
        /// 获取提取码
        /// </summary>
        /// <returns></returns>
        public string GetCode();

        /// <summary>
        /// 是否显示资源
        /// </summary>
        /// <param name="resourceId">资源id</param>
        /// <param name="delete_sign">显示标记位</param>
        /// <returns>是否修改成功</returns>
        public bool ShowOrNot(int resourceId, bool delete_sign);

        /// <summary>
        /// 删除资源
        /// </summary>
        /// <param name="resourceId">资源id</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteResource(int resourceId);

        /// <summary>
        /// 根据资源id和提取码获取资源URL
        /// </summary>
        /// <param name="resourceId">资源id</param>
        /// <param name="code">提取码</param>
        /// <returns>资源URL</returns>
        public string GetResourceUrl(int resourceId,string code);

        /// <summary>
        /// 根据关键字搜索资源
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public ResourceDTOPage SearchResource(string keyword,int page =1,int size = 10);

   
    }
}
