# Project Context and Architecture 
**Project Name:** Metropolitan Stock Exchange (MSE) - Enterprise Web Application
**Location:** `d:\Projects\My Project\DotnetCore\MSE.StockExchange\`

## Overview
This is a secure, enterprise-grade ASP.NET Core MVC web application designed to handle Stock Exchange operations, secure authentication, and complex user workflows.

> [!IMPORTANT]
> **AI Agent Instructions:** Always check this document to understand the project architecture, dependencies, and rules. When adding new functionality, strict adherence to these established patterns is required to ensure stability and maintainability.

## Technology Stack

### Backend
- **Framework:** .NET 9.0 (`net9.0`) - ASP.NET Core MVC
- **Data Access:** Dapper (Micro-ORM) via `System.Data.SqlClient` mapping to SQL Server. Entity Framework is intentionally NOT used.
- **Dependency Injection:** Native ASP.NET Core DI Container. Services use Scoped (`AddScoped`), Singleton (`AddSingleton`), etc.
- **Authentication:** Custom Cookie-Based Authentication with Role-Based Access Control (RBAC). 
- **Security:** 
  - `BCrypt.Net-Next` for robust server-side password hashing.
  - End-to-end client-side encryption (SHA-256 via CryptoJS) so plaintext passwords do not travel over the network.
  - Rate limiting / Lockout mechanism (e.g., locking out after 5 consecutive failed attempts).
- **Caching:** `IMemoryCache` is utilized for fast, short-lived data storage securely, such as 6-digit OTP codes for Two-Factor Authentication (2FA) and Account Recovery.
- **Email:** `System.Net.Mail.SmtpClient` via a custom `IEmailService`. Configuration bound in `appsettings.json`.

### Frontend
- **Rendering Engine:** Razor Views (`.cshtml`).
- **Styling Framework:** Bootstrap 5 (loaded via CDN) & Vanilla CSS for deep customization. Wait to use Tailwind unless explicitly asked by the user.
- **Design Guidelines:** Deep, high-fidelity premium designs. Glassmorphism, dynamic animations, dark mode palettes, and smooth typography (`Inter` font).
- **Libraries Included:** FontAwesome 6 for iconography. `CryptoJS` for client-side auth data hashing.

## Architectural Patterns

1. **Repository Pattern:** All database queries reside in `Repositories/`. Ensure Dapper is exclusively used. Avoid complex abstract repository base classes; instead, favor distinct repositories inject dependency of `IDbConnectionFactory`.
2. **Service Layer:** Business logic resides in `Services/` (e.g., `AuthService`, `OtpService`, `EmailService`). Controllers must inject Services, not Repositories directly, mapping view models to domain entities.
3. **ViewModels:** Forms map accurately to dedicated ViewModels in `Models/ViewModels/` utilizing DataAnnotations for instant Validation.
4. **App Settings:** Configurations (`SmtpSettings`, `ConnectionStrings:DefaultConnection`) must be tightly read from `appsettings.json` natively. 

## Best Practices for Modifications
- **Dependency Tracking:** When incorporating any new functionality, first verify if an existing interface (`IMemoryCache`, `IAuthService`) accomplishes this. If an external package is required, add it directly to `MSE.StockExchange.csproj`.
- **Database Modularity:** Make sure that if standard tables are being altered, matching queries in the respective `Repository` are securely parameterized (e.g., `new { ParameterName = value }`) to prevent SQL injection.
- **Views & UI Validation:** All POST actions must include `@Html.AntiForgeryToken()` and robust `ModelState.IsValid` checks. Return the model back cleanly if server-side validation hits a reject. Ensure consistent branding and responsive UI rules apply in newly created views.
