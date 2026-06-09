using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantMenuApp.Data;
using RestaurantMenuApp.Models;

namespace RestaurantMenuApp.Controllers;

/// <summary>
/// Контроллер для работы с основной таблицей "Блюда".
/// </summary>
public class MenuItemsController : Controller
{
    /// <summary>
    /// Отображает список всех блюд с названиями ресторанов.
    /// </summary>
    public IActionResult Index()
    {
        using var context = new AppDbContext();
        var menuItems = context.MenuItems
            .Include(m => m.Restaurant)
            .OrderBy(m => m.Name)
            .ToList();
        return View(menuItems);
    }
    
    /// <summary>
    /// GET-форма создания нового блюда (выпадающий список ресторанов).
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        using var context = new AppDbContext();
        ViewBag.Restaurants = context.Restaurants.OrderBy(r => r.Name).ToList();
        return View();
    }
    
    /// <summary>
    /// POST-обработчик создания блюда с валидацией цены.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(MenuItem menuItem)
    {
        if (ModelState.IsValid)
        {
            using var context = new AppDbContext();
            context.MenuItems.Add(menuItem);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        using var context2 = new AppDbContext();
        ViewBag.Restaurants = context2.Restaurants.OrderBy(r => r.Name).ToList();
        return View(menuItem);
    }
    
    /// <summary>
    /// GET-форма редактирования блюда.
    /// </summary>
    [HttpGet]
    public IActionResult Edit(int id)
    {
        using var context = new AppDbContext();
        var menuItem = context.MenuItems.Find(id);
        if (menuItem == null) return NotFound();
        ViewBag.Restaurants = context.Restaurants.OrderBy(r => r.Name).ToList();
        return View(menuItem);
    }
    
    /// <summary>
    /// POST-обработчик редактирования блюда.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, MenuItem menuItem)
    {
        if (id != menuItem.Id) return NotFound();
        
        if (ModelState.IsValid)
        {
            using var context = new AppDbContext();
            context.Entry(menuItem).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        using var context2 = new AppDbContext();
        ViewBag.Restaurants = context2.Restaurants.OrderBy(r => r.Name).ToList();
        return View(menuItem);
    }
    
    /// <summary>
    /// GET-страница подтверждения удаления блюда.
    /// </summary>
    [HttpGet]
    public IActionResult Delete(int id)
    {
        using var context = new AppDbContext();
        var menuItem = context.MenuItems.Include(m => m.Restaurant).FirstOrDefault(m => m.Id == id);
        if (menuItem == null) return NotFound();
        return View(menuItem);
    }
    
    /// <summary>
    /// POST-обработчик удаления блюда.
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        using var context = new AppDbContext();
        var menuItem = context.MenuItems.Find(id);
        if (menuItem != null)
        {
            context.MenuItems.Remove(menuItem);
            context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}
