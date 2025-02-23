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
    [Migration("20231217114650_English_Study_v2.2")]
    partial class English_Study_v22
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

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

            modelBuilder.Entity("EnglishStudy.Entity.Question", b =>
                {
                    b.HasOne("EnglishStudy.Entity.Passage", "Passage")
                        .WithMany("QuestionList")
                        .HasForeignKey("PassageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Passage");
                });

            modelBuilder.Entity("EnglishStudy.Entity.Passage", b =>
                {
                    b.Navigation("QuestionList");
                });
#pragma warning restore 612, 618
        }
    }
}
