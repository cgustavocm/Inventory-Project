namespace Inventory_Project.Models
{
    public class ErrorViewModel //Error en la aplicacion 
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
