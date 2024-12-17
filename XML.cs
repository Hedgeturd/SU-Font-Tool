using System.Xml;
using System.Text;

namespace SonicUnleashedFCOConv {
    public static class XML {
        public static string tableNoName;
        public static int texCount = 0, charaCount = 0, spriteIndex = 0;
        public static bool returnEarly = false, FCO = false, FTE = false;
        public static List<Structs.Group> groups = new List<Structs.Group>();
        public static List<Structs.Texture> textures = new List<Structs.Texture>();
        public static List<Structs.Character> characters = new List<Structs.Character>();
        public static void ReadXML(string path) {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + ".xml");
            Common.RemoveComments(xDoc);
            XmlElement? xRoot = xDoc.DocumentElement;

            if (xRoot != null && xRoot.Name == "FCO") {
                tableNoName = Program.currentDir + "/tables/" + (xRoot.Attributes.GetNamedItem("Table")!.Value!);
                Common.fcoTable = tableNoName + ".json";
                Translator.iconsTablePath = "tables/Icons.json";

                foreach (XmlElement node in xRoot) {
                    if (node.Name == "Groups") {
                        foreach (XmlElement groupNode in node.ChildNodes) {
                            Structs.Group group = new Structs.Group();

                            group.groupName = groupNode.Attributes.GetNamedItem("Name")!.Value!;    // Group's Name

                            List<Structs.Cell> cells = new List<Structs.Cell>();
                            foreach (XmlElement cellNode in groupNode.ChildNodes) {
                                if (cellNode.Name == "Cell") {
                                    Structs.Cell cell = new Structs.Cell();
                                    cell.cellName = cellNode.Attributes.GetNamedItem("Name")!.Value!;   // Cell's Name
                                    
                                    cell.alignment = int.Parse(cellNode.Attributes.GetNamedItem("Alignment")!.Value!);
                                    if (cell.alignment > 3) {
                                        cell.alignment = 0;
                                    }

                                    foreach (XmlElement messageNode in cellNode.ChildNodes) {
                                        if (messageNode.Name == "Message") {
                                            cell.cellMessage = messageNode.Attributes.GetNamedItem("MessageData")!.Value!;
                                            string hexString = Translator.TXTtoHEX(cell.cellMessage);
                                            hexString = hexString.Replace(" ", "");

                                            byte[] messageByteArray = Common.StringToByteArray(hexString);
                                            messageByteArray = Common.StringToByteArray(hexString);
                                            //int numberOfBytes = hexString.Length;
                                            cell.messageCharAmount = hexString.Length / 8;
                                            cell.cellMessageWrite = messageByteArray;
                                        }
                                    }

                                    List<Structs.Colour> coloursMain = new List<Structs.Colour>();  // Colour0
                                    foreach (XmlElement colourNode in cellNode.ChildNodes) {
                                        if (colourNode.Name == "ColourMain") {
                                            Structs.Colour colourMain = new Structs.Colour() {
                                                colourStart = int.Parse(colourNode.Attributes.GetNamedItem("Start")!.Value!),
                                                colourEnd = int.Parse(colourNode.Attributes.GetNamedItem("End")!.Value!),
                                                colourMarker = int.Parse(colourNode.Attributes.GetNamedItem("Marker")!.Value!),
                                                colourAlpha = byte.Parse(colourNode.Attributes.GetNamedItem("Alpha")!.Value!),
                                                colourRed = byte.Parse(colourNode.Attributes.GetNamedItem("Red")!.Value!),
                                                colourGreen = byte.Parse(colourNode.Attributes.GetNamedItem("Green")!.Value!),
                                                colourBlue = byte.Parse(colourNode.Attributes.GetNamedItem("Blue")!.Value!),
                                            };
                                            coloursMain.Add(colourMain);
                                            cell.colourMainList = coloursMain;
                                        }
                                    }

                                    List<Structs.Colour> coloursSub1 = new List<Structs.Colour>();  // Colour1
                                    foreach (XmlElement colourNode in cellNode.ChildNodes) {
                                        if (colourNode.Name == "ColourSub1") {
                                            Structs.Colour colourSub1 = new Structs.Colour() {
                                                colourStart = int.Parse(colourNode.Attributes.GetNamedItem("Start")!.Value!),
                                                colourEnd = int.Parse(colourNode.Attributes.GetNamedItem("End")!.Value!),
                                                colourMarker = int.Parse(colourNode.Attributes.GetNamedItem("Marker")!.Value!),
                                                colourAlpha = byte.Parse(colourNode.Attributes.GetNamedItem("Alpha")!.Value!),
                                                colourRed = byte.Parse(colourNode.Attributes.GetNamedItem("Red")!.Value!),
                                                colourGreen = byte.Parse(colourNode.Attributes.GetNamedItem("Green")!.Value!),
                                                colourBlue = byte.Parse(colourNode.Attributes.GetNamedItem("Blue")!.Value!),
                                            };
                                            coloursSub1.Add(colourSub1);
                                            cell.colourSub1List = coloursSub1;
                                        }
                                    }

                                    List<Structs.Colour> coloursSub2 = new List<Structs.Colour>();  // Colour2
                                    foreach (XmlElement colourNode in cellNode.ChildNodes) {
                                        if (colourNode.Name == "ColourSub2") {
                                            Structs.Colour colourSub2 = new Structs.Colour() {
                                                colourStart = int.Parse(colourNode.Attributes.GetNamedItem("Start")!.Value!),
                                                colourEnd = int.Parse(colourNode.Attributes.GetNamedItem("End")!.Value!),
                                                colourMarker = int.Parse(colourNode.Attributes.GetNamedItem("Marker")!.Value!),
                                                colourAlpha = byte.Parse(colourNode.Attributes.GetNamedItem("Alpha")!.Value!),
                                                colourRed = byte.Parse(colourNode.Attributes.GetNamedItem("Red")!.Value!),
                                                colourGreen = byte.Parse(colourNode.Attributes.GetNamedItem("Green")!.Value!),
                                                colourBlue = byte.Parse(colourNode.Attributes.GetNamedItem("Blue")!.Value!),
                                            };
                                            coloursSub2.Add(colourSub2);
                                            cell.colourSub2List = coloursSub2;
                                        }
                                    }

                                    List<Structs.Colour> highlights = new List<Structs.Colour>();     // Highlight
                                    int hightlightcount = 0;
                                    foreach (XmlElement highlightNode in cellNode.ChildNodes) {
                                        if (highlightNode.Name == "Highlight" + hightlightcount) {
                                            Structs.Colour highlight = new Structs.Colour() {
                                                colourStart = int.Parse(highlightNode.Attributes.GetNamedItem("Start")!.Value!),
                                                colourEnd = int.Parse(highlightNode.Attributes.GetNamedItem("End")!.Value!),
                                                colourMarker = int.Parse(highlightNode.Attributes.GetNamedItem("Marker")!.Value!),
                                                colourAlpha = byte.Parse(highlightNode.Attributes.GetNamedItem("Alpha")!.Value!),
                                                colourRed = byte.Parse(highlightNode.Attributes.GetNamedItem("Red")!.Value!),
                                                colourGreen = byte.Parse(highlightNode.Attributes.GetNamedItem("Green")!.Value!),
                                                colourBlue = byte.Parse(highlightNode.Attributes.GetNamedItem("Blue")!.Value!),
                                            };
                                            highlights.Add(highlight);
                                            cell.highlightList = highlights;
                                            hightlightcount++;
                                        }
                                    }

                                    cells.Add(cell);
                                }
                                group.cellList = cells;
                            }
                            groups.Add(group);
                        }
                    }
                }

                Console.WriteLine("XML read!");
                FCO = true;
            }

            if (xRoot != null && xRoot.Name == "FTE") {
                foreach (XmlElement node in xRoot) {
                    if (node.Name == "Textures") { 
                        foreach (XmlElement textureNode in node.ChildNodes) {
                            Structs.Texture texture = new Structs.Texture() {
                                textureName = textureNode.Attributes.GetNamedItem("Name")!.Value!,
                                textureSizeX = int.Parse(textureNode.Attributes.GetNamedItem("Size_X")!.Value!),
                                textureSizeY = int.Parse(textureNode.Attributes.GetNamedItem("Size_Y")!.Value!),
                            };

                            textures.Add(texture);
                            texCount++;
                        }
                    }

                    if (node.Name == "Characters") {
                        foreach (XmlElement charaNode in node.ChildNodes) {
                            Structs.Character character = new Structs.Character() {
                                textureIndex = int.Parse(charaNode.Attributes.GetNamedItem("TextureIndex")!.Value!),
                                convID = charaNode.Attributes.GetNamedItem("ConverseID")!.Value!,
                                charPoint1X = int.Parse(charaNode.Attributes.GetNamedItem("Point1_X")!.Value!),
                                charPoint1Y = int.Parse(charaNode.Attributes.GetNamedItem("Point1_Y")!.Value!),
                                charPoint2X = int.Parse(charaNode.Attributes.GetNamedItem("Point2_X")!.Value!),
                                charPoint2Y = int.Parse(charaNode.Attributes.GetNamedItem("Point2_Y")!.Value!),
                            };

                            characters.Add(character);
                            charaCount++;
                        }
                    }
                }

                Console.WriteLine("XML read!");
                FTE = true;
            }
        }

        public static void WriteFCO(string path) {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
            File.Delete(Path.Combine(filePath + ".fco"));
            BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath + ".fco", FileMode.OpenOrCreate));

            // Writing Header
            binaryWriter.Write(Common.EndianSwap(0x00000004));
            binaryWriter.Write(0x00000000);

            // Group Count
            binaryWriter.Write(Common.EndianSwap(groups.Count));
            for (int g = 0; g < groups.Count; g++) {
                // Group Name
                binaryWriter.Write(Common.EndianSwap(groups[g].groupName.Length));
                Common.WriteStringWithoutLength(binaryWriter, Common.PadString(groups[g].groupName, '@'));

                // Cell Count
                binaryWriter.Write(Common.EndianSwap(groups[g].cellList.Count));
                for (int c = 0; c < groups[g].cellList.Count; c++) {
                    var standardArea = groups[g].cellList[c];
                    // Cell Name
                    binaryWriter.Write(Common.EndianSwap(standardArea.cellName.Length));
                    Common.WriteStringWithoutLength(binaryWriter, Common.PadString(standardArea.cellName, '@'));

                    //Message Data
                    binaryWriter.Write(Common.EndianSwap(standardArea.messageCharAmount));
                    binaryWriter.Write(standardArea.cellMessageWrite);
                    //Console.WriteLine("Message Data Written!");

                    //Colour Start
                    binaryWriter.Write(Common.EndianSwap(0x00000004));

                    for (int a = 0; a < standardArea.colourMainList.Count; a++) {
                        var maincolour = standardArea.colourMainList[a];
                        var sub1colour = standardArea.colourSub1List[a];
                        var sub2colour = standardArea.colourSub2List[a];

                        //Main Colours
                        binaryWriter.Write(Common.EndianSwap(maincolour.colourStart));
                        binaryWriter.Write(Common.EndianSwap(maincolour.colourEnd));
                        binaryWriter.Write(Common.EndianSwap(maincolour.colourMarker));
                        binaryWriter.Write(maincolour.colourAlpha);
                        binaryWriter.Write(maincolour.colourRed);
                        binaryWriter.Write(maincolour.colourGreen);
                        binaryWriter.Write(maincolour.colourBlue);

                        //Sub Colours 1
                        binaryWriter.Write(Common.EndianSwap(sub1colour.colourStart));
                        binaryWriter.Write(Common.EndianSwap(sub1colour.colourEnd));
                        binaryWriter.Write(Common.EndianSwap(sub1colour.colourMarker));
                        binaryWriter.Write(sub1colour.colourAlpha);
                        binaryWriter.Write(sub1colour.colourRed);
                        binaryWriter.Write(sub1colour.colourGreen);
                        binaryWriter.Write(sub1colour.colourBlue);

                        //Sub Colours 2
                        binaryWriter.Write(Common.EndianSwap(sub2colour.colourStart));
                        binaryWriter.Write(Common.EndianSwap(sub2colour.colourEnd));
                        binaryWriter.Write(Common.EndianSwap(sub2colour.colourMarker));
                        binaryWriter.Write(sub2colour.colourAlpha);
                        binaryWriter.Write(sub2colour.colourRed);
                        binaryWriter.Write(sub2colour.colourGreen);
                        binaryWriter.Write(sub2colour.colourBlue);

                        //End Colours
                        binaryWriter.Write(Common.EndianSwap(maincolour.colourStart));
                        binaryWriter.Write(Common.EndianSwap(maincolour.colourEnd));
                        binaryWriter.Write(Common.EndianSwap(0x00000003));

                        binaryWriter.Write(Common.EndianSwap(standardArea.alignment));

                        if (standardArea.highlightList != null) {
                            binaryWriter.Write(Common.EndianSwap(standardArea.highlightList.Count));

                            for (int h = 0; h < standardArea.highlightList.Count; h++) {
                                var highlights = standardArea.highlightList[h];
                                binaryWriter.Write(Common.EndianSwap(highlights.colourStart));
                                binaryWriter.Write(Common.EndianSwap(highlights.colourEnd));
                                binaryWriter.Write(Common.EndianSwap(highlights.colourMarker));
                                binaryWriter.Write(highlights.colourAlpha);
                                binaryWriter.Write(highlights.colourRed);
                                binaryWriter.Write(highlights.colourGreen);
                                binaryWriter.Write(highlights.colourBlue);
                                Common.skipFlag = true;
                            }
                        }

                        if (Common.skipFlag) {
                            binaryWriter.Write(Common.EndianSwap(0x00000000));
                            //Console.WriteLine("Highlight Data Written!");
                            Common.skipFlag = false;
                        }
                        else {
                            binaryWriter.Write(Common.EndianSwap(0x00000000));
                            binaryWriter.Write(Common.EndianSwap(0x00000000));
                            //Console.WriteLine("Cell Data Written!");
                        }
                    }
                }

                //Console.WriteLine("Group Data Written!");
            }

            binaryWriter.Close();
            Console.WriteLine("FCO written!");
        }

        public static void WriteFTE(string path) {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
            File.Delete(Path.Combine(filePath + ".fte"));
            BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath + ".fte", FileMode.OpenOrCreate));

            // Writing Header
            binaryWriter.Write(Common.EndianSwap(0x00000004));
            binaryWriter.Write(0x00000000);

            // Texture Count
            binaryWriter.Write(Common.EndianSwap(textures.Count));
            for (int t = 0; t < textures.Count; t++) {
                binaryWriter.Write(Common.EndianSwap(textures[t].textureName.Length));
                Common.WriteStringWithoutLength(binaryWriter, Common.PadString(textures[t].textureName, '@'));
                binaryWriter.Write(Common.EndianSwap(textures[t].textureSizeX));
                binaryWriter.Write(Common.EndianSwap(textures[t].textureSizeY));
            }

            binaryWriter.Write(Common.EndianSwap(characters.Count));
            for (int c = 0; c < characters.Count; c++)
            {
                var textureData = textures[characters[c].textureIndex];
                binaryWriter.Write(Common.EndianSwap(characters[c].textureIndex));
                binaryWriter.Write(Common.EndianSwapFloat(characters[c].charPoint1X / textureData.textureSizeX));
                binaryWriter.Write(Common.EndianSwapFloat(characters[c].charPoint1Y / textureData.textureSizeY));
                binaryWriter.Write(Common.EndianSwapFloat(characters[c].charPoint2X / textureData.textureSizeX));
                binaryWriter.Write(Common.EndianSwapFloat(characters[c].charPoint2Y / textureData.textureSizeY));
            }

            binaryWriter.Close();
            Console.WriteLine("FTE written!");
        }
    }
}