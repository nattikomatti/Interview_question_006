using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Example.InterviewQuestion006.Infrastructure.Persistence.Migrations
{
     
    public partial class InitialCreate : Migration
    {
       
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCode = table.Column<string>(type: "char(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.CheckConstraint("CK_Products_ProductCode", "LEN([ProductCode]) = 16 AND [ProductCode] COLLATE Latin1_General_BIN NOT LIKE '%[^A-Z0-9]%'");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCode",
                table: "Products",
                column: "ProductCode",
                unique: true);
        }

         
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
