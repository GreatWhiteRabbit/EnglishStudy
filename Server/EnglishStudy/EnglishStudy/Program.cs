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

// 读取配置文件
var connectionString = configuration["DBconnection"];
var version = new MySqlServerVersion(new Version(8, 0, 28));
builder.Services.AddDbContext<MyDbContext>(opts =>
    opts.UseMySql(connectionString, version)
);

// 注册JWT
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters() {
        ValidateIssuer = true, //是否验证Issuer
        ValidIssuer = configuration["JWT:Issuer"], //发行人Issuer
        ValidateAudience = true, //是否验证Audience
        ValidAudience = configuration["JWT:Audience"], //订阅人Audience
        ValidateIssuerSigningKey = true, //是否验证SecurityKey
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])), //SecurityKey
        ValidateLifetime = true, //是否验证失效时间
        ClockSkew = TimeSpan.FromSeconds(30), //过期时间容错值，解决服务器端时间不同步问题（秒）
        RequireExpirationTime = true,
    };
});

// 注册服务
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

// 注入定时任务
builder.Services.AddSingleton<ISchedulerFactory,StdSchedulerFactory>();
builder.Services.AddSingleton<QuartzFactory>();
builder.Services.AddScoped<PullMessageJob>();
builder.Services.AddSingleton<IJobFactory, IOCJobFactory>();

// 注入SignalR服务
builder.Services.AddSignalR();

// 修改文件上传大小
builder.Services.Configure<FormOptions>(options => {
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});


// 授权
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

// 需要做用户授权，有些操作需要区别管理员和一般用户
app.UseAuthorization();


// 启动quartz
var quartz = app.Services.GetRequiredService<QuartzFactory>();
//var quartz = app.Services.CreateScope().ServiceProvider.GetService<QuartzFactory>();
app.Lifetime.ApplicationStarted.Register(async () => {
    await quartz.Start();
});


string savaPath = @"D:\nginx";
string accessPath = @"/resouce";

// 添加文件访问路径
app.UseStaticFiles(new StaticFileOptions() {
    FileProvider = new PhysicalFileProvider(savaPath), // 文件存放路径
    RequestPath = new PathString(accessPath) // 外界访问路径
}
);

app.MapControllers();



// 配置hub
app.MapHub<MessageHub>("/sys_msg");

app.Run();
