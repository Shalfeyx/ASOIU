using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantMenuApp.Data;
using RestaurantMenuApp.Models;

namespace RestaurantMenuApp.Controllers;

/// <summary>
/// Контроллер для отображения отчёта (три раздела).
/// </summary>
public class ReportsController : Controller
{
    /// <summary>
    /// Главное действие отчёта – формирует три раздела с помощью LINQ.
    /// </summary>
    public IActionResult Index()
    {
        using var context = new AppDbContext();
        
        // Раздел 1: Полный список блюд с названиями ресторанов
        var fullList = context.MenuItems
            .Include(m => m.Restaurant)
            .OrderBy(m => m.Name)
            .Select(m => new FullListEntry
            {
                DishName = m.Name,
                RestaurantName = m.Restaurant!.Name,
                Price = m.Price
            })
            .ToList();
        
        // Раздел 2: Количество блюд по ресторанам (GroupBy + Count)
        var countByRestaurant = context.MenuItems
            .Include(m => m.Restaurant)
            .GroupBy(m => m.Restaurant!.Name)
            .Select(g => new CountByCategory
            {
                RestaurantName = g.Key,
                Count = g.Count()
            })
            .OrderBy(r => r.RestaurantName)
            .ToList();
        
        // Раздел 3: Средняя цена по ресторанам – решение для SQLite (приводим decimal к double)
        var avgPriceByRestaurant = context.MenuItems
            .Include(m => m.Restaurant)
            .GroupBy(m => m.Restaurant!.Name)
            .Select(g => new
            {
                RestaurantName = g.Key,
                AveragePrice = g.Average(m => (double)m.Price) // преобразование в double
            })
            .AsEnumerable()
            .Select(x => new AvgPriceByCategory
            {
                RestaurantName = x.RestaurantName,
                AveragePrice = (decimal)x.AveragePrice
            })
            .OrderByDescending(r => r.AveragePrice)
            .ToList();
        
        var model = new ReportViewModel
        {
            FullList = fullList,
            CountByRestaurant = countByRestaurant,
            AvgPriceByRestaurant = avgPriceByRestaurant
        };
        
        return View(model);
    }
}
