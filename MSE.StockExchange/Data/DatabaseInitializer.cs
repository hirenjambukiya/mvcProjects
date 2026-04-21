using System;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MSE.StockExchange.Models.Domain;
using BCrypt.Net;

namespace MSE.StockExchange.Data;

public class DatabaseInitializer
{
    private readonly IConfiguration _configuration;

    public DatabaseInitializer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Initialize()
    {
        var localdbConnectionString = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;";
        var defaultConnection = _configuration.GetConnectionString("DefaultConnection");

        // 1. Create database if it doesn't exist
        using (var masterConnection = new SqlConnection(localdbConnectionString))
        {
            masterConnection.Open();
            var dbCheckQuery = "SELECT database_id FROM sys.databases WHERE Name = 'MSE_AuthDb'";
            var dbId = masterConnection.ExecuteScalar<int?>(dbCheckQuery);

            if (!dbId.HasValue)
            {
                masterConnection.Execute("CREATE DATABASE MSE_AuthDb");
            }
        }

        // 2. Create tables and seed data
        using (var connection = new SqlConnection(defaultConnection))
        {
            connection.Open();

            var createRolesTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Roles' and xtype='U')
                BEGIN
                    CREATE TABLE Roles (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        RoleName NVARCHAR(50) NOT NULL UNIQUE
                    )
                END";

            var createUsersTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
                BEGIN
                    CREATE TABLE Users (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Username NVARCHAR(50) NOT NULL UNIQUE,
                        PasswordHash NVARCHAR(255) NOT NULL,
                        Email NVARCHAR(100) NOT NULL,
                        IsActive BIT DEFAULT 1,
                        FailedAttemptCount INT DEFAULT 0,
                        IsLockedOut BIT DEFAULT 0,
                        LockoutEnd DATETIME NULL,
                        CreatedAt DATETIME DEFAULT GETUTCDATE()
                    )
                END";

            var createUserRolesTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' and xtype='U')
                BEGIN
                    CREATE TABLE UserRoles (
                        UserId INT FOREIGN KEY REFERENCES Users(Id),
                        RoleId INT FOREIGN KEY REFERENCES Roles(Id),
                        PRIMARY KEY (UserId, RoleId)
                    )
                END";

            connection.Execute(createRolesTable);
            connection.Execute(createUsersTable);
            connection.Execute(createUserRolesTable);

            // Seed Roles
            var roles = new[] { "Company", "Maker", "Checker", "HOD" };
            foreach (var role in roles)
            {
                var roleExists = connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Roles WHERE RoleName = @RoleName", new { RoleName = role });
                if (roleExists == 0)
                {
                    connection.Execute("INSERT INTO Roles (RoleName) VALUES (@RoleName)", new { RoleName = role });
                }
            }

            // Seed Admin User (HOD Role) - Password: "Password123" (then client-side SHA256 "Password123" before BCrypt for true match, but let's assume raw string here for seeding and we will SHA256 it client side. Note: The client side hashes "Password123" using SHA256 then sends it, we must match the hashed variant if we want to seed a user).
            // Let's seed a user whose client-encrypted password is a known SHA256 and then BCrypted.
            // SHA256 of "Password123" is "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f"
            var prehashedPassword = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f";
            var bcryptedPassword = BCrypt.Net.BCrypt.HashPassword(prehashedPassword);

            var userExists = connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Users WHERE Username = 'admin'");
            if (userExists == 0)
            {
                var adminId = connection.QuerySingle<int>(
                    "INSERT INTO Users (Username, PasswordHash, Email, IsActive) VALUES (@Username, @PasswordHash, @Email, 1); SELECT SCOPE_IDENTITY();",
                    new { Username = "admin", PasswordHash = bcryptedPassword, Email = "admin@mse.in" }
                );

                var hodRoleId = connection.ExecuteScalar<int>("SELECT Id FROM Roles WHERE RoleName = 'HOD'");
                connection.Execute("INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)", new { UserId = adminId, RoleId = hodRoleId });
            }
            
            // Seed Company user
            var userExistsCompany = connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Users WHERE Username = 'company1'");
            if (userExistsCompany == 0)
            {
                var companyId = connection.QuerySingle<int>(
                    "INSERT INTO Users (Username, PasswordHash, Email, IsActive) VALUES (@Username, @PasswordHash, @Email, 1); SELECT SCOPE_IDENTITY();",
                    new { Username = "company1", PasswordHash = bcryptedPassword, Email = "company@mse.in" }
                );

                var companyRoleId = connection.ExecuteScalar<int>("SELECT Id FROM Roles WHERE RoleName = 'Company'");
                connection.Execute("INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)", new { UserId = companyId, RoleId = companyRoleId });
            }
        }
    }
}
