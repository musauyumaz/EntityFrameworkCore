using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemporalTables.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("SqlServer:IsTemporal", true)//Bu kolonların IsTemporal konfigürasyonuyla Temporal Table'da kayda alınıp alınmayacağının bilgisi tutuluyor
                        .Annotation("SqlServer:TemporalHistoryTableName", "EmployeesHistory")//Hangi Temporal Table'da geçmiş zamansal davranışların tutulacağı da burada bildiriliyor
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")//Bu kolonlara karşılık zamansal kayıtların atılacağı bildirilmiştir.
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "EmployeesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "EmployeesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "EmployeesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "EmployeesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                })
                //Tablo seviyesindeki anatasyonlar. Oluşturulacak Temporal Table'ın konfigürasyonları burada sağlanıyor
                .Annotation("SqlServer:IsTemporal", true)//Burada bu tablonun Temporal table olacağını bildiriyoruz.
                .Annotation("SqlServer:TemporalHistoryTableName", "EmployeesHistory")//Temporal table tablosunun/yapılanmasının ismini de burada bildiriyoruz.
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")//Bu tabloda yapılacak bütün temporal table çalışmalarının veri kayıtlarını tarihsel zamanını tutabilmek için 2 kolona ihtiyacımız olacak bu kolonlardan bir tanesi end bir tanesi start olarak değerlendirilecek bu kolonlarıda burada bildiriyoruz.
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EmployeesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
