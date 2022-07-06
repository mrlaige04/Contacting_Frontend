using System.Net;
using Contacting_Frontend.ApiModels;
using System.Text.Json;

namespace Contacting_Frontend.Clients;

public class apiclient
{
    private readonly HttpClient _httpClient = new();
    private HttpRequestMessage _requestMessage ;
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
        _requestMessage = new()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://localhost:7203/Ankete/GetMyAnketa?id={id}")
        };
        try
        {
            var resp = _httpClient.SendAsync(_requestMessage);
            var json = resp.Result.Content.ReadAsStringAsync().Result;
            var user = JsonSerializer.Deserialize<User>(json);
            return user;
        } catch { return null; }
    }
    
    public async Task<HttpResponseMessage> FillData(long id, int age, string city, string male, string descrip, string name, string photopath)
    {
        _requestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri =
                new Uri(
                    $"https://localhost:7203/Ankete/FillData?tgid={id}&name={name}&age={age}&male={male}&city={city}&description={descrip}&photopath={photopath}")
        };
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


    public async void DeleteMyAccount(long id)
    {
        _requestMessage = new()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("https://localhost:7203/User/DeleteMe?id=" + id)
        };
        using var response = await _httpClient.SendAsync(_requestMessage);
    }


    public async Task<List<User>> GetMales()
    {
        _requestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://localhost:7203/LookAnketes/GetMaleAnketes")
        };
        using var resp = await _httpClient.SendAsync(_requestMessage);
        return JsonSerializer.Deserialize<List<User>>(resp.Content.ReadAsStringAsync().Result);
    }
    
    public async Task<List<User>> GetFemales()
    {
        _requestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://localhost:7203/LookAnketes/GetFemaleAnketes")
        };
        using var resp = await _httpClient.SendAsync(_requestMessage);
        return JsonSerializer.Deserialize<List<User>>(resp.Content.ReadAsStringAsync().Result);
    }
}