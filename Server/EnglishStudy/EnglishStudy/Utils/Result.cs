namespace EnglishStudy.Utils {

    /// <summary>
    /// 包装好的网络响应
    /// </summary>
    public class Result {
        public bool success { set; get; }

        public  StatusCode StatusCode { get; set; }

        public Object data { get; set; }

        public Result(StatusCode code, bool success, Object data) {
            this.data = data;
            this.success = success;
            this.StatusCode = code;
        }
        public Result() {

        }

        /// <summary>
        /// 返回响应成功的结果
        /// </summary>
        /// <param name="data">响应成功的结果</param>
        /// <returns>响应成功的结果</returns>
        public Result Ok(Object data) {
            return new Result(StatusCode = StatusCode.Success, true, data);
        }

        public Result Ok() {
            return new Result(StatusCode.Success, true, "success");
        }

        /// <summary>
        /// 返回响应失败的结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="data">响应失败结果</param>
        /// <returns>响应失败的结果</returns>
        public Result failed(StatusCode code, Object data) {
            return new Result(code, false, data);
        }
    }
}
