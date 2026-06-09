namespace RestaurantMenuApp.Models;

public class ReportViewModel
{
    public List<FullListEntry> FullList { get; set; } = new();
    public List<CountByCategory> CountByRestaurant { get; set; } = new();
    public List<AvgPriceByCategory> AvgPriceByRestaurant { get; set; } = new();
}

public class FullListEntry
{
    public string DishName { get; set; } = "";
    public string RestaurantName { get; set; } = "";
    public decimal Price { get; set; }
}

public class CountByCategory
{
    public string RestaurantName { get; set; } = "";
    public int Count { get; set; }
}

public class AvgPriceByCategory
{
    public string RestaurantName { get; set; } = "";
    public decimal AveragePrice { get; set; }
}
