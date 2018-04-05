using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DES_Algorithm {

    public class DES {

        
        #region Dane potrzebne do przeprowadzenia algorytmu szyfrowania

        #region Tekst jawny

        private int [] encryptionInput;

        private int encryptionInputBlocksNumber;

        private int [,] encryptionInputBlocks;

        #endregion

        #region Permutacja tekstu jawnego
        
        private int [,] encryptionInputPermutedBlocks;

        private int [,,] encryptionInputPermutedBlocksSplit;

        #endregion

        #region Klucz

        private int [] encryptionKey;

        private int [,] encryptionSplitKey;

        private int [] encryptionKeyPlus;

        private int [] encryptionKeyPlusLeftPart;
        private int [] encryptionKeyPlusRightPart;

        private int [,] permutedChoiceEncryptionSubkeys;
        private int [,,] permutedChoiceEncryptionSubkeysBlocks;

        private int [,] encryptionSubkeys;

        #endregion

        #region Tekst odszyfrowany
        
        private int [,,] almostEncryptionOutputBlocksSplit;

        private int [,] almostEncryptionOutputBlocks;

        private int [,] encryptionOutputBlocks;
        
        private int [] encryptionOutput;

        #endregion


        #endregion


        


        public void ReadEncryptionInput () {

            #region Czytanie pliku wejściowego

            StreamReader streamReader = new StreamReader("encryption-input.txt");

            string encryptionInputString = streamReader.ReadLine();
            string encryptionKeyString = streamReader.ReadLine();

            #endregion

            #region Obsługa tekstu zaszyfrowanego
            
            // Sprawdzanie formatu zaszyfrowanego tekstu

            bool binaryFormat = true;
            
            for (int i=0; i<encryptionInputString.Length; i++)
                if (encryptionInputString[i]!='0' && encryptionInputString[i]!='1')
                    binaryFormat = false;

            // Jeśli format zaszyfrowanego tekstu jest bibnarny

            if (binaryFormat) {
                encryptionInput = new int [encryptionInputString.Length];
                encryptionInputBlocksNumber = encryptionInputString.Length / 64;
                encryptionInputBlocks = new int [encryptionInputBlocksNumber,64];
                encryptionInputPermutedBlocks = new int [encryptionInputBlocksNumber,64];
                encryptionInputPermutedBlocksSplit = new int [encryptionInputBlocksNumber,2,32];
                for (int i=0; i<encryptionInputBlocksNumber; i++) {
                    for (int j=0; j<64; j++) {
                        encryptionInput[64*i+j] = Int32.Parse(encryptionInputString[64*i+j].ToString());
                        encryptionInputBlocks[i,j] = Int32.Parse(encryptionInputString[64*i+j].ToString());
                    }
                }
            }

            // Jeśli format zaszyfrowanego tekstu jest heksadecymalny

            else {
                
            }

            #endregion
            
            #region Obsługa klucza szyfrowania

            encryptionKey = new int [64];

            encryptionSplitKey = new int [2,32];

            for (int i=0; i<32; i++) {
                encryptionKey[i] = Int32.Parse(encryptionKeyString[i].ToString());
                encryptionKey[i+32] = Int32.Parse(encryptionKeyString[i+32].ToString());
                encryptionSplitKey[0,i] = Int32.Parse(encryptionKeyString[i].ToString());
                encryptionSplitKey[1,i] = Int32.Parse(encryptionKeyString[i+32].ToString());
            }
            
            encryptionKeyPlus = new int [56];

            encryptionKeyPlusLeftPart = new int [28];
            encryptionKeyPlusRightPart = new int [28];
            
            permutedChoiceEncryptionSubkeys = new int [16,56];
            permutedChoiceEncryptionSubkeysBlocks = new int [16,2,28];

            encryptionSubkeys = new int [16,48];

            #endregion

            #region Obsługa tekstu odszyfrowanego
            
            almostEncryptionOutputBlocksSplit = new int [encryptionInputBlocksNumber,2,32];
            almostEncryptionOutputBlocks = new int [encryptionInputBlocksNumber,64];
            encryptionOutputBlocks = new int [encryptionInputBlocksNumber,64];
            encryptionOutput = new int [encryptionInputString.Length];

            #endregion

        }


        public void EncryptionInitialPermutation () {

            for (int i=0; i<encryptionInputBlocksNumber; i++) {
                for (int j=0; j<64; j++)
                    encryptionInputPermutedBlocks[i,j] = encryptionInputBlocks[i,initialPermutationNumbers[j]-1];
                for (int j=0; j<32; j++) {
                    encryptionInputPermutedBlocksSplit[i,0,j] = encryptionInputPermutedBlocks[i,j];
                    encryptionInputPermutedBlocksSplit[i,1,j] = encryptionInputPermutedBlocks[i,j+32];
                }
            }

        }
        

        public void GenerateEncryptionSubkeys () {

            // Ustalanie K+

            for (int i=0; i<56; i++)
                encryptionKeyPlus[i] = encryptionKey[permutedChoiceOneNumbers[i]-1];

            // Podział K+ na dwie części

            for (int i=0; i<28; i++) {
                encryptionKeyPlusLeftPart[i] = encryptionKeyPlus[i];
                encryptionKeyPlusRightPart[i] = encryptionKeyPlus[i+28];
            }

            // Stworzenie tymczasowych kluczy pomocniczych

            int [] tempLeftKey = new int [28];
            int [] tempRightKey = new int [28];
            int [] tempWholeKey = new int [56];

            // Przypisanie wartości do lewego oraz prawego klucza tymczasowego jeszcze przed rozpoczęciem iteracji

            for (int i=0; i<28; i++) {
                tempLeftKey[i] = encryptionKeyPlusLeftPart[i];
                tempRightKey[i] = encryptionKeyPlusRightPart[i];
            }
            
            // Generowanie szesnastu kluczy

            for (int i=0; i<16; i++) {

                // Realizacja aktualnej iteracji na oficjalnej tablicy kluczy

                int shift = keysTransformNumbers[i];

                for (int j=0; j<28-shift; j++) {
                    permutedChoiceEncryptionSubkeysBlocks[i,0,j] = tempLeftKey[j+shift];
                    permutedChoiceEncryptionSubkeysBlocks[i,1,j] = tempRightKey[j+shift];
                }
                for (int j=28-shift; j<28; j++) {
                    permutedChoiceEncryptionSubkeysBlocks[i,0,j] = tempLeftKey[j-(28-shift)];
                    permutedChoiceEncryptionSubkeysBlocks[i,1,j] = tempRightKey[j-(28-shift)];
                }

                // Aktualizowanie kluczy pomocniczych

                for (int j=0; j<28; j++) {
                    tempLeftKey[j] = permutedChoiceEncryptionSubkeysBlocks[i,0,j];
                    tempRightKey[j] = permutedChoiceEncryptionSubkeysBlocks[i,1,j];
                    tempWholeKey[j] = tempLeftKey[j];
                    tempWholeKey[j+28] = tempRightKey[j];
                }
                for (int j=0; j<28; j++) {
                    permutedChoiceEncryptionSubkeys[i,j] = tempLeftKey[j];
                    permutedChoiceEncryptionSubkeys[i,j+28] = tempRightKey[j];
                    permutedChoiceEncryptionSubkeysBlocks[i,0,j] = tempLeftKey[j];
                    permutedChoiceEncryptionSubkeysBlocks[i,1,j] = tempRightKey[j];
                }
            }
        }



        public void TransformEncryptionSubkeys () {
            for (int i=0; i<16; i++)
                for (int j=0; j<48; j++)
                    encryptionSubkeys[i,j] = permutedChoiceEncryptionSubkeys[i,permutedChoiceTwoNumbers[j]-1];
        }


        public void EncryptionAlgorithm () {

            for (int i=0; i<encryptionInputBlocksNumber; i++) {

                int [] currentKey = new int [48];
                int [] currentLeft = new int [32];
                int [] currentRight = new int [32];
                
                // Mejbi propabli są poczebne L i R

                for (int j=0; j<32; j++) {
                    currentLeft[j] = encryptionInputPermutedBlocks[i,j];
                    currentRight[j] = encryptionInputPermutedBlocks[i,j+32];
                }
                
                almostEncryptionOutput = new int [64];
                
                for (int j=0; j<16; j++) {
                    for (int k=0; k<48; k++)
                        currentKey[k] = encryptionSubkeys[j,k];
                    int [] tempRight = new int [32];
                    int [] extendedRight = new int [48];
                    int [] XOROne = new int [48];
                    for (int k=0; k<32; k++)
                        tempRight[k] = currentRight[k];
                    for (int k=0; k<48; k++) {
                        extendedRight[k] = tempRight[enlargementTableNumbers[k]-1];
                        bool xorOne = ((extendedRight[k]==1 && currentKey[k]==0) || (extendedRight[k]==0 && currentKey[k]==1));
                        XOROne[k] = (xorOne) ? 1 : 0;
                    }
                    int [] permutationInput = new int [32];
                    int [] permutationOutput = new int [32];
                    for (int k=0; k<8; k++) {
                        int rowIndex = 2*XOROne[k*6] + 1*XOROne[k*6+5];
                        int columnIndex = 8*XOROne[k*6+1] + 4*XOROne[k*6+2] + 2*XOROne[k*6+3] + 1*XOROne[k*6+4];
                        int intValue = S[k,rowIndex*16+columnIndex];
                        string stringValue = Convert.ToString(intValue,2);
                        int [] binaryValue = new int [4];
                        for (int l=0; l<4-stringValue.Length; l++)
                            binaryValue[l] = 0;
                        for (int l=0; l<stringValue.Length; l++)
                            binaryValue[l+(4-stringValue.Length)] = Int32.Parse(stringValue[l].ToString());
                        for (int l=0; l<4; l++)
                            permutationInput[k*4+l] = binaryValue[l];
                    }
                    for (int k=0; k<32; k++)
                        permutationOutput[k] = permutationInput[permutationNumbers[k]-1];
                    for (int k=0; k<32; k++) {
                        bool xorTwo = ((currentLeft[k]==1 && permutationOutput[k]==0) || (currentLeft[k]==0 && permutationOutput[k]==1));
                        currentRight[k] = (xorTwo) ? 1 : 0;
                    }
                    for (int k=0; k<32; k++)
                        currentLeft[k] = tempRight[k];
                }

                for (int j=0; j<32; j++) {
                    almostEncryptionOutputBlocks[i,j] = currentRight[j];
                    almostEncryptionOutputBlocks[i,j+32] = currentLeft[j];
                }

                for (int j=0; j<64; j++)
                    encryptionOutputBlocks[i,j] = almostEncryptionOutputBlocks[i,inversedInitialPermutationNumbers[j]-1];

            }

            for (int i=0; i<encryptionInputBlocksNumber; i++)
                for (int j=0; j<64; j++)
                    encryptionOutput[64*i+j] = encryptionOutputBlocks[i,j];
            
            for (int i=0; i<encryptionInputBlocksNumber; i++)
                for (int j=0; j<64; j++)
                    Console.Write(encryptionOutput[64*i+j]);


        }




        
        

        
        private int inputLength;
        private int keyLength;

        private int iterations;

        private int [] almostEncryptionOutput;
        
        private int [] key;

        private int [] keyPlus;

        private int [] keyPlusSplitLeft;
        private int [] keyPlusSplitRight;

        private int [] initialPermutedInput;

        private int [,] initialPermutedInputSplit;

        private int [,] permutedChoiceKeys;

        private int [,,] permutedChoiceKeysBlocks;

        private int [,] transformedSubkeys;


        private int [] initialPermutationNumbers = {58,50,42,34,26,18,10,2,60,52,44,36,28,20,12,4,62,54,46,38,30,22,14,6,64,56,48,40,32,24,16,8,57,49,41,33,25,17,9,1,59,51,43,35,27,19,11,3,61,53,45,37,29,21,13,5,63,55,47,39,31,23,15,7};

        private int [] permutedChoiceOneNumbers = {57,49,41,33,25,17,9,1,58,50,42,34,26,18,10,2,59,51,43,35,27,19,11,3,60,52,44,36,63,55,47,39,31,23,15,7,62,54,46,38,30,22,14,6,61,53,45,37,29,21,13,5,28,20,12,4};

        private int [] permutedChoiceTwoNumbers = {14,17,11,24,1,5,3,28,15,6,21,10,23,19,12,4,26,8,16,7,27,20,13,2,41,52,31,37,47,55,30,40,51,45,33,48,44,49,39,56,34,53,46,42,50,36,29,32};

        private int [] enlargementTableNumbers = {32,1,2,3,4,5,4,5,6,7,8,9,8,9,10,11,12,13,12,13,14,15,16,17,16,17,18,19,20,21,20,21,22,23,24,25,24,25,26,27,28,29,28,29,30,31,32,1};

        private int [] keysTransformNumbers = {1,1,2,2,2,2,2,2,1,2,2,2,2,2,2,1};

        private int [,] S = {{14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7,0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8,4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0,15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13}, /* S1 */{15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10,3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5,0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15,13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9}, /* S2 */{10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8,13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1,13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7,1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12}, /* S3 */{7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15,13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9,10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4,3,15,0,6,10,1,13,8,9,4,5,11,12,7,2,14}, /* S4 */{2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9,14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6,4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14,11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3}, /* S5 */{12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11,10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8,9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6,4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13}, /* S6 */{4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1,13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6,1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2,6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12}, /* S7 */{13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7,1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2,7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8,2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11}}; /* S8 */


        private int [] permutationNumbers = {16,7,20,21,29,12,28,17,1,15,23,26,5,18,31,10,2,8,24,14,32,27,3,9,19,13,30,6,22,11,4,25};

        private int [] inversedInitialPermutationNumbers = {40,8,48,16,56,24,64,32,39,7,47,15,55,23,63,31,38,6,46,14,54,22,62,30,37,5,45,13,53,21,61,29,36,4,44,12,52,20,60,28,35,3,43,11,51,19,59,27,34,2,42,10,50,18,58,26,33,1,41,9,49,17,57,25};

        public DES () {}

        public void ReadInput () {
            StreamReader streamReader = new StreamReader("encryption-input.txt");
            string stringInput = streamReader.ReadLine();
            string stringKey = streamReader.ReadLine();
            this.inputLength = stringInput.Length;
            this.keyLength = stringKey.Length;
            this.iterations = stringInput.Length / 64;
            this.encryptionInput = new int [inputLength];
            this.encryptionOutput = new int [inputLength];
            this.decryptionOutput = new int [inputLength];
            this.key = new int [keyLength];
            this.keyPlus = new int [56];
            this.keyPlusSplitLeft = new int [28];
            this.keyPlusSplitRight = new int [28];
            this.initialPermutedInput = new int [inputLength];
            this.initialPermutedInputSplit = new int [2,32]; // lewa część - 0, prawa część - 1
            this.permutedChoiceKeys = new int [16,56];
            this.permutedChoiceKeysBlocks = new int [16,2,28]; // 16 iteracji, 2 części (lewa - 0, prawa - 1), 28 bitów na stronę
            this.transformedSubkeys = new int [16,48]; // 16 iteracji, 48 bitów na klucz (scalone dwie części po transformacji)
            this.almostEncryptionOutput = new int [64];
            for (int i=0; i<inputLength; i++)
                encryptionInput[i] = (stringInput[i]=='1') ? 1 : 0;
            for (int i=0; i<keyLength; i++)
                key[i] = (stringKey[i]=='1') ? 1 : 0;
        }





        /* Initial Input Permutation */

        public void InitialPermutation () {
            for (int i=0; i<64; i++)
                initialPermutedInput[i] = encryptionInput[initialPermutationNumbers[i]-1];
            for (int i=0; i<32; i++) {
                initialPermutedInputSplit[0,i] = initialPermutedInput[i];
                initialPermutedInputSplit[1,i] = initialPermutedInput[i+32];
            }
            int [] xd = new int [64];
            for (int i=0; i<64; i++)
                xd[initialPermutationNumbers[i]-1] = initialPermutedInput[i];
        }



        /* Keys generation */

        public void PermutedChoice () {

            // Ustalanie K+ - keyPlus

            for (int i=0; i<56; i++)
                keyPlus[i] = key[permutedChoiceOneNumbers[i]-1];

            // Dzielenie go na dwie części

            for (int i=0; i<28; i++) {
                keyPlusSplitLeft[i] = keyPlus[i];
                keyPlusSplitRight[i] = keyPlus[i+28];
            }

            // Tymczasowy klcuz do obliczeń - lewa strona, prawa strona, całość

            int [] tempLeftKey = new int [28];
            int [] tempRightKey = new int [28];
            int [] tempWholeKey = new int [56];

            for (int i=0; i<28; i++) {
                tempLeftKey[i] = keyPlusSplitLeft[i];
                tempRightKey[i] = keyPlusSplitRight[i];
            }
            
            // Tworzenie 16 subkluczy - przesunięcie bitowe obu części, potem scalenie

            for (int i=0; i<16; i++) {
                // Tutaj jest zapisywanie aktualnej przemiany do 'oficjalnej' tablicy
                int shift = keysTransformNumbers[i];
                for (int j=0; j<28-shift; j++) {
                    permutedChoiceKeysBlocks[i,0,j] = tempLeftKey[j+shift];
                    permutedChoiceKeysBlocks[i,1,j] = tempRightKey[j+shift];
                }
                for (int j=28-shift; j<28; j++) {
                    permutedChoiceKeysBlocks[i,0,j] = tempLeftKey[j-(28-shift)];
                    permutedChoiceKeysBlocks[i,1,j] = tempRightKey[j-(28-shift)];
                }
                // A tutaj aktualizowanie kluczy tymczasowych (tych pomocniczych do obliczeń)
                for (int j=0; j<28; j++) {
                    tempLeftKey[j] = permutedChoiceKeysBlocks[i,0,j];
                    tempRightKey[j] = permutedChoiceKeysBlocks[i,1,j];
                    tempWholeKey[j] = tempLeftKey[j];
                    tempWholeKey[j+28] = tempRightKey[j];
                }
                for (int j=0; j<28; j++) {
                    permutedChoiceKeys[i,j] = tempLeftKey[j];
                    permutedChoiceKeys[i,j+28] = tempRightKey[j];
                    permutedChoiceKeysBlocks[i,0,j] = tempLeftKey[j];
                    permutedChoiceKeysBlocks[i,1,j] = tempRightKey[j];
                }
            }
        }



        // Chuj wie, jak to nazwać, ale to jest część z przesunięciami tych kluczy. Jeszcze nie wiem, po co to robić, ale chyba trzeba

        public void TransformSubkeys () {
            for (int i=0; i<16; i++)
                for (int j=0; j<48; j++)
                    transformedSubkeys[i,j] = permutedChoiceKeys[i,permutedChoiceTwoNumbers[j]-1];
        }




        public void XORowanko () {
            Console.Clear();
            int [] currentKey = new int [48];
            int [] currentLeft = new int [32];
            int [] currentRight = new int [32];
            
            // Mejbi propabli są poczebne L i R

            for (int i=0; i<32; i++) {
                currentLeft[i] = initialPermutedInput[i];
                currentRight[i] = initialPermutedInput[i+32];
            }
            
            almostEncryptionOutput = new int [64];
            
            for (int i=0; i<16; i++) {
                for (int j=0; j<48; j++)
                    currentKey[j] = transformedSubkeys[i,j];
                int [] tempRight = new int [32];
                int [] extendedRight = new int [48];
                int [] XOROne = new int [48];
                for (int j=0; j<32; j++)
                    tempRight[j] = currentRight[j];
                for (int j=0; j<48; j++) {
                    extendedRight[j] = tempRight[enlargementTableNumbers[j]-1];
                    bool xorOne = ((extendedRight[j]==1 && currentKey[j]==0) || (extendedRight[j]==0 && currentKey[j]==1));
                    XOROne[j] = (xorOne) ? 1 : 0;
                }
                int [] permutationInput = new int [32];
                int [] permutationOutput = new int [32];
                for (int j=0; j<8; j++) {
                    int rowIndex = 2*XOROne[j*6] + 1*XOROne[j*6+5];
                    int columnIndex = 8*XOROne[j*6+1] + 4*XOROne[j*6+2] + 2*XOROne[j*6+3] + 1*XOROne[j*6+4];
                    int intValue = S[j,rowIndex*16+columnIndex];
                    string stringValue = Convert.ToString(intValue,2);
                    int [] binaryValue = new int [4];
                    for (int k=0; k<4-stringValue.Length; k++)
                        binaryValue[k] = 0;
                    for (int k=0; k<stringValue.Length; k++)
                        binaryValue[k+(4-stringValue.Length)] = Int32.Parse(stringValue[k].ToString());
                    for (int k=0; k<4; k++)
                        permutationInput[j*4+k] = binaryValue[k];
                }
                for (int j=0; j<32; j++)
                    permutationOutput[j] = permutationInput[permutationNumbers[j]-1];
                for (int j=0; j<32; j++) {
                    bool xorTwo = ((currentLeft[j]==1 && permutationOutput[j]==0) || (currentLeft[j]==0 && permutationOutput[j]==1));
                    currentRight[j] = (xorTwo) ? 1 : 0;
                }
                for (int j=0; j<32; j++)
                    currentLeft[j] = tempRight[j];

                


                /*Console.Write("L"+(i+1)+" - ");
                for (int j=0; j<32; j++)
                    Console.Write(currentLeft[j]);
                Console.WriteLine();
                Console.Write("R"+(i+1)+" - ");
                for (int j=0; j<32; j++)
                    Console.Write(currentRight[j]);
                Console.WriteLine();
                */
            }

            for (int i=0; i<32; i++) {
                almostEncryptionOutput[i] = currentRight[i];
                almostEncryptionOutput[i+32] = currentLeft[i];
            }

            for (int i=0; i<64; i++) {
                encryptionOutput[i] = almostEncryptionOutput[inversedInitialPermutationNumbers[i]-1];
                Console.Write(encryptionOutput[i]);
            }

        }






        #region Dane potrzebne do przeprowadzenia algorytmu deszyfrowania

        #region Tekst zaszyfrowany

        private int [] decryptionInput;

        private int decryptionInputBlocksNumber;

        private int [,] decryptionInputBlocks;

        #endregion

        #region Klucz

        private int [] decryptionKey;

        private int [,] decryptionSplitKey;

        private int [] decryptionKeyPlus;

        private int [] decryptionKeyPlusLeftPart;
        private int [] decryptionKeyPlusRightPart;

        private int [,] permutedChoiceDecryptionSubkeys;
        private int [,,] permutedChoiceDecryptionSubkeysBlocks;

        private int [,] decryptionSubkeys;

        #endregion

        #region Tekst odszyfrowany
        
        private int [,,] almostDecryptionOutputBlocksSplit;

        private int [,] almostDecryptionOutputBlocks;

        private int [,] decryptionOutputBlocks;
        
        private int [] decryptionOutput;

        #endregion


        #endregion


        


        public void ReadDecryptionInput () {

            #region Czytanie pliku wejściowego

            StreamReader streamReader = new StreamReader("decryption-input.txt");

            string decryptionInputString = streamReader.ReadLine();
            string decryptionKeyString = streamReader.ReadLine();

            #endregion

            #region Obsługa tekstu zaszyfrowanego
            
            // Sprawdzanie formatu zaszyfrowanego tekstu

            bool binaryFormat = true;
            
            for (int i=0; i<decryptionInputString.Length; i++)
                if (decryptionInputString[i]!='0' && decryptionInputString[i]!='1')
                    binaryFormat = false;

            // Jeśli format zaszyfrowanego tekstu jest bibnarny

            if (binaryFormat) {
                decryptionInput = new int [decryptionInputString.Length];
                decryptionInputBlocksNumber = decryptionInputString.Length / 64;
                decryptionInputBlocks = new int [decryptionInputBlocksNumber,64];
                for (int i=0; i<decryptionInputBlocksNumber; i++) {
                    for (int j=0; j<64; j++) {
                        decryptionInput[64*i+j] = Int32.Parse(decryptionInputString[64*i+j].ToString());
                        decryptionInputBlocks[i,j] = Int32.Parse(decryptionInputString[64*i+j].ToString());
                    }
                }
            }

            // Jeśli format zaszyfrowanego tekstu jest heksadecymalny

            else {
                
            }

            #endregion
            
            #region Obsługa klucza szyfrowania

            decryptionKey = new int [64];

            for (int i=0; i<32; i++) {
                decryptionKey[i] = Int32.Parse(decryptionKeyString[i].ToString());
                decryptionKey[i+32] = Int32.Parse(decryptionKeyString[i+32].ToString());
                decryptionSplitKey[0,i] = Int32.Parse(decryptionKeyString[i].ToString());
                decryptionSplitKey[1,i] = Int32.Parse(decryptionKeyString[i+32].ToString());
            }
            
            decryptionKeyPlus = new int [56];

            decryptionKeyPlusLeftPart = new int [28];
            decryptionKeyPlusRightPart = new int [28];
            
            permutedChoiceDecryptionSubkeys = new int [16,56];
            permutedChoiceDecryptionSubkeysBlocks = new int [16,2,28];

            decryptionSubkeys = new int [16,48];

            #endregion

            #region Obsługa tekstu odszyfrowanego
            
            almostDecryptionOutputBlocksSplit = new int [decryptionInputBlocksNumber,2,32];
            almostDecryptionOutputBlocks = new int [decryptionInputBlocksNumber,64];
            decryptionOutputBlocks = new int [decryptionInputBlocksNumber,64];
            decryptionOutput = new int [decryptionInputString.Length];
            #endregion

        }
        

        public void GenerateDecryptionSubkeys () {

            // Ustalanie K+

            for (int i=0; i<56; i++)
                decryptionKeyPlus[i] = decryptionKey[permutedChoiceOneNumbers[i]-1];

            // Podział K+ na dwie części

            for (int i=0; i<28; i++) {
                decryptionKeyPlusLeftPart[i] = decryptionKeyPlus[i];
                decryptionKeyPlusRightPart[i] = decryptionKeyPlus[i+28];
            }

            // Stworzenie tymczasowych kluczy pomocniczych

            int [] tempLeftKey = new int [28];
            int [] tempRightKey = new int [28];
            int [] tempWholeKey = new int [56];

            // Przypisanie wartości do lewego oraz prawego klucza tymczasowego jeszcze przed rozpoczęciem iteracji

            for (int i=0; i<28; i++) {
                tempLeftKey[i] = decryptionKeyPlusLeftPart[i];
                tempRightKey[i] = decryptionKeyPlusRightPart[i];
            }
            
            // Generowanie szesnastu kluczy

            for (int i=0; i<16; i++) {

                // Realizacja aktualnej iteracji na oficjalnej tablicy kluczy

                int shift = keysTransformNumbers[i];

                for (int j=0; j<28-shift; j++) {
                    permutedChoiceDecryptionSubkeysBlocks[i,0,j] = tempLeftKey[j+shift];
                    permutedChoiceDecryptionSubkeysBlocks[i,1,j] = tempRightKey[j+shift];
                }
                for (int j=28-shift; j<28; j++) {
                    permutedChoiceDecryptionSubkeysBlocks[i,0,j] = tempLeftKey[j-(28-shift)];
                    permutedChoiceDecryptionSubkeysBlocks[i,1,j] = tempRightKey[j-(28-shift)];
                }

                // Aktualizowanie kluczy pomocniczych

                for (int j=0; j<28; j++) {
                    tempLeftKey[j] = permutedChoiceDecryptionSubkeysBlocks[i,0,j];
                    tempRightKey[j] = permutedChoiceDecryptionSubkeysBlocks[i,1,j];
                    tempWholeKey[j] = tempLeftKey[j];
                    tempWholeKey[j+28] = tempRightKey[j];
                }
                for (int j=0; j<28; j++) {
                    permutedChoiceDecryptionSubkeys[i,j] = tempLeftKey[j];
                    permutedChoiceDecryptionSubkeys[i,j+28] = tempRightKey[j];
                    permutedChoiceDecryptionSubkeysBlocks[i,0,j] = tempLeftKey[j];
                    permutedChoiceDecryptionSubkeysBlocks[i,1,j] = tempRightKey[j];
                }
            }
        }


        public void InverseSubkeysIterations () {

        }


        public void ConnectDecryptionOutput () {

            // Łączenie obu stron poszczególnych bloków
            
            for (int i=0; i<decryptionInputBlocksNumber; i++) {
                for (int j=0; j<32; j++) {
                    almostDecryptionOutputBlocks[i,j] = almostDecryptionOutputBlocksSplit[i,0,j];
                    almostDecryptionOutputBlocks[i,j+32] = almostDecryptionOutputBlocksSplit[i,1,j];
                }
            }

        }

        public void DenyInitialPermutation () {

            // Odwracanie permutacji początkowej na poszczególnych blokach

            for (int i=0; i<decryptionInputBlocksNumber; i++)
                for (int j=0; j<64; j++)
                    decryptionOutputBlocks[i,initialPermutationNumbers[j]-1] = almostDecryptionOutputBlocks[i,j];

            // Synteza bloków odszyfrowanego tekstu jawnego

            for (int i=0; i<decryptionInputBlocksNumber; i++)
                for (int j=0; j<64; j++)
                    decryptionOutput[64*i+j] = decryptionOutputBlocks[i,j];

        }


        private void DisplayDecryptionResult () {
            Console.Clear();
            Console.WriteLine("   Etap drugi - deszyfrowanie\n\n");
            Console.Write("      Zaszyfrowane wyjście: ");
            for (int i=0; i<decryptionInput.Length; i++)
                Console.Write(decryptionInput[i]);
            Console.WriteLine("\n");
            Console.Write("      Klucz: ");
            for (int i=0; i<64; i++)
                Console.Write(decryptionKey[i]);
            Console.WriteLine("\n");
            Console.Write("      Odszyfrowane wejście: ");
            for (int i=0; i<decryptionOutput.Length; i++)
                Console.Write(decryptionOutput[i]);
            Console.WriteLine("\n\n\n");
            Console.Write("   Wciśnij dowolny klawisz, aby zakończyć działanie programu.");
        }


        
    }
}