using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.SD;
using WhiteLagoon.Application.Services.interfaces;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Application.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previusMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        // Get the first day of the previous month, the previusMonth var should be static to use it in the class of DateTime
        readonly DateTime previusMonthStartDate = new(DateTime.Now.Year, previusMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public RadialChartDto GetTotalUsersForChartData()
        {            
            var totalBookings = _unitOfWork.BookingRepo.GetAll(b => b.Status != SD.StatusPending || b.Status != SD.StatusCancelled).ToList();
            var countByCurrentMonth = totalBookings.Count(b => b.BookingDate >= DateOnly.FromDateTime(previusMonthStartDate)
                && b.BookingDate < DateOnly.FromDateTime(DateTime.Now));
            var countByPreviusMonth = totalBookings.Count(b => b.BookingDate >= DateOnly.FromDateTime(previusMonthStartDate)
                && b.BookingDate <= DateOnly.FromDateTime(currentMonthStartDate));
            return SD.GetRadialChartDataModel(totalBookings.Count, countByCurrentMonth, countByPreviusMonth);
        }
        public RadialChartDto GetTotalBookingsForChartData()
        {
            var totalUsers = _unitOfWork.UserRepo.GetAll().ToList();
            var countByCurrentMonth = totalUsers.Count(b => b.CreatedAt >= previusMonthStartDate
                && b.CreatedAt < DateTime.Now);
            var countByPreviusMonth = totalUsers.Count(b => b.CreatedAt >= previusMonthStartDate
                && b.CreatedAt <= currentMonthStartDate);
            return SD.GetRadialChartDataModel(totalUsers.Count, countByCurrentMonth, countByPreviusMonth);
        }
        public RadialChartDto GetTotalRevenuesForChartData()
        {
            var totalBooking = _unitOfWork.BookingRepo.GetAll(b => b.Status != SD.StatusPending && b.Status != SD.StatusCancelled);
            int totalRevenue = Convert.ToInt32(totalBooking.Sum(b => b.TotalCost));
            var countByCurrentMonth = totalBooking.Where(b => b.BookingDate >= DateOnly.FromDateTime(previusMonthStartDate)
                && b.BookingDate <= DateOnly.FromDateTime(DateTime.Now)).Count();
            var countByPreviusMonth = totalBooking.Where(b => b.BookingDate >= DateOnly.FromDateTime(previusMonthStartDate)
                && b.BookingDate <= DateOnly.FromDateTime(currentMonthStartDate)).Count();
            return SD.GetRadialChartDataModel(totalRevenue, countByCurrentMonth, countByPreviusMonth);
        }
        public PieChartDto GetTotalBookingPieChart()
        {
            var totalBooking = _unitOfWork.BookingRepo.GetAll(b => b.BookingDate > DateOnly.FromDateTime(DateTime.Now.AddDays(-30))
                 && (b.Status != SD.StatusPending && b.Status != SD.StatusCancelled)).ToList();

            var newCustomers = totalBooking.GroupBy(b => b.UserId).Where(user => user.Count() == 1).Select(customer => customer.Key).ToList();
            int newCustomersCount = newCustomers.Count;
            int returningCustomersCount = totalBooking.Count - newCustomersCount;

            PieChartDto PieChartDto = new PieChartDto()
            {
                labels = new string[] { "New Customer Bookings", "Returns Customer Bookings" },
                Series = new decimal[] { newCustomersCount, returningCustomersCount }
            };
            return PieChartDto;
        }
        public LineChartDto GetUsersAndBookingsLineChartData()
        {
            var totalBookings = _unitOfWork.BookingRepo.GetAll(b => b.BookingDate >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)) && b.BookingDate <= DateOnly.FromDateTime(DateTime.Now)).GroupBy(b => b.BookingDate).Select(
                b => new
                {
                    DateTime = b.Key,
                    NewBookingCount = b.Count()
                }).ToList();
            var totalUsers = _unitOfWork.UserRepo.GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30)).GroupBy(user => DateOnly.FromDateTime(user.CreatedAt)).Select(x =>
                new
                {
                    DateTime = x.Key,
                    NewUserCount = x.Count()
                }).ToList();

            var leftJoin = totalBookings.GroupJoin(totalUsers,
                booking => booking.DateTime,
                user => user.DateTime,
                (booking, u) => new
                {
                    booking.DateTime,
                    booking.NewBookingCount,
                    NewUserCount = u.Select(x => x.NewUserCount).FirstOrDefault()

                });
            var rightJoin = totalUsers.GroupJoin(totalBookings, user => user.DateTime, booking => booking.DateTime,
                (user, booking) => new
                {
                    user.DateTime,
                    NewBookingCount = booking.Select(b => b.NewBookingCount).FirstOrDefault(),
                    user.NewUserCount
                });

            var mergedDate = leftJoin.Union(rightJoin).OrderBy(ele => ele.DateTime).ToList();

            var newUsersCount = mergedDate.Select(d => d.NewUserCount).ToArray();
            var newBookingsCount = mergedDate.Select(d => d.NewBookingCount).ToArray();
            var Categories = mergedDate.Select(d => d.DateTime.ToString("dd/MM/yyyy")).ToArray();

            List<ChartData> chartDataList = new List<ChartData>()
            {
                new ChartData()
                {
                    Name="New Bookings",
                    data = newBookingsCount
                },
                new ChartData()
                {
                    Name="New Users",
                    data = newUsersCount
                }
            };
            LineChartDto LineChartDto = new LineChartDto()
            {
                series = chartDataList,
                Categories = Categories
            };
            return LineChartDto;
        }
    }
}
