namespace Taxually.TechnicalTest.Clients
{
    public interface ITaxuallyQueueClient
    {
        Task EnqueueAsync<TPayload>(string queueName, TPayload payload);
    }
}