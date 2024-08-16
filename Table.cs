using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Drawing;

namespace SonicUnleashedFCOConv {
    public static class Table {
        public static List<Structs.Character> characters = XML.characters;
        public class ConversionEntry {
            public string Letter { get; set; }
            public string HexString { get; set; }
        }
        public static void WriteJSON() {

            List<ConversionEntry> conversionTable = new List<ConversionEntry>();

            foreach (Structs.Character character in characters) {
                conversionTable.Add(new ConversionEntry { Letter = "{FILL}", HexString = character.CharID });
            }

            // Convert the list to JSON and write to a file
            string json = JsonSerializer.Serialize(conversionTable, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("conversionTable.json", json);

            Console.WriteLine("JSON file created successfully.");
        }
    }
}