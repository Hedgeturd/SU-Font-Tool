using System.Xml;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SonicUnleashedFCOConv {
    public static class XML {
        public static int EndianSwap(int a) {
            byte[] x = BitConverter.GetBytes(a);
                //if (BitConverter.IsLittleEndian)
                Array.Reverse(x);
                int b = BitConverter.ToInt32(x, 0);
                return b;
        }

        static byte[] StringToByteArray(string hex) {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        static string PadString(string input, char fillerChar)
        {
            int padding = (4 - input.Length % 4) % 4;
            return input.PadRight(input.Length + padding, fillerChar);
        }

        static void WriteStringWithoutLength(BinaryWriter writer, string value)
        {
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(utf8Bytes);
        }

        public static void XMLtoFCO(string path) {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
			
            // ==================================================================================
            // Reading XML File

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + ".xml");
            XmlElement? xRoot = xDoc.DocumentElement;

            List<Structs.Group> groups = new List<Structs.Group>();

            if(xRoot != null) {
                foreach(XmlElement node in xRoot) {
                    // Groups
                    if(node.Name == "Groups") {
                        foreach(XmlElement groupNode in node.ChildNodes) {
                            Structs.Group group = new Structs.Group();

                            // Category's name
                            group.GroupName = groupNode.Attributes.GetNamedItem("Name")!.Value!;

                            List<Structs.Cell> cells = new List<Structs.Cell>();

                            foreach(XmlElement cellNode in groupNode.ChildNodes) {
                                if(cellNode.Name == "Cell") {
                                    Structs.Cell cell = new Structs.Cell();

                                    // Cell's name
                                    cell.CellName = cellNode.Attributes.GetNamedItem("Name")!.Value!;

                                    foreach (XmlElement messageNode in cellNode.ChildNodes) {   
                                        if(messageNode.Name == "Message") {
                                            cell.CellMessage = messageNode.Attributes.GetNamedItem("MessageData")!.Value!;
                                            string hexString = cell.CellMessage.Replace(" ", "");
                                            byte[] messageByteArray = StringToByteArray(hexString);
                                            hexString = BitConverter.ToString(messageByteArray).Replace("-", "");
                                            messageByteArray = StringToByteArray(hexString);

                                            int numberOfBytes = messageByteArray.Length;
                                            cell.MessageCharAmount = numberOfBytes / 4;

                                            cell.CellMessageWrite = messageByteArray;
                                        }
                                    }
                                    
                                    List<Structs.ColourMain> coloursMain = new List<Structs.ColourMain>();

                                    foreach(XmlElement colourNode in cellNode.ChildNodes) {   
                                        if(colourNode.Name == "ColourMain") {
                                            Structs.ColourMain colourMain = new Structs.ColourMain();

                                            // Cell's name
                                            colourMain.colourMainStart = int.Parse(colourNode.Attributes.GetNamedItem("Start")!.Value!);
                                            colourMain.colourMainEnd = int.Parse(colourNode.Attributes.GetNamedItem("End")!.Value!);
                                            colourMain.colourMainMarker = int.Parse(colourNode.Attributes.GetNamedItem("Marker")!.Value!);

                                            colourMain.colourMainAlpha = byte.Parse(colourNode.Attributes.GetNamedItem("Alpha")!.Value!);
                                            colourMain.colourMainRed = byte.Parse(colourNode.Attributes.GetNamedItem("Red")!.Value!);
                                            colourMain.colourMainGreen = byte.Parse(colourNode.Attributes.GetNamedItem("Green")!.Value!);
                                            colourMain.colourMainBlue = byte.Parse(colourNode.Attributes.GetNamedItem("Blue")!.Value!);

                                            coloursMain.Add(colourMain);

                                            cell.ColourMainList = coloursMain;
                                        }
                                    }

                                    List<Structs.ColourSub1> coloursSub1 = new List<Structs.ColourSub1>();

                                    foreach (XmlElement colourNode in cellNode.ChildNodes)
                                    {
                                        if (colourNode.Name == "ColourSub1")
                                        {
                                            Structs.ColourSub1 colourSub1 = new Structs.ColourSub1();

                                            // Cell's name
                                            colourSub1.colourSub1Start = int.Parse(colourNode.Attributes.GetNamedItem("Start")!.Value!);
                                            colourSub1.colourSub1End = int.Parse(colourNode.Attributes.GetNamedItem("End")!.Value!);
                                            colourSub1.colourSub1Marker = int.Parse(colourNode.Attributes.GetNamedItem("Marker")!.Value!);

                                            colourSub1.colourSub1Alpha = byte.Parse(colourNode.Attributes.GetNamedItem("Alpha")!.Value!);
                                            colourSub1.colourSub1Red = byte.Parse(colourNode.Attributes.GetNamedItem("Red")!.Value!);
                                            colourSub1.colourSub1Green = byte.Parse(colourNode.Attributes.GetNamedItem("Green")!.Value!);
                                            colourSub1.colourSub1Blue = byte.Parse(colourNode.Attributes.GetNamedItem("Blue")!.Value!);

                                            coloursSub1.Add(colourSub1);

                                            cell.ColourSub1List = coloursSub1;
                                        }
                                    }

                                    List<Structs.ColourSub2> coloursSub2 = new List<Structs.ColourSub2>();

                                    foreach (XmlElement colourNode in cellNode.ChildNodes)
                                    {
                                        if (colourNode.Name == "ColourSub2")
                                        {
                                            Structs.ColourSub2 colourSub2 = new Structs.ColourSub2();

                                            // Cell's name
                                            colourSub2.colourSub2Start = int.Parse(colourNode.Attributes.GetNamedItem("Start")!.Value!);
                                            colourSub2.colourSub2End = int.Parse(colourNode.Attributes.GetNamedItem("End")!.Value!);
                                            colourSub2.colourSub2Marker = int.Parse(colourNode.Attributes.GetNamedItem("Marker")!.Value!);

                                            colourSub2.colourSub2Alpha = byte.Parse(colourNode.Attributes.GetNamedItem("Alpha")!.Value!);
                                            colourSub2.colourSub2Red = byte.Parse(colourNode.Attributes.GetNamedItem("Red")!.Value!);
                                            colourSub2.colourSub2Green = byte.Parse(colourNode.Attributes.GetNamedItem("Green")!.Value!);
                                            colourSub2.colourSub2Blue = byte.Parse(colourNode.Attributes.GetNamedItem("Blue")!.Value!);

                                            coloursSub2.Add(colourSub2);

                                            cell.ColourSub2List = coloursSub2;
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
            }

            // ==================================================================================
            // Writing FCO File

            File.Delete(Path.Combine(filePath + ".fco"));

            Encoder UTF16 = Encoding.Unicode.GetEncoder();

            BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath + ".fco", FileMode.OpenOrCreate));
            
            // Writing first 8 bytes
            binaryWriter.Write(EndianSwap(0x00000004));
            binaryWriter.Write(0x00000000);

            // Group Count
            binaryWriter.Write(EndianSwap(groups.Count));
            for(int i = 0; i < groups.Count; i++) {
                // Group Name
                binaryWriter.Write(EndianSwap(groups[i].GroupName.Length));
                WriteStringWithoutLength(binaryWriter, PadString(groups[i].GroupName, '@'));

                // Cell Count
                binaryWriter.Write(EndianSwap(groups[i].CellList.Count));

                for (int i2 = 0; i2 < groups[i].CellList.Count; i2++) {
                    // Cell Name
                    binaryWriter.Write(EndianSwap(groups[i].CellList[i2].CellName.Length));
                    WriteStringWithoutLength(binaryWriter, PadString(groups[i].CellList[i2].CellName, '@'));

                    //Message Data
                    binaryWriter.Write(EndianSwap(groups[i].CellList[i2].MessageCharAmount));
                    binaryWriter.Write(groups[i].CellList[i2].CellMessageWrite);
                    Console.WriteLine("Message Data Written!");

                    //Colour Start
                    binaryWriter.Write(EndianSwap(0x00000004));

                    for (int i3 = 0; i3 < groups[i].CellList[i2].ColourMainList.Count; i3++) {
                        //Main Colours
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourMainList[i3].colourMainStart));
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourMainList[i3].colourMainEnd));
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourMainList[i3].colourMainMarker));
                        binaryWriter.Write(groups[i].CellList[i2].ColourMainList[i3].colourMainAlpha);
                        binaryWriter.Write(groups[i].CellList[i2].ColourMainList[i3].colourMainRed);
                        binaryWriter.Write(groups[i].CellList[i2].ColourMainList[i3].colourMainGreen);
                        binaryWriter.Write(groups[i].CellList[i2].ColourMainList[i3].colourMainBlue);

                        //Sub Colours 1
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourSub1List[i3].colourSub1Start));
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourSub1List[i3].colourSub1End));
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourSub1List[i3].colourSub1Marker));
                        binaryWriter.Write(EndianSwap(0x0000001C));

                        //Sub Colours 2
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourSub2List[i3].colourSub2Start));
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourSub2List[i3].colourSub2End));
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourSub2List[i3].colourSub2Marker));
                        binaryWriter.Write(EndianSwap(0x00000001));

                        //End Colours
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourMainList[i3].colourMainStart));
                        binaryWriter.Write(EndianSwap(groups[i].CellList[i2].ColourMainList[i3].colourMainEnd));
                        binaryWriter.Write(EndianSwap(0x00000003));

                        Console.WriteLine("Colour Data Written!");
                    }

                    //Padding
                    binaryWriter.Write(EndianSwap(0x00000000));
                    binaryWriter.Write(EndianSwap(0x00000000));
                    binaryWriter.Write(EndianSwap(0x00000000));
                    Console.WriteLine("Cell Data Written!");
                }

                Console.WriteLine("Group Data Written!");
            }
	        binaryWriter.Close();
            return;
        }

        public static Structs.Highlight Highlight { get; set; }
        public static Structs.Skip Skip { get; set; }
        public static Structs.ColourMain ColourMain { get; set; }
        public static Structs.Cell Cell { get; set; }
        public static Structs.Group Group { get;set;}
    }
}