using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Net;
using System.Web.Mvc;
using PBShop.Models;
using System.Net.Mail;

namespace PBShop.Controllers
{
    public class UserController : Controller
    {
        PBShopEntities db = new PBShopEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var sEmail = collection["Email"];
            var sPassword = collection["Password"];
            if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err1"] = "Bạn chưa nhập Email";
            }
            else if (String.IsNullOrEmpty(sPassword))
            {
                ViewData["err2"] = "Bạn chưa nhập mật khẩu";
            }
            else
            {
                Customer kh = db.Customers.SingleOrDefault(n => n.Email == sEmail && n.Password == sPassword);
                if (kh != null)
                {

                    Session["NameCustomers"] = kh.Name;
                    Session["Customers"] = kh;
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    return RedirectToAction("Index","PBSopHome");
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            if (Session["Customers"] != null)
            {
                Session["NameCustomers"] = null;
                Session["Customers"] = null;
            }    
            return RedirectToAction("Index","PBSopHome");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection collection, Customer kh)
        {

            var sHoTen = collection["Name"];
            var sMatKhau = collection["Password"];
            var sEmail = collection["Email"];
            if (String.IsNullOrEmpty(sHoTen))
            {
                ViewData["err1"] = "Họ tên không được rỗng";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["err3"] = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err5"] = "Email không được rỗng";
            }
            else
            {
                kh.Name = sHoTen;
                kh.Password = sMatKhau.ToString();
                kh.Email = sEmail.ToString();
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Customers.Add(kh);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return this.Register();
        }

        [HttpGet]
        public ActionResult ResetPW(int? ID)
        {
            
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(ID);
            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        [HttpPost]
        public ActionResult resetPW([Bind(Include = "Email,Name,Password,Phone,ID")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(customer);
        }

        [HttpGet]
        public ActionResult RequestResetPX()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RequestResetPX(FormCollection collection)
        {
            var sEmail = collection["Email"];
                Customer TK = db.Customers.SingleOrDefault(n => n.Email == sEmail );
                if (TK != null)
                {
                //save id customer
                Session["IDCustomers"] = TK.ID;
                ViewBag.IDCTM = Session["IDCustomers"];
                ViewBag.ThongBao = "1";
                MailMessage mail = new MailMessage();
                mail.To.Add("testwebpbshop@gmail.com");
                mail.From = new MailAddress("testwebpbshop@gmail.com");
                mail.Subject = "Đặt lại mật khẩu";
                string link = "https://localhost:44364/User/ResetPW/" + Session["IDCustomers"];
                string Body = $"Bấm vào đây để <a href='{link}'>Đặt lại mật khẩu</a>";
               
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("testwebpbshop@gmail.com", "yeka giez ewzg uoqm"); // Enter seders User name and password       
                smtp.EnableSsl = true;
                smtp.Send(mail);
               
            }
                else
                {
                ViewBag.ThongBao = " ";
            }
         
            return View();
        }
    }
}