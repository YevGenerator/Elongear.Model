using Elongear.Server.Encryption;
using Elongear.Server.StringConstants;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Engine;

public abstract class BaseSubSession()
{
    protected string CurrentCommand { get; set; } = "";
    protected Session Session { get; init; }
    protected StringArrayConvert Converter { get; } = new();
    protected TokenOperator TokenOperator => Session.MyServer.TokenOperator;
    protected FileSubSession FileSubSession => Session.FileSubSession;
    protected Dictionary<string, Action<HttpRequest>> CommandMethods { get; set; } = [];

    public static T Create<T>(Session session) where T: BaseSubSession, new()
    {
        var sub = new T
        {
            Session = session
        };
        sub.InitDict();
        return sub;
    }

    protected abstract void InitDict();

    public void InvokeCommand(string command, HttpRequest request)
    {
        if (command == "") return;
        CommandMethods[command](request);
    }
}
