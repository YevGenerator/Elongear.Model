using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Database.QueryTemplates;

public class UpdateQuery : BaseQuery
{
    public string ColumnName { get; set; } = "";
    public string ReplaceValue { get; set; } = "";
    public override string ToQuery() => $"update {TableName} set {ColumnName} = {ReplaceValue} " + MergedConditions;
}
