using System.Xml;
using libfco;

namespace SUFontTool {
    public static class XML {
        public static string tableNoName;
        public static int texCount = 0, charaCount = 0, spriteIndex = 0;
        public static bool FCO = false;
        public static List<Structs.Group> groups = new List<Structs.Group>();
        public static List<Structs.Texture> textures = new List<Structs.Texture>();
        public static List<Structs.Character> characters = new List<Structs.Character>();
        public static FontConverse ReadXML(string path) {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + ".xml");
            //Common.RemoveComments(xDoc);
            XmlElement? xRoot = xDoc.DocumentElement;

            if (xRoot is { Name: "FCO" }) {
                FontConverse fco = new FontConverse();
                tableNoName = Program.currentDir + "/tables/" + (xRoot.Attributes.GetNamedItem("Table")!.Value!);
                Common.fcoTable = tableNoName + ".json";
                //OLDTranslator.iconsTablePath = "tables/Icons.json";
                TranslationTable thing = TranslationTable.Read(Common.fcoTable);

                foreach (XmlElement node in xRoot) {
                    if (node.Name == "Groups") {
                        foreach (XmlElement groupNode in node.ChildNodes) {
                            Group group = new Group();
                            group.Name = groupNode.Attributes.GetNamedItem("Name")!.Value!;    // Group's Name

                            List<Cell> cells = new List<Cell>();
                            foreach (XmlElement cellNode in groupNode.ChildNodes) {
                                if (cellNode.Name == "Cell") {
                                    Cell cell = new Cell
                                    {
                                        Name = cellNode.Attributes.GetNamedItem("Name")!.Value!, // Cell's Name
                                        Alignment = (Cell.TextAlign)Enum.Parse(typeof(Cell.TextAlign), cellNode.Attributes.GetNamedItem("Alignment")!.Value!)
                                    };

                                    if (Enum.IsDefined(typeof(Cell.TextAlign), cell.Alignment) == false) {
                                        cell.Alignment = 0;
                                    }

                                    var messageNode = cellNode.ChildNodes[0];
                                    XmlElement colourNode = cellNode.ChildNodes[1] as XmlElement;
                                    XmlElement colourNode2 = cellNode.ChildNodes[2] as XmlElement;
                                    XmlElement colourNode3 = cellNode.ChildNodes[3] as XmlElement;
                                    
                                    if (messageNode.Name == "Message") {
                                        cell.Message = TranslationService.RawTXTtoHEX(messageNode.InnerText, thing.Standard);
                                    }
                                    
                                    if (colourNode.Name == "ColourMain") {
                                        cell.MainColor = Common.ReadXMLColour(colourNode);
                                    }
                                    
                                    if (colourNode2.Name == "ColourSub1") {
                                        cell.ExtraColor1 = Common.ReadXMLColour(colourNode2);
                                    }
                                    
                                    if (colourNode3.Name == "ColourSub2") {
                                        cell.ExtraColor2 = Common.ReadXMLColour(colourNode3);
                                    }

                                    //List<Structs.Colour> highlights = new List<Structs.Colour>();
                                    if (cellNode.ChildNodes[4].Name == "Highlights")
                                    {
                                        int workCount = 0;
                                        foreach (XmlElement highlightNode in cellNode.ChildNodes[4].ChildNodes)
                                        {
                                            if (highlightNode.Name == "Highlight" + workCount)
                                            {
                                                CellColor highlight = new CellColor();
                                                //Common.ReadXMLColour(ref highlight, highlightNode);
                                                highlight = Common.ReadXMLColour(highlightNode);
                                                cell.Highlights.Add(highlight);
                                                //cell.highlightList = highlights;
                                                
                                                workCount++;
                                            }
                                        }
                                    }

                                    if (cellNode.ChildNodes[5].Name == "SubCells")
                                    {
                                        int workCount = 0;
                                        foreach (XmlElement subcellnode in cellNode.ChildNodes[5].ChildNodes)
                                        {
                                            if (subcellnode.Name == "SubCell" + workCount)
                                            {
                                                SubCell subcell = new SubCell
                                                {
                                                    Start = int.Parse(subcellnode.Attributes.GetNamedItem("Start")!.Value!),
                                                    End = int.Parse(subcellnode.Attributes.GetNamedItem("End")!.Value!),
                                                    SubMessage = TranslationService.RawTXTtoHEX(subcellnode.InnerText, thing.Standard)
                                                };
                                                cell.SubCells.Add(subcell);
                                                
                                                workCount++;
                                            }
                                        }
                                    }

                                    cells.Add(cell);
                                }
                                group.Cells = cells;
                            }
                            fco.Groups.Add(group);
                        }
                    }
                }

                Console.WriteLine("XML read!");
                fco.Header.Field00 = 4;
                fco.Header.Version = 0;
                
                FCO = true;
                return fco;
            }
            if (xRoot is { Name: "FTE" }) {
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
                return null;
            }
            
            return null;
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
                Common.ConvString(binaryWriter, Common.PadString(groups[g].groupName, '@'));

                // Cell Count
                binaryWriter.Write(Common.EndianSwap(groups[g].cellList.Count));
                for (int c = 0; c < groups[g].cellList.Count; c++) {
                    var standardArea = groups[g].cellList[c];
                    // Cell Name
                    binaryWriter.Write(Common.EndianSwap(standardArea.cellName.Length));
                    Common.ConvString(binaryWriter, Common.PadString(standardArea.cellName, '@'));

                    //Message Data
                    binaryWriter.Write(Common.EndianSwap(standardArea.messageCharAmount));
                    binaryWriter.Write(standardArea.cellMessageWrite);

                    // Colour Start
                    binaryWriter.Write(Common.EndianSwap(0x00000004));
                    
                    /*Common.WriteXMLColour(binaryWriter, standardArea.colourMain);  // Text Colours
                    Common.WriteXMLColour(binaryWriter, standardArea.colourSub1);  // Check
                    Common.WriteXMLColour(binaryWriter, standardArea.colourSub2);  // Check*/

                    //End Colours
                    binaryWriter.Write(Common.EndianSwap(standardArea.colourMain.colourStart));
                    binaryWriter.Write(Common.EndianSwap(standardArea.colourMain.colourEnd));
                    binaryWriter.Write(Common.EndianSwap(0x00000003));
                    
                    Structs.TextAlign alignConv = (Structs.TextAlign)Enum.Parse(typeof(Structs.TextAlign), standardArea.alignment);
                    binaryWriter.Write(Common.EndianSwap((int)alignConv));
                    
                    if (standardArea.highlightList != null) {
                        binaryWriter.Write(Common.EndianSwap(standardArea.highlightList.Count));
                        for (int h = 0; h < standardArea.highlightList.Count; h++) {
                            var highlights = standardArea.highlightList[h];
                            //Common.WriteXMLColour(binaryWriter, highlights);
                        }
                    }

                    if (standardArea.highlightList != null) {
                        binaryWriter.Write(Common.EndianSwap(0x00000000));
                    }
                    else {
                        binaryWriter.Write(Common.EndianSwap(0x00000000));
                        binaryWriter.Write(Common.EndianSwap(0x00000000));
                    }
                }
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
                Common.ConvString(binaryWriter, Common.PadString(textures[t].textureName, '@'));
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