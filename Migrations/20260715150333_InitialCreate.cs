using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricoTemp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    TemperatureUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApparentTemperature = table.Column<double>(type: "float", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    HumidityUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WindSpeed = table.Column<double>(type: "float", nullable: false),
                    WindSpeedUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeatherCode = table.Column<int>(type: "int", nullable: false),
                    WeatherTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsultedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoTemp", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoTemp");
        }
    }
}
