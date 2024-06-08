using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Database.QueryTemplates;

public abstract class BaseQuery
{
    public string TableName { get; set; } = "";
    public List<string> Conditions { get; set; } = [];
    public List<string> ReplaceValues { get; set; } = [];
    public void AddCondition(string columnName, string action, string replaceValue)
    {
        Conditions.Add($"{columnName} {action} {replaceValue}");
        ReplaceValues.Add(replaceValue);
    }

    public void AddConditions(string[] columnNames, string[] actions, string[] replaceValues)
    {
        for(int i =0; i <columnNames.Length; i++)
        {
            AddCondition(columnNames[i], actions[i], replaceValues[i]);
        }
    }
    public string MergedConditions => Conditions.Count == 0 ? "" : "where " + string.Join(" and ", Conditions);
    public abstract string ToQuery();
}
