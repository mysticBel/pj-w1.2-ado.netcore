using Microsoft.AspNetCore.Mvc;

//para la cadena de conexion
using Microsoft.Extensions.Configuration;
//
using Microsoft.Data.SqlClient;
using sesion02.Models;
//
using System.Data; //para los sp
using Microsoft.AspNetCore.Mvc.Rendering;

namespace sesion02.Controllers
{
    public class NegociosController : Controller
    {

        // W1 - action result Negocios para Producto Model
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


        // w2.1 -PAGINACION
        public async Task<IActionResult> PaginacionProductos(int pagina = 0)
        {
            IEnumerable<Producto> productos = GetProductos();

            int filasPagina = 4;  // indico que tome 4 items por pagina
            int totalFilas = productos.Count();
            int numeroPaginas = totalFilas % filasPagina == 0 ? totalFilas / filasPagina : (totalFilas / filasPagina) + 1;


            // Ejemplo : totalFilas = 8 productos
            // -- 1er PAGINA item 1, 2, 3 , 4    
            // int  pagina = 0  --> muestra a pratir de la posicion 0 --> 0 * 4 = 0 osea pagina * filasPagina
            // -- 2da PAGINA 5, 6, 7, 8         
            // int  pagina = 1  --> muestra desde la posicion 4       --> 1 * 4 = 4
            // total tendriamos 2 paginas  ( total filas/ 4 items)

            //si fuese 14
            //totalPaginas = 14/ 4 = 3.5 => 3+1 = 4 paginas

            ViewBag.pagina = pagina; //necesitamos enviar la pagina que estamos
            ViewBag.numeroPaginas = numeroPaginas; // y el numero de paginas


            return View(await Task.Run(() => productos.Skip(pagina * filasPagina).Take(filasPagina)));
        }


        // w2.1 - busqueda por producto 
        IEnumerable<Producto> GetProductosByName(string nombre)
        {
            List<Producto> productos = new List<Producto>();

            using (SqlConnection connection = new SqlConnection(this.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_GetProductosByName", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@prmNombre", nombre);
                //aqui agregariamos mas parametros de haberlo

                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    productos.Add(new Producto()
                    {
                        idProducto = dr.GetInt32(0),
                        nombreProducto= dr.GetString(1),
                        nombreCategoria = dr.GetString(2),
                        precioUnitario= dr.GetDecimal(3),
                        stock = dr.GetInt32(4)
                    });
                }
            }
            return productos;
        }

        // su ActionRESULT  + PAGINACION
        public async Task<IActionResult> FiltroProductos(string nombre="", int pagina = 0)
        {
            //validacion por si no se envia nada
            if (nombre == null)
                nombre = "";

            IEnumerable<Producto> productos = GetProductosByName(nombre);
            int filasPagina = 5;  // indico que tome 5 items por pagina
            int totalFilas = productos.Count();
            int numeroPaginas = totalFilas % filasPagina == 0 ? totalFilas / filasPagina : (totalFilas / filasPagina) + 1;

            ViewBag.pagina = pagina;
            ViewBag.numeroPaginas = numeroPaginas;
            ViewBag.nombre = nombre;

            return View(await Task.Run(() => productos.Skip(pagina * filasPagina).Take(filasPagina)));

        }

        // w2.1. - Traemos la lista de Categorias
        IEnumerable<Categoria> GetCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();

            using (SqlConnection connection = new SqlConnection(cadena)) 
            {
                SqlCommand command = new SqlCommand("sp_GetCategorias", connection); 
                command.CommandType = CommandType.StoredProcedure;
                
                connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    categorias.Add(new Categoria()
                    {
                        idCategoria = dr.GetInt32(0),
                        nombreCategoria = dr.GetString(1)
                    });
                }

            }

            return categorias;
        }

        // -- su action result de GetCategorias
        public async Task<IActionResult> CreateProducto() 
        {
            ViewBag.categorias = new SelectList(
                    await Task.Run( () => GetCategorias() ),
                    "idCategoria",
                    "nombreCategoria"
                );

            return View( new Producto());
        }
    }
}
