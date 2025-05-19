using System.Text.Json;
using libfco;

namespace SUFontTool {
    public static class Table {
        public class ConversionEntry {
            public string? letter { get; set; }
            public string? hexString { get; set; }
        }
        public static void WriteJSON(FontTexture fteFile) {

            List<ConversionEntry> conversionTable = new List<ConversionEntry>();

            string? userInput;

            foreach (Character character in fteFile.Characters) {
                Console.WriteLine(character.CharacterID + ": ");
                userInput = Console.ReadLine();
                if (userInput == null) {
                    userInput = "{FILL}";
                }
                conversionTable.Add(new ConversionEntry { letter = userInput, hexString = character.CharacterID.ToString() });
            }

            // Convert the list to JSON and write to a file
            string tableData = JsonSerializer.Serialize(conversionTable, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("conversionTable.json", tableData);

            Console.WriteLine("JSON file created successfully.");
        }
    }
}