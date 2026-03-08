using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Repository;
using Dotnet9.Repository.Irepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt => 
opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(s =>
{
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
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    policy.RequireRole("Admin"));
});



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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//app.MapGroup("/api").MapIdentityApi<AppUser>();

app.Run();
