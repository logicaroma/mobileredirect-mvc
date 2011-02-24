using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mobile.Redirect.Example.Web.Utils;

namespace Mobile.Redirect.Example.Web.Controllers
{
    public class DesktopController : Controller
    {
        [MobileRedirect]
        public ActionResult Index()
        {
            return View();
        }

        [MobileRedirect("list")]
        public ActionResult ShowAll()
        {
            return View();
        }

        [MobileRedirect("list/:tag")]
        public ActionResult ShowAllByTag(string tag)
        {
            ViewBag.Tag = tag;
            return View();
        }

        [MobileRedirect("details/:id")]
        public ActionResult Show(int id)
        {
            ViewBag.Id = id;
            return View(id);
        }
    }
}
