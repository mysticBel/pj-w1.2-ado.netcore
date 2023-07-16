using System.ComponentModel.DataAnnotations;
namespace sesion02.Models


{
    public class Producto
    {
        [Display(Name="ID")]
        public int idProducto { get; set; }
        [Display(Name = "Nombre")]
        public string? nombreProducto { get; set; }

        //para el combobox
        [Display(Name = "Categoria")]
        public int idCategoria { get; set; }

        [Display(Name = "Nombre Categoria")]
        public string? nombreCategoria { get; set; }
        [Display(Name = "Precio Unitario")]
        public decimal precioUnitario { get; set; }
        [Display(Name = "Stock")]
        public int stock { get; set; }
    }
}
