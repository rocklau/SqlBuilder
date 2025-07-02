# SqlBuilder - C# SQL Builder

SqlBuilder is a lightweight, efficient C# SQL builder designed to simplify database operations and SQL statement construction. It provides a fluent API interface, supports parameterized queries, integrates with multiple ORM technologies, and offers easy management of SQL business logic with EDMX table structure generation support.

## 🚀 Easy to Manage SQL Business Logic

SqlBuilder provides an intuitive and powerful way to manage SQL business logic through its fluent API design, making complex database operations simple and maintainable.

### Key Business Logic Features

#### 1. Fluent API for Complex Queries
Build complex SQL statements with method chaining:

```csharp
// Initialize SqlBuilder with connection string
var sqlBuilder = new SqlBuilder(AppSettings.GetConnectionString("SampleDatabaseEntities"));

// Reset state before each operation
sqlBuilder.ReSet();

// Build and execute complex query
sqlBuilder.Select(SampleItem_Table.TableName)
          .AndEqualIntSql(SampleItem_Table.Id, 1)
          .Execute<SampleItemViewModel>();
```

#### 2. Simplified CRUD Operations
Manage all database operations with consistent patterns:

```csharp
// SELECT with custom mapping
sqlBuilder.ReSet();
sqlBuilder.Select(SampleItem_Table.TableName);
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
var result = sqlBuilder.Execute<SampleItemViewModel, SampleItemRowMapper>().FirstOrDefault();

// UPDATE with parameterized values
sqlBuilder.ReSet();
sqlBuilder.AddPara(SampleItem_Table.Name, "UpdatedName");
sqlBuilder.Update(SampleItem_Table.TableName);
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
var rowsAffected = sqlBuilder.ExecuteScalarText();

// INSERT with multiple parameters
sqlBuilder.ReSet();
sqlBuilder.AddPara(SampleItem_Table.Id, 2);
sqlBuilder.AddPara(SampleItem_Table.Name, "NewItem");
sqlBuilder.AddPara(SampleItem_Table.A, "ValueA");
sqlBuilder.AddPara(SampleItem_Table.Date, DateTime.Now);
var insertResult = sqlBuilder.ExecuteInsert(SampleItem_Table.TableName);

// DELETE with conditions
sqlBuilder.ReSet();
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 2);
var deleteResult = sqlBuilder.ExecuteDelete(SampleItem_Table.TableName);
```

#### 3. Advanced Pagination Support
Handle large datasets efficiently with built-in pagination:

```csharp
sqlBuilder.ReSet();
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);

var count = 0;
var pagedResults = sqlBuilder.ExecutePageData<SampleItemViewModel, SampleItemRowMapper>(
    out count,          // Total records
    0,                  // Page index (0-based)
    10,                 // Page size
    SampleItem_Table.Id, // Primary key
    "*",                // Display columns
    SampleItem_Table.TableName, // Table name
    SampleItem_Table.Id, // Order field
    SqlOrderType.Asc    // Sort order
);
```

## 📊 EDMX Table Structure Generation

SqlBuilder seamlessly integrates with Entity Framework's EDMX files to automatically generate table structure files, providing strongly-typed database access and eliminating manual table definition work.

### Entity Framework Integration

#### 1. EDMX Configuration
The project includes Entity Framework 6.1.1 integration with EDMX support:

```xml
<!-- Connection string in App.config -->
<connectionStrings>
  <add name="SampleDatabaseEntities" 
       connectionString="metadata=res://*/Db.SampleDatabase.csdl|res://*/Db.SampleDatabase.ssdl|res://*/Db.SampleDatabase.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=|DataDirectory|\SampleDatabase.mdf;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>
```

#### 2. Auto-Generated Entity Classes
EDMX automatically generates entity classes and table structure definitions:

```csharp
// Auto-generated from EDMX: SampleItem.cs
namespace EasySql.Db
{
    public partial class SampleItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    }
    
    // Auto-generated table structure helper
    public class SampleItem_Table
    {
        public static string TableName = "SampleItem";
        public static string Id = "Id";
        public static string Name = "Name";
        public static string A = "A";
        public static string B = "B";
        public static string C = "C";
        public static string Date = "Date";
    }
}
```

#### 3. DbContext Integration
EDMX generates DbContext for Entity Framework operations:

```csharp
public partial class SampleDatabaseEntities : DbContext
{
    public SampleDatabaseEntities() : base("name=SampleDatabaseEntities") { }
    public virtual DbSet<SampleItem> SampleItems { get; set; }
}
```
