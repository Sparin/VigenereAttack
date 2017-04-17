using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VigenereAttack
{
    class Program
    {
        private const string Alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя_";
        //private const string Alphabet = "abcdefghijklmnopqrstuvwxyz_";
        static void Main(string[] args)
        {
            Vigenere vigenere = new Vigenere(Alphabet);

            #region Plain text encryption
            ////Replace all non alphabetic letters to whitespace
            ////All letters to lower register
            ////Replace double whitespace to one once
            //char[] plainText = Regex.Replace(File.ReadAllText("plaintext.txt").ToLower(), "[^" + Alphabet + "]", " ").Replace("  ", " ").ToCharArray();

            //vigenere.Encrypt(ref plainText, "doomsday");
            //Console.WriteLine(plainText);
            #endregion

            VigenereAttack vigenereAttack = new VigenereAttack(Alphabet, File.ReadAllText("ciphertext.txt").ToLower());

            #region Bruteforce key lengtn
            int maxKeyLenght = 0;
            while (maxKeyLenght <= 0 || maxKeyLenght > 100)
            {
                Console.Write("Print max lenght of key for bruteforce: ");
                maxKeyLenght = int.Parse(Console.ReadLine());
            }
            Console.WriteLine("Length\t\tIndex of Coincidence");
            for (int i = 1; i <= maxKeyLenght; i++)
            {
                double index = vigenereAttack.GetIndexOfCoincidence(i);
                Console.WriteLine("{0}\t\t{1}", i, index);
            }
            #endregion

            #region Char frequency analysis
            int keyLength = 0;
            while (keyLength < 1 || keyLength > 100)
            {
                Console.Write("Print length of key: ");
                keyLength = int.Parse(Console.ReadLine());
            }


            while (true)
            {
                Console.Write("CipherRow\tshift/letter from ");
                int startIndex = Alphabet.IndexOf(Console.ReadLine()[0]) + 1;
                for (int i = 0; i < keyLength; i++)
                {
                    int[] charFrequency = vigenereAttack.GetCharFrequency(i, keyLength);
                    int shift = vigenereAttack.GetMaxIndexOfCharFrequency(charFrequency);
                    Console.WriteLine("{0}\t\t{1}\t{2}", i + 1, shift + startIndex, Alphabet[(shift + startIndex) % Alphabet.Length]);
                }

                Console.Write("Continue (y/n)... ");
                if ("n" == Console.ReadLine())
                    break;
            }
            #endregion

            #region Cipher text decryption
            string key = string.Empty;
            while (key == string.Empty)
            {
                Console.Write("Print key for decryption of ciphertext: ");
                key = Console.ReadLine();
            }
            char[] ciphertext = vigenereAttack.ChiperText.ToCharArray();
            vigenere.Decrypt(ref ciphertext, key);
            Console.WriteLine(ciphertext);
            #endregion
        }
    }

    public class Vigenere
    {
        private string alphabet = string.Empty;
        public string Alphabet
        {
            get { return alphabet; }
            set
            {
                if (value.Length == 0)
                    throw new Exception("Alphabet length must be greater than zero");
                else alphabet = value;
            }
        }

        public Vigenere(string alphabet)
        {
            Alphabet = alphabet;
        }

        public void Encrypt(ref char[] plainText, string key)
        {
            for (int i = 0; i < plainText.Length; i++)
                plainText[i] = Alphabet[(Alphabet.IndexOf(plainText[i]) + Alphabet.IndexOf(key[i % key.Length])) % Alphabet.Length];
        }

        public void Decrypt(ref char[] chiperText, string key)
        {
            for (int i = 0; i < chiperText.Length; i++)
            {
                int index = Alphabet.IndexOf(chiperText[i]) - Alphabet.IndexOf(key[i % key.Length]) % Alphabet.Length;
                if (index < 0)
                    index = Alphabet.Length + index;
                chiperText[i] = Alphabet[index];
            }
        }
    }

    public class VigenereAttack
    {
        private string alphabet = string.Empty;
        public string Alphabet
        {
            get { return alphabet; }
            set
            {
                if (value.Length == 0)
                    throw new Exception("Alphabet length must be greater than zero");
                else alphabet = value;
            }
        }

        private string chiperText = string.Empty;
        public string ChiperText
        {
            get { return chiperText; }
            set
            {
                if (value.Length == 0)
                    throw new Exception("Chiper text length must be greater than zero");
                else chiperText = value;
            }
        }

        public VigenereAttack(string alphabet, string chiperText)
        {
            Alphabet = alphabet;
            ChiperText = chiperText;
        }

        /// <summary>
        /// Gets letter frequency for nth char in ciphertext
        /// </summary>
        /// <param name="offsetIndex"></param>
        /// <param name="nthChild"></param>
        /// <returns></returns>
        public int[] GetCharFrequency(int offsetIndex, int nthChild)
        {
            int[] result = new int[Alphabet.Length];
            for (int i = offsetIndex; i < ChiperText.Length; i += nthChild)
                result[Alphabet.IndexOf(ChiperText[i])]++;
            return result;
        }

        public int GetMaxIndexOfCharFrequency(int[] charFrequency)
        {
            int maxIndex = 0;
            for (int i = 0; i < charFrequency.Length; i++)
                if (charFrequency[maxIndex] < charFrequency[i])
                    maxIndex = i;
            return maxIndex;
        }

        public double GetIndexOfCoincidence(int nthChild)
        {
            double[] indexesOfCoincidence = new double[Alphabet.Length];
            int[] charFrequency = GetCharFrequency(0, nthChild);

            //IoC_i = (n(n-1)/(L(L-1)
            for (int i = 0; i < indexesOfCoincidence.Length; i++)
                indexesOfCoincidence[i] = (double)charFrequency[i] * (charFrequency[i] - 1) / (charFrequency.Sum() * (charFrequency.Sum() - 1));

            return indexesOfCoincidence.Sum();
        }
    }
}