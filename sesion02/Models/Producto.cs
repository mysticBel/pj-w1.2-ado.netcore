namespace sesion02.Models
{
    public class Producto
    {
        public int idProducto { get; set; }
        public string? nombreProducto { get; set; }
        public string? nombreCategoria { get; set; }
        public decimal precioUnitario { get; set; }
        public int stock { get; set; }
    }
}
