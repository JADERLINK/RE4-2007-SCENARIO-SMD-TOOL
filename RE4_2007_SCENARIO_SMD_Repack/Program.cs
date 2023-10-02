using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_2007_SCENARIO_SMD_Repack
{
    class Program
    {
        public static string VERSION = "B.1.0.0.0";

        static void Main(string[] args)
        {

            Console.WriteLine("## RE4_2007_SCENARIO_SMD_Repack ##");
            Console.WriteLine($"## Version {VERSION} ##");
            Console.WriteLine("## By JADERLINK ##");

            if (args.Length >= 1 && File.Exists(args[0])
                && (new FileInfo(args[0]).Extension.ToUpper() == ".IDXSCENARIO"))

            {
                var fileinfo = new FileInfo(args[0]); //.IDXSCENARIO

                string objPath = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".OBJ";
                string mtlPath = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".MTL";
                //string drawdistancePath = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".DRAWDISTANCE.OBJ"; // não usado

                string Diretory = fileinfo.DirectoryName + "\\";

                Console.WriteLine(args[0]);

                if (File.Exists(objPath))
                {
                    if (File.Exists(mtlPath))
                    {
                        try
                        {
                            ScenarioRepack.Repack(objPath, mtlPath, fileinfo.FullName, Diretory);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex);
                        }
                    }
                    else
                    {
                        Console.WriteLine(mtlPath + " does not exist");
                    }

                }
                else
                {
                    Console.WriteLine(objPath + " does not exist");
                }


            }
            else
            {
                Console.WriteLine("no arguments or invalid file");
            }

            Console.WriteLine("End");



        }



    }
}
