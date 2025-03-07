using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.SD;
using WhiteLagoon.Infrastructure.Repository;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previusMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        // Get the first day of the previous month, the previusMonth var should be static to use it in the class of DateTime
        readonly DateTime previusMonthStartDate = new(DateTime.Now.Year, previusMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        private static RadialBarCharVM GetRadialChartDataModel(int totalCounts, int countByCurrentMonth, int countByPreviusMonth)
        {
            RadialBarCharVM radialBarChar = new();
            int IncreaseDecreaseRation = 100;
            if (countByPreviusMonth != 0)
            {
                IncreaseDecreaseRation = Convert.ToInt32(countByCurrentMonth - countByPreviusMonth / countByPreviusMonth * 100);
            }
            radialBarChar.TotalCount = totalCounts;
            radialBarChar.CountInCurrentMonth = countByCurrentMonth;
            radialBarChar.HasRationIncreased = countByCurrentMonth > countByPreviusMonth;
            radialBarChar.Series = new int[] { IncreaseDecreaseRation };

            return radialBarChar;
        }
        public async Task<IActionResult> GetTotalBookingsForCharts()
        {
            var totalBookings = _unitOfWork.BookingRepo.GetAll(b => b.Status != SD.StatusPending || b.Status != SD.StatusCancelled);
            var countByCurrentMonth = totalBookings.Count(b => b.BookingDate >= DateOnly.FromDateTime(previusMonthStartDate)
                && b.BookingDate < DateOnly.FromDateTime(DateTime.Now));
            var countByPreviusMonth = totalBookings.Count(b => b.BookingDate >= DateOnly.FromDateTime(previusMonthStartDate)
                && b.BookingDate <= DateOnly.FromDateTime(currentMonthStartDate));
            return Json(GetRadialChartDataModel(totalBookings.Count(), countByCurrentMonth, countByPreviusMonth));
        }
        public async Task<IActionResult> GetTotalRegisteredUserChartDataAsync()
        {
            var totalUsers = _unitOfWork.UserRepo.GetAll();
            var countByCurrentMonth = totalUsers.Count(b => b.CreatedAt >= previusMonthStartDate
                && b.CreatedAt < DateTime.Now);
            var countByPreviusMonth = totalUsers.Count(b => b.CreatedAt >= previusMonthStartDate 
                && b.CreatedAt <= currentMonthStartDate);
            

            return Json(GetRadialChartDataModel(totalUsers.Count(), countByCurrentMonth, countByPreviusMonth));
        }
    }
}
