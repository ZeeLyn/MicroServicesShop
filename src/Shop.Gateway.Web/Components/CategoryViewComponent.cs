using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.IGoods;

namespace Shop.Gateway.Web.Components
{
    public class CategoryViewComponent : ViewComponent
    {
        private IGoodsCategoryService CategoryService { get; }

        public CategoryViewComponent(IGoodsCategoryService categoryService)
        {
            CategoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(await CategoryService.List());
        }
    }
}
