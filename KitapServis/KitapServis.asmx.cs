
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Text;

namespace KitapServis
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        SqlConnection con;

        public bool baglan()
        {
            try
            {
                string STRconnect = "Data Source=DESKTOP-2KJ5RFR\\MSSQLSERVER1;Initial Catalog=AndroidKitap; user id=sa; password=12345";
                con = new SqlConnection(STRconnect);
                con.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataSet getir(string commandText)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, con);
            try
            {
                DataSet dset = new DataSet();
                adapter.Fill(dset);
                return dset;
            }
            catch (Exception Exp)
            {
                DataSet dset = new DataSet();
                return dset;
            }
        }

        [WebMethod]
        public String kitapKaydet(string ad,string resim,string yayimyili,string fiyat,string  yazarlar)
        {

            if (baglan())
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO KITAP (Ad,Resim,YayimYili,Fiyat) VALUES (@ad,@resim,@yayimyili,@fiyat)", con);

                cmd.Parameters.AddWithValue("@ad", ad);
                cmd.Parameters.AddWithValue("@resim", resim);
                cmd.Parameters.AddWithValue("@yayimyili", yayimyili);
                cmd.Parameters.AddWithValue("@fiyat", fiyat);
                cmd.Connection = con;
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("SELECT max(Id) AS Id FROM KITAP ", con);
                SqlDataReader dr= cmd.ExecuteReader();
                int a=0;
                while (dr.Read()){                
                a = int.Parse(dr["Id"].ToString());                
                }
                dr.Close();
                con.Close();
                baglan();
               
                string[] parcalar = yazarlar.Split(',');
                for (int i = 0; i < parcalar.Length-1; i++)
                {
                    cmd = new SqlCommand("INSERT INTO KITAPYAZAR (kitapId,yazarId) VALUES (@kitapId, @yazarId)", con);
                    cmd.Parameters.AddWithValue("@kitapId", a);
                    cmd.Parameters.AddWithValue("@yazarId", parcalar[i]);
                    cmd.ExecuteNonQuery();
                }

                con.Close();
                return "1";
            }
            else { return "Bağlanamadı"; }
        }

        [WebMethod]
        public Boolean kitapSil(int kitapId)
        {
            if (baglan())
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM KITAPYAZAR WHERE kitapId=@kitapId", con);
                cmd.Parameters.AddWithValue("@kitapId", kitapId);
                cmd.ExecuteNonQuery();
              
                cmd = new SqlCommand("DELETE FROM KITAP WHERE Id=@Id", con);
                cmd.Parameters.AddWithValue("@Id", kitapId);
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            else { return false; }
        }

        [WebMethod]
        public List<KITAP> kitapListele()
        {
            KITAP kitap;
            List<KITAP> kitaplar = new List<KITAP>();

            if (baglan())
            {
                DataSet Kitaplar = getir("SELECT * FROM KITAP ");
                if (Kitaplar.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < Kitaplar.Tables[0].Rows.Count; i++)
                    {
                        kitap = new KITAP();
                        kitap.Id = int.Parse(Kitaplar.Tables[0].Rows[i]["Id"].ToString().Trim());
                        kitap.Ad = Kitaplar.Tables[0].Rows[i]["Ad"].ToString().Trim();
                        kitap.Resim = Kitaplar.Tables[0].Rows[i]["Resim"].ToString();
                        kitap.YayimYili = Kitaplar.Tables[0].Rows[i]["YayimYili"].ToString().Trim();
                        kitap.Fiyat = float.Parse(Kitaplar.Tables[0].Rows[i]["Fiyat"].ToString().Trim());
                        kitaplar.Add(kitap);
                    }
                }
            }
          return kitaplar;
        }

        [WebMethod]
        public List<YAZAR> yazarListele()
        {
            string B = "";
            YAZAR yazar;
            List<YAZAR> yazarlar = new List<YAZAR>();
            if (baglan())
            {
                DataSet Yazarlar = getir("SELECT * FROM YAZAR ");
                if (Yazarlar.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < Yazarlar.Tables[0].Rows.Count; i++)
                    {
                        yazar = new YAZAR();
                        yazar.Id = int.Parse(Yazarlar.Tables[0].Rows[i]["Id"].ToString().Trim());
                        yazar.Ad = Yazarlar.Tables[0].Rows[i]["Ad"].ToString().Trim();
                        yazar.Soyad = Yazarlar.Tables[0].Rows[i]["Soyad"].ToString().Trim();
                        yazarlar.Add(yazar);
                    }
                    B = "{\"a\":" + JsonConvert.SerializeObject(yazarlar) + "}";
                }
            }
            else
            {
                B = "Bağlanamadı";
            }
            return yazarlar;
        }

        [WebMethod]
        public List<YAZAR> kitapYazarlari(int kitapId)
        { 
            YAZAR yazar;
            List<YAZAR> yazarlar = new List<YAZAR>();

            if (baglan())
            {
                DataSet Yazarlar = getir("select * from YAZAR WHERE Id IN(select yazarId from KITAPYAZAR where kitapId="+kitapId+") ");
                if (Yazarlar.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < Yazarlar.Tables[0].Rows.Count; i++)
                    {
                        yazar = new YAZAR();
                        yazar.Id = int.Parse(Yazarlar.Tables[0].Rows[i]["Id"].ToString().Trim());
                        yazar.Ad = Yazarlar.Tables[0].Rows[i]["Ad"].ToString().Trim();
                        yazar.Soyad = Yazarlar.Tables[0].Rows[i]["Soyad"].ToString().Trim();
                        yazarlar.Add(yazar);
                    }                
                }
            } 
            return yazarlar;
        }
    }
} 