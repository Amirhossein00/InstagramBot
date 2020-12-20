using InstaBot.Terminal.Authorize;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using InstaSharper.Logger;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace InstaBot.Terminal
{

    class Program
    {
        #region Private
        private static string _userName = string.Empty;
        private static string _password = string.Empty;
        #endregion 

        #region Fields
        private const string arrow = "---------->";
        private static UserSessionData _user;
        private static IInstaApi _api;
        #endregion

        static void Main(string[] args)
        {

            var authorizeUser = new AutorizeUser(_api);

            string currentUsername, currentPassword;
            var userAuthorized = false;

            while(!userAuthorized)
            {
                Console.WriteLine("Username:");
                currentUsername = Console.ReadLine();
                Arrow(_default: true);

                Console.WriteLine("Pasword:");
                currentPassword = Console.ReadLine();
                Arrow(_default: true);

                if(!string.IsNullOrWhiteSpace(currentUsername) && !string.IsNullOrWhiteSpace(currentPassword) &&
                    authorizeUser.ValidateUserSession(currentUsername,currentPassword).Result)
                {
                    _userName = currentUsername;
                    _password = currentPassword;
                    userAuthorized = true;
                }
                else
                {
                    currentPassword = string.Empty;
                    currentUsername = string.Empty;
                }
            }

            Console.WriteLine("{0}Request:",arrow);
            string response = Console.ReadLine();
            int i = 0;
            while(i <= 1)
            {
                var pathImg = $@"c:\Users\a.Hosseinzadeh\Desktop\{Guid.NewGuid().ToString()}.jpg";
                var pathVideo = $@"c:\Users\a.Hosseinzadeh\Desktop\{Guid.NewGuid().ToString()}.mp4";


                string userName = string.Empty;

                Console.WriteLine("{0}Command:",arrow);
                response = Console.ReadLine();
                switch(response.ToLower())
                {
                    case "checkout":
                        Console.WriteLine("{0}UserName:",arrow);
                        string targetUserName = Console.ReadLine();
                        var result = TrackingUser(targetUserName);
                        if(!result.Result.Succeeded)
                        {
                            Console.WriteLine("{0}{1}",arrow,result.Result.Info.Message);
                            break;
                        }

                        Console.WriteLine($"{arrow}Full name is:/n{result.Result.Value.FullName}");
                        Console.WriteLine($"{arrow}Followers count:{result.Result.Value.FollowersCount}");
                        string isPrivate = (result.Result.Value.IsPrivate == true) ? "yes" : "no";
                        Console.WriteLine($"{arrow}Account is private:{isPrivate}");
                        Console.WriteLine($"{arrow}Search social content{result.Result.Value.SearchSocialContext}");
                        Console.WriteLine($"{arrow}Social content{result.Result.Value.SocialContext}");

                        break;

                    case "close":
                        i = 2;
                        break;

                    case "saveimage":

                        Console.Write("{0}UserName:",arrow);
                        userName = Console.ReadLine();

                        if(string.IsNullOrEmpty(userName))
                        {
                            Console.WriteLine("{0}Username is invalid",arrow);
                            break;
                        }

                        var resultImg = TrackingUser(userName);

                        if(!resultImg.Result.Succeeded)
                        {
                            Console.WriteLine("{0}Not found",arrow);
                        }

                        var isSavedSu = CreateImg(resultImg.Result.Value.ProfilePicture,pathImg).Result;
                        if(!isSavedSu)
                        {
                            Console.WriteLine("{0}Cant create image",arrow);
                            break;
                        }

                        Console.WriteLine("{0}File created successfully !",arrow);

                        break;

                    case "clear":
                        Console.Clear();
                        break;


                    case "downloadimage":

                        Console.WriteLine("{0}Img-url:",arrow);
                        var imgUrl = Console.ReadLine();
                        if(string.IsNullOrEmpty(imgUrl))
                        {
                            Console.WriteLine("url is not valid");
                            break;
                        }

                        var res = CreateImg(imgUrl,pathImg).Result;

                        if(!res)
                        {
                            Console.WriteLine("Image was not found");
                            break;
                        }

                        Console.WriteLine("Image created successfully !");

                        break;

                    case "userposts":

                        Console.Write("{0}UserName:",arrow);
                        userName = Console.ReadLine();

                        if(string.IsNullOrEmpty(userName))
                        {
                            Console.WriteLine("{0}User name is invalid",arrow);
                        }
                        Console.WriteLine("{0}Post-count(1=10):",arrow);

                        var strPostCount = Console.ReadLine();
                        int postCount;
                        try
                        {
                            postCount = Convert.ToInt32(strPostCount);
                        }
                        catch
                        {
                            Console.WriteLine("It nust contain only by numbers");
                            break;
                        }



                        var allPosts = GetUserMediaByName(userName,postCount).Result;

                        if(!allPosts.Succeeded)
                        {
                            Console.WriteLine("Unable to load user's data");
                            break;
                        }

                        Console.WriteLine("{0}Continue(download)",arrow);
                        string cont = Console.ReadLine();

                        if(cont != null && (cont == "download" || cont == "d"))
                        {
                            Console.WriteLine("{0}How many:",arrow);
                            int count;
                            try
                            {
                                count = Convert.ToInt32(Console.ReadLine());

                                Console.WriteLine("{0}Select which type of media you need?(vid,img,bot)");

                                var downloadType = new DownloadType();

                                var downloadT = Console.ReadLine();

                                switch(downloadT)
                                {
                                    case "img":
                                        downloadType = DownloadType.ImageOnly;
                                        break;
                                    case "vid":
                                        downloadType = DownloadType.VideoOnly;
                                        break;

                                    default:
                                        break;
                                }

                                DownloadMedia(allPosts,count,downloadType);

                            }
                            catch
                            {
                                Console.WriteLine("Invalid number");
                            }

                        }

                        Console.WriteLine("all posts received successfully !");
                        break;


                    default:
                        Console.WriteLine("404 :)");
                        break;

                    case "test":
                        Console.WriteLine("Url:");
                        string url = Console.ReadLine();
                        var ressss = CreateVideoAsync(url,pathVideo).Result;

                        break;

                }
            }
            Console.ReadKey();
        }

        #region Utils

        private static void DownloadMedia(IResult<InstaMediaList> mediaCollection,int count,DownloadType downloadType,string path = null)
        {

        }
        private static async Task<IResult<InstaMediaList>> GetUserMediaByName(string userName,int postCount)
        {
            var pagination = PaginationParameters.MaxPagesToLoad(postCount);

            IResult<InstaMediaList> media = await _api.GetUserMediaAsync(userName,pagination);

            return await Task.FromResult(media);

        }

        private static async Task<bool> CreateVideoAsync(string url,string savePath)
        {
            try
            {
                using(FileStream fileStream = File.Create(savePath))
                {
                    var httpClient = new HttpClient();

                    var videoFile = httpClient.GetAsync(url).Result;
                    await videoFile.Content.CopyToAsync(fileStream);
                }
                return await Task.FromResult(true);
            }
            catch(Exception re)
            {
                return await Task.FromResult(false);

            }


        }

        private static async Task<bool> CreateImg(string imgUrl,string savePath)
        {
            try
            {
                using(FileStream fileStream = File.Create(savePath))
                {
                    var httpClient = new HttpClient();
                    byte[] byteArray = httpClient.GetByteArrayAsync(imgUrl).Result;
                    fileStream.Write(byteArray,0,byteArray.Length);
                }
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }


        private async static Task<IResult<InstaUser>> TrackingUser(string targetUserName)
        {
            IResult<InstaUser> result = await _api.GetUserAsync(targetUserName);
            return result;
        }

        private static async void Login()
        {
            var delay = new Delay();
            _api = InstaApiBuilder.CreateBuilder()
                .SetUser(_user)
                .UseLogger(new DebugLogger(LogLevel.Response))
                .SetRequestDelay(delay)
                .Build();
            var loginrequest = await _api.LoginAsync();

            if(loginrequest.Succeeded)
                Console.WriteLine("Login proccessed successfully !");

            else
                ///Todo:
                ///Add condition about handdeling if user cant load his account app ask user and pass again
                Console.WriteLine("Login failed");
        }
        public static void Arrow(bool _default = false,ConsoleColor? color = null)
        {
            if(_default)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(arrow);
                Console.ResetColor();
                return;
            }
            if(color.HasValue)
            {
                Console.ForegroundColor = color.Value;
                Console.WriteLine(arrow);
                Console.ResetColor();
                return;
            }
            Console.WriteLine(arrow);
        }
        #endregion
    }
}
