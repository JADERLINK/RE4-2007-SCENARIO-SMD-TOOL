using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_2007_SCENARIO_SMD_EXTRACT
{
    public static class OutputMtl
    {
        public static void MtlCreate(Dictionary<string, PmdMaterialPart> mat, string baseDiretory, string baseFileName)
        {

            StreamWriter text = new FileInfo(Path.Combine(baseDiretory, baseFileName + ".mtl")).CreateText();
            text.WriteLine(Program.HeaderText());;
            text.WriteLine("");
            text.WriteLine("");

            foreach (var item in mat)
            {
              
                text.WriteLine("newmtl " + item.Key);
                text.WriteLine("Ka 1.000 1.000 1.000");
                text.WriteLine("Kd 1.000 1.000 1.000");
                text.WriteLine("Ks 0.000 0.000 0.000");
                text.WriteLine("Ns 0");
                text.WriteLine("d 1");
                text.WriteLine("map_Kd " + item.Value.TextureName);
                text.WriteLine("map_d " + item.Value.TextureName);
                text.WriteLine("");
                text.WriteLine("");
            }

            text.Close();
        }

    }
}
