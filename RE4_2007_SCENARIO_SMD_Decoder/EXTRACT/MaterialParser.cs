using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMD_API;

namespace RE4_2007_SCENARIO_SMD_EXTRACT
{
    public static class MaterialParser
    {
        public static Dictionary<string, PmdMaterialPart> Parser(PMD[] pmds, out string[][] Materialref)
        {
            Materialref = new string[pmds.Length][];

            Dictionary<PmdMaterialPart, string> MaterialDic = new Dictionary<PmdMaterialPart, string>();

            Dictionary<string, int> materialNameList = new Dictionary<string, int>();

            for (int i = 0; i < pmds.Length; i++)
            {
                Materialref[i] = new string[pmds[i].Materials.Length];
                for (int im = 0; im < pmds[i].Materials.Length; im++)
                {
                    string TextureName = pmds[i].Materials[im].TextureName.ToLowerInvariant();
                    if (TextureName == null || TextureName.Length == 0)
                    {
                        TextureName = "PMD_MATERIAL_NULL.tga";
                    }

                    PmdMaterialPart part = new PmdMaterialPart();
                    part.TextureName = TextureName;
                    part.TextureEnable = pmds[i].Materials[im].TextureEnable;
                    part.TextureData = pmds[i].Materials[im].TextureData;

                    if (!MaterialDic.ContainsKey(part))
                    {
                        string MaterialName = TextureName;

                        if (!materialNameList.ContainsKey(MaterialName))
                        {
                            materialNameList.Add(MaterialName, 0);
                        }
                        else
                        {
                            int val = ++materialNameList[MaterialName];
                            MaterialName = TextureName + val.ToString();
                        }

                        Materialref[i][im] = MaterialName;

                        MaterialDic.Add(part, MaterialName);
                    }
                    else 
                    {
                        string MaterialName = MaterialDic[part];
                        Materialref[i][im] = MaterialName;
                    }
                }
            }

            return MaterialDic.OrderBy(v => v.Value).ToDictionary(v => v.Value, k => k.Key);
        }

    }
}
