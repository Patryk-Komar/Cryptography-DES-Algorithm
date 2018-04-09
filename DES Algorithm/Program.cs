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

            bool exit = false;

            while (!exit) {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("   [1] Algorytm DES - szyfrowanie");
                Console.WriteLine("   [2] Algorytm DES - deszyfrowanie");
                Console.WriteLine("   [3] Algorytm DES - obsługa bliku binarnego\n");
                Console.WriteLine("   [P] Pomoc\n\n");
                Console.WriteLine("   [ESC] Wyjście");
                ConsoleKey key = Console.ReadKey().Key;
                Console.Clear();
                switch (key) {
                    case ConsoleKey.D1:
                        des.ReadEncryptionInput();
                        des.GenerateEncryptionSubkeys();
                        des.TransformEncryptionSubkeys();
                        des.EncryptionInitialPermutation();
                        des.EncryptionAlgorithm();
                        des.FormatEncryptionOutput();
                        des.DisplayEncryptionResult();
                        break;
                    case ConsoleKey.D2:
                        des.ReadDecryptionInput();
                        des.GenerateDecryptionSubkeys();
                        des.TransformDecryptionSubkeys();
                        des.InverseSubkeysIterations();
                        des.DenyInitialPermutation();
                        des.FormatDecryptionOutput();
                        des.DisplayDecryptionResult();
                        break;
                    case ConsoleKey.D3:
                        des.ReadBinaryFileInput();
                        des.GenerateEncryptionSubkeys();
                        des.TransformEncryptionSubkeys();
                        des.EncryptionInitialPermutation();
                        des.EncryptionAlgorithm();
                        des.ConvertBinaryFileEncryptionOutput();
                        des.SaveOutputBinaryFile();
                        des.GenerateDecryptionSubkeys();
                        des.TransformDecryptionSubkeys();
                        des.InverseSubkeysIterations();
                        des.DenyInitialPermutation();
                        des.ConvertBinaryFileDecryptionOutput();
                        des.SaveOutputBinaryDecryptionFile();
                        break;
                    case ConsoleKey.P:
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }

            Environment.Exit(0);

            /*
            byte [] byteArray = File.ReadAllBytes("co.png");

            int [] binaryArray = new int [byteArray.Length*8];

            for (int i=0; i<byteArray.Length; i++) {
                int ascii = byteArray[i];
                string stringValue = Convert.ToString(ascii,2);
                for (int j=0; j<8-stringValue.Length; j++)
                    binaryArray[8*i+j] = 0;
                for (int j=0; j<stringValue.Length; j++)
                    binaryArray[8*i+j+(8-stringValue.Length)] = Int32.Parse(stringValue[j].ToString());
            }

            using(Image image = Image.FromStream(new MemoryStream(byteArray))) {
                image.Save("output.jpg", ImageFormat.Jpeg);
            }
            */

            Console.ReadKey();

        }

    }

}