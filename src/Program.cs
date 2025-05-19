using System.Text;
using Amicitia.IO.Binary;
using libfco;

namespace SUFontTool {
    class Program {
        public static string? fileDir, fileName, currentDir, tableArg;
        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("SU Font Tool v1.0\nUsage: SUFontTool <Path to .fte/.fco/.xml>");
                Console.ReadKey();
            }

            if (args.Length == 2) {
                currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                fileDir = Path.GetDirectoryName(args[0]);
                fileName = Path.GetFileName(args[0]);
                tableArg = args[1];
                string file = fileDir + "\\" + fileName;
                
                if (file.EndsWith(".fco")) {
                    Console.WriteLine("The 2 Argument Feature of the tool does not work anymore at the moment!");
                    Console.WriteLine("A new Args system will be implemented instead!");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    
                    // This is the old FCO Conv Code
                    //FCO.ReadFCO(args[0]);
                    //FCO.WriteXML(args[0]);
                }
            }
            else {
                currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                fileDir = Path.GetDirectoryName(args[0]);
                fileName = Path.GetFileName(args[0]);
                string file = fileDir + "\\" + fileName;

                if(File.Exists(file)) {
                    if (file.EndsWith(".fte")) {
                        BinaryObjectReader reader = new BinaryObjectReader(args[0], Endianness.Big, Encoding.UTF8);
                        FontTexture fteFile = reader.ReadObject<FontTexture>();
                        
                        //FTE.ReadFTE(args[0]);
                        FTE.WriteXML(args[0], fteFile);
                    }
                    if (file.EndsWith(".fco")) {
                        BinaryObjectReader reader = new BinaryObjectReader(args[0], Endianness.Big, Encoding.UTF8);
                        FontConverse fcoFile = reader.ReadObject<FontConverse>();
                        
                        Common.TableAssignment();
                        TranslationTable thing = TranslationTable.Read(Common.fcoTable);
                        
                        FCO.WriteXML(args[0], fcoFile, thing);
                    }
                    if (file.EndsWith(".xml")) {
                        FontConverse fcoFile = XML.ReadXML(args[0]);
                        string filePath = Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]);
                        File.Delete(Path.Combine(filePath + ".fco"));
                        
                        if (XML.FCO) {
                            if (Common.ErrorCheck() == false) {
                                using BinaryObjectWriter writer = new BinaryObjectWriter(filePath + ".fco", Endianness.Big, Encoding.UTF8);
                                writer.WriteObject(fcoFile);
                                Console.WriteLine("FCO Written");
                            }
                        }
                        else {
                            if (Common.ErrorCheck() == false)
                            {
                                using BinaryObjectWriter writer = new BinaryObjectWriter(filePath + ".fte", Endianness.Big, Encoding.UTF8);
                                writer.WriteObject(fcoFile);
                                Console.WriteLine("FTE Written");
                                //XML.WriteFTE(args[0]);
                            }
                            Common.ExtractCheck();
                        }
                        
                    }
                }
                else {
                    Console.WriteLine($"Can't find file " + Path.GetFileName(args[0]) + ", aborting.");
                }
            }
        }
    }
}