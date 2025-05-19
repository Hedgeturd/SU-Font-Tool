using System.Text;
using System.Xml;
using libfco;

namespace SUFontTool {
    class Common {
        public static string? fcoTable, fcoTableDir, fcoTableName;
        public static bool noLetter = false;

        // Common Functions
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
                    //OLDTranslator.iconsTablePath = fcoTableDir + "Icons.json";
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
            //Console.WriteLine(fcoTable + "\n" + OLDTranslator.iconsTablePath);
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