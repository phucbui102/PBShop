using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PBShop.Models;
namespace PBShop.Models
{
    public class ProductModel
    {
        PBShopEntities db = new PBShopEntities();

        public int IDP { get; set; }
        public string NameP { get; set; }
        public string AnhBiaP { get; set; }
        public double PriceP { get; set; }
        public double PricePromotionalP { get; set; }
        public int ID_TypeP { get; set; }
        public DateTime DateAddP { get; set; }
        public int SoLuongP { get; set; }
        public double dThanhTien
        {
            get { return SoLuongP * PricePromotionalP; }
        }
        public ProductModel(int ms)
        {
            IDP = ms;
            Product s = db.Products.SingleOrDefault(n => n.ID == IDP);
            NameP = s.Name;
            AnhBiaP = s.Img;
            PriceP = double.Parse(s.Price.ToString());
            PricePromotionalP = double.Parse(s.Promotional_Price.ToString());
            DateAddP = (DateTime)s.DateAdded;
            ID_TypeP = s.Id_Type;
            SoLuongP = 1;
        }


    }
}