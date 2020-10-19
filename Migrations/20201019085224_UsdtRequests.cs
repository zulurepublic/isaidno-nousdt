using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OKexTime.Migrations
{
    public partial class UsdtRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestUsdts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    EthereumAddress = table.Column<string>(maxLength: 42, nullable: true),
                    ExpectedAmount = table.Column<decimal>(nullable: false),
                    Settled = table.Column<bool>(nullable: false),
                    TxId = table.Column<string>(maxLength: 100, nullable: true),
                    EthereumStatus = table.Column<string>(maxLength: 100, nullable: true),
                    UserId = table.Column<string>(maxLength: 12, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestUsdts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestUsdts");
        }
    }
}
