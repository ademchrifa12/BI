using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WideWorldImportersBI;
using WideWorldImportersBI.Data.DataWarehouse;
using WideWorldImportersBI.Data.Oltp;
using WideWorldImportersBI.Repositories.Implementations;
using WideWorldImportersBI.Repositories.Interfaces;
using WideWorldImportersBI.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// DATABASE CONFIGURATION
// =============================================================================
// Configure TWO DbContexts as required:
// 1. OltpDbContext - Connects to WideWorldImporters OLTP database
// 2. DwDbContext - Connects to Data Warehouse database (for future ETL)

builder.Services.AddDbContext<OltpDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OltpConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(3)
    ));

builder.Services.AddDbContext<DwDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DwConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(3)
    ));

// =============================================================================
// REPOSITORY PATTERN CONFIGURATION
// =============================================================================
// Register Unit of Work and all repositories
// Controllers → Services → Unit of Work → Repositories → DbContext

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// =============================================================================
// SERVICES CONFIGURATION
// =============================================================================
// Register business logic services

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IDwAnalyticsService, DwAnalyticsService>();

// =============================================================================
// FIREBASE INITIALIZATION
// =============================================================================

var firebaseCredPath = Path.Combine(AppContext.BaseDirectory, "firebase-service-account.json");
if (File.Exists(firebaseCredPath))
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(firebaseCredPath),
        ProjectId = builder.Configuration["Firebase:ProjectId"]
    });
}
else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
{
    FirebaseApp.Create(new AppOptions
    {
        ProjectId = builder.Configuration["Firebase:ProjectId"]
    });
}
else
{
    // No Firebase credentials available — Firebase Admin SDK features (user creation/deletion) won't work.
    // The app will still start, and Firebase Auth token validation via JwtBearer middleware will work
    // as long as the frontend sends valid Firebase ID tokens.
    Console.WriteLine("WARNING: No Firebase service account found. Firebase Admin features disabled.");
}

// =============================================================================
// JWT AUTHENTICATION CONFIGURATION
// =============================================================================
// Backend generates its own JWT after Firebase login verification.
// This JWT contains role claims and is validated locally (no external key fetch needed).

var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTAuthenticationMustBeAtLeast32CharactersLong!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "WideWorldImportersBI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "WideWorldImportersBI.Client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    // Define policies for role-based access
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserAccess", policy => policy.RequireRole("Admin", "User"));
});

// =============================================================================
// CORS CONFIGURATION
// =============================================================================
// Allow Angular frontend to access the API

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "https://localhost:4200"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// =============================================================================
// API CONTROLLERS
// =============================================================================

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// =============================================================================
// SWAGGER/OPENAPI CONFIGURATION
// =============================================================================
// Configure Swagger with JWT authentication support

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WideWorldImporters BI Platform API",
        Version = "v1",
        Description = @"
Enterprise Business Intelligence Platform API for WideWorldImporters.

## Features
- **OLTP Data Access**: Access real WideWorldImporters database tables
- **Analytics APIs**: Sales by period, product, customer, and KPIs
- **JWT Authentication**: Secure API access with role-based authorization
- **Data Warehouse Ready**: Architecture prepared for ETL integration

## Authentication
All endpoints (except login/register) require JWT Bearer authentication.
Use the Authorize button to add your token.

## Roles
- **Admin**: Full access to all operations (CRUD)
- **User**: Read-only access to data

## Database Architecture
- **OLTP**: WideWorldImporters (Sales, Warehouse schemas)
- **DW**: Data Warehouse (DW schema - for ETL integration)
",
        Contact = new OpenApiContact
        {
            Name = "BI Platform Support",
            Email = "support@wideworldimporters.com"
        }
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme.
Enter 'Bearer' [space] and then your token in the text input below.
Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// =============================================================================
// LOGGING CONFIGURATION
// =============================================================================

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// =============================================================================
// MIDDLEWARE PIPELINE
// =============================================================================

// Development environment configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WideWorldImporters BI API v1");
        options.RoutePrefix = string.Empty; // Swagger at root
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

// Global exception handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = "An unexpected error occurred. Please try again later."
        });
    });
});

app.UseHttpsRedirection();

// CORS must come before authentication
app.UseCors("AllowAngular");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// =============================================================================
// DATABASE INITIALIZATION
// =============================================================================
// Ensure database is created and seed data exists

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var oltpContext = scope.ServiceProvider.GetRequiredService<OltpDbContext>();
        var dwContext = scope.ServiceProvider.GetRequiredService<DwDbContext>();
        
        logger.LogInformation("Checking database connections...");
        
        if (await oltpContext.Database.CanConnectAsync())
        {
            logger.LogInformation("OLTP database connection successful");
            
            try
            {
                // Ensure Application schema exists
                await oltpContext.Database.ExecuteSqlRawAsync("IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Application') EXEC('CREATE SCHEMA [Application]')");
                
                // Create BiRoles table
                await oltpContext.Database.ExecuteSqlRawAsync(
                    @"IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id 
                      WHERE s.name = 'Application' AND t.name = 'BiRoles')
                      CREATE TABLE [Application].[BiRoles] (
                          [RoleID] INT IDENTITY(1,1) PRIMARY KEY,
                          [RoleName] NVARCHAR(50) NOT NULL UNIQUE,
                          [Description] NVARCHAR(500),
                          [CreatedAt] DATETIME2 NOT NULL
                      )"
                );
                
                // Create BiUsers table
                await oltpContext.Database.ExecuteSqlRawAsync(
                    @"IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id 
                      WHERE s.name = 'Application' AND t.name = 'BiUsers')
                      CREATE TABLE [Application].[BiUsers] (
                          [UserID] INT IDENTITY(1,1) PRIMARY KEY,
                          [Username] NVARCHAR(100) NOT NULL UNIQUE,
                          [Email] NVARCHAR(256) NOT NULL UNIQUE,
                          [PasswordHash] NVARCHAR(500) NOT NULL,
                          [FirstName] NVARCHAR(100) NOT NULL,
                          [LastName] NVARCHAR(100) NOT NULL,
                          [IsActive] BIT NOT NULL DEFAULT 1,
                          [FirebaseUid] NVARCHAR(128) NULL,
                          [RoleID] INT NOT NULL,
                          [CreatedAt] DATETIME2 NOT NULL,
                          [LastLoginAt] DATETIME2,
                          CONSTRAINT [FK_BiUsers_BiRoles] FOREIGN KEY ([RoleID]) REFERENCES [Application].[BiRoles]([RoleID])
                      )"
                );
                
                // Add FirebaseUid column if not exists (migration for existing tables)
                await oltpContext.Database.ExecuteSqlRawAsync(
                    @"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Application].[BiUsers]') AND name = 'FirebaseUid')
                      ALTER TABLE [Application].[BiUsers] ADD [FirebaseUid] NVARCHAR(128) NULL"
                );
                
                logger.LogInformation("Tables created successfully");
                
                // Seed roles
                await oltpContext.Database.ExecuteSqlRawAsync(
                    @"IF NOT EXISTS (SELECT 1 FROM [Application].[BiRoles] WHERE RoleName = 'Admin')
                      INSERT INTO [Application].[BiRoles] ([RoleName], [Description], [CreatedAt])
                      VALUES ('Admin', 'Administrator with full access', GETUTCDATE())"
                );
                
                await oltpContext.Database.ExecuteSqlRawAsync(
                    @"IF NOT EXISTS (SELECT 1 FROM [Application].[BiRoles] WHERE RoleName = 'User')
                      INSERT INTO [Application].[BiRoles] ([RoleName], [Description], [CreatedAt])
                      VALUES ('User', 'Standard user with read access', GETUTCDATE())"
                );
                
                logger.LogInformation("Roles seeded successfully");
                
                // Seed admin user with dynamically generated BCrypt hash
                var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                logger.LogInformation("Generated BCrypt hash for Admin@123");
                
                await oltpContext.Database.ExecuteSqlRawAsync(
                    @"IF NOT EXISTS (SELECT 1 FROM [Application].[BiUsers] WHERE Username = 'admin')
                      BEGIN
                          DECLARE @AdminRoleId INT = (SELECT TOP 1 [RoleID] FROM [Application].[BiRoles] WHERE [RoleName] = 'Admin')
                          INSERT INTO [Application].[BiUsers] ([Username], [Email], [PasswordHash], [FirstName], [LastName], [IsActive], [RoleID], [CreatedAt])
                          VALUES ('admin', 'admin@wideworldimporters.com', {0}, 'System', 'Administrator', 1, @AdminRoleId, GETUTCDATE())
                      END",
                    adminPasswordHash
                );
                
                logger.LogInformation("Admin user seeded successfully");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Warning during table setup (may already exist): {Message}", ex.Message);
            }
        }
        else
        {
            logger.LogWarning("Cannot connect to OLTP database. Ensure WideWorldImporters is restored.");
        }
        
        // Check DW connection
        if (await dwContext.Database.CanConnectAsync())
        {
            logger.LogInformation("Data Warehouse database connection successful");
        }
        else
        {
            logger.LogInformation("Data Warehouse database not available. ETL will create it when ready.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during database initialization");
    }
}

app.Run();
