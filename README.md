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

## 📚 Usage Examples

### Comprehensive Test Reference

The `SqlBuilderTest/SqlBuilderUnitTest.cs` file provides extensive examples of SqlBuilder usage patterns. Here are the key test methods and their implementations:

#### Test Initialization Pattern
```csharp
[TestInitialize]
public void TestInitial()
{
    sqlbuilder = new SqlBuilder(AppSettings.GetConnectionString("SampleDatabaseEntities"));
    sqlbuilder.ReSet(); // Always reset state before operations
}

[TestCleanup]
public void TestCleanup()
{
    sqlbuilder.Dispose(); // Proper resource disposal
}
```

#### ReSet() Method Usage
Essential for clearing previous state before each operation:
```csharp
sqlbuilder.ReSet(); // Clear previous SQL state
// Proceed with new SQL operation
```

#### Select() Method Examples
```csharp
// Select all fields with custom mapping
[TestMethod]
public void SelectAllTest()
{
    sqlbuilder.ReSet();
    sqlbuilder.Select(SampleItem_Table.TableName);
    sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
    var result = sqlbuilder.Execute<SampleItemViewModel, SampleItemRowMapper>().FirstOrDefault();
}

// Select specific fields with auto-reflection
[TestMethod]
public void SelectTwoFieldsTest()
{
    sqlbuilder.ReSet();
    sqlbuilder.Select(SampleItem_Table.TableName, new[] { SampleItem_Table.Id, SampleItem_Table.Name });
    sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
    var result = sqlbuilder.Execute<SampleItemViewModel>().FirstOrDefault();
}
```

#### AndEqualIntSql() Method Usage
Add integer equality conditions to WHERE clause:
```csharp
sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1); // WHERE Id = 1
```

#### Execute<T>() Method Examples
Execute queries with strongly-typed results:
```csharp
// With custom mapper
var result = sqlbuilder.Execute<SampleItemViewModel, SampleItemRowMapper>();

// With auto-reflection
var result = sqlbuilder.Execute<SampleItemViewModel>();
```

#### ExecutePageData() Method Usage
Implement efficient pagination:
```csharp
[TestMethod]
public void SelectPageTest()
{
    sqlbuilder.ReSet();
    var count = 0;
    sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
    
    var result = sqlbuilder.ExecutePageData<SampleItemViewModel, SampleItemRowMapper>(
        out count,                    // Total record count
        0,                           // Page index (0-based)
        10,                          // Page size
        SampleItem_Table.Id,         // Primary key for sorting
        "*",                         // Columns to select
        SampleItem_Table.TableName,  // Table name
        SampleItem_Table.Id,         // Order by field
        SqlOrderType.Asc            // Sort direction
    );
}
```

#### ExecuteScalarText() Method Usage
Execute scalar queries returning single values:
```csharp
[TestMethod]
public void SelectCountTest()
{
    sqlbuilder.ReSet();
    sqlbuilder.Select(SampleItem_Table.TableName, new[] { "count(*)" });
    sqlbuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
    var count = sqlbuilder.ExecuteScalarText(); // Returns count as string
}
```

### Basic Query Building

```csharp
using (var sqlBuilder = new SqlBuilder())
{
    sqlBuilder.ReSet();
    
    // Simple SELECT query
    sqlBuilder.Select("Users");
    sqlBuilder.AndEqualIntSql("Id", 1);
    
    var users = sqlBuilder.Execute<User>();
}
```

### Advanced Query Operations

```csharp
// Complex query with multiple conditions
sqlBuilder.ReSet();
sqlBuilder.Select("Products", new[] { "Id", "Name", "Price" });
sqlBuilder.AndEqualIntSql("CategoryId", 1);
sqlBuilder.AddPara("MinPrice", 100);
sqlBuilder.AppendCondition(" AND Price >= @MinPrice");

var products = sqlBuilder.Execute<Product>();
```

### Pagination Example

```csharp
int totalCount;
var pagedData = sqlBuilder.ExecutePageData<Product, ProductMapper>(
    out totalCount,
    pageIndex: 0,
    pageSize: 20,
    primaryKey: "Id",
    fields: "*",
    tableName: "Products",
    orderField: "Name",
    orderType: SqlOrderType.Asc
);
```

## 🔧 Core Code Reference

### SqlBuilder.cs Architecture Overview

The `SqlBuilder/SqlBuilder.cs` file contains the core implementation with the following key architectural components:

#### Singleton Pattern Implementation
```csharp
// Thread-safe singleton with lazy initialization
private static readonly Lazy<SqlBuilder> instance =
    new Lazy<SqlBuilder>(
        delegate { return new SqlBuilder(); },
        LazyThreadSafetyMode.PublicationOnly  // Thread safety first
    );

public static SqlBuilder Instance
{
    get { return instance.Value; }
}
```

#### Thread Safety Features
- Uses `LazyThreadSafetyMode.PublicationOnly` for thread-safe singleton initialization
- Supports concurrent operations in multi-threaded environments
- Thread-safe lazy instantiation prevents race conditions

#### IDisposable Implementation
```csharp
public class SqlBuilder : IDisposable
{
    public void Dispose()
    {
        var conn = this.Db.GetConnection();
        if (conn != null)
        {
            conn.Close();
            conn.Dispose();
        }
        this.Db = null;
    }
}
```

#### Main API Methods

**Core Query Methods:**
- `Select(string tableName)` - Initialize SELECT statement
- `Select(string tableName, string[] fields)` - SELECT with specific fields
- `Update(string tableName)` - Initialize UPDATE statement
- `ExecuteInsert(string tableName)` - Execute INSERT operation
- `ExecuteDelete(string tableName)` - Execute DELETE operation

**Condition Methods:**
- `AndEqualIntSql(string fieldName, int value)` - Add integer equality condition
- `AddPara(string name, object value)` - Add parameterized value

**Execution Methods:**
- `Execute<T>()` - Execute with auto-reflection mapping
- `Execute<T, TMapper>()` - Execute with custom mapper
- `ExecutePageData<T, TMapper>()` - Execute with pagination
- `ExecuteScalarText()` - Execute scalar query
- `ExecuteNonQueryText()` - Execute non-query statement

## 🗄️ SQL Server 2000+ Support

SqlBuilder provides comprehensive support for SQL Server 2000 and all subsequent versions, ensuring compatibility across a wide range of database environments.

### Version Compatibility

- **SQL Server 2000**: Full compatibility with legacy systems
- **SQL Server 2005**: Enhanced features support including ROW_NUMBER() for pagination
- **SQL Server 2008/2008 R2**: Advanced data types and features
- **SQL Server 2012+**: Modern SQL Server features and optimizations
- **SQL Server Express/LocalDB**: Development and lightweight deployment support

### Database Features Support

#### Pagination Implementation
Uses SQL Server's ROW_NUMBER() window function for efficient pagination:

```csharp
// Generated pagination SQL for SQL Server 2005+
WITH OrderedRows As
(
    SELECT tbltbl.*, ROW_NUMBER() OVER (Order By tbltbl.Id ASC) as RowNum 
    FROM (SELECT * FROM SampleItem WHERE 1=1) as tbltbl
)
SELECT * FROM OrderedRows Where RowNum > @startRowIndex 
Order By Id ASC
```

#### Parameterized Query Support
- Full support for SQL Server parameterized queries
- Automatic parameter type detection and conversion
- SQL injection prevention through proper parameterization

#### Connection Management
- Supports SQL Server connection strings
- Compatible with Entity Framework connection strings
- LocalDB and SQL Server Express support for development

#### Sample Database Integration
- Includes `SampleDatabase.mdf` for immediate testing
- Pre-configured connection strings for quick setup
- Entity Framework EDMX integration for table structure generation

#### Advanced SQL Server Features
- Support for multiple active result sets (MARS)
- Transaction support through ComLib.Data.Database
- Bulk operations and batch processing capabilities
- Custom data type handling and conversion

## 🏢 Core Architecture

### Main Components

#### 1. SqlBuilder Class
- **Namespace**: `EasySql`
- **Inheritance**: Implements `IDisposable` interface
- **Design Pattern**: Singleton pattern (thread-safe)
- **Main Responsibilities**: SQL statement building, query execution, connection management

#### 2. SqlContainer Class
- **Purpose**: Maintains SQL building state
- **Core Properties**:
  - `SqlParameter`: Parameter list (`List<DbParameter>`)
  - `SqlStatement`: SQL statement (`StringBuilder`)
  - `Condition`: Condition statement (`StringBuilder`)
  - `OrderStatement`: Order statement (`StringBuilder`)
  - `SqlEnd`: SQL end statement (`string`)

### Dependencies

Based on `packages.config` analysis, the project depends on the following core components:

- **AutoMapper 3.2.1**: Object mapping
- **Dapper 1.42**: Lightweight ORM operations
- **EntityFramework 6.1.1**: EDMX integration support
- **ComLib**: Custom library providing Database class and other utilities
- **Target Framework**: .NET Framework 4.5

## ✨ Core Features

### 1. Fluent API Design
Supports method chaining for intuitive SQL building experience:

```csharp
sqlBuilder.Select(SampleItem_Table.TableName)
          .AndEqualIntSql(SampleItem_Table.Id, 1)
          .Execute<SampleItemViewModel>();
```

### 2. Thread Safety
- Uses `LazyThreadSafetyMode.PublicationOnly` to ensure thread-safe singleton
- Supports concurrent operations in multi-threaded environments

### 3. Parameterized Queries
Automatically handles SQL parameters to prevent SQL injection attacks:

```csharp
sqlBuilder.AddPara(SampleItem_Table.Name, "test");
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
```

### 4. Multiple Mapping Options
- **Auto-reflection mapping**: Direct mapping to entity classes
- **Custom mapping**: Implemented by inheriting `ModelMapper<T>`
- **AutoMapper integration**: Supports complex object mapping

### 5. Pagination Support
Implements efficient pagination using `ROW_NUMBER() OVER` clause for SQL Server 2005+

## 🔧 Configuration

### Connection String Setup

```csharp
// Using AppSettings class
public class AppSettings
{
    public static string DbConnectionString 
    {
        get { return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; }
    }
    
    public static string GetConnectionString(string name)
    {
        return ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }
}
```

### App.config Configuration

```xml
<configuration>
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Data Source=.;Initial Catalog=YourDatabase;Integrated Security=True" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

## 🗺️ Custom Mapping

### Creating Custom Mappers

```csharp
public class SampleItemRowMapper : ModelMapper<SampleItemViewModel>
{
    public override SampleItemViewModel BuildModel(IDataReader reader, int rowNumber)
    {
        SampleItemViewModel entity = new SampleItemViewModel();
        entity.ID = reader[SampleItem_Table.Id] == DBNull.Value ? 0 : (int)reader[SampleItem_Table.Id];
        entity.Name = reader[SampleItem_Table.Name] == DBNull.Value ? string.Empty : reader[SampleItem_Table.Name].ToString();
        entity.Abc = reader[SampleItem_Table.A].ToString() + "  " + reader[SampleItem_Table.B].ToString() + " (" + reader[SampleItem_Table.C].ToString() + ")";
        entity.Date = reader[SampleItem_Table.Date] == DBNull.Value ? DateTime.MinValue : (DateTime)reader[SampleItem_Table.Date];
        return entity;
    }
}
```

### Table Structure Definition
```csharp
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
```

## 🔒 Security Features

1. **Parameterized Queries**: All user input is processed through parameterization to prevent SQL injection
2. **Connection Management**: Implements `IDisposable` interface to ensure proper connection resource disposal
3. **Thread Safety**: Singleton pattern uses thread-safe lazy initialization

## 📋 Best Practices

1. **Use using statements**: Ensure SqlBuilder instances are properly disposed
```csharp
using (var sqlBuilder = new SqlBuilder())
{
    // Database operations
}
```

2. **Reset state**: Call `ReSet()` method before each operation
```csharp
sqlBuilder.ReSet();
```

3. **Parameterized queries**: Always use `AddPara()` method to add parameters
```csharp
sqlBuilder.AddPara("paramName", value);
```

4. **Exception handling**: Properly handle `SqlStatmentException` and `AppSettingReadErrorException`

5. **Connection string management**: Use `AppSettings` class for centralized configuration

6. **Custom mapping**: Inherit from `ModelMapper<T>` for complex mapping scenarios

7. **Pagination**: Use `ExecutePageData()` for efficient large dataset handling

## 🤝 Contributing

Issues and Pull Requests are welcome to improve SqlBuilder.

## 📄 License

Please refer to the license file in the project root directory for detailed information.
