using System.Xml;
using System.Text;

namespace SonicUnleashedFCOConv {
    public static class FCO {
        public static bool noFoot = false;
        public static long address;
        public static List<Structs.Group> groups = new List<Structs.Group>();
        public static List<string> highlightlocal = new List<string>();
        public static void Read(string path) {
            Common.TableAssignment();
            Common.fcoTable = Translator.jsonFilePath;

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            // Why do we need 2 encodings? Well, the actual text data for the cells are in unicode,
            // when names of cells, categories and styles are in UTF-8
            Encoding Unicode = Encoding.GetEncoding("Unicode");
            Encoding UTF8Encoding = Encoding.GetEncoding("UTF-8");

            binaryReader.ReadInt64();   // This is always the same

            //List<Structs.Group> groups = new List<Structs.Group>();

            // Groups
            int groupCount = Common.EndianSwap(binaryReader.ReadInt32());
            Console.WriteLine("Processing " + groupCount + " Groups");

            for (int i = 0; i < groupCount; i++) {
                Structs.Group groupData = new Structs.Group();

                // Group Name
                groupData.GroupName = UTF8Encoding.GetString(binaryReader.ReadBytes(Common.EndianSwap(binaryReader.ReadInt32())));
                Common.SkipPadding(binaryReader);

                // Cells count
                int cellCount = Common.EndianSwap(binaryReader.ReadInt32());
                Console.WriteLine("Processing " + cellCount + " Cells of Group " + i);

                // Cells
                List<Structs.Cell> Cells = new List<Structs.Cell>();
                for (int j = 0; j < cellCount; j++) {
                    Structs.Cell cellData = new Structs.Cell();

                    // Cell's name
                    cellData.CellName = UTF8Encoding.GetString(binaryReader.ReadBytes(Common.EndianSwap(binaryReader.ReadInt32())));
                    Common.SkipPadding(binaryReader);

                    int cellLength = Common.EndianSwap(binaryReader.ReadInt32());
                    byte[] cellMessageBytes = binaryReader.ReadBytes(cellLength * 4);
                    cellData.CellMessage = Translator.HEXtoTXT(BitConverter.ToString(cellMessageBytes).Replace("-", " "));
                    //Console.WriteLine("Cell Message Read!");

                    binaryReader.ReadInt32();

                    //Colour Data
                    List<Structs.ColourMain> ColourMain = new List<Structs.ColourMain>();
                    for (int p = 0; p < 1; p++) {
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
                        //Console.WriteLine("Colour Data Read!");
                    }

                    //ColourSub1 Data
                    List<Structs.ColourSub1> ColourSub1 = new List<Structs.ColourSub1>();
                    for (int p = 0; p < 1; p++) {
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
                        //Console.WriteLine("ColourSub1 Data Read!");
                    }

                    //ColourSub2 Data
                    List<Structs.ColourSub2> ColourSub2 = new List<Structs.ColourSub2>();
                    for (int p = 0; p < 1; p++) {
                        Structs.ColourSub2 colourSub2Data = new Structs.ColourSub2() {
                            colourSub2Start = binaryReader.ReadInt32(),
                            colourSub2End = binaryReader.ReadInt32(),
                            colourSub2Marker = binaryReader.ReadInt32(),
                            colourSub2Alpha = binaryReader.ReadByte(),
                            colourSub2Red = binaryReader.ReadByte(),
                            colourSub2Green = binaryReader.ReadByte(),
                            colourSub2Blue = binaryReader.ReadByte(),
                        };

                        /* int colourExtraStart =  */
                        binaryReader.ReadInt32();
                        /* int colourExtraEnd =  */
                        binaryReader.ReadInt32();
                        binaryReader.ReadInt32();

                        ColourSub2.Add(colourSub2Data);
                        //Console.WriteLine("ColourSub2 Data Read!");
                    }

                    cellData.ColourMainList = ColourMain;
                    cellData.ColourSub1List = ColourSub1;
                    cellData.ColourSub2List = ColourSub2;

                    // Footer
                    Structs.Skip skipData = new Structs.Skip();

                    try {
                        skipData.skip1 = binaryReader.ReadInt32();
                    }
                    catch (EndOfStreamException) {
                        noFoot = true;
                        address = binaryReader.BaseStream.Position;
                        return;
                    }

                    // Separator
                    skipData.skip2 = Common.EndianSwap(binaryReader.ReadInt32());

                    if (skipData.skip2 >= 1) {
                        highlightlocal.Add(groupData.GroupName + ": " + cellData.CellName);
                        //Console.WriteLine(highlightlocal[j]);
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
                        //Console.WriteLine("Highlight Data Read!");
                        Common.skipFlag = true;
                    }

                    cellData.HighlightList = Highlights;

                    // Back to Footer
                    int skip3 = binaryReader.ReadInt32();

                    Cells.Add(cellData);
                    //Console.WriteLine("Cell Read!");
                }

                // Adding the cell list in the group
                groupData.CellList = Cells;

                // Adding the group in the categories list
                groups.Add(groupData);
            }

            binaryReader.Close();
            binaryReader.Dispose();
            Console.WriteLine("FCO read!");
            return;
        }

        public static void Write(string path) {
            File.Delete(Path.Combine(Path.GetFileNameWithoutExtension(path) + ".xml"));

            var xmlWriterSettings = new XmlWriterSettings { Indent = true };
            using var writer = XmlWriter.Create(Path.GetDirectoryName(path) + "\\" +
            Path.GetFileNameWithoutExtension(path) + ".xml", xmlWriterSettings);

            writer.WriteStartDocument();
            writer.WriteStartElement("FCO");
            writer.WriteAttributeString("Table", Common.tableName + " " + Common.tableType);

            // Categories
            writer.WriteStartElement("Groups");
            foreach (Structs.Group group in groups) {
                writer.WriteStartElement("Group");
                writer.WriteAttributeString("Name", group.GroupName);

                foreach (Structs.Cell cell in group.CellList) {
                    writer.WriteStartElement("Cell");
                    writer.WriteAttributeString("Name", cell.CellName);

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

                    while (highlightlocal != null) {
                        foreach (string highlight in highlightlocal) {
                            if (highlight.Contains(group.GroupName) && highlight.Contains(cell.CellName)) {
                                Common.skipFlag = true;
                                break;
                            }
                        }
                        break;
                    }

                    if (Common.skipFlag == true) {
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
            Console.WriteLine("XML written!");
            return;
        }

        public static Structs.Highlight Highlight { get; set; }
        public static Structs.Skip Skip { get; set; }
        public static Structs.ColourSub2 ColourSub2 { get; set; }
        public static Structs.ColourSub1 ColourSub1 { get; set; }
        public static Structs.ColourMain ColourMain { get; set; }
        public static Structs.Cell Cell { get; set; }
        public static Structs.Group Group { get; set; }
    }
}