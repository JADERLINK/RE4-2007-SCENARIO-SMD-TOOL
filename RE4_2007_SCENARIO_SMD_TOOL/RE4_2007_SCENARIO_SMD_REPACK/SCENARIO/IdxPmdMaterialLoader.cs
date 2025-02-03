using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace RE4_2007_PMD_REPACK
{
    public class PmdMaterialLine
    {
        public string MaterialName;
        public string TextureName;
        public int TextureEnable;
        public float[] TextureData; // new float[17];

        public PmdMaterialLine() 
        {
            MaterialName = "";
            TextureName = "";
            TextureEnable = 0;
            TextureData = new float[17];
        }
    }

    public static class IdxPmdMaterialLoader
    {
        public static Dictionary<string, PmdMaterialLine> Load(Stream stream)
        {
            StreamReader reader = new StreamReader(stream, Encoding.ASCII);

            Dictionary<string, PmdMaterialLine> MaterialDic = new Dictionary<string, PmdMaterialLine>();

            PmdMaterialLine temp = new PmdMaterialLine();

            while (!reader.EndOfStream)
            {
                string nline = reader.ReadLine().Trim();
                string line = nline.ToUpperInvariant();

                if (line == null || line.Length == 0 || line.StartsWith("\\") || line.StartsWith("/") || line.StartsWith("#") || line.StartsWith(":"))
                {
                    continue;
                }
                else if (line.StartsWith("USEMATERIAL"))
                {
                    temp = new PmdMaterialLine();

                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        string name = split[1].Trim();

                        if (!MaterialDic.ContainsKey(name))
                        {
                            temp.MaterialName = name;
                            MaterialDic.Add(name, temp);
                        }
                    }
                }
                else if (line.StartsWith("TEXTURENAME"))
                {
                    var split = nline.Split(':');
                    if (split.Length >= 2)
                    {
                        temp.TextureName = split[1].Trim();
                    }
                }
                else if (line.StartsWith("TEXTUREENABLE"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.TextureEnable = int.Parse(Utils.ReturnValidDecWithNegativeValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else 
                {
                    _ = SetFloatDec(ref line, "M00_", ref temp.TextureData[00])
                     || SetFloatDec(ref line, "M01_", ref temp.TextureData[01])
                     || SetFloatDec(ref line, "M02_", ref temp.TextureData[02])
                     || SetFloatDec(ref line, "M03_", ref temp.TextureData[03])
                     || SetFloatDec(ref line, "M04_", ref temp.TextureData[04])
                     || SetFloatDec(ref line, "M05_", ref temp.TextureData[05])
                     || SetFloatDec(ref line, "M06_", ref temp.TextureData[06])
                     || SetFloatDec(ref line, "M07_", ref temp.TextureData[07])
                     || SetFloatDec(ref line, "M08_", ref temp.TextureData[08])
                     || SetFloatDec(ref line, "M09_", ref temp.TextureData[09])
                     || SetFloatDec(ref line, "M10_", ref temp.TextureData[10])
                     || SetFloatDec(ref line, "M11_", ref temp.TextureData[11])
                     || SetFloatDec(ref line, "M12_", ref temp.TextureData[12])
                     || SetFloatDec(ref line, "M13_", ref temp.TextureData[13])
                     || SetFloatDec(ref line, "M14_", ref temp.TextureData[14])
                     || SetFloatDec(ref line, "M15_", ref temp.TextureData[15])
                     || SetFloatDec(ref line, "M16_", ref temp.TextureData[16])
                      ;

                }

            }

            stream.Close();

            return MaterialDic;
        }

        private static bool SetFloatDec(ref string line, string key, ref float varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = float.Parse(Utils.ReturnValidFloatValue(split[1]), NumberStyles.Float, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }
    }
}
