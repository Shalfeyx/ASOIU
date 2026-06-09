using System.ComponentModel.DataAnnotations;

namespace RestaurantMenuApp.Models;

public class Restaurant
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Название ресторана обязательно")]
    [Display(Name = "Название ресторана")]
    public string Name { get; set; } = "";
    
    [Display(Name = "Фото ресторана")]
    public string? ImagePath { get; set; }
    
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
