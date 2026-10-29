using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RateController.Classes
{
    public class clsQrCode
    {
        // minimal QR encoder for the phone switches URL
        // fixed: version 3 (29x29), error correction M, byte mode, mask 0
        // capacity 42 bytes, longest URL "http://255.255.255.255:65535" is 28

        private const int DataCodewords = 44;
        private const int EccCodewords = 26;
        private const int FormatBits = 0x5412;     // EC level M + mask 0, BCH and XOR mask applied
        private const int MaxBytes = 42;
        private const int QuietZone = 4;
        private const int Size = 29;

        private bool[,] IsFunction = new bool[Size, Size];
        private bool[,] Modules = new bool[Size, Size];

        private clsQrCode()
        { }

        public static Bitmap Create(string Text, int Scale)
        {
            Bitmap Result = null;
            byte[] Data = Encoding.UTF8.GetBytes(Text);
            if (Data.Length <= MaxBytes && Scale > 0)
            {
                clsQrCode Qr = new clsQrCode();
                Qr.Encode(Data);

                int Width = (Size + QuietZone * 2) * Scale;
                Result = new Bitmap(Width, Width);
                using (Graphics g = Graphics.FromImage(Result))
                {
                    g.Clear(Color.White);
                    for (int y = 0; y < Size; y++)
                    {
                        for (int x = 0; x < Size; x++)
                        {
                            if (Qr.Modules[y, x]) g.FillRectangle(Brushes.Black, (x + QuietZone) * Scale, (y + QuietZone) * Scale, Scale, Scale);
                        }
                    }
                }
            }
            return Result;
        }

        private static void AppendBits(List<bool> Bits, int Value, int Count)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                Bits.Add(((Value >> i) & 1) != 0);
            }
        }

        private static byte[] BuildCodewords(byte[] Data)
        {
            List<bool> Bits = new List<bool>();
            AppendBits(Bits, 0x4, 4);               // byte mode
            AppendBits(Bits, Data.Length, 8);       // character count
            foreach (byte b in Data) AppendBits(Bits, b, 8);

            AppendBits(Bits, 0, Math.Min(4, DataCodewords * 8 - Bits.Count));     // terminator
            AppendBits(Bits, 0, (8 - Bits.Count % 8) % 8);                        // byte align

            byte[] Result = new byte[DataCodewords + EccCodewords];
            for (int i = 0; i < Bits.Count; i++)
            {
                if (Bits[i]) Result[i >> 3] |= (byte)(0x80 >> (i & 7));
            }

            // pad codewords alternate 0xEC, 0x11
            int Pad = 0xEC;
            for (int i = Bits.Count / 8; i < DataCodewords; i++)
            {
                Result[i] = (byte)Pad;
                Pad ^= 0xEC ^ 0x11;
            }

            byte[] Ecc = ReedSolomon(Result, DataCodewords, EccCodewords);
            Array.Copy(Ecc, 0, Result, DataCodewords, EccCodewords);
            return Result;
        }

        private static byte GfMultiply(int X, int Y)
        {
            // GF(256), polynomial 0x11D
            int Z = 0;
            for (int i = 7; i >= 0; i--)
            {
                Z = (Z << 1) ^ ((Z >> 7) * 0x11D);
                Z ^= ((Y >> i) & 1) * X;
            }
            return (byte)Z;
        }

        private static byte[] ReedSolomon(byte[] Message, int Length, int Degree)
        {
            // generator polynomial, product of (x - 2^i), leading term omitted
            byte[] Divisor = new byte[Degree];
            Divisor[Degree - 1] = 1;
            int Root = 1;
            for (int i = 0; i < Degree; i++)
            {
                for (int j = 0; j < Degree; j++)
                {
                    Divisor[j] = GfMultiply(Divisor[j], Root);
                    if (j + 1 < Degree) Divisor[j] ^= Divisor[j + 1];
                }
                Root = GfMultiply(Root, 0x02);
            }

            // remainder of message / generator
            byte[] Result = new byte[Degree];
            for (int i = 0; i < Length; i++)
            {
                int Factor = Message[i] ^ Result[0];
                Array.Copy(Result, 1, Result, 0, Degree - 1);
                Result[Degree - 1] = 0;
                for (int j = 0; j < Degree; j++)
                {
                    Result[j] ^= GfMultiply(Divisor[j], Factor);
                }
            }
            return Result;
        }

        private void DrawAlignment(int Cx, int Cy)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                for (int dx = -2; dx <= 2; dx++)
                {
                    SetFunction(Cx + dx, Cy + dy, Math.Max(Math.Abs(dx), Math.Abs(dy)) != 1);
                }
            }
        }

        private void DrawFinder(int Cx, int Cy)
        {
            // 7x7 finder plus its light separator ring
            for (int dy = -4; dy <= 4; dy++)
            {
                for (int dx = -4; dx <= 4; dx++)
                {
                    int Dist = Math.Max(Math.Abs(dx), Math.Abs(dy));
                    int x = Cx + dx;
                    int y = Cy + dy;
                    if (x >= 0 && x < Size && y >= 0 && y < Size) SetFunction(x, y, Dist != 2 && Dist != 4);
                }
            }
        }

        private void DrawFormat()
        {
            // first copy, around the top-left finder
            for (int i = 0; i <= 5; i++) SetFunction(8, i, FormatBit(i));
            SetFunction(8, 7, FormatBit(6));
            SetFunction(8, 8, FormatBit(7));
            SetFunction(7, 8, FormatBit(8));
            for (int i = 9; i < 15; i++) SetFunction(14 - i, 8, FormatBit(i));

            // second copy, split between the other two finders
            for (int i = 0; i < 8; i++) SetFunction(Size - 1 - i, 8, FormatBit(i));
            for (int i = 8; i < 15; i++) SetFunction(8, Size - 15 + i, FormatBit(i));
            SetFunction(8, Size - 8, true);     // dark module
        }

        private void Encode(byte[] Data)
        {
            // function patterns
            for (int i = 0; i < Size; i++)
            {
                SetFunction(6, i, i % 2 == 0);      // timing
                SetFunction(i, 6, i % 2 == 0);
            }
            DrawFinder(3, 3);
            DrawFinder(Size - 4, 3);
            DrawFinder(3, Size - 4);
            DrawAlignment(22, 22);                  // version 3 has one
            DrawFormat();

            // data, zig-zag in column pairs from the bottom right
            byte[] Codewords = BuildCodewords(Data);
            int Bit = 0;
            for (int Right = Size - 1; Right >= 1; Right -= 2)
            {
                if (Right == 6) Right = 5;          // skip the vertical timing column
                bool Upward = ((Right + 1) & 2) == 0;
                for (int Vert = 0; Vert < Size; Vert++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        int x = Right - j;
                        int y = Upward ? Size - 1 - Vert : Vert;
                        if (!IsFunction[y, x] && Bit < Codewords.Length * 8)
                        {
                            Modules[y, x] = ((Codewords[Bit >> 3] >> (7 - (Bit & 7))) & 1) != 0;
                            Bit++;
                        }
                    }
                }
            }

            // mask 0, data modules only
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (!IsFunction[y, x] && (x + y) % 2 == 0) Modules[y, x] = !Modules[y, x];
                }
            }
        }

        private bool FormatBit(int i)
        {
            return ((FormatBits >> i) & 1) != 0;
        }

        private void SetFunction(int x, int y, bool Dark)
        {
            Modules[y, x] = Dark;
            IsFunction[y, x] = true;
        }
    }
}
