

# Entity'de Service Inject Etme
```C#
var persons = await context.Persons.ToListAsync();
foreach (var person in persons)
{
    person.ToString();
}
```

- Entity üzerinde uygulama çapında herhangi bir servisi inject edebileyim.

- Bir servisin entity üzerinde kullanılabilir olmasını istiyorsam öncelikle entity'e bu servisle ilgili bir property kazandıracak arayüze ihtiyacım var. (IHasPersonService interface'i gibi)

- Arayüz kendi içerisinde arayüzün referans türünde bir property imzası barındırmakta ve biz bu arayüzümüzü entity'mize implement etmekteyiz.(IHasPersonService interface'i)

- Person entity'si IHasPersonService arayüzü ile bir PersonService instance'ını kendi içerisine inject edecektir. Bunu bildirmiş oluyoruz. Haliyle IHasPersonService'i Person'da implement ettiğimizde public IPersonLogService? PersonService { get; set; } property'e karşılık bir tanımlamada bulunuyoruz.

- Gerekli enjeksiyon neticesinde IPersonLogService enjeksiyonunu gerçekleştirdiğimde bu property kullanılabilir olacaktır.

- Bu vaziyette Person entity'sinin içindeki PersonService property'sine herhangi bir instance gelmeyecektir. Bunun için EF Core'da araya interceptor sokmamız gerekecektir. Interceptor'la buradaki property'e bir instance göndereceğiz. Yani enjeksiyonu/inject işlemini burada interceptor sayesinde gerçekleştireceğiz.

- Entity'nin içerisine bir inject bekleyen enjeksiyon sonucunda instance'ı referans edecek property yerleştiriyoruz. Bu property'i yerleştirirken bir arayüz sayesinde bunu gerçekleştiriyoruz. Ve interceptor görevini görecek bir sınıf üzerinde de bu arayüzden ilgili instance'ın bir instance beklediğini ve o instance'la da ilgili member'a bir işaretleme yapılacağını anlıyoruz haliyle o member'a karşılık bir instance üretip işaretliyoruz.

-  Tüm bunlardan sonra interceptor sınıfımızı context nesnesine gelip OnConfiguring içerisinde AddInterceptors(new PersonServiceInjectionInterceptor()); fonksiyonu eşliğinde vermemeiz yeterli olacaktır.

```C#
public class PersonServiceInjectionInterceptor : IMaterializationInterceptor
{
    public object InitializedInstance(MaterializationInterceptionData materializationData, object instance)
    {
        if (instance is IHasPersonService hasPersonService)
        {
            hasPersonService.PersonService = new PersonLogService();
        }

        return instance;
    }
}
public interface IHasPersonService //IPersonLogService referans türünde bir property imzası barındırmakta
{
    IPersonLogService PersonService { get; set; }
}
public interface IPersonLogService //Burada bir servisimin arayüzü bulunmakta ve içinde imzaları taşımaktadır
{
    void LogPerson(string name);
}
public class PersonLogService : IPersonLogService //Burada servisime gerekli imzaları doldurmuş bir şekilde bulunmakta 
{
    public void LogPerson(string name)
    {
        Console.WriteLine($"{name} isimli kişi loglanmıştır.");
    }
}
public class Person : IHasPersonService
{
    public int PersonId { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        PersonService?.LogPerson(Name);
        return base.ToString();
    }

    [NotMapped]
    public IPersonLogService? PersonService { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r!.;TrustServerCertificate=True");

        optionsBuilder.AddInterceptors(new PersonServiceInjectionInterceptor());

    }
}
```