using Model.PublicModels;
using System.Text;

namespace MWSMan.Core.Persistence.Security
{

    public  class MinooSecurity
    {
        public MinooSecurity()
        {
            InternalKey = this.DefaultKey;
            MinooKey = "_M_I_N_O_O_";
        }
        public readonly string InternalKey;
        public string MinooKey { get; }

        public  string Encode(string myString, string myKey)
        {
            bool flag = string.CompareOrdinal(this.DefaultKey, myKey) != 0;
            if (string.IsNullOrEmpty(myString) || string.IsNullOrEmpty(myKey) & flag)
                return (string)null;
            byte[] hashCodeTable = flag ? this.GetHashCodeTable(myKey) : (byte[])null;
            Random random = new Random((int)DockerStaticDateTime.NowDateTime.TimeOfDay.TotalSeconds);
            byte number1 = (byte)random.Next(1, (int)byte.MaxValue);
            EncodeType number2 = (EncodeType)random.Next(0, 8);
            string empty = string.Empty;
            int num1 = 0;
            for (int count = 1; count <= myString.Length; ++count)
            {
                byte number3 = (byte)myString[count - 1];
                byte number4 = (number2 & EncodeType.FirstRotate) == EncodeType.Min ? this.Rotate((byte)((uint)number3 ^ (uint)number1), count, (number2 & EncodeType.RotateRight) != 0) : (byte)((uint)this.Rotate(number3, count, (number2 & EncodeType.RotateRight) != 0) ^ (uint)number1);
                if (flag)
                {
                    byte number5 = hashCodeTable[count % hashCodeTable.Length];
                    byte num2 = (number2 & EncodeType.FirstRotate) == EncodeType.Min ? this.Rotate((byte)((uint)number5 ^ (uint)number1), count, (number2 & EncodeType.RotateRight) != 0) : (byte)((uint)this.Rotate(number5, count, (number2 & EncodeType.RotateRight) != 0) ^ (uint)number1);
                    number4 ^= num2;
                }
                num1 += (int)number4;
                empty += this.HexOf(number4);
            }
            string str1 = flag ? this.HexOf((byte)(num1 & (int)byte.MaxValue ^ 170)) : this.HexOf((byte)(num1 & (int)byte.MaxValue ^ (int)byte.MaxValue));
            string str2 = this.HexOf((byte)number2);
            string str3 = this.HexOf(number1);
            return (number2 & EncodeType.KeyAtStart) == EncodeType.Min ? str2 + empty + str3 + str1 : str2 + str3 + str1 + empty;
        }

        public  string Encode(string myString) => this.Encode(myString, this.InternalKey);

        public  string Decode(string myString, string myKey)
        {
            bool flag = string.CompareOrdinal(this.DefaultKey, myKey) != 0;
            if (string.IsNullOrEmpty(myString) || string.IsNullOrEmpty(myKey) & flag)
                return (string)null;
            EncodeType encodeType = (EncodeType)this.ByteOf(myString.Substring(0, 2));
            int startIndex1;
            int startIndex2;
            int startIndex3;
            if ((encodeType & EncodeType.KeyAtStart) != EncodeType.Min)
            {
                startIndex1 = 2;
                startIndex2 = 4;
                startIndex3 = 6;
            }
            else
            {
                startIndex1 = myString.Length - 4;
                startIndex2 = myString.Length - 2;
                startIndex3 = 2;
            }
            int num1 = (int)this.ByteOf(myString.Substring(startIndex2, 2));
            byte[] hashCodeTable = flag ? this.GetHashCodeTable(myKey) : (byte[])null;
            string str1 = myString.Substring(startIndex3, myString.Length - 6);
            byte num2 = this.ByteOf(myString.Substring(startIndex1, 2));
            string str2 = string.Empty;
            int num3 = 0;
            for (int count = 1; count <= str1.Length >> 1; ++count)
            {
                byte number1 = this.ByteOf(str1.Substring(count - 1 << 1, 2));
                num3 += (int)number1;
                if (flag)
                {
                    byte number2 = hashCodeTable[count % hashCodeTable.Length];
                    byte num4 = (encodeType & EncodeType.FirstRotate) == EncodeType.Min ? this.Rotate((byte)((uint)number2 ^ (uint)num2), count, (encodeType & EncodeType.RotateRight) != 0) : (byte)((uint)this.Rotate(number2, count, (encodeType & EncodeType.RotateRight) != 0) ^ (uint)num2);
                    number1 ^= num4;
                }
                byte num5 = (encodeType & EncodeType.FirstRotate) == EncodeType.Min ? (byte)((uint)this.Rotate(number1, count, (encodeType & EncodeType.RotateRight) == EncodeType.Min) ^ (uint)num2) : this.Rotate((byte)((uint)number1 ^ (uint)num2), count, (encodeType & EncodeType.RotateRight) == EncodeType.Min);
                str2 += ((char)num5).ToString();
            }
            if (flag)
            {
                if ((num3 & (int)byte.MaxValue ^ num1) != 170)
                    str2 = (string)null;
            }
            else if ((num3 & (int)byte.MaxValue ^ num1) != (int)byte.MaxValue)
                str2 = (string)null;
            return str2;
        }

        public  string Decode(string myString) => this.Decode(myString, this.InternalKey);

        public  bool Compare(
          string my1stString,
          string my1stKey,
          string my2ndString,
          string my2ndKey)
        {
            try
            {
                if (!string.IsNullOrEmpty(my1stString))
                {
                    my1stKey = my1stKey.Replace("-", "");
                    my1stKey = this.Decode(my1stKey, this.AlternateKey);
                    if (!string.IsNullOrEmpty(my1stKey))
                    {
                        my1stString = my1stString.Replace("-", "");
                        my1stString = this.Decode(my1stString, my1stKey);
                    }
                }
                if (!string.IsNullOrEmpty(my2ndString))
                {
                    my2ndKey = my2ndKey.Replace("-", "");
                    my2ndKey = this.Decode(my2ndKey, this.AlternateKey);
                    if (!string.IsNullOrEmpty(my2ndKey))
                    {
                        my2ndString = my2ndString.Replace("-", "");
                        my2ndString = this.Decode(my2ndString, my2ndKey);
                    }
                }
                return !string.IsNullOrEmpty(my1stString) && !string.IsNullOrEmpty(my2ndString) && string.CompareOrdinal(my1stString, my2ndString) == 0;
            }
            catch
            {
                return false;
            }
        }

        public  bool Compare(string my1stString, string my2ndString, string myKey)
        {
            return this.Compare(my1stString, myKey, my2ndString, myKey);
        }

        public  bool Compare(string my1stString, string my2ndString)
        {
            return my1stString == my2ndString || this.Compare(my1stString, my2ndString, this.Encode(this.DefaultKey, this.AlternateKey));
        }

        private  byte Rotate(byte number, int count, bool toRight)
        {
            count %= 8;
            if (toRight)
            {
                for (; count > 0; --count)
                {
                    if (((int)number & 1) != 0)
                        number = (byte)(128 | (int)number >> 1);
                    else
                        number >>= 1;
                }
            }
            else
            {
                for (; count > 0; --count)
                    number = ((int)number & 128) == 0 ? (byte)((int)number << 1 & (int)byte.MaxValue) : (byte)(((int)number << 1 | 1) & (int)byte.MaxValue);
            }
            return number;
        }

        private  string HexOf(byte number)
        {
            string str = string.Format("{0:X}", (object)number);
            if (str.Length < 2)
                str = "0" + str;
            return str;
        }

        private  byte ByteOf(string number)
        {
            try
            {
                return (byte)Convert.ToInt16(number, 16);
            }
            catch
            {
                return 0;
            }
        }

        private  byte[] GetHashCodeTable(string key)
        {
            if (string.IsNullOrEmpty(key))
                return (byte[])null;
            if (key.Length < 5)
                key = (key + "\u0001\u0002\u0003\u0004").Substring(0, 5);
            int length1 = key.Length;
            double num1 = 1.0 + (1.0 + (double)length1) / (1.0 + (double)length1 + (double)(length1 * length1));
            byte[] bytes = Encoding.Unicode.GetBytes(key);
            int length2 = bytes.Length >> 1;
            int[] numArray = new int[length2];
            for (int index = 0; index < length2; ++index)
                numArray[index] = Convert.ToInt32(bytes[index << 1]) + (Convert.ToInt32(bytes[(index << 1) + 1]) << 8);
            for (int index = 64; index < length2; ++index)
                numArray[index % 64] += numArray[index];
            if (length2 > 64)
                length2 = 64;
            double num2 = 0.0;
            double num3 = 1.0;
            for (int index = 0; index < length2; ++index)
            {
                num3 *= num1;
                double num4 = Convert.ToDouble(numArray[index]) * num3;
                num2 += num4;
            }
            return BitConverter.GetBytes(num2);
        }

        private  string DefaultKey
        {
            get
            {
                int num1 = 19;
                string str1 = "";
                char ch1 = (char)(num1 * 5);
                int num2 = 4 << 3;
                char ch2 = (char)num2;
                string str2 = str1 + ch2.ToString() + ch1.ToString();
                int num3 = num2 + 45;
                char ch3 = (char)num3;
                string str3 = str2 + ch3.ToString() + ch1.ToString();
                int num4 = num3 - 4;
                char ch4 = (char)num4;
                string str4 = str3 + ch4.ToString() + ch1.ToString();
                int num5 = num4 + 5;
                char ch5 = (char)num5;
                string str5 = str4 + ch5.ToString() + ch1.ToString();
                int num6 = num5 + 1;
                char ch6 = (char)num6;
                return str5 + ch6.ToString() + ch1.ToString() + ((char)num6).ToString() + ch1.ToString() + ((char)(4 << 3)).ToString();
            }
        }

        private  string AlternateKey
        {
            get
            {
                int num1 = 11;
                string str = "" + ((char)(num1 * num1 - 10)).ToString();
                int num2 = 11;
                int num3;
                char ch = (char)(num3 = num2 * num2 - 10);
                int num4;
                return str + ch.ToString() + ((char)(num4 = num3 - 1)).ToString() + ((char)(num4 - 5)).ToString() + ((char)(11 * 7)).ToString();
            }
        }
    }

}

