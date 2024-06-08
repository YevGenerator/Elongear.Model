using Elongear.Server.Database.QueryTemplates;
using Elongear.Server.Encryption;
using MySqlConnector;

namespace Elongear.Server.Database.Operators;



public class BaseOperator
{
    StringArrayConvert _encoder;
    public BaseOperator()
    {
        _encoder = new();
    }

    protected async Task ExecuteWithoutResult(string[] replaces, object[] values, string query, MySqlConnection connection)
    {

        using var command = GetCommand(replaces, values, query, connection);
        await command.ExecuteNonQueryAsync();
    }
    
    private async Task<MySqlDataReader> GetReaderAsync(string[] replaces, object[] values, string query, MySqlConnection connection)
    {
        using var command = GetCommand(replaces, values, query, connection);
        return await command.ExecuteReaderAsync();
    }

    protected MySqlCommand GetCommand(string[] replaces, object[] values, string query, MySqlConnection connection)
    {
        var command = new MySqlCommand(query, connection);
        SetParameters(replaces, values, command);
        return command;
    }

    private static void SetParameters(string[] replaceValues, object[] values, MySqlCommand command)
    {
        for (int i = 0; i < replaceValues.Length; i++)
        {
            command.Parameters.AddWithValue(replaceValues[i], values[i]);
        }
    }

    protected async Task<bool> Exists(string[] replaces, object[] values, string query, MySqlConnection connection)
    {
        var reader = await GetReaderAsync(replaces, values, query, connection);
        return reader.HasRows;
    }

    protected async Task<string> GetIdIfExists(string[] replaces, object[] values, string query, MySqlConnection connection)
    {
        var reader = await GetReaderAsync(replaces, values, query, connection);
        if (await reader.ReadAsync())
        {
            return reader.GetValue(0).ToString() ?? "";
        }
        return "";
    }
    protected async Task<string[]> ExecuteOneRow(string[] replaces, object[] values, string query, MySqlConnection connection)
    {
        var reader = await GetReaderAsync(replaces, values, query, connection);
        var columns = reader.GetColumnSchema();
        if(await reader.ReadAsync())
        {
            string[] record = new string[columns.Count];
            for (int i = 0; i < columns.Count; i++)
            {
                record[i] = reader.GetValue(i).ToString() ?? "";
            }
            return record;
        }
        return [];
    }
    protected async Task<string[]> ExecuteWithResultEncoded(string[] replaces, object[] values, string query, MySqlConnection connection)
    {
        var reader = await GetReaderAsync(replaces, values, query, connection);
        List<string> results = [];
        var columns = reader.GetColumnSchema();
        while(await reader.ReadAsync())
        {
            string[] record = new string[columns.Count];
            for (int i = 0; i < columns.Count; i++)
            {
                record[i] = reader.GetValue(i).ToString() ?? "";
            }
            var ljson = _encoder.ToLjson(record);
            results.Add(ljson);
        }
        return [.. results];
        
    }

    protected async Task<long> InsertAsync(object[] values, InsertQuery query, MySqlConnection connection)
    {
        var com = query.ToQuery();
        using var command = GetCommand(query.ReplaceValues.ToArray(), values, com, connection);
        await command.ExecuteNonQueryAsync();
        return command.LastInsertedId;
    }

    protected async Task<long> SelectAsync(object[] values, SelectQuery query, MySqlConnection connection)
    {
        var com = query.ToQuery();
        using var command = GetCommand(query.ReplaceValues.ToArray(), values, com, connection);
        await command.ExecuteNonQueryAsync();
        return command.LastInsertedId;
    }

    protected async Task<string[]> SelectEncodedAsync(object[] values, SelectQuery query, MySqlConnection connection) =>
        await ExecuteWithResultEncoded(query.ReplaceValues.ToArray(), values, query.ToQuery(), connection);

    protected async Task<bool> Exists(object[] values, SelectQuery query, MySqlConnection connection)
    {
        return await Exists(query.ReplaceValues.ToArray(), values, query.ToQuery(), connection);
    }

    protected async Task ExecuteIdOnlyWithoutResult(string id, string query, MySqlConnection connection)
        => await ExecuteWithoutResult(["@id"], [id], query, connection);

    protected async Task<string[]> ExecuteIdOnlyWithResult(string id, string query, MySqlConnection connection) => 
        await ExecuteWithResultEncoded(["@id"], [id], query, connection);
}