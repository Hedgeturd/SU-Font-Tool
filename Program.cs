namespace SonicUnleashedFCOConv {
    class Program {
        static void Main(string[] args) {
            if(args.Length == 0) {
                Console.WriteLine("SonicUnleashedFCOConv v1.0\nUsage: SonicUnleashedFCOConv <Path to .fte/.fco/.xml file>");
                return;
            }
            else {
                string file = Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileName(args[0]);

                if(File.Exists(file)) {
                    if (file.EndsWith(".fte")) {
                        FTE.Read(args[0]);
                        if (Common.ErrorCheck() == false) FTE.Write(args[0]);
                    }
                    if (file.EndsWith(".fco")) {
                        FCO.Read(args[0]);
                        if (Common.ErrorCheck() == false) FCO.Write(args[0]);
                    }
                    if (file.EndsWith(".xml")) {
                        XML.Read(args[0]);
                        if (Common.ErrorCheck() == false) XML.Write(args[0]);
                    }

                    Console.WriteLine("\nPress Enter to Exit.");
                    Console.Read();
                }
                else {
                    Console.WriteLine($"Can't find file {file}, aborting");
                    return;
                }
            }
        }
    }
}