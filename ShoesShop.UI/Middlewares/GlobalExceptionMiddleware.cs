using System.Net;
using System.Text.Json;
using FluentValidation;
using ShoesShop.Domain.Exceptions;

namespace ShoesShop.UI.Middlewares;

/// <summary>
/// middleware này sẽ bắt tất cả các lỗi phát sinh trong quá trình xử lý request và trả về một response có cấu trúc
/// chuẩn với thông tin lỗi chi tiết. Cụ thể, middleware sẽ xử lý các loại lỗi sau:
/// - ValidationException: lỗi xác thực dữ liệu
/// - AppException: lỗi ứng dụng
/// - UnauthorizedAccessException: lỗi không có quyền truy cập
/// - Exception: các lỗi khác
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (AppException ex)
        {
            await HandleAppExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleUnauthorizedExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnknownExceptionAsync(context, ex);
        }
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var errors = ex.Errors.Select(x => new
        {
            field = x.PropertyName,
            message = x.ErrorMessage
        });

        var response = new
        {
            success = false,
            message = "Dữ liệu không hợp lệ.",
            errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static async Task HandleAppExceptionAsync(HttpContext context, AppException ex)
    {
        context.Response.StatusCode = ex.StatusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = ex.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static async Task HandleUnauthorizedExceptionAsync(HttpContext context, UnauthorizedAccessException ex)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = ex.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private async Task HandleUnknownExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = "Đã xảy ra lỗi hệ thống.",
            detail = _environment.IsDevelopment() ? ex.Message : null
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
