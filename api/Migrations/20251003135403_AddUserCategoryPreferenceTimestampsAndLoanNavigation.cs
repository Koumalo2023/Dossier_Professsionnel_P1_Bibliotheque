using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCategoryPreferenceTimestampsAndLoanNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FavoriteAuthor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FavoriteGenre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailNotifications = table.Column<bool>(type: "bit", nullable: false),
                    PushNotifications = table.Column<bool>(type: "bit", nullable: false),
                    NewBookAlerts = table.Column<bool>(type: "bit", nullable: false),
                    ReturnReminders = table.Column<bool>(type: "bit", nullable: false),
                    ReviewReminders = table.Column<bool>(type: "bit", nullable: false),
                    Theme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemsPerPage = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalBooksRead = table.Column<int>(type: "int", nullable: false),
                    TotalPagesRead = table.Column<int>(type: "int", nullable: false),
                    AverageReadingTime = table.Column<double>(type: "float", nullable: false),
                    ReadingLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCategoryPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreferenceScore = table.Column<int>(type: "int", nullable: false),
                    LastInteracted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InteractionCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCategoryPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCategoryPreferences_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCategoryPreferences_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserReadingGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TargetBooks = table.Column<int>(type: "int", nullable: false),
                    CurrentProgress = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReadingGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReadingGoals_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserReadingHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedReading = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedReading = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadingTimeMinutes = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Review = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReadingHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReadingHistories_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserReadingHistories_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserReadingHistories_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCategoryPreferences_CategoryId",
                table: "UserCategoryPreferences",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCategoryPreferences_UserProfileId_CategoryId",
                table: "UserCategoryPreferences",
                columns: new[] { "UserProfileId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserReadingGoals_UserProfileId_Title",
                table: "UserReadingGoals",
                columns: new[] { "UserProfileId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserReadingHistories_BookId",
                table: "UserReadingHistories",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReadingHistories_LoanId",
                table: "UserReadingHistories",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReadingHistories_StartedReading",
                table: "UserReadingHistories",
                column: "StartedReading");

            migrationBuilder.CreateIndex(
                name: "IX_UserReadingHistories_UserProfileId_BookId_LoanId",
                table: "UserReadingHistories",
                columns: new[] { "UserProfileId", "BookId", "LoanId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCategoryPreferences");

            migrationBuilder.DropTable(
                name: "UserReadingGoals");

            migrationBuilder.DropTable(
                name: "UserReadingHistories");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
