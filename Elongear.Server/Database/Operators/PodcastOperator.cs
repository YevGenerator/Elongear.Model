using Elongear.Server.Database.QueryTemplates;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Database.Operators;

public class PodcastOperator: BaseOperator
{
    public async Task<string> CreatePodcastRecordAsync(object[] values)
    {
        var query = new InsertQuery()
        {
            TableName = "PodcastData",
            ColumnNames = ["PodcastName", "PodcastDescription", "PodcastUploadTime", "UserUpload", "CategoryId"],
            ReplaceValues = ["@podcastName", "@podcastDescription", "@uploadTime", "@userUploadId", "@categoryId"]
        };

        var insertedId = await InsertAsync(values, query, await DatabaseConnection.GetConnectionAsync());
        return insertedId.ToString();
    }
    public async Task<string[]> SelectPodcastsAsync()
    {
        var query = new SelectQuery()
        {
            TableName = "PodcastData",
        };
        return await SelectEncodedAsync([], query, await DatabaseConnection.GetConnectionAsync());
    }
}
