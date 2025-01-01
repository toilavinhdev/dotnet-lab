using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Service.Payment;

public static class Utilities
{
    public static string BuildQueryString(IDictionary<string, string?> dictionary)
    {
        var queries = dictionary
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(pair => $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(pair.Value)}");
        return string.Join("&", queries);
    }

    public static string ComputeHmacSha256(string key, string input)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var buffer = Encoding.UTF8.GetBytes(input);
        var hash = hmac.ComputeHash(buffer);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
    
    public static string GetIpAddress(HttpContext context)
    {
        return "127.0.0.1";
    }
}