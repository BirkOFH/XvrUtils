using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xvr.Utils.Helpers;

namespace Xvr.Utils.Network
{
    public class HttpClientService: IHttpService
    {
        private ILocalStorageService _localStorageService;
        private HttpClient _httpClient;
        private string _tokenStorageKey;

        public HttpClientService(ILocalStorageService localStorageService, HttpClient httpClient, string tokenStorageKey="user")
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
            _tokenStorageKey = tokenStorageKey;
        }

        public async Task<HttpServiceResponse<T>?> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await sendRequest<T>(request);
        }

        public async Task<HttpServiceResponse<string>?> Post(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            return await sendRequest(request);
        }

        public async Task<HttpServiceResponse<T>?> Post<T>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            return await sendRequest<T>(request);
        }

        public async Task<HttpServiceResponse<string>?> Put(string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            return await sendRequest(request);
        }

        public async Task<HttpServiceResponse<T>?> Put<T>(string uri, object value)
        {
            var request = createRequest(HttpMethod.Put, uri, value);
            return await sendRequest<T>(request);
        }

        public async Task<HttpServiceResponse<string>?> Delete(string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            return await sendRequest(request);
        }

        public async Task<HttpServiceResponse<T>?> Delete<T>(string uri)
        {
            var request = createRequest(HttpMethod.Delete, uri);
            return await sendRequest<T>(request); 

        }

        // helper methods

        private HttpRequestMessage createRequest(HttpMethod method, string uri, object? value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
                request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return request;
        }

        private async Task<HttpServiceResponse<string>?> sendRequest(HttpRequestMessage request)
        {
            await addJwtHeader(request);

            // send request
            using var response = await _httpClient.SendAsync(request);

            //todo: auto logout on 401 response
            //if (response.StatusCode == HttpStatusCode.Unauthorized)
            //{
            //    _navigationManager.NavigateTo("account/logout");
            //    return null;
            //}

            var errors = await handleErrors(response);
            return new HttpServiceResponse<string>(response.StatusCode, message: errors);
        }

        private async Task<HttpServiceResponse<T>> sendRequest<T>(HttpRequestMessage request)
        {
            await addJwtHeader(request);

            // send request
            using var response = await _httpClient.SendAsync(request);
            var bt = await response.Content.ReadAsStringAsync();


            //todo: auto logout on 401 response
            //if (response.StatusCode == HttpStatusCode.Unauthorized)
            //{
            //    _navigationManager.NavigateTo("account/logout");
            //    return default;
            //}

            var errors = await handleErrors(response);

            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new StringConverter());

            return new HttpServiceResponse<T>(response.StatusCode, errors,
                await response.Content.ReadFromJsonAsync<T>(options));
        }

        private async Task addJwtHeader(HttpRequestMessage request)
        {
            // add jwt auth header if user is logged in and request is to the api url
            var tokenStorage = await _localStorageService.GetItem<ITokenStorage>(_tokenStorageKey);
            var isApiUrl = request.RequestUri != null && !request.RequestUri.IsAbsoluteUri;
            if (tokenStorage != null && isApiUrl)
                request.Headers.Add("Authorization", "Bearer " + tokenStorage.Token);
        }

        private async Task<string?> handleErrors(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (error != null) return error["message"];
            }
            return String.Empty;
        }
    }
}
