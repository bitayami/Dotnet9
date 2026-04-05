using Asp.Versioning;
using Dotnet9.Data;
using Dotnet9.Middleware;
using Dotnet9.Models;
using Dotnet9.Repository;
using Dotnet9.Repository.Irepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Scalar.AspNetCore;
using Serilog;
using System.Security.Claims;
using System.Text;



var logger = LogManager.Setup()
    .LoadConfigurationFromFile("nlog.config")
    .GetCurrentClassLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builtin logger
builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();


//serilog
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .Enrich.FromLogContext()
//    .Enrich.WithMachineName()
//    .Enrich.WithThreadId()
//    .Enrich.WithProcessId()
//    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} -{Message:lj}{NewLine}{Exception}")
//    .WriteTo.File(
//        path: "Serilogs/dotnet9-api-.log",
//        rollingInterval: RollingInterval.Day,
//        retainedFileCountLimit: 7,
//        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext} -{Message:lj}{NewLine}{Exception}"
//    ).CreateLogger();

//builder.Host.UseSerilog();

builder.Host.UseNLog();

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;

    o.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),// api/v1/courses
        new HeaderApiVersionReader("x-api-version"), //header
        new QueryStringApiVersionReader("api-version") //?api-version = 1.0
        );
}).AddApiExplorer(a =>
{
    a.GroupNameFormat = "'v'VVV"; //v1,v2
    a.SubstituteApiVersionInUrl = true;
});


builder.Services.AddControllers().AddJsonOptions(opt => 
opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dotnet9 Api v1",
        Version = "v1"
    });

    s.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Dotnet9 Api v1",
        Version = "v2"
    });
    s.AddSecurityDefinition("Bearer", (new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    }));

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });

});

var connstr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connstr));

builder.Services.AddTransient<ITransient, Transient>();
builder.Services.AddScoped<IScoped, Scoped>();
builder.Services.AddSingleton<ISingleton, Singleton>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
        options.DefaultChallengeScheme =
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(a =>
    {
        a.SaveToken = false;
        a.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["jwt:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["jwt:Key"]!)),

            RoleClaimType = ClaimTypes.Role
        };

        a.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var response = new ProblemDetails
                {
                    Status = 401,
                    Title = "Unauthorized",
                    Detail = "mall owner dont have access",
                    Instance = context.HttpContext.Request.Path
                };

                response.Extensions["traceId"] =
                    context.HttpContext.TraceIdentifier;

                await context.Response.WriteAsJsonAsync(response);
            },

            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var problemDetails = new ProblemDetails
                {
                    Status = 403,
                    Title = "Forbidden",
                    Detail = "You do not have permission to access this resource",
                    Instance = context.HttpContext.Request.Path
                };

                problemDetails.Extensions["traceId"] =
                    context.HttpContext.TraceIdentifier;

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    policy.RequireRole("Admin"));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapScalarApiReference();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region CORS
app.UseCors();
#endregion

//app.UseMiddleware<ExceptionMiddleware>();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

//app.UseSerilogRequestLogging();//automatically creates log for all http req

app.MapControllers();

//app.MapGroup("/api").MapIdentityApi<AppUser>();

//serilog
try
{
    app.Run();
}
catch(Exception ex)
{
    //Log.Fatal(ex, "Application terminated");
    logger.Fatal(ex, "Application terminated unexpectely");
}
finally
{
    //Log.CloseAndFlush();
    LogManager.Shutdown();
}