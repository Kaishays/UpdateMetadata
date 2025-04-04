using System.Data;
using Dapper;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Collections;
namespace MetaDatabaseLibrary.DBLogic
{
    public static partial class MySQLDataAccess
    {
        public static async Task<List<ReturnParameter>> QuerySQL<ReturnParameter, InputParameterToQuery>(string sql, InputParameterToQuery parameter, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<ReturnParameter>(sql, parameter, commandTimeout: 9999);
                return rows.ToList();
            }
        }
        public static async Task<List<ReturnParameter>> QuerySQL<ReturnParameter>(string sql, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<ReturnParameter>(sql, commandTimeout: 9999);
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
                // Debug.WriteLine(sql);
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
        public static async Task DeleteEntryFromDatabase<GenericParameter_1>(string sql, GenericParameter_1 parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }
        public static void TestConnection(string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                if (connection != null)
                {
                    Debug.WriteLine("Connection successful to database");
                }
            }
        }
        public static async Task DeleteAllEntries_in_videoModel(string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM video_id;" +
                             "\nALTER TABLE video_id AUTO_INCREMENT = 1;";
                await connection.ExecuteAsync(sql);
                Debug.WriteLine("Entries are Deleted");
            }
        }
    }
}