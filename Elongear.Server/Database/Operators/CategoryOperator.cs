using Elongear.Server.Database.QueryTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Database.Operators;

public class CategoryOperator: BaseOperator
{
    public async Task<string> GetIdIfExists(object name)
    {
        var query = new SelectQuery()
        {
            TableName = "Category",
            ColumnNames = ["CategoryId"],
            ReplaceValues = ["@categoryId"]
        };
        var id = await GetIdIfExists([..query.ReplaceValues], [name], query.ToQuery(), await DatabaseConnection.GetConnectionAsync());
        return id;
    }
    public async Task<string> GetOrCreateCategoryRecordAsync(object[] values)
    {
        var id = await GetIdIfExists(values[0]);
        if (id != "") return id;
        var query = new InsertQuery()
        {
            TableName = "Category",
            ColumnNames = ["CategoryName"],
            ReplaceValues = ["@categoryName"]
        };

        var insertedId = await InsertAsync(values, query, await DatabaseConnection.GetConnectionAsync());
        return insertedId.ToString();
    }

    public async Task<string[]> SelectAllAsync()
    {
        var query = new SelectQuery()
        {
            TableName = "Category",
        };
        return await SelectEncodedAsync([], query, await DatabaseConnection.GetConnectionAsync());
    }
}
