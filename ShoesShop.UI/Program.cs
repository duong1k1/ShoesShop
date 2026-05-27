using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoesShop.Application.Interfaces.Payments;
using ShoesShop.Application.Interfaces.Repositories;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.Application.Services;
using ShoesShop.Application.Validators.Carts;
using ShoesShop.Application.Validators.Payments;
using ShoesShop.Application.Validators.Reviews;
using ShoesShop.Infrastructure.Data;
using ShoesShop.Infrastructure.Persistence;
using ShoesShop.Infrastructure.Repositories;
using ShoesShop.Infrastructure.Services.Payments;
using ShoesShop.UI.Filters;
using ShoesShop.UI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ValidationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ValidationFilter>();
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddValidatorsFromAssemblyContaining<AddCartItemRequestValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Nạp toàn bộ Service (AuthService, UserService, VoucherService,...) từ tầng Application của bạn
builder.Services.AddApplicationServices();


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

// Đăng ký các dịch vụ liên quan đến thanh toán
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Đăng ký các dịch vụ liên quan đến đánh giá
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateReviewRequestValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<CheckoutRequestValidator>();

builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddHttpClient<IMomoService, MomoService>();
//

// 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = builder.Configuration["Jwt:Key"];
    var issuer = builder.Configuration["Jwt:Issuer"];
    var audience = builder.Configuration["Jwt:Audience"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();


var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoesShop API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();