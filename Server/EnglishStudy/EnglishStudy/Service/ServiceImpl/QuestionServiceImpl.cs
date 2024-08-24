using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Utils;

namespace EnglishStudy.Service.ServiceImpl {
    public class QuestionServiceImpl : IQuestionService {

        private MyDbContext dbContext;

        private RedisHelper redisHelper = new RedisHelper();

        public QuestionServiceImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }

       
        public int AddQuestionList(List<UpdateQuestionDTO> questionList) {
            // 将UpdateQuestionDTO转换为question类型
           List<Question> list = new List<Question>();
            foreach (UpdateQuestionDTO item in questionList) {
                Question question = new Question();
                question.PassageId = item.PassageId;
                question.Title = item.Title;
                question.OptionsA = item.OptionsA;
                question.OptionsB = item.OptionsB;
                question.OptionsC = item.OptionsC;
                question.OptionsD = item.OptionsD;
                question.Answer = item.Answer;
                question.Explanation = item.Explanation;
                list.Add(question);
            }
            dbContext.AddRange(list);
            int count =  dbContext.SaveChanges();
            if (count != 0) {
                // 删除Redis中的数据
                try {
                    foreach (var item in list) {

                        redisHelper.deleteKey(MyConstant.QuestionList + "-" + item.PassageId);
                        redisHelper.deleteKey(MyConstant.AnswerList + "-" + item.PassageId);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
               
            }
            return count;
        }

        public bool DeleteById(int id) {
            // 先从MySQL中查找数据
            var question = dbContext.Questions
                .Where(item => item.QuestionId == id && item.DeleteSign == 0)
                .SingleOrDefault();
            if (question == null) return false;
            // 把删除标记位设置为1
            question.DeleteSign = 1;
            int count = dbContext.SaveChanges();
            if(count == 1) {
                // 删除Redis中的数据
                try {
                    redisHelper.deleteKey(MyConstant.QuestionList + "-" + question.PassageId);
                    redisHelper.deleteKey(MyConstant.AnswerList + "-" + question.PassageId);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
                return true;
            }
            return false;
        }

        public List<AnwserDTO> GetAnwserByPassageId(int PassageId) {
            // 先从Redis中查找
            string key = MyConstant.AnswerList + "-" + PassageId;
            if(redisHelper.exist(key)) {
                return redisHelper.getListRange<AnwserDTO>(key, 0, -1);
            }
            // 从MySQL中查找数据
            var result = dbContext.Questions
                  .Where(item => item.PassageId == PassageId && item.DeleteSign == 0)
                  .ToList();
           
            List<AnwserDTO> anwserDTOs = new List<AnwserDTO>();
            // 将result赋值给anwerDTOs，并存储到Redis中
            foreach (var item in result) {
                AnwserDTO anwserDTO = new AnwserDTO();
                QuestionDTO questionDTO = new QuestionDTO();
                anwserDTO.Answer = item.Answer;
                anwserDTO.QuestionId = item.QuestionId;
                anwserDTO.Explanation = item.Explanation;
                anwserDTO.PassageId = item.PassageId;
                anwserDTOs.Add(anwserDTO);
            }
            try {
                redisHelper.listLeftPushAll(key, anwserDTOs);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return anwserDTOs;
        }

        public List<QuestionDTO> GetQuestionListByPassageId(int id) {
            // 先从Redis中查找
            if (redisHelper.exist(MyConstant.QuestionList + "-" + id)) {
                return redisHelper.getListRange<QuestionDTO>(MyConstant.QuestionList + "-" + id, 0, -1);
            }
            // 从MySQL中查找数据
            var result = dbContext.Questions
                  .Where(item => item.PassageId == id && item.DeleteSign == 0)
                  .ToList();
            List<QuestionDTO> questionDTOs = new List<QuestionDTO>();
            List<AnwserDTO> anwserDTOs = new List<AnwserDTO>();
            // 将result赋值给questionDTOs和anwerDTOs，并存储到Redis中
            foreach (var item in result) {
                AnwserDTO anwserDTO = new AnwserDTO();
                QuestionDTO questionDTO = new QuestionDTO();
                anwserDTO.Answer = item.Answer;
                anwserDTO.QuestionId = item.QuestionId;
                anwserDTO.Explanation = item.Explanation;
                anwserDTO.PassageId = item.PassageId;

                questionDTO.QuestionId = item.QuestionId;
                questionDTO.Title = item.Title;
                questionDTO.OptionsA = item.OptionsA;
                questionDTO.OptionsB = item.OptionsB;
                questionDTO.OptionsC = item.OptionsC;
                questionDTO.OptionsD = item.OptionsD;

                questionDTOs.Add(questionDTO);
                anwserDTOs.Add(anwserDTO);
            }
            try {
                redisHelper.listLeftPushAll(MyConstant.QuestionList + "-" + id, questionDTOs);
              //  redisHelper.listLeftPushAll(MyConstant.AnswerList + "-" + id, anwserDTOs);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }

            return questionDTOs;
        }

        public bool RecoverQuestion(int id) {
            // 从MySQL中查找数据
            var question = dbContext.Questions
                .Where(item => item.QuestionId == id && item.DeleteSign == 1)
                .SingleOrDefault();
            // 数据不存在直接返回false
            if (question == null) return false;
            // 将删除标记位设置为0
            question.DeleteSign = 0;
            int count = dbContext.SaveChanges();
            if (count == 1) return true;
            return false;
        }

        public bool UpdateQuestion(UpdateQuestionDTO question) {         
            var result = dbContext.Questions
                .Where(item => item.QuestionId == question.QuestionId)
                .SingleOrDefault();
            if (result == null) return false;
            result.OptionsA = question.OptionsA;
            result.OptionsB = question.OptionsB;
            result.OptionsC = question.OptionsC;
            result.OptionsD = question.OptionsD;
            result.Answer = question.Answer;
            result.Explanation = question.Explanation;
            result.Title = question.Title;
            int count = dbContext.SaveChanges();
            // 删除Redis中数据
            try {
                redisHelper.deleteKey(MyConstant.QuestionList + "-" + question.PassageId);
                redisHelper.deleteKey(MyConstant.AnswerList +  "-" + question.PassageId);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            if(count == 1) return true;
            return false;
        }
    }
}
