# Transaction Nedir?
- Bir veritabanı üzerinde işlem yaparken bu işlemin bir transaction üzerinde gerçekleştirilmesi gerekir.

- Transaction veritabanı işlemlerini birden fazla kümülatif olan işlemleri atomik bir şekilde ggerçekleştirmemizi sağlayan bir yapılandırmadır.

- Transaction'ın buradaki gayesi veritabanı işlemlerini yaparken olabilecek herhangi bir hata veyahut başarısızlık durumlarında veritabanı tutarlılığını sağlayabilmektir.

- Diyelim ki 10 tane işlem yapıyorsun işte veri ekliyorsun/güncelliyorsun/siliyorsun buradaki işlem süreçlerinde bunlar kümülatif olarak bir bütünlük arz edecekse ve süreç gerçekleşirken bir yerde hata ya da başarısızlık durumu meydana geldiğinde orada Transaction'da o noktaya kadar yapılan tüm işlemlerin geri alınması veritabanı tutarlılığı açısından kritik arz edecektir. 

- Eğer ki bizler yapılan işlemleri hata ya da başarısızlık durumu arz edene kadar işlemleri geri almazsak burada veritabanının bütünselliği açısından istatiksel açıdan tutarsızlık meydan gelebilir. İşte bu tutarsızlıkları öyle ya da böyle engelleyebileceğimiz yapılar/teknolojiler/yaklaşımlar mevcuttur. Veritabanı seviyesinde bu yapı transaction'dır.

- Transaction, veritabanındaki kümülatif işlemleri atomik bir şekilde gerçekleştirmemizi sağlayan bir özelliktir.

- Transaction'ın genel davranışı içerisinde ne kadar işlem olursa olsun bir Transaction Commit edildiği taktirde bu işlemleri fiziksel olarak veritabanına işletecektir. Yok eğer Commit edilmiyorsa yahut Rollback ediliyorsa yapılan işlemler geri alınmış olacaktır.

- Bir Transaction içerisindeki tüm işlemler Commit edildiği taktirde veritabanına fiziksel olarak yansıtılacaktır. Ya da Rollback edilirse tüm işlemler geri alınacak ve fiziksel olarak veritabanında herhangi bir verisel değişiklik durumu söz konusu olmayacaktır.

- Transaction'ın genel amacı veritabanındaki tutarlılık durumunu korumaktır. Ya da bir başka deyişle veritabanındaki tutarsızlık durumlarına karşı önlem almaktır.

# Default Transaction Davranışı?
- EF Core'u kullandığımızda default bir transaction söz konusudur.

- Normalde veritabanı seviyesinde de herhangi bir sorguyu basitinden çalıştırdığınızda onda da default bir transaction geçerli olacaktır.

- Yani sen bir transaction'ı iradenle yazsan da yazmasan da herhangi bir sorguyu mesela bir insert sorgusunu çalıştırıyorsan eğer default bir transaction'la ya da yazdıysan eğer yazmış olduğunla o sorgu veritabanına işlenmiş/çalıştırılmış olacaktır.

- Eğer ki yazmış olduğun sorguda Default Transaction kullanıyorsan ve herhangi bir hata ya da başarısızlık durumu meydana geldiyse Default Transaction rollback yaparak yapılan işlemi geri alacaktır. yok eğer sen kendi iradenle bir transaction oluşturup işlem yapıyorsan yine bir hata meydan geldiyse burada da Rollback işlemini evreye sokarak yapılan işlemleri geri alabiliriz.

- Default Transaction davranışı EF Core açısından şu mantıkla değerlendirilecektir; Bizler EF Core'da programatik olarak inşa etmiş olduğumuz sorguları SaveChanges fonksiyonunu çağırarak veritabanına göndeririz. SaveChanges fonksiyonunu çağırdığınızda arka planda varsayılan bir Transaction'la yapmış olduğunuz işlemler veritabanına gönderilir. Veritabanına gönderildiği taktirde de eğer ki bu işlemler sürecinde herhangi bir hata ya da başarısızlık durumu meydana gelirse Rollback edilirler yok eğer bir başarısızlık durumu söz konusu değilse o zaman default olarak bunlar commit edilirler.

- SaveChanges'ı kullanıyorsanız eğer sizler bir Transaction'ı başlatıyorsunuz anlamına geliyor.

- EF Core'da varsayılan olarak, yapılan tüm işlemler SaveChanges fonksiyonuyla veritabanına fiziksel olarak uygulanır.

- Çünkü SaveChanges default olarak bir transaction'a sahiptir.

- Eğer ki bu süreçte bir problem/hata/başarısızlık durumu söz konusu olursa tüm işlemler geri alınır ve işlemlerin hiçbiri veritabanına uygulanmaz.

- Böylece SaveChanges tüm işlemlerin ya tamamen başarılı olacağını ya da bir hata oluşursa veritabanını değiştirmeden işlemleri sonlandıracağını ifade etmektedir.

- SaveChanges'taki bu default Transaction davranışı esasında birçok uygulama için yeterli olacaktır. Orta ölçekli ya da kompleks uygulamlarda da dahil EF Core kullanıyorsanız SaveChanges'taki bu default Transaction davranışı sizlerin herhangi bir Transaction'ı iradenizle yönetme gibi bir duruma ihtiyaç hissettirmeksizin işlemlerinizi/ihtiyaçlarınızı görecektir.

# Transaction Kontrolünü Manuel Sağlama
```C#
IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
```
- EF Core'da transaction kontrolü iradeli bir şekilde manuel yani elde etmek istiyorsak eğer BeginTransactionAsync fonksiyonu çağırılmalıdır.

- BeginTransactionAsync fonksiyonu ile context'teki Transaction'ı elde ettiysen bundan sonra yapmış olduğun işlemlerde bu transaction'ı commit'lemediğin sürece herhangi bir fiziksel değişiklik veritabanına göremeyeceksin. Çünkü artık Transaction default değil senin elinde olan/iradenle yönettiğin Transaction'dır.

```C#
Person p = new() { Name = "Abuzer" };
await context.Persons.AddAsync(p);
await context.SaveChangesAsync();
```

- Burada bir işlem gerçekleştirdik ve SaveChanges'ı çağırdığımızda artık burada SaveChanges'a şunu demiş oluyoruz : Bak kardeşim senin default Transaction'ını ben ezdim. Transaction ile ilgili irade/karar mekanizması benim elimde. Ben bunu commit'lemediğim sürece senin yapacağın işlemin herhangi bir anlamı yok. 

- Dolayısıyla bizler burada Transaction'ının kontrolünün manuel bir şekilde ele aldıysak eğer yapılan işlemler neticesinde bu işlemlerin sonuçlarının veritabanına fiziksel olarak yansıtılmasını istiyorsak bunun iradeli bir şekilde Commit edilmesi gerekmektedir.

- Transaction iradeli bir şekilde kullanılıyorsa SaveChanges sadece execute operasyonu için geçerlidir. Bu execute edilen verinin ya da işlemlerin veritabanında fiziksel olarak kaydı sağlanabilmesi/işlenebilmesi için bu Transaction'ının Commit edilmesi gerekmektedir.

- Transaction'ı Commit etmezseniz defaut olarak Rollback anlamına gelecektir. Yani ekstradan Rollback yapmanıza bir daha gerek yoktur.

```C#
await transaction.CommitAsync();
```

# Savepoints
- EF Core 5.0 sürümüyle gelmiştir.

- Bazen yapıtığımız operasyonlarda veritabanı işlemlerini yoğun ve yığın bir şekilde yapmak zorunda kalabiliyoruz. Gerçek bir uygulama da bazen ister istemez Transaction çok yoğun olabiliyor yani içerisinde insert'ler, update,ler, delete'ler select sorgusuyla aldığın verinin kontrolleri vs. İşte bu tarz operasyonlarda ister istemez bazen hatalar oluşabiliyor yahut mantıksal olarak belirli durumlara göre bazı işlemlerin geri alınması da istenebiliyor. Bu tarz kompleks sorgularda/transaction'larda kompleks işlemler barındıran Transaction'larda bizler dönüş yapılabilir yani Transaction'da rollback yapılabilir noktaları işaretleyerek bir hata meydana geldiyse şu noktaya kadar bütün işlemleri rollback'le yahut bir mantıksal durum söz konusuysa şu noktaya kadar bütün işlemleri geri al gibi mantık yürütmek isteyebiliriz. İşte bunu EF Core'da SavePoints'ler yardımıyla gerçekleştirebiliyoruz.

- Savepoints veritabanı işlemleri sürecinde bir hata oluşursa veya başka bir nedenle yapılan işlemlerin geri alınması gerekiyorsa transaction içerisinde dönüş yapılabilecek noktaları ifade eden bir özelliktir.

- Savepoint tanımlayabilmemiz için Transaction kontrolünü iradeli bir şekilde ele almamız gerekiyor.

## CreateSavepoint
- Yapılan işlemlerde bir geri dönüş noktası oluşturmak istiyorsanız eğer bu metodu kullanbilirsiniz.

- Transaction içerisinde geri dönüş noktası oluşturmamızı sağlayan bir fonksiyondur.

## RollbackToSavepoint
- Herahngi bir SavePoint'e Transaction Rollback etmek istiyorsanız bu metodu kullanabilirsiniz.

- Transaction içerisinde herhangi bir geri dönüş noktasına(Savepoint'e) Rollback yapmamızı sağlayan fonksiyondur.

- Savepoints özelliği bir Transaction içerisinde istenildiği kadar kullanılabilir.

```C#
IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

Person p11 = await context.Persons.FindAsync(11);
Person p13 = await context.Persons.FindAsync(13);
context.Persons.RemoveRange(p11,p13);
await context.SaveChangesAsync();

await transaction.CreateSavepointAsync("t1");

Person p10 = await context.Persons.FindAsync(10);
context.Persons.Remove(p10);
await context.SaveChangesAsync();

await transaction.RollbackToSavepointAsync("t1");
await transaction.CommitAsync();
```

# TransactionScope
- Veritabanı işlemlerini kod üzerinde bir grup olarak yapmamızı sağlayan hazır bir sınıftır. Yani bildiğiniz Transaction dürecini ayriyeten yönetmemizi sağlayan bir sınıftır.

- Veritabanı işlemlerini bir grup olarak yapmamızı sağlayan bir sınıftır.

- ADO.NET ile de kullanılabilir.

- using bloğu ne zaman ki TransactionScope'u dispose eder işte o zaman yapılan tüm veritabanı işlemleri rollback yapılır.

```C#
using TransactionScope transactionScope = new();
//Veritabanı işlemleri...
//..
//..
transactionScope.Complete(); //Complete fonksiyonu yapılan veritabanı işlemlerinin commit edilmesini sağlar.
//Eğer ki Rollback yapacaksanız Complete fonksiyonunun tetiklenmemesi yeterlidir.
```

# Entity & DbContext
```C#
public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }

    public ICollection<Order> Orders { get; set; }
}
public class Order
{
    public int OrderId { get; set; }
    public int PersonId { get; set; }
    public string Description { get; set; }

    public Person Person { get; set; }
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
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB; User ID=SA; Password=1q2w3e4r!.;TrustServerCertificate=True;");
    }
}
```