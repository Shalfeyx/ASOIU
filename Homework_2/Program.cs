using System.Text;
using RestaurantMenuApp_Console.Database;
using RestaurantMenuApp_Console.Reports;
using RestaurantMenuApp_Console.Models;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string dbPath = "restaurant.db";

// Исправляем пути к CSV-файлам
string baseDir = AppContext.BaseDirectory;
string restaurantsCsv = Path.GetFullPath(Path.Combine(baseDir, "../..", "Data/restaurants.csv"));
string menuItemsCsv = Path.GetFullPath(Path.Combine(baseDir, "../..", "Data/menuitems.csv"));

Console.WriteLine($"Поиск файлов в: {restaurantsCsv}");
Console.WriteLine($"Поиск файлов в: {menuItemsCsv}");

var db = new DatabaseManager(dbPath);
db.InitializeDatabase(restaurantsCsv, menuItemsCsv);

Console.WriteLine();
Console.WriteLine("Добро пожаловать в систему управления ресторанами и блюдами!");
Console.WriteLine();

string choice;
do
{
    Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║           УПРАВЛЕНИЕ РЕСТОРАНАМИ И БЛЮДАМИ               ║");
    Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
    Console.WriteLine("║  1 — Показать все рестораны                              ║");
    Console.WriteLine("║  2 — Показать все блюда                                  ║");
    Console.WriteLine("║  3 — Добавить блюдо                                      ║");
    Console.WriteLine("║  4 — Редактировать блюдо                                 ║");
    Console.WriteLine("║  5 — Удалить блюдо                                       ║");
    Console.WriteLine("║  6 — Отчёты                                              ║");
    Console.WriteLine("║  7 — Фильтр по ресторану [ГРУППА Г]                      ║");
    Console.WriteLine("║  0 — Выход                                               ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
    Console.Write("Ваш выбор: ");
    
    choice = Console.ReadLine()?.Trim() ?? "";
    Console.WriteLine();
    
    switch (choice)
    {
        case "1": ShowRestaurants(db); break;
        case "2": ShowMenuItems(db); break;
        case "3": AddMenuItem(db); break;
        case "4": EditMenuItem(db); break;
        case "5": DeleteMenuItem(db); break;
        case "6": ReportsMenu(db); break;
        case "7": FilterByRestaurant(db); break;
        case "0": Console.WriteLine("До свидания!"); break;
        default: Console.WriteLine("Неверный пункт меню."); break;
    }
    Console.WriteLine();
} while (choice != "0");

static void ShowRestaurants(DatabaseManager db)
{
    Console.WriteLine("---- Все рестораны ----");
    var restaurants = db.GetAllRestaurants();
    foreach (var r in restaurants)
        Console.WriteLine(" " + r);
    Console.WriteLine($"Итого: {restaurants.Count}");
}

static void ShowMenuItems(DatabaseManager db)
{
    Console.WriteLine("---- Все блюда ----");
    var items = db.GetAllMenuItems();
    foreach (var item in items)
        Console.WriteLine(" " + item);
    Console.WriteLine($"Итого: {items.Count}");
}

static void AddMenuItem(DatabaseManager db)
{
    Console.WriteLine("---- Добавление блюда ----");
    
    Console.WriteLine("Доступные рестораны:");
    var restaurants = db.GetAllRestaurants();
    foreach (var r in restaurants)
        Console.WriteLine(" " + r);
    
    Console.Write("ID ресторана: ");
    if (!int.TryParse(Console.ReadLine(), out int restId))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    
    Console.Write("Название блюда: ");
    string name = Console.ReadLine()?.Trim() ?? "";
    if (name.Length == 0)
    {
        Console.WriteLine("Ошибка: название не может быть пустым.");
        return;
    }
    
    Console.Write("Цена (руб.): ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal price))
    {
        Console.WriteLine("Ошибка: введите число.");
        return;
    }
    
    try
    {
        var item = new MenuItem(0, restId, name, price);
        db.AddMenuItem(item);
        Console.WriteLine("Блюдо добавлено.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

static void EditMenuItem(DatabaseManager db)
{
    Console.WriteLine("---- Редактирование блюда ----");
    Console.Write("Введите ID блюда: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    
    var item = db.GetMenuItemById(id);
    if (item == null)
    {
        Console.WriteLine($"Блюдо с ID={id} не найдено.");
        return;
    }
    
    Console.WriteLine($"Текущие данные: {item}");
    Console.WriteLine("(Нажмите Enter, чтобы оставить значение без изменений)");
    
    Console.Write($"Название [{item.Name}]: ");
    string input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0) item.Name = input;
    
    Console.Write($"ID ресторана [{item.RestaurantId}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && int.TryParse(input, out int newRestId))
        item.RestaurantId = newRestId;
    
    Console.Write($"Цена [{item.Price:F2}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && decimal.TryParse(input, out decimal newPrice))
    {
        try
        {
            item.Price = newPrice;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return;
        }
    }
    
    db.UpdateMenuItem(item);
    Console.WriteLine("Данные обновлены.");
}

static void DeleteMenuItem(DatabaseManager db)
{
    Console.WriteLine("---- Удаление блюда ----");
    Console.Write("Введите ID блюда: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    
    var item = db.GetMenuItemById(id);
    if (item == null)
    {
        Console.WriteLine($"Блюдо с ID={id} не найдено.");
        return;
    }
    
    Console.Write($"Удалить «{item.Name}»? (да/нет): ");
    string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
    if (confirm == "да")
    {
        db.DeleteMenuItem(id);
        Console.WriteLine("Блюдо удалено.");
    }
    else
    {
        Console.WriteLine("Удаление отменено.");
    }
}

static void ReportsMenu(DatabaseManager db)
{
    string choice;
    do
    {
        Console.WriteLine("--- Отчёты ---");
        Console.WriteLine(" 1 - Полный список блюд с названиями ресторанов");
        Console.WriteLine(" 2 - Количество блюд по ресторанам");
        Console.WriteLine(" 3 - Средняя цена блюд по ресторанам (сортировка по убыванию)");
        Console.WriteLine(" 0 - Назад");
        Console.Write("Ваш выбор: ");
        choice = Console.ReadLine()?.Trim() ?? "";
        switch (choice)
        {
            case "1": Report1_FullList(db); break;
            case "2": Report2_CountByRestaurant(db); break;
            case "3": Report3_AvgPriceByRestaurant(db); break;
            case "0": break;
            default: Console.WriteLine("Неверный пункт."); break;
        }
        Console.WriteLine();
    } while (choice != "0");
}

static void Report1_FullList(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT mi.item_name, r.restaurant_name, mi.price
                 FROM menu_items mi
                 JOIN restaurants r ON mi.restaurant_id = r.restaurant_id
                 ORDER BY mi.item_name")
        .Title("Полный список блюд с ресторанами")
        .Header("Блюдо", "Ресторан", "Цена (руб.)")
        .ColumnWidths(30, 30, 15)
        .Numbered()
        .Footer("Всего блюд")
        .Print();
}

static void Report2_CountByRestaurant(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT r.restaurant_name, COUNT(*) AS cnt
                 FROM menu_items mi
                 JOIN restaurants r ON mi.restaurant_id = r.restaurant_id
                 GROUP BY r.restaurant_name
                 ORDER BY r.restaurant_name")
        .Title("Количество блюд по ресторанам")
        .Header("Ресторан", "Кол-во блюд")
        .ColumnWidths(30, 15)
        .Print();
}

static void Report3_AvgPriceByRestaurant(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT r.restaurant_name, ROUND(AVG(mi.price), 2) AS avg_price
                 FROM menu_items mi
                 JOIN restaurants r ON mi.restaurant_id = r.restaurant_id
                 GROUP BY r.restaurant_name
                 ORDER BY avg_price DESC")
        .Title("Средняя цена блюд по ресторанам (убывание)")
        .Header("Ресторан", "Средняя цена (руб.)")
        .ColumnWidths(30, 20)
        .Print();
}

static void FilterByRestaurant(DatabaseManager db)
{
    Console.WriteLine("---- Фильтр по ресторану ----");
    Console.WriteLine("Доступные рестораны:");
    var restaurants = db.GetAllRestaurants();
    foreach (var r in restaurants)
        Console.WriteLine(" " + r);
    
    Console.Write("Введите ID ресторана: ");
    if (!int.TryParse(Console.ReadLine(), out int restId))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }
    
    var items = db.GetMenuItemsByRestaurant(restId);
    if (items.Count == 0)
    {
        Console.WriteLine("В этом ресторане нет блюд.");
        return;
    }
    
    Console.WriteLine($"\nБлюда ресторана #{restId}:");
    foreach (var item in items)
        Console.WriteLine(" " + item);
    Console.WriteLine($"Итого: {items.Count}");
}
