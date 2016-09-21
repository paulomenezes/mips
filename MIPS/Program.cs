using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS
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

            string[] input = System.IO.File.ReadAllLines("../../entrada.txt");
            string output = String.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                string _out = Decode(input[i]);
                Console.WriteLine(_out);

                output += _out + (i < input.Length - 1 ? "\n" : "");
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter("../../saida.txt");
            file.Write(output);
            file.Close();
        }

        static string Decode(string input)
        {
            string binary = HexToBin(input);

            string op = binary.Substring(0, OP_INDEX);
            string label = OP_FUNCTIONS[BinToDec(op)];

            int rs = 0;
            int rt = 0;
            int rd = 0;
            int sh = 0;
            int imm = 0;

            string l = string.Empty;
            string fn = string.Empty;

            string op_name = OP_FUNCTIONS[BinToDec(op)];
            Dictionary<string, int> funcs = SIZES[op_name];
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
                            case "rs":
                                rs = BinToDec(binary.Substring(k, size));
                                break;
                            case "rt":
                                rt = BinToDec(binary.Substring(k, size));
                                break;
                            case "rd":
                                rd = BinToDec(binary.Substring(k, size));
                                break;
                            case "sh":
                                sh = BinToDec(binary.Substring(k, size));
                                break;
                            case "fn":
                                fn = R_FN[BinToDec(binary.Substring(k, size))]; 
                                break;
                        }

                        break;
                    case "bltz":
                    case "beq":
                    case "bne":
                        switch (name)
                        {
                            case "rs":
                                rs = BinToDec(binary.Substring(k, size));
                                break;
                            case "rt":
                                rt = BinToDec(binary.Substring(k, size));
                                break;
                            case "L":
                                l = binary.Substring(k, size);
                                break;
                        }

                        break;
                    case "lui":
                        switch (name)
                        {
                            case "rt":
                                rt = BinToDec(binary.Substring(k, size));
                                break;
                            case "IMM":
                                imm = BinToDec(binary.Substring(k, size));
                                break;
                        }

                        break;
                    case "j":
                    case "jal":
                        switch (name)
                        {
                            case "L":
                                l = binary.Substring(k, size);
                                break;
                        }

                        break;
                    default:
                        switch (name)
                        {
                            case "rs":
                                rs = BinToDec(binary.Substring(k, size));
                                break;
                            case "rt":
                                rt = BinToDec(binary.Substring(k, size));
                                break;
                            case "IMM":
                                imm = BinToDec(binary.Substring(k, size));
                                break;
                        }

                        break;
                }

                k += size;
            }

            string output = String.Empty;

            fn = String.IsNullOrEmpty(fn) ? op_name : fn;

            switch (label)
            {
                case "MATH_LOG":
                    switch (fn)
                    {
                        case "mfhi":
                        case "mflo":
                            output = String.Format("{0} ${1}", fn, rd);
                            break;
                        case "mult":
                        case "multu":
                        case "div":
                        case "divu":
                            output = String.Format("{0} ${1}, ${2}", fn, rs, rt);
                            break;
                        case "jr":
                            output = String.Format("{0} ${1}", fn, rs);
                            break;
                        case "sll":
                        case "srl":
                        case "sra":
                            output = String.Format("{0} ${1}, ${2}, {3}", fn, rd, rt, sh);
                            break;
                        case "sllv":
                            output = String.Format("{0} ${1}, ${2}, ${3}", fn, rd, rt, rs);
                            break;
                        case "srlv":
                        case "srav":
                            output = String.Format("{0} ${1}, ${2}, ${3}", fn, rd, rt, rs);
                            break;
                        case "syscall":
                            output = String.Format("{0}", fn);
                            break;
                        default:
                            output = String.Format("{0} ${1}, ${2}, ${3}", fn, rd, rs, rt);
                            break;
                    }
                    break;
                case "bltz":
                    output = String.Format("{0} ${1}, {2}", fn, rs, BinToDec(l.ToString()));
                    break;
                case "beq":
                case "bne":
                    output = String.Format("{0} ${1}, ${2}, {3}", fn, rs, rt, BinToDec(l.ToString()));
                    break;
                case "j":
                case "jal":
                    output = String.Format("{0} {1}", fn, BinToDec(l.ToString()));
                    break;
                case "lui":
                    output = String.Format("{0} ${1}, {2}", op_name, rt, imm);
                    break;
                default:
                    if (label.Equals("lw") || label.Equals("sw") ||
                        label.Equals("lb") || label.Equals("lbu") || label.Equals("sb"))
                        output = String.Format("{0} ${1}, {2}(${3})", fn, rt, imm, rs);
                    else
                        output = String.Format("{0} ${1}, ${2}, {3}", fn, rt, rs, imm);
                    break;
            }

            return output;
        }

        static void StartDict()
        {
            OP_FUNCTIONS.Add(0, "MATH_LOG");    // JR, ADD, SUB, SLT, AND, OR, XOR, NOR
            OP_FUNCTIONS.Add(1, "bltz");        // BLTZ
            OP_FUNCTIONS.Add(2, "j");           // J
            OP_FUNCTIONS.Add(4, "beq");         // BEQ
            OP_FUNCTIONS.Add(5, "bne");         // BNE
            OP_FUNCTIONS.Add(8, "addi");        // ADDI
            OP_FUNCTIONS.Add(10, "slti");       // SLTI
            OP_FUNCTIONS.Add(12, "andi");       // andi
            OP_FUNCTIONS.Add(13, "ori");        // ori
            OP_FUNCTIONS.Add(14, "xori");       // xori
            OP_FUNCTIONS.Add(15, "lui");        // LUI
            OP_FUNCTIONS.Add(35, "lw");         // lw
            OP_FUNCTIONS.Add(43, "sw");         // sw

            OP_FUNCTIONS.Add(3, "jal");         // JAL
            OP_FUNCTIONS.Add(9, "addiu");       // ADDIU
            OP_FUNCTIONS.Add(32, "lb");         // LB
            OP_FUNCTIONS.Add(36, "lbu");        // LBU
            OP_FUNCTIONS.Add(40, "sb");         // SB

            var MATH_LOG_SIZES = new Dictionary<string, int>() { { "rs", 5 }, { "rt", 5 }, { "rd", 5 }, { "sh", 5 }, { "fn", 6 } };
            var BLTZ_BEQ_BNE = new Dictionary<string, int>() { { "rs", 5 }, { "rt", 5 }, { "L", 16 } };
            var J = new Dictionary<string, int>() { { "L", 26 } };
            var IMM = new Dictionary<string, int>() { { "rs", 5 }, { "rt", 5 }, { "IMM", 16 } };

            SIZES.Add("MATH_LOG", MATH_LOG_SIZES);
            SIZES.Add("bltz", BLTZ_BEQ_BNE);
            SIZES.Add("j", J);
            SIZES.Add("beq", BLTZ_BEQ_BNE);
            SIZES.Add("bne", BLTZ_BEQ_BNE);

            SIZES.Add("addi", IMM);
            SIZES.Add("slti", IMM);
            SIZES.Add("addiu", IMM);
            SIZES.Add("andi", IMM);
            SIZES.Add("ori", IMM);
            SIZES.Add("xori", IMM);

            SIZES.Add("lw", IMM);
            SIZES.Add("sw", IMM);

            SIZES.Add("lui", IMM);

            SIZES.Add("lb", IMM);
            SIZES.Add("lbu", IMM);
            SIZES.Add("sb", IMM);
            SIZES.Add("jal", J);

            R_FN.Add(8, "jr");
            R_FN.Add(32, "add");
            R_FN.Add(34, "sub");
            R_FN.Add(36, "and");
            R_FN.Add(37, "or");
            R_FN.Add(38, "xor");
            R_FN.Add(39, "nor");
            R_FN.Add(42, "slt");

            R_FN.Add(16, "mfhi");
            R_FN.Add(18, "mflo");
            R_FN.Add(33, "addu");
            R_FN.Add(35, "subu");
            R_FN.Add(24, "mult");
            R_FN.Add(25, "multu");
            R_FN.Add(26, "div");
            R_FN.Add(27, "divu");
            R_FN.Add(0, "sll");
            R_FN.Add(2, "srl");
            R_FN.Add(3, "sra");
            R_FN.Add(4, "sllv");
            R_FN.Add(6, "srlv");
            R_FN.Add(7, "srav");
            R_FN.Add(12, "syscall");
        }

        static string HexToBin(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);

            return String.Join(String.Empty, hex.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
        }

        static int BinToDec(string binary)
        {
            return Convert.ToInt32(binary, 2);
        }
    }
}