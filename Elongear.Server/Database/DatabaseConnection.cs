using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Database;

public static class DatabaseConnection
{
    private static string ConnectionUserName { get; set; } = "root";
    private static string ConnectionPassword { get; set; } = "hackerdown123";
    private static string DbName { get; set; } = "elongear";

    public static string Address { get; set; } = "127.0.0.1";
    public static string ConnectionString => $"server={Address};uid={ConnectionUserName};pwd={ConnectionPassword};database={DbName};charset=utf8";

    public static async Task<MySqlConnector.MySqlConnection> GetConnectionAsync()
    {
        var builder = new MySqlConnector.MySqlConnectionStringBuilder()
        {
            Server = Address,
            Database = DbName,
            CharacterSet = "utf8mb4",
            UserID = ConnectionUserName,
            Password = ConnectionPassword
        };
        var connection = new MySqlConnection(builder.ConnectionString);
        await connection.OpenAsync();
        return connection;
    }

    public static async Task ReadConnection(string fileName)
    {
        using var stream = new StreamReader(fileName);
        ConnectionUserName = await stream.ReadLineAsync() ?? "";
        ConnectionPassword = await stream.ReadLineAsync() ?? "";
        DbName = await stream.ReadLineAsync() ?? "";
        Address = await stream.ReadLineAsync() ?? "";
    }
}
