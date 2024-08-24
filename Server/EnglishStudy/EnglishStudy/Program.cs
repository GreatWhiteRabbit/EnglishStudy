using EnglishStudy;
using EnglishStudy.Authorization;
using EnglishStudy.Entity;
using EnglishStudy.Quartz;
using EnglishStudy.Service;
using EnglishStudy.Service.ServiceImpl;
using EnglishStudy.SignalR;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


var configuration = builder.Configuration;

// ��ȡ�����ļ�
var connectionString = configuration["DBconnection"];
var version = new MySqlServerVersion(new Version(8, 0, 28));
builder.Services.AddDbContext<MyDbContext>(opts =>
    opts.UseMySql(connectionString, version)
);

// ע��JWT
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters() {
        ValidateIssuer = true, //�Ƿ���֤Issuer
        ValidIssuer = configuration["JWT:Issuer"], //������Issuer
        ValidateAudience = true, //�Ƿ���֤Audience
        ValidAudience = configuration["JWT:Audience"], //������Audience
        ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])), //SecurityKey
        ValidateLifetime = true, //�Ƿ���֤ʧЧʱ��
        ClockSkew = TimeSpan.FromSeconds(30), //����ʱ���ݴ�ֵ�������������ʱ�䲻ͬ�����⣨�룩
        RequireExpirationTime = true,
    };
});

// ע�����
builder.Services.AddScoped<IWordService, WordServiceImpl>();
builder.Services.AddScoped<IUserService, UserServiceImpl>();
builder.Services.AddScoped<IPassageService, PassageServiceImpl>();
builder.Services.AddScoped<IQuestionService, QuestionServiceImpl>();
builder.Services.AddScoped<IEveryDayEnglish,EveryDayEnglishImpl>();
builder.Services.AddScoped<IEveryDayEnglishRecord, EveryDayEnglishRecordImpl>();
builder.Services.AddScoped<IPassageRecordService, PassageRecordServiceImpl>();
builder.Services.AddScoped<IListeningPaperService,ListeningPaperServiceImpl>();
builder.Services.AddScoped<IListeningPaperRecordService, ListeningPaperRecordServiceImpl>();
builder.Services.AddScoped<IWordRecordService, WordRecordServiceImpl>();
builder.Services.AddScoped<IMessageService, MessageServiceImpl>();
builder.Services.AddScoped<ICommentService, CommentServiceImpl>();
builder.Services.AddScoped<IMyResourceService, MyResourceServiceImpl>();
builder.Services.AddSingleton(new JWTHelper(configuration));

// ע�붨ʱ����
builder.Services.AddSingleton<ISchedulerFactory,StdSchedulerFactory>();
builder.Services.AddSingleton<QuartzFactory>();
builder.Services.AddScoped<PullMessageJob>();
builder.Services.AddSingleton<IJobFactory, IOCJobFactory>();

// ע��SignalR����
builder.Services.AddSignalR();

// �޸��ļ��ϴ���С
builder.Services.Configure<FormOptions>(options => {
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});


// ��Ȩ
builder.Services.AddSingleton<IAuthorizationHandler, StatusAuthorizationHandler>();
builder.Services.AddAuthorization(options => {
    options.AddPolicy(MyConstant.Admin, policy => policy.AddRequirements(new StatusAuthorizationRequirement("1")));
    options.AddPolicy(MyConstant.User, policy => policy.AddRequirements(new StatusAuthorizationRequirement("2")));
    options.AddPolicy(MyConstant.UserOrAdmin, policy => policy.AddRequirements(new StatusAuthorizationRequirement("1,2")));
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();

// ��Ҫ���û���Ȩ����Щ������Ҫ�������Ա��һ���û�
app.UseAuthorization();


// ����quartz
var quartz = app.Services.GetRequiredService<QuartzFactory>();
//var quartz = app.Services.CreateScope().ServiceProvider.GetService<QuartzFactory>();
app.Lifetime.ApplicationStarted.Register(async () => {
    await quartz.Start();
});


string savaPath = @"D:\nginx";
string accessPath = @"/resouce";

// ����ļ�����·��
app.UseStaticFiles(new StaticFileOptions() {
    FileProvider = new PhysicalFileProvider(savaPath), // �ļ����·��
    RequestPath = new PathString(accessPath) // ������·��
}
);

app.MapControllers();



// ����hub
app.MapHub<MessageHub>("/sys_msg");

app.Run();
