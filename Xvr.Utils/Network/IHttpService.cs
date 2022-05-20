namespace Xvr.Utils.Network;

public interface IHttpService
{
    Task<HttpServiceResponse<T>?> Get<T>(string uri);
    Task<HttpServiceResponse<string>?> Post(string uri, object value);
    Task<HttpServiceResponse<T>?> Post<T>(string uri, object value);
    Task<HttpServiceResponse<string>?> Put(string uri, object value);
    Task<HttpServiceResponse<T>?> Put<T>(string uri, object value);
    Task<HttpServiceResponse<string>?> Delete(string uri);
    Task<HttpServiceResponse<T>?> Delete<T>(string uri);
}