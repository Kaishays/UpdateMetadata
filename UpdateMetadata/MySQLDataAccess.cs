using System.Data;
using Dapper;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Collections;
using System.Windows.Controls.Primitives;
namespace UpdateMetadata
{
    public static partial class MySQLDataAccess
    {
    
        public static async Task<List<ReturnParameter>> QuerySQL<ReturnParameter, InputParameterToQuery>(string sql, InputParameterToQuery parameter, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<ReturnParameter>(sql, parameter);
                return rows.ToList();
            }
        }
        public static async Task<List<ReturnParameter>> QuerySQL<ReturnParameter>(string sql, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<ReturnParameter>(sql);
                return rows.ToList();
            }
        }
        public static async Task ExecuteSQL<GenericParameter_1>(string sql, GenericParameter_1 parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(sql);
                await connection.ExecuteAsync(sql, parameters);
            }
        }
        public static async Task ExecuteSQL(string sql, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(sql);
                await connection.ExecuteAsync(sql);
            }
        }
        public static async Task ExecuteSQL<GenericParameter_1, GenericParameter_>(string sql, GenericParameter_1 parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(sql);
                await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}