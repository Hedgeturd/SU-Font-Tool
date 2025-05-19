using System.Drawing;
using System.Drawing.Imaging;
using libfco;

namespace SUFontTool {
    public static class DDS {
        public static string outputFolder;

        public static void Process(FontTexture fteFile) {
            outputFolder = Program.fileDir + "/sprites";

            if (Directory.Exists(outputFolder) == false) {
                Directory.CreateDirectory(outputFolder);
            }
            if (Directory.Exists(outputFolder)) {
                foreach (string file in Directory.GetFiles(outputFolder)) {
                    File.Delete(file);
                }
            }

            Console.WriteLine("WARNING: This feature is still Experimental!");
            Console.WriteLine("Provide the following files as PNG images");

            string[] TexturePaths = new string[fteFile.Textures.Count];

            for (int i = 0; i < fteFile.Textures.Count; i++)
            {
                Console.WriteLine("Please provide the path of " + fteFile.Textures[i].Name + ".");
                string ddsPath = Console.ReadLine().Replace("\"", "");

                if (ddsPath != null && Path.GetFileNameWithoutExtension(ddsPath) != null) {
                    TexturePaths[i] = ddsPath;
                }
            }
            
            ReadCharacters(fteFile, TexturePaths);
        }

        public static void ReadCharacters(FontTexture fteFile, string[] TexturePaths) {
            foreach (string TexturePath in TexturePaths)
            {
                Bitmap sourceImage = new Bitmap(TexturePath);
            
                foreach (Character character in fteFile.Characters) {
                    // If our grabbed index for the character matches our input from Process
                    int thing = Array.IndexOf(TexturePaths, TexturePath);
                    if (character.TextureIndex == thing)
                    {
                        float width = fteFile.Textures[thing].Size.X * (character.BottomRight.X - character.TopLeft.X);
                        float height = fteFile.Textures[thing].Size.Y * (character.BottomRight.Y - character.TopLeft.Y);

                        // Extract the sprite from the image
                        // Normal Sized Sprite
                        if (width > 0 && height > 0) {
                            Rectangle spriteRect = new Rectangle(
                                (int)(fteFile.Textures[thing].Size.X * character.TopLeft.X),
                                (int)(fteFile.Textures[thing].Size.Y * character.TopLeft.Y),
                                (int)width, (int)height);
                            Bitmap sprite = sourceImage.Clone(spriteRect, sourceImage.PixelFormat);

                            string outputFilePath = Path.Combine(outputFolder, $"{character.CharacterID.ToString()}.png");

                            sprite.Save(outputFilePath, ImageFormat.Png);
                            sprite.Dispose();
                        }
                        // Blank Sprite or Broken Sprite
                        else {
                            Console.WriteLine($"Character {character.CharacterID.ToString()} may be blank or non-existant.");

                            Rectangle spriteRect = new Rectangle(
                                (int)(fteFile.Textures[thing].Size.X * character.TopLeft.X),
                                (int)(fteFile.Textures[thing].Size.Y * character.TopLeft.Y),
                                1, 1);
                            Bitmap sprite = sourceImage.Clone(spriteRect, sourceImage.PixelFormat);

                            string outputFilePath = Path.Combine(outputFolder, $"{character.CharacterID.ToString()}.png");

                            sprite.Save(outputFilePath, ImageFormat.Png);
                            sprite.Dispose();
                        }
                    }
                    else {
                        /*  The Loop will continue until the correct index is found
                            However this shouldn't be possible with the new code..
                            Unless someone somehow finds a way to cause this lol    */
                    }
                }
            }

            Console.WriteLine("Sprites extracted successfully.");
        }
    }
}