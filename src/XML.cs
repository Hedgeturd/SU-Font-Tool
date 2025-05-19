using System.Numerics;
using System.Xml;
using libfco;

namespace SUFontTool {
    public static class XML {
        public static string tableNoName;
        
        // Writing        
        public static void WriteConverseXml(string path, FontConverse fcoFile, TranslationTable thing) {
            File.Delete(Path.Combine(Path.GetFileNameWithoutExtension(path) + ".xml"));

            var xmlWriterSettings = new XmlWriterSettings { Indent = true };
            using var writer = XmlWriter.Create(Path.GetDirectoryName(path) + "\\" +
            Path.GetFileNameWithoutExtension(path) + ".xml", xmlWriterSettings);

            writer.WriteStartDocument();
            writer.WriteStartElement("FCO");
            writer.WriteAttributeString("Table", Common.fcoTableName);      // This is used later once the XML is used to convert the data back into an FCO format
            
            writer.WriteStartElement("Groups");
            
            foreach (Group group in fcoFile.Groups) {
                writer.WriteStartElement("Group");
                writer.WriteAttributeString("Name", group.Name);

                foreach (Cell cell in group.Cells) {
                    writer.WriteStartElement("Cell");
                    writer.WriteAttributeString("Name", cell.Name);                     // These parameters are part of the "Cell" Element Header
                    writer.WriteAttributeString("Alignment", cell.Alignment.ToString());
                                                                                            // The following Elements are all within the "Cell" Element
                    writer.WriteElementString("Message", TranslationService.RawHEXtoTXT(cell.Message, thing.Standard));

                    writer.WriteStartElement("ColourMain");
                    Common.WriteFCOColour(writer, cell.MainColor);
                    writer.WriteEndElement();
                    
                    writer.WriteStartElement("ColourSub1");                                 // Actually figure out what this was again
                    Common.WriteFCOColour(writer, cell.ExtraColor1);
                    writer.WriteEndElement();
                    
                    writer.WriteStartElement("ColourSub2");                                 // Actually figure out what this was again
                    Common.WriteFCOColour(writer, cell.ExtraColor2);
                    writer.WriteEndElement();

                    if (cell.Highlights != null)
                    {
                        writer.WriteStartElement("Highlights");
                        foreach (CellColor highlight in cell.Highlights)
                        {
                            writer.WriteStartElement("Highlight" + cell.Highlights.IndexOf(highlight));
                            Common.WriteFCOColour(writer, highlight);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }

                    if (cell.SubCells != null)
                    {
                        writer.WriteStartElement("SubCells");
                        foreach (SubCell subcell in cell.SubCells)
                        {
                            writer.WriteStartElement("SubCell" + cell.SubCells.IndexOf(subcell));
                            writer.WriteAttributeString("Start", subcell.Start.ToString());
                            writer.WriteAttributeString("End", subcell.End.ToString());
                            writer.WriteString(TranslationService.RawHEXtoTXT(subcell.SubMessage, thing.Standard));
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndDocument();
            writer.Close();

            Console.WriteLine("XML written!");
        }
        
        public static void WriteTextureXml(string path, FontTexture fteFile) {
            File.Delete(Path.Combine(Path.GetFileNameWithoutExtension(path) + ".xml"));

            var xmlWriterSettings = new XmlWriterSettings{ Indent = true };
            using var writer = XmlWriter.Create(Path.GetDirectoryName(path) + "\\" + 
            Path.GetFileNameWithoutExtension(path) + ".xml", xmlWriterSettings);

            writer.WriteStartDocument();
            writer.WriteStartElement("FTE");

            writer.WriteStartElement("Textures");
            foreach(TextureEntry texture in fteFile.Textures) {
                writer.WriteStartElement("Texture");
                writer.WriteAttributeString("Name", texture.Name);
                writer.WriteAttributeString("Size_X", texture.Size.X.ToString());
                writer.WriteAttributeString("Size_Y", texture.Size.Y.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteComment("ConverseID = Hex, Points = Px, Point1 = TopLeft, Point2 = BottomRight");

            writer.WriteStartElement("Characters");
            foreach(Character character in fteFile.Characters) {
                writer.WriteStartElement("Character");
                writer.WriteAttributeString("TextureIndex", character.TextureIndex.ToString());
                writer.WriteAttributeString("CharacterID", character.CharacterID.ToString());
                writer.WriteAttributeString("TopLeft_X", character.TopLeft.X.ToString());
                writer.WriteAttributeString("TopLeft_Y", character.TopLeft.Y.ToString());
                writer.WriteAttributeString("BottomRight_X", character.BottomRight.X.ToString());
                writer.WriteAttributeString("BottomRight_Y", character.BottomRight.Y.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndDocument();
	        writer.Close();

            Console.WriteLine("XML written!");
        }
        
        
        // Reading
        public static int ReadXmlHeader(string path)
        {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + ".xml");
            Common.RemoveComments(xDoc);
            XmlElement? xRoot = xDoc.DocumentElement;

            switch (xRoot.Name)
            {
                case "FCO":
                    return 1;
                case "FTE":
                    return 2;
                default:
                    return 0;
            }
        }

        public static FontConverse ReadConverseXml(string path) {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + ".xml");
            Common.RemoveComments(xDoc);
            XmlElement? xRoot = xDoc.DocumentElement;
            
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

            return fco;
        }

        public static FontTexture ReadTextureXml(string path)
        {
            FontTexture fte = new FontTexture();
            List<TextureEntry> textures = new List<TextureEntry>();
            List<Character> characters = new List<Character>();
            int texCount = 0, charaCount = 0, spriteIndex = 0;
            
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + ".xml");
            Common.RemoveComments(xDoc);
            XmlElement? xRoot = xDoc.DocumentElement;
            
            foreach (XmlElement node in xRoot) {
                if (node.Name == "Textures") { 
                    foreach (XmlElement textureNode in node.ChildNodes)
                    {
                        TextureEntry texture = new TextureEntry()
                        {
                            Name = textureNode.Attributes.GetNamedItem("Name")!.Value!,
                            Size = new Vector2(float.Parse(textureNode.Attributes.GetNamedItem("Size_X")!.Value!),
                                float.Parse(textureNode.Attributes.GetNamedItem("Size_Y")!.Value!))
                        };

                        textures.Add(texture);
                        texCount++;
                    }
                }

                if (node.Name == "Characters") {
                    foreach (XmlElement charaNode in node.ChildNodes) {
                        Character character = new Character() {
                            TextureIndex = int.Parse(charaNode.Attributes.GetNamedItem("TextureIndex")!.Value!),
                            CharacterID = int.Parse(charaNode.Attributes.GetNamedItem("CharacterID")!.Value!),    
                            TopLeft = new Vector2(float.Parse(charaNode.Attributes.GetNamedItem("TopLeft_X")!.Value!),
                                float.Parse(charaNode.Attributes.GetNamedItem("TopLeft_Y")!.Value!)),
                            BottomRight = new Vector2(float.Parse(charaNode.Attributes.GetNamedItem("BottomRight_X")!.Value!),
                                float.Parse(charaNode.Attributes.GetNamedItem("BottomRight_Y")!.Value!))
                        };

                        characters.Add(character);
                        charaCount++;
                    }
                }
            }

            Console.WriteLine("XML read!");

            fte.Header.Field00 = 4;
            fte.Header.Version = 0;
            fte.Textures = textures;
            fte.Characters = characters;
            return fte;
        }
    }
}