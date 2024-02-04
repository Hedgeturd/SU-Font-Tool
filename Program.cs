namespace SonicUnleashedFCOConv {
    class Program {
        static void Main(string[] args) {
            if(args.Length == 0) {
                Console.WriteLine("SonicUnleashedFCOConv v0.1\nUsage: SonicUnleashedFCOConv <source file>");
                return;
            }
            else {
                string file = Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileName(args[0]);

                if(File.Exists(file)) {
                    if (file.EndsWith(".fco")) {
                        FCO.FCOtoXML(args[0]);
                    }
                    if (file.EndsWith(".xml")) {
                        XML.XMLtoFCO(args[0]);
                    }
                }
                else {
                    Console.WriteLine($"Can't find file {file}, aborting");
                    return;
                }


            }
        }
    }
}