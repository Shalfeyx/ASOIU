using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantMenuApp.Data;
using RestaurantMenuApp.Models;

namespace RestaurantMenuApp.Controllers;

public class RestaurantsController : Controller
{
    public IActionResult Index()
    {
        using var context = new AppDbContext();
        var restaurants = context.Restaurants
            .Include(r => r.MenuItems)
            .OrderBy(r => r.Name)
            .ToList();
        return View(restaurants);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Restaurant restaurant, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/restaurants");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                restaurant.ImagePath = "/images/restaurants/" + fileName;
            }
            
            using var context = new AppDbContext();
            context.Restaurants.Add(restaurant);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(restaurant);
    }
    
    [HttpGet]
    public IActionResult Edit(int id)
    {
        using var context = new AppDbContext();
        var restaurant = context.Restaurants.Find(id);
        if (restaurant == null) return NotFound();
        return View(restaurant);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Restaurant restaurant, IFormFile? imageFile)
    {
        if (id != restaurant.Id) return NotFound();
        
        if (ModelState.IsValid)
        {
            using var context = new AppDbContext();
            var existing = await context.Restaurants.FindAsync(id);
            if (existing == null) return NotFound();
            
            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existing.ImagePath))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + existing.ImagePath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }
                
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/restaurants");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                existing.ImagePath = "/images/restaurants/" + fileName;
            }
            
            existing.Name = restaurant.Name;
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(restaurant);
    }
    
    [HttpGet]
    public IActionResult Delete(int id)
    {
        using var context = new AppDbContext();
        var restaurant = context.Restaurants
            .Include(r => r.MenuItems)
            .FirstOrDefault(r => r.Id == id);
        if (restaurant == null) return NotFound();
        return View(restaurant);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        using var context = new AppDbContext();
        var restaurant = await context.Restaurants
            .Include(r => r.MenuItems)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (restaurant == null) return NotFound();
        
        if (restaurant.MenuItems.Any())
        {
            TempData["ErrorMessage"] = "Невозможно удалить ресторан, так как за ним закреплены блюда.";
            return RedirectToAction(nameof(Index));
        }
        
        if (!string.IsNullOrEmpty(restaurant.ImagePath))
        {
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + restaurant.ImagePath);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
        }
        
        context.Restaurants.Remove(restaurant);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
