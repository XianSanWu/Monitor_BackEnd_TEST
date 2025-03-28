﻿using WebAPi.Extensions;
using Serilog;
using Serilog.Events;
using Services.Interfaces;
using WebAPi.Profile;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using WebAPi.Filters;
using static Utilities.Extensions.JsonExtension;
using static WebAPi.Filters.ExceptionHandleActionFilters;
using static Utilities.Monitor.HealthCheckHelper;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()                              // This line requires the Serilog.Sinks.Console package
    .WriteTo.File("logs/Server-Init-.log",          //產生的log文字檔﹐檔名是init-log開頭
        rollingInterval: RollingInterval.Day,       //每天產新的檔案
        retainedFileCountLimit: 720                 //Log保留時間(24 hr * 30 Day=720)
    )
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// try catch.. 是 Serilog 用的
try
{
    Serilog.Log.Information("Starting web host");

    #region Serilog
    //  Read Serilog config from appsettings.json (https://blog.miniasp.com/post/2021/11/29/How-to-use-Serilog-with-NET-6)
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.With(new LogEnricher()) //客製化取log內容
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        //.Enrich.WithProperty(ThreadNameEnricher.ThreadNamePropertyName, "MyDefaultThreadId")
        .CreateLogger();

    builder.Host.UseSerilog();
    #endregion

    #region 服務註冊方法 Scrutor實現自動註冊(Service + Repository Layer)
    /*
     * Singleton：  整個應用程式(Process)只建立一個 Instance，任何時候都共用它(非常少用，類似全域變數)。
     * Scoped：     在 Request到 Response 前的處理過程執行期間共用一個 Instance，每個 Request 都會建立一個新的實例。
     * Transien：   每次要求元件時就建立一個新的，永不共用(少用，較耗資源)。
    */
    //https://code-maze.com/dotnet-dependency-injection-with-scrutor/
    // 假設所有的接口和實現都在同一個程序集或命名空間中
    var servicesAssembly = typeof(IJourneyService).Assembly;

    builder.Services.Scan(selector => selector
        .FromAssemblies(servicesAssembly) // 掃描 Services 專案的 Assembly
        .AddClasses(
        //classes => classes
        //.Where(type => type != typeof(IUnitOfWork)
        //            && type != typeof(IDbHelper)
        //            && type != typeof(DbHelper)
        //            && type != typeof(UnitOfWork))  // 排除這些類型
        )
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    );
    #endregion

    #region AutoMapper
    builder.Services.AddAutoMapper(typeof(MapperProfile));
    #endregion

    #region 客製化輸出內容
    builder.Services.AddControllers(options =>
    {
        // 格式化 Response
        options.Filters.Add<ResultWrapperFilter>();
        // 格式化 Exception
        options.Filters.Add<GlobalExceptionFilter>();

    })

    /*
     * //採用 Newtonsoft.Json (非預設 System.Text.Json)
     * .AddNewtonsoftJson(options =>
     * {
     *     //https://learn.microsoft.com/zh-tw/aspnet/core/web-api/advanced/formatting?view=aspnetcore-8.0
     *     //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
     *     options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
     *     options.SerializerSettings.Converters.Add(new IsoDateTimeConverter());
     * });
     */
    //採用 System.Text.Json
    .AddJsonOptions(j =>
    {
        //PascalCase 格式設定。
        j.JsonSerializerOptions.PropertyNamingPolicy = null;

        //日期格式化處理 (Model 的 property 要 DateTime 型別)
        j.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
    });
    #endregion

    #region CORS 設定
    var AllowMyFrontEnd = "AllowMyFrontEnd";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: AllowMyFrontEnd,
            policy =>
            {
                // policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader(); 

                var allowCors = (builder.Configuration["AppConfig:Cors"] ?? "").Split(",");
                policy.WithOrigins(allowCors).AllowAnyMethod().AllowAnyHeader();
            });
    });
    #endregion

    #region FluentValidation(驗證)
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        //停用 Model State Invalid Filter
        options.SuppressModelStateInvalidFilter = true;
    });

    //https://docs.fluentvalidation.net/en/latest/di.html
    //builder.Services.AddFluentValidationAutoValidation();
    //builder.Services.AddFluentValidationClientsideAdapters();
    //builder.Services.AddValidatorsFromAssemblyContaining<Models.Dtos.Demo.DetailRequestModelValidator>(); //指定 Model
    builder.Services.AddValidatorsFromAssembly(Assembly.Load("Models"));    //抓 Models Project dll
    #endregion

    #region 監控【HealthChecks】設定UI端點(Endpoint)分類

    #region 檢康檢查邏輯
    #region 資料庫分類
    builder.Services.AddHealthChecks()
        .AddCheck(
            "資料庫連線",
            new SqlConnectionHealthCheck((builder.Configuration["ConnectionStrings:DefaultConnection"] ?? "")),
            HealthStatus.Unhealthy,
            tags: ["db-check"]);
    #endregion
    #region API分類
    builder.Services.AddHealthChecks()
        .AddCheck<SampleHealthCheck>(
            "API方法無引數",
            failureStatus: HealthStatus.Degraded,
            tags: ["api-sample"]);

    builder.Services.AddHealthChecks()
        .AddTypeActivatedCheck<SampleHealthCheckWithArgs>(
            "API方法帶引數",
            failureStatus: HealthStatus.Degraded,
            tags: ["api-sample"],
            args: [1, "arg1"]);
    #endregion
    #endregion

    #region Healthcheck UI參數設定
    builder.Services.AddHealthChecksUI(options =>
    {
        #region UI端點分類(可由此處或appsettings.json 中 HealthChecks 設定，擇一即可，避免資料重複顯示)
        //options.AddHealthCheckEndpoint("DB", "/_db");
        //options.AddHealthCheckEndpoint("API", "/_api");    
        #endregion
        #region UI相關設定
        //options.SetEvaluationTimeInSeconds(10);                     //time in seconds between check
        //options.MaximumHistoryEntriesPerEndpoint(60);               //maximum history of checks
        //options.SetMinimumSecondsBetweenFailureNotifications(60);   //失敗通知之間的最小秒數，以避免接收器氾濫。
        #endregion
    }).AddInMemoryStorage();
    #endregion

    #endregion

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // 在中介程序中全域處理例外
    app.UseExceptionHandleMiddleware();

    #region Serilog 紀錄每一個Request的請求及過濾器
    //app.UseSerilogRequestLogging();
    // Serilog(保哥) https://gist.github.com/doggy8088/32f7c179f06ab0616b2e32728f734c5b
    app.UseSerilogRequestLogging(options =>
    {
        // 如果要自訂訊息的範本格式，可以修改這裡，但修改後並不會影響結構化記錄的屬性
        //options.MessageTemplate = "Handled {RequestPath}";

        // 使用過濾器排除特定的 Controller
        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            var path = httpContext.Request.Path.Value ?? string.Empty;

            #region 如果請求路徑包含 "xxxController"，則忽略日誌記錄 (測試沒成功)
            //if (path.Contains("/HealthReportCollector", StringComparison.OrdinalIgnoreCase))
            //{
            //    return Serilog.Events.LogEventLevel.Verbose; // 或者直接返回 `null`，表示不記錄
            //}
            //if (path.Contains("/_self", StringComparison.OrdinalIgnoreCase))
            //{
            //    return Serilog.Events.LogEventLevel.; // 或者直接返回 `null`，表示不記錄
            //}
            //if (path.Contains("/_api", StringComparison.OrdinalIgnoreCase))
            //{
            //    return Serilog.Events.LogEventLevel.Verbose; // 或者直接返回 `null`，表示不記錄
            //}
            //if (path.Contains("/_db", StringComparison.OrdinalIgnoreCase))
            //{
            //    return Serilog.Events.LogEventLevel.Verbose; // 或者直接返回 `null`，表示不記錄
            //}
            #endregion

            return Serilog.Events.LogEventLevel.Debug;
        };

        // 從 httpContext 取得 HttpContext 下所有可以取得的資訊！
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserID", (httpContext.User.Identity?.Name ?? ""));
        };

    });
    #endregion

    app.UseCors(AllowMyFrontEnd);

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    #region 監控【HealthChecks】自訂端點(Endpoint)中要顯示的內容
    /* 不要安裝 AspNetCore.Diagnostics.HealthChecks 套件(已過時)
     * 
     * ==== UseHealthChecks與MapHealthChecks ====
     * UseHealthChecks 會登錄中介軟體來處理中介軟體管線中的健康情況檢查要求。
     * MapHealthChecks 會登錄健康情況檢查端點。 端點會與應用程式中的其他端點進行比對和執行。
     **/

    //app.UseHealthChecks("/hc", new HealthCheckOptions()
    app.MapHealthChecks("/_self", new HealthCheckOptions()
    {
        //Predicate = _ => true,  //顯示全部已建立的的健康情況檢查服務
        Predicate = _ => false, //檢查應用程式(本身Web API)是否健康
                                //ResultStatusCodes = { [HealthStatus.Unhealthy] = 200 },
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/_db", new HealthCheckOptions()
    {
        //限定 tags: new string[] { "db-check" } 才出現
        Predicate = healthCheck => healthCheck.Tags.Contains("db-check"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/_api", new HealthCheckOptions()
    {
        //限定  tags: new[] { "api-sample" }, 才出現
        Predicate = healthCheck => healthCheck.Tags.Contains("api-sample"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    // HealthChecks UI 網址
    app.MapHealthChecksUI(options => options.UIPath = "/hc-ui");
    #endregion   


    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}


