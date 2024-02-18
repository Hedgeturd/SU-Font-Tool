using System.Xml;
using System.Text;
using System.IO;
using System.Text.Json.Serialization;

namespace SonicUnleashedFCOConv {
    public static class FCO {

        public static bool skipFlag = false;
        public static int EndianSwap(int a) {
            byte[] x = BitConverter.GetBytes(a);
                //if (BitConverter.IsLittleEndian)
                Array.Reverse(x);
                int b = BitConverter.ToInt32(x, 0);
                return b;
        }

        public static void FCOtoXML(string path) {
            // ==================================================================================
            // Reading FCO File

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            // Why do we need 2 encodings? Well, the actual text data for the cells are in unicode,
            // when names of cells, categories and styles are in UTF-8
            Encoding Unicode = Encoding.GetEncoding("Unicode");
            Encoding UTF8Encoding = Encoding.GetEncoding("UTF-8");

            binaryReader.ReadInt64();   // Seems to always be the same

            List<Structs.Group> groups = new List<Structs.Group>();

            // Groups
            int groupCount = binaryReader.ReadInt32();
            int groupCountBig = EndianSwap(groupCount);
            Console.WriteLine("Group Count = " + groupCountBig);
            
            for(int i = 0; i < groupCountBig; i++) {
                Structs.Group groupData = new Structs.Group();

                // Name
                int groupNameCharsCount = binaryReader.ReadInt32();
                int groupNameCharsCountBig = EndianSwap(groupNameCharsCount);
                Console.WriteLine("Group Name Chara Count = " + groupNameCharsCountBig);
                groupData.GroupName = UTF8Encoding.GetString(binaryReader.ReadBytes(groupNameCharsCountBig));

                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length) {
                        byte afterGroupName = binaryReader.ReadByte();
                        int afterGroupNameBig = EndianSwap(afterGroupName);

                        if (afterGroupNameBig == 64) {
                            // Move forward if the next byte is 64
                            // In this case, we'll just skip one byte
                            binaryReader.BaseStream.Seek(1, SeekOrigin.Current);
                        }
                        else if (afterGroupNameBig < 64) {
                            binaryReader.BaseStream.Seek(-1, SeekOrigin.Current);
                            break;
                        }
                    }
                    
                // Cells count
                int cellCount = binaryReader.ReadInt32();
                int cellCountBig = EndianSwap(cellCount);
                Console.WriteLine("Cell Count = " + cellCountBig);

                // Cells
                List<Structs.Cell> Cells = new List<Structs.Cell>();
                for(int j = 0; j < cellCountBig; j++) {
                    Structs.Cell cellData = new Structs.Cell();

                    // Cell's name
                    int cellNameCharCount = binaryReader.ReadInt32();
                    int cellNameCharCountBig = EndianSwap(cellNameCharCount);
                    Console.WriteLine("Cell Name Chara Count = " + cellNameCharCountBig);
                    cellData.CellName = UTF8Encoding.GetString(binaryReader.ReadBytes(cellNameCharCountBig));

                        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length) {
                            byte afterCellName = binaryReader.ReadByte();
                            int afterCellNameBig = EndianSwap(afterCellName);

                            if (afterCellNameBig == 64) {
                                // Move forward if the next byte is 64
                                // In this case, we'll just skip one byte
                                binaryReader.BaseStream.Seek(1, SeekOrigin.Current);
                            }
                            else if (afterCellNameBig < 64) {
                                binaryReader.BaseStream.Seek(-1, SeekOrigin.Current);
                                break;
                            }
                        }

                    int cellLength = binaryReader.ReadInt32();
                    int cellLengthBig = EndianSwap(cellLength);
                    Console.WriteLine("Cell Length = " + cellLengthBig);

                    // Read the hexadecimal data as a byte array
                    //byte[] cellMessageBytes = binaryReader.ReadBytes(cellLengthBig * 4);
                    byte[] cellMessageBytes = binaryReader.ReadBytes(cellLengthBig * 4);

                    cellData.CellMessage = BitConverter.ToString(cellMessageBytes).Replace("-", " ");
                    //cellData.CellMessage = String.Format("x{0:X2}", cellMessageBytes);

                    //cellData.CellMessage = Unicode.GetString(binaryReader.ReadBytes(cellLengthBig * 4));
                    Console.WriteLine("Cell Message Read!");

                    binaryReader.ReadInt32();

                    //Colour Data
                    List<Structs.ColourMain> ColourMain = new List<Structs.ColourMain>();
                    for(int p = 0; p < 1; p++) {
                        Structs.ColourMain colourMainData = new Structs.ColourMain();

                        colourMainData.colourMainStart = binaryReader.ReadInt32();
                        colourMainData.colourMainEnd = binaryReader.ReadInt32();
                        colourMainData.colourMainMarker = binaryReader.ReadInt32();
                        colourMainData.colourMainAlpha = binaryReader.ReadByte();
                        colourMainData.colourMainRed = binaryReader.ReadByte();
                        colourMainData.colourMainGreen = binaryReader.ReadByte();
                        colourMainData.colourMainBlue = binaryReader.ReadByte();

                        ColourMain.Add(colourMainData);
                        Console.WriteLine("Colour Data Read!");
                    }

                    //ColourSub1 Data
                    List<Structs.ColourSub1> ColourSub1 = new List<Structs.ColourSub1>();
                    for (int p = 0; p < 1; p++)
                    {
                        Structs.ColourSub1 colourSub1Data = new Structs.ColourSub1();

                        colourSub1Data.colourSub1Start = binaryReader.ReadInt32();
                        colourSub1Data.colourSub1End = binaryReader.ReadInt32();
                        colourSub1Data.colourSub1Marker = binaryReader.ReadInt32();
                        colourSub1Data.colourSub1Alpha = binaryReader.ReadByte();
                        colourSub1Data.colourSub1Red = binaryReader.ReadByte();
                        colourSub1Data.colourSub1Green = binaryReader.ReadByte();
                        colourSub1Data.colourSub1Blue = binaryReader.ReadByte();

                        ColourSub1.Add(colourSub1Data);
                        Console.WriteLine("ColourSub1 Data Read!");
                    }

                    //ColourSub2 Data
                    List<Structs.ColourSub2> ColourSub2 = new List<Structs.ColourSub2>();
                    for (int p = 0; p < 1; p++)
                    {
                        Structs.ColourSub2 colourSub2Data = new Structs.ColourSub2();

                        colourSub2Data.colourSub2Start = binaryReader.ReadInt32();
                        colourSub2Data.colourSub2End = binaryReader.ReadInt32();
                        colourSub2Data.colourSub2Marker = binaryReader.ReadInt32();
                        colourSub2Data.colourSub2Alpha = binaryReader.ReadByte();
                        colourSub2Data.colourSub2Red = binaryReader.ReadByte();
                        colourSub2Data.colourSub2Green = binaryReader.ReadByte();
                        colourSub2Data.colourSub2Blue = binaryReader.ReadByte();

                        int colourExtraStart = binaryReader.ReadInt32();
                        int colourExtraEnd = binaryReader.ReadInt32();
                        binaryReader.ReadInt32();

                        ColourSub2.Add(colourSub2Data);
                        Console.WriteLine("ColourSub2 Data Read!");
                    }

                    cellData.ColourMainList = ColourMain;
                    cellData.ColourSub1List = ColourSub1;
                    cellData.ColourSub2List = ColourSub2;

                    //Separater
                    Structs.Skip skipData = new Structs.Skip();
                    skipData.skip1 = binaryReader.ReadInt32();
                    skipData.skip2 = binaryReader.ReadInt32();
                    skipData.skip2Big = EndianSwap(skipData.skip2);

                    List<Structs.Highlight> Highlights = new List<Structs.Highlight>();
                    for(int h = 0; h < skipData.skip2Big; h++) {
                        Structs.Highlight hightlightData = new Structs.Highlight();
                                
                        hightlightData.highlightStart = binaryReader.ReadInt32();
                        hightlightData.highlightEnd = binaryReader.ReadInt32();
                        hightlightData.highlightMarker = binaryReader.ReadInt32();
                        hightlightData.highlightAlpha = binaryReader.ReadByte();
                        hightlightData.highlightRed = binaryReader.ReadByte();
                        hightlightData.highlightGreen = binaryReader.ReadByte();
                        hightlightData.highlightBlue = binaryReader.ReadByte();

                        Highlights.Add(hightlightData);
                        Console.WriteLine("Highlight Data Read!");
                        skipFlag = true;
                    }

                    cellData.HighlightList = Highlights;

                    /*if (skipData.skip2Big > 0)
                    {
                        skipFlag = true;
                        List<Structs.Highlight> Highlights = new List<Structs.Highlight>();
                        for (int h = 0; h < skipData.skip2Big; h++)
                        {
                            Structs.Highlight hightlightData = new Structs.Highlight();

                            hightlightData.highlightStart = binaryReader.ReadInt32();
                            hightlightData.highlightEnd = binaryReader.ReadInt32();
                            hightlightData.highlightMarker = binaryReader.ReadInt32();
                            hightlightData.highlightAlpha = binaryReader.ReadByte();
                            hightlightData.highlightRed = binaryReader.ReadByte();
                            hightlightData.highlightGreen = binaryReader.ReadByte();
                            hightlightData.highlightBlue = binaryReader.ReadByte();

                            Highlights.Add(hightlightData);
                            Console.WriteLine("Highlight Data Read!");
                        }

                        cellData.HighlightList = Highlights;
                    }*/

                    int skip3 = binaryReader.ReadInt32();

                    Cells.Add(cellData);
                    Console.WriteLine("Cell Read!");
                }
                
                // Adding the cell list in the group
                groupData.CellList = Cells;
                
                // Adding the group in the categories list
                groups.Add(groupData);
            }
			
	    binaryReader.Close();

            // ==================================================================================
            // Writing XML File

            File.Delete(Path.Combine(Path.GetFileNameWithoutExtension(path) + ".xml"));

            var xmlWriterSettings = new XmlWriterSettings{ Indent = true };

            using var writer = XmlWriter.Create(Path.GetDirectoryName(path) + "\\" + 
            Path.GetFileNameWithoutExtension(path) + ".xml", xmlWriterSettings);

            writer.WriteStartDocument();

            writer.WriteStartElement("FCO");

            // Categories
            writer.WriteStartElement("Groups");
            foreach(Structs.Group group in groups) {
                writer.WriteStartElement("Group");
                writer.WriteAttributeString("Name", group.GroupName);

                foreach(Structs.Cell cell in group.CellList) {
                    writer.WriteStartElement("Cell");
                    writer.WriteAttributeString("Name", cell.CellName);

                        writer.WriteStartElement("Message");
                        writer.WriteAttributeString("MessageData", cell.CellMessage);
                        writer.WriteEndElement();
                        
                        writer.WriteStartElement("ColourMain");

                        foreach(Structs.ColourMain colourMain in cell.ColourMainList) {
                            writer.WriteAttributeString("Start", EndianSwap(colourMain.colourMainStart).ToString());
                            writer.WriteAttributeString("End", EndianSwap(colourMain.colourMainEnd).ToString());
                            writer.WriteAttributeString("Marker", EndianSwap(colourMain.colourMainMarker).ToString());
                            
                            writer.WriteAttributeString("Alpha", colourMain.colourMainAlpha.ToString());
                            writer.WriteAttributeString("Red", colourMain.colourMainRed.ToString());
                            writer.WriteAttributeString("Green", colourMain.colourMainGreen.ToString());
                            writer.WriteAttributeString("Blue", colourMain.colourMainBlue.ToString());
                            writer.WriteEndElement();
                        }

                        writer.WriteStartElement("ColourSub1");

                        foreach(Structs.ColourSub1 colourSub1 in cell.ColourSub1List) {
                            writer.WriteAttributeString("Start", EndianSwap(colourSub1.colourSub1Start).ToString());
                            writer.WriteAttributeString("End", EndianSwap(colourSub1.colourSub1End).ToString());
                            writer.WriteAttributeString("Marker", EndianSwap(colourSub1.colourSub1Marker).ToString());
                            
                            writer.WriteAttributeString("Alpha", colourSub1.colourSub1Alpha.ToString());
                            writer.WriteAttributeString("Red", colourSub1.colourSub1Red.ToString());
                            writer.WriteAttributeString("Green", colourSub1.colourSub1Green.ToString());
                            writer.WriteAttributeString("Blue", colourSub1.colourSub1Blue.ToString());
                            writer.WriteEndElement();
                        }

                        writer.WriteStartElement("ColourSub2");

                        foreach(Structs.ColourSub2 colourSub2 in cell.ColourSub2List) {
                            writer.WriteAttributeString("Start", EndianSwap(colourSub2.colourSub2Start).ToString());
                            writer.WriteAttributeString("End", EndianSwap(colourSub2.colourSub2End).ToString());
                            writer.WriteAttributeString("Marker", EndianSwap(colourSub2.colourSub2Marker).ToString());
                            
                            writer.WriteAttributeString("Alpha", colourSub2.colourSub2Alpha.ToString());
                            writer.WriteAttributeString("Red", colourSub2.colourSub2Red.ToString());
                            writer.WriteAttributeString("Green", colourSub2.colourSub2Green.ToString());
                            writer.WriteAttributeString("Blue", colourSub2.colourSub2Blue.ToString());
                            writer.WriteEndElement();
                        }

                        /*writer.WriteStartElement("Highlight");

                        foreach (Structs.Highlight highlight in cell.HighlightList) {
                            writer.WriteAttributeString("Start", EndianSwap(highlight.highlightStart).ToString());
                            writer.WriteAttributeString("End", EndianSwap(highlight.highlightEnd).ToString());
                            writer.WriteAttributeString("Marker", EndianSwap(highlight.highlightMarker).ToString());
                            
                            writer.WriteAttributeString("Alpha", highlight.highlightAlpha.ToString());
                            writer.WriteAttributeString("Red", highlight.highlightRed.ToString());
                            writer.WriteAttributeString("Green", highlight.highlightGreen.ToString());
                            writer.WriteAttributeString("Blue", highlight.highlightBlue.ToString());
                        }
                        writer.WriteEndElement();*/

                        /*while (skipFlag == true)
                        {
                            writer.WriteStartElement("Highlight");

                            foreach (Structs.Highlight highlight in cell.HighlightList)
                            {
                                writer.WriteAttributeString("Start", EndianSwap(highlight.highlightStart).ToString());
                                writer.WriteAttributeString("End", EndianSwap(highlight.highlightEnd).ToString());
                                writer.WriteAttributeString("Marker", EndianSwap(highlight.highlightMarker).ToString());

                                writer.WriteAttributeString("Alpha", highlight.highlightAlpha.ToString());
                                writer.WriteAttributeString("Red", highlight.highlightRed.ToString());
                                writer.WriteAttributeString("Green", highlight.highlightGreen.ToString());
                                writer.WriteAttributeString("Blue", highlight.highlightBlue.ToString());
                                writer.WriteEndElement();
                            }

                            writer.WriteEndElement();
                            skipFlag = false;
                        }*/

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
	        writer.Close();
            
            Console.WriteLine("XML written!");

            if (skipFlag == true) {
                Console.WriteLine("FCO is in Complex Format! Expect Extra features to be lost in conversion!");
                Console.ReadKey();
            }

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
