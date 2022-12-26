using Microsoft.EntityFrameworkCore;
using System.Reflection;


ApplicationDbContext context = new();

#region EF Core Select Sorgularını Güçlendirme Teknikleri

#region IQueryable - IEnumerable Farkı
//Sen IQueryable türünde bir çalışma gerçekleştiriyorsan bu tür üzerinde yapmış olduğun işlemler Generate edilecek sorguya yansıtılacaktır. Yani sorgu üzerinden çalışıyormuşsun gibi düşün.
//IQueryable, bu arayüz üzerinde yapılan işlemler direkt generate edilecek sorguya yansıtılacaktır.
//context.Persons.Where();//Arkada generate edilecek sorguya bir where şartı ekliyoruz. Where'de IQueryable türündeyiz. Dolayısıyla burada yapılacak işlem oluşturulacak sorguya bu Where şartını ekleticektir.
//IEnumerable ise yapılan işlemleri In-Memory'de gerçekleştirir.
//Yani IEnumerable referansı üzerinde bir operasyon(Where, Take, Top vb işlemler) gerçekleştiriyorsanız bunların hiçbiri generate edilecek sorguya yansıtılmaz temelde hangi sorgu üzerinde çalışıyorsak o sorgu neticesinde bütün veriler belleğe alınır belleğe alındıktan sonra IEnumerable üzerinde yapılan işlemler In-Memory'de/bellekte gerçekleştirilir.
//IEnumerable, bu arayüz üzerinde yapılan işlemler temel sorgu neticesinde gelen ve in-memory'e yüklenen instance'lar üzerinde gerçekleştirilir. Yani sorguya yansıtılmaz.
//IQueryable'da yapmış olduğunuz bütün işlemler generate edilecek sorguya yansıtılmış olacaktır. Amma velakin IEnumerable'da temel bir sorgunuz olacak sen bunu IEnumerable'a dönüştürdükten sonra artık temel sorgu neticesinde gelecek olan verilerin üzerinde in-memory'de o işlemler gerçekleştirilecektir. Yani IEnumerable üzerinde hangi işlemleri yapıyorsan bundan sonrası sorguya yansıtılmayacak elde edilmiş olan verinin üzerinde in-memory'de bu işlemler sağlanacaktır. Bunlar fiili olarak in-memory'de uygulanmış olacaktır.
//IQueryable'da olduğumuz için Where, Top, Skip ekliyorsun bunlara göre bir SQL sorgusu generate ediliyor dolayısıyla hedef verileri direkt sana getirecek bir sorgu üretilmiş oluyor.
//IQueryable ile yapılan sorgulama çalışmalarında sql sorgusu hedef verileri elde edecek şekilde generate edilecekken, IEnumerable ile yapılan sorgulama çalışmalarında sql daha geniş verileri getirebilecek şekilde execute edilerek hedef veriler in-memory'de ayıklanır.

//IQueryable hedef verileri getirirken, IEnumerable hedef verilerden daha fazlasını getirip in-memory'de ayıklar.
//DbSet ve IQueryable aynı işlemleri gerçekleştirir.

//---------IQueryable---------
//context.Persons => Persons DbSet ise ya da IQueryable'sa ikisi de aynı işlemi yapacaktır. İkisinde de aynı kapıya/sonuca varacaksınız. Bundan sonra yapılan tüm işlemler sorguya yansıtılacaktır.
//Burada yapılan tüm işlemler arka planda üretilecek sql sorgusuna yansıtılmış olacaktır. Çünkü Persons özünde DbSet ya da IQueryable türünde
//IQueryable'da çalışıyorsan arkada kod üzerinde yapmış olduğun bütün mantıksal operasyonlar Generate edilecek sorguya eklenecek/yansıtılacak o sorguyu geliştirmeni sağlayacak yani optimize edilebilir hale getirecek dolayısıyla istediğin hedef verileri nokta atışı veritabanında elde etmeni sağlayacak bir arayüzdür.
//---------IEnumerable---------
//context.Persons.AsEnumerable() => Temelde hangi SQL yapılanması oluşturulduysa bundan sonra yapacağı işlemleri generate edilecek SQL cümleciğine ekleme bundan sonra yapacağım işlemler bu gelecek olan veri üzerinde in-memory'de işlensin. In-Memory üzerinde artık o işlemlere/operasyonlara tabi tutulsun gelecek olan veriler.
//AsEnumerable fonksiyonunu çağırdığımızdan dolayı Veritabanına yollanan sorgu Select * From Persons'tur bundan sonra yapılacak operasyonların hepsi In-Memory'de gerçekleşecektir.
//Burada tüm personları çektikten sonra yazdığımız şartları in-memory'de uygulamış olacağız.
//Örneğin salonda ailecek bir yemek yiyorsunuz Baban dedi ki oğlum/kızım bana mutfaktan gidip tuz getirir misin dedi IQueryable mantığında çalışıyorsan eğer mutfağa gidersin tuzu/tuzluğu alır gelirsin babana verirsin IQueryable mantığında hareket ediyorsan mutfağa gidersin bütün mutfağı getirirsin alakalı alakasız hangisine ihtiyacın varsa kendin şeç al dersin.
//Bir usta yan keski istediğinde IQueryable yan heskiyi getirmekse IEnumerable takım çantasını getirmektir.
//Bizler veritabanı üzerinde sorgulama yaparken bu sorgu neticesinde hedef verileri elde etmeye odaklı çalışmalıyız. Yani IQueryable'da sorgularımız inşa etmeye özen göstermeliyiz. Aksi taktirde lüzumsuz yere yapacağımız operasyonla çok fazla alakası olmayacak hedefin dışındaki verileri de in-memory'e taşıyarak performansta sorgulama sürecinde lüzumsuz maliyetlere sebebiyet verebiliriz.
//IEnumerable ile çalıştığımız taktirde temel sorgunun dışındaki işlemlerin hepsi bellek üzerinde işlenmiş olacaktır.
//IQueryable'da, IEnumerable'da her ikisi de Deferred Execution(ertelenmiş çalışma) dediğimiz davranışı sergiler. Şimdi sen IQueryable ile oluşturmuş olduğun bir sorguyu execute edebilmen için bunu ToList ile ya da bunun gibi foreach ile bir şekilde döngüsel yapılanmayla bir tetikleme mekanizmasıyla çalıştırman lazım ki aksi taktirde kendisi gecikmeli çalışma davranışı sergileyeceğinden dolayı yazdığın noktada devreye girmeyecektir tetiklenmeyecektir.
//IQeuryable ve IEnumerable davranışsal olarak aralarında farklar barındırsalarda her ikisi de Deferred Execution(gecikmeli çalıştırma) davranışı sergiler.
//Yani her iki arayüz üzerinden de oluşturulan işlemi execute edebilmek için ToList gibi tetikleyici fonksiyonları yahut foreach gibi tetikleyici işlemleri gerçekleştirmemiz gerekmektedir.
//Aksi taktirde bizlerin her iki arayüz üzerinden yapmış olduğumuz sorguyu oluşturulduğu noktada execute etmemiz mümkün değildir. Bunları bizim bir şekilde tetikliyor olmamız gerekmektedir.
//DbSet ya da IQueryable görüyorsak hala sorguyu etkiliyoruz anlamına gelir.
//EF Core üzerinden yapacağımız sorgulama operasyonlarında sorgunun ve sorgu neticesinin işlenme süreçlerinin performans ve maliyet optimizasyonu gerçekleştirebilmek için mümkün mertebe IQueryable'da çalıştığımıza özen göstermeliyiz.
//IQueryable'la IEnumerable arasındaki farkı anladıktan sonra big datalarda özellikle sorgulama gerçekleştiriyorsak hedef verileri elde edip sistemi/kaynakları pek fazla yormaksızın işlemlerimizi gerçekleştirebilmeliyiz. Bunun içinde yapılacak sorgulamarda IQueryable üzerinde çalışmaya özen göstermeliyiz.
//IQueryable üzerinde çalışacaksın ki hedef verilerini getirebilecek istediğin sorguyu üretebilesin. Bu sorguyu ürettikten sonra execute ettiğinde zaten istediğin verilerin ham bir şekilde eline gelecek ekstradan in-memory'de herhangi bir işleme gerek duymaksızın o veriler üzerinde operasyonlarına direkt devam edebileceksin. Aksi taktirde IEnumerable'da hedef verilerin dışında veriler elde edersin elde ettiğin bu verilerin üzerinde in-memory'de işlemler gerçekleştirip ekstradan hedef verilerinin işlenmesini/elde edilmesini sağlayabilmek için çaba/maliyet harcamak zorunda kalabilirsin.
//

#region IQueryable Örnek
//var persons = await context.Persons.Where(p => p.Name.Contains("a"))
//                             .Take(3)
//                             .ToListAsync();
//foreach (var person in persons)
//{

//}
//var persons = await context.Persons.Where(p => p.Name.Contains("a"))
//                                   .Where(p=> p.PersonId > 3)
//                                   .Take(3)
//                                   .Skip(3)
//                                   .ToListAsync();

#endregion
#region IEnumerable Örnek
//var persons = context.Persons.Where(p => p.Name.Contains("a"))
//                             .AsEnumerable()
//                             .Take(3)
//                             .ToList();
//foreach (var person in persons)
//{

//}
#endregion

//IEnumerable üzerinden yapılan çalışmayı IQueryable'a dönüştürmek istiyorsak AsQueryable, IQueryAble üzerinde yapılan sorgulama sürecini de belirli bir noktada kırıp geri kalan sorumluluğu in-memory'e bırakmak istiyorsak AsEnumerable fonksiyonlarını kullanabiliriz.
#region AsQueryable

#endregion
#region AsEnumerable

#endregion
#endregion

#region Yalnızca İhtiyaç Olan Kolonları Listeleyin - Select
// EF Core'da bir işlem yaparken arka planda oluşturduğun sorgu neticesinde bu execute ediliyor ya  Execute neticesinde gelen veriler bir objeye dönüştürülürken bunların property'leri eşleştiriliyor veriler bunlara set ediliyor. Ne kadar fazla property ne kadar fazla kolon o kadar fazla property eşleştirmesi dolayısıyla burada ekstradan mikro düzeyde bir maliyet.
// Hem veritabanı sorgulama sürecinde bir maliyet hem de bunun eşleştirme sürecinde/map edilmesi sürecinde bir maliyet. Bunu optimize etmek istiyorsan yapman gereken ihtiyaç olan kolonları sadece çek. 
//İhtiyacın olan kolonarı iradeli bir şekilde çekmen gerekiyor.
//var persons = await context.Persons.Select(p=> new {p.Name}).ToListAsync();//IQueryable'da üretilecek sorgunun çekilecek kolonlarını belirliyoruz.

#endregion

#region Result'ı Limitleyin - Take
//Bir sorgulama yapıyorsun yapmış olduğun sorgulama da arkada tabloda binlerce veri olabilir.
//Binlerce veriyi hiçbir uygulamada ha deyince göstermeyiz. En fazla 100 - 200 adet gösterirsin. Hatta pagination/sayfalama yapıyorsun. Sayfalama neticesinde bunları kısmi olarak gösteriyorsun. Hiçbir zaman bir tablodaki verilerin hepsinin topluca kümülatif bir şekilde çekmeyin. Bunları LİMİTLEYİN!!!
//Büyük veriler söz konusuysa buralarda Take ile yani Top keywordü ile limitleme yapmanız oluşturacağınız sorgunun daha performanslı çalışmasına sebep olacaktır.
//Bundan sonraki bütün sorgulamalarda hiç yoktan sayfada gösterebileceğimiz adil sayıda bir sınırlandırma/limitleme işlemini sorgularımıza uyguluyoruz.

//await context.Persons.Take(50).ToListAsync();
#endregion

#region Join Sorgularında Eager Loading Sürecinde Verileri Filtreleyin
//Join işlemlerinde Include fonksiyonu üzerinden Eager Loading operasyonunu uyguluyoruz.
//Eğer ki yapacağınız çalışma da ilgili veri/join yapılacak data belirli bir şarta tabi tutulacak data ise tüm dataları çekmemelisin
//Direkt Eager Loading sürecinde hedef verilere filtreleme uygulayabilirsin.

//var persons = await context.Persons.Include(p=> p.Orders
//                                                .Where(o=> o.OrderId % 2 == 0)
//                                                .OrderByDescending(o=>o.OrderId)
//                                                .Take(4))
//    .ToListAsync();

//foreach (var person in persons)
//{
//    var orders = person.Orders.Where(o => o.CreatedDate.Year == 2022);
//}
#endregion

#region Şartlara Bağlı Join Yapılacaksa Eğer Explicit Loading Kullanın
//Eğer ki şartlara bağlı bir join işlemi yapılacaksa bunu peşinen Eager Loading(Include) ile gerçekleştirmektense Explicit Loading ile gerçekleştirmek daha optimize edilmiş, daha performanslı ve az maliyetli bir çalışma sağlamış olacaktır.


//var person = await context.Persons.Include(p => p.Orders).FirstOrDefaultAsync(p => p.PersonId == 1); //Bu işlemde hangi veri olursa olsun Order'ları elde edeceğim. Ama maliyet açısından pahalıdır.
//Bizlerin bu senaryo da Ayşe'nin dışındaki personellerin Order'larını çekmeye ihtiyacımız yok. Gerek yoksa ve Ayşe'nin dışında bir person geliyorsa niye ben orada Order'ı çekiyorum. Burada bir optimizasyon gerekiyor
//var person = await context.Persons.FirstOrDefaultAsync(p => p.PersonId == 1);//Tüm personlar için ilgili Navigation Property'i sorgulamaya eklemiyoruz/join yapmıyoruz.

//if (person.Name == "Ayşe")//Şart durumunda join'leme işlemini gerçekleştiriyoruz. Bunuda Explicit Loading yöntemiyle gerçekleştiriyoruz.
//{
//    //Eğer ki Ayşe ise bu person Order'larını getir.
//    await context.Entry(person).Collection(p=>p.Orders).LoadAsync();//Burada yapmış olduğumuz işlem neticesinde elimizdeki person kimse yani şarta uygunsa, iş mantığına/iş kuralına uygunsa o zaman gerekli joinleme işlemini gerçekleştirmiş oluyoruz.
//    //Esasında burada join'leme yapmıyoruz. İlgili veriye uygun ilişkisel tablodaki karşılıklı verileri çekmiş oluyoruz.
//    //In Memory'de var olan ilişkisel veriler context üzerinde de birbirleriyle ilişkilendiriliyorlar.
//}
#endregion

#region Lazy Loading Kullanırken Dikkatli Olun
#region Riskli Durum
//Bu tarz senaryolarda her bir veri için ilişkisi olduğu tablodaki verileri çekerken sürekli sorgu oluşturur.
//Lüzumsuz yere veritabanına aynı sorguları sürekli göndermiş olursunuz.
//var persons = await context.Persons.ToListAsync();
//foreach (var person in persons)
//{
//    foreach (var order in person.Orders)
//    {
//        Console.WriteLine($"{person.Name} - {order.OrderId}");
//    }
//    Console.WriteLine("******************");
//}
#endregion
#region İdeal Durum
//var persons = await context.Persons.Select(p => new { p.Name, p.Orders }).ToListAsync();
//foreach (var person in persons)
//{
//    foreach (var order in person.Orders)
//    {
//        Console.WriteLine($"{person.Name} - {order.OrderId}");
//    }
//    Console.WriteLine("******************");
//}
#endregion
#endregion

#region İhtiyaç Noktalarında Ham SQL Kullanın - FromSql
//EF Core'da çalışırken bazen kompleks sorgulara ihtiyacımız olabiliyor. bunu LINQ kullanarak yapmaktansa bazen ham Row Sql'leride kullanabilirsiniz.
//Bazen veritabanında oluşturabileceğiniz sorguyu EF Core'da oluşturmak zahmetli olabilir. Bunun için View, Stored Procedure gibi veritabanı nesnelerinden istifade ederek onların üzerinde operasyonları yürütebilirsiniz. Bunları sadece EF Core'da tetikleyebilirsiniz.
#endregion

#region Asenkron Fonksiyonları Tercih Edin
//Asenkron fonksiyonlar üzerinden çalışmalrınızı sergilemeniz en azından veritabanı seviyesinde değilde bu taraftaki kaynak kısmında işinizi kolaylaştıracaktır.
//Performansı bir nebze olsun ölçekli hale getirecek arttıracaktır.
#endregion
#endregion

public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Order> Orders { get; set; }
}
public class Order
{
    public int OrderId { get; set; }
    public int PersonId { get; set; }
    public string Description { get; set; }

    public virtual Person Person { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Person>()
            .HasMany(p => p.Orders)
            .WithOne(o => o.Person)
            .HasForeignKey(o => o.PersonId);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r+!;TrustServerCertificate=True")
            .UseLazyLoadingProxies();
    }
}