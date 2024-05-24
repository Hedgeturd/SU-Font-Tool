using System.Xml;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace SonicUnleashedFCOConv {
    public static class XML {
        public static void XMLtoFCO(string path) {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
			
            // ==================================================================================
            // Reading XML File

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + ".xml");
            Common.RemoveComments(xDoc);
            XmlElement? xRoot = xDoc.DocumentElement;

            List<Structs.Group> groups = new List<Structs.Group>();

            if(xRoot != null && xRoot.Name == "FCO") {

                /* Translator.jsonFilePath = xRoot.Attributes.GetNamedItem("Table")!.Value!; */
                Common.fcoTable = "tables/" + (xRoot.Attributes.GetNamedItem("Table")!.Value!) + ".json";
                Translator.jsonFilePath = Common.fcoTable;

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
                                            string hexString = Translator.TXTtoHEX(cell.CellMessage);

                                            /* cell.CellMessage = messageNode.Attributes.GetNamedItem("MessageData")!.Value!; */
                                            hexString = hexString.Replace(" ", "");
                                            byte[] messageByteArray = Common.StringToByteArray(hexString);
                                            /* hexString = BitConverter.ToString(messageByteArray).Replace("-", ""); */
                                            messageByteArray = Common.StringToByteArray(hexString);

                                            int numberOfBytes = hexString.Length;
                                            cell.MessageCharAmount = numberOfBytes / 8;

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

                                    List<Structs.Highlight> highlights = new List<Structs.Highlight>();

                                    int hightlightcount = 0;

                                    foreach (XmlElement highlightNode in cellNode.ChildNodes)
                                    {
                                        if (highlightNode.Name == "Highlight" + hightlightcount)
                                        {
                                            Structs.Highlight highlight = new Structs.Highlight();

                                            // Cell's name
                                            highlight.highlightStart = int.Parse(highlightNode.Attributes.GetNamedItem("Start")!.Value!);
                                            highlight.highlightEnd = int.Parse(highlightNode.Attributes.GetNamedItem("End")!.Value!);
                                            highlight.highlightMarker = int.Parse(highlightNode.Attributes.GetNamedItem("Marker")!.Value!);

                                            highlight.highlightAlpha = byte.Parse(highlightNode.Attributes.GetNamedItem("Alpha")!.Value!);
                                            highlight.highlightRed = byte.Parse(highlightNode.Attributes.GetNamedItem("Red")!.Value!);
                                            highlight.highlightGreen = byte.Parse(highlightNode.Attributes.GetNamedItem("Green")!.Value!);
                                            highlight.highlightBlue = byte.Parse(highlightNode.Attributes.GetNamedItem("Blue")!.Value!);

                                            highlights.Add(highlight);

                                            cell.HighlightList = highlights;
                                            //Common.skipFlag = true;
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
            }

            // ==================================================================================
            // Writing FCO File

            File.Delete(Path.Combine(filePath + ".fco"));

            Encoder UTF16 = Encoding.Unicode.GetEncoder();

            BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath + ".fco", FileMode.OpenOrCreate));
            
            // Writing first 8 bytes
            binaryWriter.Write(Common.EndianSwap(0x00000004));
            binaryWriter.Write(0x00000000);

            // Group Count
            binaryWriter.Write(Common.EndianSwap(groups.Count));
            for(int i = 0; i < groups.Count; i++) {
                // Group Name
                binaryWriter.Write(Common.EndianSwap(groups[i].GroupName.Length));
                Common.WriteStringWithoutLength(binaryWriter, Common.PadString(groups[i].GroupName, '@'));

                // Cell Count
                binaryWriter.Write(Common.EndianSwap(groups[i].CellList.Count));

                for (int i2 = 0; i2 < groups[i].CellList.Count; i2++) {
                    // Cell Name
                    binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].CellName.Length));
                    Common.WriteStringWithoutLength(binaryWriter, Common.PadString(groups[i].CellList[i2].CellName, '@'));

                    //Message Data
                    binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].MessageCharAmount));
                    binaryWriter.Write(groups[i].CellList[i2].CellMessageWrite);
                    //Console.WriteLine("Message Data Written!");

                    //Colour Start
                    binaryWriter.Write(Common.EndianSwap(0x00000004));

                    for (int i3 = 0; i3 < groups[i].CellList[i2].ColourMainList.Count; i3++) {
                        //Main Colours
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourMainList[i3].colourMainStart));
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourMainList[i3].colourMainEnd));
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourMainList[i3].colourMainMarker));
                        binaryWriter.Write(groups[i].CellList[i2].ColourMainList[i3].colourMainAlpha);
                        binaryWriter.Write(groups[i].CellList[i2].ColourMainList[i3].colourMainRed);
                        binaryWriter.Write(groups[i].CellList[i2].ColourMainList[i3].colourMainGreen);
                        binaryWriter.Write(groups[i].CellList[i2].ColourMainList[i3].colourMainBlue);

                        //Sub Colours 1
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourSub1List[i3].colourSub1Start));
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourSub1List[i3].colourSub1End));
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourSub1List[i3].colourSub1Marker));
                        binaryWriter.Write(Common.EndianSwap(0x0000001C));

                        //Sub Colours 2
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourSub2List[i3].colourSub2Start));
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourSub2List[i3].colourSub2End));
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourSub2List[i3].colourSub2Marker));
                        binaryWriter.Write(Common.EndianSwap(0x00000001));

                        //End Colours
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourMainList[i3].colourMainStart));
                        binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].ColourMainList[i3].colourMainEnd));
                        binaryWriter.Write(Common.EndianSwap(0x00000003));

                        //Console.WriteLine("Colour Data Written!");

                        binaryWriter.Write(Common.EndianSwap(0x00000000));

                        if (groups[i].CellList[i2].HighlightList != null) {
                            binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].HighlightList.Count));

                            for (int i4 = 0; i4 < groups[i].CellList[i2].HighlightList.Count; i4++) {
                                binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].HighlightList[i4].highlightStart));
                                binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].HighlightList[i4].highlightEnd));
                                binaryWriter.Write(Common.EndianSwap(groups[i].CellList[i2].HighlightList[i4].highlightMarker));
                                binaryWriter.Write(groups[i].CellList[i2].HighlightList[i4].highlightAlpha);
                                binaryWriter.Write(groups[i].CellList[i2].HighlightList[i4].highlightRed);
                                binaryWriter.Write(groups[i].CellList[i2].HighlightList[i4].highlightGreen);
                                binaryWriter.Write(groups[i].CellList[i2].HighlightList[i4].highlightBlue);

                                Common.skipFlag = true;
                            }
                        }

                        if (Common.skipFlag == true) {
                            binaryWriter.Write(Common.EndianSwap(0x00000000));
                            //Console.WriteLine("Highlight Data Written!");
                            Common.skipFlag = false;
                        }
                        else {
                            //Padding
                            binaryWriter.Write(Common.EndianSwap(0x00000000));
                            binaryWriter.Write(Common.EndianSwap(0x00000000));
                            //Console.WriteLine("Cell Data Written!");
                        }
                    }
                }

                //Console.WriteLine("Group Data Written!");
            }
	        binaryWriter.Close();

            if (Common.noLetter == true) {
                Console.WriteLine("Some letters in the XML are NOT in the current table and have been removed\nPlease check your XML and the temp file!\n");
            }
            Console.WriteLine("FCO written!\nPress any key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
            return;
        }

        public static Structs.Highlight Highlight { get; set; }
        public static Structs.Skip Skip { get; set; }
        public static Structs.ColourMain ColourMain { get; set; }
        public static Structs.Cell Cell { get; set; }
        public static Structs.Group Group { get;set;}
    }
}