namespace Inventory_Project.Models
{
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Item>? Items { get; set; }
    }
}
