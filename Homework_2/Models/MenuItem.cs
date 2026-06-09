using System;

namespace RestaurantMenuApp_Console.Models;

/// <summary>
/// Блюдо (основная таблица, сторона "много")
/// </summary>
public class MenuItem
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string Name { get; set; }
    
    private decimal _price;
    
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentException("Цена не может быть отрицательной");
            _price = value;
        }
    }
    
    public MenuItem(int id, int restaurantId, string name, decimal price)
    {
        Id = id;
        RestaurantId = restaurantId;
        Name = name;
        Price = price;
    }
    
    public MenuItem() : this(0, 0, "", 0) { }
    
    public override string ToString() => $"[{Id}] {Name}, ресторан #{RestaurantId}, цена: {Price:F2} руб.";
}
