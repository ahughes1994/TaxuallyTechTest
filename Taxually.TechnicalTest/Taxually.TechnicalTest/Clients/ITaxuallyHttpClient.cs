namespace Taxually.TechnicalTest.Clients
{
    public interface ITaxuallyHttpClient
    {
        Task PostAsync<TRequest>(string url, TRequest request);
    }
}