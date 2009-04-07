using System;

namespace XNATerrainEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (Editor editor = new Editor())
            {
                editor.Run();
            }
        }
    }
}

