using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES_Algorithm {

    class Program {

        static void Main (string[] args) {

            DES des = new DES();
            des.ReadInput();
            des.InitialPermutation();
            des.PermutedChoice();
            des.TransformKeysBlocks();
            des.Wypisz();

        }

    }

}