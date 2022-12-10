using Microsoft.EntityFrameworkCore;


ApplicationDbContext context = new();

#region Temporal Tables Nedir?
//Temporal Tables'lar yapısı itibariyle verisel geçmişini kayıt altına alan tabiri caizse o tablonun history'sini oluşturmamızı sağlayan bir yapılanmaydı.
//Verisel geçmişten kastımız herahngi bir tabloda update işlemi gerçekleştiriliyorsa update'ten önceki verilerin ve update'ten sonraki verilerin bir şekilde kayda alınamsı gerekiyor.Yahut silme operasyonu gerçekleştiriliyorsa aynı şekilde işte bu veri şu tarihler arasında silinmiştir gibisinden bir kaydın tutulması gerekiyor.Silinme tarihinden ziyade hangi tarihler arasında o verinin olduğuna dair bir kaydın tutulması gerekiyor.İşte bu tarz verisel bir arşivi oluşturmamızı sağlayan yapılanma veritabanı seviyesinde Temporal Tables olarak nitelendiriliyor.
//Verisel bir arşiv oluşturmamızı sağlar.
//Veri değişikliği süreçlerinde kayıtları depolayan ve zaman içinde farklı noktalardaki tablo verilerinin analizi için kullanılan ve sistem tarafından yönetilen tablolardır.
//EF Core 6.0 ile desteklenmektedir.
//Uzun lafın kısası bu tablolar herhangi bir tablonun verisel kaydını tutuyor, zamansal veri kaydını tutuyor işte eklenen verilerin üzerinde yapılan değişikliklere dair hangi tarih aralığında ne zaman değişiklik yapılmış, önceki verisi neymiş, önceden hangi veriler varmışta silinmiş bunların hepsinin bilgilerini Temporal Table'lardan rahatlıkla elde edebiliyoruz.
#endregion
#region Temporal Tables Özelliğiyle Nasıl Çalışılır?
//EF Core'daki migration yapıları sayesinde Temporal Table'lar oluşturulup veritabanında üretilebilmektedir.
//Var olan mevcut tabloları migration'lar aracılığıyla Temporal Table'lara dönüştürebiliyoruz.
//Herhangi bir tablonun verisel olarak geçmişini rahatlıkla sorgulayabiliriz.
//Herhangi bir tablodaki bir verinin geçmişteki herhangi bir T anındaki hali/durumu/verileri geri getirebilmekteyiz.
//Ne zaman ki verilerin üzerinde bir değişiklik yaparız bir veriyi sileriz ya da güncelleriz o zaman sistem tarafından otomatik olarak buradaki history tablosuna kayıt atılacaktır. Burada log mantığı düşünebilirsiniz. Bu veri şöyle güncellenmiştir işte bu veri silinmiştir şu tarihler arasında şeklinde log tutmamızı sağlayan tablolardır.
#endregion
#region Temporal Tables Nasıl Uygulanır?

#region IsTemporal Yapılanması
//EF Core bu yapılandırma fonksiyonu sayesinde ilgili entity'e karşılık üretilecek tabloda Temporal Table oluşturacağını anlamaktadır. Yahut önceden ilgili tablo üretilmişse eğer onu temporal table'a dönüştürecektir.

//protected override void OnModelCreating(ModelBuilder modelBuilder)
//{
//    modelBuilder.Entity<Employee>().ToTable("Employees", builder => builder.IsTemporal());
//    modelBuilder.Entity<Person>().ToTable("Persons", builder => builder.IsTemporal());
//}
#endregion
#region Temporal Table İçin Üretilen Migration İncelenmesi
////Tablo seviyesindeki anatasyonlar. Oluşturulacak Temporal Table'ın konfigürasyonları burada sağlanıyor
//                .Annotation("SqlServer:IsTemporal", true)//Burada bu tablonun Temporal table olacağını bildiriyoruz.
//                .Annotation("SqlServer:TemporalHistoryTableName", "EmployeesHistory")//Temporal table tablosunun/yapılanmasının ismini de burada bildiriyoruz.
//                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
//                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")//Bu tabloda yapılacak bütün temporal table çalışmalarının veri kayıtlarını tarihsel zamanını tutabilmek için 2 kolona ihtiyacımız olacak bu kolonlardan bir tanesi end bir tanesi start olarak değerlendirilecek bu kolonlarıda burada bildiriyoruz.
//                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

//Id = table.Column<int>(type: "int", nullable: false)
//    .Annotation("SqlServer:Identity", "1, 1")
//    .Annotation("SqlServer:IsTemporal", true)//Bu kolonların IsTemporal konfigürasyonuyla Temporal Table'da kayda alınıp alınmayacağının bilgisi tutuluyor
//    .Annotation("SqlServer:TemporalHistoryTableName", "EmployeesHistory")//Hangi Temporal Table'da geçmiş zamansal davranışların tutulacağı da burada bildiriliyor
//    .Annotation("SqlServer:TemporalHistoryTableSchema", null)
//    .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")//Bu kolonlara karşılık zamansal kayıtların atılacağı bildirilmiştir.
//    .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
#endregion
#endregion
#region Temporal Table'ı Test Edelim
//Temporal Table var olan veriler üzerindeki zamansal değişimleri takip etmemizi sağlayan bir tarihsel tablo
//Temporal Table'ı konfigüre edip veritabanında oluşturduktan sonra EF Core üzerinden yapılan sorgulama süreçlerinde ilgili history tablosunun çalışması artık veritabanı sistemine kalıyor. Var olduğu için o da otomatik olarak gelen sorguya göre gerekli logları tutmuş oluyor.
#region Veri Eklerken
//Temporal Table'a veri ekleme süreçlerinde herhangi bir kayıt atılmaz! Temporal Table'ın yapısı, var olan veriler üzerindeki zamansal değişimleri takip etmek üzerine kuruludur.
//var persons = new List<Person>() {
//    new() { Name = "Musa", Surname = "Uyumaz", BirthDate = DateTime.UtcNow }, 
//    new() { Name = "Gençay", Surname = "Yıldız", BirthDate = DateTime.UtcNow }, 
//    new() { Name = "Mustafa", Surname = "Uyumaz", BirthDate = DateTime.UtcNow }, 
//    new() { Name = "Osman", Surname = "Uyumaz", BirthDate = DateTime.UtcNow }, 
//    new() { Name = "Serhat", Surname = "Uyumaz", BirthDate = DateTime.UtcNow }, 
//    new() { Name = "Özcan", Surname = "Uyumaz", BirthDate = DateTime.UtcNow }, 
//    new() { Name = "Alparslan", Surname = "Uyumaz", BirthDate = DateTime.UtcNow }
//};

//await context.Persons.AddRangeAsync(persons);
//await context.SaveChangesAsync();
#endregion
#region Veri Güncellerken
//var person = await context.Persons.FindAsync(3);
//person.Name = "Ahmet";
//await context.SaveChangesAsync();
#endregion
#region Veri Silerken
//var person = await context.Persons.FindAsync(3);
//context.Persons.Remove(person);
//await context.SaveChangesAsync();
#endregion
#endregion
#region Temporal Table Üzerinden Geçmiş Verisel İzleri Sorgulama

#region TemporalAsOf
//Belirli bir zaman için değişikliğe uğrayan tüm öğeleri döndüren bir fonksiyondur.
var datas = await context.Persons.TemporalAsOf(new DateTime(2022, 12, 10, 16, 01, 12)).Select(p => new
{
    p.Id,
    p.Name,
    PeriodStart = EF.Property<DateTime>(p, "PeriodStart"),
    PeriodEnd = EF.Property<DateTime>(p, "PeriodEnd")
}).ToListAsync();

foreach (var data in datas)
{
    Console.WriteLine($"{data.Id} {data.Name} | {data.PeriodStart} - {data.PeriodEnd}");
}
#endregion
#region TemporalAll
//Güncellenmiş yahut silinmiş olan tüm verilerin geçmiş sürümleri ve geçerli durumlarını döndüren bir fonksiyondur.
//Gelmiş geçmiş tüm verileri elde etmek istiyorsam TemporalAll fonksiyonunu kullanabilirim.
var datas2 = await context.Persons.TemporalAll().Select(p => new
{
    p.Id,
    p.Name,
    PeriodStart = EF.Property<DateTime>(p, "PeriodStart"),
    PeriodEnd = EF.Property<DateTime>(p, "PeriodEnd")
}).ToListAsync();

foreach (var data in datas2)
{
    Console.WriteLine($"{data.Id} {data.Name} | {data.PeriodStart} - {data.PeriodEnd}");
}
#endregion
#region TemporalFromTo
//Belirli bir zaman aralığı içerisindeki verileri döndüren bir fonksiyondur. Başlangı.ç ve bitiş zamanı dahil değildir.
var baslangic = new DateTime(2022, 12, 10, 15, 57, 04);
var bitis = new DateTime(2022, 12, 10, 16, 01, 12);
var datas3 = await context.Persons.TemporalFromTo(baslangic, bitis).Select(p => new
{
    p.Id,
    p.Name,
    PeriodStart = EF.Property<DateTime>(p, "PeriodStart"),
    PeriodEnd = EF.Property<DateTime>(p, "PeriodEnd")
}).ToListAsync();

foreach (var data in datas3)
{
    Console.WriteLine($"{data.Id} {data.Name} | {data.PeriodStart} - {data.PeriodEnd}");
}
#endregion
#region TemporalBetween
//Belirli bir zaman aralığı içindeki verileri yine döndürüyor başlangıç tarihi dahil değilken sadece bitiş tarihi dahildir.
var baslangic2 = new DateTime(2022, 12, 10, 15, 57, 04);
var bitis2 = new DateTime(2022, 12, 10, 16, 01, 12);
var datas4 = await context.Persons.TemporalFromTo(baslangic2, bitis2).Select(p => new
{
    p.Id,
    p.Name,
    PeriodStart = EF.Property<DateTime>(p, "PeriodStart"),
    PeriodEnd = EF.Property<DateTime>(p, "PeriodEnd")
}).ToListAsync();

foreach (var data in datas4)
{
    Console.WriteLine($"{data.Id} {data.Name} | {data.PeriodStart} - {data.PeriodEnd}");
}
#endregion
#region TemporalContainedIn
//Belirli bir zaman aralığı içerisindeki verileri döndüren bir fonksiyondur. Başlangıç ve bitiş zamanı dahildir.
var baslangic3 = new DateTime(2022, 12, 10, 15, 57, 04);
var bitis3 = new DateTime(2022, 12, 10, 16, 01, 12);
var datas5 = await context.Persons.TemporalFromTo(baslangic3, bitis3).Select(p => new
{
    p.Id,
    p.Name,
    PeriodStart = EF.Property<DateTime>(p, "PeriodStart"),
    PeriodEnd = EF.Property<DateTime>(p, "PeriodEnd")
}).ToListAsync();

foreach (var data in datas5)
{
    Console.WriteLine($"{data.Id} {data.Name} | {data.PeriodStart} - {data.PeriodEnd}");
}
#endregion
#endregion
#region Silinmiş Bir Veriyi Temporal Table'dan Geri Getirme
//Silinmiş bir veriyi Temporal Table'dan getirebilmek için öncelikel yapılması gereken ilgili verinin silindiği tarihi bulmamız gerekmektedir. Ardından TemporalAsOf fonksiyonu ile silinen verinin geçmiş değeri elde edilebilir ve fiziksel tabloya bu veri taşınabilir.

//Silindiği Tarih
var dateOfDelete = await context.Persons.TemporalAll()
    .Where(p => p.Id == 3)
    .OrderByDescending(p => EF.Property<DateTime>(p, "PeriodEnd"))
    .Select(p => EF.Property<DateTime>(p, "PeriodEnd"))
    .FirstAsync();

var deletedPerson = await context.Persons.TemporalAsOf(dateOfDelete.AddMilliseconds(-1)).FirstOrDefaultAsync(p => p.Id == 3);

await context.AddAsync(deletedPerson);

await context.Database.OpenConnectionAsync();

await context.Database.ExecuteSqlInterpolatedAsync($"SET IDENTITY_INSERT dbo.Persons ON");
await context.SaveChangesAsync();
await context.Database.ExecuteSqlInterpolatedAsync($"SET IDENTITY_INSERT dbo.Persons OFF");
#region SET IDENTITY_INSERT Konfigürasyonu
//Id ile veri ekleme sürecinde ilgili verinin id sütununa kayıt işleyenbilmek için veriyi fiziksel tabloaya taşıma işleminden önce veritabanı seviyesinde SET IDENTITY_INSERT komutu eşliğinde id bazlı veri ekleme işlemi aktifleştirilmelidir.
#endregion
#endregion

class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
}
class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Employee> Employees { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().ToTable("Employees", builder => builder.IsTemporal());
        modelBuilder.Entity<Person>().ToTable("Persons", builder => builder.IsTemporal());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r!.;TrustServerCertificate=True");
    }
}