using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtomicHabits.Migrations
{
    /// <inheritdoc />
    public partial class AdjustTokenConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "JwtKeys",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IsActive = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_JwtKeys", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Modules",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Modules", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Roles",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Roles", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ResetTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsRevoked = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Permissions",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ModuleId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Permissions", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Permissions_Modules_ModuleId",
            //            column: x => x.ModuleId,
            //            principalTable: "Modules",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Habits",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Color = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        GoalValue = table.Column<int>(type: "int", nullable: false),
            //        GoalUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        GoalFrequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        IsArchived = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Habits", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Habits_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "UserRoles",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        RoleId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserRoles", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_UserRoles_Roles_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "Roles",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_UserRoles_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RolePermissions",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        RoleId = table.Column<int>(type: "int", nullable: false),
            //        PermissionId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RolePermissions", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_RolePermissions_Permissions_PermissionId",
            //            column: x => x.PermissionId,
            //            principalTable: "Permissions",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_RolePermissions_Roles_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "Roles",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "HabitReminders",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        HabitId = table.Column<int>(type: "int", nullable: false),
            //        ReminderTime = table.Column<TimeSpan>(type: "time", nullable: false),
            //        DaysOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        IsEnabled = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_HabitReminders", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_HabitReminders_Habits_HabitId",
            //            column: x => x.HabitId,
            //            principalTable: "Habits",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "HabitTrackings",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        HabitId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        TrackingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsCompleted = table.Column<bool>(type: "bit", nullable: false),
            //        Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        TimeSpentMinutes = table.Column<int>(type: "int", nullable: true),
            //        CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_HabitTrackings", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_HabitTrackings_Habits_HabitId",
            //            column: x => x.HabitId,
            //            principalTable: "Habits",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_HabitTrackings_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Streaks",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        HabitId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        CurrentStreak = table.Column<int>(type: "int", nullable: false),
            //        CurrentStreakStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        BestStreak = table.Column<int>(type: "int", nullable: false),
            //        BestStreakStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        BestStreakEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        TotalCompletions = table.Column<int>(type: "int", nullable: false),
            //        TotalDaysTracked = table.Column<int>(type: "int", nullable: false),
            //        CompletionRate = table.Column<float>(type: "real", nullable: false),
            //        LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Streaks", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Streaks_Habits_HabitId",
            //            column: x => x.HabitId,
            //            principalTable: "Habits",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Streaks_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_HabitReminders_HabitId",
            //    table: "HabitReminders",
            //    column: "HabitId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Habits_UserId",
            //    table: "Habits",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_HabitTrackings_HabitId",
            //    table: "HabitTrackings",
            //    column: "HabitId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_HabitTrackings_UserId",
            //    table: "HabitTrackings",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Permissions_ModuleId",
            //    table: "Permissions",
            //    column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RolePermissions_PermissionId",
            //    table: "RolePermissions",
            //    column: "PermissionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RolePermissions_RoleId",
            //    table: "RolePermissions",
            //    column: "RoleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Streaks_HabitId",
            //    table: "Streaks",
            //    column: "HabitId",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Streaks_UserId",
            //    table: "Streaks",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserRoles_RoleId",
            //    table: "UserRoles",
            //    column: "RoleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserRoles_UserId",
            //    table: "UserRoles",
            //    column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HabitReminders");

            migrationBuilder.DropTable(
                name: "HabitTrackings");

            migrationBuilder.DropTable(
                name: "JwtKeys");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Streaks");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Habits");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
