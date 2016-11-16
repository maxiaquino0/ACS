using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Sobre nosotros.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Nuestros medios de contacto.";

            return View();
        }
    }
}