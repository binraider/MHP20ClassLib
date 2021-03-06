using System;
using System.Security.Cryptography;
using System.Data;
using System.Configuration;
using System.Web;
using System.Messaging;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Text;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Collections.Generic;

namespace MHP20ClassLib {
    public class PasswordGenerator {

        /*
 * 
 * ومن فرعها في دبي، تقدم M: حالياً خدماتها لعملائها في المملكة العربية السعودية ومصر والعراق وأبوظبي والشارقة ورأس الخيمة. كما قدمت الشركة خدماتها خلال السنوات الأخيرة في الأراضي الفلسطينية وسوريا وعُمان. ولتعزيز ورفع مستوى عروض خدماتها الأساسية، تتمتع الشركة بعلاقات متميزة مع عدد من الشركاء المحليين في أنحاء منطقة الخليج، وبالتحديد في كل من البحرين وقطر والمملكة العربية السعودية ومصر والكويت. 

 * */

        public PasswordGenerator() { 
        
        }

        public string CreatePassword(){
            Password password = new Password(false, true, true, true);
            password.MinimumLength = 8;
            password.MaximumLength = 8;
            return password.Create();
        }

        public string CreatePassword(int minlen, int maxlen) {
            Password password = new Password(false, true, true, true);
            password.MinimumLength = minlen;
            password.MaximumLength = maxlen;
            return password.Create();
        }

    }

    public class Password {
        private static readonly char[] _Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] _Numbers = "1234567890".ToCharArray();
        private static readonly char[] _Symbols = "!@#$%^&*.?".ToCharArray();

        int _MinimumLength, _MaximumLength;
        bool _IncludeUpper, _IncludeLower, _IncludeNumber, _IncludeSpecial;

        string[] _CharacterTypes;

        enum CharacterType {
            Uppercase,
            Lowercase,
            Special,
            Number
        }

        public bool IncludeUpper {
            get {
                return _IncludeUpper;
            }
            set {
                _IncludeUpper = value;
            }
        }

        public bool IncludeLower {
            get {
                return _IncludeLower;
            }
            set {
                _IncludeLower = value;
            }
        }

        public bool IncludeNumber {
            get {
                return _IncludeNumber;
            }
            set {
                _IncludeNumber = value;
            }
        }

        public bool IncludeSpecial {
            get {
                return _IncludeSpecial;
            }
            set {
                _IncludeSpecial = value;
            }
        }

        public int MinimumLength {
            get {
                return _MinimumLength;
            }
            set {
                if (value > _MaximumLength) {
                    throw new ArgumentOutOfRangeException("MinimumLength must be greater than MaximumLength");
                }
                _MinimumLength = value;
            }
        }

        public int MaximumLength {
            get {
                return _MaximumLength;
            }
            set {
                if (value < _MinimumLength) {
                    throw new ArgumentOutOfRangeException("MaximumLength must be greater than MinimumLength");
                }
                _MaximumLength = value;
            }
        }

        public Password() {
            _MinimumLength = 6;
            _MaximumLength = 20;
            _IncludeSpecial = true;
            _IncludeNumber = true;
            _IncludeUpper = true;
            _IncludeLower = true;
        }

        public Password(bool includeSpecial, bool includeNumber, bool includeUpper, bool includeLower)
            : this() {
            _IncludeNumber = includeNumber;
            _IncludeSpecial = includeSpecial;
            _IncludeUpper = includeUpper;
            _IncludeLower = includeLower;
        }

        /// <summary>
        /// Randomly creates a password.
        /// </summary>
        /// <returns>A random string of characters.</returns>
        public string Create() {
            _CharacterTypes = getCharacterTypes();

            StringBuilder password = new StringBuilder(_MaximumLength);

            //Get a random length for the password.
            int currentPasswordLength = RandomNumber.Next(_MaximumLength);

            //Only allow for passwords greater than or equal to the minimum length.
            if (currentPasswordLength < _MinimumLength) {
                currentPasswordLength = _MinimumLength;
            }

            //Generate the password
            for (int i = 0; i < currentPasswordLength; i++) {
                password.Append(getCharacter());
            }

            return password.ToString();
        }

        /// <summary>
        /// Determines which character types should be used to generate
        /// the current password.
        /// </summary>
        /// <returns>A string[] of character that should be used to generate the current password.</returns>
        private string[] getCharacterTypes() {
            ArrayList characterTypes = new ArrayList();
            foreach (string characterType in Enum.GetNames(typeof(CharacterType))) {
                CharacterType currentType = (CharacterType)Enum.Parse(typeof(CharacterType), characterType, false);
                bool addType = false;
                switch (currentType) {
                    case CharacterType.Lowercase:
                        addType = IncludeLower;
                        break;
                    case CharacterType.Number:
                        addType = IncludeNumber;
                        break;
                    case CharacterType.Special:
                        addType = IncludeSpecial;
                        break;
                    case CharacterType.Uppercase:
                        addType = IncludeUpper;
                        break;
                }
                if (addType) {
                    characterTypes.Add(characterType);
                }
            }
            return (string[])characterTypes.ToArray(typeof(string));
        }

        /// <summary>
        /// Randomly determines a character type to return from the 
        /// available CharacterType enum.
        /// </summary>
        /// <returns>The string character to append to the password.</returns>
        private string getCharacter() {
            string characterType = _CharacterTypes[RandomNumber.Next(_CharacterTypes.Length)];
            CharacterType typeToGet = (CharacterType)Enum.Parse(typeof(CharacterType), characterType, false);
            switch (typeToGet) {
                case CharacterType.Lowercase:
                    return _Letters[RandomNumber.Next(_Letters.Length)].ToString().ToLower();
                case CharacterType.Uppercase:
                    return _Letters[RandomNumber.Next(_Letters.Length)].ToString().ToUpper();
                case CharacterType.Number:
                    return _Numbers[RandomNumber.Next(_Numbers.Length)].ToString();
                case CharacterType.Special:
                    return _Symbols[RandomNumber.Next(_Symbols.Length)].ToString();
            }
            return null;
        }
    }

    public sealed class RandomNumber {
        private static RNGCryptoServiceProvider _Random = new RNGCryptoServiceProvider();
        private static byte[] bytes = new byte[4];

        private RandomNumber() { }

        public static int Next(int max) {
            if (max <= 0) {
                throw new ArgumentOutOfRangeException("max");
            }
            _Random.GetBytes(bytes);
            int value = BitConverter.ToInt32(bytes, 0) % max;
            if (value < 0) {
                value = -value;
            }
            return value;
        }
    }
}
