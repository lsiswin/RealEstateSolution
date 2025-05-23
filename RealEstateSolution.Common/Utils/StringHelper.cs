using System.Text.RegularExpressions;

namespace RealEstateSolution.Common.Utils;

/// <summary>
/// 字符串工具类
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// 生成随机字符串
    /// </summary>
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// 判断字符串是否为手机号
    /// </summary>
    public static bool IsValidPhoneNumber(string phone)
    {
        return Regex.IsMatch(phone, @"^1[3-9]\d{9}$");
    }

    /// <summary>
    /// 判断字符串是否为邮箱
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    }

    /// <summary>
    /// 获取字符串的MD5值
    /// </summary>
    public static string GetMD5(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    /// <summary>
    /// 截取字符串
    /// </summary>
    public static string Truncate(string input, int maxLength)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return input.Length <= maxLength ? input : input.Substring(0, maxLength) + "...";
    }
} 