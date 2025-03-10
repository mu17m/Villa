namespace WhiteLagoon.Web.ViewModels
{
    public class RadialChartDto
    {
        //total bookings
        public decimal TotalCount { get; set; }
        //Bookings in current month
        public decimal CountInCurrentMonth { get; set; }
        public bool HasRationIncreased { get; set; }
        public int[]? Series { get; set; }
    }
}
