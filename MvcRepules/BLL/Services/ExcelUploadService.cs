using MvcRepules.BLL.DTO;
using MvcRepules.DAL;
using MvcRepules.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ExcelUploadService
    {
        private readonly ApplicationDbContext _appDbContext;
        public ExcelUploadService(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        
        public async Task<bool> ExcelUploadAsync(ExcelUploadDto excelUploadDto)
        {
            if (excelUploadDto.ExcelFile != null &&
                (Path.GetExtension(excelUploadDto.ExcelFile.FileName).Equals(".xlsx") ||
                Path.GetExtension(excelUploadDto.ExcelFile.FileName).Equals(".xls")))
            {
                using (var stream = new MemoryStream())
                {
                    await excelUploadDto.ExcelFile.CopyToAsync(stream);
                    using (ExcelPackage ep = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = ep.Workbook.Worksheets[0];
                        int rowsCount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowsCount; row++)
                        {
                            GlobalPoint globalPoint = new GlobalPoint
                            {
                                Latitude = (float)(double)(worksheet.Cells[row, 2].Value),
                                Longitude = (float)(double)(worksheet.Cells[row, 3].Value)

                            };
                            Airport airport = new Airport
                            {
                                AirportName = worksheet.Cells[row, 1].Value.ToString(),
                                GlobalPoint = globalPoint
                            };

                            Airport isExist = _appDbContext.Airport
                                .SingleOrDefault(m => m.AirportName == airport.AirportName);
                            if (isExist != null)
                            {
                                _appDbContext.Airport.Remove(isExist);
                                _appDbContext.GlobalPoint.Remove(isExist.GlobalPoint);
                            }
                            await _appDbContext.GlobalPoint.AddAsync(globalPoint);
                            await _appDbContext.Airport.AddAsync(airport);
                        }
                        await _appDbContext.SaveChangesAsync();
                    }
                }
                return true;
            }
            else { return false; }
        }
    }
}
