using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcRepules.BLL.Services;
using MvcRepules.Model;

namespace Web.Controllers
{
    public class FlightsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly FlightService _flightService;
        public FlightsController(UserManager<User> userManager, FlightService flightService)
        {
            _userManager = userManager;
            _flightService = flightService;
        }

        //retrive all flight's
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AllFlights(int id)
        {
            var flights = await _flightService.GetAllFlightsAsync(id);
            return View(flights);
        }

        //retrieve user's flights
        [Authorize(Roles = "Pilot")]
        [HttpGet]
        public async Task<IActionResult> MyFlights()
        {
            var flights = await _flightService.GetMyFlightsAsync(GetUserId());
            return View(flights);
        }

        //give a selected flight's deteils back
        [Authorize(Roles = "Admin")]
        [HttpGet]       
        public async Task<IActionResult> Details(Guid Id)
        {
            var flight = await _flightService.GetSelectedFlightAdminAsync(Id);
            return View(flight);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditToAccept(Guid Id)
        {
            await _flightService.EditStatusToAccept(Id);
            return RedirectToAction("Details");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditToDeny(Guid Id)
        {
            await _flightService.EditStatusToDeny(Id);
            return RedirectToAction("Details");
        }

        [Authorize(Roles = "Pilot")]
        [HttpGet]
        public async Task<IActionResult> PilotDetails(Guid Id)
        {
            var flight = await _flightService.GetSelectedFlightPilotAsync(Id);
            return View(flight);
        }

        // give loged in user's id
        private Guid GetUserId()
        {
            var userIdString = _userManager.GetUserId(HttpContext.User);
            Guid.TryParse(userIdString, out Guid userIdGuid);
            return userIdGuid;
        }
    }
}