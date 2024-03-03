using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_2007_PMD_REPACK;

namespace RE4_2007_SCENARIO_SMD_REPACK
{
    class Program
    {
        public static string VERSION = "B.1.0.1.2 (2024-03-03)";

        static void Main(string[] args)
        {
            Console.WriteLine("# github.com/JADERLINK/RE4-2007-SCENARIO-SMD-TOOL");
            Console.WriteLine("# youtube.com/@JADERLINK");
            Console.WriteLine("# RE4_2007_SCENARIO_SMD_TOOL (REPACK) By JADERLINK");
            Console.WriteLine($"# Version {VERSION}");

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-2007-SCENARIO-SMD-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else if (args.Length >= 1 && File.Exists(args[0]))
            {
                var fileinfo = new FileInfo(args[0]); //.IDXSCENARIO

                if (fileinfo.Extension.ToUpper() == ".IDXSCENARIO")
                {
                    Console.WriteLine(fileinfo.Name);

                    Action(fileinfo);
                }
                else
                {
                    Console.WriteLine("Invalid file");
                }

            }
            else
            {
                Console.WriteLine("The file does not exist");
            }

            Console.WriteLine("End");
        }

        private static void Action(FileInfo fileinfo)
        {
            string Diretory = fileinfo.DirectoryName + "\\";
            string basePath = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length);
            string objPath = basePath + ".OBJ";
            string mtlPath = basePath + ".MTL";
            string materialPath = basePath + ".IDXPMDMATERIAL";

            if (File.Exists(objPath))
            {
                try
                {

                    Dictionary<string, PmdMaterialLine> idxMaterial = new Dictionary<string, PmdMaterialLine>();
                    ObjLoader.Loader.Data.Material[] MtlMaterials = new ObjLoader.Loader.Data.Material[0];

                    var idxScenario = IdxScenarioLoader.Loader(new StreamReader(new FileInfo(fileinfo.FullName).OpenRead(), Encoding.ASCII));

                    if (idxScenario.UseIdxPmdMaterial)
                    {
                        if (!File.Exists(materialPath))
                        {
                            Console.WriteLine(new FileInfo(materialPath).Name + " does not exist");
                            return;
                        }
                        else
                        {
                            Stream matFile = new FileInfo(materialPath).OpenRead();
                            idxMaterial = IdxPmdMaterialLoader.Load(matFile);
                        }
                    }

                    if (!File.Exists(mtlPath))
                    {
                        Console.WriteLine(new FileInfo(mtlPath).Name + " does not exist");
                        return;
                    }
                    else
                    {
                        MtlMaterials = PMDrepackMat.LoadMtlMaterials(mtlPath, true);
                    }


                    ScenarioRepack.Repack(objPath, Diretory, idxScenario, idxMaterial, MtlMaterials);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }

            }
            else
            {
                Console.WriteLine(new FileInfo(objPath).Name + " does not exist");
            }

        }



    }

}
