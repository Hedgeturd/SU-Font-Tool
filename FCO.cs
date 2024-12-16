using System.Xml;
using System.Text;
using System.Runtime.CompilerServices;

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

            // Why do we need 2 encodings? Well, the actual text data for the cells are in unicode,
            // when names of cells, categories and styles are in UTF-8
            Encoding Unicode = Encoding.GetEncoding("Unicode");
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
                groupData.GroupName = UTF8Encoding.GetString(binaryReader.ReadBytes(Common.EndianSwap(binaryReader.ReadInt32())));
                Common.SkipPadding(binaryReader);

                // Cells count
                int cellCount = Common.EndianSwap(binaryReader.ReadInt32());
                Console.WriteLine("Processing " + cellCount + " Cells of Group " + g);

                // Cells
                List<Structs.Cell> Cells = new List<Structs.Cell>();
                for (int c = 0; c < cellCount; c++) {
                    Structs.Cell cellData = new Structs.Cell();

                    // Cell's name
                    cellData.CellName = UTF8Encoding.GetString(binaryReader.ReadBytes(Common.EndianSwap(binaryReader.ReadInt32())));
                    Common.SkipPadding(binaryReader);

                    int cellLength = Common.EndianSwap(binaryReader.ReadInt32());
                    byte[] cellMessageBytes = binaryReader.ReadBytes(cellLength * 4);
                    cellData.CellMessage = Translator.HEXtoTXT(BitConverter.ToString(cellMessageBytes).Replace("-", " "));

                    int colourHeader = Common.EndianSwap(binaryReader.ReadInt32());
                    if (colourHeader != 4) {
                        structureError = true;
                        address = binaryReader.BaseStream.Position;
                        return;
                    }

                    //Colour Data
                    List<Structs.ColourMain> ColourMain = new List<Structs.ColourMain>();
                    for (int a = 0; a < 1; a++) {
                        Structs.ColourMain colourMainData = new Structs.ColourMain() {
                            colourMainStart = binaryReader.ReadInt32(),
                            colourMainEnd = binaryReader.ReadInt32(),
                            colourMainMarker = binaryReader.ReadInt32(),
                            colourMainAlpha = binaryReader.ReadByte(),
                            colourMainRed = binaryReader.ReadByte(),
                            colourMainGreen = binaryReader.ReadByte(),
                            colourMainBlue = binaryReader.ReadByte(),
                        };
                        ColourMain.Add(colourMainData);
                    }

                    //ColourSub1 Data
                    List<Structs.ColourSub1> ColourSub1 = new List<Structs.ColourSub1>();
                    for (int a = 0; a < 1; a++) {
                        Structs.ColourSub1 colourSub1Data = new Structs.ColourSub1() {
                            colourSub1Start = binaryReader.ReadInt32(),
                            colourSub1End = binaryReader.ReadInt32(),
                            colourSub1Marker = binaryReader.ReadInt32(),
                            colourSub1Alpha = binaryReader.ReadByte(),
                            colourSub1Red = binaryReader.ReadByte(),
                            colourSub1Green = binaryReader.ReadByte(),
                            colourSub1Blue = binaryReader.ReadByte(),
                        };
                        ColourSub1.Add(colourSub1Data);
                    }

                    //ColourSub2 Data
                    List<Structs.ColourSub2> ColourSub2 = new List<Structs.ColourSub2>();
                    for (int a = 0; a < 1; a++) {
                        Structs.ColourSub2 colourSub2Data = new Structs.ColourSub2() {
                            colourSub2Start = binaryReader.ReadInt32(),
                            colourSub2End = binaryReader.ReadInt32(),
                            colourSub2Marker = binaryReader.ReadInt32(),
                            colourSub2Alpha = binaryReader.ReadByte(),
                            colourSub2Red = binaryReader.ReadByte(),
                            colourSub2Green = binaryReader.ReadByte(),
                            colourSub2Blue = binaryReader.ReadByte(),
                        };

                        // int colourExtraStart =   //I'm still unsure what these values do
                        binaryReader.ReadInt32();
                        // int colourExtraEnd =
                        binaryReader.ReadInt32();
                        binaryReader.ReadInt32();   // This one is a footer value, 0x03 marks the very end of the data

                        ColourSub2.Add(colourSub2Data);
                    }
                    cellData.ColourMainList = ColourMain;
                    cellData.ColourSub1List = ColourSub1;
                    cellData.ColourSub2List = ColourSub2;

                    // Separator
                    Structs.Skip skipData = new Structs.Skip();
                    cellData.Alignment = Common.EndianSwap(binaryReader.ReadInt32());

                    if (cellData.Alignment > 3) {
                        structureError = true;
                        address = binaryReader.BaseStream.Position;
                        return;
                    }

                    // Separator
                    skipData.skip2 = Common.EndianSwap(binaryReader.ReadInt32());   // If this is anything but 0, it's the highlight count in the cell
                    if (skipData.skip2 >= 1) {
                        highlightlocal.Add(groupData.GroupName + ": " + cellData.CellName);
                    }

                    // Highlights
                    List<Structs.Highlight> Highlights = new List<Structs.Highlight>();
                    for (int h = 0; h < skipData.skip2; h++) {
                        Structs.Highlight hightlightData = new Structs.Highlight() {
                            highlightStart = binaryReader.ReadInt32(),
                            highlightEnd = binaryReader.ReadInt32(),
                            highlightMarker = binaryReader.ReadInt32(),
                            highlightAlpha = binaryReader.ReadByte(),
                            highlightRed = binaryReader.ReadByte(),
                            highlightGreen = binaryReader.ReadByte(),
                            highlightBlue = binaryReader.ReadByte(),
                        };

                        Highlights.Add(hightlightData);
                        Common.skipFlag = true;
                    }

                    cellData.HighlightList = Highlights;

                    // Back to Separator
                    int skip3 = binaryReader.ReadInt32();

                    Cells.Add(cellData);
                    /*  This will add together the following into the Struct:
                        Cell Name, Message, Colour0, Colour1, Colour2 and (possibly) Highlights */
                    //Console.WriteLine("Cell Read!");
                }

                groupData.CellList = Cells;
                groups.Add(groupData);
            }

            binaryReader.Close();
            binaryReader.Dispose();
            Console.WriteLine("FCO read!");
            return;
        }

        public static void WriteXML(string path) {
            File.Delete(Path.Combine(Path.GetFileNameWithoutExtension(path) + ".xml"));

            var xmlWriterSettings = new XmlWriterSettings { Indent = true };
            using var writer = XmlWriter.Create(Path.GetDirectoryName(path) + "\\" +
            Path.GetFileNameWithoutExtension(path) + ".xml", xmlWriterSettings);

            writer.WriteStartDocument();
            writer.WriteStartElement("FCO");
            writer.WriteAttributeString("Table", Common.fcoTableName);

            // Categories
            writer.WriteStartElement("Groups");
            foreach (Structs.Group group in groups) {
                writer.WriteStartElement("Group");
                writer.WriteAttributeString("Name", group.GroupName);

                foreach (Structs.Cell cell in group.CellList) {
                    writer.WriteStartElement("Cell");
                    writer.WriteAttributeString("Name", cell.CellName);
                    writer.WriteAttributeString("Alignment", cell.Alignment.ToString());

                    writer.WriteStartElement("Message");
                    writer.WriteAttributeString("MessageData", cell.CellMessage);
                    writer.WriteEndElement();

                    writer.WriteStartElement("ColourMain");
                    foreach (Structs.ColourMain colourMain in cell.ColourMainList) {
                        writer.WriteAttributeString("Start", Common.EndianSwap(colourMain.colourMainStart).ToString());
                        writer.WriteAttributeString("End", Common.EndianSwap(colourMain.colourMainEnd).ToString());
                        writer.WriteAttributeString("Marker", Common.EndianSwap(colourMain.colourMainMarker).ToString());

                        writer.WriteAttributeString("Alpha", colourMain.colourMainAlpha.ToString());
                        writer.WriteAttributeString("Red", colourMain.colourMainRed.ToString());
                        writer.WriteAttributeString("Green", colourMain.colourMainGreen.ToString());
                        writer.WriteAttributeString("Blue", colourMain.colourMainBlue.ToString());
                        writer.WriteEndElement();
                    }

                    writer.WriteStartElement("ColourSub1");
                    foreach (Structs.ColourSub1 colourSub1 in cell.ColourSub1List) {
                        writer.WriteAttributeString("Start", Common.EndianSwap(colourSub1.colourSub1Start).ToString());
                        writer.WriteAttributeString("End", Common.EndianSwap(colourSub1.colourSub1End).ToString());
                        writer.WriteAttributeString("Marker", Common.EndianSwap(colourSub1.colourSub1Marker).ToString());

                        writer.WriteAttributeString("Alpha", colourSub1.colourSub1Alpha.ToString());
                        writer.WriteAttributeString("Red", colourSub1.colourSub1Red.ToString());
                        writer.WriteAttributeString("Green", colourSub1.colourSub1Green.ToString());
                        writer.WriteAttributeString("Blue", colourSub1.colourSub1Blue.ToString());
                        writer.WriteEndElement();
                    }

                    writer.WriteStartElement("ColourSub2");
                    foreach (Structs.ColourSub2 colourSub2 in cell.ColourSub2List) {
                        writer.WriteAttributeString("Start", Common.EndianSwap(colourSub2.colourSub2Start).ToString());
                        writer.WriteAttributeString("End", Common.EndianSwap(colourSub2.colourSub2End).ToString());
                        writer.WriteAttributeString("Marker", Common.EndianSwap(colourSub2.colourSub2Marker).ToString());

                        writer.WriteAttributeString("Alpha", colourSub2.colourSub2Alpha.ToString());
                        writer.WriteAttributeString("Red", colourSub2.colourSub2Red.ToString());
                        writer.WriteAttributeString("Green", colourSub2.colourSub2Green.ToString());
                        writer.WriteAttributeString("Blue", colourSub2.colourSub2Blue.ToString());
                        writer.WriteEndElement();
                    }

                    //writer.WriteStartElement("Hithere");
                    
                    //writer.WriteEndElement();

                    while (highlightlocal != null) {
                        foreach (string highlight in highlightlocal) {
                            if (highlight.Contains(group.GroupName) && highlight.Contains(cell.CellName)) {
                                Common.skipFlag = true;
                                break;
                            }
                        }
                        break;
                    }

                    if (Common.skipFlag) {
                        int highlightcount = 0;
                        foreach (Structs.Highlight highlight in cell.HighlightList) {
                            writer.WriteStartElement("Highlight" + highlightcount);
                            writer.WriteAttributeString("Start", Common.EndianSwap(highlight.highlightStart).ToString());
                            writer.WriteAttributeString("End", Common.EndianSwap(highlight.highlightEnd).ToString());
                            writer.WriteAttributeString("Marker", Common.EndianSwap(highlight.highlightMarker).ToString());

                            writer.WriteAttributeString("Alpha", highlight.highlightAlpha.ToString());
                            writer.WriteAttributeString("Red", highlight.highlightRed.ToString());
                            writer.WriteAttributeString("Green", highlight.highlightGreen.ToString());
                            writer.WriteAttributeString("Blue", highlight.highlightBlue.ToString());
                            writer.WriteEndElement();
                            highlightcount++;
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
            writer.Dispose();

            groups.Clear();
            if (highlightlocal != null) highlightlocal.Clear();

            Console.WriteLine("XML written!");
            return;
        }
    }
}