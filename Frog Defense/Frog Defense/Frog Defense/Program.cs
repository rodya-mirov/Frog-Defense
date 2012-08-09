using System;

namespace Frog_Defense
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TDGame game = new TDGame())
            {
                game.Run();
            }
        }
    }
#endif
}

