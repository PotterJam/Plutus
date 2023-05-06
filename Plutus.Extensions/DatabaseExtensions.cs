using System;
using System.Data.Common;
using Npgsql;

namespace Extensions;

public static class DatabaseExtensions
{
    public static void AddParam<T>(this DbCommand command, string paramName, T value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = paramName;

        parameter.Value = (object?) value ?? DBNull.Value;
        
        command.Parameters.Add(parameter);
    }

    public static void AddEnumParam<TEnum>(this DbCommand command, string paramName, TEnum value) where TEnum : Enum
    {
        var parameter = new NpgsqlParameter();
        
        parameter.ParameterName = paramName;
        parameter.DataTypeName = value.GetType().Name.ToLower();
        parameter.Value = value.ToString();
        
        command.Parameters.Add(parameter);
    }
    
    public static T Get<T>(this DbDataReader reader, string paramName) where T : notnull
    {
        return (T) reader[paramName];
    }
    
    public static T? GetNullable<T>(this DbDataReader reader, string paramName)
    {
        var value = reader[paramName];
        
        if (value is DBNull)
        {
            return default;
        }
        
        return (T) value;
    }
    
    public static TEnum GetEnum<TEnum>(this DbDataReader reader, string paramName) where TEnum : struct
    {
        var valueStr = reader.Get<string>(paramName);
        
        return Enum.Parse<TEnum>(valueStr);
    }
}