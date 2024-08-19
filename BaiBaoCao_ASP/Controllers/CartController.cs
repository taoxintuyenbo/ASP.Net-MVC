using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaiBaoCao_ASP.Models;

namespace BaiBaoCao_ASP.Controllers
{
    public class CartController : Controller
    {
        private ASPEntities db = new ASPEntities();

        // GET: Cart
        public ActionResult Index()
        {
            var cart = (List<CartModel>)Session["cart"];
            if (cart != null && cart.Any())
            {
                ViewBag.TotalPrice = cart.Sum(item => item.Product.price * item.Quantity);
                ViewBag.Discount = cart.Sum(item => item.Product.pricesale.HasValue ? (item.Product.price - item.Product.pricesale.Value) * item.Quantity : 0);
                ViewBag.GrandTotal = ViewBag.TotalPrice - ViewBag.Discount;
            }
            else
            {
                ViewBag.TotalPrice = 0;
                ViewBag.Discount = 0;
                ViewBag.GrandTotal = 0;
            }
            return View((List<CartModel>)Session["cart"]);
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            if (Session["cart"] == null)
            {
                List<CartModel> cart = new List<CartModel>();
                cart.Add(new CartModel { Product = db.products.Find(id), Quantity = quantity });
                Session["cart"] = cart;
                Session["count"] = 1;
            }
            else
            {
                List<CartModel> cart = (List<CartModel>)Session["cart"];
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartModel { Product = db.products.Find(id), Quantity = quantity });
                    Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                }
                Session["cart"] = cart;
            }

            int cartCount = ((List<CartModel>)Session["cart"]).Count;
            return Json(new { Message = "Thành công", CartCount = cartCount, CartItems = Session["cart"] }, JsonRequestBehavior.AllowGet);
        }
 


         

        [HttpPost]
        public JsonResult UpdateQuantity(int id, int quantity)
        {
            var cart = (List<CartModel>)Session["cart"];
            if (cart != null)
            {
                int index = cart.FindIndex(c => c.Product.id == id);
                if (index != -1)
                {
                    cart[index].Quantity = quantity;
                    Session["cart"] = cart;

                    var totalItemPrice = cart[index].Quantity * cart[index].Product.price;
                    var totalPrice = cart.Sum(item => item.Product.price * item.Quantity);
                    var discount = cart.Sum(item => item.Product.pricesale.HasValue ? (item.Product.price - item.Product.pricesale.Value) * item.Quantity : 0);
                    var grandTotal = totalPrice - discount;

                    return Json(new
                    {
                        Message = "Thanh cong",
                        TotalPrice = totalPrice.ToString(),
                        Discount = discount.ToString(),
                        GrandTotal = grandTotal.ToString(),
                        TotalItemPrice = totalItemPrice.ToString()
                    });
                }
            }

            return Json(new { Message = "Error: Product not found in cart" });
        }

        private int isExist(int id)
        {
            List<CartModel> cart = (List<CartModel>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.id.Equals(id))
                    return i;
            return -1;
        }
        // Remove a product from the cart by id



        [HttpPost]
        public ActionResult Remove(int id)
        {
            List<CartModel> cart = (List<CartModel>)Session["cart"];
            cart.RemoveAll(x => x.Product.id == id);
            Session["cart"] = cart;
            Session["count"] = Convert.ToInt32(Session["count"]) - 1;
            return Json(new { Message = "Thành công", CartItems = cart }, JsonRequestBehavior.AllowGet);
        }
        private void ClearCart()
        {
            Session["cart"] = null;
            Session["count"] = 0;
        }

    
        public ActionResult Payment()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                // Retrieve cart information from session
                var lstCart = (List<CartModel>)Session["cart"];
                if (lstCart == null || !lstCart.Any())
                {
                    // Handle empty cart scenario
                    return RedirectToAction("Index", "Cart");
                }

                // Create a new order
                order objOrder = new order();
                objOrder.name = "DonHang" + DateTime.Now.ToString("yyyyMMddHHmmss");
                objOrder.userid = int.Parse(Session["UserId"].ToString());
                objOrder.CreatedOnUtc = DateTime.Now;
                objOrder.address = Session["Address"] != null ? Session["Address"].ToString() : "Default Address"; // Add address field
                objOrder.status = 1;

                db.orders.Add(objOrder);
                db.SaveChanges(); // Save order to generate order ID

                // Retrieve generated order ID
                int intOrderId = objOrder.id;

                // Calculate total amount for the order
                List<orderdetail> lstOrderDetail = new List<orderdetail>();
                decimal totalAmount = 0;
                foreach (var item in lstCart)
                {
                    orderdetail obj = new orderdetail();
                    obj.qty = item.Quantity;
                    obj.order_id = intOrderId;
                    obj.product_id = (int)item.Product.id; // Assuming product_id is int
                    obj.price = (decimal)item.Product.price; // Explicitly convert price to decimal
                    obj.discount = 0; // Assuming no discount for now
                    obj.amount = item.Quantity * obj.price; // Calculate amount and convert to decimal
                    obj.created_at = DateTime.Now;
                    obj.updated_at = DateTime.Now;

                    totalAmount += obj.amount; // Sum up the total amount

                    lstOrderDetail.Add(obj);
                }

                db.orderdetails.AddRange(lstOrderDetail);
                db.SaveChanges();

                // Update the order with the total amount
                objOrder.amount = (double?)totalAmount;
                db.Entry(objOrder).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                Session["TotalAmount"] = totalAmount;

                // Retrieve user information from session
                string userName = $"{Session["Firstname"]} {Session["Lastname"]}";
                string userPhone = Session["Phone"].ToString();
                string userEmail = Session["Email"].ToString();
                string userAddress = Session["Address"].ToString();

                // Construct the email content
                string content = $"Xin chào {userName},\n\n";
                content += "Cảm ơn bạn đã mua hàng!\n";
                content += $"Đơn hàng của bạn với tổng số tiền {totalAmount.ToString("N0")} VND đã được đặt thành công.\n\n";
                content += "Chi tiết đơn hàng:\n";
                content += $"Số điện thoại: {userPhone}\n";
                content += $"Email: {userEmail}\n";
                content += $"Địa chỉ: {userAddress}\n\n";
                content += "Chúng tôi sẽ thông báo cho bạn khi đơn hàng của bạn đã được chuyển đi.\n";
                content += "Cảm ơn bạn đã mua sắm với chúng tôi!\n";

                string subject = "Xác nhận đơn hàng - OnlineShop";
                MailHelper mailHelper = new MailHelper();

                try
                {
                    bool emailSent = mailHelper.SendMail(userEmail, subject, content);
                    if (emailSent)
                    {
                        System.Diagnostics.Debug.WriteLine("Email xác nhận đơn hàng đã gửi tới " + userEmail);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Không thể gửi email xác nhận đơn hàng tới " + userEmail);
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine("Gửi email thất bại: " + ex.Message);
                }

                // Clear the cart
                ClearCart();
 
                return RedirectToAction("PaymentSuccess", "Cart");
            }
        }

        public ActionResult PaymentSuccess()
        {
            return View();
        }
    }
}
