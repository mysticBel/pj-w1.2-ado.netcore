using Microsoft.AspNetCore.Mvc;



namespace sesion02.Controllers
{
    public class NegociosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
