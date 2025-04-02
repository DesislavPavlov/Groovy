using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Groovy.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7021/api";
        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_apiBaseUrl}/{endpoint}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }

            return default;
        }

        public async Task<bool> PostReturnBoolAsync<T>(string endpoint, T data)
        {
            string json = JsonConvert.SerializeObject(data);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_apiBaseUrl}/{endpoint}", content);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                int idOrAffectedRows = JsonConvert.DeserializeObject<int>(jsonResponse);
                if (idOrAffectedRows > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<T> PostAsync<T>(string endpoint, T data)
        {
            string json = JsonConvert.SerializeObject(data);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_apiBaseUrl}/{endpoint}", content);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonResponse);
            }

            return default;
        }

        public async Task<bool> PostMultipartAsync(string endpoint, Dictionary<string, string> formFields, Dictionary<string, IFormFile> files)
        {
            using (var formData = new MultipartFormDataContent())
            {
                // Add form fields
                foreach (var field in formFields)
                {
                    formData.Add(new StringContent(field.Value), field.Key);
                }

                // Add files
                if (files != null && files.Count > 0)
                {
                    foreach (var field in files)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await field.Value.CopyToAsync(stream);
                            var fileContent = new ByteArrayContent(stream.ToArray());
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(field.Value.ContentType);
                            formData.Add(fileContent, field.Key, field.Value.FileName);
                        }
                    }
                }

                // Send request
                HttpResponseMessage response = await _httpClient.PostAsync($"{_apiBaseUrl}/{endpoint}", formData);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> PutAsync<T>(string endpoint, T data)
        {
            string json = JsonConvert.SerializeObject(data);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{_apiBaseUrl}/{endpoint}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutMultipartAsync(string endpoint, Dictionary<string, string> formFields, Dictionary<string, IFormFile> files)
        {
            using (var formData = new MultipartFormDataContent())
            {
                // Add form fields
                foreach (var field in formFields)
                {
                    formData.Add(new StringContent(field.Value), field.Key);
                }

                // Add files
                if (files != null && files.Count > 0)
                {
                    foreach (var field in files)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await field.Value.CopyToAsync(stream);
                            var fileContent = new ByteArrayContent(stream.ToArray());
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(field.Value.ContentType);
                            formData.Add(fileContent, field.Key, field.Value.FileName);
                        }
                    }
                }

                // Send request
                HttpResponseMessage response = await _httpClient.PutAsync($"{_apiBaseUrl}/{endpoint}", formData);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{endpoint}");
            return response.IsSuccessStatusCode;
        }

    }
}
