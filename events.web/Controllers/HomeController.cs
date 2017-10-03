using events.dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using events.parser.Engine;
using events.parser.Parsers;
using events.parser.Parsers.MicroData;
using System.Globalization;

namespace events.web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
