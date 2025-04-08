using System.Collections.Generic;

namespace K9.WebApplication.Options
{
    public class PieChartOptions
    {
        public List<PieChartItem> PieChartItems { get; set; }
    }

    public class PieChartItem
    {
        public string Title { get; set; }
        public int Value { get; set; }
        public string Color { get; set; }
    }
}