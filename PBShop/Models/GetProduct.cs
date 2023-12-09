using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PBShop.Models
{
    public class GetProduct
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<double> Price { get; set; }
        public double Promotional_Price { get; set; }
        public string Img { get; set; }
        public string Describe { get; set; }
        public int Id_Type { get; set; }
        public int Id_Promotional { get; set; }

        public double Rated { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }

        public string NameType { get; set; }
        public string Icon { get; set; }
    }
}