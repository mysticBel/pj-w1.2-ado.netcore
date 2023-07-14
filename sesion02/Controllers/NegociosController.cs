using Microsoft.AspNetCore.Mvc;

//para la cadena de conexion
using Microsoft.Extensions.Configuration;
//
using Microsoft.Data.SqlClient;
using sesion02.Models;
//
using System.Data; //para los sp

namespace sesion02.Controllers
{
    public class NegociosController : Controller
    {

        //
        public readonly IConfiguration _config;
        public string cadena;
        public NegociosController(IConfiguration _config)
        {
            this._config = _config;  //hace referencia al 2do config, this config a la 1er(var global)
            this.cadena = _config["ConnectionStrings:connection"]; //obtenemos la cadena de appsettings.json
        }

        // ActionRESULT es mejor llamarlos en FORMA ASINCRONA (promesas en javascript)
        public async Task<IActionResult> Index()
        {
            return View( await Task.Run( () => GetProductos() ) );
        }


        //public IActionResult Index()
        //{
        //    return View(GetProductos());
        //}

        // usando microsoft.dATA
        IEnumerable<Producto> GetProductos() 
        { 
            List<Producto> productos = new List<Producto>();

            using (SqlConnection connection = new SqlConnection(cadena)) 
            {
                /*SqlCommand cmd = new SqlCommand("SELECT P.idProducto, " +
                                "  P.nombreProducto, C.nombreCategoria, " +
                                "  P.precioUnitario, P.stock\r\n" +
                                "  FROM PRODUCTO P\r\n" +
                 "  INNER JOIN CATEGORIA C ON C.idCategoria = P.idCategoria", connection);
                */
                //Es un texto, la version 2 se indica que es un sp
                //SqlCommand cmd = new SqlCommand("EXEC sp_GetProductos", connection);

                SqlCommand cmd = new SqlCommand("sp_GetProductos", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    productos.Add(new Producto()
                    {
                        idProducto = dr.GetInt32(0),
                        nombreProducto = dr.GetString(1),
                        nombreCategoria= dr.GetString(2),
                        precioUnitario = dr.GetDecimal(3),
                        stock = dr.GetInt32(4)
                    });
                }
            }

            return productos;
        }
    }
}
