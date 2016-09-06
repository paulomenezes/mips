using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arc
{
    class Program
    {
        static Dictionary<int, string> OP_FUNCTIONS = new Dictionary<int, string>();
        static Dictionary<string, Dictionary<string, int>> SIZES = new Dictionary<string, Dictionary<string, int>>();
        static int OP_INDEX = 6;

        static Dictionary<int, string> R_FN = new Dictionary<int, string>();

        static void Main(string[] args)
        {
            StartDict();

            string input = "02114020";
            string binary = HexToBin(input);

            string op = binary.Substring(0, OP_INDEX);
            string label = OP_FUNCTIONS[BinToDec(op)];

            int rs = 0;
            int rt = 0;
            int rd = 0;
            int l = 0;
            int imm = 0;
            string fn = String.Empty;

            Dictionary<string, int> funcs = SIZES[OP_FUNCTIONS[BinToDec(op)]];
            int k = OP_INDEX;
            for (int i = 0; i < funcs.Keys.Count; i++)
            {
                string name = funcs.Keys.ElementAt(i);
                int size = funcs[name];

                switch (label)
                {
                    case "MATH_LOG":
                        switch (name)
                        {
                            case "rs": rs = BinToDec(binary.Substring(k, size));
                                break;
                            case "rt": rt = BinToDec(binary.Substring(k, size));
                                break;
                            case "rd": rd = BinToDec(binary.Substring(k, size)); 
                                break;
                            case "sh": break;
                            case "fn": fn = R_FN[BinToDec(binary.Substring(k, size))];
                                break;
                        }

                        break;
                    case "BLTZ":
                    case "BEQ":
                    case "BNE":
                        switch (name)
                        {
                            case "rs": rs = BinToDec(binary.Substring(k, size));
                                break;
                            case "rt": rt = BinToDec(binary.Substring(k, size));
                                break;
                            case "L": l = BinToDec(binary.Substring(k, size));
                                break;
                        }

                        break;
                    case "J":
                        switch (name)
                        {
                            case "L": l = BinToDec(binary.Substring(k, size));
                                break;
                        }

                        break;
                    default:
                        switch (name)
                        {
                            case "rs": rs = BinToDec(binary.Substring(k, size));
                                break;
                            case "rt": rt = BinToDec(binary.Substring(k, size));
                                break;
                            case "IMM": imm = BinToDec(binary.Substring(k, size));
                                break;
                        }

                        break;
                }

                k += size;
            }

            switch (label)
            {
                case "MATH_LOG":
                    Console.WriteLine("{0} ${1}, ${2}, ${3}", fn, rd, rs, rt);
                    break;
                case "BLTZ":
                case "BEQ":
                case "BNE":
                    Console.WriteLine("{0} ${1}, ${2}, ${3}", fn, rs, rt, l);
                    break;
                case "J":
                    Console.WriteLine("{0} ${1}", fn, l);
                    break;
                default:
                    if (label.Equals("LW") || label.Equals("SW"))
                        Console.WriteLine("{0} ${1}, ${2}(${3})", fn, rt, imm, rs);
                    else
                        Console.WriteLine("{0} ${1}, ${2}, ${3}", fn, rt, rs, imm);
                    break;
            }

            Console.ReadKey();
        }

        static void StartDict()
        {
            OP_FUNCTIONS.Add(0, "MATH_LOG");    // JR, ADD, SUB, SLT, AND, OR, XOR, NOR
            OP_FUNCTIONS.Add(1, "BLTZ");        // BLTZ
            OP_FUNCTIONS.Add(2, "J");           // J
            OP_FUNCTIONS.Add(4, "BEQ");         // BEQ
            OP_FUNCTIONS.Add(5, "BNE");         // BNE
            OP_FUNCTIONS.Add(8, "MATH_01");     // ADDI
            OP_FUNCTIONS.Add(10, "MATH_02");    // SLTI
            OP_FUNCTIONS.Add(12, "ANDI");       // ANDI
            OP_FUNCTIONS.Add(13, "ORI");        // ORI
            OP_FUNCTIONS.Add(14, "XORI");       // XORI
            OP_FUNCTIONS.Add(15, "COPY");       // LUI
            OP_FUNCTIONS.Add(35, "LW");         // LW
            OP_FUNCTIONS.Add(43, "SW");         // SW

            var MATH_LOG_SIZES  = new Dictionary<string, int>() { { "rs", 5 }, { "rt", 5 }, { "rd", 5 }, { "sh", 5 }, { "fn", 6 } };
            var BLTZ_BEQ_BNE    = new Dictionary<string, int>() { { "rs", 5 }, { "rt", 5 }, { "L", 16 } };
            var J               = new Dictionary<string, int>() { { "L", 16 } };
            var IMM             = new Dictionary<string, int>() { { "rs", 5 }, { "rt", 5 }, { "IMM", 16 } };

            SIZES.Add("MATH_LOG", MATH_LOG_SIZES);
            SIZES.Add("BLTZ", BLTZ_BEQ_BNE);
            SIZES.Add("J", J);
            SIZES.Add("BEQ", BLTZ_BEQ_BNE);
            SIZES.Add("BNE", BLTZ_BEQ_BNE);

            SIZES.Add("MATH_01", IMM);
            SIZES.Add("MATH_02", IMM);
            SIZES.Add("ADDI", IMM);
            SIZES.Add("ORI", IMM);
            SIZES.Add("XORI", IMM);

            SIZES.Add("LW", IMM);
            SIZES.Add("SW", IMM);

            SIZES.Add("COPY", IMM);

            R_FN.Add(8, "JR");
            R_FN.Add(32, "ADD");
            R_FN.Add(34, "SUB");
            R_FN.Add(36, "AND");
            R_FN.Add(37, "OR");
            R_FN.Add(38, "XOR");
            R_FN.Add(39, "NOR");
            R_FN.Add(42, "SLT");
        }

        static string HexToBin(string hex)
        {
            return String.Join(String.Empty, hex.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
        }

        static int BinToDec(string binary)
        {
            return Convert.ToInt32(binary, 2);
        }
    }
}
