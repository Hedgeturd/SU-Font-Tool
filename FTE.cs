using System.Xml;
using System.Text;

namespace SonicUnleashedFCOConv {
    public static class FTE {
        public static List<Structs.Texture> textures = new List<Structs.Texture>();
        public static List<Structs.Character> characters = new List<Structs.Character>();
        
        public static void ReadFTE(string path) {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            Encoding Unicode = Encoding.GetEncoding("Unicode");
            Encoding UTF8Encoding = Encoding.GetEncoding("UTF-8");

            // Starting 8 Bytes
            binaryReader.ReadInt64();

            // Textures
            int textureCount = Common.EndianSwap(binaryReader.ReadInt32());
            
            for(int i = 0; i < textureCount; i++) {
                Structs.Texture textureData = new Structs.Texture();

                // Texture Name
                textureData.TextureName = UTF8Encoding.GetString(binaryReader.ReadBytes(Common.EndianSwap(binaryReader.ReadInt32())));
                Common.SkipPadding(binaryReader);

                textureData.TextureSizeX = Common.EndianSwap(binaryReader.ReadInt32());
                textureData.TextureSizeY = Common.EndianSwap(binaryReader.ReadInt32());

                textures.Add(textureData);
            }

            // Characters
            int charaCount = Common.EndianSwap(binaryReader.ReadInt32());

            int CurrentID = 100;
            bool IndexChange = false;

            for(int i = 0; i < charaCount; i++) {
                Structs.Character charaData = new Structs.Character();
                
                charaData.TextureIndex = Common.EndianSwap(binaryReader.ReadInt32());

                if (charaData.TextureIndex == 2 && IndexChange == false) {
                    CurrentID = 246;
                    IndexChange = true;
                }

                charaData.CharID = CurrentID.ToString("X8");

                charaData.CharPoint1X = textures[charaData.TextureIndex].TextureSizeX * Common.EndianSwapFloat(binaryReader.ReadSingle());
                charaData.CharPoint1Y = textures[charaData.TextureIndex].TextureSizeY * Common.EndianSwapFloat(binaryReader.ReadSingle());
                charaData.CharPoint2X = textures[charaData.TextureIndex].TextureSizeX * Common.EndianSwapFloat(binaryReader.ReadSingle());
                charaData.CharPoint2Y = textures[charaData.TextureIndex].TextureSizeY * Common.EndianSwapFloat(binaryReader.ReadSingle());

                characters.Add(charaData);
                CurrentID++;
            }
			
            binaryReader.Close();
            Console.WriteLine("FTE Read!");
            return;
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
                writer.WriteAttributeString("Name", texture.TextureName);
                writer.WriteAttributeString("Size_X", texture.TextureSizeX.ToString());
                writer.WriteAttributeString("Size_Y", texture.TextureSizeY.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteComment("ConverseID = Hex, Points = Px, Point1 = TopLeft, Point2 = BottomRight");

            writer.WriteStartElement("Characters");
            foreach(Structs.Character character in characters) {
                writer.WriteStartElement("Character");
                writer.WriteAttributeString("TextureIndex", character.TextureIndex.ToString());
                writer.WriteAttributeString("ConverseID", character.CharID);
                writer.WriteAttributeString("Point1_X", character.CharPoint1X.ToString());
                writer.WriteAttributeString("Point1_Y", character.CharPoint1Y.ToString());
                writer.WriteAttributeString("Point2_X", character.CharPoint2X.ToString());
                writer.WriteAttributeString("Point2_Y", character.CharPoint2Y.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndDocument();
	        writer.Close();

            textures.Clear();
            characters.Clear();

            Console.WriteLine("XML written!");
            return;
        }
    }
}