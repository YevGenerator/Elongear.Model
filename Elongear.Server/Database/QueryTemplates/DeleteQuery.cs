using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Database.QueryTemplates;

public class DeleteQuery : BaseQuery
{
    public override string ToQuery() => $"delete from {TableName} " + MergedConditions;
}
