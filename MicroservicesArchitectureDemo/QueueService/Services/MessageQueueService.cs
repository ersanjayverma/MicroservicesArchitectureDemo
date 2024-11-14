using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace QueueService.Services
{
    public class InMemoryQueueStore
    {
        private readonly List<string> _queue = new(); // Simulating the Redis stream

        public void AddToQueue(string message)
        {
            _queue.Add(message);
        }

        public List<string> GetQueueMessages()
        {
            return new List<string>(_queue);
        }
    }
    public class MessageQueueService : IMessageQueueService
    {
        private readonly InMemoryQueueStore _queueStore;

        public MessageQueueService(InMemoryQueueStore queueStore)
        {
            _queueStore = queueStore;
        }

       public async Task PublishMessageAsync(string message)
    {
        _queueStore.AddToQueue(message);
    }

    public async Task<List<string>> GetMessagesAsync()
    {
        return _queueStore.GetQueueMessages();
    }
    }
}
