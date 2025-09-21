using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Application.Services.interfaces
{
    public interface IDashboardService
    {
        public RadialChartDto GetTotalBookingsForChartData();
        public RadialChartDto GetTotalUsersForChartData();
        public RadialChartDto GetTotalRevenuesForChartData();
        public PieChartDto GetTotalBookingPieChart();
        public LineChartDto GetUsersAndBookingsLineChartData();
    }
}
