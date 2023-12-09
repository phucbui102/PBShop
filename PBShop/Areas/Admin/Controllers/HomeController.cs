using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using PBShop.Models;
using PagedList;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net;
using System.Data.Entity;

namespace PBShop.Areas.Admin.Controllers
{
   
    public class HomeController : Controller
    {
        PBShopEntities db = new PBShopEntities();
        // GET: Admin/Home
        public ActionResult Index(String search, string sortProperty, string sortOrder)
        {
            var LP = (from s in db.Products
                      join c in db.Types on s.Id_Type equals c.ID
                      select new GetProduct

                      {
                          ID = s.ID,
                          Name = s.Name,
                          Price = s.Price,
                          Promotional_Price = s.Promotional_Price,
                          Img = s.Img,
                          Describe = s.Describe,
                          DateAdded = s.DateAdded,
                          NameType = c.NameType,
                          Id_Type = c.ID
                      });
            if (!String.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                LP = LP.Where(b => b.Name.ToLower().Contains(search));
            }
            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "desc" : "";
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "Name";

            if (sortOrder == "desc")
                LP = LP.OrderBy(sortProperty + " desc");
            else
                LP = LP.OrderBy(sortProperty);
            return View( LP.ToList());
        }
        public ActionResult ReloadData()
        {
            var LP = (from s in db.Products
                      join c in db.Types on s.Id_Type equals c.ID
                      select new GetProduct

                      {
                          ID = s.ID,
                          Name = s.Name,
                          Price = s.Price,
                          Promotional_Price = s.Promotional_Price,
                          Img = s.Img,
                          Describe = s.Describe,
                          DateAdded = s.DateAdded,
                          NameType = c.NameType,
                          Id_Type = c.ID
                      });
            return PartialView("_PartialData", LP.ToList());
        }


        
        public ActionResult Create()
        {
            ViewBag.Id_Type = new SelectList(db.Types, "ID", "NameType");
            ViewBag.Id_Promotional = new SelectList(db.Promotions, "ID", "ID");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Name,Price,Promotional_Price,Img,Describe,Id_Type,Id_Promotional")] Product p, HttpPostedFileBase Img, FormCollection form)
        {
            if (ModelState.IsValid)
            {
               

                    if (Img.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(Img.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Theme/assets/images/products"), _FileName);
                        Img.SaveAs(_path);
                        p.Img = _FileName;
                    }
                    p.Rated = 0;
                    p.DateAdded = DateTime.Today;
                    db.Products.Add(p);
                    db.SaveChanges();
                    return RedirectToAction("Index");
               

            }

            ViewBag.Id_Type = new SelectList(db.Types, "ID", "NameType", p.Id_Type);
            ViewBag.Id_Promotional = new SelectList(db.Promotions, "ID", "ID", p.Id_Promotional);
            return View(p);
        }


        // GET: Book/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product Product = db.Products.Find(id);
            if (Product == null)
            {
                return HttpNotFound();
            }
            return View(Product);
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product Product = db.Products.Find(id);
            if (Product == null)
            {
                return HttpNotFound();
            }
            return View("Details",Product);
        }

        public ActionResult Modals(int id)
        {
           
            Product p = db.Products.Find(id);
            
            return PartialView("_PartialModal", p);
        }
       

        // GET: Book/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Promotional = new SelectList(db.Promotions, "ID", "ID", product.Id_Promotional);
            ViewBag.Id_Type = new SelectList(db.Types, "ID", "NameType", product.Id_Type);
            return View(product);


        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Price,Promotional_Price,Img,Describe,Id_Type,Id_Promotional,DateAdded,Rated")] Product product, HttpPostedFileBase Img, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (Img != null)
                {
                    string _FileName = Path.GetFileName(Img.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Theme/assets/images/products"), _FileName);
                    Img.SaveAs(_path);
                    product.Img = _FileName;
                    // get Path of old image for deleting it
                    _path = Path.Combine(Server.MapPath("~/Theme/assets/images/products"), form["oldimage"]);
                    if (System.IO.File.Exists(_path))
                        System.IO.File.Delete(_path);

                }
                else
                    product.Img = form["oldimage"];
               

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
              
            }




            ViewBag.Id_Promotional = new SelectList(db.Promotions, "ID", "ID", product.Id_Promotional);
            ViewBag.Id_Type = new SelectList(db.Types, "ID", "NameType", product.Id_Type);
            return View(product);

           
        }


        // POST: Book/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Product Product = db.Products.Find(id);
            db.Products.Remove(Product);
            db.SaveChanges();
            return Json(new { success = true, message = "Item deleted successfully" });
        }

        [HttpGet]
        public ActionResult Login()
        {

            return View();

        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
         
            var sPassword = collection["password"];
            
            if(!String.IsNullOrEmpty(sPassword))
            {
                //Admin kh = db.Admin.SingleOrDefault(n => n.Email == sEmail && n.Password == sPassword);
                var admin = db.Admins.SingleOrDefault(n => n.Pass == sPassword);
                if (admin != null)
                {

                    Session["TKAdmin"] = admin;
                   
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }

    }
}