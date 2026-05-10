using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MexicanRestaurant.Models;

namespace MexicanRestaurant.Services;

public class PaystackService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public PaystackService(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
        
        var secretKey = _configuration["Paystack:SecretKey"];
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<PaystackInitializeResponse> InitializeTransaction(string email, decimal amount, string? reference = null)
    {
        var request = new
        {
            email = email,
            amount = amount * 100, // Convert to kobo
            reference = reference ?? GenerateReference()
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.paystack.co/transaction/initialize", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<PaystackInitializeResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new PaystackInitializeResponse();
    }

    public async Task<PaystackVerifyResponse> VerifyTransaction(string reference)
    {
        var response = await _httpClient.GetAsync($"https://api.paystack.co/transaction/verify/{reference}");
        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<PaystackVerifyResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new PaystackVerifyResponse();
    }

    private string GenerateReference()
    {
        return $"PS_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}";
    }
}

public class PaystackInitializeResponse
{
    public bool Status { get; set; }
    public string? Message { get; set; }
    public PaystackData? Data { get; set; }
}

public class PaystackData
{
    public string? AuthorizationUrl { get; set; }
    public string? Reference { get; set; }
    public string? AccessCode { get; set; }
}

public class PaystackVerifyResponse
{
    public bool Status { get; set; }
    public string? Message { get; set; }
    public PaystackTransactionData? Data { get; set; }
}

public class PaystackTransactionData
{
    public string? Reference { get; set; }
    public string? Status { get; set; }
    public string? GatewayResponse { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public string? Customer { get; set; }
}