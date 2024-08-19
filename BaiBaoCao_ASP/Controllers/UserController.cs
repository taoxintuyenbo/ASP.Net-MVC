using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BaiBaoCao_ASP.Helpers;
using BaiBaoCao_ASP.Models;

namespace BaiBaoCao_ASP.Controllers
{
    public class UserController : Controller
    {
        private ASPEntities db = new ASPEntities();

        // GET: User
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        public ActionResult Register()
        {
            return View(new user());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "id,firstname,lastname,username,password,gender,phone,email,address")] user user)
        {
            int flag = 0;

            // Basic null or empty checks and other validation
            if (string.IsNullOrEmpty(user.firstname))
            {
                ModelState.AddModelError("firstname", "Họ không được để trống.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(user.lastname))
            {
                ModelState.AddModelError("lastname", "Tên không được để trống.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(user.username))
            {
                ModelState.AddModelError("username", "Tên đăng nhập không được để trống.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(user.password))
            {
                ModelState.AddModelError("password", "Mật khẩu không được để trống.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(user.email))
            {
                ModelState.AddModelError("email", "Email không được để trống.");
                flag = 1;
            }
            else
            {
                // Check if the email already exists
                var existingUser = db.users.FirstOrDefault(u => u.email == user.email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("email", "Email đã tồn tại. Vui lòng sử dụng email khác.");
                    flag = 1;
                }
            }
            if (string.IsNullOrEmpty(user.phone))
            {
                ModelState.AddModelError("phone", "Điện thoại không được để trống.");
                flag = 1;
            }
            else if (!user.phone.All(char.IsDigit))
            {
                ModelState.AddModelError("phone", "Số điện thoại chỉ được chứa chữ số.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(user.address))
            {
                ModelState.AddModelError("address", "Địa chỉ không được để trống.");
                flag = 1;
            }

            // Additional validation for password confirmation
            if (!string.Equals(user.password, Request.Form["confirm_password"]))
            {
                ModelState.AddModelError("confirm_password", "Mật khẩu và nhập lại mật khẩu không khớp.");
                flag = 1;
            }

            // If flag is set to 1, return to the view with the validation errors
            if (flag == 1)
            {
                return View(user);
            }

            // Hash the password using MD5 (consider using a stronger hash function like BCrypt)
            user.password = PasswordHelper.GetMd5Hash(user.password);

            db.users.Add(user);
            db.SaveChanges();

            // Set success message
           TempData["SuccessMessage"] = "Dang ky thanh cong! Vui long đang nhap.";

            return RedirectToAction("Login");
        }


        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName,Password")] user model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.username) || string.IsNullOrEmpty(model.password))
                {
                    if (string.IsNullOrEmpty(model.username))
                    {
                        ModelState.AddModelError("Username", "Username khong duoc de trong.");
                    }

                    if (string.IsNullOrEmpty(model.password))
                    {
                        ModelState.AddModelError("Password", "Password khong duoc de trong.");
                    }

                    return View(); // Return the view with validation errors
                }
                var user = db.users.SingleOrDefault(u => u.username == model.username);
                if (user != null && PasswordHelper.VerifyMd5Hash(model.password, user.password))
                {
                    // Successfully authenticated, set up session or authentication cookie
                    // Example:
                    Session["UserName"] = user.username;
                    Session["UserId"] = user.id;
                    Session["Firstname"] = user.firstname;
                    Session["Lastname"] = user.lastname;
                    Session["Address"] = user.address;
                    Session["Email"] = user.email;
                    Session["Phone"] = user.phone;
                    Session["Roles"] = user.roles;
                     return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError("Incorrect", "Username hoac password khong dung.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Home/Index

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,firstname,lastname,username,password,gender,phone,email,address")] user user)
        {
            if (ModelState.IsValid)
            {
                db.users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,firstname,lastname,username,password,gender,phone,email,address")] user user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user user = db.users.Find(id);
            db.users.Remove(user);
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
        public  ActionResult UserOrder(int id)
        {
            var user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var orders = db.orders.Where(o => o.userid == id).ToList();
            var ordersWithDetails = orders.Select(o => new OrderWithDetailsModel
            {
                Order = o,
                OrderDetails = db.orderdetails.Where(od => od.order_id == o.id).ToList()
            }).ToList();
            var viewModel = new UserProfileViewModel
            {
                User = user,
                Orders = ordersWithDetails
            };
            return View(viewModel);
        }
        public new ActionResult Profile(int id)
        {
            var user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var orders = db.orders.Where(o => o.userid == id).ToList();
            var ordersWithDetails = orders.Select(o => new OrderWithDetailsModel
            {
                Order = o,
                OrderDetails = db.orderdetails.Where(od => od.order_id == o.id).ToList()
            }).ToList();
            var viewModel = new UserProfileViewModel
            {
                User = user,
                Orders = ordersWithDetails
            };
            return View(viewModel);
        }

    }
}
