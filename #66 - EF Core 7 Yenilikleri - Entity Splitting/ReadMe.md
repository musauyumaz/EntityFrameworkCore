# Entity Splitting
- Birden fazla fiziksel tabloyu Entity Framework Core kısmında tek bir entity ile temsil etmemizi sağlayan bir özelliktir.

- Entity'lerimizi bölmemizi sağlayan bir özelliktir.

- Entity'i fiziksel olarak bölmüyoruz. Entity'nin veritabanındaki yansımasını bölüyoruz.

- Yani birden fazla tablomuz var bu tabloların hepsini EF Core/kodlama kısmında tek bir entity üzerinde kümülatif olarak birleştirmemizi sağlayan bir özelliktir.

- Bütünsel olarak oluşturmuş olduğumuz bir entity'i belirli mantıksal ayrımlar neticesinde tablolara ayrılması özelliğidir.

- Bir entity'nin birden fazla tablodaki verileri bütünsel olarak alarak/çekerek bunları tek bir instance olarak nizlere sunmasını istiyorsak Entity Splitting özelliğini kullanabiliriz.

- Entity'nin arka planda çalıştığı tablo sayısını arttırabilmektedir. Tek bir tabloya bağlı entity'lerden ziyade birden fazla tabloyla verisel işlemleri gerçekleştiren tekil entity'ler oluşturmamızı sağlayan bir özelliktir.

- Konfigürasyon sürecinde ise DbContext nesnesinde OnModelCreating metodunda ilgili entity'e ModelBuilder üzerinden Entity fonksiyonuyla generic olarak giriyoruz. Ardından EntityBuilder bekleyecek bizden EntityBuilder'a ToTable diyerek ana tabloyu burada bildiriyoruz. Ondan sonra SplitToTable fonksiyonunu çağırarak burada ayrıştırılacak kolonları/property'leri belirliyoruz. İlk parametre de ayrıştırılacak tablonun adını sonraki parametre'de ise bu tabloya verilecek kolonları belirliyoruz. 

# Veri Eklerken
- Entity üzerinde ekleme operasyonu gerçekleştirirken bu ekleme sürecinde ilgili property'leri ilişkilendirildikleri tablolara gönderecek onlara insert oluşturacak o tablolara ekleyebilecek bir insert sorgusu generate edecek.

```C#
Person person = new()
{
    Name = "Serhat",
    Surname = "Uyumaz",
    City = "Eskişehir",
    Country = "Türkiye",
    PhoneNumber= "12345678910",
    PostCode = "1234567890",
    Street = "..."
};

await context.Persons.AddAsync(person);
await context.SaveChangesAsync();
```

# Veri Okurken
- Entity üzerinde sorgulama operasyonu gerçekleştirirken ilgili tablolardan sorgulama yapıp bu nesneyi kümülatif olarak sizlere sunacaktır.

- Sorgulama sürecinde tüm verileri join yapılanmaları eşliğinde ilgili tablolardan toplayarak bize kümülatif olarak tek bir instance üzerinde vermektedir.

```C#
person = await context.Persons.FindAsync(1);
Console.WriteLine();
```

# Entity & DbContext
```C#
public class Person//Bu entity'i veritabanına yansıtırken/veritabanında bu entity ile ilgili verileri tutarken esasında 3 farklı tablo üzerinde işlem gerçekleştireceğiz. Yani person entity'si veritabanı seviyesinde birden fazla tabloya karşılık geliyor.
{
    #region Persons Tablosu
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    #endregion
    #region PhoneNumbers Tablosu
    public string? PhoneNumber { get; set; }
    #endregion
    #region Addresses Tablosu
    public string Street { get; set; }
    public string City { get; set; }
    public string? PostCode { get; set; }
    public string Country { get; set; }
    #endregion
}

class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entityBuilder =>
        {
            entityBuilder.ToTable("Persons")
            .SplitToTable("PhoneNumbers", tableBuilder =>
            {
                tableBuilder.Property(person => person.Id).HasColumnName("PersonId");
                tableBuilder.Property(person => person.PhoneNumber);
            })
            .SplitToTable("Addresses", tableBuilder =>
            {
                tableBuilder.Property(person => person.Id).HasColumnName("PersonId");
                tableBuilder.Property(person => person.Street);
                tableBuilder.Property(person => person.City);
                tableBuilder.Property(person => person.PostCode);
                tableBuilder.Property(person => person.Country);
            });
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database = ApplicationDB; User ID=SA; Password=1q2w3e4r!.;TrustServerCertificate=True;");
    }
}
```