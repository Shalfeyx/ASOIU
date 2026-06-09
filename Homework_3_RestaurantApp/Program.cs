using Microsoft.EntityFrameworkCore;
using RestaurantMenuApp.Data;
using RestaurantMenuApp.Models;

namespace RestaurantMenuApp;

/// <summary>
/// Главный класс приложения.
/// </summary>
public class Program
{
    /// <summary>
    /// Точка входа в приложение.
    /// </summary>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Добавление сервисов MVC
        builder.Services.AddControllersWithViews();
        
        var app = builder.Build();
        
        // Создание БД и начальное заполнение данных
        using (var scope = app.Services.CreateScope())
        {
            var context = new AppDbContext();
            context.Database.EnsureCreated();          // создаём БД, если её нет
            SeedData.Initialize(context);              // заполняем начальными данными
        }
        
        // Конвейер запросов
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Restaurants}/{action=Index}/{id?}");
        
        app.Run();
    }
}

/// <summary>
/// Класс для начального заполнения базы данных.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Инициализация БД начальными данными, если таблицы пусты.
    /// </summary>
    public static void Initialize(AppDbContext context)
    {
        // Добавляем рестораны, если их нет
        if (!context.Restaurants.Any())
        {
            var restaurants = new[]
            {
                new Restaurant { Name = "Итальянский дворик" },
                new Restaurant { Name = "Японская кухня Сакура" },
                new Restaurant { Name = "Мексиканский кантри" },
                new Restaurant { Name = "Стейк-хаус Золотой бык" }
            };
            context.Restaurants.AddRange(restaurants);
            context.SaveChanges();
        }
        
        // Добавляем блюда, если их нет
        if (!context.MenuItems.Any())
        {
            var italian = context.Restaurants.First(r => r.Name == "Итальянский дворик");
            var japanese = context.Restaurants.First(r => r.Name == "Японская кухня Сакура");
            var mexican = context.Restaurants.First(r => r.Name == "Мексиканский кантри");
            var steakhouse = context.Restaurants.First(r => r.Name == "Стейк-хаус Золотой бык");
            
            var menuItems = new[]
            {
                new MenuItem { Name = "Пицца Маргарита", Price = 450m, RestaurantId = italian.Id },
                new MenuItem { Name = "Спагетти Карбонара", Price = 380m, RestaurantId = italian.Id },
                new MenuItem { Name = "Тирамису", Price = 250m, RestaurantId = italian.Id },
                new MenuItem { Name = "Суши Филадельфия", Price = 550m, RestaurantId = japanese.Id },
                new MenuItem { Name = "Рамен", Price = 420m, RestaurantId = japanese.Id },
                new MenuItem { Name = "Моти с клубникой", Price = 300m, RestaurantId = japanese.Id },
                new MenuItem { Name = "Тако", Price = 320m, RestaurantId = mexican.Id },
                new MenuItem { Name = "Бурито", Price = 400m, RestaurantId = mexican.Id },
                new MenuItem { Name = "Чипотле", Price = 280m, RestaurantId = mexican.Id },
                new MenuItem { Name = "Стейк Рибай", Price = 1200m, RestaurantId = steakhouse.Id },
                new MenuItem { Name = "Бургер с говядиной", Price = 490m, RestaurantId = steakhouse.Id },
                new MenuItem { Name = "Картофель фри", Price = 150m, RestaurantId = steakhouse.Id }
            };
            context.MenuItems.AddRange(menuItems);
            context.SaveChanges();
        }
    }
}