using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Drive.Base.EFPlugins;
using Drive.Base.Swagger;
using Drive.Logic;
using Drive.Models.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using XWidget.Extensions;

namespace Drive {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // 使用EFCore
            services.AddDbContext<DriveContext>();

            // 加入Logic
            services.AddLogic<DriveLogicManager, DriveContext>().AddFromDbContext("Id");

            // 使用認證
            services.AddAuthentication(options => {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.IncludeErrorDetails = true;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters() {
                    IssuerSigningKey = new SymmetricSecurityKey(Configuration.GetSection("JWT:SecureKey").Value.ToHash<MD5>()),
                    ValidIssuer = Configuration.GetSection("JWT:Issuer").Value, // 驗證的發行者
                    ValidAudience = Configuration.GetSection("JWT:Audience").Value, // 驗證的TOKEN接受者

                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true, // 檢查TOKEN發行者
                    ValidateAudience = true, // 檢查該TOKEN是否發給本服務
                    ValidateLifetime = true // 檢查TOKEN是否有效
                };
            });

            // 使用MVC服務
            services.AddMvc()
                .AddJsonOptions(options => { // 設定JSON格式化選項
                    // 使用忽略LazyLoader屬性
                    options.SerializerSettings.ContractResolver = new IgnoreLazyLoaderContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            // 註冊Swagger產生器
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new Info { Title = "Drive API", Version = "v1" });

                options.AddSecurityDefinition(
                   "bearer",
                   new ApiKeyScheme() {
                       In = "header",
                       Description = "請輸入Bearer類型的JWT在此欄位",
                       Name = "Authorization",
                       Type = "apiKey",
                   });

                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>() {
                    ["bearer"] = new string[] { },
                    ["basic"] = new string[] { }
                });
                foreach (var file in Directory.GetFiles(
                    PlatformServices.Default.Application.ApplicationBasePath,
                    "*.xml")) {
                    options.IncludeXmlComments(file);
                }

                options.OperationFilter<FormFileOperationFilter>();
                options.OperationFilter<AuthorizeOperationFilter>();
            });

            // 設定SPA根目錄
            services.AddSpaStaticFiles(options => {
                options.RootPath = "./wwwroot";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, DriveLogicManager logicManager) {
            var createScript = logicManager.Database.Database.GenerateCreateScript();
            try {
                await logicManager.Database.Database.ExecuteSqlCommandAsync(createScript);
            } catch { }

            //logicManager.Database.Database.ExecuteSqlCommand(createScript);
            #region Init Default User
            if (logicManager.UserLogic.List().Count() == 0) {
                logicManager.UserLogic.Create(new User() {
                    Id = "admin",
                    IsAdmin = true
                }.Process(x => {
                    x.SetPassword("admin");
                }));
            }
            #endregion

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // 使用認證
            app.UseAuthentication();

            // 使用MVC
            app.UseMvc();

            // 使用Swagger
            app.UseSwagger();

            // 使用Swagger UI
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Drive API");
            });

            // 使用靜態檔案
            app.UseStaticFiles();

            // 使用SPA
            app.UseSpaStaticFiles();

            // SPA例外處理
            app.Use(async (context, next) => {
                try {
                    await next();
                } catch (Exception e) {
                    if (e is InvalidOperationException && e.Message.Contains("/index.html")) {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                }
            });

            // SPA設定
            app.UseSpa(c => { });
        }
    }
}
