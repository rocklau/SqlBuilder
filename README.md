# SqlBuilder - C# SQL Builder

SqlBuilder is a lightweight, efficient C# SQL builder designed to simplify database operations and SQL statement construction. It provides a fluent API interface, supports parameterized queries, and integrates with multiple ORM technologies.

## 🏗️ Core Architecture

### Main Components

#### 1. SqlBuilder Class
- **Namespace**: `EasySql`
- **Inheritance**: Implements `IDisposable` interface
- **Design Pattern**: Singleton pattern (thread-safe)
- **Main Responsibilities**: SQL statement building, query execution, connection management

```csharp
// Thread-safe singleton implementation
private static readonly Lazy<SqlBuilder> instance = 
    new Lazy<SqlBuilder>(() => new SqlBuilder(), LazyThreadSafetyMode.PublicationOnly);

public static SqlBuilder Instance => instance.Value;
```

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
Implements efficient pagination using `ROW_NUMBER() OVER` clause:

```csharp
var result = sqlBuilder.ExecutePageData<SampleItemViewModel, SampleItemRowMapper>(
    out count,      // Total record count
    0,              // Page index
    10,             // Page size
    SampleItem_Table.Id,        // Primary key
    "*",            // Display columns
    SampleItem_Table.TableName, // Table name
    SampleItem_Table.Id,        // Sort field
    SqlOrderType.Asc            // Sort direction
);
```

### 6. Connection Management
Supports multiple connection methods:
- Default connection string
- Custom connection string
- IDatabase interface injection
- SqlContainer injection

## 🚀 Usage Examples

### Basic Query Operations

#### 1. Simple Query
```csharp
// Initialize
var sqlBuilder = new SqlBuilder();
sqlBuilder.ReSet();

// Query single record
sqlBuilder.Select(SampleItem_Table.TableName);
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
var result = sqlBuilder.Execute<SampleItemViewModel>().FirstOrDefault();
```

#### 2. Specific Field Query
```csharp
sqlBuilder.ReSet();
sqlBuilder.Select(SampleItem_Table.TableName, 
    new[] { SampleItem_Table.Id, SampleItem_Table.Name });
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
var result = sqlBuilder.Execute<SampleItemViewModel>().FirstOrDefault();
```

#### 3. Scalar Query
```csharp
sqlBuilder.ReSet();
sqlBuilder.Select(SampleItem_Table.TableName);
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
var count = sqlBuilder.ExecuteScalarText();
```

### Data Modification Operations

#### 1. Insert Operation
```csharp
sqlBuilder.ReSet();

// Set insert fields
sqlBuilder.AddPara(SampleItem_Table.Id, 2);
sqlBuilder.AddPara(SampleItem_Table.Name, "test");
sqlBuilder.AddPara(SampleItem_Table.A, "aaa");
sqlBuilder.AddPara(SampleItem_Table.B, "bbb");
sqlBuilder.AddPara(SampleItem_Table.C, "ccc");
sqlBuilder.AddPara(SampleItem_Table.Date, DateTime.Now);

// Execute insert
var result = sqlBuilder.ExecuteInsert(SampleItem_Table.TableName);
```

#### 2. Update Operation
```csharp
sqlBuilder.ReSet();

// Add set fields
sqlBuilder.AddPara(SampleItem_Table.Name, "test2");
sqlBuilder.Update(SampleItem_Table.TableName);

// Add where condition
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);

// Execute update
var result = sqlBuilder.ExecuteScalarText();
```

#### 3. Delete Operation
```csharp
sqlBuilder.ReSet();

// Add where condition
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 2);

// Execute delete
var result = sqlBuilder.ExecuteDelete(SampleItem_Table.TableName);
```

### Pagination Query
```csharp
sqlBuilder.ReSet();
int count;

// Add where condition
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);

// Execute pagination query
var result = sqlBuilder.ExecutePageData<SampleItemViewModel, SampleItemRowMapper>(
    out count,           // Total records
    0,                   // Page index
    10,                  // Page size
    SampleItem_Table.Id, // Primary key field
    "*",                 // Query fields
    SampleItem_Table.TableName, // Table name
    SampleItem_Table.Id,        // Sort field
    SqlOrderType.Asc            // Sort direction
);
```

## 🔧 Configuration & Settings

### Connection String Configuration

Manage connection strings through the `AppSettings` class:

```csharp
public static class AppSettings
{
    public static string DbConnectionString => GetConnectionString("DbConnectionString");
    
    public static string GetConnectionString(string name)
    {
        return System.Configuration.ConfigurationManager
            .ConnectionStrings[name].ConnectionString;
    }
}
```

### App.config Configuration Example
```xml
<connectionStrings>
    <add name="DbConnectionString" 
         connectionString="Data Source=.;Initial Catalog=SampleDatabase;Integrated Security=True" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

## 🎯 Custom Mapping

### Entity Class Definition
```csharp
public class SampleItemViewModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Abc { get; set; }
    public DateTime Date { get; set; }
}
```

### Custom Mapper
```csharp
public partial class SampleItemRowMapper : ModelMapper<SampleItemViewModel>
{
    public override SampleItemViewModel MapRow(IDataReader reader, int rowNumber)
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

## 🗄️ Database Support

- **SQL Server 2000 and above**
- Includes sample database (`SampleDatabase.mdf`)
- Supports EDMX integration for automatic table structure file generation
- Compatible with Entity Framework 6.1.1

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

