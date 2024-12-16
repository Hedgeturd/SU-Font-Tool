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

                            group.GroupName = groupNode.Attributes.GetNamedItem("Name")!.Value!;    // Group's Name

                            List<Structs.Cell> cells = new List<Structs.Cell>();
                            foreach (XmlElement cellNode in groupNode.ChildNodes) {
                                if (cellNode.Name == "Cell") {
                                    Structs.Cell cell = new Structs.Cell();
                                    cell.CellName = cellNode.Attributes.GetNamedItem("Name")!.Value!;   // Cell's Name
                                    
                                    cell.Alignment = int.Parse(cellNode.Attributes.GetNamedItem("Alignment")!.Value!);
                                    if (cell.Alignment > 3) {
                                        cell.Alignment = 0;
                                    }

                                    foreach (XmlElement messageNode in cellNode.ChildNodes) {
                                        if (messageNode.Name == "Message") {
                                            cell.CellMessage = messageNode.Attributes.GetNamedItem("MessageData")!.Value!;
                                            string hexString = Translator.TXTtoHEX(cell.CellMessage);
                                            hexString = hexString.Replace(" ", "");

                                            byte[] messageByteArray = Common.StringToByteArray(hexString);
                                            messageByteArray = Common.StringToByteArray(hexString);
                                            //int numberOfBytes = hexString.Length;
                                            cell.MessageCharAmount = hexString.Length / 8;
                                            cell.CellMessageWrite = messageByteArray;
                                        }
                                    }

                                    List<Structs.ColourMain> coloursMain = new List<Structs.ColourMain>();  // Colour0
                                    foreach (XmlElement colourNode in cellNode.ChildNodes) {
                                        if (colourNode.Name == "ColourMain") {
                                            Structs.ColourMain colourMain = new Structs.ColourMain() {
                                                colourMainStart = int.Parse(colourNode.Attributes.GetNamedItem("Start")!.Value!),
                                                colourMainEnd = int.Parse(colourNode.Attributes.GetNamedItem("End")!.Value!),
                                                colourMainMarker = int.Parse(colourNode.Attributes.GetNamedItem("Marker")!.Value!),
                                                colourMainAlpha = byte.Parse(colourNode.Attributes.GetNamedItem("Alpha")!.Value!),
                                                colourMainRed = byte.Parse(colourNode.Attributes.GetNamedItem("Red")!.Value!),
                                                colourMainGreen = byte.Parse(colourNode.Attributes.GetNamedItem("Green")!.Value!),
                                                colourMainBlue = byte.Parse(colourNode.Attributes.GetNamedItem("Blue")!.Value!),
                                            };
                                            coloursMain.Add(colourMain);
                                            cell.ColourMainList = coloursMain;
                                        }
                                    }

                                    List<Structs.ColourSub1> coloursSub1 = new List<Structs.ColourSub1>();  // Colour1
                                    foreach (XmlElement colourNode in cellNode.ChildNodes) {
                                        if (colourNode.Name == "ColourSub1") {
                                            Structs.ColourSub1 colourSub1 = new Structs.ColourSub1() {
                                                colourSub1Start = int.Parse(colourNode.Attributes.GetNamedItem("Start")!.Value!),
                                                colourSub1End = int.Parse(colourNode.Attributes.GetNamedItem("End")!.Value!),
                                                colourSub1Marker = int.Parse(colourNode.Attributes.GetNamedItem("Marker")!.Value!),
                                                colourSub1Alpha = byte.Parse(colourNode.Attributes.GetNamedItem("Alpha")!.Value!),
                                                colourSub1Red = byte.Parse(colourNode.Attributes.GetNamedItem("Red")!.Value!),
                                                colourSub1Green = byte.Parse(colourNode.Attributes.GetNamedItem("Green")!.Value!),
                                                colourSub1Blue = byte.Parse(colourNode.Attributes.GetNamedItem("Blue")!.Value!),
                                            };
                                            coloursSub1.Add(colourSub1);
                                            cell.ColourSub1List = coloursSub1;
                                        }
                                    }

                                    List<Structs.ColourSub2> coloursSub2 = new List<Structs.ColourSub2>();  // Colour2
                                    foreach (XmlElement colourNode in cellNode.ChildNodes) {
                                        if (colourNode.Name == "ColourSub2") {
                                            Structs.ColourSub2 colourSub2 = new Structs.ColourSub2() {
                                                colourSub2Start = int.Parse(colourNode.Attributes.GetNamedItem("Start")!.Value!),
                                                colourSub2End = int.Parse(colourNode.Attributes.GetNamedItem("End")!.Value!),
                                                colourSub2Marker = int.Parse(colourNode.Attributes.GetNamedItem("Marker")!.Value!),
                                                colourSub2Alpha = byte.Parse(colourNode.Attributes.GetNamedItem("Alpha")!.Value!),
                                                colourSub2Red = byte.Parse(colourNode.Attributes.GetNamedItem("Red")!.Value!),
                                                colourSub2Green = byte.Parse(colourNode.Attributes.GetNamedItem("Green")!.Value!),
                                                colourSub2Blue = byte.Parse(colourNode.Attributes.GetNamedItem("Blue")!.Value!),
                                            };
                                            coloursSub2.Add(colourSub2);
                                            cell.ColourSub2List = coloursSub2;
                                        }
                                    }

                                    List<Structs.Highlight> highlights = new List<Structs.Highlight>();     // Highlight
                                    int hightlightcount = 0;
                                    foreach (XmlElement highlightNode in cellNode.ChildNodes) {
                                        if (highlightNode.Name == "Highlight" + hightlightcount) {
                                            Structs.Highlight highlight = new Structs.Highlight() {
                                                highlightStart = int.Parse(highlightNode.Attributes.GetNamedItem("Start")!.Value!),
                                                highlightEnd = int.Parse(highlightNode.Attributes.GetNamedItem("End")!.Value!),
                                                highlightMarker = int.Parse(highlightNode.Attributes.GetNamedItem("Marker")!.Value!),
                                                highlightAlpha = byte.Parse(highlightNode.Attributes.GetNamedItem("Alpha")!.Value!),
                                                highlightRed = byte.Parse(highlightNode.Attributes.GetNamedItem("Red")!.Value!),
                                                highlightGreen = byte.Parse(highlightNode.Attributes.GetNamedItem("Green")!.Value!),
                                                highlightBlue = byte.Parse(highlightNode.Attributes.GetNamedItem("Blue")!.Value!),
                                            };
                                            highlights.Add(highlight);
                                            cell.HighlightList = highlights;
                                            hightlightcount++;
                                        }
                                    }

                                    cells.Add(cell);
                                }
                                group.CellList = cells;
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
                                TextureName = textureNode.Attributes.GetNamedItem("Name")!.Value!,
                                TextureSizeX = int.Parse(textureNode.Attributes.GetNamedItem("Size_X")!.Value!),
                                TextureSizeY = int.Parse(textureNode.Attributes.GetNamedItem("Size_Y")!.Value!),
                            };

                            textures.Add(texture);
                            texCount++;
                        }
                    }

                    if (node.Name == "Characters") {
                        foreach (XmlElement charaNode in node.ChildNodes) {
                            Structs.Character character = new Structs.Character() {
                                TextureIndex = int.Parse(charaNode.Attributes.GetNamedItem("TextureIndex")!.Value!),
                                CharID = charaNode.Attributes.GetNamedItem("ConverseID")!.Value!,
                                CharPoint1X = int.Parse(charaNode.Attributes.GetNamedItem("Point1_X")!.Value!),
                                CharPoint1Y = int.Parse(charaNode.Attributes.GetNamedItem("Point1_Y")!.Value!),
                                CharPoint2X = int.Parse(charaNode.Attributes.GetNamedItem("Point2_X")!.Value!),
                                CharPoint2Y = int.Parse(charaNode.Attributes.GetNamedItem("Point2_Y")!.Value!),
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

            Encoder UTF16 = Encoding.Unicode.GetEncoder();

            BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath + ".fco", FileMode.OpenOrCreate));

            // Writing Header
            binaryWriter.Write(Common.EndianSwap(0x00000004));
            binaryWriter.Write(0x00000000);

            // Group Count
            binaryWriter.Write(Common.EndianSwap(groups.Count));
            for (int g = 0; g < groups.Count; g++) {
                // Group Name
                binaryWriter.Write(Common.EndianSwap(groups[g].GroupName.Length));
                Common.WriteStringWithoutLength(binaryWriter, Common.PadString(groups[g].GroupName, '@'));

                // Cell Count
                binaryWriter.Write(Common.EndianSwap(groups[g].CellList.Count));
                for (int c = 0; c < groups[g].CellList.Count; c++) {
                    // Cell Name
                    binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].CellName.Length));
                    Common.WriteStringWithoutLength(binaryWriter, Common.PadString(groups[g].CellList[c].CellName, '@'));

                    //Message Data
                    binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].MessageCharAmount));
                    binaryWriter.Write(groups[g].CellList[c].CellMessageWrite);
                    //Console.WriteLine("Message Data Written!");

                    //Colour Start
                    binaryWriter.Write(Common.EndianSwap(0x00000004));

                    for (int a = 0; a < groups[g].CellList[c].ColourMainList.Count; a++) {
                        var maincolour = groups[g].CellList[c].ColourMainList[a];
                        var sub1colour = groups[g].CellList[c].ColourSub1List[a];
                        var sub2colour = groups[g].CellList[c].ColourSub2List[a];

                        //Main Colours
                        binaryWriter.Write(Common.EndianSwap(maincolour.colourMainStart));
                        binaryWriter.Write(Common.EndianSwap(maincolour.colourMainEnd));
                        binaryWriter.Write(Common.EndianSwap(maincolour.colourMainMarker));
                        binaryWriter.Write(maincolour.colourMainAlpha);
                        binaryWriter.Write(maincolour.colourMainRed);
                        binaryWriter.Write(maincolour.colourMainGreen);
                        binaryWriter.Write(maincolour.colourMainBlue);

                        //Sub Colours 1
                        binaryWriter.Write(Common.EndianSwap(sub1colour.colourSub1Start));
                        binaryWriter.Write(Common.EndianSwap(sub1colour.colourSub1End));
                        binaryWriter.Write(Common.EndianSwap(sub1colour.colourSub1Marker));
                        binaryWriter.Write(sub1colour.colourSub1Alpha);
                        binaryWriter.Write(sub1colour.colourSub1Red);
                        binaryWriter.Write(sub1colour.colourSub1Green);
                        binaryWriter.Write(sub1colour.colourSub1Blue);

                        //Sub Colours 2
                        binaryWriter.Write(Common.EndianSwap(sub2colour.colourSub2Start));
                        binaryWriter.Write(Common.EndianSwap(sub2colour.colourSub2End));
                        binaryWriter.Write(Common.EndianSwap(sub2colour.colourSub2Marker));
                        binaryWriter.Write(sub2colour.colourSub2Alpha);
                        binaryWriter.Write(sub2colour.colourSub2Red);
                        binaryWriter.Write(sub2colour.colourSub2Green);
                        binaryWriter.Write(sub2colour.colourSub2Blue);

                        //End Colours
                        binaryWriter.Write(Common.EndianSwap(maincolour.colourMainStart));
                        binaryWriter.Write(Common.EndianSwap(maincolour.colourMainEnd));
                        binaryWriter.Write(Common.EndianSwap(0x00000003));

                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].Alignment));

                        if (groups[g].CellList[c].HighlightList != null) {
                            binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].HighlightList.Count));

                            for (int h = 0; h < groups[g].CellList[c].HighlightList.Count; h++) {
                                binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].HighlightList[h].highlightStart));
                                binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].HighlightList[h].highlightEnd));
                                binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].HighlightList[h].highlightMarker));
                                binaryWriter.Write(groups[g].CellList[c].HighlightList[h].highlightAlpha);
                                binaryWriter.Write(groups[g].CellList[c].HighlightList[h].highlightRed);
                                binaryWriter.Write(groups[g].CellList[c].HighlightList[h].highlightGreen);
                                binaryWriter.Write(groups[g].CellList[c].HighlightList[h].highlightBlue);
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

            Encoder UTF16 = Encoding.Unicode.GetEncoder();

            BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath + ".fte", FileMode.OpenOrCreate));

            // Writing Header
            binaryWriter.Write(Common.EndianSwap(0x00000004));
            binaryWriter.Write(0x00000000);

            // Texture Count
            binaryWriter.Write(Common.EndianSwap(textures.Count));
            for (int t = 0; t < textures.Count; t++) {
                binaryWriter.Write(Common.EndianSwap(textures[t].TextureName.Length));
                Common.WriteStringWithoutLength(binaryWriter, Common.PadString(textures[t].TextureName, '@'));
                binaryWriter.Write(Common.EndianSwap(textures[t].TextureSizeX));
                binaryWriter.Write(Common.EndianSwap(textures[t].TextureSizeY));
            }

            binaryWriter.Write(Common.EndianSwap(characters.Count));
            for (int c = 0; c < characters.Count; c++) {
                binaryWriter.Write(Common.EndianSwap(characters[c].TextureIndex));
                binaryWriter.Write(Common.EndianSwapFloat(characters[c].CharPoint1X / textures[characters[c].TextureIndex].TextureSizeX));
                binaryWriter.Write(Common.EndianSwapFloat(characters[c].CharPoint1Y / textures[characters[c].TextureIndex].TextureSizeY));
                binaryWriter.Write(Common.EndianSwapFloat(characters[c].CharPoint2X / textures[characters[c].TextureIndex].TextureSizeX));
                binaryWriter.Write(Common.EndianSwapFloat(characters[c].CharPoint2Y / textures[characters[c].TextureIndex].TextureSizeY));
            }

            binaryWriter.Close();
            Console.WriteLine("FTE written!");
        }
    }
}