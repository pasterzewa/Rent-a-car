using Microsoft.EntityFrameworkCore.Migrations;

namespace Rent_a_Car.Migrations
{
    public partial class migration7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentCar_ReturnFile_ReturnFileID",
                table: "RentCar");

            migrationBuilder.DropIndex(
                name: "IX_RentCar_ReturnFileID",
                table: "RentCar");

            migrationBuilder.DropColumn(
                name: "ReturnFileID",
                table: "RentCar");

            migrationBuilder.DropColumn(
                name: "NumberOfOverallRentedCar",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "NumberOfRentedCar",
                table: "Customer");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnFile_RentCar_ReturnFileID",
                table: "ReturnFile",
                column: "ReturnFileID",
                principalTable: "RentCar",
                principalColumn: "RentCarEventID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnFile_RentCar_ReturnFileID",
                table: "ReturnFile");

            migrationBuilder.AddColumn<string>(
                name: "ReturnFileID",
                table: "RentCar",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfOverallRentedCar",
                table: "Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRentedCar",
                table: "Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RentCar_ReturnFileID",
                table: "RentCar",
                column: "ReturnFileID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentCar_ReturnFile_ReturnFileID",
                table: "RentCar",
                column: "ReturnFileID",
                principalTable: "ReturnFile",
                principalColumn: "ReturnFileID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
