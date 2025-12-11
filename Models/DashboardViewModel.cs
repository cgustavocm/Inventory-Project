using System.Collections.Generic;

namespace Inventory_Project.Models
{
    public class DashboardViewModel
    {
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }

        // ESTA propiedad es la que usas en la vista
        public List<CategorySummary> CategorySummary { get; set; } = new();
    }

    public class CategorySummary
    {
        public string CategoryName { get; set; }
        public int ItemCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }
}
