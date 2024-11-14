using System.Threading.Tasks;

namespace QueueService.Services
{
    public interface IMessageQueueService
    {
        Task PublishMessageAsync(string message);
        Task<List<string>> GetMessagesAsync();
    }
}
