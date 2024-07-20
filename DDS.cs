using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;

namespace SonicUnleashedFCOConv {
    public static class DDS {
        public static int texCount = 0, charaCount = 0, spriteIndex = 0;
        public static List<Structs.Texture> textures = new List<Structs.Texture>();
        public static List<Structs.Character> characters = new List<Structs.Character>();

        public static void Process(string path) {
            ReadXML(path);

            foreach(Structs.Texture texture in textures) {
                Console.WriteLine("Please input the path of " + texture.TextureName + ".");

                string ddsPath = Console.ReadLine();
                string ddsname = Path.GetFileNameWithoutExtension(ddsPath);

                if (ddsPath != null && ddsname != null) {
                    int index = textures.FindIndex(0, texCount, texture => texture.TextureName == ddsname);
                    ReadDDS(ddsPath, index);
                }
            }

            return;
        }

        public static void ReadXML(string path) {
            string filePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath + ".xml");
            Common.RemoveComments(xDoc);
            XmlElement? xRoot = xDoc.DocumentElement;

            if (xRoot != null && xRoot.Name == "FTE") {
                foreach (XmlElement node in xRoot) {
                    if (node.Name == "Textures") { 
                        foreach (XmlElement textureNode in node.ChildNodes) {
                            Structs.Texture texture = new Structs.Texture() {
                                TextureName = textureNode.Attributes.GetNamedItem("Name")!.Value!,
                                TextureSizeX = int.Parse(textureNode.Attributes.GetNamedItem("Size_X")!.Value!),
                                TextureSizeY = int.Parse(textureNode.Attributes.GetNamedItem("Size_Y")!.Value!),
                            };

                            textures.Add(texture);
                            texCount++;
                        }
                    }

                    if (node.Name == "Characters") {
                        foreach (XmlElement charaNode in node.ChildNodes) {
                            Structs.Character character = new Structs.Character() {
                                TextureIndex = int.Parse(charaNode.Attributes.GetNamedItem("TextureIndex")!.Value!),
                                CharID = charaNode.Attributes.GetNamedItem("ConverseID")!.Value!,
                                CharPoint1X = int.Parse(charaNode.Attributes.GetNamedItem("Point1_X")!.Value!),
                                CharPoint1Y = int.Parse(charaNode.Attributes.GetNamedItem("Point1_Y")!.Value!),
                                CharPoint2X = int.Parse(charaNode.Attributes.GetNamedItem("Point2_X")!.Value!),
                                CharPoint2Y = int.Parse(charaNode.Attributes.GetNamedItem("Point2_Y")!.Value!),
                            };

                            characters.Add(character);
                            charaCount++;
                        }
                    }
                }
            }

            Console.WriteLine("XML read!");
            return;
        }

        public static void ReadDDS(string filePath, int extTexIndex) {
            string outputFolder = Program.fileDir + "/sprites";
            if (Directory.Exists(outputFolder) == false) {
                Directory.CreateDirectory(outputFolder);
            }
            if (Directory.Exists(outputFolder)) {
                foreach (string file in Directory.GetFiles(outputFolder)) {
                    File.Delete(file);
                }
            }

            Bitmap sourceImage = new Bitmap(filePath);
            
            foreach (var chara in characters) {
                // If the Character count has been hit/exceeded
                if (spriteIndex == charaCount) {
                    break;
                }

                int textureIndex = characters[spriteIndex].TextureIndex;

                // If our grabbed index for the character matches our input from Process
                if (textureIndex == extTexIndex) {
                    string ConverseID = characters[spriteIndex].CharID.ToString();
                    int p1_x = (int)characters[spriteIndex].CharPoint1X;
                    int p1_y = (int)characters[spriteIndex].CharPoint1Y;
                    int p2_x = (int)characters[spriteIndex].CharPoint2X;
                    int p2_y = (int)characters[spriteIndex].CharPoint2Y;

                    int width = p2_x - p1_x;
                    int height = p2_y - p1_y;

                    // Extract the sprite from the image
                    // Normal Sized Sprite
                    if (width > 0 && height > 0) {
                        Rectangle spriteRect = new Rectangle(p1_x, p1_y, width, height);
                        Bitmap sprite = sourceImage.Clone(spriteRect, sourceImage.PixelFormat);

                        string outputFilePath = Path.Combine(outputFolder, $"{ConverseID}.png");

                        sprite.Save(outputFilePath, ImageFormat.Png);
                        spriteIndex++;
                        sprite.Dispose();
                    }
                    // Blank Sprite or Broken Sprite
                    else {
                        Console.WriteLine($"Character {ConverseID} may be blank or non-existant.");

                        Rectangle spriteRect = new Rectangle(p1_x, p1_y, 1, 1);
                        Bitmap sprite = sourceImage.Clone(spriteRect, sourceImage.PixelFormat);

                        string outputFilePath = Path.Combine(outputFolder, $"{ConverseID}.png");

                        sprite.Save(outputFilePath, ImageFormat.Png);
                        spriteIndex++;
                        sprite.Dispose();
                    }
                }
                else {
                    /*  The Loop will continue until the correct index is found
                        However this shouldn't be possible with the new code..
                        Unless someone somehow finds a way to cause this lol    */
                }
            }

            Console.WriteLine("Sprites extracted successfully.");
        }
    }
}