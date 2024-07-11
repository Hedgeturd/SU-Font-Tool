using System.Text;
using System.Xml;

namespace SonicUnleashedFCOConv {
    class Common {
        public static string? fcoTable, tableName, tableType;
        public static bool skipFlag = false, noLetter = false;

        // Common Functions
        public static void TempCheck(int mode) {
            FileStream fs;
            
            if (File.Exists("temp.txt")) {
                File.Delete("temp.txt");
                if (mode == 1) {
                    fs = File.Create("temp.txt");
                    fs.Close();        
                    Console.WriteLine("Restored Temp File");
                }
                Console.WriteLine("Deleted Temp File");
            }
            else {
                fs = File.Create("temp.txt");
                fs.Close();
            }
        }

        public static bool ErrorCheck() {
            if (Common.noLetter == true) {
                //Console.WriteLine("\nSome characters in the FCO are NOT in the current table and the XML has not been written!");
                Console.WriteLine("\nMissing Characters between " + "FCO" + " and the " + tableName + " " + tableType + " Table, XML writing aborted!");
                Console.WriteLine("Please check your FCO and the temp file!");
                return true;
            }
            if (FCO.noFoot == true) {
                Console.WriteLine("\nException occurred during parsing at: 0x" + unchecked((int)FCO.address).ToString("X")  + ".");
                Console.WriteLine("There is a structural abnormality within the FCO file!");
                return true;
            }

            return false;
        }
        
        // FCO and FTE Functions
        public static void TableAssignment() {
            Console.WriteLine("Please Input the number corresponding to the original location of your FCO file:");
            Console.WriteLine("\n1: Menu\nInstall\nTown_[Country]_Common\nTown_[CountryLabo]_Common\nWorldMap");
            Console.WriteLine("\n2: In-Stage\nEvilActionCommon\nSonicActionCommon");
            Console.WriteLine("\n3: Tornado\nExStageTails_Common\n");

            string? tableSwitch = Console.ReadLine();

            if (tableSwitch == "TEST") {
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
                        Console.WriteLine("\nThis is not a valid table!\nPress any key to exit.");
                        Console.Read();
                        break;
                }

                Console.WriteLine("\nPlease Input the number corresponding to the original version of your FCO file:");
                Console.WriteLine("\n1: Retail");
                Console.WriteLine("\n2: DLC");
                Console.WriteLine("\n3: Preview\n");

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
                        Console.WriteLine("\nThis is not a valid type!\nPress any key to exit.");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;
                }

                fcoTable = "tables/" + tableName + " " + tableType + ".json";
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