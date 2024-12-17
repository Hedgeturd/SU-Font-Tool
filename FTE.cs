using System.Xml;
using System.Text;

namespace SonicUnleashedFCOConv {
    public static class FTE {
        public static bool structureError = false;
        public static long address;
        public static List<Structs.Texture> textures = new List<Structs.Texture>();
        public static List<Structs.Character> characters = new List<Structs.Character>();
        
        public static void ReadFTE(string path) {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            Encoding Unicode = Encoding.GetEncoding("Unicode");
            Encoding UTF8Encoding = Encoding.GetEncoding("UTF-8");

            // Starting 8 Bytes
            long header = binaryReader.ReadInt64();   // This is always the same
            if (header != 67108864) {
                structureError = true;
                address = binaryReader.BaseStream.Position;
                return;
            }

            // Textures
            int textureCount = Common.EndianSwap(binaryReader.ReadInt32());
            
            for(int i = 0; i < textureCount; i++) {
                Structs.Texture textureData = new Structs.Texture();

                // Texture Name
                textureData.textureName = UTF8Encoding.GetString(binaryReader.ReadBytes(Common.EndianSwap(binaryReader.ReadInt32())));
                Common.SkipPadding(binaryReader);

                textureData.textureSizeX = Common.EndianSwap(binaryReader.ReadInt32());
                textureData.textureSizeY = Common.EndianSwap(binaryReader.ReadInt32());

                textures.Add(textureData);
            }

            // Characters
            int charaCount = Common.EndianSwap(binaryReader.ReadInt32());

            int CurrentID = 100;
            bool IndexChange = false;

            for(int i = 0; i < charaCount; i++) {
                Structs.Character charaData = new Structs.Character();
                
                charaData.textureIndex = Common.EndianSwap(binaryReader.ReadInt32());

                if (charaData.textureIndex == 2 && IndexChange == false) {
                    CurrentID += 100;
                    IndexChange = true;
                }

                charaData.convID = CurrentID.ToString("X8").Insert(2, " ").Insert(5, " ").Insert(8, " ");

                charaData.charPoint1X = textures[charaData.textureIndex].textureSizeX * Common.EndianSwapFloat(binaryReader.ReadSingle());
                charaData.charPoint1Y = textures[charaData.textureIndex].textureSizeY * Common.EndianSwapFloat(binaryReader.ReadSingle());
                charaData.charPoint2X = textures[charaData.textureIndex].textureSizeX * Common.EndianSwapFloat(binaryReader.ReadSingle());
                charaData.charPoint2Y = textures[charaData.textureIndex].textureSizeY * Common.EndianSwapFloat(binaryReader.ReadSingle());

                characters.Add(charaData);
                CurrentID++;
            }
			
            binaryReader.Close();
            Console.WriteLine("FTE Read!");
        }

        public static void WriteXML(string path) {
            File.Delete(Path.Combine(Path.GetFileNameWithoutExtension(path) + ".xml"));

            var xmlWriterSettings = new XmlWriterSettings{ Indent = true };
            using var writer = XmlWriter.Create(Path.GetDirectoryName(path) + "\\" + 
            Path.GetFileNameWithoutExtension(path) + ".xml", xmlWriterSettings);

            writer.WriteStartDocument();
            writer.WriteStartElement("FTE");

            writer.WriteStartElement("Textures");
            foreach(Structs.Texture texture in textures) {
                writer.WriteStartElement("Texture");
                writer.WriteAttributeString("Name", texture.textureName);
                writer.WriteAttributeString("Size_X", texture.textureSizeX.ToString());
                writer.WriteAttributeString("Size_Y", texture.textureSizeY.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteComment("ConverseID = Hex, Points = Px, Point1 = TopLeft, Point2 = BottomRight");

            writer.WriteStartElement("Characters");
            foreach(Structs.Character character in characters) {
                writer.WriteStartElement("Character");
                writer.WriteAttributeString("TextureIndex", character.textureIndex.ToString());
                writer.WriteAttributeString("ConverseID", character.convID);
                writer.WriteAttributeString("Point1_X", character.charPoint1X.ToString());
                writer.WriteAttributeString("Point1_Y", character.charPoint1Y.ToString());
                writer.WriteAttributeString("Point2_X", character.charPoint2X.ToString());
                writer.WriteAttributeString("Point2_Y", character.charPoint2Y.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndDocument();
	        writer.Close();

            textures.Clear();
            characters.Clear();

            Console.WriteLine("XML written!");
        }
    }
}