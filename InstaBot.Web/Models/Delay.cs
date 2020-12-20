using InstaSharper.Classes;
using System;

namespace InstaBot.Web.Models
{
    public class Delay : IRequestDelay
    {
        public TimeSpan Value => TimeSpan.FromSeconds(8);

        public bool Exist => true;

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public void Enable()
        {
            throw new NotImplementedException();
        }
    }
}
