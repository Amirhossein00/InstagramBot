using InstaSharper.Classes;
using System;

namespace InstaBot.Terminal
{
    public class Delay:IRequestDelay
    {
        public TimeSpan Value => TimeSpan.FromSeconds(5);

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
