using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Gateway.Web.Controllers
{
    [Route("profile")]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("order")]
        public IActionResult Order()
        {
            return View();
        }

        [Route("order/detail/{orderCode}")]
        public IActionResult Detail(string orderCode)
        {
            return View("detail", orderCode);
        }
    }
}