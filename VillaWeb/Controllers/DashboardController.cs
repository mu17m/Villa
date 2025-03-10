using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.SD;
using WhiteLagoon.Application.Services.Implementations;
using WhiteLagoon.Application.Services.interfaces;
using WhiteLagoon.Infrastructure.Repository;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region API Calls
        [HttpGet]
        public IActionResult GetTotalBookingsForChartData()
        {
            return Json(_dashboardService.GetTotalBookingsForChartData());
        }
        [HttpGet]
        public IActionResult GetTotalUsersForChartData()
        {
            return Json(_dashboardService.GetTotalUsersForChartData());
        }
        [HttpGet]
        public IActionResult GetTotalRevenuesForChartData()
        {
            return Json(_dashboardService.GetTotalRevenuesForChartData());
        }
        [HttpGet]
        public IActionResult GetTotalBookingPieChart()
        {
            return Json(_dashboardService.GetTotalBookingPieChart());
        }
        [HttpGet]
        public IActionResult GetUsersAndBookingsLineChartData()
        {
            return Json(_dashboardService.GetUsersAndBookingsLineChartData());
        }
        #endregion

    }
}
