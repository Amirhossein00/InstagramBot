using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Logger;
using System.Threading.Tasks;

namespace InstaBot.Terminal.Authorize
{
    public partial class AutorizeUser
    {
        private IInstaApi _api;

        public AutorizeUser(IInstaApi api)
        {
            this._api = api;
        }

        public async virtual Task<bool> ValidateUserSession(string userName,string password)
        {
            var userAuthorizeInfo = new UserSessionData()
            {
                UserName = userName,
                Password = password
            };

            var delay = new Delay();
            _api = InstaApiBuilder.CreateBuilder()
                .SetUser(userAuthorizeInfo)
                .UseLogger(new DebugLogger(LogLevel.Response))
                .SetRequestDelay(delay)
                .Build();
            var loginrequest = await _api.LoginAsync();

            if(loginrequest.Succeeded)
                return await Task.FromResult(true);

            return await Task.FromResult(false);
        }
    }
}
