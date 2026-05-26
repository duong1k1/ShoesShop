using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoesShop.Application;
using ShoesShop.Application.Interfaces.Services;

using ShoesShop.Application.Services;

// Lưu ý: Thay đổi namespace dưới này cho đúng với folder chứa DbContext của nhóm bạn nếu bị báo đỏ
using ShoesShop.Infrastructure.Data;
using ShoesShop.Infrastructure.Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Nạp toàn bộ Service (AuthService, UserService, VoucherService,...) từ tầng Application của bạn
builder.Services.AddApplicationServices();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? "Key_Bao_Mat_Sieu_Cap_Cua_Shoes_Shop_2026"))
        };
    });


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShoesShop API", Version = "v1" });

    // Cấu hình nút khóa bảo mật JWT ngay trên giao diện Swagger UI để test API cần đăng nhập
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Điền Token của bạn theo cấu trúc: Bearer [chuỗi_token_của_bạn]",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoesShop API v1"));
}

app.UseHttpsRedirection();

// LƯU Ý: Phải đặt UseAuthentication() TRƯỚC UseAuthorization() để hệ thống nhận diện được ai đang gọi API
app.UseAuthentication();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();