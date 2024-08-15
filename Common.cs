using System.Text;
using System.Xml;

namespace SonicUnleashedFCOConv {
    class Common {
        public static string? fcoTable, tableName, tableType, tableLang;
        public static bool skipFlag = false, noLetter = false;

        // Common Functions
        public static void TempCheck(int mode) {    // This is no longer needed but will be kept for future use            
            if (mode == 1) {
                FileStream fs;
                
                if (File.Exists("temp.txt")) {
                    File.Delete("temp.txt");
                    fs = File.Create("temp.txt");
                    fs.Close();

                    Console.WriteLine("Deleted and Restored Temp File.");
                }
                else {
                    fs = File.Create("temp.txt");
                    Console.WriteLine("Created Temp File.");
                    fs.Close();
                }
            }
            if (mode == 2) {
                if (Common.noLetter) {
                    return;
                }
                
                File.Delete("temp.txt");
            }
        }

        public static bool ErrorCheck() {
            if (Common.noLetter) {
                TempCheck(1);
                StreamWriter sw = new StreamWriter("temp.txt", append: true);
                for (int i = 0; i < Translator.missinglist.Count; i++) {
                    sw.WriteLine(Translator.missinglist[i]);
                }                
                sw.Close();

                if (Path.GetExtension(Program.fileName) == ".fco") {
                    Console.WriteLine("\nMissing Characters between " + Program.fileName + " and " + tableLang + "/" + tableName + " " + tableType + " Table");
                    Console.WriteLine("XML writing aborted!");
                }
                if (Path.GetExtension(Program.fileName) == ".xml") {
                    Console.WriteLine("\nMissing Characters between " + Program.fileName + " and " + Common.fcoTable);
                    Console.WriteLine("FCO writing aborted!");
                }

                Console.WriteLine("Please check your temp file!");
                Console.WriteLine("\nPress Enter to Exit.");
                Console.Read();
                return true;
            }
            if (FCO.noFoot) {
                Console.WriteLine("\nException occurred during parsing at: 0x" + unchecked((int)FCO.address).ToString("X")  + ".");
                Console.WriteLine("There is a structural abnormality within the FCO file!");
                Console.WriteLine("\nPress Enter to Exit.");
                Console.Read();
                return true;
            }

            return false;
        }

        public static void ExtractCheck() {
            Console.WriteLine("Do you want to extract sprites using " + Program.fileName + "? [Y/N]");
            
            if (Console.ReadLine().ToLower() != "y") {
                return;
            }
            
            XML.ReadXML(Program.fileDir + "\\" + Program.fileName);
            DDS.Process();
            Console.WriteLine("\nPress Enter to Exit.");
            Console.Read();
        }
        
        // FCO and FTE Functions
        public static void TableAssignment() {
            Console.WriteLine("Please Input the number corresponding to the original location of your FCO file:");

            Console.WriteLine("\n1: Menu\nInstall\nTown_[Country]_Common\nTown_[CountryLabo]_Common\nWorldMap");
            Console.WriteLine("\n2: In-Stage\nEvilActionCommon\nSonicActionCommon");
            Console.WriteLine("\n3: Tornado\nExStageTails_Common\n");

            string? tableSwitch = Console.ReadLine();

            if (tableSwitch.ToLower() == "test") {
                Console.WriteLine("\nWhat is the name of the table you want to test?");
                tableSwitch = Console.ReadLine();
                fcoTable = "tables/" + tableSwitch + ".json";
            }
            else {
                switch (tableSwitch) {
                    case "1":
                        tableName = "Common";
                        break;
                    case "2":
                        tableName = "In-Stage";
                        break;
                    case "3":
                        tableName = "Tornado";
                        break;
                    default:
                        Console.WriteLine("\nThis is not a valid table selection!\nPress any key to exit.");
                        Console.Read();
                        Environment.Exit(0);
                        return;
                }

                Console.WriteLine("\nPlease Input the number corresponding to the original version of your FCO file:");
                Console.WriteLine("\n1: Retail\n2: DLC\n3: Preview\n");

                tableSwitch = Console.ReadLine();
                switch (tableSwitch) {
                    case "1":
                        tableType = "Retail";
                        break;
                    case "2":
                        tableType = "DLC";
                        break;
                    case "3":
                        tableType = "Preview";
                        break;
                    default:
                        Console.WriteLine("\nThis is not a valid type selection!\nPress any key to exit.");
                        Console.Read();
                        Environment.Exit(0);
                        return;
                }

                Console.WriteLine("\nPlease Input the number corresponding to the language of your FCO file");
                Console.WriteLine("\n1: English\n2: French\n3: German\n4: Italian\n5: Japanese\n6: Spanish\n");

                tableSwitch = Console.ReadLine();
                switch (tableSwitch) {
                    case "1":
                        tableLang = "English";
                        break;
                    case "2":
                        tableLang = "French";
                        break;
                    case "3":
                        tableLang = "German";
                        break;
                    case "4":
                        tableLang = "Italian";
                        break;
                    case "5":
                        tableLang = "Japanese";
                        break;
                    case "6":
                        tableLang = "Spanish";
                        break;
                    default:
                        Console.WriteLine("\nThis is not a valid language selection!\nPress any key to exit.");
                        Console.Read();
                        Environment.Exit(0);
                        return;
                }
                
                fcoTable = Program.currentDir + "/tables/" + tableLang + "/" + tableName + " " + tableType + ".json";
            }
        }

        public static int EndianSwap(int a) {
            byte[] x = BitConverter.GetBytes(a);
            Array.Reverse(x);
            int b = BitConverter.ToInt32(x, 0);
            return b;
        }

        public static float EndianSwapFloat(float a) {
            byte[] x = BitConverter.GetBytes(a);
            Array.Reverse(x);
            float b = BitConverter.ToSingle(x, 0);
            return b;
        }

        public static void SkipPadding(BinaryReader binaryReader) {
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length) {
                int padding = Common.EndianSwap(binaryReader.ReadByte());

                if (padding == 64) {
                    binaryReader.BaseStream.Seek(1, SeekOrigin.Current);
                }
                else if (padding < 64) {
                    binaryReader.BaseStream.Seek(-1, SeekOrigin.Current);
                    break;
                }
            }
        }

        // XML Functions
        public static byte[] StringToByteArray(string hex) {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string PadString(string input, char fillerChar)
        {
            int padding = (4 - input.Length % 4) % 4;
            return input.PadRight(input.Length + padding, fillerChar);
        }

        public static void WriteStringWithoutLength(BinaryWriter writer, string value)
        {
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(utf8Bytes);
        }

        public static void RemoveComments(XmlNode node)
        {
            if (node == null) return;

            // Remove comment nodes
            for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
            {
                XmlNode? child = node.ChildNodes[i];
                if (child.NodeType == XmlNodeType.Comment)
                {
                    node.RemoveChild(child);
                }
                else
                {
                    // Recursively remove comments from child nodes
                    RemoveComments(child);
                }
            }
        }
    }
}