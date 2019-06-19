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

                            IQueryable<Airport> airports = from m in _appDbContext.Airport select m;
                            IQueryable<GlobalPoint> gp = from g in _appDbContext.GlobalPoint select g;
                            Airport isExist = airports.SingleOrDefault(m => m.AirportName == airport.AirportName);
                            if (isExist != null)
                            {
                                GlobalPoint itHasToDelete = gp.SingleOrDefault(g => g.GlobalPointId == isExist.GlobalPointId);
                                _appDbContext.Airport.Remove(isExist);
                                _appDbContext.GlobalPoint.Remove(itHasToDelete);
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
