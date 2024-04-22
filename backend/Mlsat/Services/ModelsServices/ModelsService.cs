using System.Text;
using Mlsat.Services.ModelsServices.Requests;
using Mlsat.Services.ModelsServices.Responses;
using Mlsat.Services.PythonServices.Requests;
using Newtonsoft.Json;

namespace Mlsat.Services.ModelsServices;

public class ModelsService
{
    private readonly HttpClient _httpClient;
    private string _url;

    public ModelsService()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = new TimeSpan(1, 0, 0);
        _url = "http://models";
    }

    public async Task<IReadOnlyCollection<string>> GetColumns(string path)
    {
        var url = $@"{_url}/datasets/columns?path={path}";
        var response = await _httpClient.GetAsync(url, CancellationToken.None);

        if (response.IsSuccessStatusCode)
        {
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert
                .DeserializeObject<GetColumnsResponseDto>(jsonResult);

            return result!.Columns;
        }

        throw new Exception();
    }

    public async Task CreateOnBasis(CreateOnBasisRequestDto parameters)
    {
        var url = $@"{_url}/datasets/create-on-basis";
        var body = JsonConvert.SerializeObject(parameters);
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
    }
    
    public async Task<GetDatesResponseDto> GetDates(GetDatesRequestDto parameters)
    {
        var url = $@"{_url}/datasets/dates?path={parameters.Path}&time_column={parameters.TimeColumn}";
        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert
                .DeserializeObject<GetDatesResponseDto>(jsonResult);

            return result!;
        }

        throw new Exception();
    }

    public async Task RunModel(CreateModelRequestDto parameters)
    {
        var url = $@"{_url}/models";
        var body = JsonConvert.SerializeObject(parameters);
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
    }

    #region Модели

    public async Task RunKnn(CreateKnnRequestDto parameters)
    {
        var url = $@"{_url}/models/knn";
        var body = JsonConvert.SerializeObject(parameters);
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
    }

    public async Task RunIsolationForest(CreateIsolationForestRequestDto parameters)
    {
        var url = $@"{_url}/models/isolation_forest";
        var body = JsonConvert.SerializeObject(parameters);
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
    }

    public async Task RunLof(CreateLofRequestDto parameters)
    {
        var url = $@"{_url}/models/lof";
        var body = JsonConvert.SerializeObject(parameters);
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
    }

    public async Task RunAutoencoder(CreateAutoencoderRequestDto parameters)
    {
        var url = $@"{_url}/models/autoencoder";
        var body = JsonConvert.SerializeObject(parameters);
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
    }

    
    public async Task RunSiameseAutoencoder(CreateAutoencoderRequestDto parameters)
    {
        var url = $@"{_url}/models/siamese-autoencoder";
        var body = JsonConvert.SerializeObject(parameters);
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
    }
    
    public async Task RunGanAutoencoder(CreateAutoencoderRequestDto parameters)
    {
        var url = $@"{_url}/models/gan";
        var body = JsonConvert.SerializeObject(parameters);
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
    }

    #endregion
    
}