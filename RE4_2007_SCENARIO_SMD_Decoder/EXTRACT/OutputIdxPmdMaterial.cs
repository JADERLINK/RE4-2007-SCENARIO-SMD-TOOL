using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace RE4_2007_SCENARIO_SMD_EXTRACT
{
    public static class OutputIdxPmdMaterial
    {
        public static void IdxPmdMaterialCreate(Dictionary<string, PmdMaterialPart> mat, string baseDiretory, string baseFileName)
        {

            StreamWriter text = new FileInfo(Path.Combine(baseDiretory, baseFileName + ".idxpmdmaterial")).CreateText();
            text.WriteLine("# youtube.com/@JADERLINK");
            text.WriteLine(Program.HeaderText());
            text.WriteLine("");
            text.WriteLine("");

            foreach (var item in mat)
            {
                text.WriteLine("UseMaterial:" + item.Key);
                text.WriteLine("TextureName:" + item.Value.TextureName);
                text.WriteLine("TextureEnable:" + item.Value.TextureEnable);

                FloatPrint(ref text, "M00_unknown", item.Value.TextureData[00]);
                FloatPrint(ref text, "M01_unknown", item.Value.TextureData[01]);
                FloatPrint(ref text, "M02_unknown", item.Value.TextureData[02]);
                FloatPrint(ref text, "M03_unknown", item.Value.TextureData[03]);
                FloatPrint(ref text, "M04_colorR", item.Value.TextureData[04]);
                FloatPrint(ref text, "M05_colorG", item.Value.TextureData[05]);
                FloatPrint(ref text, "M06_colorB", item.Value.TextureData[06]);
                FloatPrint(ref text, "M07_colorA", item.Value.TextureData[07]);
                FloatPrint(ref text, "M08_unknown", item.Value.TextureData[08]);
                FloatPrint(ref text, "M09_unknown", item.Value.TextureData[09]);
                FloatPrint(ref text, "M10_unknown", item.Value.TextureData[10]);
                FloatPrint(ref text, "M11_unknown", item.Value.TextureData[11]);
                FloatPrint(ref text, "M12_unknown", item.Value.TextureData[12]);
                FloatPrint(ref text, "M13_unknown", item.Value.TextureData[13]);
                FloatPrint(ref text, "M14_unknown", item.Value.TextureData[14]);
                FloatPrint(ref text, "M15_unknown", item.Value.TextureData[15]);
                FloatPrint(ref text, "M16_unknown", item.Value.TextureData[16]);

                text.WriteLine();
                text.WriteLine();
            }

            text.Close();
        }

        private static void FloatPrint(ref StreamWriter text, string key, float value) 
        {
            text.WriteLine(key + ":" + value.ToString("F9", CultureInfo.InvariantCulture));
        }

    }
}
