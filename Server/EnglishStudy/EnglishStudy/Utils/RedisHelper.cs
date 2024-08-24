using NPOI.Util;
using StackExchange.Redis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace EnglishStudy.Utils {

    /// <summary>
    /// 封装Redis工具类
    /// </summary>

    public class RedisHelper {
        private static ConfigurationOptions options;
        // 设置编码，否则中文乱码
        private static JsonSerializerOptions encoder = new JsonSerializerOptions() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs)
        };
        static RedisHelper() {
            options = new ConfigurationOptions();

            // 读取配置文件
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json").Build();
            int timeout = int.Parse(configuration["Redis:timeout"]);
            string server = configuration["Redis:server"];
            string port = configuration["Redis:port"];
            string password = configuration["Redis:password"];
            /* options.ConnectTimeout = 5000; // 设置超时时间
             options.EndPoints.Add("192.168.26.129:6379"); // 设置IP和端口
             options.Password = "123456"; // 密码*/
            options.ConnectTimeout = timeout;
            options.EndPoints.Add(server + ":" + port);
            options.Password = password;
        }

        /// <summary>
        /// 把对象以string的方式存入到Redis中
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="Object">对象的值</param>
        /// <returns>true表示存储成功</returns>
        public bool setString<T>(string key, T Object) {
            // 先将object转换成json
            string json = transfeToJson(Object);
            if (json == "") {
                // 转换失败，直接返回false
                return false;
            }
            // 获取Redis的操作对象
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                // 获取操作的数据库对象
                IDatabase db = conn.GetDatabase();
                // 将json存入到Redis
                return db.StringSet(key, json);
            }

        }

        /// <summary>
        /// 以string的方式存入到Redis中并且设置过期时间
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="Object">对象的值</param>
        /// <param name="seconds">过期时间，单位为秒</param>
        /// <returns>true表示存储成功</returns>
        public bool setString<T>(string key, T Object, int seconds) {
            // 先将object转换成json
            string json = transfeToJson(Object);
            if (json == "") {
                // 转换失败，直接返回false
                return false;
            }
            // 获取Redis的操作对象
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                // 获取操作的数据库对象
                IDatabase db = conn.GetDatabase();
                // 将json存入到Redis
                return db.StringSet(key, json, TimeSpan.FromSeconds(seconds));
            }
        }

        /// <summary>
        /// 获取string类型的key对应的object对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>key对应存在返回对象，不存在返回空</returns>
        public T getStringObject<T>(string key) {
            string json = getString(key);
            if (json == "") {
                return default(T);
            }
            else {
                return transfeToObject<T>(json);
            }
        }

        /// <summary>
        /// 获取key对应的json
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>返回空串表示不存在</returns>
        public string getString(string key) {

            // 操作redis
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                // 获取数据库
                IDatabase db = conn.GetDatabase();
                // key存在则返回
                if (exist(key)) {
                    return db.StringGet(key);
                }
                else {
                    // 不存在返回空串
                    return "";
                }

            }
        }

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true</returns>
        public bool exist(string key) {
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                return db.KeyExists(key);
            }
        }

        /// <summary>
        /// 设置key的过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="seconds">过期时间，单位为秒</param>
        /// <returns>设置成功返回true</returns>
        public bool setExpire(string key, int seconds) {

            if (!exist(key)) {
                return false;
            }
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                // 设置过期时间
                return db.KeyExpire(key, TimeSpan.FromSeconds(seconds));

            }
        }

        /// <summary>
        /// 将objectList插入到List列表中
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">list的key</param>
        /// <param name="objectList">对象集合</param>
        /// <returns>true表示插入成功</returns>
        public bool listLeftPushAll<T>(string key, List<T> objectList) {
            // 先将objectList转换成jsonList
            List<string> stringList = transfeToStringList(objectList);
            if (stringList == null) {
                return false;
            }
            // 将jsonList插入到Redis中
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                long before = db.ListLength(key);
                
                foreach (string json in stringList) {
                    db.ListLeftPush(key, json);
                }
                long after = db.ListLength(key);
                return !(before == after);
            }
        }

        /// <summary>
        /// 向List中leftPush元素
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="Object">对象的值</param>
        /// <returns>日否插入成功</returns>
        public bool listLeftPush<T>(string key, T Object) {
            // 先将object转换成json
            string json = transfeToJson(Object);
            if (json == "") {
                return false;
            }
            // 将json插入到List中
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                // 获取插入前的长度
                long before = db.ListLength(key);
                long after = db.ListLeftPush(key, json);
                return !(before == after);
            }
        }

        /// <summary>
        /// 将objectList插入到List列表中
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">list的key</param>
        /// <param name="objectList">对象集合</param>
        /// <returns>true表示插入成功</returns>
        public bool listRightPushAll<T>(string key, List<T> objectList) {
            // 先将objectList转换成jsonList
            List<string> stringList = transfeToStringList(objectList);
            if (stringList == null) {
                return false;
            }
            // 将jsonList插入到Redis中
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                long before = db.ListLength(key);
                foreach (string json in stringList) {
                    db.ListRightPush(key, json);
                }
                long after = db.ListLength(key);
                return !(before == after);
            }
        }

        /// <summary>
        /// 向List中rightPush元素
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="Object">对象的值</param>
        /// <returns>日否插入成功</returns>
        public bool listRightPush<T>(string key, T Object) {
            // 先将object转换成json
            string json = transfeToJson(Object);
            if (json == "") {
                return false;
            }
            // 将json插入到List中
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                // 获取插入前的长度
                long before = db.ListLength(key);
                long after = db.ListRightPush(key, json);
                return !(before == after);
            }
        }

        /// <summary>
        /// 从List左边移除并返回第一个对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">List对应的key</param>
        /// <returns>对象存在返回对象的值，否则返回null</returns>
        public T getListLeftPop<T>(string key) {
            // key不存在返回空
            if (!exist(key)) {
                return default(T);
            }
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                // 获取Redisvalue
                RedisValue value = db.ListLeftPop(key);
                // 将RedisValue转换成T类型的对象
                return transfeToObject<T>(value.ToString());
            }

        }


        /// <summary>
        /// 从List右边移除并返回第一个对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">List对应的key</param>
        /// <returns>对象存在返回对象的值，否则返回null</returns>
        public T getListRightPop<T>(string key) {
            // key不存在返回空
            if (!exist(key)) {
                return default(T);
            }
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                // 获取Redisvalue
                RedisValue value = db.ListRightPop(key);
                // 将RedisValue转换成T类型的对象
                return transfeToObject<T>(value.ToString());
            }
        }

        /// <summary>
        /// 返回从start到end的对象的集合
        /// start = 0,end = -1表示返回所有的元素
        /// start = 0, end = 1表示返回下标为0到1的元素，包括1
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">开始下标，默认值0</param>
        /// <param name="end">结束下标，默认值-1</param>
        /// <returns>对象的集合</returns>
        public List<T> getListRange<T>(string key, int start = 0, int end = -1) {
            List<T> objectList = new List<T>();
            if (!exist(key)) {
                return objectList;
            }
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                // 获取stringList
                List<RedisValue> stringList = db.ListRange(key, start, end).ToList();
                objectList = transfeToObjectList<T>(stringList);
            }
            return objectList;
        }

        /// <summary>
        /// 返回key对应的list的长度
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>list的长度</returns>
        public long getListLength(string key) {
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                return db.ListLength(key);
            }
        }


        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true表示删除成功</returns>
        public bool deleteKey(string key) {
            if (!exist(key)) {
                return false;
            }
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
              
                return db.KeyDelete(key);
            }
        }

        /// <summary>
        /// 将value以hash的形式存储，键为key，值为value
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="filed">hash表中的键</param>
        /// <param name="value">值</param>
        /// <param name="expireTime">过期时间，-1表示不过期，单位为秒</param>
        /// <returns></returns>
        public bool setHash<T>(string key, string filed,T value, int expireTime = -1) {
            string json = transfeToJson(value);
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                   bool success = db.HashSet(key, filed, json);
                    
                if(success) {
                    // 设置过期时间
                    if (expireTime > 0) {
                       return db.KeyExpire(key + filed, TimeSpan.FromSeconds(expireTime));
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 将元素批量插入到Redis中
        /// </summary>
        /// <typeparam name="T">元素的类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="filedList">键集合</param>
        /// <param name="valueList">值</param>
        /// <param name="expireTime">过期时间，默认值为-1表示不过期</param>
        /// <returns></returns>
        public bool setHash<T>(string key, List<string> filedList, List<T> valueList, int expireTime = -1) {
            bool success = false;
            // filed和value的个数不同，直接返回false
            if (filedList.Count != valueList.Count) return success;
            
            try {
                List<string> jsonList = transfeToStringList(valueList);
                using (var conn = ConnectionMultiplexer.Connect(options)) {
                    IDatabase db = conn.GetDatabase();
                    IBatch batch = db.CreateBatch();
                    
                    // 批量插入
                    if (expireTime > 0) {
                        
                        for (int i = 0; i < jsonList.Count; i++) {
                            batch.HashSetAsync(key, filedList[i], jsonList[i]);
                            batch.KeyExpireAsync(key + filedList[i], TimeSpan.FromSeconds(expireTime));
                        }
                    }
                    else {
                        for (int i = 0; i < jsonList.Count; i++) {
                            batch.HashSetAsync(key, filedList[i], jsonList[i]);
                        }
                    }
                    batch.Execute();
                    success = true;
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return success;
        }
        
        /// <summary>
        /// 判断hash中的filed是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="filed">filed</param>
        /// <returns>true表示存在</returns>
        public bool hashExist(string key,string filed) {
            bool exist = false;
            using(var conn =  ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                exist = db.HashExists(key, filed);
            }
            return exist;
        }

        /// <summary>
        /// 删除hash表中的数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="filed">filed</param>
        /// <returns></returns>
        public bool hashDelete(string key, string filed) {
            bool exist = false;
            using(var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                 exist = db.HashDelete(key, filed);
            }
            return exist;
        }

        /// <summary>
        /// 获取hash
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="filed">filed</param>
        /// <returns></returns>
        public T getHash<T>(string key, string filed) {
            using(var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                string valueJson = db.HashGet(key, filed);
                if(valueJson == null) {
                    return default(T);
                }
                return transfeToObject<T>(valueJson);
            }
        }

        public List<T> getHashList<T>(string key, List<string> filedList) {
            List<T> valueList = new List<T>();
            try {
                using(var conn = ConnectionMultiplexer.Connect(options)) {
                    List<RedisValue> stringList = new List<RedisValue>();
                    IDatabase db = conn.GetDatabase();
                    // 在这里使用bach好像会卡死
                    for (int i = 0; i < filedList.Count; i++) {
                        stringList.Add(db.HashGet(key, filedList[i]));
                    }
                    valueList = transfeToObjectList<T>(stringList);
                }

            } catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return valueList;
        }

        /// <summary>
        /// 将对象以set的形式存储在Redis中
        /// </summary>
        /// <typeparam name="T">对象类习惯</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expireTime">过期时间，-1表示不过期，单位为秒</param>
        /// <returns></returns>
        public bool setSet<T>(string key, T value, int expireTime = -1) {
            string json = transfeToJson(value);
            using(var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                bool success = db.SetAdd(key, json);
                // 设置过期时间
                if(success) {
                    if(expireTime > 0) {
                        return db.KeyExpire(key, TimeSpan.FromSeconds(expireTime));
                    }
                }
                return false;
            }
        }

       /// <summary>
       /// 获取指定索引区间内sortset集合
       /// </summary>
       /// <param name="key">key</param>
       /// <param name="start">起始下标</param>
       /// <param name="stop">结束下标</param>
       /// <returns>集合</returns>
        public List<T> Zrange<T>(string key,int start = 0, int stop = -1) {
            using(var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                // 获取集合
                var valueList = db.SortedSetRangeByRank(key, start, stop).ToList();
                // 将获取到的集合转换为List<T>
                var result = transfeToObjectList<T>(valueList);
                return result;
            }
        }


        /// <summary>
        /// 获取指定分数区间内sortset集合
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="startScore">起始分数</param>
        /// <param name="endScore">结束分数</param>
        /// <returns>集合</returns>
        public List<T> ZrangeByScore<T>(string key,int startScore, int endScore) {
            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                // 获取集合
                var valueList = db.SortedSetRangeByScore(key, startScore, endScore).ToList();
                // 将获取到的集合转换为List<T>
                var result = transfeToObjectList<T>(valueList);
                return result;
            }
        }

        /// <summary>
        /// 向sortset中添加值集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="valueList">值集合</param>
        /// <param name="scoreList">值的分数</param>
        /// <returns></returns>
        public bool Zadd<T>(string key,List<T> valueList, List<int> scoreList) {
            bool success = false;
            if (valueList.Count != scoreList.Count) return success;
            // 将valueList转换为string类型
            List<string> stringList = transfeToStringList(valueList);
            using(var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                IBatch batch = db.CreateBatch();
                for(int i = 0; i < valueList.Count; i++) {
                    batch.SortedSetAddAsync(key, stringList[i], scoreList[i]);  
                }
                batch.Execute();
                success = true;
            }
            return success;
        } 

        /// <summary>
        /// 向sortset中添加单个元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="score">score</param>
        /// <returns></returns>
        public bool Zadd<T>(string key,T value, int score) {
            bool success = false;
            string json = transfeToJson(value);

            using (var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
               success = db.SortedSetAdd(key, json, score);
            }
            return success;

        }

        /// <summary>
        /// 通过member删除sortset中的数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">member</param>
        /// <returns></returns>
        public bool Zremove(string key, string member) {
            bool success = false;
            using(var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                success = db.SortedSetRemove(key, member);
            }
            return success;
        }

        /// <summary>
        /// 根据score删除sortset中的数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="startScore">起始分数</param>
        /// <param name="endScore">结束分数</param>
        /// <returns></returns>
        public bool ZremoveByScore(string key,int startScore,int endScore) {
            bool success = false;
            using(var conn = ConnectionMultiplexer.Connect(options)) {
                IDatabase db = conn.GetDatabase();
                long count = db.SortedSetRemoveRangeByScore(key, startScore, endScore);
                if (count != 0) success = true;
            }
            return success;
        }

        /// <summary>
        /// 将json集合转换成object集合
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="stringList">json集合</param>
        /// <returns>转换后的对象集合</returns>
        private List<T> transfeToObjectList<T>(List<RedisValue> stringList) {
            List<T> objectList = new List<T>();

            try {
                for (int i = 0; i < stringList.Count; i++) {
                    string json = stringList[i].ToString();
                    objectList.Add(JsonSerializer.Deserialize<T>(json));
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return objectList;
        }

        /// <summary>
        /// 将objectList转换成stringList
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="objectList">对象集合</param>
        /// <returns>字符串集合</returns>
        private List<string> transfeToStringList<T>(List<T> objectList) {
            List<string> list = new List<string>();
            try {
                foreach (T Object in objectList) {
                    list.Add(JsonSerializer.Serialize(Object, encoder));
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return list;
        }


        /// <summary>
        /// 将对象转换成json字符串
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="Object">对象的值</param>
        /// <returns>转换后的json字符串</returns>
        private string transfeToJson<T>(T Object) {
            string json = "";
            try {
                json = JsonSerializer.Serialize(Object, encoder);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return json;
        }

        /// <summary>
        /// 将json字符串转换成对应的object对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns>转换后的object对象</returns>
        private T transfeToObject<T>(string json) {
            try {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return default(T);
        }

    }
}


