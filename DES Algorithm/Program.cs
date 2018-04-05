using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace DES_Algorithm {

    class Program {

        static void Main (string[] args) {

            DES des = new DES();
            des.ReadInput();
            des.InitialPermutation();
            des.PermutedChoice();
            des.TransformSubkeys();
            des.XORowanko();
            des.xd();
            des.ConnectDecryptionOutput();
            des.DenyInitialPermutation();
            

            Console.ReadKey();

        }

    }

}