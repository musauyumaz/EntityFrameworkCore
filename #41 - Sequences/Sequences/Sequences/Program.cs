﻿using Microsoft.EntityFrameworkCore;

ApplicationDbContext context = new();
Console.WriteLine();
#region Sequence Nedir?
//Veritabanında benzersiz ve ardışık sayısal değerler üreten bir veritabanı nesnesidir.
//Sequence herhangi bir tablonun özelliği değildir. Veritabanı nesnesidir. Birden fazla tablo tarafından kullanılabilir.
#endregion
#region Sequence Tanımlama
//Sequence'ler üzerinden değer oluştururken veritabanına özgü çalışma yapılması zaruridir. SQL Server'a özel yazılan Sequence tanımı misal olarak Oracle için hata verebilir.
#region HasSequence
//protected override void OnModelCreating(ModelBuilder modelBuilder)
//{
//    modelBuilder.HasSequence("EC_Sequence");
//}
#endregion
#region HasDefaultValueSql
//Tanımlanan Sequence'in hangi entity'lere karşılık generate edilmiş tablolarda hangi kolonlara karşılık kullanılacağını bu fonksiyon ile bildiririz.

//protected override void OnModelCreating(ModelBuilder modelBuilder)
//{
//    modelBuilder.HasSequence("EC_Sequence");

//    modelBuilder.Entity<Employee>()
//        .Property(e => e.Id)
//        .HasDefaultValueSql("NEXT VALUE FOR EC_Sequence");

//    modelBuilder.Entity<Customer>()
//        .Property(c => c.Id)
//        .HasDefaultValueSql("NEXT VALUE FOR EC_Sequence");
//}
#endregion
#endregion

await context.Employees.AddAsync(new() { Name = "Musa", Surname = "Uyumaz", Salary = 1000 });
await context.Employees.AddAsync(new() { Name = "Gençay", Surname = "Yıldız", Salary = 1000 });
await context.Employees.AddAsync(new() { Name = "Mustafa", Surname = "Yıldız", Salary = 1000 });
await context.Employees.AddAsync(new() { Name = "Şuayip", Surname = "Yıldız", Salary = 1000 });

await context.Customers.AddAsync(new() {Name = "Muiddin" });
await context.SaveChangesAsync();

#region Sequence Yapılandırması

#region StartsAt

#endregion
#region IncrementsBy

#endregion
//protected override void OnModelCreating(ModelBuilder modelBuilder)
//{
//    modelBuilder.HasSequence("EC_Sequence")
//        .StartsAt(100)
//        .IncrementsBy(5);
//}
#endregion
#region Sequence İle Identity Farkı
//Sequence fıtrat olarak bir veritabanı nesnesidir Identity ise herhangi bir tablonun içerisindeki özelliktir.Sequence veritabanına özel bir nesneyken Identity tablolardaki bir özellik.
//Sequence veritabanında bir nesne olmasından dolayı herhangi bir tabloya bağımlı olmuyor. Haliyle istediğiniz kadar tabloyla bir tane sequence'i kullanabilirsiniz.
//Idenity bir sonraki değeri diskten alırken Sequence ise Ram'den almaktadır. Bu yüzden önemli ölçüde identity'e nazaran daha hızlı bir çalışma sergileyecektir. Yani Sequence Identity'e nazaran daha hızlı ve performanslıdır. Maliyeti biraz daha düşük yapılanmalardır.
//Sequence bir veritabanı nesnesiyken, Identity ise tabloların özellikleridir.
//Yani Sequence herhangi bir tabloya bağımlı değildir.
//Identity bir sonraki değeri diskten alırken Sequence ise RAM'den alır. Bu yüzden önemli lçüde Identity'e nazaran daha hızlı, performanslı ve az maliyetlidir.
#endregion
class Employee
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }
}
class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence("EC_Sequence")
            .StartsAt(100)
            .IncrementsBy(5);

        modelBuilder.Entity<Employee>()
            .Property(e => e.Id)
            .HasDefaultValueSql("NEXT VALUE FOR EC_Sequence");

        modelBuilder.Entity<Customer>()
            .Property(c => c.Id)
            .HasDefaultValueSql("NEXT VALUE FOR EC_Sequence");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ApplicationDb;User ID=SA;Password=1q2w3e4r!.;");
    }
}