using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantMenuApp.Models;

public class MenuItem
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Название блюда обязательно")]
    [Display(Name = "Название блюда")]
    public string Name { get; set; } = "";
    
    [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
    [DataType(DataType.Currency)]
    [Display(Name = "Цена (руб.)")]
    public decimal Price { get; set; }
    
    [ForeignKey("Restaurant")]
    [Display(Name = "Ресторан")]
    public int RestaurantId { get; set; }
    
    public Restaurant? Restaurant { get; set; }
    
    [Display(Name = "Фото блюда")]
    public string? ImagePath { get; set; }
}
