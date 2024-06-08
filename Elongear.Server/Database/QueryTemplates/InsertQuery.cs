using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Database.QueryTemplates;

public class InsertQuery: BaseQuery
{
    public string[] ColumnNames { get; set; } = [];

    public override string ToQuery() =>
        $"insert into {TableName}({string.Join(", ", ColumnNames)}) values({string.Join(", ", ReplaceValues)})";
}
