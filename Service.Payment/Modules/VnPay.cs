using System.Globalization;

namespace Service.Payment.Modules;

public static class VnPay
{
    private const string Version = "2.1.0";
    
    private const string Command = "pay";
    
    private const string Locale = "vn";
    
    private const string CurrCode = "VND";

    private const string BaseUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";

    private const string ReturnUrl = "http://localhost:5104/api/payment/vnpay/return";
    
    private const string TmnCode = "8KHODCPT";
    
    private const string HashSecret = "0F3LBVJB0VN4P0CI34N1JIVREDNB3FL5";
    
    public static void MapEndpoints(WebApplication app)
    {
        var group = app
            .MapGroup("/api/payment/vnpay")
            .WithTags(nameof(VnPay));
        
        // TODO(1): Tạo URL thanh toán VnPay
        group.MapPost("/create-payment", (HttpContext context, double amount) =>
        {
            var parameters = new SortedList<string, string?>(new VnPayParameterComparer())
            {
                { "vnp_Version", Version },
                { "vnp_Command", Command },
                { "vnp_TmnCode", TmnCode },
                { "vnp_Amount", $"{amount * 100}" },
                { "vnp_BankCode", null },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", CurrCode },
                { "vnp_IpAddr", Utilities.GetIpAddress(context) },
                { "vnp_Locale", Locale },
                { "vnp_OrderInfo", $"Thanh toan hoa don VN150. So tien {amount} VND" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", ReturnUrl },
                { "vnp_ExpireDate", DateTime.Now.AddMinutes(5).ToString("yyyyMMddHHmmss") },
                { "vnp_TxnRef", DateTime.Now.Ticks.ToString() }
            };
            var queryString = Utilities.BuildQueryString(parameters);
            var url = $"{BaseUrl}?{queryString}";
            var signature = Utilities.ComputeHmacSha256(HashSecret, queryString);
            url += $"&vnp_SecureHash={signature}";
            return url;
        });
        
        // TODO(2): Sau khi KH thanh toán, VnPay sẽ redirect về ReturnUrl để hiển thị thông tin
        group.MapGet("/return", (HttpContext context) =>
        {
            var parameters = context.Request.Query
                .ToDictionary(x => x.Key, x => x.Value.ToString());
            var checkSum = Utilities.ComputeHmacSha256(
                HashSecret, 
                Utilities.BuildQueryString(
                    parameters
                        .Where(x => x.Key != "vnp_SecureHash")
                        .ToDictionary()!
                )
            );
            return new
            {
                CheckSum = checkSum,
                IsValidSignature = checkSum.Equals(parameters["vnp_SecureHash"]),
                ResponseSuccess = parameters["vnp_ResponseCode"].Equals("00"),
                Parameters = parameters
            };
        });
        
        // TODO(3): Sau khi KH thanh toán, VnPay sẽ call api IPN đã cài đặt trên Merchant để xử lý lưu database
        group.MapGet("/ipn", (HttpContext context, ILogger<VnPayLogger> logger) =>
        {
            var parameters = context.Request.Query
                .ToDictionary(x => x.Key, x => x.Value.ToString());
            logger.LogInformation("{Module}: {@Result}", nameof(VnPay), parameters);
        });
    }

    private sealed class VnPayLogger;
    
    private sealed class VnPayParameterComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}

