using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_2007_SCENARIO_SMD_Extractor
{
    class Program
    {
        public static string VERSION = "B.1.0.0.2 (2024-02-11)";

        static void Main(string[] args)
        {
            Console.WriteLine("## RE4_2007_SCENARIO_SMD_Extractor ##");
            Console.WriteLine($"## Version {VERSION} ##");
            Console.WriteLine("## By JADERLINK ##");

            //args[0] = SMD file
            //args[1] = Diretory for PMD files
            //args[2] = PMD files base name

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-2007-SCENARIO-SMD-TOOL");
            }
            else if (args.Length >= 3
                && File.Exists(args[0])
                && args[2].Length != 0
                && Directory.Exists(args[1]))
            {
                FileInfo fileInfo = new FileInfo(args[0]);
                Console.WriteLine(fileInfo.Name);

                if (fileInfo.Extension.ToUpper() == ".SMD")
                {
                    string Diretory = args[1];
                    if (Diretory.Last() != '\\')
                    {
                        Diretory += "\\";
                    }

                    string basePmdName = args[2].Trim();
                    if (basePmdName.Last() == '_')
                    {
                        basePmdName = basePmdName.TrimEnd('_');
                    }

                    string baseFileName = basePmdName + ".scenario";


                    try
                    {

                        BinRenderBox[] renderBoxes;

                        SMDLine[] SMDLines = SmdExtract.Extrator(fileInfo.FullName, out renderBoxes);

                        var pmds = Scenario.GetPmds(Diretory, basePmdName, SMDLines.Length);

                        Scenario.CreateOBJ(SMDLines, pmds, Diretory, baseFileName, true);

                        Scenario.CreateMTL(pmds, Diretory, baseFileName);

                        Scenario.CreateDrawDistanceObj(SMDLines, renderBoxes, Diretory, baseFileName);
                        Scenario.CreateIdxScenario(SMDLines, pmds, Diretory, baseFileName, fileInfo.Name, basePmdName, renderBoxes);


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                    }

                }
                else
                {
                    Console.WriteLine("it is not an SMD file");
                }
            }
            else
            {
                Console.WriteLine("Invalid arguments or invalid file");
            }

            Console.WriteLine("end");
        }
    }
}
