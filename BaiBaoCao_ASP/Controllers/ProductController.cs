using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BaiBaoCao_ASP.Models;
using PagedList;

namespace BaiBaoCao_ASP.Controllers
{
    public class ProductController : Controller
    {
        private ASPEntities db = new ASPEntities();

        // GET: Product

        public ActionResult Index(string sortOrder, int? page)
        {
            var products = db.products.AsQueryable();

            // Apply sorting based on sortOrder
            switch (sortOrder)
            {
                case "latest":
                    products = products.OrderByDescending(p => p.created_at);
                    break;
                case "oldest":
                    products = products.OrderBy(p => p.created_at);
                    break;
                case "priceAsc":
                    products = products.OrderBy(p => p.pricesale ?? p.price);
                    break;
                case "priceDesc":
                    products = products.OrderByDescending(p => p.pricesale ?? p.price);
                    break;
                default:
                    products = products.OrderByDescending(p => p.created_at); // Default to latest items
                    break;
            }

            ViewBag.SortOrder = sortOrder;
            int pageSize = 8; // Number of items per page
            int pageNumber = (page ?? 1);

            return View(products.ToPagedList(pageNumber, pageSize));
        }


        // GET: Product/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

    
        public ActionResult Search(string searchType, string query, string sortOrder)
        {
            var products = db.products.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                switch (searchType)
                {
                    case "Product":
                        products = products.Where(p => p.name.Contains(query));
                        break;
                    case "Category":
                        products = from p in db.products
                                   join c in db.categories on p.category_id equals c.id
                                   where c.name.Contains(query)
                                   select p;
                        break;
                    case "Brand":
                        products = from p in db.products
                                   join b in db.brands on p.brand_id equals b.id
                                   where b.name.Contains(query)
                                   select p;
                        break;
                    default:
                        break;
                }
            }

            // Apply sorting based on sortOrder
            switch (sortOrder)
            {
                case "latest":
                    products = products.OrderByDescending(p => p.created_at);
                    break;
                case "oldest":
                    products = products.OrderBy(p => p.created_at);
                    break;
                case "priceAsc":
                    products = products.OrderBy(p => p.pricesale ?? p.price); // Sort by price or sale price if available
                    break;
                case "priceDesc":
                    products = products.OrderByDescending(p => p.pricesale ?? p.price); // Sort by price or sale price if available
                    break;
                default:
                    products = products.OrderByDescending(p => p.created_at); // Default to latest items
                    break;
            }

            ViewBag.SearchType = searchType;
            ViewBag.Query = query;
            ViewBag.SortOrder = sortOrder;

            return View(products.ToList());
        }
        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,category_id,brand_id,name,slug,detail,description,image,price,pricesale,created_at,updated_at,created_by,updated_by,status,qty")] product product)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,category_id,brand_id,name,slug,detail,description,image,price,pricesale,created_at,updated_at,created_by,updated_by,status,qty")] product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            product product = db.products.Find(id);
            db.products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
