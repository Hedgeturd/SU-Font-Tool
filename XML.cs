using System.Xml;
using System.Text;

namespace SonicUnleashedFCOConv {
    public static class XML {
        public static bool returnEarly = false;
        public static List<Structs.Group> groups = new List<Structs.Group>();
        public static void ReadXML(string path) {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + ".xml");
            Common.RemoveComments(xDoc);
            XmlElement? xRoot = xDoc.DocumentElement;

            if (xRoot != null && xRoot.Name == "FCO") {
                Common.fcoTable = "tables/" + (xRoot.Attributes.GetNamedItem("Table")!.Value!) + ".json";
                Translator.jsonFilePath = Common.fcoTable;

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
                                    foreach (XmlElement messageNode in cellNode.ChildNodes) {
                                        if (messageNode.Name == "Message") {
                                            cell.CellMessage = messageNode.Attributes.GetNamedItem("MessageData")!.Value!;
                                            string hexString = Translator.TXTtoHEX(cell.CellMessage);

                                            hexString = hexString.Replace(" ", "");
                                            byte[] messageByteArray = Common.StringToByteArray(hexString);
                                            messageByteArray = Common.StringToByteArray(hexString);

                                            int numberOfBytes = hexString.Length;
                                            cell.MessageCharAmount = numberOfBytes / 8;

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
                if (Common.ErrorCheck()  == false) WriteFCO(filePath);
            }

            if (xRoot != null && xRoot.Name == "FTE") {
                Console.WriteLine("Program recognised XML as FTE Format");
                Common.ExtractCheck();
            }

            return;
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
                        //Main Colours
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourMainList[a].colourMainStart));
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourMainList[a].colourMainEnd));
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourMainList[a].colourMainMarker));
                        binaryWriter.Write(groups[g].CellList[c].ColourMainList[a].colourMainAlpha);
                        binaryWriter.Write(groups[g].CellList[c].ColourMainList[a].colourMainRed);
                        binaryWriter.Write(groups[g].CellList[c].ColourMainList[a].colourMainGreen);
                        binaryWriter.Write(groups[g].CellList[c].ColourMainList[a].colourMainBlue);

                        //Sub Colours 1
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourSub1List[a].colourSub1Start));
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourSub1List[a].colourSub1End));
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourSub1List[a].colourSub1Marker));
                        binaryWriter.Write(groups[g].CellList[c].ColourSub1List[a].colourSub1Alpha);
                        binaryWriter.Write(groups[g].CellList[c].ColourSub1List[a].colourSub1Red);
                        binaryWriter.Write(groups[g].CellList[c].ColourSub1List[a].colourSub1Green);
                        binaryWriter.Write(groups[g].CellList[c].ColourSub1List[a].colourSub1Blue);

                        //Sub Colours 2
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourSub2List[a].colourSub2Start));
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourSub2List[a].colourSub2End));
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourSub2List[a].colourSub2Marker));
                        binaryWriter.Write(groups[g].CellList[c].ColourSub2List[a].colourSub2Alpha);
                        binaryWriter.Write(groups[g].CellList[c].ColourSub2List[a].colourSub2Red);
                        binaryWriter.Write(groups[g].CellList[c].ColourSub2List[a].colourSub2Green);
                        binaryWriter.Write(groups[g].CellList[c].ColourSub2List[a].colourSub2Blue);

                        //End Colours
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourMainList[a].colourMainStart));
                        binaryWriter.Write(Common.EndianSwap(groups[g].CellList[c].ColourMainList[a].colourMainEnd));
                        binaryWriter.Write(Common.EndianSwap(0x00000003));
                        binaryWriter.Write(Common.EndianSwap(0x00000000));

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

                        if (Common.skipFlag == true) {
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

            /* if (Common.noLetter == true) {
                Console.WriteLine("Some letters in the XML are NOT in the current table and have been removed!");
                Console.WriteLine("Please check your XML and the temp file!");
            } */

            Console.WriteLine("FCO written!");
            return;
        }
    }
}