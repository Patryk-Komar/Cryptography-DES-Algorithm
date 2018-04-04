using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DES_Algorithm {

    public class DES {
        
        private int inputLength;

        private int iterations;

        private int [] input;

        private int [] initialPermutedInput;

        private int [,] initialPermutedInputBlocks;

        private int [,,] initialPermutedInputSplitBlocks;

        private int [,] permutedChoiceKeys;

        private int [,,] permutedChoiceKeysBlocks;

        private int [,,] transformedKeys;

        private int [] initialPermutationNumbers = {57,49,41,33,25,17,9,1,58,50,42,34,26,18,10,2,59,51,43,35,27,19,11,3,60,52,44,36,28,20,12,4,61,53,45,37,29,21,13,5,62,54,46,38,30,22,14,6,63,55,47,39,31,23,15,7,64,56,48,40,32,24,16,8};

        private int [] permutedChoiceOneNumbers = {57,49,41,33,25,17,9,1,58,50,42,34,26,18,10,2,59,51,43,35,27,19,11,3,60,52,44,36,63,55,47,39,31,23,15,7,62,54,46,38,30,22,14,6,61,53,45,37,29,21,13,5,28,20,12,4};

        private int [] permutedChoiceTwoNumbers = {14,17,11,24,1,5,3,28,15,6,21,10,23,19,12,4,26,8,16,7,27,20,13,2,41,52,31,37,47,55,30,40,51,45,33,48,44,49,39,56,34,53,46,42,50,36,29,32};

        private int [] enlargementTableNumbers = {32,1,2,3,4,5,4,5,6,7,8,9,8,9,10,11,12,13,12,13,14,15,16,17,16,17,18,19,20,21,20,21,22,23,24,25,24,25,26,27,28,29,28,29,30,31,32,1};

        private int [] keysTransformNumbers = {1,1,2,2,2,2,2,2,1,2,2,2,2,2,2,1};

        private int [] S1 = {14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7,0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8,4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0,15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13};

        private int [] S2 = {15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10,3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5,0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15,13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9};

        private int [] S3 = {10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8,13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1,13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7,1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12};

        private int [] S4 = {7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15,13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9,10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4,3,15,0,6,10,1,13,8,9,4,5,11,12,7,2,14};

        private int [] S5 = {2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9,14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6,4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14,11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3};

        private int [] S6 = {12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11,10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8,9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6,4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13};

        private int [] S7 = {4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1,13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6,1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2,6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12};

        private int [] S8 = {13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7,1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2,7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8,2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11};

        private int [] permutationNumbers = {16,7,20,21,29,12,28,17,1,15,23,26,5,18,31,10,2,8,24,14,32,27,3,9,19,13,30,6,22,11,4,25};

        public DES () {}

        public void ReadInput () {
            StreamReader streamReader = new StreamReader("input.txt");
            string [] inputLine = streamReader.ReadLine().Split(' ');
            string stringInput = inputLine[0];
            this.inputLength = stringInput.Length;
            this.iterations = stringInput.Length / 64;
            this.input = new int [inputLength];
            this.initialPermutedInput = new int [inputLength];
            this.initialPermutedInputBlocks = new int [iterations,64];
            this.initialPermutedInputSplitBlocks = new int [iterations,2,32]; // lewa część - 0, prawa część - 1
            this.permutedChoiceKeys = new int [iterations,56];
            this.permutedChoiceKeysBlocks = new int [iterations,2,28]; // pierwsza część - 0, druga część - 1
            this.transformedKeys = new int [iterations,16,48]; // to są scalone klucze - punkt 6 i 7 z PDF'a
            for (int i=0; i<inputLength; i++)
                input[i] = (stringInput[i]=='1') ? 1 : 0;
        }


        /* Initial Input Permutation */

        public void InitialPermutation () {
            if (inputLength % 64 == 0)
                for (int i=0; i<inputLength/64; i++)
                    for (int j=0; j<64; j++)
                        initialPermutedInput[i*64+j] = input[i*64+initialPermutationNumbers[j]-1];
            else {
                int multiple64 = inputLength/64;
                for (int i=0; i<multiple64; i++)
                    for (int j=0; j<64; j++)
                        initialPermutedInput[i*64+j] = input[i*64+initialPermutationNumbers[j]-1];
                bool [] alreadyTaken = new bool [64];
                for (int i=multiple64*64; i<inputLength; i++)
                    for (int j=0; j<64; j++)
                        if (inputLength > multiple64*64 + initialPermutationNumbers[j] - 1 && !alreadyTaken[j]) {
                            initialPermutedInput[i] = input[multiple64*64+initialPermutationNumbers[j]-1];
                            alreadyTaken[j] = true;
                            break;
                        }
            }
            for (int i=0; i<iterations; i++)
                for (int j=0; j<64; j++)
                    initialPermutedInputBlocks[i,j] = initialPermutedInput[i*64+j];
        }



        /* Keys generation */

        public void PermutedChoice () {
            for (int i=0; i<iterations; i++) {
                for (int j=0; j<56; j++)
                    permutedChoiceKeys[i,j] = initialPermutedInputBlocks[i,permutedChoiceOneNumbers[j]-1];
                for (int j=0; j<28; j++) {
                    permutedChoiceKeysBlocks[i,0,j] = permutedChoiceKeys[i,j];
                    permutedChoiceKeysBlocks[i,1,j] = permutedChoiceKeys[i,j+28];
                }
            }
        }



        // Chuj wie, jak to nazwać, ale to jest część z przesunięciami tych kluczy. Jeszcze nie wiem, po co to robić, ale chyba trzeba

        public void TransformKeysBlocks () {
            int [] tempLeftKey = new int [28];
            int [] tempRightKey = new int [28];
            int [] tempWholeKey = new int [56];
            for (int i=0; i<iterations; i++) {
                for (int j=0; j<16; j++) {
                    int shift = keysTransformNumbers[j];
                    for (int k=0; k<28-shift; k++) {
                        tempLeftKey[k] = permutedChoiceKeysBlocks[i,0,k+shift];
                        tempRightKey[k] = permutedChoiceKeysBlocks[i,1,k+shift];
                    }
                    for (int k=28-shift; k<28; k++) {
                        tempLeftKey[k] = permutedChoiceKeysBlocks[i,0,k-(28-shift)];
                        tempRightKey[k] = permutedChoiceKeysBlocks[i,1,k-(28-shift)];
                    }
                    for (int k=0; k<28; k++) {
                        tempWholeKey[k] = tempLeftKey[k];
                        tempWholeKey[k+28] = tempRightKey[k];
                    }
                    for (int k=0; k<48; k++)
                        transformedKeys[i,j,k] = tempWholeKey[keysTransformNumbers[k]-1];
                }
            }
        }




        // Teraz jeszcze lepiej. Po pierwsze - chuj wie, jak to nazwać, a po drugie - chuj wie, co tu w ogóle zrobić.
        // Trzeba przekształcić 32-bitowy blok Rn-1 w blok 48 bitowy przy pomocy tablicy rozszerzenia

        public void BlocksEnlargement () { // że niby rozszerzenie bloków
            int [] tempBlock = new int [48];
            for (int i=0; i<48; i++)
                tempBlock[i] = initialPermutedInputSplitBlocks[0,1,enlargementTableNumbers[i]];
            // Chyba ^^^, bo nie jestem do końca pewny, co trzeba XOR'ować - tutaj wziąłem prawą część pierwszego ciągu 64-bitowego z wejścia
            // Wejście było najpierw poddane przekształceniu IP, a potem podzielone na 64-bitowe bloki. I to jest prawa część pierwszego z nich
        }





        




    
        public void Wypisz () {
            Console.Clear();
            for (int i=0; i<inputLength; i++)
                Console.Write(input[i]);
            Console.ReadKey();
        }

    }
}