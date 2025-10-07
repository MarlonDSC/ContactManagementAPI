using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fund_Contacts_Fix_FK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FundContacts_ContactId",
                table: "FundContacts");

            migrationBuilder.CreateIndex(
                name: "IX_FundContacts_ContactId_FundId",
                table: "FundContacts",
                columns: new[] { "ContactId", "FundId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FundContacts_ContactId_FundId",
                table: "FundContacts");

            migrationBuilder.CreateIndex(
                name: "IX_FundContacts_ContactId",
                table: "FundContacts",
                column: "ContactId");
        }
    }
}
