using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_2007_PMD_REPACK
{
    public static class PMDrepackMat
    {

        public static Dictionary<string, FinalMaterialLine> GetMaterials(
        string[] ModelMataterial,
        Dictionary<string, PmdMaterialLine> idxMaterial,
        ObjLoader.Loader.Data.Material[] MtlMaterials,
        bool useMtl,
        bool useidxMaterial)
        {
            Dictionary<string, FinalMaterialLine> dic = new Dictionary<string, FinalMaterialLine>(ModelMataterial.Length);


            if (useidxMaterial)
            {
                for (int i = 0; i < ModelMataterial.Length; i++)
                {
                    string key = ModelMataterial[i].ToUpperInvariant();

                    if (!dic.ContainsKey(key))
                    {
                        if (idxMaterial.ContainsKey(key))
                        {
                            var mat = idxMaterial[key];
                            FinalMaterialLine line = new FinalMaterialLine();
                            line.TextureName = FixTextureName(mat.TextureName);
                            line.TextureEnable = mat.TextureEnable;
                            line.TextureData = mat.TextureData;
                            dic.Add(key, line);
                        }
                    }

                }
            }


            if (useMtl)
            {
                for (int i = 0; i < ModelMataterial.Length; i++)
                {
                    string key = ModelMataterial[i].ToUpperInvariant();

                    if (!dic.ContainsKey(key))
                    {
                        var first = (from mat in MtlMaterials
                                     where mat.Name.ToUpperInvariant() == key
                                     select mat).FirstOrDefault();

                        if (first != null)
                        {
                            string texture = FixTextureName(first.DiffuseTextureMap);

                            FinalMaterialLine line = new FinalMaterialLine();
                            line.TextureName = texture;
                            line.TextureEnable = 1;
                            line.TextureData = materialDefaultValues();
                            dic.Add(key, line);
                        }
                    }
                }
            }


            for (int i = 0; i < ModelMataterial.Length; i++)
            {
                string key = ModelMataterial[i].ToUpperInvariant();

                if (!dic.ContainsKey(key))
                {
                    string texture = FixTextureName(ModelMataterial[i]);

                    FinalMaterialLine line = new FinalMaterialLine();
                    line.TextureName = texture;
                    line.TextureEnable = 1;
                    line.TextureData = materialDefaultValues();
                    dic.Add(key, line);
                }
            }

            return dic;
        }

        private static float[] materialDefaultValues() // editado
        {
            //return new float[] { 0.7f, 0.7f, 0.7f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 50.0f };
            return new float[] { 0.8f, 0.8f, 0.8f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        }

        private static string FixTextureName(string texture)
        {
            if (texture == null)
            {
                texture = "null";
            }
            texture = texture.Replace('/', '\\')
                .Replace(":", "").Replace("*", "").Replace("\"", "").Replace("|", "")
                .Replace("<", "").Replace(">", "").Replace("?", "").Replace(" ", "_");

            texture = texture.Split('\\').Last();

            if (texture.Length == 0)
            {
                texture = "null";
            }

            var fileinfo = new FileInfo(texture);
            return fileinfo.Name.Remove(fileinfo.Name.Length - fileinfo.Extension.Length, fileinfo.Extension.Length) + ".tga";
        }

        public static ObjLoader.Loader.Data.Material[] LoadMtlMaterials(string mtlPath, bool loadMtlFile)
        {
            List<ObjLoader.Loader.Data.Material> MtlMaterials = new List<ObjLoader.Loader.Data.Material>();
            if (loadMtlFile)
            {
                if (File.Exists(mtlPath))
                {
                    try
                    {
                        var mtlLoaderFactory = new ObjLoader.Loader.Loaders.MtlLoaderFactory();
                        var mtlLoader = mtlLoaderFactory.Create();
                        var streamReaderMtl = new StreamReader(new FileInfo(mtlPath).OpenRead(), Encoding.ASCII);
                        ObjLoader.Loader.Loaders.LoadResultMtl arqMtl = mtlLoader.Load(streamReaderMtl);
                        streamReaderMtl.Close();
                        MtlMaterials = arqMtl.Materials.ToList();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error loading Mtl file: " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Mtl file does not exist!");
                }

            }
            return MtlMaterials.ToArray();
        }

        public static void PrintTextureNamesInConsole(FinalMaterialLine[] materials)
        {
            HashSet<string> list = new HashSet<string>();
            for (int i = 0; i < materials.Length; i++)
            {
                list.Add(materials[i].TextureName);
            }
            Console.WriteLine("Texture Names: " + string.Join(", ", list));
        }

        // public static IntermediaryNodeGroup[] GetIntermediaryNodeGroups(PmdNodeGroup[] nodeGroups)
    }
}
