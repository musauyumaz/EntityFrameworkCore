# Connection Resiliency Nedir?
- Veritabanı bağlantısında bir problem meydana geldiği taktirde o bağlantıyı tekrardan kurup gerekli aksiyonları almamızı sağlayan bir özelliktir.

- Bizler bir veri eklerken ya da bir transaction içerisinde uzun uzadıya çalışmalar gerçekleştirirken bir bağlantı kopuşu yaşıyorsak, bağlantıda kesinti yaşıyoorsak yapmış olduğumuz çalışmalarda veritabanına yansıtılamayacağından dolayı veri kaybı durumu söz konusu olacaktır.

- Biz genllikle bağlantı koptuğunda bağlantı yeniden kurulana kadar bekliyoruz halbuki bu çok doğru bir aksiyon değildir.

- Bağlantı koptuğunda öncelikle bağlantının yeniden kurulabilmesi için denemeler yapmalı tekrardan bağlantı taleplerinde buılunmamız lazım. 

- Bağlantının kopğtuğu noktalarda bazı önemli işlemler vardır ki onların gerçekten commit edilebilir olması lazımdır ve baştan sona tekrar işlenmesi gerekir. Baştan sona tekrardan oradaki sorgulama yapılanmalarının işlenmesi gerekir.

- Öncelikle veritabanının bağlantısını tekrar tekrar denemeye çalışacağız.

- Veritabanı bağlantısı tekrardan kurulursa eğer burada yapılamsı gereken kayıtları baştan tekrar elde edeceğiz ta ki bağlantı kurulduktan sonra buradaki kayıtlar buradaki trasaction commit edilene kadar süreci bizim baştan sarmamız gerekecektir.

- EF Core üzerinde yapılan veritabanı çalışmaları sürecinde ister istemez veritabanı bağlantısında kopuşlar/kesintiler vs. meydana gelebilmektedir.

- Connection Resiliency ile kopan bağlantıyı tekrar kurmak için gerekli tekrar bağlantı taleplerinde bulunabilir ve bir yandan da execution strategy dediğimiz davranış modellerini belirleyerek bağlantıların kopması durumunda tekrar edecek olan sorguları baştan sona yeniden tetikleyebiliriz.

- Böylece bir bağlantının koptuğu taktirde mümkün mertebe dayanıklılığını sağlayarak koptuğu transaction'ı baştan tetiklemeyi bu özellikle gerçekleştirebiliyoruz.

# EnableRetryOnFailure
- Uygulama sürecinde veritabanı bağlantısı koptuğu taktirde bu yapılandırma sayesinde bağlantıyı tekrardan kurmaya çalışabiliyoruz.

- EF Core üzerinde yapmış olduğumuz sorgulama süreçlerinde ufacık bir saniyede/milisaniyeler cinsinden bir bağlantı kesintisi söz konusu olsun uygulama orada hata fırlatıyor ve yapmış olduğu çalışmayı sonlandırıyor.

- Uygulamanın yaşm döngüsü için ve son kullanıcıya hitabı için burada hata fırlatmaktansa bağlantı kopukluğu durumunu yönetilebilir kılmak gerekir.İşte bunun için Connection Resiliency özelliğini aktifleştirmek gerekir.

- EF Core EnableRetryOnFailure bu özellikle birlikte default ayarlarda 30sn'de bir 6 kez bu bağlantıyı tekrardan denemeye çalışacaktır. Baktı ki en son bağlantı kesinlikle olmuyor o zaman ilgili yapılanma da hata verecektir.

```C#
optionsbuilder.usesqlserver("server=localhost, 1433;database=applicationdb;user id=sa;password=1q2w3e4r!.;trustservercertificate=true", builder => builder.enableretryonfailure()).logto(
    filter: (eventid, level) => eventid.id == coreeventid.executionstrategyretrying,
    logger: eventdata =>
    {
        console.writeline($"bağlantı tekrar kurulmaktadır.");
    });
//veritabanı bağlantısı olduğu durumlarda eğer ki bir kesinti meydana gelirse default ayarlarla sen bu kesintiye karşılık bağlantıyı tekrardan sağlamaya çalış.
//sqlserverdbcontextoptionsbuilder parametresi üzerinden buradaki sql server bağlantısına karşılık yapılacak davranışların gerekli konfigürasyonlarını belirtebiliyoruz.


while (true)
{
    await task.delay(2000);
    var persons = await context.persons.tolistasync();
    persons.foreach(p => console.writeline(p.name));
    console.writeline("*******************************");
}
```
# MaxRetryCount
- Yeniden bağlantı sağlanması durumunun kaç kere gerçekleştirileceğini bildirmektedir.

- Default değeri 6'dır.

# MaxRetryDelay
- Yeniden bağlantı sağlanması periyodunu bildirmektedir.

- Default değeri 30'dur.

```C#
optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r!.;TrustServerCertificate=True", builder => builder.EnableRetryOnFailure(
    maxRetryCount: 5,
    maxRetryDelay: TimeSpan.FromSeconds(15),
    errorNumbersToAdd: new[] { 4060 }
```

# Execution Strategies
- Eğer ki bir veritabanı bağlantısı kesilmesi durumu söz konusuysa ve sen Connection Resiliency kullanarak bu bağlantıyı tekrardan sağlamaya çalışıyorsan buradaki yapmış olduğun davranışa Execution Strategy derim diyor.

- EF Core ile yapılan bir işlem sürecinde veritabanı bağlantısı koptuğu taktirde yeniden bağlantı denenirken yapılan davranışa/alınan aksiyona Execution Strategy denmektedir.

- Bu stratejiyi default değerlerde kullanabileceğimiz gibi custom olarak da kendimize göre özelleştirebilir ve bağlantı loptuğu durumlarda istediğimiz aksiyonları alabiliriz.

# Default Execution Strategy
- Eğer ki Connection Resiliency için EnableRetryOnFailure metodunu kullanıyorsak bu default execution strategy'e karşılık gelecektir.

- MaxRetryCount : 6

- Delay : 30

- Default değerlerin kullanılabilmesi için EnableRetryOnFailure metodunun parametresiz overload'unun kullanılması gerekmektedir.

# Custom Execution Strategy

## Oluşturma
```C#
class CustomExecutionStrategy : ExecutionStrategy
{
    public CustomExecutionStrategy(DbContext context, int maxRetryCount, TimeSpan maxRetryDelay) : base(context, maxRetryCount, maxRetryDelay){}

    public CustomExecutionStrategy(ExecutionStrategyDependencies dependencies, int maxRetryCount, TimeSpan maxRetryDelay) : base(dependencies, maxRetryCount, maxRetryDelay){}
    int retryCount = 0;
    protected override bool ShouldRetryOn(Exception exception)
    {
        //Yeniden bağlantı durumunun söz konusu olduğu anlarda yapılacak işlemler...
        Console.WriteLine($"#{++retryCount}. Bağlantı tekrar kuruluyor...");
        return true;
    }
}
```

## Kullanma - Execution Strategy
```C#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r!.;TrustServerCertificate=True", builder => builder.ExecutionStrategy(dependencies => new CustomExecutionStrategy(dependencies, 3, TimeSpan.FromSeconds(15))));
}
while (true)
{
    await Task.Delay(2000);
    var persons = await context.Persons.ToListAsync();
    persons.ForEach(p => Console.WriteLine(p.Name));
    Console.WriteLine("*******************************");
}
```

# Bağlantı Koptuğu Anda Execute Edilmesi Gereken Tüm Çalışmaları Tekrar İşlemek
- Bu fonksiyon ile yapmış olduğumuz çalışma süreçlerinde veritabanı bağlantısı kesilem durumu söz konusu olursa tekrardan bağlantıyı sağladığı durumda ilgili çalışmaları baştan alıp işleyebilmemizi sağlamaktadır.

- EF Core ile yapılan çalışma sürecinde veritabanı bağlantısının kesildiği durumlarda, bazen bağlantının tekrardan kurulması tek başına yetmemekte kesintinin olduğu çalışmanın da baştan tekrardan işlenmesi gerekebilmektedir.İşte bu tarz durumlara karşılık EF Core Execute - ExecuteAsync fonksiyonunu bizlere sunmakatadır.

- Execute fonksiyonu, içerisine vermiş olduğumuz kodları commit edilene kadar işleyecektir. Eğer ki bağlantı kesilmesi meydana gelirse, bağlantının tekrardan kurulması durumunda Execute içerisindeki çalışmalar tekrar baştan işlenecek ve böylece yapılan işmein tutarlılığı için gerkli çalışma sağlanmış olacaktır.

```C#
var strategy = context.Database.CreateExecutionStrategy();
await strategy.ExecuteAsync(async () =>
{
    using var transaction = await context.Database.BeginTransactionAsync();
    await context.Persons.AddAsync(new() { Name = "Musa"});
    await context.SaveChangesAsync();

    await context.Persons.AddAsync(new() { Name = "Hilmi" });
    await context.SaveChangesAsync();

    await transaction.CommitAsync();
});
```

# Execution Strategy Hangi Durumlarda Kullanılır?
- Veritabanının şifresi belirli periyotlarda otomatik olarak değişen uygulamalarda güncel şifreyle connection string'i sağlayacak bir operasyonu custom execution strategy belirleyerek gerçekleştirebilirsiniz.



# DbContext & Entity & CustomExecutionStrategy
```C#
public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }
}
public class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        #region Default Execution Strategy
        //optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r!.;TrustServerCertificate=True", builder => builder.EnableRetryOnFailure(
        //    maxRetryCount: 5,
        //    maxRetryDelay: TimeSpan.FromSeconds(15),
        //    errorNumbersToAdd: new[] { 4060 }
        //    )).LogTo(
        //    filter: (eventId, level) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
        //    logger: eventData =>
        //    {
        //        Console.WriteLine($"Bağlantı tekrar kurulmaktadır.");
        //    });
        //Veritabanı bağlantısı olduğu durumlarda eğer ki bir kesinti meydana gelirse default ayarlarla sen bu kesintiye karşılık bağlantıyı tekrardan sağlamaya çalış.
        //SqlServerDbContextOptionsBuilder parametresi üzerinden buradaki SQL Server bağlantısına karşılık yapılacak davranışların gerekli konfigürasyonlarını belirtebiliyoruz. 
        #endregion
        #region Custom Execution Strategy
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r!.;TrustServerCertificate=True", builder => builder.ExecutionStrategy(dependencies => new CustomExecutionStrategy(dependencies, 10, TimeSpan.FromSeconds(15))));
        #endregion
    }
}
class CustomExecutionStrategy : ExecutionStrategy
{
    public CustomExecutionStrategy(DbContext context, int maxRetryCount, TimeSpan maxRetryDelay) : base(context, maxRetryCount, maxRetryDelay)
    {
    }

    public CustomExecutionStrategy(ExecutionStrategyDependencies dependencies, int maxRetryCount, TimeSpan maxRetryDelay) : base(dependencies, maxRetryCount, maxRetryDelay)
    {
    }
    int retryCount = 0;
    protected override bool ShouldRetryOn(Exception exception)
    {
        //Yeniden bağlantı durumunun söz konusu olduğu anlarda yapılacak işlemler...
        Console.WriteLine($"#{++retryCount}. Bağlantı tekrar kuruluyor...");
        return true;
    }
}
```