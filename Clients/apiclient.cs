using System.Net;
using Contacting_Frontend.ApiModels;
using System.Text.Json;

namespace Contacting_Frontend.Clients;

public class apiclient
{
    private readonly HttpClient _httpClient = new();
    private HttpRequestMessage _requestMessage;
    public async Task<HttpResponseMessage> CreateUser(long tgid, string tg_username)
    {
        
        _requestMessage = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"https://localhost:7203/User/CreateUser?id={tgid}&nickname={tg_username}"),
        };
        Console.WriteLine(_requestMessage.RequestUri);
        try
        {
            var response = await _httpClient.SendAsync(_requestMessage);
            return response.StatusCode == HttpStatusCode.OK
                ? response
                : new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
        catch
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }

    public User ShowAnketa(long id)
    {
        _requestMessage.RequestUri = new Uri($"https://localhost:7203/Anketa/GetMyAnketa?id={id}");
        try
        {
            var resp = _httpClient.SendAsync(_requestMessage);
            var json = resp.Result.Content.ReadAsStringAsync().Result;
            var user = JsonSerializer.Deserialize<User>(json);
            return user;
        } catch { return null; }
    }
    
    public async Task<HttpResponseMessage> FillData(long id, int age, string city, string male, string descrip, string name)
    {
        _requestMessage.RequestUri = new Uri($"https://localhost:7203/Ankete/FillData?tgid={id}&name={name}&age={age}&male={male}&city={city}&description={descrip}");

        try
        {
            var response = await _httpClient.SendAsync(_requestMessage);
            return response.StatusCode == HttpStatusCode.OK
                ? response
                : new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
        catch
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}