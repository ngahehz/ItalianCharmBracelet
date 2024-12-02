using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ItalianCharmBracelet.Helpers
{
    public sealed class PaypalClient
    {
        public string Mode { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }

        public string BaseUrl => Mode == "Live"
            ? "https://api-m.paypal.com"
            : "https://api-m.sandbox.paypal.com";

        public PaypalClient(string clientId, string clientSecret, string mode)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Mode = mode;
        }

        private async Task<AuthResponse> Authenticate()
        {
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));

            var content = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials")
            };

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}/v1/oauth2/token"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Authorization", $"Basic {auth}" }
                },
                Content = new FormUrlEncodedContent(content)
            };

            var httpClient = new HttpClient();
            var httpResponse = await httpClient.SendAsync(request);
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);

            return response;
        }

        public async Task<string> GetPaypalAccessToken()
        {
            string accessToken = "";
            string url = BaseUrl + "/v1/oauth2/token";
            using (var client = new HttpClient())
            {
                string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId + ":" + ClientSecret));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");
                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var response = await httpResponse.Content.ReadAsStringAsync();
                    var json = JsonNode.Parse(response);
                    if (json != null)
                    {
                        accessToken = json["access_token"]?.ToString() ?? "";
                    }
                }
            }
            return accessToken;
        }


        //public async Task<CreateOrderResponse> CreateOrder(string value, string currency, string reference)
        //{
        //    var auth = await Authenticate();

        //    var request = new CreateOrderRequest
        //    {
        //        intent = "CAPTURE",
        //        purchase_units = new List<PurchaseUnit>
        //        {
        //            new()
        //            {
        //                reference_id = reference,
        //                amount = new Amount
        //                {
        //                    currency_code = currency,
        //                    value = value
        //                }
        //            }
        //        }
        //    };

        //    var httpClient = new HttpClient();

        //    httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {auth.access_token}");

        //    var httpResponse = await httpClient.PostAsJsonAsync($"{BaseUrl}/v2/checkout/orders", request);

        //    var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
        //    var response = JsonSerializer.Deserialize<CreateOrderResponse>(jsonResponse);

        //    return response;
        //}

        public async Task<JsonNode?> CreateOrder(string value, string currency, string reference)
        {
            JsonObject createOrderRequest = new JsonObject();
            createOrderRequest.Add("intent", "CAPTURE");

            JsonObject amount = new JsonObject();
            amount.Add("currency_code", currency);
            amount.Add("value", value);

            JsonObject purchaseUnit1 = new JsonObject();
            purchaseUnit1.Add("reference_id", reference);
            purchaseUnit1.Add("amount", amount);

            JsonArray purchaseUnits = new JsonArray();
            purchaseUnits.Add(purchaseUnit1);

            createOrderRequest.Add("purchase_units", purchaseUnits);

            string accessToken = await GetPaypalAccessToken();

            string url = BaseUrl + "/v2/checkout/orders";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent(createOrderRequest.ToString(), null, "application/json");
                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var response = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(response);
                    if (jsonResponse != null)
                    {
                        return jsonResponse;
                    }
                }
            }
            return null;
        }

        public async Task<JsonNode?> CaptureOrder(string orderId)
        {
            string accessToken = await GetPaypalAccessToken();

            string url = BaseUrl + "/v2/checkout/orders/" + orderId + "/capture";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("", null, "application/json");
                
                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var response = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResponse = JsonNode.Parse(response);

                    if (jsonResponse != null)
                    {
                        return jsonResponse;
                    }
                }
            }

            return null;
        }
        //public async Task<CaptureOrderResponse> CaptureOrder(string orderId)
        //{
        //    var auth = await Authenticate();

        //    var httpClient = new HttpClient();

        //    httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {auth.access_token}");

        //    var httpContent = new StringContent("", Encoding.Default, "application/json");

        //    var httpResponse = await httpClient.PostAsync($"{BaseUrl}/v2/checkout/orders/{orderId}/capture", httpContent);
            
        //    var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
        //    var response = JsonSerializer.Deserialize<CaptureOrderResponse>(jsonResponse);

        //    return response;
        //}
    }

    public sealed class AuthResponse
    {
        public string scope { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string app_id { get; set; }
        public int expires_in { get; set; }
        public string nonce { get; set; }
    }

    public sealed class CreateOrderRequest
    {
        public string intent { get; set; }
        public List<PurchaseUnit> purchase_units { get; set; } = new();
    }

    public sealed class CreateOrderResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public List<Link> links { get; set; }
    }

    public sealed class CaptureOrderResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public PaymentSource payment_source { get; set; }
        public List<PurchaseUnit> purchase_units { get; set; }
        public Payer payer { get; set; }
        public List<Link> links { get; set; }
    }

    public sealed class PurchaseUnit
    {
        public Amount amount { get; set; }
        public string reference_id { get; set; }
        public Shipping shipping { get; set; }
        public Payments payments { get; set; }
    }

    public sealed class Payments
    {
        public List<Capture> captures { get; set; }
    }

    public sealed class Shipping
    {
        public Address address { get; set; }
    }

    public class Capture
    {
        public string id { get; set; }
        public string status { get; set; }
        public Amount amount { get; set; }
        public SellerProtection seller_protection { get; set; }
        public bool final_capture { get; set; }
        public string disbursement_mode { get; set; }
        public SellerReceivableBreakdown seller_receivable_breakdown { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public List<Link> links { get; set; }
    }

    public class Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public sealed class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }

    public sealed class Name
    {
        public string given_name { get; set; }
        public string surname { get; set; }
    }

    public sealed class SellerProtection
    {
        public string status { get; set; }
        public List<string> dispute_categories { get; set; }
    }

    public sealed class SellerReceivableBreakdown
    {
        public Amount gross_amount { get; set; }
        public PaypalFee paypal_fee { get; set; }
        public Amount net_amount { get; set; }
    }

    public sealed class Paypal
    {
        public Name name { get; set; }
        public string email_address { get; set; }
        public string account_id { get; set; }
    }

    public sealed class PaypalFee
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Address
    {
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string admin_area_2 { get; set; }
        public string admin_area_1 { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
    }

    public sealed class Payer
    {
        public Name name { get; set; }
        public string email_address { get; set; }
        public string payer_id { get; set; }
    }

    public sealed class PaymentSource
    {
        public Paypal paypal { get; set; }
    }

}
