using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Gateway.Web.Models;
using Shop.IGoods;

namespace Shop.Gateway.Web.Controllers
{
    public class HomeController : Controller
    {
        private IGoodsService GoodsService { get; }

        public HomeController(IGoodsService goodsService)
        {
            GoodsService = goodsService;
        }
        public async Task<IActionResult> Index()
        {
            //var result = await GoodsService.GoodsList(1);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
