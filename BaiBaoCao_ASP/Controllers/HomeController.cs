using BaiBaoCao_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
 
namespace BaiBaoCao_ASP.Controllers
{
    public class HomeController : Controller
    {
        private ASPEntities db = new ASPEntities();

        // GET: Product
        public ActionResult Index()
        {
            var products = db.products.Take(5).ToList();

            HomeModel objHomeModel = new HomeModel();
            objHomeModel.ListCategory = db.categories.ToList();
            objHomeModel.ListProduct = db.products.ToList();
            objHomeModel.ListBrand = db.brands.ToList();
            //objHomeModel.ListBanner = db.banners.ToList();
            return View(objHomeModel);
            //return View(products);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Hello()
        {
            ViewBag.Message = "Your Hello page.";

            return View();
        }
        

    }

}