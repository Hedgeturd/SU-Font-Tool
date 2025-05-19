using System.Text;
using System.Xml;
using libfco;

namespace SUFontTool {
    class Common {
        public static string? fcoTable, fcoTableDir, fcoTableName;
        public static bool noLetter = false;

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
            if (OLDTranslator.missinglist.Count > 0) {
                TempCheck(1);
            
                StreamWriter sw = new StreamWriter("temp.txt", append: true);
                for (int i = 0; i < OLDTranslator.missinglist.Count; i++) {
                    sw.WriteLine(OLDTranslator.missinglist[i]);
                }                
                sw.Close();

                switch (Path.GetExtension(Program.fileName)) {
                    case ".fco":
                        Console.WriteLine("\nMissing Characters between " + Program.fileName + " and " + Common.fcoTableName + " Table");
                        Console.WriteLine("XML writing aborted!");
                        break;
                    case ".xml":
                        Console.WriteLine("\nMissing Characters between " + Program.fileName + " and " + Common.fcoTable);
                        Console.WriteLine("FCO writing aborted!");
                        break;
                    default:
                        Console.WriteLine("What did you do to me..");
                        break;
                }

                Console.WriteLine("ERROR: Please check your temp file!");
                Console.WriteLine("\nPress Enter to Exit.");
                Console.Read();
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ExtractCheck(FontTexture fteFile) {
            Console.WriteLine("Do you want to extract sprites using " + Program.fileName + "? [Y/N]");
            string choice = Console.ReadLine();
            if (choice.ToLower() != "y") return;

            DDS.Process(fteFile);
            Table.WriteJSON(fteFile);
            Console.WriteLine("\nPress Enter to Exit.");
            Console.Read();
        }

        static void IndexCheck(string userInput, int length) {
            int userInt = Convert.ToInt32(userInput);
            if (userInt < 1 || userInt > length) {
                Console.WriteLine("\nThis is not a valid selection!\nPress any key to exit.");
                Console.Read();
                Environment.Exit(0);
                return;
            }
        }
        
        
        // FCO and FTE Functions
        public static void TableAssignment() {      // This block of code is probably the worst thing I have ever made :)
            fcoTableDir = Program.currentDir + "/tables/";
            Console.WriteLine("Please Input the number corresponding to the original location of your FCO file:");
            Console.WriteLine("\n1: Languages\n2: Subtitle");

            string[] location = {"Languages/", "Subtitle/"};
            string[] language = {"English/", "French/", "German/", "Italian/", "Japanese/", "Spanish/"};
            string[] version = {"Retail/", "DLC/", "Preview/"};

            string? userInput = Console.ReadLine();

            switch (userInput.ToLower()) {
                case "1":
                    IndexCheck(userInput, location.Length);
                    fcoTableName += location[Convert.ToInt32(userInput) - 1];
                    OLDTranslator.iconsTablePath = fcoTableDir + "Icons.json";
                    break;
                case "2":
                    IndexCheck(userInput, location.Length);
                    fcoTableName += location[Convert.ToInt32(userInput) - 1];
                    break;
                case "test":
                    Console.WriteLine("\nWhat is the name of the table you want to test?");
                    userInput = Console.ReadLine();
                    fcoTableName = fcoTableDir + userInput + ".json";
                    return;
                default:
                    IndexCheck(userInput, location.Length);
                    return;
            }

            Console.WriteLine("\nPlease Input the number corresponding to the language of your FCO file");
            Console.WriteLine("\n1: English\n2: French\n3: German\n4: Italian\n5: Japanese\n6: Spanish\n");
            userInput = Console.ReadLine();
            IndexCheck(userInput, language.Length);
            fcoTableName += language[Convert.ToInt32(userInput) - 1];

            Console.WriteLine("\nPlease Input the number corresponding to the original version of your FCO file:");
            Console.WriteLine("\n1: Retail\n2: DLC\n3: Preview\n");
            userInput = Console.ReadLine();
            IndexCheck(userInput, version.Length);
            fcoTableName += version[Convert.ToInt32(userInput) - 1];

            Console.WriteLine("\nWhat is the name of the archive the FCO originated from?");
            fcoTableName += userInput = Console.ReadLine();

            fcoTable = fcoTableDir + fcoTableName + ".json";
            Console.WriteLine(fcoTable + "\n" + OLDTranslator.iconsTablePath);
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

        public static void WriteFCOColour(XmlWriter writer, CellColor colourType) {
            writer.WriteAttributeString("Start", colourType.Start.ToString());
            writer.WriteAttributeString("End", colourType.End.ToString());
            writer.WriteAttributeString("Marker", colourType.Type.ToString());

            writer.WriteAttributeString("Alpha", colourType.ArgbColor.W.ToString());
            writer.WriteAttributeString("Red", colourType.ArgbColor.X.ToString());
            writer.WriteAttributeString("Green", colourType.ArgbColor.Y.ToString());
            writer.WriteAttributeString("Blue", colourType.ArgbColor.Z.ToString());
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

        public static CellColor ReadXMLColour(XmlElement? colourNode)  {
            try
            {
                CellColor colourType = new CellColor();
                colourType.Start = int.Parse(colourNode.Attributes.GetNamedItem("Start")!.Value!);
                colourType.End = int.Parse(colourNode.Attributes.GetNamedItem("End")!.Value!);
                colourType.Type = int.Parse(colourNode.Attributes.GetNamedItem("Marker")!.Value!);
                colourType.ArgbColor = colourType.ArgbColor with
                {
                    W = float.Parse(colourNode.Attributes.GetNamedItem("Alpha")!.Value!),
                    X = float.Parse(colourNode.Attributes.GetNamedItem("Red")!.Value!),
                    Y = float.Parse(colourNode.Attributes.GetNamedItem("Green")!.Value!),
                    Z = float.Parse(colourNode.Attributes.GetNamedItem("Blue")!.Value!)
                };
                return colourType;
            }
            catch (FormatException e) {
                //Console.WriteLine(e);
                var groupName = colourNode.ParentNode.ParentNode.Attributes.GetNamedItem("Name")!.Value!;
                var cellName = colourNode.ParentNode.Attributes.GetNamedItem("Name")!.Value!;
                Console.WriteLine("ERROR: Check your Colour Values in Group: " + groupName + ", Cell: " + cellName );
                Console.ReadKey();
                throw;
            }
        }

        public static string PadString(string input, char fillerChar)
        {
            int padding = (4 - input.Length % 4) % 4;
            return input.PadRight(input.Length + padding, fillerChar);
        }

        public static void ConvString(BinaryWriter writer, string value)  {
            // Turning string into Byte Array so the data can be written properly
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