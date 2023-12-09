using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PBShop.Models;
using PagedList;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace PBShop.Controllers
{
    public class PBSopHomeController : Controller
     {
        
        PBShopEntities db = new PBShopEntities();
        // GET: PBSopHome
        public ActionResult Index()
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
                          Id_Type = c.ID,
                          Rated = (double)s.Rated
                      });


            ViewBag.NewProduct = LP.OrderBy("DateAdded" + " desc");

            ViewBag.BALProduct = LP.OrderBy("DateAdded" + " desc");

            ViewBag.OSProduct = LP.OrderBy("DateAdded" + " desc");

            ViewBag.SaleOnDay = LP.OrderBy("DateAdded" + " desc").Take(4);
            return View();




        }
        public ActionResult Contact()
        {
           
            return View();

        }
        public ActionResult PartialMenu()
        {
            var ListTypeP = from LTP in db.Types select LTP;
            return PartialView(ListTypeP);
        }
        public ActionResult FindProduct()
        {
            var ListType = from a in db.Types select a;
            return View(ListType);

        }

        public ActionResult ProductDetails(int id)
        {
            var Products = from s in db.Products where s.ID == id select s;
            return View(Products.Single());
        }
        public ActionResult PartialProductDetails(int id)
        {
            var Products = from s in db.Products where s.ID == id select s;
            return PartialView(Products.Single());
        }
        public ActionResult shop(int? size, int? page, int? IDType, string searchString, string Pr_From, string Pr_To, string sortOrder = "", int categoryID = 0)
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
                      }) ;

            if (!String.IsNullOrEmpty(Pr_To) )
            {
               
                float F = 0;
                float T = float.Parse(Pr_To);
                if (String.IsNullOrEmpty(Pr_From))
                {
                    LP = LP.Where(a => a.Price <= T);
                }

                ViewBag.PrFrom = F;
                ViewBag.PrTo = T; 
            }

           

            if (IDType != 0 && IDType != null)
            {
                LP = LP.Where(a => a.Id_Type == IDType);
            }

            ViewBag.Subject = IDType;
            ViewBag.Keyword = searchString;

            if (!String.IsNullOrEmpty(searchString))
                LP = LP.Where(b => b.Name.Contains(searchString));
            if (categoryID != 0 && categoryID != null)
            {
                LP = LP.Where(b => b.Id_Type == categoryID);
            }
            

            if (sortOrder == "asc") ViewBag.SortOrder = "asc";
            if (sortOrder == "desc") ViewBag.SortOrder = "desc";


            string sortProperty = "Price";

            if (sortOrder == "desc")
                LP = LP.OrderBy(sortProperty + " desc");
            else
                LP = LP.OrderBy(sortProperty);

            ViewBag.Page = page;


            page = page ?? 1; 

            int pageSize = (size ?? 9);

            ViewBag.pageSize = pageSize;

          
            int pageNumber = (page ?? 1);

           
            ViewBag.Listb = LP.Take(3);
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Giá: Tăng Dần", Value = "asc" });
            items.Add(new SelectListItem { Text = "Giá: Giảm Dần", Value = "desc" });
            ViewBag.SortValue = items;
            return View(LP.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult FindPartial(string searchString)
        {

            //var books = from a in db.Products join b in db.Types on a.Id_Type equals b.ID select new { a, b };
            //var LP = (from s in db.Products select s);
            ViewBag.Keyword = searchString;

            ////3. Tìm kiếm theo searchString
            //if (!String.IsNullOrEmpty(searchString))
            //    LP = LP.Where(b => b.Name.Contains(searchString));

            ////4. Tìm kiếm theo CategoryID
            //if (category != 0)
            //{
            //    LP = LP.Where(c => c.Id_Type == category);
            //}
            //5. Tạo danh sách danh mục để hiển thị ở giao diện View thông qua DropDownList
            ViewBag.categoryID = new SelectList(db.Types, "ID", "NameType"); // danh sách Category
            return PartialView();
        }

       


        

    }
}