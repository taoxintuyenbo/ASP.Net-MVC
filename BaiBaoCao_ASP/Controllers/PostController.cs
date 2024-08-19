using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using BaiBaoCao_ASP.Models;

namespace BaiBaoCao_ASP.Controllers
{
    public class PostController : Controller
    {
        private ASPEntities db = new ASPEntities();

        // GET: Post
        public ActionResult Index()
        {

            var lstPost = db.posts.ToList();

            return View(lstPost);
        }
        public ActionResult Details(int id)
        {
            var post = db.posts.FirstOrDefault(p => p.id == id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }
    }
}