using System;
using System.Threading.Tasks;
using BLL.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcRepules.BLL.DTO;
using MvcRepules.BLL.Services;
using MvcRepules.Model;
using MvcRepules.Web.ViewModels;

namespace Web.Controllers
{
    public class UploadController : Controller
    {
        private readonly LogUploadService _logUploadService;
        private readonly ExcelUploadService _excelUploadService;
        private readonly BackgroundProcessService _backGroundProcessService;
        private readonly UserManager<User> _userManager;

        public UploadController(
            LogUploadService logUploadService, UserManager<User> userManager, BackgroundProcessService backgroundProcessService, ExcelUploadService excelUploadService)
        {
            _logUploadService = logUploadService;
            _excelUploadService = excelUploadService;
            _backGroundProcessService = backgroundProcessService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult LogUpload()
        {
            return View();
        }

        // upload user's log
        [HttpPost]
        public async Task<IActionResult> LogUploaded(LogUploadDto logUploadDto)
        {
            if (ModelState.IsValid)
            {
                if(await _logUploadService.LogUploadAsync(logUploadDto, GetUserId()))
                {
                    // Ezt inkább ne itt hívjuk, hanem configolj be egy RecurringJob-ot, 
                    // ami pl percenként lefut és megcsinálja ugyanezt, amit most itt direkbe elindítasz.     
                    // Ez is okés olyan szempontból, hogy nem blokkolja a controllert, 
                    // viszont úgy a controller nem is tudna a JOB-ról 
                    ViewBag.Message = "Upload Success!";
                    _backGroundProcessService.RunInBackground();
                }
                else
                {
                    ViewBag.Message = "Upload failed!";
                }
            }
            else
            {
                ViewBag.Message = "Upload failed!";
            }
            return View();
        }

        [HttpGet]
        public IActionResult ExcelUpload()
        {
            return View();
        }

        // upload user's excel
        [HttpPost]
        public async Task<IActionResult> ExcelUploaded(ExcelUploadDto excelUploadDto)
        {
            if (ModelState.IsValid)
            {
                if (await _excelUploadService.ExcelUploadAsync(excelUploadDto))
                {
                    ViewBag.Message = "Upload Success!";
                }
                else
                {
                    ViewBag.Message = "Upload failed!";
                }
            }
            else
            {
                ViewBag.Message = "Upload failed!";
            }
            return View();
        }


        private Guid GetUserId()
        {
            var userIdString = _userManager.GetUserId(HttpContext.User);
            Guid.TryParse(userIdString, out Guid userIdGuid);
            return userIdGuid;
        }
    }
}