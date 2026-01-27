using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Area27.Hub.Helpers;

/// <summary>
/// Provides utilities for generating SQL CREATE TABLE statements from .NET objects.
/// </summary>
public static class SqlTableGenerator
{
    /// <summary>
    /// Supported SQL database systems.
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>PostgreSQL</summary>
        PostgreSql,
        /// <summary>MySQL</summary>
        MySql,
        /// <summary>SQL Server</summary>
        SqlServer,
        /// <summary>SQLite</summary>
        SQLite
    }

    /// <summary>
    /// Generates a CREATE TABLE statement from an object type.
    /// </summary>
    /// <param name="type">The type to generate table structure from.</param>
    /// <param name="databaseType">The target database system.</param>
    /// <param name="tableName">Optional table name (uses class name if not provided).</param>
    /// <returns>SQL CREATE TABLE statement.</returns>
    public static string GenerateCreateTable(Type type, DatabaseType databaseType, string? tableName = null)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        tableName = tableName ?? type.Name;
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        if (properties.Length == 0)
            throw new ArgumentException($"Type {type.Name} has no public properties.", nameof(type));

        var columns = new List<string>();

        foreach (var property in properties)
        {
            var columnDef = GenerateColumnDefinition(property, databaseType);
            columns.Add(columnDef);
        }

        var columnList = string.Join(",\r\n    ", columns);
        var sql = $"CREATE TABLE {EscapeTableName(tableName, databaseType)} (\r\n    {columnList}\r\n);";

        return sql;
    }

    /// <summary>
    /// Generates CREATE TABLE statements for multiple objects.
    /// </summary>
    /// <param name="types">The types to generate table structures from.</param>
    /// <param name="databaseType">The target database system.</param>
    /// <returns>SQL CREATE TABLE statements separated by newlines.</returns>
    public static string GenerateCreateTables(Type[] types, DatabaseType databaseType)
    {
        if (types == null || types.Length == 0)
            throw new ArgumentException("At least one type must be provided.", nameof(types));

        var statements = types
            .Select(t => GenerateCreateTable(t, databaseType))
            .ToArray();

        return string.Join("\r\n\r\n", statements);
    }

    /// <summary>
    /// Generates a CREATE TABLE statement from a generic object instance.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="databaseType">The target database system.</param>
    /// <param name="tableName">Optional table name (uses class name if not provided).</param>
    /// <returns>SQL CREATE TABLE statement.</returns>
    public static string GenerateCreateTable<T>(DatabaseType databaseType, string? tableName = null)
    {
        return GenerateCreateTable(typeof(T), databaseType, tableName);
    }

    // ============ Private Methods ============

    private static string GenerateColumnDefinition(PropertyInfo property, DatabaseType databaseType)
    {
        var columnName = EscapeColumnName(property.Name, databaseType);
        var columnType = MapDotNetTypeToDatabaseType(property.PropertyType, databaseType);
        var isNullable = property.PropertyType.IsGenericType &&
                         property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ||
                         !property.PropertyType.IsValueType;

        var nullability = isNullable && property.PropertyType != typeof(string) ? "NULL" : "NOT NULL";

        // Add PRIMARY KEY for Id properties
        var isPrimaryKey = property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                          property.Name.Equals($"{property.DeclaringType?.Name}Id", StringComparison.OrdinalIgnoreCase);

        var constraint = isPrimaryKey ? " PRIMARY KEY" : string.Empty;

        // Add AUTO_INCREMENT or equivalent for Id
        if (isPrimaryKey)
        {
            switch (databaseType)
            {
                case DatabaseType.MySql:
                    return $"{columnName} {columnType} AUTO_INCREMENT PRIMARY KEY";
                case DatabaseType.PostgreSql:
                    return $"{columnName} SERIAL PRIMARY KEY";
                case DatabaseType.SqlServer:
                    return $"{columnName} {columnType} IDENTITY(1,1) PRIMARY KEY";
                case DatabaseType.SQLite:
                    return $"{columnName} {columnType} PRIMARY KEY AUTOINCREMENT";
            }
        }

        return $"{columnName} {columnType} {nullability}";
    }

    private static string MapDotNetTypeToDatabaseType(Type type, DatabaseType databaseType)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        // Handle collections and objects as JSON
        if (IsComplexType(underlyingType))
        {
            return databaseType switch
            {
                DatabaseType.PostgreSql => "JSONB",
                DatabaseType.MySql => "JSON",
                DatabaseType.SqlServer => "NVARCHAR(MAX)",
                DatabaseType.SQLite => "TEXT",
                _ => "TEXT"
            };
        }

        if (underlyingType == typeof(int))
            return "INT";
        if (underlyingType == typeof(long))
            return "BIGINT";
        if (underlyingType == typeof(short))
            return "SMALLINT";
        if (underlyingType == typeof(byte))
            return "TINYINT";
        if (underlyingType == typeof(decimal))
            return databaseType == DatabaseType.MySql ? "DECIMAL(18,2)" : "DECIMAL(18,2)";
        if (underlyingType == typeof(double))
            return "DOUBLE";
        if (underlyingType == typeof(float))
            return "FLOAT";
        if (underlyingType == typeof(bool))
            return databaseType switch
            {
                DatabaseType.PostgreSql => "BOOLEAN",
                DatabaseType.MySql => "BOOLEAN",
                DatabaseType.SqlServer => "BIT",
                DatabaseType.SQLite => "INTEGER",
                _ => "BOOLEAN"
            };
        if (underlyingType == typeof(DateTime))
            return databaseType switch
            {
                DatabaseType.MySql => "DATETIME",
                DatabaseType.PostgreSql => "TIMESTAMP",
                DatabaseType.SqlServer => "DATETIME2",
                DatabaseType.SQLite => "DATETIME",
                _ => "DATETIME"
            };
        if (underlyingType == typeof(DateOnly))
            return "DATE";
        if (underlyingType == typeof(TimeOnly))
            return databaseType switch
            {
                DatabaseType.MySql => "TIME",
                DatabaseType.PostgreSql => "TIME",
                DatabaseType.SqlServer => "TIME",
                DatabaseType.SQLite => "TIME",
                _ => "TIME"
            };
        if (underlyingType == typeof(Guid))
            return databaseType switch
            {
                DatabaseType.PostgreSql => "UUID",
                DatabaseType.MySql => "CHAR(36)",
                DatabaseType.SqlServer => "UNIQUEIDENTIFIER",
                DatabaseType.SQLite => "TEXT",
                _ => "CHAR(36)"
            };
        if (underlyingType == typeof(byte[]))
            return databaseType switch
            {
                DatabaseType.PostgreSql => "BYTEA",
                DatabaseType.MySql => "LONGBLOB",
                DatabaseType.SqlServer => "VARBINARY(MAX)",
                DatabaseType.SQLite => "BLOB",
                _ => "BLOB"
            };

        // Default to string/text for unknown types
        return databaseType switch
        {
            DatabaseType.SqlServer => "NVARCHAR(MAX)",
            DatabaseType.MySql => "VARCHAR(255)",
            DatabaseType.PostgreSql => "VARCHAR(255)",
            DatabaseType.SQLite => "TEXT",
            _ => "TEXT"
        };
    }

    private static bool IsComplexType(Type type)
    {
        if (type == typeof(string))
            return false;

        var typeInfo = type.GetTypeInfo();
        return typeInfo.IsClass || typeInfo.IsInterface || type.IsGenericType;
    }

    private static string EscapeTableName(string name, DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.MySql => $"`{name}`",
            DatabaseType.PostgreSql => $"\"{name}\"",
            DatabaseType.SqlServer => $"[{name}]",
            DatabaseType.SQLite => $"`{name}`",
            _ => name
        };
    }

    private static string EscapeColumnName(string name, DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.MySql => $"`{name}`",
            DatabaseType.PostgreSql => $"\"{name}\"",
            DatabaseType.SqlServer => $"[{name}]",
            DatabaseType.SQLite => $"`{name}`",
            _ => name
        };
    }
}
