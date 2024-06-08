using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elongear.Server.StringConstants;
using Elongear.Server.Database.Operators;
namespace Elongear.Server.Engine;

public class GetSubSession : BaseSubSession
{
    protected override void InitDict()
    {
        CommandMethods = new()
        {
            { Commands.Select, OnSelect},
            { Commands.Images, FileSubSession.SendImage},
            { Commands.Listen, FileSubSession.OnListen},
            { Commands.Download, FileSubSession.SendPodcast},
            { Commands.ConfirmActivation, OnConfirmActivation}
        };
    }
   
   
    public async void OnConfirmActivation(HttpRequest request)
    {
        var commands = RequestHeaders.UrlToCommands(request);
        var values = TokenOperator.GetUserAndDigitsFromActivationTokenAsync(commands[1]);
        if (values.Length == 0)
        {
            Session.SendErrorResponseAsync(ResponseMessages.InvalidActivationToken);
            return;
        }
        if (TokenOperator.ConfirmSignUp(values[0], values[1]))
        {
            var userOperator = new UserOperator();
            await userOperator.Activate(values[0]);
            var oo3 = Session.SendSuccessResponseAsync(ResponseMessages.SuccessfulActivation);
            return;
        }
        Session.SendErrorResponseAsync(ResponseMessages.InvalidActivationToken);
    }
    
    public async void OnSelect(HttpRequest request)
    {
        var selectOperator = new PodcastOperator();
        var result = await selectOperator.SelectPodcastsAsync();
        var toSend = string.Join("\r\n", result);
        Session.SendSuccessResponseAsync(toSend);
    }
}
