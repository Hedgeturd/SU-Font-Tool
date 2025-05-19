using System.Text;
using Amicitia.IO.Binary;
using libfco;

namespace SUFontTool {
    class Program {
        public static string? fileDir, fileName, currentDir, tableArg;
        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("SU Font Tool v3.0\nUsage: SUFontTool <Path to .fte/.fco/.xml>");
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
                        XML.WriteTextureXml(args[0], fteFile);
                    }
                    if (file.EndsWith(".fco")) {
                        BinaryObjectReader reader = new BinaryObjectReader(args[0], Endianness.Big, Encoding.UTF8);
                        FontConverse fcoFile = reader.ReadObject<FontConverse>();
                        
                        Common.TableAssignment();
                        TranslationTable thing = TranslationTable.Read(Common.fcoTable);
                        
                        XML.WriteConverseXml(args[0], fcoFile, thing);
                    }
                    if (file.EndsWith(".xml")) {
                        string filePath = Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]);
                        string ext = null;
                        int type = XML.ReadXmlHeader(args[0]);
                        
                        switch (type)
                        {
                            case 1:
                                ext = ".fco";
                                break;
                            case 2:
                                ext = ".fte";
                                break;
                            default:
                                Console.WriteLine("Invalid XML format!");
                                return;
                        }
                        
                        File.Delete(Path.Combine(filePath + ext));
                        using BinaryObjectWriter writer = new BinaryObjectWriter(filePath + ext, Endianness.Big, Encoding.UTF8);
                        
                        if (type == 1) {
                            FontConverse fcoFile = XML.ReadConverseXml(args[0]);
                            writer.WriteObject(fcoFile);
                            Console.WriteLine("FCO Written");
                        }
                        if (type == 2)
                        {
                            FontTexture fteFile = null;
                            
                            fteFile = XML.ReadTextureXml(args[0]);
                            writer.WriteObject(fteFile);
                            Console.WriteLine("FTE Written");
                                
                            //Common.ExtractCheck(fteFile);
                        }
                        else
                        {
                            Console.WriteLine("Invalid XML format!");
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