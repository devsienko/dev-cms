using Microsoft.EntityFrameworkCore.Migrations;

namespace devcms.Migrations
{
    public partial class AddAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Users (Email ,Password ,PasswordSalt ,RegistrationStatus, EmailStatus, FailedLoginAttemptCounter) VALUES ('admin', 'Z7xPZJTIAyFxtpSDhEejpfpuFAE=', 'Lk7i3Rkti0LuMZu+TnWScw==', 'Approved', 'Approved', 0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Users WHERE Email = 'admin'");
        }
    }
}
