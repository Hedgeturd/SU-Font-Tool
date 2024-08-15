using System.Text.Json;

namespace SonicUnleashedFCOConv {
    class Translator {
        // To whomever this may concern, I legit don't even know how I wrote some of this.. It may have been a 2AM job...
        public static string? jsonFilePath = Common.fcoTable;
        public static List<string> missinglist = new List<string>();

        public static string HEXtoTXT(String hex) {
            List<string> chunks = SplitStringIntoChunks("HEXtoTXT", hex, 11);
            return JoinChunks(chunks);
        }

        public static string TXTtoHEX(String hex) {
            List<string> chunks = SplitStringIntoChunks("TXTtoHEX", hex, 1);
            return JoinChunks(chunks);
        }

        static List<string> SplitStringIntoChunks(string mode, string str, int chunkSize) {
            List<string> chunks = new List<string>();

            int i2 = 0;

            if (mode == "HEXtoTXT") {
                for (int i = 0; i < str.Length; i += chunkSize + 1) {
                    if (i + chunkSize > str.Length) {
                        chunks.Add(JSONRead(mode, str.Substring(i)));
                    }
                    else {
                        chunks.Add(JSONRead(mode, str.Substring(i, chunkSize)));
                    }
                }
            }
            if (mode == "TXTtoHEX") {
                while (i2 < str.Length) {
                    if (str[i2] == '{') {
                        // Find the closing '}'
                        int endIndex = str.IndexOf('}', i2);
                        if (endIndex != -1) {
                            // Include the closing '}' in the chunk
                            int specialChunkLength = endIndex - i2 + 1;
                            chunks.Add(JSONRead(mode, str.Substring(i2, specialChunkLength)));
                            i2 += specialChunkLength; // Move the index past this special chunk
                            continue;
                        }
                    }
                    else {
                        int endIndex = i2 + chunkSize;
                        if (endIndex > str.Length) {
                            endIndex = str.Length;
                        }
                        else {
                            // Ensure not to break in the middle of a "{X}", "{XY}" or "{XYZ}"
                            int nextSpecialIndex = str.IndexOf("{", i2, chunkSize);
                            if (nextSpecialIndex != -1 && nextSpecialIndex < endIndex) {
                                endIndex = nextSpecialIndex;
                            }
                        }

                        string chunk = str.Substring(i2, endIndex - i2);
                        chunks.Add(JSONRead(mode, chunk));
                        i2 = endIndex;
                    }
                }
            }

            return chunks;
        }

        static string JoinChunks(List<string> chunks) {
            return string.Join("", chunks);
        }

        static string JSONRead(string searchMode, string hexString) {
            // Read the JSON file
            // Parse the JSON string into a JsonDocument

            if (File.Exists(jsonFilePath) == false) {
                Console.WriteLine("\nThis table does not exist\nPlease check your files!\nPress any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(jsonFilePath));

            if (searchMode == "HEXtoTXT") {
                string searchResult = SearchHexStringForLetter(doc.RootElement, hexString);

                if (searchResult == "?MISSING?") {
                    jsonFilePath = Program.currentDir + "/tables/Icons.json";
                    using JsonDocument docIcon = JsonDocument.Parse(File.ReadAllText(jsonFilePath));
                    searchResult = SearchHexStringForLetter(docIcon.RootElement, hexString);
                    jsonFilePath = Common.fcoTable;

                    if (searchResult == "?MISSING?") {
                        Common.noLetter = true;
                        missinglist.Add("hexString: " + hexString + " not found in the table " + Common.tableName + "!");
                    }

                    return searchResult;
                }
                else {
                    if (searchResult == "?MISSING") {
                        Common.noLetter = true;
                        missinglist.Add("hexString: " + hexString + " not found in the table " + Common.tableName + "!");
                    }

                    return searchResult;
                }
            }
            if (searchMode == "TXTtoHEX") {
                if (hexString.Contains("{")) {
                    string searchResult = SearchLetterForHexString(doc.RootElement, hexString);
                    
                    if (searchResult == null) {
                        jsonFilePath = Program.currentDir + "/tables/Icons.json";
                        using JsonDocument docIcon = JsonDocument.Parse(File.ReadAllText(jsonFilePath));
                        searchResult = SearchLetterForHexString(docIcon.RootElement, hexString);
                        jsonFilePath = Common.fcoTable;

                        if (searchResult == null) {
                            Common.noLetter = true;
                            missinglist.Add("letter: " + hexString + " not found in the table " + Common.tableName + "!");
                        }

                        return searchResult;
                    }

                    return searchResult;
                }
                else {
                    string searchResult = SearchLetterForHexString(doc.RootElement, hexString);

                    if (searchResult == null) {
                        Common.noLetter = true;
                        missinglist.Add("letter: " + hexString + " not found in the table " + Common.tableName + "!");
                    }

                    return searchResult;
                }
            }

            return null;
        }

        static string SearchHexStringForLetter(JsonElement element, string searchHexString) {
            if (element.ValueKind == JsonValueKind.Array) {
                foreach (JsonElement item in element.EnumerateArray()) {
                    if (item.TryGetProperty("hexString", out JsonElement hexStringElement) && hexStringElement.GetString().Equals(searchHexString)) {
                        if (item.TryGetProperty("letter", out JsonElement letterElement)) {
                            string? letter = letterElement.GetString();
                            /* switch (letter) {
                                case "newline":
                                    letter = "{NewLine}";
                                    break;
                                default:
                                    break;
                            } */

                            return letter;
                        }
                    }
                }
            }
            else if (element.ValueKind == JsonValueKind.Object) {
                foreach (JsonProperty property in element.EnumerateObject()) {
                    if (property.Value.ValueKind == JsonValueKind.Array || property.Value.ValueKind == JsonValueKind.Object) {
                        string result = SearchHexStringForLetter(property.Value, searchHexString);
                        if (result != null) {
                            return result; // Return the result immediately after finding the first match
                        }
                    }
                }
            }

            return "?MISSING?"; // Return null if the hexString is not found
        }

        static string SearchLetterForHexString(JsonElement element, string searchLetter)
        {
            /* if (searchLetter == "\n") {
                searchLetter = "newline";
            }*/
            if (element.ValueKind == JsonValueKind.Array) {
                foreach (JsonElement item in element.EnumerateArray()) {
                    if (item.TryGetProperty("letter", out JsonElement letterElement) && letterElement.GetString().Equals(searchLetter)) {
                        if (item.TryGetProperty("hexString", out JsonElement hexStringElement)) {
                            return hexStringElement.GetString();
                        }
                    }
                }
            }
            else if (element.ValueKind == JsonValueKind.Object) {
                foreach (JsonProperty property in element.EnumerateObject()) {
                    if (property.Value.ValueKind == JsonValueKind.Array || property.Value.ValueKind == JsonValueKind.Object) {
                        string result = SearchLetterForHexString(property.Value, searchLetter);
                        if (result != null) {
                            return result; // Return the result immediately after finding the first match
                        }
                    }
                }
            }

            return null; // Return null if the hexString is not found
        }
    }
}