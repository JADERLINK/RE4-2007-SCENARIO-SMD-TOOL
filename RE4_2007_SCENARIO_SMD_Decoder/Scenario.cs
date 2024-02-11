using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace RE4_2007_SCENARIO_SMD_Extractor
{
    public static class Scenario
    {
        public static PMD_API.PMD[] GetPmds(string Diretory, string basename, int count) 
        {
            PMD_API.PMD[] pmds = new PMD_API.PMD[count];

            for (int i = 0; i < count; i++)
            {
                try
                {
                    Console.WriteLine("Reading: " + basename + "_" + i.ToString("D3") + ".pmd");
                    string file = Diretory + basename + "_" + i.ToString("D3") + ".pmd";
                    pmds[i] = PMD_API.PmdDecoder.GetPMD(file);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error opening file: " + basename + "_" + i.ToString("D3") + ".pmd");
                }
                
            }

            return pmds;
        }

        public static void CreateOBJ(SMDLine[] smdLines, PMD_API.PMD[] pmds, string baseDiretory, string baseFileName, bool UseColorsInObjFile) 
        {
            StreamWriter obj = new StreamWriter(baseDiretory + baseFileName + ".obj", false);
            obj.WriteLine("# RE4_2007_SCENARIO_SMD_Extractor");
            obj.WriteLine("# By JADERLINK");
            obj.WriteLine($"# Version {Program.VERSION}");
            obj.WriteLine("");

            obj.WriteLine("mtllib " + baseFileName + ".mtl");

            //obj.WriteLine("o " + "OBJ_SCENARIO#");

            uint indexCount = 0;

            for (int i = 0; i < smdLines.Length; i++)
            {
                if (pmds[i] != null)
                {
                    ObjCreatePart(i, obj, pmds[i], smdLines[i], ref indexCount, UseColorsInObjFile);
                }
                
            }

            obj.Close();
        }

        public static void CreateMTL(PMD_API.PMD[] pmds, string baseDiretory, string baseFileName) 
        {
            StreamWriter MTLtext = new FileInfo(baseDiretory + baseFileName + ".mtl").CreateText();
            MTLtext.WriteLine("# RE4_2007_SCENARIO_SMD_Extractor");
            MTLtext.WriteLine("# By JADERLINK");
            MTLtext.WriteLine($"# Version {Program.VERSION}");
            MTLtext.WriteLine("");

            HashSet<string> materialNames = new HashSet<string>();
            for (int ip = 0; ip < pmds.Length; ip++)
            {
                var pmd = pmds[ip];

                for (int i = 0; i < pmd.Materials.Length; i++)
                {
                    string TextureName = pmd.Materials[i].TextureName;
                    if (TextureName == null || TextureName.Length == 0)
                    {
                        TextureName = "PMD_MATERIAL_NULL.tga";
                    }

                    if (!materialNames.Contains(TextureName))
                    {

                        MTLtext.WriteLine("");
                        MTLtext.WriteLine("newmtl " + TextureName);
                        MTLtext.WriteLine("Ka 1.000 1.000 1.000");
                        MTLtext.WriteLine("Kd 1.000 1.000 1.000");
                        MTLtext.WriteLine("Ks 0.000 0.000 0.000");
                        MTLtext.WriteLine("Ns 0");
                        MTLtext.WriteLine("d 1");
                        MTLtext.WriteLine("Tr 1");
                        MTLtext.WriteLine("map_Kd " + TextureName);
                        MTLtext.WriteLine("");

                        materialNames.Add(TextureName);
                    }
                }
            }

      
            MTLtext.Close();
        }


        private static void ObjCreatePart(int ID, StreamWriter obj, PMD_API.PMD pmd, SMDLine smdLine, ref uint indexCount, bool UseColorsInObjFile)
        {
            
            obj.WriteLine("g " + "SCENARIO#PMD_" + ID.ToString("D3") + "#SMX_" + smdLine.SmxID.ToString("D3") + "#TYPE_" + smdLine.objectStatus.ToString("X2") + "#");

            for (int g = 0; g < pmd.Nodes.Length; g++)
            {

                for (int im = 0; im < pmd.Nodes[g].Meshs.Length; im++)
                {
                    if (pmd.Nodes[g].Meshs[im].Orders.Length != 0)
                    {
                        
                            string TextureName = pmd.Materials[pmd.Nodes[g].Meshs[im].TextureIndex].TextureName;
                            if (TextureName == null || TextureName.Length == 0)
                            {
                                TextureName = "PMD_MATERIAL_NULL.tga";
                            }
                            obj.WriteLine("usemtl " + TextureName);
               

                        for (int iv = 0; iv < pmd.Nodes[g].Meshs[im].Vertexs.Length; iv++)
                        {
                            float[] pos = new float[3];// 0 = x, 1 = y, 2 = z
                            pos[0] = pmd.Nodes[g].Meshs[im].Vertexs[iv].x * 1000f;
                            pos[1] = pmd.Nodes[g].Meshs[im].Vertexs[iv].y * 1000f;
                            pos[2] = pmd.Nodes[g].Meshs[im].Vertexs[iv].z * 1000f;

                            pos = RotationUtils.RotationInX(pos, smdLine.angleX);
                            pos = RotationUtils.RotationInY(pos, smdLine.angleY);
                            pos = RotationUtils.RotationInZ(pos, smdLine.angleZ);

                            pos[0] = ((pos[0] * pmd.SkeletonBoneData[0][0]) + (smdLine.positionX)) / 100f;
                            pos[1] = ((pos[1] * pmd.SkeletonBoneData[0][1]) + (smdLine.positionY)) / 100f;
                            pos[2] = ((pos[2] * pmd.SkeletonBoneData[0][2]) + (smdLine.positionZ)) / 100f;

                            string v = "v " + (pos[0]).ToString("f9", CultureInfo.InvariantCulture)
                                      + " " + (pos[1]).ToString("f9", CultureInfo.InvariantCulture)
                                      + " " + (pos[2]).ToString("f9", CultureInfo.InvariantCulture);

                            if (UseColorsInObjFile)
                            {
                                v += " " + (pmd.Nodes[g].Meshs[im].Vertexs[iv].r).ToString("f9", CultureInfo.InvariantCulture)
                                   + " " + (pmd.Nodes[g].Meshs[im].Vertexs[iv].g).ToString("f9", CultureInfo.InvariantCulture)
                                   + " " + (pmd.Nodes[g].Meshs[im].Vertexs[iv].b).ToString("f9", CultureInfo.InvariantCulture)
                                   + " " + (pmd.Nodes[g].Meshs[im].Vertexs[iv].a).ToString("f9", CultureInfo.InvariantCulture);
                            }
                            obj.WriteLine(v);

                            float[] normal = new float[3];// 0 = x, 1 = y, 2 = z
                            normal[0] = pmd.Nodes[g].Meshs[im].Vertexs[iv].nx;
                            normal[1] = pmd.Nodes[g].Meshs[im].Vertexs[iv].ny;
                            normal[2] = pmd.Nodes[g].Meshs[im].Vertexs[iv].nz;

                            normal = RotationUtils.RotationInX(normal, smdLine.angleX);
                            normal = RotationUtils.RotationInY(normal, smdLine.angleY);
                            normal = RotationUtils.RotationInZ(normal, smdLine.angleZ);

                            obj.WriteLine("vn " + 
                                (normal[0]).ToString("f9", CultureInfo.InvariantCulture)+ " " + 
                                (normal[1]).ToString("f9", CultureInfo.InvariantCulture)+ " " + 
                                (normal[2]).ToString("f9", CultureInfo.InvariantCulture));

                            obj.WriteLine("vt " + (pmd.Nodes[g].Meshs[im].Vertexs[iv].tu).ToString("f9", CultureInfo.InvariantCulture)
                              + " " + ((pmd.Nodes[g].Meshs[im].Vertexs[iv].tv - 1) * -1).ToString("f9", CultureInfo.InvariantCulture)
                              );
                        }

                        for (int io = 0; io < pmd.Nodes[g].Meshs[im].Orders.Length; io += 3)
                        {
                            string a = (pmd.Nodes[g].Meshs[im].Orders[io] + indexCount + 1).ToString();
                            string b = (pmd.Nodes[g].Meshs[im].Orders[io + 1] + indexCount + 1).ToString();
                            string c = (pmd.Nodes[g].Meshs[im].Orders[io + 2] + indexCount + 1).ToString();

                            obj.WriteLine("f " + a + "/" + a + "/" + a + " "
                                + b + "/" + b + "/" + b + " "
                                + c + "/" + c + "/" + c);

                        }

                        indexCount += (uint)pmd.Nodes[g].Meshs[im].Vertexs.Length;
                    }
                }
            }

        }



        public static void CreateDrawDistanceObj(SMDLine[] smdLine, BinRenderBox[] boxes, string baseDiretory, string baseFileName)
        {
            //
            TextWriter text = new FileInfo(baseDiretory + baseFileName + ".DrawDistance.obj").CreateText();
            text.WriteLine("# RE4_2007_SCENARIO_SMD_Extractor");
            text.WriteLine("# By JADERLINK");
            text.WriteLine($"# Version {Program.VERSION}");
            text.WriteLine("");
            int index = 0;

            //text.WriteLine("o " + "OBJ_DRAWDISTANCE#");

            for (int i = 0; i < smdLine.Length; i++)
            {
              
                CreateDrawDistancePart(i, ref text, smdLine[i], boxes[smdLine[i].BinID], ref index);
            }

            text.Close();
        }

        private static void CreateDrawDistancePart(int id, ref TextWriter text, SMDLine smdLine, BinRenderBox box, ref int index)
        {
            text.WriteLine("g " + "DRAWDISTANCE#PMD_" + id.ToString("D3") + "#");

            float[] pos1 = new float[3];// 0 = x, 1 = y, 2 = z
            pos1[0] = box.DrawDistanceNegativeX;
            pos1[1] = box.DrawDistanceNegativeY;
            pos1[2] = box.DrawDistanceNegativeZ;

            pos1 = RotationUtils.RotationInX(pos1, smdLine.angleX);
            pos1 = RotationUtils.RotationInY(pos1, smdLine.angleY);
            pos1 = RotationUtils.RotationInZ(pos1, smdLine.angleZ);

            pos1[0] = ((pos1[0] * smdLine.scaleX) + (smdLine.positionX)) / 100f;
            pos1[1] = ((pos1[1] * smdLine.scaleY) + (smdLine.positionY)) / 100f;
            pos1[2] = ((pos1[2] * smdLine.scaleZ) + (smdLine.positionZ)) / 100f;


            float[] pos2 = new float[3];// 0 = x, 1 = y, 2 = z
            pos2[0] = box.DrawDistanceNegativeX + box.DrawDistancePositiveX;
            pos2[1] = box.DrawDistanceNegativeY + box.DrawDistancePositiveY;
            pos2[2] = box.DrawDistanceNegativeZ + box.DrawDistancePositiveZ;

            pos2 = RotationUtils.RotationInX(pos2, smdLine.angleX);
            pos2 = RotationUtils.RotationInY(pos2, smdLine.angleY);
            pos2 = RotationUtils.RotationInZ(pos2, smdLine.angleZ);

            pos2[0] = ((pos2[0] * smdLine.scaleX) + (smdLine.positionX)) / 100f;
            pos2[1] = ((pos2[1] * smdLine.scaleY) + (smdLine.positionY)) / 100f;
            pos2[2] = ((pos2[2] * smdLine.scaleZ) + (smdLine.positionZ)) / 100f;

            string DrawDistanceNegativeX = (pos1[0]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeY = (pos1[1]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeZ = (pos1[2]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);

            string DrawDistancePositiveX = (pos2[0]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveY = (pos2[1]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveZ = (pos2[2]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);


            text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistanceNegativeY + " " + DrawDistanceNegativeZ);
            text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistancePositiveY + " " + DrawDistancePositiveZ);

            int i1 = index + 1;
            int i2 = index + 2;

            text.WriteLine($"l {i1} {i2}");

            index += 2;
        }




        public static void CreateIdxScenario(SMDLine[] smdLines, PMD_API.PMD[] pmds, string baseDiretory, string baseFileName, string SmdFileName, string PmdFilesName, BinRenderBox[] boxes) 
        {
            //
            TextWriter text = new FileInfo(baseDiretory + baseFileName + ".idxscenario").CreateText();
            text.WriteLine("# RE4_2007_SCENARIO_SMD_Extractor");
            text.WriteLine("# By JADERLINK");
            text.WriteLine($"# Version {Program.VERSION}");
            text.WriteLine("");

            text.WriteLine("SmdAmount:" + smdLines.Length);
            text.WriteLine("SmdFileName:" + SmdFileName);
            text.WriteLine("PmdBaseName:" + PmdFilesName);

            text.WriteLine("");
            for (int i = 0; i < smdLines.Length; i++)
            {
                text.WriteLine("");
                CreateIdxScenario_parts(i, ref text,smdLines[i], pmds[i]);

                CreateIdxScenario_DrawDistance(i, ref text, smdLines[i], boxes[smdLines[i].BinID]);
            }

            text.Close();


        }


        private static void CreateIdxScenario_parts(int id, ref TextWriter text, SMDLine smdLine, PMD_API.PMD pmd) 
        {
            string scaleX = (smdLine.scaleX).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string scaleY = (smdLine.scaleY).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string scaleZ = (smdLine.scaleZ).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            text.WriteLine(id.ToString("D3") + "_scaleX:" + scaleX);
            text.WriteLine(id.ToString("D3") + "_scaleY:" + scaleY);
            text.WriteLine(id.ToString("D3") + "_scaleZ:" + scaleZ);

            string angleX = (smdLine.angleX).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string angleY = (smdLine.angleY).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string angleZ = (smdLine.angleZ).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            text.WriteLine(id.ToString("D3") + "_angleX:" + angleX);
            text.WriteLine(id.ToString("D3") + "_angleY:" + angleY);
            text.WriteLine(id.ToString("D3") + "_angleZ:" + angleZ);

            string positionX = (smdLine.positionX / 100f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string positionY = (smdLine.positionY / 100f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string positionZ = (smdLine.positionZ / 100f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            text.WriteLine(id.ToString("D3") + "_positionX:" + positionX);
            text.WriteLine(id.ToString("D3") + "_positionY:" + positionY);
            text.WriteLine(id.ToString("D3") + "_positionZ:" + positionZ);

        }


        // a remover no futuro
        private static void CreateIdxScenario_DrawDistance(int id, ref TextWriter text, SMDLine smdLine, BinRenderBox box) 
        {
            float[] pos1 = new float[3];// 0 = x, 1 = y, 2 = z
            pos1[0] = box.DrawDistanceNegativeX;
            pos1[1] = box.DrawDistanceNegativeY;
            pos1[2] = box.DrawDistanceNegativeZ;

            pos1 = RotationUtils.RotationInX(pos1, smdLine.angleX);
            pos1 = RotationUtils.RotationInY(pos1, smdLine.angleY);
            pos1 = RotationUtils.RotationInZ(pos1, smdLine.angleZ);

            pos1[0] = ((pos1[0] * smdLine.scaleX) + (smdLine.positionX)) / 100f;
            pos1[1] = ((pos1[1] * smdLine.scaleY) + (smdLine.positionY)) / 100f;
            pos1[2] = ((pos1[2] * smdLine.scaleZ) + (smdLine.positionZ)) / 100f;

            float[] pos2 = new float[3];// 0 = x, 1 = y, 2 = z
            pos2[0] = box.DrawDistancePositiveX;
            pos2[1] = box.DrawDistancePositiveY;
            pos2[2] = box.DrawDistancePositiveZ;

            pos2 = RotationUtils.RotationInX(pos2, smdLine.angleX);
            pos2 = RotationUtils.RotationInY(pos2, smdLine.angleY);
            pos2 = RotationUtils.RotationInZ(pos2, smdLine.angleZ);

            pos2[0] = ((pos2[0] * smdLine.scaleX) + (smdLine.positionX)) / 100f;
            pos2[1] = ((pos2[1] * smdLine.scaleY) + (smdLine.positionY)) / 100f;
            pos2[2] = ((pos2[2] * smdLine.scaleZ) + (smdLine.positionZ)) / 100f;


            string DrawDistanceNegativeX = (pos1[0]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeY = (pos1[1]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeZ = (pos1[2]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);

            string DrawDistancePositiveX = (pos2[0]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveY = (pos2[1]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveZ = (pos2[2]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);


            //text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistanceNegativeY + " " + DrawDistanceNegativeZ);
            //text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistancePositiveY + " " + DrawDistancePositiveZ);

            text.WriteLine(id.ToString("D3") + "_DrawDistanceNegativeX:" + DrawDistanceNegativeX);
            text.WriteLine(id.ToString("D3") + "_DrawDistanceNegativeY:" + DrawDistanceNegativeY);
            text.WriteLine(id.ToString("D3") + "_DrawDistanceNegativeZ:" + DrawDistanceNegativeZ);

            text.WriteLine(id.ToString("D3") + "_DrawDistancePositiveX:" + DrawDistancePositiveX);
            text.WriteLine(id.ToString("D3") + "_DrawDistancePositiveY:" + DrawDistancePositiveY);
            text.WriteLine(id.ToString("D3") + "_DrawDistancePositiveZ:" + DrawDistancePositiveZ);


        }



    }

    public static class RotationUtils
    {
        //pos = x, y, z

        public static float[] RotationInX(float[] pos, float angleInRadian)
        {
            float[] res = new float[3];
            res[0] = pos[0];
            res[1] = (float)(Math.Cos(angleInRadian) * pos[1] - Math.Sin(angleInRadian) * pos[2]);
            res[2] = (float)(Math.Sin(angleInRadian) * pos[1] + Math.Cos(angleInRadian) * pos[2]);
            return res;
        }

        public static float[] RotationInY(float[] pos, float angleInRadian)
        {
            float[] res = new float[3];
            res[0] = (float)(Math.Cos(angleInRadian) * pos[0] + Math.Sin(angleInRadian) * pos[2]);
            res[1] = pos[1];
            res[2] = (float)(-1 * Math.Sin(angleInRadian) * pos[0] + Math.Cos(angleInRadian) * pos[2]);
            return res;
        }

        public static float[] RotationInZ(float[] pos, float angleInRadian)
        {
            float[] res = new float[3];
            res[0] = (float)(Math.Cos(angleInRadian) * pos[0] - Math.Sin(angleInRadian) * pos[1]);
            res[1] = (float)(Math.Sin(angleInRadian) * pos[0] + Math.Cos(angleInRadian) * pos[1]);
            res[2] = pos[2];
            return res;
        }

    }
}
