using EnglishStudy.DTO.Comment;
using EnglishStudy.Entity;
using EnglishStudy.Utils;

namespace EnglishStudy.Service.ServiceImpl {
    public class CommentServiceImpl : ICommentService {

        private MyDbContext dbContext;

      

        public CommentServiceImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }

        // 添加评论
        public int AddComment(Comment comment) {
            int length = comment.Content.Length;
            // 评论字数超标
            if (length > 450)
                return -1;
            // 设置时间
            comment.Time = DateTime.Now;
            // 如果此评论是一级评论或者回复的是自己的评论
            if((comment.ReplyUserId == 0 && comment.ReplyCommentId == 0)
                || (comment.UserId == comment.ReplyUserId)) {
                // 直接插入到数据库中
                dbContext.Comments.Add(comment);
                int count = dbContext.SaveChanges();
                if(count == 0) {
                    return 0;
                }
                return comment.CommentId;
            }
                // 回复的是别人的评论
                else {
                    // 插入到comment和commentRecive表中
                    dbContext.Comments.Add(comment);
                    int count = dbContext.SaveChanges();
                    if(count != 0) {
                        CommentRecive commentRecive = new CommentRecive();
                        commentRecive.CommentId = comment.CommentId;
                        commentRecive.UserId = comment.ReplyUserId;
                        commentRecive.Time = DateTime.Now;
                        dbContext.Add(commentRecive);
                        dbContext.SaveChanges();
                        return comment.CommentId;
                    }
                    return 0;
                }
        }

        // 删除评论
        public bool DeleteComment(int userId, int commentId) {
            // 查找数据是否存在
           var result =  dbContext.Comments
                .Where(item => item.UserId == userId 
                && item.CommentId == commentId && item.DeleteSign == 0)
                .SingleOrDefault();
            // 不存在这条数据
            if(result == null) {
                return false;
            }
            // 将删除标记位设置为0
            result.DeleteSign = 1;
            int count = dbContext.SaveChanges();
            if (count != 0)
            {
                return true;
            }
            return false;
        }

        public int FirstCommentTotal() {
            // 一级评论即replyCommentId为0
            int total = dbContext.Comments
                .Where(item => item.ReplyCommentId == 0 && item.DeleteSign == 0)
                .Count();
            return total;
        }

        // 分页获取评论包括一级评论和二级评论，主要是一级评论,二级评论默认获取第一页的size个数据
        public FirstCommentPage GetComment(int page, int size) {
            // 先获取分页一级评论
            FirstCommentPage firstCommentPage = GetFirstCommentPage(page, size);
            if(firstCommentPage.Total == 0) {
                return firstCommentPage;
            }
            // 然后for循环获取一级评论中的二级评论
            for(int i = 0; i < firstCommentPage.FirstLeverCommentDTOList.Count; i++) {
                FirstLeverCommentDTO firstLeverCommentDTO = firstCommentPage.FirstLeverCommentDTOList[i];
                SecondCommentPage secondCommentPage = GetSecondCommentPage(firstLeverCommentDTO.CommentId, 1, size);
                firstLeverCommentDTO.SecondCommentPage = secondCommentPage;
            }
            return firstCommentPage;
        }

        // 分页获取一级评论，不包括二级评论
        public FirstCommentPage GetFirstCommentPage(int page, int size) {
            FirstCommentPage firstCommentPage = new FirstCommentPage();
            // 先获取总数
            int total = FirstCommentTotal();
            if(total == 0) {
                return firstCommentPage;
            }
            firstCommentPage.Total = total;
            // 获取一级评论的内容，先查comment表，再查user表

            // 首先按照置顶评论排序，然后按照时间降序排序
            var sqlResult = (from newCommentTable in (
             from comment in dbContext.Comments
             where comment.ReplyCommentId == 0 && comment.DeleteSign == 0
             orderby comment.Top descending, comment.Time descending
             select new {
                 comment.CommentId,
                 comment.UserId,
                 comment.Content,
                 comment.Time,
                 comment.Top
             })
             .Skip((page - 1) * size)
             .Take(size)
             join user in dbContext.Users
             on newCommentTable.UserId equals user.UserId
             select new {
                 newCommentTable.CommentId,
                 newCommentTable.UserId,
                 newCommentTable.Content,
                 newCommentTable.Time,
                 newCommentTable.Top,
                 user.NickName,
                 user.Status,
                 user.Image
             })
             .ToList();
            List<FirstLeverCommentDTO> firstLeverCommentDTOList = new List<FirstLeverCommentDTO>();
            // 将获取到sql数据装载到list中
            for(int i = 0; i < sqlResult.Count; i++) {
                FirstLeverCommentDTO firstLeverCommentDTO = new FirstLeverCommentDTO();
                firstLeverCommentDTO.CommentId = sqlResult[i].CommentId;
                firstLeverCommentDTO.UserId = sqlResult[i].UserId;
                firstLeverCommentDTO.NickName = sqlResult[i].NickName;
                firstLeverCommentDTO.Image = sqlResult[i].Image;
                firstLeverCommentDTO.Status = sqlResult[i].Status;
                firstLeverCommentDTO.Content = sqlResult[i].Content;
                firstLeverCommentDTO.Time = sqlResult[i].Time;
                firstLeverCommentDTO.Top = sqlResult[i].Top;

                firstLeverCommentDTOList.Add(firstLeverCommentDTO);
            }
            firstCommentPage.FirstLeverCommentDTOList = firstLeverCommentDTOList;
            return firstCommentPage;

        }

        // 根据一级评论id分页获取二级评论
        public SecondCommentPage GetSecondCommentPage(int replyCommentId,int page,int size) {
            SecondCommentPage secondCommentPage = new SecondCommentPage();
            // 获取二级评论的总条数
            int total = SecondCommentTotal(replyCommentId);
            if(total == 0) {
                return secondCommentPage;
            }
            secondCommentPage.Total = total;
            // 获取根据一级评论id获取二级评论的基本内容
            var commentResult = dbContext.Comments
                .Where(item => item.ReplyCommentId == replyCommentId && item.DeleteSign == 0)
                .Select(item => new {
                    item.CommentId,
                    item.ReplyCommentId,
                    item.UserId,
                    item.ReplyUserId,
                    item.Content,
                    item.Time
                })
                .OrderByDescending(item => item.Time)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
            List<SecondCommentDTO> secondCommentDTOList = new List<SecondCommentDTO>();
            // 根据获取到的userId获取昵称
            for(int i = 0; i < commentResult.Count; i++) {
                var user = dbContext.Users
                    .Where(item => item.UserId == commentResult[i].UserId)
                    .Select(item => new {
                        item.NickName,
                        item.Status,
                        item.Image
                    })
                    .SingleOrDefault();
                var replyUser = dbContext.Users
                    .Where(item => item.UserId == commentResult[i].ReplyUserId)
                    .Select(item =>new {
                        item.NickName,
                        item.Image
                    })
                    .SingleOrDefault();
                // 将获取到的数据封装到SecondCommentDTO中
                SecondCommentDTO secondCommentDTO = new SecondCommentDTO();
                secondCommentDTO.CommentId = commentResult[i].CommentId;
                secondCommentDTO.ReplyCommentId = commentResult[i].ReplyCommentId;
                secondCommentDTO.UserId = commentResult[i].UserId;
                secondCommentDTO.ReplyUserId = commentResult[i].ReplyUserId;
                secondCommentDTO.NickName = user.NickName;
                secondCommentDTO.ReplyNickName = replyUser.NickName;
                secondCommentDTO.Image = user.Image;
                secondCommentDTO.ReplyImage = replyUser.Image;
                secondCommentDTO.Status = user.Status;
                secondCommentDTO.Content = commentResult[i].Content;
                secondCommentDTO.Time = commentResult[i].Time;

                // 将DTO添加到list
                secondCommentDTOList.Add(secondCommentDTO);
            }
            // 将list添加到page中
            secondCommentPage.SecondCommentList = secondCommentDTOList;
            return secondCommentPage;
        }

        // 获取被评论的具体内容
        public ReplyCommentDetail GetReplyCommentDetail(int userId, int commentReciveId, int commentId) {
           ReplyCommentDetail replyCommentDetail = new ReplyCommentDetail();
            // 获取被回复的详细内容
            var result = dbContext.Comments
                 .Where(item => item.CommentId == commentId)
                 .SingleOrDefault();
            // 获取回复对象的昵称、头像
            var user = dbContext.Users
                .Where(item => item.UserId == result.UserId)
                .SingleOrDefault();
            replyCommentDetail.CommentId = commentId;
            replyCommentDetail.ReplyCommentId = result.ReplyCommentId;
            replyCommentDetail.UserId = result.UserId;
            replyCommentDetail.Image = user.Image;
            replyCommentDetail.NickName = user.NickName;
            replyCommentDetail.Content = result.Content;
            replyCommentDetail.Time = result.Time;
            replyCommentDetail.CommentReciveId = commentReciveId;
            return replyCommentDetail;
        }

        // 分页获取被回复的消息
        public ReplyCommentDetailPage GetReplyCommentDetailByPage(int userId, int page, int size) {
            ReplyCommentDetailPage replyCommentDetailPage = new ReplyCommentDetailPage();
            var sqlResult = dbContext.CommentRecives
                .Where(item => item.UserId == userId)
                .OrderByDescending(item => item.CommentReciveId)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            if(sqlResult.Count == 0) {
                return replyCommentDetailPage;
            }
            // 循环查找被回复的具体内容
            for(int i = 0; i <  sqlResult.Count; i++) {
                var item = sqlResult[i];
                replyCommentDetailPage.ReplyComments
                    .Add(GetReplyCommentDetail(item.UserId, item.CommentReciveId, item.CommentId));
            }
            return replyCommentDetailPage;

        }

        public int SecondCommentTotal(int replyCommentId) {
            int total = dbContext.Comments
                .Where(item => item.ReplyCommentId == replyCommentId)
                .Count();
            return total;
        }

        public bool SetTopOrNot(int commentId, bool top) {
            var result = dbContext.Comments
                .Where(item => item.CommentId == commentId && item.DeleteSign == 0)
                .SingleOrDefault();
            result.Top = !top;
            int count = dbContext.SaveChanges();
            if (count != 0) return true;
            return false;
        }

        public bool SetRead(int commentReciveId, int userId) {
            var result = dbContext.CommentRecives
                .Where(item => item.CommentReciveId == commentReciveId
                && item.UserId == userId && item.Read == 0)
                .SingleOrDefault();
            if (result == null) return false;
            result.Read = 1;
            result.Time = DateTime.Now;
            int count = dbContext.SaveChanges();
            if (count != 0)
            {
                return true;
            }
            return false;
        }

        public bool SetAllRead(int userId) {
            var list = dbContext.CommentRecives
                .Where(item => item.UserId == userId && item.Read == 0)
                .ToList();
            for(int i = 0; i < list.Count; i++) {
                list[i].Read = 1;
                list[i].Time = DateTime.Now;
            }
           int count =  dbContext.SaveChanges();
            if (count != 0) return true;
            return false;
        }

        public int GetUnReadCount(int userId) {
           int total =  dbContext.CommentRecives
                .Where(item => item.UserId == userId && item.Read == 0)
                .Count();
            return total;
        }


        public FirstLeverCommentDTO GetFirstLeverComment(int commentId) {
            FirstLeverCommentDTO firstLeverCommentDTO = new FirstLeverCommentDTO();
            // 先查找一级评论是否被删除
            var first =  dbContext.Comments
                .Where(item => item.CommentId == commentId && item.DeleteSign == 0)
                .SingleOrDefault();
            if(first == null) {
                return firstLeverCommentDTO;
            }
            // 查找发布这条评论的用户
            int userId = first.UserId;
            var user =  dbContext.Users
                .Where(item => item.UserId == userId)
                .SingleOrDefault();
            if(user == null) {
                return firstLeverCommentDTO;
            }
            firstLeverCommentDTO.CommentId = first.CommentId;
            firstLeverCommentDTO.UserId = user.UserId;
            firstLeverCommentDTO.NickName = user.NickName;
            firstLeverCommentDTO.Image = user.Image;
            firstLeverCommentDTO.Status = user.Status;
            firstLeverCommentDTO.Content = first.Content;
            firstLeverCommentDTO.Time = first.Time;
            firstLeverCommentDTO.Top = first.Top;
            return firstLeverCommentDTO;
        }
    }
}
