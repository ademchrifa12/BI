using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WideWorldImportersBI.Data.Oltp.Migrations
{
    /// <inheritdoc />
    public partial class CreateBiUsersTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Application");

            migrationBuilder.EnsureSchema(
                name: "Sales");

            migrationBuilder.EnsureSchema(
                name: "Warehouse");

            migrationBuilder.CreateTable(
                name: "BiRoles",
                schema: "Application",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiRoles", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "Sales",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BillToCustomerID = table.Column<int>(type: "int", nullable: false),
                    CustomerCategoryID = table.Column<int>(type: "int", nullable: false),
                    BuyingGroupID = table.Column<int>(type: "int", nullable: true),
                    PrimaryContactPersonID = table.Column<int>(type: "int", nullable: false),
                    AlternateContactPersonID = table.Column<int>(type: "int", nullable: true),
                    DeliveryMethodID = table.Column<int>(type: "int", nullable: false),
                    DeliveryCityID = table.Column<int>(type: "int", nullable: false),
                    PostalCityID = table.Column<int>(type: "int", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccountOpenedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StandardDiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsStatementSent = table.Column<bool>(type: "bit", nullable: false),
                    IsOnCreditHold = table.Column<bool>(type: "bit", nullable: false),
                    PaymentDays = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FaxNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DeliveryRun = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    RunPosition = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    WebsiteURL = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeliveryAddressLine1 = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DeliveryAddressLine2 = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DeliveryPostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PostalAddressLine1 = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    PostalAddressLine2 = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    PostalPostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    LastEditedBy = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerID);
                });

            migrationBuilder.CreateTable(
                name: "StockItems",
                schema: "Warehouse",
                columns: table => new
                {
                    StockItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    ColorID = table.Column<int>(type: "int", nullable: true),
                    UnitPackageID = table.Column<int>(type: "int", nullable: false),
                    OuterPackageID = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    QuantityPerOuter = table.Column<int>(type: "int", nullable: false),
                    IsChillerStock = table.Column<bool>(type: "bit", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecommendedRetailPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TypicalWeightPerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MarketingComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomFields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastEditedBy = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItems", x => x.StockItemID);
                });

            migrationBuilder.CreateTable(
                name: "BiUsers",
                schema: "Application",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiUsers", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_BiUsers_BiRoles_RoleID",
                        column: x => x.RoleID,
                        principalSchema: "Application",
                        principalTable: "BiRoles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                schema: "Sales",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    BillToCustomerID = table.Column<int>(type: "int", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    DeliveryMethodID = table.Column<int>(type: "int", nullable: false),
                    ContactPersonID = table.Column<int>(type: "int", nullable: false),
                    AccountsPersonID = table.Column<int>(type: "int", nullable: false),
                    SalespersonPersonID = table.Column<int>(type: "int", nullable: false),
                    PackedByPersonID = table.Column<int>(type: "int", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerPurchaseOrderNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsCreditNote = table.Column<bool>(type: "bit", nullable: false),
                    CreditNoteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalDryItems = table.Column<int>(type: "int", nullable: false),
                    TotalChillerItems = table.Column<int>(type: "int", nullable: false),
                    DeliveryRun = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    RunPosition = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    ReturnedDeliveryData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmedDeliveryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedReceivedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastEditedBy = table.Column<int>(type: "int", nullable: false),
                    LastEditedWhen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceID);
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalSchema: "Sales",
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "Sales",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    SalespersonPersonID = table.Column<int>(type: "int", nullable: false),
                    PickedByPersonID = table.Column<int>(type: "int", nullable: true),
                    ContactPersonID = table.Column<int>(type: "int", nullable: false),
                    BackorderOrderID = table.Column<int>(type: "int", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerPurchaseOrderNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsUndersupplyBackordered = table.Column<bool>(type: "bit", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickingCompletedWhen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditedBy = table.Column<int>(type: "int", nullable: false),
                    LastEditedWhen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalSchema: "Sales",
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLines",
                schema: "Sales",
                columns: table => new
                {
                    InvoiceLineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceID = table.Column<int>(type: "int", nullable: false),
                    StockItemID = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PackageTypeID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExtendedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastEditedBy = table.Column<int>(type: "int", nullable: false),
                    LastEditedWhen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLines", x => x.InvoiceLineID);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalSchema: "Sales",
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_StockItems_StockItemID",
                        column: x => x.StockItemID,
                        principalSchema: "Warehouse",
                        principalTable: "StockItems",
                        principalColumn: "StockItemID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "Application",
                table: "BiRoles",
                columns: new[] { "RoleID", "CreatedAt", "Description", "RoleName" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 1, 19, 52, 8, 172, DateTimeKind.Utc).AddTicks(4749), "Administrator with full access", "Admin" },
                    { 2, new DateTime(2026, 2, 1, 19, 52, 8, 172, DateTimeKind.Utc).AddTicks(4750), "Standard user with read access", "User" }
                });

            migrationBuilder.InsertData(
                schema: "Application",
                table: "BiUsers",
                columns: new[] { "UserID", "CreatedAt", "Email", "FirstName", "IsActive", "LastLoginAt", "LastName", "PasswordHash", "RoleID", "Username" },
                values: new object[] { 1, new DateTime(2026, 2, 1, 19, 52, 8, 172, DateTimeKind.Utc).AddTicks(4898), "admin@wideworldimporters.com", "System", true, null, "Administrator", "$2a$11$rBLRG5K7GDlpGYGg8YJYa.KMG7X3TGjGC2l5QKGM2VE8jK9UhG9Hy", 1, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_BiRoles_RoleName",
                schema: "Application",
                table: "BiRoles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiUsers_Email",
                schema: "Application",
                table: "BiUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiUsers_RoleID",
                schema: "Application",
                table: "BiUsers",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_BiUsers_Username",
                schema: "Application",
                table: "BiUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_InvoiceID",
                schema: "Sales",
                table: "InvoiceLines",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_StockItemID",
                schema: "Sales",
                table: "InvoiceLines",
                column: "StockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerID",
                schema: "Sales",
                table: "Invoices",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerID",
                schema: "Sales",
                table: "Orders",
                column: "CustomerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BiUsers",
                schema: "Application");

            migrationBuilder.DropTable(
                name: "InvoiceLines",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "BiRoles",
                schema: "Application");

            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "StockItems",
                schema: "Warehouse");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "Sales");
        }
    }
}
