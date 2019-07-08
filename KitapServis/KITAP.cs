using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KitapServis
{
    public class KITAP
    {
        public int Id { get; set; }

        public String Ad { get; set; }

        public String Resim { get; set; }

        public String YayimYili { get; set; }

        public float Fiyat { get; set; }    
    }
}