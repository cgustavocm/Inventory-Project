using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;   // 👈 IMPORTANTE

namespace Inventory_Project.Models
{
    public class Item
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Display(Name = "Serial Number / Code")]
        [StringLength(50)]
        public string ItemCode { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(260)]
        public string? ImagePath { get; set; }

        [NotMapped]
        [Display(Name = "Product Image")]
        public IFormFile? ImageFile { get; set; }
    }
}

