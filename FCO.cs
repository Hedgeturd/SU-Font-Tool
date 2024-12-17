using System.Xml;
using System.Text;

namespace SonicUnleashedFCOConv {
    public static class FCO {
        public static bool structureError = false;
        public static long address;
        public static List<Structs.Group> groups = new List<Structs.Group>();
        public static List<string> highlightlocal = new List<string>();
        public static void ReadFCO(string path) {
            Common.TableAssignment();
            if (Common.fcoTable == null) return;

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            
            // Names of Groups and Cells are in UTF-8
            Encoding UTF8Encoding = Encoding.GetEncoding("UTF-8");

            long fileHeader = binaryReader.ReadInt64();   // This is always the same
            if (fileHeader != 67108864) {
                structureError = true;
                address = binaryReader.BaseStream.Position;
                return;
            }

            // Groups
            int groupCount = Common.EndianSwap(binaryReader.ReadInt32());
            Console.WriteLine("Processing " + groupCount + " Groups");

            for (int g = 0; g < groupCount; g++) {
                Structs.Group groupData = new Structs.Group();

                // Group Name
                groupData.groupName = UTF8Encoding.GetString(binaryReader.ReadBytes(Common.EndianSwap(binaryReader.ReadInt32())));
                Common.SkipPadding(binaryReader);

                // Cells count
                int cellCount = Common.EndianSwap(binaryReader.ReadInt32());
                Console.WriteLine("Processing " + cellCount + " Cells of Group " + g);

                // Cells
                List<Structs.Cell> Cells = new List<Structs.Cell>();
                for (int c = 0; c < cellCount; c++) {
                    Structs.Cell cellData = new Structs.Cell();

                    // Cell's name
                    cellData.cellName = UTF8Encoding.GetString(binaryReader.ReadBytes(Common.EndianSwap(binaryReader.ReadInt32())));
                    Common.SkipPadding(binaryReader);

                    int cellLength = Common.EndianSwap(binaryReader.ReadInt32());
                    byte[] cellMessageBytes = binaryReader.ReadBytes(cellLength * 4);
                    cellData.cellMessage = Translator.HEXtoTXT(BitConverter.ToString(cellMessageBytes).Replace("-", " "));

                    int colourHeader = Common.EndianSwap(binaryReader.ReadInt32());
                    if (colourHeader != 4) {
                        structureError = true;
                        address = binaryReader.BaseStream.Position;
                        return;
                    }

                    //Colour Data
                    List<Structs.Colour> ColourMain = new List<Structs.Colour>();
                    for (int a = 0; a < 1; a++) {
                        Structs.Colour colourMainData = new Structs.Colour() {
                            colourStart = binaryReader.ReadInt32(),
                            colourEnd = binaryReader.ReadInt32(),
                            colourMarker = binaryReader.ReadInt32(),
                            colourAlpha = binaryReader.ReadByte(),
                            colourRed = binaryReader.ReadByte(),
                            colourGreen = binaryReader.ReadByte(),
                            colourBlue = binaryReader.ReadByte(),
                        };
                        ColourMain.Add(colourMainData);
                    }

                    //ColourSub1 Data
                    List<Structs.Colour> ColourSub1 = new List<Structs.Colour>();
                    for (int a = 0; a < 1; a++) {
                        Structs.Colour colourSub1Data = new Structs.Colour() {
                            colourStart = binaryReader.ReadInt32(),
                            colourEnd = binaryReader.ReadInt32(),
                            colourMarker = binaryReader.ReadInt32(),
                            colourAlpha = binaryReader.ReadByte(),
                            colourRed = binaryReader.ReadByte(),
                            colourGreen = binaryReader.ReadByte(),
                            colourBlue = binaryReader.ReadByte(),
                        };
                        ColourSub1.Add(colourSub1Data);
                    }

                    //ColourSub2 Data
                    List<Structs.Colour> ColourSub2 = new List<Structs.Colour>();
                    for (int a = 0; a < 1; a++) {
                        Structs.Colour colourSub2Data = new Structs.Colour() {
                            colourStart = binaryReader.ReadInt32(),
                            colourEnd = binaryReader.ReadInt32(),
                            colourMarker = binaryReader.ReadInt32(),
                            colourAlpha = binaryReader.ReadByte(),
                            colourRed = binaryReader.ReadByte(),
                            colourGreen = binaryReader.ReadByte(),
                            colourBlue = binaryReader.ReadByte(),
                        };

                        // int colourExtraStart =   //I'm still unsure what these values do
                        binaryReader.ReadInt32();
                        // int colourExtraEnd =
                        binaryReader.ReadInt32();
                        binaryReader.ReadInt32();   // This one is a footer value, 0x03 marks the very end of the data

                        ColourSub2.Add(colourSub2Data);
                    }
                    cellData.colourMainList = ColourMain;
                    cellData.colourSub1List = ColourSub1;
                    cellData.colourSub2List = ColourSub2;

                    // Separator
                    cellData.alignment = Common.EndianSwap(binaryReader.ReadInt32());
                    if (cellData.alignment > 3) {
                        structureError = true;
                        address = binaryReader.BaseStream.Position;
                        return;
                    }

                    // Separator
                    cellData.highlightCount = Common.EndianSwap(binaryReader.ReadInt32());   // If this is anything but 0, it's the highlight count in the cell
                    if (cellData.highlightCount >= 1) {
                        highlightlocal.Add(groupData.groupName + ": " + cellData.cellName);
                    }

                    // Highlights
                    List<Structs.Colour> Highlights = new List<Structs.Colour>();
                    for (int h = 0; h < cellData.highlightCount; h++) {
                        Structs.Colour hightlightData = new Structs.Colour() {
                            colourStart = binaryReader.ReadInt32(),
                            colourEnd = binaryReader.ReadInt32(),
                            colourMarker = binaryReader.ReadInt32(),
                            colourAlpha = binaryReader.ReadByte(),
                            colourRed = binaryReader.ReadByte(),
                            colourGreen = binaryReader.ReadByte(),
                            colourBlue = binaryReader.ReadByte(),
                        };

                        Highlights.Add(hightlightData);
                        Common.skipFlag = true;
                    }

                    cellData.highlightList = Highlights;

                    // Back to Separator
                    binaryReader.ReadInt32();   // Yet to find out what this really does..

                    Cells.Add(cellData);
                    /*  This will add together the following into the Struct:
                        Cell Name, Message, Colour0, Colour1, Colour2 and (possibly) Highlights */
                    //Console.WriteLine("Cell Read!");
                }

                groupData.cellList = Cells;
                groups.Add(groupData);
            }

            binaryReader.Close();
            binaryReader.Dispose();
            Console.WriteLine("FCO read!");
        }

        public static void WriteXML(string path) {
            File.Delete(Path.Combine(Path.GetFileNameWithoutExtension(path) + ".xml"));

            var xmlWriterSettings = new XmlWriterSettings { Indent = true };
            using var writer = XmlWriter.Create(Path.GetDirectoryName(path) + "\\" +
            Path.GetFileNameWithoutExtension(path) + ".xml", xmlWriterSettings);

            writer.WriteStartDocument();
            writer.WriteStartElement("FCO");
            // This is used later once the XML is used to convert the data back into an FCO format
            writer.WriteAttributeString("Table", Common.fcoTableName);
            
            writer.WriteStartElement("Groups");
            foreach (Structs.Group group in groups) {
                writer.WriteStartElement("Group");
                writer.WriteAttributeString("Name", group.groupName);

                foreach (Structs.Cell cell in group.cellList) {
                    writer.WriteStartElement("Cell");
                    // These parameters are part of the "Cell" Element Header
                    writer.WriteAttributeString("Name", cell.cellName);
                    writer.WriteAttributeString("Alignment", cell.alignment.ToString());

                    // The following Elements are all within the "Cell" Element
                    writer.WriteStartElement("Message");
                    writer.WriteAttributeString("MessageData", cell.cellMessage);
                    writer.WriteEndElement();

                    writer.WriteStartElement("ColourMain");
                    foreach (Structs.Colour colourMain in cell.colourMainList) {
                        writer.WriteAttributeString("Start", Common.EndianSwap(colourMain.colourStart).ToString());
                        writer.WriteAttributeString("End", Common.EndianSwap(colourMain.colourEnd).ToString());
                        writer.WriteAttributeString("Marker", Common.EndianSwap(colourMain.colourMarker).ToString());

                        writer.WriteAttributeString("Alpha", colourMain.colourAlpha.ToString());
                        writer.WriteAttributeString("Red", colourMain.colourRed.ToString());
                        writer.WriteAttributeString("Green", colourMain.colourGreen.ToString());
                        writer.WriteAttributeString("Blue", colourMain.colourBlue.ToString());
                        writer.WriteEndElement();
                    }

                    writer.WriteStartElement("ColourSub1");
                    foreach (Structs.Colour colourSub1 in cell.colourSub1List) {
                        writer.WriteAttributeString("Start", Common.EndianSwap(colourSub1.colourStart).ToString());
                        writer.WriteAttributeString("End", Common.EndianSwap(colourSub1.colourEnd).ToString());
                        writer.WriteAttributeString("Marker", Common.EndianSwap(colourSub1.colourMarker).ToString());

                        writer.WriteAttributeString("Alpha", colourSub1.colourAlpha.ToString());
                        writer.WriteAttributeString("Red", colourSub1.colourRed.ToString());
                        writer.WriteAttributeString("Green", colourSub1.colourGreen.ToString());
                        writer.WriteAttributeString("Blue", colourSub1.colourBlue.ToString());
                        writer.WriteEndElement();
                    }

                    writer.WriteStartElement("ColourSub2");
                    foreach (Structs.Colour colourSub2 in cell.colourSub2List) {
                        writer.WriteAttributeString("Start", Common.EndianSwap(colourSub2.colourStart).ToString());
                        writer.WriteAttributeString("End", Common.EndianSwap(colourSub2.colourEnd).ToString());
                        writer.WriteAttributeString("Marker", Common.EndianSwap(colourSub2.colourMarker).ToString());

                        writer.WriteAttributeString("Alpha", colourSub2.colourAlpha.ToString());
                        writer.WriteAttributeString("Red", colourSub2.colourRed.ToString());
                        writer.WriteAttributeString("Green", colourSub2.colourGreen.ToString());
                        writer.WriteAttributeString("Blue", colourSub2.colourBlue.ToString());
                        writer.WriteEndElement();
                    }

                    while (highlightlocal != null) {
                        foreach (string highlight in highlightlocal) {
                            if (highlight.Contains(group.groupName) && highlight.Contains(cell.cellName)) {
                                Common.skipFlag = true;
                                break;
                            }
                        }
                        break;
                    }

                    if (Common.skipFlag) {
                        int workCount = 0;
                        foreach (Structs.Colour highlight in cell.highlightList) {
                            writer.WriteStartElement("Highlight" + workCount);
                            writer.WriteAttributeString("Start", Common.EndianSwap(highlight.colourStart).ToString());
                            writer.WriteAttributeString("End", Common.EndianSwap(highlight.colourEnd).ToString());
                            writer.WriteAttributeString("Marker", Common.EndianSwap(highlight.colourMarker).ToString());

                            writer.WriteAttributeString("Alpha", highlight.colourAlpha.ToString());
                            writer.WriteAttributeString("Red", highlight.colourRed.ToString());
                            writer.WriteAttributeString("Green", highlight.colourGreen.ToString());
                            writer.WriteAttributeString("Blue", highlight.colourBlue.ToString());
                            writer.WriteEndElement();
                            workCount++;
                        }

                        Common.skipFlag = false;
                    }

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndDocument();
            writer.Close();

            groups.Clear();
            if (highlightlocal != null) highlightlocal.Clear();

            Console.WriteLine("XML written!");
        }
    }
}