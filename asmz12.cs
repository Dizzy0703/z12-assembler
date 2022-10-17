using System;
using System.IO;

public class mk2
{
    public static void Main(String[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("ERROR: No input file specified.");
        }
        else if (!File.Exists(args[0]))
        {
            Console.WriteLine("ERROR: \"" + args[0] + "\" does not exist in this directory.");
        }
        else
        {
            Console.WriteLine("v2.0 raw"); // Logisim memory format tag
            using (StreamReader sr = new StreamReader(args[0]))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.IndexOf('#') == 0) continue;
                    else
                    {
                        String[] parts = line.Split(' ');

                        int opcode = 0;
                        int field1 = 0;
                        int field2 = 0;
                        int field3 = 0;
                        int inst_type = 0;
                        int output = 0;

                        switch (parts[0])
                        {
                            case "ADD":
                                opcode = 0;
                                break;
                            case "SUB":
                                opcode = 1;
                                break;
                            case "ADDI":
                                opcode = 2;
                                inst_type = 1;
                                break;
                            case "SUBI":
                                opcode = 3;
                                inst_type = 1;
                                break;
                            case "AND":
                                opcode = 4;
                                break;
                            case "OR":
                                opcode = 5;
                                break;
                            case "XOR":
                                opcode = 6;
                                break;
                            case "NOR":
                                opcode = 7;
                                break;
                            case "LW":
                                opcode = 8;
                                inst_type = 1;
                                break;
                            case "SW":
                                opcode = 9;
                                inst_type = 1;
                                break;
                            case "SRL":
                                opcode = 10;
                                break;
                            case "SRA":
                                opcode = 11;
                                break;
                            case "JUMP":
                                opcode = 12;
                                inst_type = 2;
                                break;
                            case "BEQF":
                                opcode = 13;
                                inst_type = 1;
                                break;
                            case "BEQB":
                                opcode = 14;
                                inst_type = 1;
                                break;
                            case "BLB":
                                opcode = 15;
                                inst_type = 1;
                                break;
                            default:
                                Console.WriteLine("ERROR: There is an invalid opcode in the source file.");
                                goto End;
                        }

                        if (inst_type == 0)
                        {
                            // This is a Register-type instruction.
                            parts[1] = parts[1].Trim('r');
                            parts[2] = parts[2].Trim('r');
                            parts[3] = parts[3].Trim('r');

                            field1 = int.Parse(parts[1]);
                            field2 = int.Parse(parts[2]);
                            field3 = int.Parse(parts[3]);

                            if (field1 > 3 || field2 > 3 || field3 > 3)
                            {
                                Console.WriteLine("ERROR: There are only four registers.");
                                goto End;
                            }

                            output = opcode << 8;
                            output |= field1 << 6;
                            output |= field2 << 4;
                            output |= field3 << 2;
                        }
                        else if (inst_type == 1)
                        {
                            // This is an Immediate-type instruction.
                            parts[1] = parts[1].Trim('r');
                            parts[2] = parts[2].Trim('r');

                            field1 = int.Parse(parts[1]);
                            field2 = int.Parse(parts[2]);
                            field3 = int.Parse(parts[3]);

                            if (field1 > 3 || field2 > 3)
                            {
                                Console.WriteLine("ERROR: There are only four registers.");
                                goto End;
                            }
                            else if (field3 > 15 || field3 < 0)
                            {
                                Console.WriteLine("ERROR: An immediate field can only contain values in the range 0 - 15.");
                                goto End;
                            }

                            output = opcode << 8;
                            output |= field1 << 6;
                            output |= field2 << 4;
                            output |= field3;
                        }
                        else if (inst_type == 2)
                        {
                            // This is a Jump-type instruction.
                            sbyte jump_amt = 0;
                            try
                            { jump_amt = SByte.Parse(parts[1]); }
                            catch
                            { Console.WriteLine("ERROR: Jump fields can only be in the range -128 - 127."); goto End; }

                            /* This is the sticky part - the jump field must be 
                             * signed, but it is 8 bits long. So we parse it as a
                             * signed byte and promote that byte implicitly to an int
                             * with a bitwise AND. This keeps the total value within
                             * 8 bits as opposed to automatically sign-extending it.
                             */

                            field1 = jump_amt & 0xff;

                            output = opcode << 8;
                            output |= field1;
                        }

                        /* Console.WriteLine comes with a nice handy hex-parsing
                         * option that eliminates the need to format the output
                         * as a string. This means that only integers are operated
                         * on after the switch statement, which should give better
                         * performance.
                         */
                        Console.WriteLine("{0:X3}", output);
                    }
                }
            }
        }
    End:;
    }
}