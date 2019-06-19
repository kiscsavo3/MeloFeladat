using Microsoft.AspNetCore.Hosting;
using MvcRepules.DAL;
using MvcRepules.Model;
using MvcRepules.BLL.DTO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MvcRepules.BLL.Services
{
    public class LogUploadService
    {

        private readonly ApplicationDbContext _appDbContext;
        public LogUploadService( ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
         public async Task<bool> LogUploadAsync(LogUploadDto logUploadDto, Guid userId)
        {
            if (logUploadDto.LogFile != null)
            {
                PilotLog pilotLog = new PilotLog
                {
                    UserId = userId
                };
                byte[] fileData = null;                              

                using (var stream = new MemoryStream())
                {
                    await (logUploadDto.LogFile.CopyToAsync(stream));                  
                    fileData = stream.ToArray();                    
                }              

                pilotLog.File = fileData;
                _appDbContext.PilotLog.Add(pilotLog);
                _appDbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
