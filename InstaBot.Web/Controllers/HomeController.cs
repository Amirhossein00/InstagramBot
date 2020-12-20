using InstaBot.Web.Models;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstaBot.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Info

        private const string _userName = "programmingdaily1";
        private const string _password = "amir!=amir";
        #endregion

        private UserSessionData _user;
        private IInstaApi _api;

        private async Task<bool> Login()
        {

            _user = new UserSessionData()
            {
                UserName = _userName,
                Password = _password
            };

            var delay = new Delay();
            _api = InstaApiBuilder.CreateBuilder()
                .SetUser(_user)
                .SetRequestDelay(delay)
                .Build();
            var loginRequest = await _api.LoginAsync();

            if (loginRequest.Succeeded)
                return await Task.FromResult(true);

            return await Task.FromResult(false);

        }


        public async Task<IActionResult> Index()
        {
            var loginResult = await Login();

            if (!loginResult)
            {
                return await Task.FromResult((IActionResult)NotFound());
            }

            var currentUser = await _api.GetCurrentUserAsync();

            //var paginationPara = PaginationParameters.MaxPagesToLoad(2);

            //var userPosts = await _api.GetUserMediaAsync(_userName, paginationPara);

            if (!currentUser.Succeeded)
                return null;

            var model = new UserModel()
            {
                UserName = currentUser.Value.UserName,
                Bio = currentUser.Value.Biography,
                ProfilePictureLink = currentUser.Value.ProfilePicture,
                Posts = null
            };


            return View();
        }
    }
}
