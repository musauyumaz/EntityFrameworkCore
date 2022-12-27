using Microsoft.EntityFrameworkCore;
using System.Reflection;


ApplicationDbContext context = new();


#region EF Core 7 Öncesi Toplu Güncelleme
//İlk olarak verileri veritabanından elde ediyoruz ardından döngü eşliğinde bu personlarda tek tek işlemlerimizi gerçekleştirip ardından SaveChanges fonksiyonunu çağırıyoruz. 
//var persons  = await context.Persons.Where(p=> p.PersonId > 5).ToListAsync();
//foreach (var person in persons)
//{
//    person.Name = $"{person.Name}...";
//}
//await context.SaveChangesAsync();
#endregion
#region EF Core 7 Öncesi Toplu Silme
////İlk olarak verileri veritabanından elde ediyoruz daha sonrasında bu verileri silmek için RemoveRange fonksiyonuna veriyoruz ve SaveChanges fonksiyonunu çağırıyoruz. 
//var persons = await context.Persons.Where(p => p.PersonId > 5).ToListAsync();
//context.RemoveRange(persons);
//await context.SaveChangesAsync();
#endregion
// EF Core 7'den önce toplu bir şekilde veri güncelleme ve silme işlemlerini yaparken biraz uzun uzadıya işlem yapmamız gerekiyordu.
#region ExecuteUpdate
//Hedef veriler elimdeyken daha hala IQueryable'dayken biz işlemlerimize devam ediyoruz. Bunları bir koleksiyona atıp ya da bir referansa atıp üzerinde döngüsel işlemler yapmaya gerek kalmayacak şekilde çalışmalarımızı gerçekleştirebiliyoruz.
//Bizi EF Core 7'den önceki birçok angarya işten kurtarmış oluyor herhangi bir döngüsel işleme vs girmeksizin elindeki hedef veriler üzerinde tek satırda güncelleme işlemini gerçekleştirebiliyoruz.
//ExecuteUpdate fonksiyonunu kullanırken string interpolation kullanmaksızın işlemi gerçekleştirmeniz önerilmektedir. Aksi taktirde expression yapılanmasında string interpolation çözülemeyeceğinden dolayı hataya sebeb olacaktır.
//await context.Persons.Where(p => p.PersonId > 3).ExecuteUpdateAsync(p=> p.SetProperty(p=>p.Name, v=> v.Name + " yeni"));//Böylelikle tek satırda elimizdeki hedef veri üzerinde güncelleştirme işlemini gerçekleştirmiş oluyoruz


#endregion
#region ExecuteDelete
//hedef verileri elde ettikten sonra yine herhangi bir döngüye gitmeksizin RemoveRange fonksiyonuna bu verileri vermeksizin daha IQueryable'dayken yani bunları ilk veritabanından in-memory'e çekip ondan sonra işleme tabi tutmaksızın direkt silmek istiyorsanız ExecuteDelete fonksiyonunu çağırabilirsiniz.
//await context.Persons.Where(p=> p.PersonId > 3).ExecuteDeleteAsync();
#endregion
//ExecuteUpdate ve ExecuteDelete fonksiyonlarıyla bulk(toplu) veri güncelleme ve veri silme işlemlerini gerçekleştirirken SaveChanges fonksiyonunu çağırmanız gerekmemektedir. Çünkü bu fonksiyonlar adları üzerinde Execute... fonksiyonlarıdır. Yani direkt veritabanına fiziksel etkide bulunurlar.
//Direkt Execute fonksiyonları bunlar yapacağınız işlemler direkt veritabanına execute edilecektir. Yani fiziksel olarak yansıtılacaktır.
//Eğer ki istiyorsanız transaction kontrolünü ele alarak bu fonksiyonların işlevlerini de süreçte kontrol edebilirsiniz.
public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }
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
        optionsBuilder
            .UseSqlServer("Server=localhost, 1433;Database=ApplicationDB;User ID=SA;Password=1q2w3e4r!.;TrustServerCertificate=True");
    }
}