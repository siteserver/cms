using System;
using System.Data;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 数据库操作Api接口。
    /// </summary>
    public interface IDatabaseApi
    {
        /// <summary>
        /// 返回给定连接字符串的IDbConnection对象。
        /// </summary>
        /// <param name="databaseType">需要连接的数据库类型。</param>
        /// <param name="connectionString">用于创建连接的连接字符串。</param>
        /// <returns>IDbConnection 对象。</returns>
        /// <exception cref="System.ArgumentNullException">如果connectionString是null就抛出。</exception>
        IDbConnection GetConnection(DatabaseType databaseType, string connectionString);

        /// <summary>
        /// 返回给定连接字符串的IDbConnection对象。
        /// </summary>
        /// <param name="connectionString">用于创建连接的连接字符串。</param>
        /// <returns>IDbConnection 对象。</returns>
        /// <exception cref="System.ArgumentNullException">如果connectionString是null就抛出。</exception>
        IDbConnection GetConnection(string connectionString);

        /// <summary>
        /// 获取用于SQL命令的IDataParameter。
        /// </summary>
        /// <param name="name">要创建的参数的名称。</param>
        /// <param name="value">指定参数的值。</param>
        /// <returns>IDataParameter 对象。</returns>
        IDataParameter GetParameter(string name, object value);

        /// <summary>
        /// 获取数据库字符串类型值。
        /// </summary>
        /// <param name="rdr">IDataReader 对象。</param>
        /// <param name="i">位于第几列，从零开始计算。</param>
        /// <returns>数据库中存储的字符串类型值。</returns>
        string GetString(IDataReader rdr, int i);

        /// <summary>
        /// 获取数据库布尔类型值。
        /// </summary>
        /// <param name="rdr">IDataReader 对象。</param>
        /// <param name="i">位于第几列，从零开始计算。</param>
        /// <returns>数据库中存储的布尔类型值。</returns>
        bool GetBoolean(IDataReader rdr, int i);

        /// <summary>
        /// 获取数据库整数类型值。
        /// </summary>
        /// <param name="rdr">IDataReader 对象。</param>
        /// <param name="i">位于第几列，从零开始计算。</param>
        /// <returns>数据库中存储的整数类型值。</returns>
        int GetInt(IDataReader rdr, int i);

        /// <summary>
        /// 获取数据库小数类型值。
        /// </summary>
        /// <param name="rdr">IDataReader 对象。</param>
        /// <param name="i">位于第几列，从零开始计算。</param>
        /// <returns>数据库中存储的整数类型值。</returns>
        decimal GetDecimal(IDataReader rdr, int i);

        /// <summary>
        /// 获取数据库日期/时间类型值。
        /// </summary>
        /// <param name="rdr">IDataReader 对象。</param>
        /// <param name="i">位于第几列，从零开始计算。</param>
        /// <returns>数据库中存储的日期/时间类型值。</returns>
        DateTime GetDateTime(IDataReader rdr, int i);

        /// <summary>
        /// 获取数据库字符串类型值。
        /// </summary>
        /// <param name="rdr">IDataReader 对象。</param>
        /// <param name="name">需要获取的列名称。</param>
        /// <returns>数据库中存储的字符串类型值。</returns>
        string GetString(IDataReader rdr, string name);

        /// <summary>
        /// 获取数据库布尔类型值。
        /// </summary>
        /// <param name="rdr">IDataReader 对象。</param>
        /// <param name="name">需要获取的列名称。</param>
        /// <returns>数据库中存储的布尔类型值。</returns>
        bool GetBoolean(IDataReader rdr, string name);

        /// <summary>
        /// 获取数据库整数类型值。
        /// </summary>
        /// <param name="rdr">IDataReader 对象。</param>
        /// <param name="name">需要获取的列名称。</param>
        /// <returns>数据库中存储的整数类型值。</returns>
        int GetInt(IDataReader rdr, string name);

        /// <summary>
        /// 获取数据库小数类型值。
        /// </summary>
        /// <param name="rdr">IDataReader 对象。</param>
        /// <param name="name">需要获取的列名称。</param>
        /// <returns>数据库中存储的小数类型值。</returns>
        decimal GetDecimal(IDataReader rdr, string name);

        /// <summary>
        /// 获取数据库日期/时间类型值。
        /// </summary>
        /// <param name="rdr">IDataReader 对象。</param>
        /// <param name="name">需要获取的列名称。</param>
        /// <returns>数据库中存储的日期/时间类型值。</returns>
        DateTime GetDateTime(IDataReader rdr, string name);

        /// <summary>
        /// 获取当前数据库类型的能够分页的SQL语句。
        /// </summary>
        /// <param name="tableName">数据库表名称。</param>
        /// <param name="columnNames">需要返回的字段名称，多个字段用英文逗号分隔。</param>
        /// <param name="whereSqlString">WHERE SQL语句。</param>
        /// <param name="orderSqlString">ORDER SQL语句。</param>
        /// <param name="offset">
        /// 返回结果的偏移量。
        /// offset 表示在开始返回行之前跳过这么多行，如果从第一行开始返回，offset 是 0，以此类推。
        /// </param>
        /// <param name="limit">
        /// 返回结果的数量。
        /// 如果不限制，将 limit 值设置为 0。
        /// 如果给定大于0的值，返回的结果将不会超过这个数目。
        /// </param>
        /// <returns>完整的翻页SQL语句。</returns>
        string GetPageSqlString(string tableName, string columnNames, string whereSqlString, string orderSqlString,
            int offset, int limit);

        /// <summary>
        /// 获取当前数据库类型的加操作符SQL语句。
        /// </summary>
        /// <param name="columnName">需要增加的字段的名称。</param>
        /// <param name="plusNum">需要增加的值。</param>
        /// <returns>当前数据库类型的加操作符SQL语句。</returns>
        string ToPlusSqlString(string columnName, int plusNum);

        /// <summary>
        /// 获取当前数据库类型的减操作符SQL语句。
        /// </summary>
        /// <param name="columnName">需要减少的字段的名称。</param>
        /// <param name="minusNum">需要减少的值。</param>
        /// <returns>当前数据库类型的减操作符SQL语句。</returns>
        string ToMinusSqlString(string columnName, int minusNum);

        /// <summary>
        /// 获取当前数据库类型代表当前时间的SQL语句。
        /// </summary>
        /// <returns>当前数据库类型代表当前时间的SQL语句。</returns>
        string ToNowSqlString();

        /// <summary>
        /// 将日期/时间值转换为当前数据库类型能够识别的日期SQL语句。
        /// </summary>
        /// <param name="val">日期/时间值。</param>
        /// <returns>当前数据库类型能够识别的日期SQL语句。</returns>
        string ToDateSqlString(DateTime val);

        /// <summary>
        /// 将日期/时间值转换为当前数据库类型能够识别的时间SQL语句。
        /// </summary>
        /// <param name="val">日期/时间值。</param>
        /// <returns>当前数据库类型能够识别的时间SQL语句。</returns>
        string ToDateTimeSqlString(DateTime val);

        /// <summary>
        /// 将布尔值转换为当前数据库类型能够识别的布尔SQL语句。
        /// </summary>
        /// <param name="val">布尔值。</param>
        /// <returns>当前数据库类型能够识别的布尔SQL语句。</returns>
        string ToBooleanSqlString(bool val);

        /// <summary>
        /// 对连接字符串中指定的数据库执行命令并返回DataSet。
        /// </summary>
        /// <example>
        /// <code>
        /// DataSet ds = databaseApi.ExecuteDataset(connString, "SELECT * FROM table");
        /// </code>
        /// </example>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="sqlString">SELECT SQL 语句。</param>
        /// <returns>命令执行后获取的DataSet。</returns>
        /// <exception cref="System.ArgumentNullException">如果connectionString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        DataSet ExecuteDataset(string connectionString, string sqlString);

        /// <summary>
        /// 使用所提供的参数对连接字符串中指定的数据库执行命令并返回DataSet。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="sqlString">SELECT SQL 语句。</param>
        /// <param name="commandParameters">用于执行命令的IDbParamters的数组。</param>
        /// <returns>命令执行后获取的DataSet。</returns>
        /// <exception cref="System.ArgumentNullException">如果connectionString为空，则抛出。</exception>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        DataSet ExecuteDataset(string connectionString, string sqlString, params IDataParameter[] commandParameters);

        /// <summary>
        /// 使用所提供的参数对连接字符串中指定的数据库执行命令并返回DataSet。
        /// </summary>
        /// <param name="connection">有效的IDbConnection。</param>
        /// <param name="sqlString">SELECT SQL 语句。</param>
        /// <returns>命令执行后获取的DataSet。</returns>
        /// <exception cref="System.ArgumentNullException">如果connection为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        DataSet ExecuteDataset(IDbConnection connection, string sqlString);

        /// <summary>
        /// 使用所提供的参数对连接字符串中指定的数据库执行命令并返回DataSet。
        /// </summary>
        /// <param name="connection">有效的IDbConnection。</param>
        /// <param name="sqlString">SELECT SQL 语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>命令执行后获取的DataSet。</returns>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果connection为空，则抛出。</exception>
        DataSet ExecuteDataset(IDbConnection connection, string sqlString, params IDataParameter[] commandParameters);

        /// <summary>
        /// 使用所提供的参数对连接字符串中指定的数据库执行命令并返回DataSet。
        /// </summary>
        /// <param name="transaction">有效的IDbTransaction。</param>
        /// <param name="sqlString">SELECT SQL 语句。</param>
        /// <returns>命令执行后获取的DataSet。</returns>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果transaction为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果transaction.Connection为空，则抛出。</exception>
        DataSet ExecuteDataset(IDbTransaction transaction, string sqlString);

        /// <summary>
        /// 使用所提供的参数对连接字符串中指定的数据库执行命令并返回DataSet。
        /// </summary>
        /// <param name="transaction">有效的IDbTransaction。</param>
        /// <param name="sqlString">SELECT SQL 语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>命令执行后获取的DataSet。</returns>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果transaction为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果transaction.Connection为空，则抛出。</exception>
        DataSet ExecuteDataset(IDbTransaction transaction, string sqlString,
            params IDataParameter[] commandParameters);

        /// <summary>
        /// 对连接字符串中指定的数据库执行命令。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <returns>返回执行SQL命令所影响的行数。</returns>
        /// <exception cref="System.ArgumentNullException">如果connectionString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        int ExecuteNonQuery(string connectionString, string sqlString);

        /// <summary>
        /// 对连接字符串中指定的数据库执行命令。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>返回执行SQL命令所影响的行数。</returns>
        /// <exception cref="System.ArgumentNullException">如果connectionString为空，则抛出。</exception>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        int ExecuteNonQuery(string connectionString, string sqlString, params IDataParameter[] commandParameters);

        /// <summary>
        /// 对连接字符串中指定的数据库执行命令。
        /// </summary>
        /// <param name="connection">有效的IDbConnection。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <returns>返回执行SQL命令所影响的行数。</returns>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果connection为空，则抛出。</exception>
        int ExecuteNonQuery(IDbConnection connection, string sqlString);

        /// <summary>
        /// 对连接字符串中指定的数据库执行命令。
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="connection">有效的IDbConnection。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>返回执行SQL命令所影响的行数。</returns>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果connection为空，则抛出。</exception>
        int ExecuteNonQuery(IDbConnection connection, string sqlString, params IDataParameter[] commandParameters);

        /// <summary>
        /// 对连接字符串中指定的数据库执行命令。
        /// </summary>
        /// <param name="transaction">有效的IDbTransaction。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <returns>返回执行SQL命令所影响的行数。</returns>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果transaction为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果transaction.Connection为空，则抛出。</exception>
        int ExecuteNonQuery(IDbTransaction transaction, string sqlString);

        /// <summary>
        /// 对连接字符串中指定的数据库执行命令。
        /// </summary>
        /// <param name="transaction">有效的IDbTransaction。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>返回执行SQL命令所影响的行数。</returns>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果transaction为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果transaction.Connection为空，则抛出。</exception>
        int ExecuteNonQuery(IDbTransaction transaction, string sqlString, params IDataParameter[] commandParameters);

        /// <summary>
        /// 执行INSERT SQL命令并返回自增长Id的值。
        /// </summary>
        /// <param name="tableName">表名称。</param>
        /// <param name="idColumnName">自增长字段名称。</param>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="sqlString">INSERT SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>INSERT SQL语句执行后表生成的自增长Id。</returns>
        int ExecuteNonQueryAndReturnId(string tableName, string idColumnName, string connectionString,
            string sqlString, params IDataParameter[] commandParameters);

        /// <summary>
        /// 执行INSERT SQL命令并返回自增长Id的值。
        /// </summary>
        /// <param name="tableName">表名称。</param>
        /// <param name="idColumnName">自增长字段名称。</param>
        /// <param name="transaction">有效的IDbTransaction。</param>
        /// <param name="sqlString">INSERT SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>INSERT SQL语句执行后表生成的自增长Id。</returns>
        int ExecuteNonQueryAndReturnId(string tableName, string idColumnName, IDbTransaction transaction,
            string sqlString, params IDataParameter[] commandParameters);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回IDataReader。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <returns>命令执行后获取的DataReader。</returns>
        /// <exception cref="System.ArgumentNullException">如果connectionString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        IDataReader ExecuteReader(string connectionString, string sqlString);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回IDataReader。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>命令执行后获取的DataReader。</returns>
        /// <exception cref="System.ArgumentNullException">如果connectionString为空，则抛出。</exception>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        IDataReader ExecuteReader(string connectionString, string sqlString,
            params IDataParameter[] commandParameters);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回IDataReader。
        /// </summary>
        /// <example>
        /// <code>
        /// IDataReader dr = databaseApi.ExecuteReader(conn, "SELECT * FROM table");
        /// </code>
        /// </example>
        /// <param name="connection">有效的IDbConnection。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <returns>命令执行后获取的DataReader。</returns>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        IDataReader ExecuteReader(IDbConnection connection, string sqlString);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回IDataReader。
        /// </summary>
        /// <param name="connection">有效的IDbConnection。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>命令执行后获取的DataReader。</returns>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果connection为空，则抛出。</exception>
        IDataReader ExecuteReader(IDbConnection connection, string sqlString,
            params IDataParameter[] commandParameters);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回IDataReader。
        /// </summary>
        /// <param name="transaction">有效的IDbTransaction。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <returns>命令执行后获取的DataReader。</returns>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        IDataReader ExecuteReader(IDbTransaction transaction, string sqlString);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回IDataReader。
        /// </summary>
        /// <param name="transaction">有效的IDbTransaction。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>命令执行后获取的DataReader。</returns>
        IDataReader ExecuteReader(IDbTransaction transaction, string sqlString,
            params IDataParameter[] commandParameters);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回值。
        /// </summary>
        /// <example>
        /// <code>
        /// int orderCount = (int)databaseApi.ExecuteScalar(connString, "SELECT COUNT(*) FROM table");
        /// </code>
        /// </example>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <returns>执行SQL 命令获取的返回值。</returns>
        /// <exception cref="System.ArgumentNullException">如果connectionString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        object ExecuteScalar(string connectionString, string sqlString);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回值。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>执行SQL 命令获取的返回值。</returns>
        /// <exception cref="System.ArgumentNullException">如果connectionString为空，则抛出。</exception>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        object ExecuteScalar(string connectionString, string sqlString, params IDataParameter[] commandParameters);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回值。
        /// </summary>
        /// <param name="connection">有效的IDbConnection。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <returns>执行SQL 命令获取的返回值。</returns>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        object ExecuteScalar(IDbConnection connection, string sqlString);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回值。
        /// </summary>
        /// <param name="connection">有效的IDbConnection。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>执行SQL 命令获取的返回值。</returns>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果connection为空，则抛出。</exception>
        object ExecuteScalar(IDbConnection connection, string sqlString, params IDataParameter[] commandParameters);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回值。
        /// </summary>
        /// <param name="transaction">有效的IDbTransaction。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <returns>执行SQL 命令获取的返回值。</returns>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        object ExecuteScalar(IDbTransaction transaction, string sqlString);

        /// <summary>
        /// 对连接字符串中指定的数据库执行SQL 命令并返回值。
        /// </summary>
        /// <param name="transaction">有效的IDbTransaction。</param>
        /// <param name="sqlString">SQL语句。</param>
        /// <param name="commandParameters">用于执行命令的IDataParameter数组。</param>
        /// <returns>执行SQL 命令获取的返回值。</returns>
        /// <exception cref="System.InvalidOperationException">如果有任何IDataParameters参数名为空，或者如果参数计数不匹配所提供的值的数量，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果sqlString为空，则抛出。</exception>
        /// <exception cref="System.ArgumentException">如果参数计数与提供的值数量不匹配，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果transaction为空，则抛出。</exception>
        /// <exception cref="System.ArgumentNullException">如果transaction.Connection为空，则抛出。</exception>
        object ExecuteScalar(IDbTransaction transaction, string sqlString, params IDataParameter[] commandParameters);

        /// <summary>
        /// 获取当前数据库类型INSERT SQL语句执行后表生成的自增长Id。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        /// <param name="tableName">数据库表名称。</param>
        /// <param name="idColumnName">自增长字段名称。</param>
        /// <returns>INSERT SQL语句执行后表生成的自增长Id值。</returns>
        int ExecuteCurrentId(string connectionString, string tableName, string idColumnName);

        /// <summary>
        /// 获取当前数据库类型INSERT SQL语句执行后表生成的自增长Id
        /// </summary>
        /// <param name="connection">有效的IDbConnection。</param>
        /// <param name="tableName">数据库表名称。</param>
        /// <param name="idColumnName">自增长字段名称。</param>
        /// <returns>INSERT SQL语句执行后表生成的自增长Id值。</returns>
        int ExecuteCurrentId(IDbConnection connection, string tableName, string idColumnName);

        /// <summary>
        /// 获取当前数据库类型INSERT SQL语句执行后表生成的自增长Id
        /// </summary>
        /// <param name="transaction">有效的IDbTransaction。</param>
        /// <param name="tableName">数据库表名称。</param>
        /// <param name="idColumnName">自增长字段名称。</param>
        /// <returns>INSERT SQL语句执行后表生成的自增长Id值。</returns>
        int ExecuteCurrentId(IDbTransaction transaction, string tableName, string idColumnName);
    }
}