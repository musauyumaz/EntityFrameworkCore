# Value Conversions Nedir?
- EF Core ile veritabanı üzerinde sorgulama yaptığımız durumlarda veriler üzerinde dönüşümler yapmamızı sağlayan bir özellikten bahsediyoruz. Ve bu verileri fiziksel olarak veritabanına da kaydedebiliyoruz. Veritabanında tuttuğumuz verileri sorgulama yaptığımızda salt olarak gelen verileri dönüştürerek elde etmek istiyorsak Value Conversions yapıalnmasını kullanmamız gerekecektir. Örneğin cinsiyetin E, K gibi değerler olarak tutulduğu bir kolonu Erkek Kadın gibi göstermek gibi.

- EF Core üzerinden veritabanı ile ytapıaln sorgulama süreçlerinde veriler üzerinde dönüşümler yapmamızı sağlayan bir özelliktir.

- SELECT sorguları sürecinde gelecek olan veriler üzerinde dönüşüm yapabiliriz. Ya da UPDATE yahut INSERT sorgularında da yazılım üzerinden veritabanına gönderdiğimiz veriler üzerinde de dönüşümler yapabilir ve böylece fiziksel olarak da verileri manipüle edebiliriz.

# Value Converter Kullanımı Nasıldır?
- Value Conversions özelliğini EF Core'daki Value Converter yapıları ile uygulayabilmekteyiz.

# HasConversion
- HasConversion fonksiyonu en sade haliyle EF Core üzerinden Value Converter özelliği gören bir fonksiyondur.

- İlk parametresinde INSERT UPDATE durumlarında entity'i veritabanına eklerken nasıl bir davranış sergileneceğini bilidirirken ikinci parametre de ise SELECT sorgusu süreçlerinde dönüşüm davranışını bildirmemizi sağlar.

```C#
var persons = await context.Persons.ToListAsync();
Console.WriteLine();
```
```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Person>()
        .Property(p => p.Gender)
        .HasConversion(
        //INSERT - UPDATE
        g => g.ToUpper()
        ,
        //SELECT
        g => g == "M" ? "Male" : "Female"
        );
}
```

# Enum Değerler İle Value Converter Kullanımı
- Eğer ki enum değerlerde ValueConverter kullanmadan direkt olarak veritabanına Migrate etseydik eğer Kolonun tipini int olarak belirleyecekti. Amma velakin biz araya converter koyduğumuzda ve bu kolonun türünü yani insert işleminde gönderilecek datanın string olduğunu bildirdiğimizde bunun üstüne bir migration basarsak eğer oradaki Conversion yapılanmasının türü neyse ona göre ilgili kolonu güncellemektedir.

- Yani sen Conversion'da hangi türle çalışıyorsan fiziksel olarak veritabanına hangi türde veri gönderiyorsan migration'da o türde ilgili kolonu oluşturur/generate eder.

- Buna dikkat etmeliyiz EF Core bu hassasiyete sahiptir. Eğer ki böyle bir çalışma yapmasaydı biz enum kullandığımız property'lerde int dışında herhangi bir dönüşüm yapamıyor olurduk. O zaman da böyle bir çalışmaya ihtiyacımız olmazdı.

- Burada önemli olan enum değerlerin kullanıldığı çalışmalarda int dışındaki türlerde nasıl dönüşüm yapabildiğimiz değerlendirmek.

- Enum'ın default türü int'e karşılık gelir.

- EF Core'da artık hangi türe karşı çalışıyorsak o türde bir dönüşüm bir tablo oluşturma Generate etme davranışı sergiliyor.

- Normal durumlarda Enum türünder tutulan property'lerin veritanbanındaki karşılıkları int olacak şekilde aktarımı gerçekleştirilmektedir. Value converter sayesinde enum türünden olan property'lerinde dönüşümlerini istediğimiz türlere sağlayarak hem ilgili kolonun türünü o türde ayarlayabilir hem de enum üzerinden çalışma sürecinde verisel dönüşümleri ilgili türde sağlayabiliriz.

```C#
var person = new Person() { Name = "Rakıf", Gender = "M", Gender2 = Gender.Male };
await context.Persons.AddAsync(person);
await context.SaveChangesAsync();
var _person = await context.Persons.FindAsync(person.Id);
Console.WriteLine();
```
```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Person>()
        .Property(p => p.Gender2)
        .HasConversion(
        //INSERT - UPDATE
        g => g.ToString(),
        //g => (int)g,

        //SELECT
        g => (Gender)Enum.Parse(typeof(Gender), g)
        );
}

```
# ValueConverter Sınıfı
- Bizim conversion işlerini yürütebileceğimiz bir sınıftır. Yani bu sınıfın instance'ı üzerinde artık hangi property'e karşılık bir dönüşüm gerçekleştireceksek bu sınıfın instance'ı üzerinde bu işlemi yaparak bu instance'ı HasConversion fonksiyonuna vermemiz yeterli olacaktır.

- ValueConverter sınıfı verisel dönüşümlerdeki çalışmaları/sorumlulukları üstlenebilecek bir sınıftır.

- Yani bu sınıfın instance'ı ile HasConversion fonksiyonunda yapılan çalışmaları üstlenebilir ve direkt bu instance'ı ilgili fonksiyona vererek dönüşümsel çalışmalarımızı gerçekleştirebiliriz.

```C#
var person = await context.Persons.FindAsync(11);
Console.WriteLine();
```
```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    ValueConverter<Gender, string> valueConverter= new(
        //INSERT - UPDATE
        g => g.ToString(),
        //SELECT
        g => (Gender)Enum.Parse(typeof(Gender), g));

    // modelBuilder.Entity<Person>()
        .Property(p => p.Gender2)
        .HasConversion(valueConverter); 
}

```

# Custom ValueConverter Sınıfı
- Veriler üzerinde dönüşüm yaparken bunu kendi oluşturduğumuz custom sınıfımız üzerinden yapmamızı sağlayan yapılanmadır.

- EF Core'da verisel dönüşümler için custom olarak custom olarak converter sınıfları üretebilmekteyiz. Bunun için tek yapılması gereken custom sınıfının ValueConverter sınıfından miras almasını sağlamaktadır.

```C#
public class GenderConverter : ValueConverter<Gender, string>
{
    public GenderConverter() : base(
        //INSERT - UPDATE
        g => g.ToString()
        ,
        //SELECT
        g => (Gender)Enum.Parse(typeof(Gender), g)
        )
    {
    }
}
```
```C#
var person = await context.Persons.FindAsync(11);
console.WriteLine();
```
```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Person>()
       .Property(p => p.Gender2)
       .HasConversion<GenderConverter>(); 
}
```

# Built-in Converters Yapıları
- EF Core basit dönüşümler için kendi bünyesinde yerleşik Convert sınıfları barındırmaktadır.

- [Diğer Built-in Converters](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations)

# BoolToZeroOneConverter
- bool türündeki veriyi 0 1 olarak veritabanında dönüştürerek tutmak ve o şekilde elde etmek istiyorsak yani 0 1'i boolean true ya da false olarak elde etmek istiyorsak bu yapılanmayı kullanabiliriz.

- bool olan verinin int olarak tutulmasını sağlar.

```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Person>()
       .Property(p => p.Married)
       .HasConversion<BoolToZeroOneConverter<int>>(
    
    //ya da direkt aşağıdaki gibi int türünü bildirirsek de aynı davranış söz konusu olacaktır.
    modelBuilder.Entity<Person>()
       .Property(p => p.Married)
       .HasConversion<int>();
}

```

# BoolToStringConverter
- bool türleri için string olarak veritabanına kaydedip aynı şekilde boolean olarak elde etmek istiyorsak BoolToStringConverter'ı kullanabiliriz.

- bool olan verinin string olarak tutulmasını sağlar.

```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    BoolToStringConverter converter = new("Bekar", "Evli")

    modelBuilder.Entity<Person>()
       .Property(p => p.Married)
       .HasConversion(converter);
}

```

# BoolToTwoValuesConverter
- bool türünde char türünde veriyi veritabanında tutup ona göre elde etmek istiyorsak char'dan boolean'a dönüştürerek elde etmek istiyorsak BoolToTwoValuesConverter sınıfını kullanabiliriz.

- bool olan verinin char olarak tutulmasını sağlar.

```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    BoolToTwoValuesConverter<char> converter = new('B', 'E');

    modelBuilder.Entity<Person>()
       .Property(p => p.Married)
       .HasConversion(converter); 
}

```

# İlkel Koleksiyonların Serilizasyonu
- Entity'lerimiz içinde ilkel türden koleksiyonlar barındırabilmek için burada Conversion yapılanmasını kullanabiliriz. 

- Conversion yapılanmasından faydalanarak serilizasyonun nasıl yapıalcağını EF Core'a bildirebiliriz.

- İçerisinde ilkel türlerden oluşturulmuş koleksiyonları barındıran modelleri migrate etmeye çalıştığımızda hata ile karşılaşmaktayız. Bu hatadan kurtulmak ve ilgili veriye koleksiyondaki verileri serilize ederek işleyebilmek için bu koleksiyonu normal metinsel değerlere dönüştürmemize forsat veren bir conversion operasyonu gerçekleştirebiliriz.

```C#
var person = new Person() { Name = "Filanca", Gender = "M", Gender2 = Gender.Male, Married = true, Titles = new() { "A", "B", "C" } };
await context.Persons.AddAsync(person);
await context.SaveChangesAsync();

var _person = await context.Persons.FindAsync(person.Id);
Console.WriteLine();
```
```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Person>()
       .Property(p => p.Titles)
       .HasConversion(
        //INSERT - UPDATE
        t => JsonSerializer.Serialize(t, (JsonSerializerOptions)null)
        ,
        //SELECT
        t => JsonSerializer.Deserialize<List<string>>(t, (JsonSerializerOptions)null)
        );
}

```

# .NET 6 - Value Converter For Nullable Fields
- .NET 6'dan önce Value Converter'lar null değerlerin dönüşümünü desteklememekteydi. .NET 6 ile artık null değerler de dönüştürülebilmektedir.

# Entity & DbContext
```C#
public class GenderConverter : ValueConverter<Gender, string>
{
    public GenderConverter() : base(
        //INSERT - UPDATE
        g => g.ToString()
        ,
        //SELECT
        g => (Gender)Enum.Parse(typeof(Gender), g)
        )
    {
    }
}
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public Gender Gender2 { get; set; }
    public bool Married { get; set; }
    public List<string>? Titles { get; set; }
}
public enum Gender
{
    Male,
    Female
}
public class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        #region Value Converter Kullanımı Nasıldır?
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Gender)
        //    .HasConversion(
        //    //INSERT - UPDATE
        //    g => g.ToUpper()
        //    ,
        //    //SELECT
        //    g => g == "M" ? "Male" : "Female"
        //    ); 
        #endregion
        #region Enum Değerler İle Value Converter Kullanımı
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Gender2)
        //    .HasConversion(
        //    //INSERT - UPDATE
        //    g => g.ToString(),
        //    //g => (int)g,

        //    //SELECT
        //    g => (Gender)Enum.Parse(typeof(Gender), g)
        //    );
        #endregion
        #region ValueConverter Sınıfı
        //ValueConverter<Gender, string> valueConverter= new(
        //    //INSERT - UPDATE
        //   g => g.ToString(),
        //   //SELECT
        //   g => (Gender)Enum.Parse(typeof(Gender), g));

        //modelBuilder.Entity<Person>()
        //   .Property(p => p.Gender2)
        //   .HasConversion(valueConverter);
        #endregion
        #region Custom ValueConverter Sınıfı
        //modelBuilder.Entity<Person>()
        //   .Property(p => p.Gender2)
        //   .HasConversion<GenderConverter>(); 
        #endregion
        #region BoolToZeroOneConverter
        //modelBuilder.Entity<Person>()
        //   .Property(p => p.Married)
        //   .HasConversion<BoolToZeroOneConverter<int>>();

        //ya da direkt aşağıdaki gibi int türünü bildirirsek de aynı davranış söz konusu olacaktır.
        //modelBuilder.Entity<Person>()
        //   .Property(p => p.Married)
        //   .HasConversion<int>();
        #endregion
        #region BoolToStringConverter
        //BoolToStringConverter converter = new("Bekar", "Evli");

        //modelBuilder.Entity<Person>()
        //   .Property(p => p.Married)
        //   .HasConversion(converter);
        #endregion
        #region BoolToTwoValuesConverter
        //BoolToTwoValuesConverter<char> converter = new('B', 'E');

        //modelBuilder.Entity<Person>()
        //   .Property(p => p.Married)
        //   .HasConversion(converter);
        #endregion
        #region İlkel Koleksiyonların Serilizasyonu
        modelBuilder.Entity<Person>()
           .Property(p => p.Titles)
           .HasConversion(
            //INSERT - UPDATE
            t => JsonSerializer.Serialize(t, (JsonSerializerOptions)null)
            ,
            //SELECT
            t => JsonSerializer.Deserialize<List<string>>(t, (JsonSerializerOptions)null)
            );
        #endregion
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r!.;TrustServerCertificate=True");
    }
}
```