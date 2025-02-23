﻿// <auto-generated />
using System;
using EnglishStudy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EnglishStudy.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20240308054429_Add_PropertyTop_CommentTable")]
    partial class Add_PropertyTop_CommentTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EnglishStudy.Entity.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("comment_id")
                        .HasComment("主键评论id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("varchar(500)")
                        .HasColumnName("content")
                        .HasComment("评论内容");

                    b.Property<int>("DeleteSign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<int>("ReplyCommentId")
                        .HasColumnType("int")
                        .HasColumnName("reply_comment_id")
                        .HasComment("被回复的一级评论id");

                    b.Property<int>("ReplyUserId")
                        .HasColumnType("int")
                        .HasColumnName("reply_user_id")
                        .HasComment("被回复的用户id");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp")
                        .HasColumnName("time")
                        .HasComment("评论时间");

                    b.Property<bool>("Top")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("top")
                        .HasComment("是否置顶");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.HasKey("CommentId");

                    b.ToTable("comment", t =>
                        {
                            t.HasComment("评论表");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.CommentRecive", b =>
                {
                    b.Property<int>("CommentReciveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("comment_recive_id")
                        .HasComment("主键，评论回复接收id");

                    b.Property<int>("CommentId")
                        .HasColumnType("int")
                        .HasColumnName("comment_id")
                        .HasComment("评论的id");

                    b.Property<int>("Read")
                        .HasColumnType("int")
                        .HasColumnName("read")
                        .HasComment("是否已读");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp")
                        .HasColumnName("time")
                        .HasComment("读取时间");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.HasKey("CommentReciveId");

                    b.ToTable("comment_recive", t =>
                        {
                            t.HasComment("评论接收表");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.EveryDayEnglish", b =>
                {
                    b.Property<int>("Everyday_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("everyday_id")
                        .HasComment("每日英语主键");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content")
                        .HasComment("内容");

                    b.Property<int>("Delete_Sign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标志位");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp")
                        .HasColumnName("time")
                        .HasComment("创建时间");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("title")
                        .HasComment("标题");

                    b.Property<string>("TitleTranslations")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("title_translations")
                        .HasComment("标题翻译");

                    b.Property<string>("Translations")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("translations")
                        .HasComment("内容翻译");

                    b.HasKey("Everyday_id");

                    b.ToTable("everyday_english");
                });

            modelBuilder.Entity("EnglishStudy.Entity.EveryDayEnglishRecord", b =>
                {
                    b.Property<int>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("record_id")
                        .HasComment("每日阅读记录id");

                    b.Property<int>("DeleteSign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<int>("EverydayId")
                        .HasColumnType("int")
                        .HasColumnName("everyday_id")
                        .HasComment("每日英语id");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp")
                        .HasColumnName("time")
                        .HasComment("阅读时间");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.HasKey("RecordId");

                    b.ToTable("en_record", t =>
                        {
                            t.HasComment("每日英语阅读记录");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.ListeningPaper", b =>
                {
                    b.Property<int>("ListeningPaperId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("listening_paper_id")
                        .HasComment("听力试题id");

                    b.Property<string>("Audio")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("audio")
                        .HasComment("听力音频URL");

                    b.Property<int>("DeleteSign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<string>("PaperTitle")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("papre_title")
                        .HasComment("听力试题标题");

                    b.HasKey("ListeningPaperId");

                    b.ToTable("listening_paper", t =>
                        {
                            t.HasComment("听力试题表");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.ListeningPaperRecord", b =>
                {
                    b.Property<int>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("record_id")
                        .HasComment("记录id");

                    b.Property<double>("Accuracy")
                        .HasColumnType("double")
                        .HasColumnName("accuracy")
                        .HasComment("准确度");

                    b.Property<int>("DeleteSign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<string>("Detail")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("detail")
                        .HasComment("做题详细记录");

                    b.Property<int>("ListeningPaperId")
                        .HasColumnType("int")
                        .HasColumnName("listeningpaper_id")
                        .HasComment("听力试题id");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp")
                        .HasColumnName("time")
                        .HasComment("创建时间");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.HasKey("RecordId");

                    b.ToTable("listeningpaper_record", t =>
                        {
                            t.HasComment("听力试题记录");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.ListeningQuestion", b =>
                {
                    b.Property<int>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("question_id")
                        .HasComment("听力小题题号");

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasColumnName("answer")
                        .HasComment("答案");

                    b.Property<int>("DeleteSign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<string>("OptionsA")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("options_a")
                        .HasComment("A选项");

                    b.Property<string>("OptionsB")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("options_b")
                        .HasComment("B选项");

                    b.Property<string>("OptionsC")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("options_c")
                        .HasComment("C选项");

                    b.Property<string>("OptionsD")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("options_d")
                        .HasComment("D选项");

                    b.Property<int>("PartId")
                        .HasColumnType("int")
                        .HasColumnName("part_id")
                        .HasComment("听力段号");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("title")
                        .HasComment("题目内容");

                    b.HasKey("QuestionId");

                    b.HasIndex("PartId");

                    b.ToTable("listening_question");
                });

            modelBuilder.Entity("EnglishStudy.Entity.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("message_id")
                        .HasComment("主键id");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp")
                        .HasColumnName("time")
                        .HasComment("发布时间");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("title")
                        .HasComment("消息标题");

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("varchar(400)")
                        .HasColumnName("content")
                        .HasComment("消息内容");

                    b.HasKey("MessageId");

                    b.ToTable("message", t =>
                        {
                            t.HasComment("系统消息表");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.Part", b =>
                {
                    b.Property<int>("PartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("part_id")
                        .HasComment("听力小题部分");

                    b.Property<int>("DeleteSign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<int>("ListeningPaperId")
                        .HasColumnType("int")
                        .HasColumnName("listening_paper_id");

                    b.Property<string>("OriginalText")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("original_text")
                        .HasComment("听力原文");

                    b.Property<string>("PartTitle")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("part_title")
                        .HasComment("题目");

                    b.HasKey("PartId");

                    b.HasIndex("ListeningPaperId");

                    b.ToTable("part", t =>
                        {
                            t.HasComment("听力段部分");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.Passage", b =>
                {
                    b.Property<int>("PassageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("passage_id")
                        .HasComment("文章ID");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content")
                        .HasComment("文章内容");

                    b.Property<int>("DeleteSign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("title")
                        .HasComment("文章标题");

                    b.HasKey("PassageId");

                    b.ToTable("passage");
                });

            modelBuilder.Entity("EnglishStudy.Entity.PassageRecord", b =>
                {
                    b.Property<int>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("record_id")
                        .HasComment("记录id");

                    b.Property<double>("Accuracy")
                        .HasColumnType("double")
                        .HasColumnName("accuracy")
                        .HasComment("准确度");

                    b.Property<int>("DeleteSign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<string>("Detail")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("detail")
                        .HasComment("做题详细记录");

                    b.Property<int>("PassageId")
                        .HasColumnType("int")
                        .HasColumnName("passage_id")
                        .HasComment("阅读理解id");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp")
                        .HasColumnName("time")
                        .HasComment("创建时间");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.HasKey("RecordId");

                    b.ToTable("passage_record", t =>
                        {
                            t.HasComment("阅读理解做题记录");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.Question", b =>
                {
                    b.Property<int>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("question_id")
                        .HasComment("问题ID");

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasColumnName("answer")
                        .HasComment("正确答案");

                    b.Property<int>("DeleteSign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<string>("Explanation")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("explanation")
                        .HasComment("答案解释");

                    b.Property<string>("OptionsA")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("options_a")
                        .HasComment("A选项");

                    b.Property<string>("OptionsB")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("options_b")
                        .HasComment("B选项");

                    b.Property<string>("OptionsC")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("options_c")
                        .HasComment("C选项");

                    b.Property<string>("OptionsD")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("options_d")
                        .HasComment("D选项");

                    b.Property<int>("PassageId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("title")
                        .HasComment("具体问题");

                    b.HasKey("QuestionId");

                    b.HasIndex("PassageId");

                    b.ToTable("question");
                });

            modelBuilder.Entity("EnglishStudy.Entity.Receive", b =>
                {
                    b.Property<int>("ReceiveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("receive_id")
                        .HasComment("主键Id");

                    b.Property<int>("MessageId")
                        .HasColumnType("int")
                        .HasColumnName("message_id")
                        .HasComment("消息id");

                    b.Property<int>("Read")
                        .HasColumnType("int")
                        .HasColumnName("read")
                        .HasComment("消息是否已读");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp")
                        .HasColumnName("time")
                        .HasComment("消息读取时间");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.HasKey("ReceiveId");

                    b.ToTable("receive", t =>
                        {
                            t.HasComment("消息接收表");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("user_id")
                        .HasComment("主键id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("email")
                        .HasComment("邮箱");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("image")
                        .HasComment("头像");

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("nickname")
                        .HasComment("昵称");

                    b.Property<string>("PassWord")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("password")
                        .HasComment("密码");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status")
                        .HasComment("级别(管理员或普通用户)");

                    b.HasKey("UserId");

                    b.ToTable("user");
                });

            modelBuilder.Entity("EnglishStudy.Entity.Word", b =>
                {
                    b.Property<int>("WordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("word_id")
                        .HasComment("主键id");

                    b.Property<string>("Paraphrase")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("varchar(300)")
                        .HasColumnName("paraphrase")
                        .HasComment("中文释义");

                    b.Property<string>("Phonetic")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("varchar(70)")
                        .HasColumnName("phonetic")
                        .HasComment("音标");

                    b.Property<int>("Type")
                        .HasColumnType("int")
                        .HasColumnName("type")
                        .HasComment("词汇类型(四六级)");

                    b.Property<string>("Words")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("word")
                        .HasComment("英语单词");

                    b.HasKey("WordId");

                    b.ToTable("words");
                });

            modelBuilder.Entity("EnglishStudy.Entity.WordPosition", b =>
                {
                    b.Property<int>("PositionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("position_id")
                        .HasComment("主键");

                    b.Property<int>("DeleteSign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<int>("LastWordId")
                        .HasColumnType("int")
                        .HasColumnName("lastword_id")
                        .HasComment("上一次最后记忆的单词id");

                    b.Property<int>("Type")
                        .HasColumnType("int")
                        .HasColumnName("type")
                        .HasComment("单词类型");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.HasKey("PositionId");

                    b.ToTable("word_position", t =>
                        {
                            t.HasComment("单词记忆位置");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.WordRecord", b =>
                {
                    b.Property<int>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("record_id")
                        .HasComment("记录id");

                    b.Property<int>("Delete_sign")
                        .HasColumnType("int")
                        .HasColumnName("delete_sign")
                        .HasComment("删除标记位");

                    b.Property<string>("Detail")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("detail")
                        .HasComment("详细记录");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp")
                        .HasColumnName("time")
                        .HasComment("创建时间");

                    b.Property<int>("Total")
                        .HasColumnType("int")
                        .HasColumnName("total")
                        .HasComment("记忆单词总数");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.Property<int>("WordType")
                        .HasColumnType("int")
                        .HasColumnName("word_type")
                        .HasComment("单词类型");

                    b.HasKey("RecordId");

                    b.ToTable("word_record", t =>
                        {
                            t.HasComment("记忆单词详细记录");
                        });
                });

            modelBuilder.Entity("EnglishStudy.Entity.ListeningQuestion", b =>
                {
                    b.HasOne("EnglishStudy.Entity.Part", null)
                        .WithMany("ListeningQuestionList")
                        .HasForeignKey("PartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EnglishStudy.Entity.Part", b =>
                {
                    b.HasOne("EnglishStudy.Entity.ListeningPaper", null)
                        .WithMany("PartList")
                        .HasForeignKey("ListeningPaperId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EnglishStudy.Entity.Question", b =>
                {
                    b.HasOne("EnglishStudy.Entity.Passage", null)
                        .WithMany("QuestionList")
                        .HasForeignKey("PassageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EnglishStudy.Entity.ListeningPaper", b =>
                {
                    b.Navigation("PartList");
                });

            modelBuilder.Entity("EnglishStudy.Entity.Part", b =>
                {
                    b.Navigation("ListeningQuestionList");
                });

            modelBuilder.Entity("EnglishStudy.Entity.Passage", b =>
                {
                    b.Navigation("QuestionList");
                });
#pragma warning restore 612, 618
        }
    }
}
