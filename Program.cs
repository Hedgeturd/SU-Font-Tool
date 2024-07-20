namespace SonicUnleashedFCOConv {
    class Program {
        public static string fileDir, fileName;
        static void Main(string[] args) {
            if(args.Length == 0) {
                Console.WriteLine("SonicUnleashedFCOConv v1.0\nUsage: SonicUnleashedFCOConv <Path to .fte/.fco/.xml file>");
                return;
            }
            else {
                fileDir = Path.GetDirectoryName(args[0]);
                fileName = Path.GetFileName(args[0]);
                string file = fileDir + "\\" + fileName;

                if(File.Exists(file)) {
                    if (file.EndsWith(".fte")) {
                        FTE.ReadFTE(args[0]);
                        if (Common.ErrorCheck() == false) FTE.WriteXML(args[0]);
                        Common.ExtractCheck();
                    }
                    if (file.EndsWith(".fco")) {
                        FCO.ReadFCO(args[0]);
                        if (Common.ErrorCheck() == false) FCO.WriteXML(args[0]);
                    }
                    if (file.EndsWith(".xml")) {
                        XML.ReadXML(args[0]);
                        //if (Common.ErrorCheck() == false) XML.WriteFCO(args[0]); //REMOVE BCZ FUNCTION HAS BEEN MOVED TO "XML.cs"
                    }
                }
                else {
                    Console.WriteLine($"Can't find file " + Path.GetFileName(args[0]) + ", aborting.");
                }
            }

            Console.WriteLine("\nPress Enter to Exit.");
            Console.Read();
        }
    }
}