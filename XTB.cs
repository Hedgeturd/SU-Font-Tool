using System.Xml;
using System.Text;
using System.IO;

namespace SonicUnleashedFCOConv
{
    public static class XTB
    {
        public static void XTBtoXML(string path)
        {
            // ==================================================================================
            // Reading XTB File

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            // Why do we need 2 encodings? Well, the actual text data for the cells are in unicode,
            // when names of cells, categories and styles are in UTF-8
            Encoding Unicode = Encoding.GetEncoding("Unicode");
            Encoding UTF8Encoding = Encoding.GetEncoding("UTF-8");

            binaryReader.ReadInt64();   // Seems like it's always 0xFFFFFFFF
            binaryReader.ReadUInt16();  // Seems like it's always 0x02
            long stylesCount = binaryReader.ReadInt64();

            List<Style> styles = new List<Style>();
            List<Category> categories = new List<Category>();

            // Styles
            for(int i = 0; i < stylesCount; i++)
            {
                Style style = new Style();

                // Name
                int styleNameCharCount = binaryReader.ReadByte();
                style.StyleName = UTF8Encoding.GetString(binaryReader.ReadBytes(styleNameCharCount));
                
                // It seems like it's always 01
                binaryReader.ReadByte();

                // Size
                style.Size = binaryReader.ReadUInt32();

                // Colors
                style.ColorR = binaryReader.ReadByte();
                style.ColorG = binaryReader.ReadByte();
                style.ColorB = binaryReader.ReadByte();

                // Horizontal Alignment
                style.HorizontalAlignment = (HorizontalAlignments)binaryReader.ReadUInt16();

                styles.Add(style);
            }

            // Categories
            long categoriesCount = binaryReader.ReadInt64();
            for(int i = 0; i < categoriesCount; i++)
            {
                Category categoryData = new Category();

                // Name
                int categoryNameCharsCount = binaryReader.ReadByte();
                categoryData.CategoryName = UTF8Encoding.GetString(binaryReader.ReadBytes(categoryNameCharsCount));
                
                // Cells count
                byte CellsCount = binaryReader.ReadByte();

                // Cells
                List<Cell> Cells = new List<Cell>();
                for(int j = 0; j < CellsCount; j++)
                {
                    Cell cellData = new Cell();

                    // Cell's name
                    byte cellNameCharCount = binaryReader.ReadByte();
                    cellData.CellName = UTF8Encoding.GetString(binaryReader.ReadBytes(cellNameCharCount));

                    // Cell's style name
                    int StyleIDCharCount = binaryReader.ReadByte();
                    cellData.CellStyleID = UTF8Encoding.GetString(binaryReader.ReadBytes(StyleIDCharCount));

                    // Cell's expression
                    // For some reason, the count repeats 2 times
                    int ExpressionCharCount = binaryReader.ReadInt32();
                    binaryReader.ReadBytes(4);
                    byte[] ExpressionCharArray = binaryReader.ReadBytes(ExpressionCharCount);
                    cellData.CellExpression = UTF8Encoding.GetString(ExpressionCharArray);

                    Cells.Add(cellData);
                }
                
                // Adding the cell list in the category
                categoryData.CellList = Cells;

                // Adding the category in the categories list
                categories.Add(categoryData);
            }
			
	    binaryReader.Close();

            // ==================================================================================
            // Writing XML File

            var xmlWriterSettings = new XmlWriterSettings{ Indent = true };

            using var writer = XmlWriter.Create(Path.GetDirectoryName(path) + "\\" + 
            Path.GetFileNameWithoutExtension(path) + ".xml", xmlWriterSettings);

            writer.WriteStartDocument();

            writer.WriteStartElement("XTB");

            // Writing styles
            writer.WriteStartElement("Styles");
            foreach(Style style in styles)
            {
                writer.WriteStartElement("Style");

                // Writing style's size
                writer.WriteAttributeString("size", style.Size.ToString());

                // Writing style's ID
                writer.WriteAttributeString("id", style.StyleName); 

                // Writing style's horizontal alignment
                writer.WriteAttributeString("horizontalAlignment", style.HorizontalAlignment.ToString());

                // Writing color R
                writer.WriteAttributeString("colorR", style.ColorR.ToString());

                // Writing color G
                writer.WriteAttributeString("colorG", style.ColorG.ToString());

                // Writing color B
                writer.WriteAttributeString("colorB", style.ColorB.ToString());

                writer.WriteEndElement();
            } 
            writer.WriteEndElement();

            // Categories
            writer.WriteStartElement("Categories");
            foreach(Category category in categories)
            {
                writer.WriteStartElement("Category");
                writer.WriteAttributeString("name", category.CategoryName);

                foreach(Cell cell in category.CellList)
                {
                    writer.WriteStartElement("Cell");
                    writer.WriteAttributeString("name", cell.CellName);
                    writer.WriteAttributeString("style", cell.CellStyleID);
                    writer.WriteAttributeString("expression", cell.CellExpression);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
			
	    writer.Close();
            
            return;
        }

        public static void XMLtoXTB(string path)
        {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
			
            // ==================================================================================
            // Reading XML File

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + ".xml");
            XmlElement? xRoot = xDoc.DocumentElement;

            List<Style> styles = new List<Style>();
            List<Category> categories = new List<Category>();

            if(xRoot != null)
            {
                foreach(XmlElement node in xRoot)
                {
                    if(node.Name == "Styles")
                    {
                        foreach(XmlElement styleNode in node.ChildNodes)
                        {
                            Style style = new Style();

                            // Style's size
                            style.Size = Convert.ToUInt16(styleNode.Attributes.GetNamedItem("size")!.Value!);

                            // Style's name
                            style.StyleName = styleNode.Attributes.GetNamedItem("id")!.Value!;

                            // Style's horizontal alignment
                            style.HorizontalAlignment = Enum.Parse<HorizontalAlignments>(styleNode.Attributes.GetNamedItem("horizontalAlignment")!.Value!);

                            // Style's color R
                            style.ColorR = Convert.ToByte(styleNode.Attributes.GetNamedItem("colorR")!.Value!);

                            // Style's color G
                            style.ColorG = Convert.ToByte(styleNode.Attributes.GetNamedItem("colorG")!.Value!);

                            // Style's color B
                            style.ColorB = Convert.ToByte(styleNode.Attributes.GetNamedItem("colorB")!.Value!);

                            styles.Add(style);
                        }
                    }

                    // Categories
                    if(node.Name == "Categories")
                    {
                        foreach(XmlElement categoryNode in node.ChildNodes)
                        {
                            Category category = new Category();

                            // Category's name
                            category.CategoryName = categoryNode.Attributes.GetNamedItem("name")!.Value!;

                            List<Cell> Cells = new List<Cell>();

                            foreach(XmlElement cellNode in categoryNode.ChildNodes)
                            {
                                if(cellNode.Name == "Cell")
                                {
                                    Cell cell = new Cell();

                                    // Cell's name
                                    cell.CellName = cellNode.Attributes.GetNamedItem("name")!.Value!;

                                    // Cell's style ID
                                    cell.CellStyleID = cellNode.Attributes.GetNamedItem("style")!.Value!;

                                    // Cell's expression
                                    cell.CellExpression = cellNode.Attributes.GetNamedItem("expression")!.Value!;

                                    Cells.Add(cell);
                                }
                                category.CellList = Cells;
                            }
                            categories.Add(category);
                        }
                    }
                }
            }

            // ==================================================================================
            // Writing XTB File

            Encoder UTF16 = Encoding.Unicode.GetEncoder();

            BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath + ".xtb", FileMode.OpenOrCreate));
            
            // Writing first 10 bytes
            binaryWriter.Write(0xFFFFFFFFFFFFFFFF);
            binaryWriter.Write((short)0x0002);

            binaryWriter.Write((long)styles.Count);

            // Styles
            for(int i = 0; i < styles.Count; i++)
            {
                // Style's name 
                // When we're writing style's name, the first byte is name's length, so we don't need
                // this line
                // binaryWriter.Write(Convert.ToByte(styles[i].StyleName.Length));
                binaryWriter.Write(styles[i].StyleName);

                // 01 byte
                binaryWriter.Write((byte)1);

                // Style's size
                binaryWriter.Write(styles[i].Size);

                // Style's colors
                // R
                binaryWriter.Write((byte)styles[i].ColorR);
                // G
                binaryWriter.Write((byte)styles[i].ColorG);
                // B
                binaryWriter.Write((byte)styles[i].ColorB);

                // Style's horizontal alignment
                binaryWriter.Write(Convert.ToInt16(styles[i].HorizontalAlignment));
            }

            // Categories
            binaryWriter.Write((long)categories.Count);
            for(int i = 0; i < categories.Count; i++)
            {
                // Name
                binaryWriter.Write(categories[i].CategoryName);

                // Cells count
                binaryWriter.Write((byte)categories[i].CellList.Count);

                for(int i2 = 0; i2 < categories[i].CellList.Count; i2++)
                {
                    // Cell's name
                    binaryWriter.Write(categories[i].CellList[i2].CellName);

                    // Cell's style
                    binaryWriter.Write(categories[i].CellList[i2].CellStyleID);

                    // Cell's expression length
                    binaryWriter.Write(categories[i].CellList[i2].CellExpression.Length);
                    binaryWriter.Write(categories[i].CellList[i2].CellExpression.Length);

                    // Cell's expression
                    char[] expression = categories[i].CellList[i2].CellExpression.ToCharArray();
                    binaryWriter.Write(expression);
                }
            }
	        binaryWriter.Close();
            return;
        }

        public struct Style
        {
            public string StyleName {get;set;}
            public uint Size {get;set;}
            public byte ColorR {get;set;}
            public byte ColorG {get;set;}
            public byte ColorB{get;set;}
            public HorizontalAlignments HorizontalAlignment {get;set;}
        }
            
        public struct Cell
        {
            public string CellName {get;set;}
            public string CellStyleID {get;set;}
            public string CellExpression {get;set;}
        }
            
        public struct Category
        {
            public string CategoryName {get;set;}
            public List<Cell> CellList {get;set;}
        }

        public enum HorizontalAlignments : byte
        {
            Left = 0, 
            Centered = 1, 
            Right = 2, 
            Distributed = 3
        };
    }
}
