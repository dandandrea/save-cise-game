using System;

namespace SaveCISE_Game
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SaveCiseGame game = new SaveCiseGame())
            {
                game.Run();
            }
        }
    }
#endif
}

