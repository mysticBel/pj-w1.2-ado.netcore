using Microsoft.AspNetCore.Mvc;

//para la cadena de conexion
using Microsoft.Extensions.Configuration;

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

        public IActionResult Index()
        {
            return View();
        }
    }
}
