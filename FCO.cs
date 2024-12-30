using System.Xml;
using System.Text;
using SUFontTool;

namespace SonicUnleashedFCOConv {
    public static class FCO {
        public static List<Structs.Group> groups = new List<Structs.Group>();
        public static List<string> highlightlocal = new List<string>();
        public static void ReadFCO(string path) {
            // Very messy 2nd arg thing, I'll clean this up
            if (Program.tableArg != null) {
                Common.fcoTableDir = Program.currentDir + "/tables/";
                Common.fcoTableName = Program.tableArg;
                Common.fcoTable = Common.fcoTableDir + Common.fcoTableName + ".json";
                Translator.iconsTablePath = Common.fcoTableDir + "Icons.json";
            }
            else {
                Common.TableAssignment();
            }
            if (Common.fcoTable == null) return;

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            Encoding UTF8Encoding = Encoding.GetEncoding("UTF-8");      // Names of Groups and Cells are in UTF-8
            
            if (binaryReader.ReadInt64() != 67108864) {     // This is always the same
                Common.structureError = true;
                Common.address = binaryReader.BaseStream.Position;
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
                    
                    if (Common.EndianSwap(binaryReader.ReadInt32()) != 4) {
                        Common.structureError = true;
                        Common.address = binaryReader.BaseStream.Position;
                        return;
                    }
                    
                    // Main Text Colour
                    Structs.Colour colourMain = new Structs.Colour();
                    Common.ReadFCOColour(binaryReader, ref colourMain);
                    cellData.colourMain = colourMain;
                    
                    // Check what this is
                    Structs.Colour ColourSub1 = new Structs.Colour();
                    Common.ReadFCOColour(binaryReader, ref ColourSub1);
                    cellData.colourSub1 = ColourSub1;
                    
                    // Check what this is
                    Structs.Colour ColourSub2 = new Structs.Colour();
                    Common.ReadFCOColour(binaryReader, ref ColourSub2);
                    cellData.colourSub2 = ColourSub2;
                    
                    
                    binaryReader.ReadInt32();   // I'm still unsure what these values do     // int colourExtraStart = 
                    binaryReader.ReadInt32();   // int colourExtraEnd =
                    binaryReader.ReadInt32();   // This 0x03 marks the very end of the data

                    // Separator
                    int alignment = Common.EndianSwap(binaryReader.ReadInt32());
                    if (alignment > 3) {
                        Common.structureError = true;
                        Common.address = binaryReader.BaseStream.Position;
                        return;
                    }
                    var enumDisplayStatus = (Structs.TextAlign)alignment;
                    cellData.alignment = enumDisplayStatus.ToString();
                    //Console.WriteLine(cellData.alignment);

                    // Separator
                    cellData.highlightCount = Common.EndianSwap(binaryReader.ReadInt32());  // If this is anything but 0, it's the highlight count in the cell
                    if (cellData.highlightCount >= 1) {
                        highlightlocal.Add(groupData.groupName + ": " + cellData.cellName);
                    }

                    // Highlights
                    List<Structs.Colour> Highlights = new List<Structs.Colour>();
                    for (int h = 0; h < cellData.highlightCount; h++) {
                        Structs.Colour hightlightData = new Structs.Colour();
                        Common.ReadFCOColour(binaryReader, ref hightlightData);
                        Highlights.Add(hightlightData);
                        Common.skipFlag = true;
                    }

                    cellData.highlightList = Highlights;

                    // End of Cell
                    binaryReader.ReadInt32();   // Yet to find out what this really does mean..

                    Cells.Add(cellData);
                    /*  This will add together the following into the Cell Struct:
                        Cell Name, Message, Colour0, Colour1, Colour2 and (possibly) Highlights */
                    //Console.WriteLine("Cell Read!");
                }

                groupData.cellList = Cells;     // This will put every Cell into a Group
                groups.Add(groupData);          // This adds all the Groups together
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
            writer.WriteAttributeString("Table", Common.fcoTableName);      // This is used later once the XML is used to convert the data back into an FCO format
            
            writer.WriteStartElement("Groups");
            foreach (Structs.Group group in groups) {
                writer.WriteStartElement("Group");
                writer.WriteAttributeString("Name", group.groupName);

                foreach (Structs.Cell cell in group.cellList) {
                    writer.WriteStartElement("Cell");
                    writer.WriteAttributeString("Name", cell.cellName);                     // These parameters are part of the "Cell" Element Header
                    writer.WriteAttributeString("Alignment", cell.alignment.ToString());
                                                                                            // The following Elements are all within the "Cell" Element
                    writer.WriteStartElement("Message");
                    writer.WriteAttributeString("MessageData", cell.cellMessage);
                    writer.WriteEndElement();

                    writer.WriteStartElement("ColourMain");
                    Common.WriteFCOColour(writer, cell.colourMain);
                    writer.WriteEndElement();
                    
                    writer.WriteStartElement("ColourSub1");
                    Common.WriteFCOColour(writer, cell.colourSub1);
                    writer.WriteEndElement();
                    
                    writer.WriteStartElement("ColourSub2");
                    Common.WriteFCOColour(writer, cell.colourSub2);
                    writer.WriteEndElement();

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
                            Common.WriteFCOColour(writer, highlight);
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