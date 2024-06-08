using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Database.Operators;
using Elongear.Server.Database.QueryTemplates;
public class UserOperator: BaseOperator
{

    public UserOperator()
    {
 
    }
    public async Task CreateSettingsAndStatisticRecords(MySqlConnection connection)
    {
        using var command = new MySqlCommand("select last_insert_id()", connection);
        var userId = await command.ExecuteScalarAsync();
        var strId = userId?.ToString() ?? "0";
    }

    public async Task<string> CreateUserAsync(string[] values)
    {
        var query = new InsertQuery()
        {
            TableName = "UserData",
            ColumnNames = ["UserName", "UserEmail", "UserPassword", "IsActivated"],
            ReplaceValues = ["@userName", "@email", "@password", "@isActive"]
        };
        
        var insertedId = await InsertAsync(values, query, await DatabaseConnection.GetConnectionAsync());
        return insertedId.ToString();
    }

    /*public async Task<string[]> SelectShortAsync(uint id, MySqlConnection connection) => 
        await SelectByIdAsync(id, Query.SelectShortRecordQuery, connection);*/

    public async Task<string[]> GetUserByLoginAndPassword(string[] values)
    {
        var query = new SelectQuery()
        {
            TableName = "UserData",
            ColumnNames = ["UserId", "IsActivated"],
        };
        var replaceValues = new string[] { "@login", "@password" };
        query.AddConditions(["UserName", "UserPassword"], ["=", "="], replaceValues);
        return await ExecuteOneRow(replaceValues, values, query.ToQuery(), await DatabaseConnection.GetConnectionAsync());
    }
    public async Task Activate(string userId)
    {
        var query = new UpdateQuery()
        {
            TableName = "UserData",
            ColumnName = "IsActivated",
            ReplaceValue ="@isActivated"
        };
        query.AddCondition("UserId", "=", "@userId");
        await ExecuteWithoutResult(["@userId", query.ReplaceValue], [userId, "1"], query.ToQuery(), await DatabaseConnection.GetConnectionAsync());
    }
    public async Task<bool> UserExistsByEmailAndPassword(string[] values, MySqlConnection connection)
    {
        var query = new SelectQuery()
        {
            TableName = "UserData"
        };
        query.AddConditions(["UserEmail", "UserPassword"], ["=", "="], ["@email", "@password"]);
        return await Exists(["@email", "@password"], values, query.ToQuery(), connection);
    }
    public async Task<bool> UserExistsByEmail(string email)
    {
        var query = new SelectQuery()
        {
            TableName = "UserData"
        };
        query.AddCondition("UserEmail", "=", "@email");
        return await Exists(["@email"], [email], query.ToQuery(), await DatabaseConnection.GetConnectionAsync());
    }

    public async Task<bool> UserExistsByUserName(string userName)
    {
        var query = new SelectQuery()
        {
            TableName = "UserData"
        };
        query.AddCondition("UserName", "=", "@userName");
        return await Exists(["@userName"], [userName], query.ToQuery(), await DatabaseConnection.GetConnectionAsync());
    }
}
