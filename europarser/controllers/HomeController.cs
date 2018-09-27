using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace europarser.controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Loader() => View();
    }
}