﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

ETicaretContext context = new();

#region Çoğul Veri Getiren Sorgulama Fonksiyonları
#region ToListAsync
//Üretilen sorguyu execute ettirmemizi sağlayan fonksiyondur.
//IQueryable'dan IEnumerable'a geçmemizi sağlayan fonksiyondur.

#region Method Syntax
//var urunler = await context.Urunler.ToListAsync();
#endregion
#region Query Syntax
//var urunler = await (from urun in context.Urunler
//               select urun).ToListAsync();
//var urunler = from urun in context.Urunler
//                     select urun;
//var datas = await urunler.ToListAsync();
#endregion
#endregion

#region Where
//Oluşturulan sorguya where şartı eklememizi sağlar.
//Where şartı kullanıldığında IQueryable seviyesinde oluruz biz bunu çalıştımak yani veritabanı sunucusuna yollamak istersem ToListAsync fonksiyonunu kullanırız.
#region Method Syntax
//var urunler = await context.Urunler.Where(u => u.Id > 500).ToListAsync();
//var urunler = await context.Urunler.Where(u => u.UrunAdi.StartsWith(a)).ToListAsync();
//Console.WriteLine();
#endregion
#region Query Syntax
//var urunler = (from urun in context.Urunler
//               where urun.Id > 500 && urun.UrunAdi.EndsWith("7")
//               select urun);
//var datas = await urunler.ToListAsync();
#endregion
#endregion

#region OrderBy
//Bu fonksiyon üzerinden ilgili sorguya biz sıralama operasyonu çekeriz
//Sorgu üzerinde alfanumerik olarak sıralama yapmamızı sağlayan bir fonksiyondur. (ascending/küçükten -> büyüğe)
//OrderBy şartı kullanıldığında IQueryable seviyesinde oluruz biz bunu çalıştımak yani veritabanı sunucusuna yollamak istersem ToListAsync fonksiyonunu kullanırız.
#region Method Syntax
//var urunler = context.Urunler.Where(u => u.Id > 500 || u.UrunAdi.EndsWith("2")).OrderBy(u => u.UrunAdi);
#endregion
#region Query Syntax
//var urunler2 = (from urun in context.Urunler
//                where urun.Id > 500 || urun.UrunAdi.EndsWith("2")
//                orderby urun.UrunAdi
//                select urun);
//await urunler.ToListAsync();
//await urunler2.ToListAsync();
#endregion
#endregion

#region ThenBy
//OrderBy üzerinde yapılan sıralama işlemini farklı kolonlara da uygulamamızı sağlayan bir fonksiyondur.
//OrderBy çektiğinde bunu birden fazla kolonda sağlamak istiyorsan ThenBy kullanmalısın.(ascending/küçükten -> büyüğe)
//ThenBy şartı kullanıldığında IQueryable seviyesinde oluruz biz bunu çalıştırmak yani veritabanı sunucusuna yollamak istersem ToListAsync fonksiyonunu kullanırız.
//var urunler = context.Urunler.Where(u => u.Id > 500 || u.UrunAdi.EndsWith("2")).OrderBy(u => u.UrunAdi).ThenBy(u => u.Fiyat).ThenBy(u => u.Id);//Ürün adları mükerrer olan varsa ThenBy diyerek fiyata bak diyoruz.fiyatta mükerrer olanlar varsa ThenBy diyerek Id'ye göre ascending türünde sıralama yap diyoruz..

//await urunler.ToListAsync();
#endregion

#region OrderByDescending
//Descending olarak sıralama yapmamızı sağlayan bir fonksiyondur
//Sorgu üzerinde alfanumerik olarak sıralama yapmamızı sağlayan bir fonksiyondur. (descending/büyükten -> küçüğe)
//OrderByDescending şartı kullanıldığında IQueryable seviyesinde oluruz biz bunu çalıştımak yani veritabanı sunucusuna yollamak istersem ToListAsync fonksiyonunu kullanırız.

#region Method Syntax
//var urunler = context.Urunler.OrderByDescending(u=>u.Fiyat);
//await urunler.ToListAsync();
#endregion
#region Query Syntax
//var urunler = await (from urun in context.Urunler
//              orderby urun.Id descending
//              select urun).ToListAsync();
#endregion
#endregion

#region ThenByDescending
//OrderByDescending üzerinde yapılan sıralama işlemini farklı kolonlara da uygulamamızı sağlayan bir fonksiyondur.
//OrderByDescending çektiğinde bunu birden fazla kolonda sağlamak istiyorsan ThenBy kullanmalısın.(descending/büyükten -> küçüğe)
//ThenBy şartı kullanıldığında IQueryable seviyesinde oluruz biz bunu çalıştırmak yani veritabanı sunucusuna yollamak istersem ToListAsync fonksiyonunu kullanırız.

var urunler = await context.Urunler.OrderByDescending(u => u.Id).ThenByDescending(u => u.Fiyat).ThenBy(u => u.UrunAdi).ToListAsync();
#endregion
#endregion


public class ETicaretContext : DbContext
{
    public DbSet<Urun> Urunler { get; set; }
    public DbSet<Parca> Parcalar { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=ETicaretDB;Trusted_Connection=true;");
    }
}
public class Parca
{
    public int Id { get; set; }
    public string ParcaAdi { get; set; }
}
public class Urun
{
    public int Id { get; set; }
    public string UrunAdi { get; set; }
    public float Fiyat { get; set; }

    public ICollection<Parca> Parcalar { get; set; }
}