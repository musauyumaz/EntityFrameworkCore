using Microsoft.EntityFrameworkCore;
using System.Reflection;

ApplicationDbContext context = new();

#region Global Query Filters Nedir?
//Bir query'e global olarak filtre eklememizi sağlayan bir özelliktir.
//Bu özellik sayesinde bizler uygulama seviyesinde belirli entity'lere karşılık şartlar koyabiliyoruz ve bu entity'ler üzeriden yapılacak tüm sorgulama süreçlerinde default olarak uygulama seviyesinde bu şartları koyduğumuzdan dolayı ve yine default olarak bu şartlar doğrultusunda sorguların oluşturulmasını sağlayabiliyoruz.
//Bir entity'e özel uygulama seviyesinde genel/ön kabullü şartlar oluşturmamızı ve böylece verileri global bir şekilde filtrelememizi sağlayan bir özelliktir.
//Böylece belirtilen entity üzerinden yapılan tüm sorgulamalarda ekstradan bir şart ifadesinde gerek kalmaksızın filtreleri otomatik uygulayarak hızlıca sorgulama yapmamızı sağlamaktadır.
//Uygulama bazında herhangi bir entity'e karşılık yapılacak sorgulama süreçlerini Global bir ön tanımlı şart koymamızı sağlıyor ve bu entity üzerinden yapılan tüm sorgulama süreçlerinde ekstradan bu şartı koymamıza gerek kalmaksızın yani Where ile ya da firstOrDefault'la artık herahngi bir fonksiyonla bu şartı tekrardan yazmamıza gerek kalmaksızın tüm oluşturulan sorgularda bu şartı otomatik olarak ilgili sorguya ekliyor. Bunun için Global Query Filters ismi verilmiştir.
//Genellikle uygulama bazında aktif(IsActive) gibi verilerle çalışıldığı durumlarda,
//MultiTenancy uygulamalarda TenantId tanımlarken vs. kullanabiliriz.
#endregion
#region Global Query Filters Nasıl Uygulanır?
await context.Persons.Where(p => p.IsActive).ToListAsync();
await context.Persons.ToListAsync();
await context.Persons.FirstOrDefaultAsync(p => p.Name.Contains("a") || p.PersonId == 3);

//protected override void OnModelCreating(ModelBuilder modelBuilder)
//{
//    modelBuilder.Entity<Person>().HasQueryFilter(p => p.IsActive);
//}
#endregion
#region Navigation Property Üzerinde Global Query Filters Kullanımı?
//Uygulama çapında ilişkisel tabloları temsil eden navigation property'ler üzerinden de global şartlarınızı sorguya ekleyebilirsiniz. Daha doğrusu ilişkisel tablolardaki şartlarınızı global bir şekilde sorgularınıza ekleyip direkt çalışmanıza odaklı bir şekilde query'lerinizi inşa edebilirsiniz.
//Navigation property entity'lerin kendi aralarında navigasyonlarını sağlayan, ilişki türlerini ortaya koymamızı sağlayan özel property'lerdir.
//var p1 = await context.Persons
//    .AsNoTracking()
//    .Include(p => p.Orders)
//    .Where(p => p.Orders.Count > 0)
//    .ToListAsync();
//var p2 = await context.Persons.AsNoTracking().ToListAsync();
//Console.WriteLine();
//protected override void OnModelCreating(ModelBuilder modelBuilder)
//{
//    modelBuilder.Entity<Person>().HasQueryFilter(p => p.Orders.Count > 0);
//}

#endregion
#region Global Query Filters Nasıl Ignore Edilir? - IgnoreQueryFilters
//Tanımlanmış olan Global Query Filter'ı anlık olarak o sorguda o şartın göz ardı edilmesini istiyorsak IgnoreQueryFilters fonksiyonunu kullanabiliriz.
//var person1 = await context.Persons.IgnoreQueryFilters().ToListAsync();
//var person2 = await context.Persons.ToListAsync();
//Console.WriteLine();
#endregion
#region Dikkat Edilmesi Gereken Husus!
//Global Query Filters uygulanmış bir kolona farkında olmaksızın şart uygulanabilmektedir. Bu duruma dikkat edilmelidir.
//Saçma bir sorguya meydan verebilir.
//await context.Persons.Where(p=> p.IsActive).ToListAsync();
#endregion

public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }

    public List<Order> Orders { get; set; }
}
public class Order
{
    public int OrderId { get; set; }
    public int PersonId { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }

    public Person Person { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Person>().HasQueryFilter(p => p.IsActive);
        //modelBuilder.Entity<Person>().HasQueryFilter(p => p.Orders.Count > 0);
    }

    protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r!.;TrustServerCertificate=True");
    }
}