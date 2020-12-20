using InstaSharper.Classes;
using InstaSharper.Classes.Models;

namespace InstaBot.Web.Models
{
    public partial class UserModel
    {
        public string UserName { get; set; }

        public string Bio { get; set; }

        public string ProfilePictureLink { get; set; }

        public IResult<InstaMediaList> Posts { get; set; }
    }
}
