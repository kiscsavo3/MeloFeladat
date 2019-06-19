using Microsoft.AspNetCore.Http;

namespace MvcRepules.BLL.DTO
{
    public class LogUploadDto
    {
        public IFormFile LogFile { get; set; }
    }
}
