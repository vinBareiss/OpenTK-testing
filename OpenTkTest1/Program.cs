using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTkTest1
{
    class Program
    {
        
        static void Main(string[] args) {
            using( Game game = new Game()) {
                game.Run(60, 60);
            }
        }
    }
}
