# SqlBuilder - C# SQL 构建器

>老代码的救星

SqlBuilder 是一个轻量级、高效的 C# SQL 构建器，专为简化数据库操作和 SQL 语句构建而设计。它提供了流畅的 API 接口，支持参数化查询，并集成了多种 ORM 技术。

## 🏗️ 核心架构

### 主要组件

#### 1. SqlBuilder 类
- **命名空间**: `EasySql`
- **继承**: 实现 `IDisposable` 接口
- **设计模式**: 单例模式（线程安全）
- **主要职责**: SQL 语句构建、查询执行、连接管理

```csharp
// 线程安全的单例实现
private static readonly Lazy<SqlBuilder> instance = 
    new Lazy<SqlBuilder>(() => new SqlBuilder(), LazyThreadSafetyMode.PublicationOnly);

public static SqlBuilder Instance => instance.Value;
```

#### 2. SqlContainer 类
- **作用**: 保存 SQL 构建状态
- **核心属性**:
  - `SqlParameter`: 参数列表 (`List<DbParameter>`)
  - `SqlStatement`: SQL 语句 (`StringBuilder`)
  - `Condition`: 条件语句 (`StringBuilder`)
  - `OrderStatement`: 排序语句 (`StringBuilder`)
  - `SqlEnd`: SQL 结束语句 (`string`)

### 依赖项

基于 `packages.config` 分析，项目依赖以下核心组件：

- **AutoMapper 3.2.1**: 对象映射
- **Dapper 1.42**: 轻量级 ORM 操作
- **EntityFramework 6.1.1**: EDMX 集成支持
- **ComLib**: 自定义库，提供 Database 类和其他工具
- **目标框架**: .NET Framework 4.5

## ✨ 核心特性

### 1. 流畅的 API 设计
支持方法链式调用，提供直观的 SQL 构建体验：

```csharp
sqlBuilder.Select(SampleItem_Table.TableName)
          .AndEqualIntSql(SampleItem_Table.Id, 1)
          .Execute<SampleItemViewModel>();
```

### 2. 线程安全
- 使用 `LazyThreadSafetyMode.PublicationOnly` 确保单例的线程安全
- 支持多线程环境下的并发操作

### 3. 参数化查询
自动处理 SQL 参数，防止 SQL 注入攻击：

```csharp
sqlBuilder.AddPara(SampleItem_Table.Name, "test");
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
```

### 4. 多种映射方式
- **自动反射映射**: 直接映射到实体类
- **自定义映射**: 通过继承 `ModelMapper<T>` 实现
- **AutoMapper 集成**: 支持复杂对象映射

### 5. 分页支持
使用 `ROW_NUMBER() OVER` 子句实现高效分页：

```csharp
var result = sqlBuilder.ExecutePageData<SampleItemViewModel, SampleItemRowMapper>(
    out count,      // 总记录数
    0,              // 页索引
    10,             // 页大小
    SampleItem_Table.Id,        // 主键
    "*",            // 显示列
    SampleItem_Table.TableName, // 表名
    SampleItem_Table.Id,        // 排序字段
    SqlOrderType.Asc            // 排序方向
);
```

### 6. 连接管理
支持多种连接方式：
- 默认连接字符串
- 自定义连接字符串
- IDatabase 接口注入
- SqlContainer 注入

## 🚀 使用示例

### 基本查询操作

#### 1. 简单查询
```csharp
// 初始化
var sqlBuilder = new SqlBuilder();
sqlBuilder.ReSet();

// 查询单条记录
sqlBuilder.Select(SampleItem_Table.TableName);
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
var result = sqlBuilder.Execute<SampleItemViewModel>().FirstOrDefault();
```

#### 2. 指定字段查询
```csharp
sqlBuilder.ReSet();
sqlBuilder.Select(SampleItem_Table.TableName, 
    new[] { SampleItem_Table.Id, SampleItem_Table.Name });
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
var result = sqlBuilder.Execute<SampleItemViewModel>().FirstOrDefault();
```

#### 3. 标量查询
```csharp
sqlBuilder.ReSet();
sqlBuilder.Select(SampleItem_Table.TableName);
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);
var count = sqlBuilder.ExecuteScalarText();
```

### 数据修改操作

#### 1. 插入数据
```csharp
sqlBuilder.ReSet();

// 设置插入字段
sqlBuilder.AddPara(SampleItem_Table.Id, 2);
sqlBuilder.AddPara(SampleItem_Table.Name, "test");
sqlBuilder.AddPara(SampleItem_Table.A, "aaa");
sqlBuilder.AddPara(SampleItem_Table.B, "bbb");
sqlBuilder.AddPara(SampleItem_Table.C, "ccc");
sqlBuilder.AddPara(SampleItem_Table.Date, DateTime.Now);

// 执行插入
var result = sqlBuilder.ExecuteInsert(SampleItem_Table.TableName);
```

#### 2. 更新数据
```csharp
sqlBuilder.ReSet();

// 设置更新字段
sqlBuilder.AddPara(SampleItem_Table.Name, "test2");
sqlBuilder.Update(SampleItem_Table.TableName);

// 添加条件
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);

// 执行更新
var result = sqlBuilder.ExecuteScalarText();
```

#### 3. 删除数据
```csharp
sqlBuilder.ReSet();

// 添加删除条件
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 2);

// 执行删除
var result = sqlBuilder.ExecuteDelete(SampleItem_Table.TableName);
```

### 分页查询
```csharp
sqlBuilder.ReSet();

int totalCount = 0;
sqlBuilder.AndEqualIntSql(SampleItem_Table.Id, 1);

var result = sqlBuilder.ExecutePageData<SampleItemViewModel, SampleItemRowMapper>(
    out totalCount,  // 输出总记录数
    0,               // 页索引（从0开始）
    10,              // 每页记录数
    SampleItem_Table.Id,        // 主键字段
    "*",             // 查询字段
    SampleItem_Table.TableName, // 表名
    SampleItem_Table.Id,        // 排序字段
    SqlOrderType.Asc            // 排序方向
);
```

## 🔧 配置与设置

### 连接字符串配置

通过 `AppSettings` 类管理连接字符串：

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

### App.config 配置示例
```xml
<connectionStrings>
    <add name="DbConnectionString" 
         connectionString="Data Source=.;Initial Catalog=SampleDatabase;Integrated Security=True" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

## 🎯 自定义映射

### 实体类定义
```csharp
public class SampleItemViewModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string A { get; set; }
    public string B { get; set; }
    public string C { get; set; }
    public DateTime Date { get; set; }
}
```

### 自定义映射器
```csharp
public partial class SampleItemRowMapper : ModelMapper<SampleItemViewModel>
{
    public override SampleItemViewModel Map(IDataReader reader, SampleItemViewModel entity)
    {
        // 自定义映射逻辑
        entity.ID = reader.GetInt32("Id");
        entity.Name = reader.GetString("Name");
        // ... 其他字段映射
        return entity;
    }
}
```

### 表结构定义
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

## 🗄️ 数据库支持

- **SQL Server 2000 及以上版本**
- 包含示例数据库 (`SampleDatabase.mdf`)
- 支持 EDMX 集成，可自动生成表结构文件
- 兼容 Entity Framework 6.1.1

## 🔒 安全特性

1. **参数化查询**: 所有用户输入都通过参数化处理，防止 SQL 注入
2. **连接管理**: 实现 `IDisposable` 接口，确保连接资源正确释放
3. **线程安全**: 单例模式采用线程安全的延迟初始化

## 📋 最佳实践

1. **使用 using 语句**: 确保 SqlBuilder 实例正确释放
```csharp
using (var sqlBuilder = new SqlBuilder())
{
    // 数据库操作
}
```

2. **重置状态**: 每次操作前调用 `ReSet()` 方法
```csharp
sqlBuilder.ReSet();
```

3. **参数化查询**: 始终使用 `AddPara()` 方法添加参数
```csharp
sqlBuilder.AddPara("paramName", value);
```

4. **异常处理**: 适当处理 `SqlStatmentException` 和 `AppSettingReadErrorException`

## 🤝 贡献

欢迎提交 Issue 和 Pull Request 来改进 SqlBuilder。

## 📄 许可证

请查看项目根目录下的许可证文件了解详细信息。

