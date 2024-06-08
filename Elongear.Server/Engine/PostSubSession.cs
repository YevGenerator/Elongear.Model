using Elongear.Server.Database.Operators;
using Elongear.Server.Encryption;
using Elongear.Server.StringConstants;
using NetCoreServer;


namespace Elongear.Server.Engine;

public class PostSubSession: BaseSubSession
{
    protected override void InitDict()
    {
        CommandMethods = new()
        {
            { Commands.SignUp, SignUp},
            { Commands.SignIn, SignIn },
            { Commands.ConfirmActivation, ConfirmActivation },
            { Commands.AddPodcast, AddPodcast },
            { Commands.GetCategories, GetCategories },
            { Commands.UploadImage, FileSubSession.UploadImage },
            { Commands.UploadPodcast, FileSubSession.UploadPodcast}

        };
    }
       

    public async void SignUp(HttpRequest request)
    {
        var values = Converter.FromLjson(request.Body);
        var userOperator = new UserOperator();
        if (await userOperator.UserExistsByEmail(values[1]))
        {
            Session.SendErrorResponseAsync(ResponseMessages.EmailExists);
            return;
        }
        if (await userOperator.UserExistsByUserName(values[0]))
        {
            Session.SendErrorResponseAsync(ResponseMessages.LoginExists);
            return;
        }
        var id = await userOperator.CreateUserAsync(values);

        await TokenOperator.AddActivationRecordAsync(id, values[1]);

        var tempLoginToken = TokenOperator.GetShortLoginToken(id);
        Session.SendSuccessResponseAsync(tempLoginToken);
    }

    public async void SignIn(HttpRequest request)
    {
        var values = Converter.FromLjson(request.Body);
        var userOperator = new UserOperator();
        var user = await userOperator.GetUserByLoginAndPassword(values);
        if (user.Length == 0)
        {
            Session.SendErrorResponseAsync(ResponseMessages.IncorrectUser);
            return;
        }
        if (user[1] == "0")
        {
            Session.SendErrorResponseAsync(ResponseMessages.NonActivatedUser);
            return;
        }
        var token = TokenOperator.GetDefaultLoginToken(user[0]);
        Session.SendSuccessResponseAsync(token);
    }

    public async void ConfirmActivation(HttpRequest request)
    {
        var headers = RequestHeaders.Parse(request);
        var digits = request.Body;
        var user = TokenOperator.GetUserFromShortLoginTokenAsync(headers.LoginToken);
        if (user is null)
        {
            Session.SendErrorResponseAsync(ResponseMessages.InvalidShortLoginToken);
            return;
        }
        if (TokenOperator.ConfirmSignUp(user, digits))
        {
            var userOperator = new UserOperator();
            await userOperator.Activate(user);
            Session.SendSuccessResponseAsync(ResponseMessages.SuccessfulActivation);
        }
    }

    public async void AddPodcast(HttpRequest request)
    {
        var headers = RequestHeaders.Parse(request);
        var user = TokenOperator.GetUserFromLoginTokenAsync(headers.LoginToken);
        if (user is null)
        {
            Session.SendErrorResponseAsync(ResponseMessages.NeedToLogin);
            return;
        }
        var values = Converter.FromLjson(request.Body);
        var categoryOperator = new CategoryOperator();
        var categoryId = await categoryOperator.GetOrCreateCategoryRecordAsync([values[3]]);
        if (categoryId == "") categoryId = "0";
        var podcastOperator = new PodcastOperator();
        var id = await podcastOperator.CreatePodcastRecordAsync([values[0], values[1], DateTime.Now, user, categoryId]);
        var uploadToken = TokenOperator.GetUploadToken(user, id);
        Session.SendSuccessResponseAsync(uploadToken);
    }

    public async void GetCategories(HttpRequest request)
    {
        var categoryOperator = new CategoryOperator();
        var categories = await categoryOperator.SelectAllAsync();
        Session.SendSuccessResponseAsync(string.Join("\r\n", categories));
    }

}
