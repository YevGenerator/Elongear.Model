using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Database.QueryTemplates;

public class SelectQuery : BaseQuery
{
    public bool IsDescOrder { get; set; } = false;
    public string[] ColumnNames { get; set; } = [];
    public string OrderColumnName { get; set; } = "";
    public string Limit { get; set; } = "";
    public string Offset { get; set; } = "";
    private string OrderSubQuery()
    {
        if (OrderColumnName == "") return "";
        var query = $"order by {OrderColumnName} ";
        if (IsDescOrder) return query + " desc";
        return query;
    }
    private string LimitSubQuery()
    {
        if (Limit == "") return "";
        return "limit " + Limit;
    }
    private string OffsetSubQuery()
    {
        if (Offset == "") return "";
        return "offset " + Offset;
    }
    public override string ToQuery()
    {
        return $"select {(ColumnNames.Length==0 ? "*" : string.Join(",", ColumnNames))} from {TableName} " + MergedConditions +
            $" {OrderSubQuery()} {LimitSubQuery()} {OffsetSubQuery()}";
    }
}
