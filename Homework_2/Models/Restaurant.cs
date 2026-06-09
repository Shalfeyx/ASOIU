namespace RestaurantMenuApp_Console.Models;

/// <summary>
/// Ресторан (справочная таблица, сторона "один")
/// </summary>
public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public Restaurant(int id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public Restaurant() : this(0, "") { }
    
    public override string ToString() => $"[{Id}] {Name}";
}
