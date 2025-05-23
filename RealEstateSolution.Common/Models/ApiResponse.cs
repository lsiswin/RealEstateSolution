namespace RealEstateSolution.Common.Models;

/// <summary>
/// API统一返回类型
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 创建成功响应
    /// </summary>
    public static ApiResponse Ok(string message = "操作成功")
    {
        return new ApiResponse
        {
            Code = 200,
            Message = message,
            Success = true
        };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    public static ApiResponse Error(string message = "操作失败", int code = 400)
    {
        return new ApiResponse
        {
            Code = code,
            Message = message,
            Success = false
        };
    }
}

/// <summary>
/// API统一返回类型（泛型）
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 创建成功响应
    /// </summary>
    public static ApiResponse<T> Ok(T data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Code = 200,
            Message = message,
            Success = true,
            Data = data
        };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    public static ApiResponse<T> Error(string message = "操作失败", int code = 400)
    {
        return new ApiResponse<T>
        {
            Code = code,
            Message = message,
            Success = false
        };
    }
} 