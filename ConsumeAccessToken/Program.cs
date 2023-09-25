// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

HttpClient httpClient = new HttpClient();

// Define your Reddit API credentials
string clientId = "JWWtLMScYmoMS1UQT9Y3oA";
string clientSecret = "aqQGqBk_2rIDqxi7ac-rz-tERVEiMA";
string username = "Gaunhoinhoi";
string password = "9GYMupU9497m#a6";

// Create a request to obtain an access token
var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://www.reddit.com/api/v1/access_token");
var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
tokenRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
tokenRequest.Content = new FormUrlEncodedContent(new[]
{
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        });

// Send the token request
HttpResponseMessage tokenResponse = await httpClient.SendAsync(tokenRequest);

if (tokenResponse.IsSuccessStatusCode)
{
    string tokenContent = await tokenResponse.Content.ReadAsStringAsync();
    Console.WriteLine(tokenContent);
}
else
{
    Console.WriteLine($"Token request failed with status code {tokenResponse.StatusCode}");
}