using Microsoft.EntityFrameworkCore;

ApplicationDbContext context = new();

#region EF Core'da In-Memory Database İle Çalışmanın Gereği Nedir?
//EF Core'da In-Memory'de bir veritabanı gibi bir alan oluşturuyoruz ve bu alan üzerinde verisel çalışmalar, sorgulamalar vs işlemler yürütübiliyoruz.
//EF Core'da yeni özellikler çıkıyor ve bu yeni özellikleri bizler denemek zorunda kalıyoruz. Yani gerçek uygulamalarda bunları operasyonlara tabi tutmadan önce gidip kendi çapında bir local test yapıyorsun işte bu test sürecinde mevcut veritabanları sunucularını test veritabanlarıyla kirletmeksizin direkt In-Memory'de oluşturacağın bir veritabanı üzerinden çalışmayı tercih edebilirsin. İşte bu tarz durumlarda In-Memory'deki veritabanını tercih edebiliyoruz.  
//Genellikle bu özelliği yeni çıkan EF Core özelliklerini test edebilmek için kullanıyoruz.
//EF Core, fiziksel veritabanlarından ziyade in-memory'de Database oluşturup üzerinde birçok işlemi yapmamızı sağlayabilmektedir. İşte bu özellik ile gerçek uygulamaların dışında test gibi operasyonları hızlıca yürütebileceğimiz imkanlar elde edebilmekteyiz.
#endregion
#region Avantajları Nelerdir?
//Test ve pre-prod(test'ten sonra production'ının öncesinde yani daha yayına çıkmadan önce) uygulamalrda gerçek/fiziksel veritabanları oluşturmak ve yapılandırmak yerine tüm veritabanını bellekte modelleyebilir ve gerekli işlemleri sanki gerçek bir veritabanında çalışıyor gibi orada gerçekleştirebiliriz. Bu da bize hem zamandan kazandıracaktır. Hem de türlü türlü veritabanında kirliliğe sebebiyet verebilecek çalışmalardan bizleri soyutlamış olacaktır.
//Bellekte çalışmak geçici bir deneyim olcağı için veritabanı server'larında test amaçlı üretilmiş olan veritabanlarının lüzumsuz yer işgal etmesini engellemiş olacaktır.
//Bellekte veritabanını modellemek kodun hızlı bir şekilde test edilmesini sağlayacaktır.
#endregion
#region Dezavantajları Nelerdir?
//EF Core ile In-Memory'de oluşturmuş olduğumuz veritabanında ilişkisel modellemeler, constraintler vs. yapılamaz.
//İlişkilerin, ilişkisel modellerin dışındaki tüm veritabanı senaryolarını In-Memory database üzerinde gerçekleştirip yapacağınız çalışmaları simüle edebilirsiniz.
//In-Memory'de yapılacak olan veritabanı işlevlerinde ilişkisel modellemeler YAPILAMAMAKTADIR! Bu durumdan dolayı veri tutarlılığı sekteye uğrayabilir ve istatiksel açıdan yanlış sonuçlar elde edilebilir.
#endregion
#region Örnek Çalışma
//Microsoft.EntityFrameworkCore.InMemory kütüphanesi uygulamaya yüklenmelidir..
await context.Persons.AddAsync(new() {Name ="Musa",Surname ="Uyumaz"});
await context.SaveChangesAsync();

var persons = await context.Persons.ToListAsync();
Console.WriteLine();
#endregion
//In-Memory database üzerinde çalışırken migration oluşturmaya ve migrate etmeye gerek yoktur!
//In-Memory'de oluşturmuş olduğumuz veritabanı uygulamayı sona erdiğinde/kapatığımızda dispose edilecektir. Veritabanı bellekten silinecektir.
//In-Memory'deki database'de verisel yapmış olduğunuz işlemler geçicidir. Kalıcı bir işlem yapamayız.
//Özellikler gerçek uygulamalarda in-memory database'i kullanıyorsanız bunun kalıcı değil geçici yani silinebilir bir özellik olduğunu UNUTMAYIN!
//EF Core'da hangi veritabanı ile çalışıyorsan çalış davranışı aynıdır DEĞİŞMEZ!
class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("exampleDatabase");
    }
}