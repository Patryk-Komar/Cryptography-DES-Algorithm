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
            des.ReadDecryptionInput();
            des.GenerateDecryptionSubkeys();
            des.TransformDecryptionSubkeys();
            des.InverseSubkeysIterations();
            des.DenyInitialPermutation();
            des.DisplayDecryptionResult();

            Console.ReadKey();

        }

    }

}