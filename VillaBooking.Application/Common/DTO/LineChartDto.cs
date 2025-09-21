namespace WhiteLagoon.Web.ViewModels
{
    public class LineChartDto
    {
        public List<ChartData>? series {  get; set; }
        public string[]? Categories { get; set; }
    }

    public class ChartData
    {
        public string? Name { get; set; }
        public int[]? data { get; set; }
    }
}
