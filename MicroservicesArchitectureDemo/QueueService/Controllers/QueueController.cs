using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueService.Services;
using System.Threading.Tasks;

namespace QueueService.Controllers
{
    [ApiController]
    [Route("queue")]
    public class QueueController : ControllerBase
    {
        private readonly IMessageQueueService _messageQueueService;

        public QueueController(IMessageQueueService messageQueueService)
        {
            _messageQueueService = messageQueueService;
        }

        [HttpPost("publish")]
        [Authorize]
        public async Task<IActionResult> PublishMessage([FromBody] string message)
        {
            await _messageQueueService.PublishMessageAsync(message);
            return Ok(new { message = "Message published successfully" });
        }

        [HttpGet("consume")]
        [Authorize]
        public async Task<IActionResult> ConsumeMessage()
        {
            var message = await _messageQueueService.GetMessagesAsync();
            if (message == null)
            {
                return NotFound(new { message = "No messages available" });
            }

            return Ok(new { message });
        }
    }
}
