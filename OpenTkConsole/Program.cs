using System;

using Util;

namespace OpenTkConsole
{
    static class Program
    {
        [STAThread]
        static void Main() {
            new MainWindow().Run(60);
        }
    }
}
