using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_2007_PMD_REPACK;

namespace RE4_2007_SCENARIO_SMD_REPACK
{
    public static partial class ScenarioRepack
    {
        public static void Repack(string objPath, string diretory, 
            IdxScenario idxScenario,
            Dictionary<string, PmdMaterialLine> idxMaterial,
            ObjLoader.Loader.Data.Material[] MtlMaterials)
        {
            string patternSCENARIO = "^(SCENARIO#)(P|S)(MD_)([0]{0,})([0-9]{1,3})(#SMX_)([0]{0,})([0-9]{1,3})(#TYPE_)([0]{0,})([0-9|A-F]{1,8})(#).*$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(patternSCENARIO, System.Text.RegularExpressions.RegexOptions.CultureInvariant);

            bool LoadColorsFromObjFile = true;

            // load .obj file
            var objLoaderFactory = new ObjLoader.Loader.Loaders.ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var streamReader = new StreamReader(new FileInfo(objPath).OpenRead(), Encoding.ASCII);
            ObjLoader.Loader.Loaders.LoadResult arqObj = objLoader.Load(streamReader);
            streamReader.Close();

            //lista de materiais usados no modelo
            HashSet<string> ModelMaterials = new HashSet<string>();
            HashSet<string> ModelMaterialsToUpper = new HashSet<string>();

     
            Vector4 color = new Vector4(1, 1, 1, 1);
            StartWeightMap weightMap = new StartWeightMap(1, 0, 1, 0, 0);


            //conjunto de struturas
            //id do pmd/ conteudo para o pmd
            Dictionary<int, StartStructure> ObjList = new Dictionary<int, StartStructure>();
            //id do pmd, outras informações 
            Dictionary<int, SmdBaseLine> objGroupInfos = new Dictionary<int, SmdBaseLine>();
            int maxPmd = 0;

            for (int iG = 0; iG < arqObj.Groups.Count; iG++)
            {                         
                string GroupName = arqObj.Groups[iG].GroupName.ToUpperInvariant().Trim();

                if (GroupName.StartsWith("SCENARIO"))
                {
                    string materialNameInvariant = arqObj.Groups[iG].MaterialName.ToUpperInvariant().Trim();
                    string materialName = arqObj.Groups[iG].MaterialName.Trim();

                    //FIX NAME
                    GroupName = GroupName.Replace("_", "#")
                        .Replace("SMD#", "SMD_")
                        .Replace("PMD#", "PMD_")
                        .Replace("SMX#", "SMX_")
                        .Replace("TYPE#", "TYPE_")
                        .Replace("BIN#", "BIN_") //campo não usado
                        ;

                    //REGEX
                    if (regex.IsMatch(GroupName))
                    {
                        Console.WriteLine("Loading in Obj: " + GroupName + " | " + materialNameInvariant);
                    }
                    else
                    {
                        Console.WriteLine("Loading in Obj: " + GroupName + " | " + materialNameInvariant + "  The group name is wrong;");
                    }


                    SmdBaseLine info = getGroupInfo(GroupName);

                    if (!objGroupInfos.ContainsKey(info.PmdId))
                    {
                        objGroupInfos.Add(info.PmdId, info);
                    }

                    if (info.PmdId > maxPmd)
                    {
                        maxPmd = info.PmdId;
                    }

                    List<List<StartVertex>> facesList = new List<List<StartVertex>>();

                    for (int iF = 0; iF < arqObj.Groups[iG].Faces.Count; iF++)
                    {
                        List<StartVertex> verticeListInObjFace = new List<StartVertex>();

                        for (int iI = 0; iI < arqObj.Groups[iG].Faces[iF].Count; iI++)
                        {
                            StartVertex vertice = new StartVertex();

                            if (arqObj.Groups[iG].Faces[iF][iI].VertexIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1 >= arqObj.Vertices.Count)
                            {
                                throw new ArgumentException("Vertex Position Index is invalid! Value: " + arqObj.Groups[iG].Faces[iF][iI].VertexIndex);
                            }

                            Vector3 position = new Vector3(
                                arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].X,
                                arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].Y,
                                arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].Z
                                );

                            vertice.Position = position;


                            if (arqObj.Groups[iG].Faces[iF][iI].TextureIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1 >= arqObj.Textures.Count)
                            {
                                vertice.Texture = new Vector2(0, 0);
                            }
                            else
                            {
                                Vector2 texture = new Vector2(
                                arqObj.Textures[arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1].U,
                                ((arqObj.Textures[arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1].V - 1) * -1)
                                );

                                vertice.Texture = texture;
                            }


                            if (arqObj.Groups[iG].Faces[iF][iI].NormalIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1 >= arqObj.Normals.Count)
                            {
                                vertice.Normal = new Vector3(0, 0, 0);
                            }
                            else
                            {
                                Vector3 normal = new Vector3(
                                arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].X,
                                arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].Y,
                                arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].Z
                                );

                                vertice.Normal = normal;
                            }

                            vertice.Color = color;
                            vertice.WeightMap = weightMap;

                            if (LoadColorsFromObjFile)
                            {
                                Vector4 vColor = new Vector4(
                               arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].R,
                               arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].G,
                               arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].B,
                               arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].A
                               );
                                vertice.Color = vColor;
                            }

                            verticeListInObjFace.Add(vertice);

                        }

                        if (verticeListInObjFace.Count >= 3)
                        {
                            for (int i = 2; i < verticeListInObjFace.Count; i++)
                            {
                                List<StartVertex> face = new List<StartVertex>();
                                face.Add(verticeListInObjFace[0]);
                                face.Add(verticeListInObjFace[i - 1]);
                                face.Add(verticeListInObjFace[i]);
                                facesList.Add(face);
                            }
                        }

                    }


                    if (ObjList.ContainsKey(info.PmdId))
                    {
                        if (ObjList[info.PmdId].FacesByMaterial.ContainsKey(materialNameInvariant))
                        {
                            ObjList[info.PmdId].FacesByMaterial[materialNameInvariant].Faces.AddRange(facesList);
                        }
                        else
                        {
                            ModelMaterials.Add(materialName);
                            ModelMaterialsToUpper.Add(materialNameInvariant);
                            ObjList[info.PmdId].FacesByMaterial.Add(materialNameInvariant, new StartFacesGroup(facesList));
                        }
                    }
                    else 
                    {
                        StartStructure startStructure = new StartStructure();
                        ModelMaterials.Add(materialName);
                        ModelMaterialsToUpper.Add(materialNameInvariant);
                        startStructure.FacesByMaterial.Add(materialNameInvariant, new StartFacesGroup(facesList));
                        ObjList.Add(info.PmdId, startStructure);
                    }

                }
                else
                {
                    Console.WriteLine("Loading in Obj: " + GroupName + "   Warning: Group not used;");
                }

            }


            // faz a compressão das vertives
            foreach (var item in ObjList)
            {
                Console.WriteLine("PMD ID: " + item.Key.ToString("D3"));
                item.Value.CompressAllFaces();
            }

            //arruma o Material
            var UseMaterial = PMDrepackMat.GetMaterials(ModelMaterials.ToArray(), idxMaterial, MtlMaterials, true, idxScenario.UseIdxPmdMaterial);

            PMDrepackMat.PrintTextureNamesInConsole(UseMaterial.Values.ToArray());

            if (idxScenario.SmdAmount < maxPmd)
            {
                idxScenario.SmdAmount = maxPmd;
            }


            for (int i = 0; i < idxScenario.SmdAmount; i++)
            {
                if (!objGroupInfos.ContainsKey(i))
                {
                    SmdBaseLine smdBaseLine = new SmdBaseLine();
                    smdBaseLine.PmdId = i;
                    smdBaseLine.SmxId = 0xFE;
                    smdBaseLine.Type = 0;
                    objGroupInfos.Add(i, smdBaseLine);
                }
            }



            foreach (var item in ObjList)
            {
                SMDLineIdx smdLineIdx = new SMDLineIdx();
                smdLineIdx.scaleX = 1f;
                smdLineIdx.scaleY = 1f;
                smdLineIdx.scaleZ = 1f;

                if (idxScenario.SmdLines.Length > item.Key)
                {
                    smdLineIdx = idxScenario.SmdLines[item.Key];
                }

                IntermediaryNodeGroup nodeGroup = new IntermediaryNodeGroup();
                nodeGroup.GroupName = "GROUP";
                nodeGroup.SkeletonIndex = 0;
                nodeGroup.MaterialList = ModelMaterialsToUpper.ToArray();

                string pmdPath = diretory + idxScenario.PmdBaseName + "_" + item.Key.ToString("D3") + ".pmd";

                Console.WriteLine("Creating: " +idxScenario.PmdBaseName + "_" + item.Key.ToString("D3") + ".pmd");

                PMDrepackScenario.CreateScenarioPMD(pmdPath, item.Value, nodeGroup, GetFinalBoneLine(smdLineIdx), UseMaterial, smdLineIdx);
            }


            Console.WriteLine("Creating: " + idxScenario.SmdFileName);
            CreateSMD(diretory + idxScenario.SmdFileName, objGroupInfos, idxScenario);

            // end
        }

        private static void CreateSMD(string smdPath, Dictionary<int, SmdBaseLine> lines, IdxScenario idxScenario) 
        {
            int SmdCount = idxScenario.SmdAmount;


            Stream stream = new FileInfo(smdPath).Create();

            byte[] header = new byte[0x10];
            header[0] = 0x40;

            byte[] b_SmdCount = BitConverter.GetBytes(SmdCount);
            header[2] = b_SmdCount[0];
            header[3] = b_SmdCount[1];

            uint binStreamPosition = (uint)(SmdCount * 64) + 0x10;
            byte[] b_binStreamPosition = BitConverter.GetBytes(binStreamPosition);
            header[4] = b_binStreamPosition[0];
            header[5] = b_binStreamPosition[1];
            header[6] = b_binStreamPosition[2];
            header[7] = b_binStreamPosition[3];

            stream.Write(header, 0, 0x10);


            for (int i = 0; i < SmdCount; i++)
            {
                float positionX = 0f; // faz influencia
                float positionY = 0f;
                float positionZ = 0f;
                float positionW = 1f;
                float angleX = 0f; // faz influencia
                float angleY = 0f;
                float angleZ = 0f;
                float angleW = 1f;
                float scaleX = 1f; // não fez diferença para o modelo, a escala esta dentro do modelo, na parte do bone
                float scaleY = 1f;
                float scaleZ = 1f;
                float scaleW = 1f;

                ushort BinID = (ushort)i;
                byte FixedFF = 0xFF;
                byte SmxID = (byte)lines[i].SmxId;
                uint unused1 = 0;
                uint objectStatus =lines[i].Type;
                uint unused2 = 0;

                if (idxScenario.SmdLines.Length > i)
                {
                    positionX = idxScenario.SmdLines[i].positionX * 100f;
                    positionY = idxScenario.SmdLines[i].positionY * 100f;
                    positionZ = idxScenario.SmdLines[i].positionZ * 100f;

                    angleX = idxScenario.SmdLines[i].angleX;
                    angleY = idxScenario.SmdLines[i].angleY;
                    angleZ = idxScenario.SmdLines[i].angleZ;

                    scaleX = idxScenario.SmdLines[i].scaleX;
                    scaleY = idxScenario.SmdLines[i].scaleY;
                    scaleZ = idxScenario.SmdLines[i].scaleZ;

                }

                //----

                byte[] SMDLine = new byte[64];

                BitConverter.GetBytes(positionX).CopyTo(SMDLine, 0);
                BitConverter.GetBytes(positionY).CopyTo(SMDLine, 4);
                BitConverter.GetBytes(positionZ).CopyTo(SMDLine, 8);
                BitConverter.GetBytes(positionW).CopyTo(SMDLine, 12);
                BitConverter.GetBytes(angleX).CopyTo(SMDLine, 16);
                BitConverter.GetBytes(angleY).CopyTo(SMDLine, 20);
                BitConverter.GetBytes(angleZ).CopyTo(SMDLine, 24);
                BitConverter.GetBytes(angleW).CopyTo(SMDLine, 28);
                BitConverter.GetBytes(scaleX).CopyTo(SMDLine, 32);
                BitConverter.GetBytes(scaleY).CopyTo(SMDLine, 36);
                BitConverter.GetBytes(scaleZ).CopyTo(SMDLine, 40);
                BitConverter.GetBytes(scaleW).CopyTo(SMDLine, 44);
                BitConverter.GetBytes(BinID).CopyTo(SMDLine, 48);
                SMDLine[50] = FixedFF;
                SMDLine[51] = SmxID;
                BitConverter.GetBytes(unused1).CopyTo(SMDLine, 52);
                BitConverter.GetBytes(objectStatus).CopyTo(SMDLine, 56);
                BitConverter.GetBytes(unused2).CopyTo(SMDLine, 60);

                stream.Write(SMDLine, 0, 64);
            }

            //---------------------------

            // PARTE DOS ARQUIVOS BINS

            // BLOCO DOS OFFSETS

            int BinCount = SmdCount; 

            int offsetBlockCount = BinCount * 4;
            int CalcLines = offsetBlockCount / 0x10;
            CalcLines += 1;
            offsetBlockCount = CalcLines * 0x10;

            long StartOffset = stream.Position;

            stream.Write(new byte[offsetBlockCount], 0, offsetBlockCount);

            uint firtOffset = (uint)offsetBlockCount;

            stream.Position = StartOffset;
            stream.Write(BitConverter.GetBytes(firtOffset), 0, 4);

            stream.Position = StartOffset + firtOffset;

            //
            long tempOffset = StartOffset + firtOffset;
            uint InternalOffset = firtOffset;


            for (int i = 0; i < BinCount; i++)
            {
               
                byte[] bb = InternalFiles.BIN;

                //DrawDistance

                float DrawDistanceNegativeX = -640000f;
                float DrawDistanceNegativeY = -640000f;
                float DrawDistanceNegativeZ = -640000f;

                float DrawDistancePositiveX = 320000f;
                float DrawDistancePositiveY = 320000f;
                float DrawDistancePositiveZ = 320000f;

                if (idxScenario.BinRenderBoxes.Length > i && idxScenario.SmdLines.Length > i)
                {

                    SMDLineIdx smdLine = idxScenario.SmdLines[i];
                    BinRenderBox box = idxScenario.BinRenderBoxes[i];

                    float[] pos1 = new float[3];// 0 = x, 1 = y, 2 = z
                    pos1[0] = box.DrawDistanceNegativeX * 100f;
                    pos1[1] = box.DrawDistanceNegativeY * 100f;
                    pos1[2] = box.DrawDistanceNegativeZ * 100f;

                    pos1[0] = ((pos1[0]) - (smdLine.positionX * 100f)) / smdLine.scaleX;
                    pos1[1] = ((pos1[1]) - (smdLine.positionY * 100f)) / smdLine.scaleY;
                    pos1[2] = ((pos1[2]) - (smdLine.positionZ * 100f)) / smdLine.scaleZ;

                    pos1 = RotationUtils.RotationInZ(pos1, -smdLine.angleZ);
                    pos1 = RotationUtils.RotationInY(pos1, -smdLine.angleY);
                    pos1 = RotationUtils.RotationInX(pos1, -smdLine.angleX);

                    DrawDistanceNegativeX = pos1[0];
                    DrawDistanceNegativeY = pos1[1];
                    DrawDistanceNegativeZ = pos1[2];



                    float[] pos2 = new float[3];// 0 = x, 1 = y, 2 = z
                    pos2[0] = box.DrawDistancePositiveX * 100f;
                    pos2[1] = box.DrawDistancePositiveY * 100f;
                    pos2[2] = box.DrawDistancePositiveZ * 100f;

                    pos2[0] = ((pos2[0]) - (smdLine.positionX * 100f)) / smdLine.scaleX;
                    pos2[1] = ((pos2[1]) - (smdLine.positionY * 100f)) / smdLine.scaleY;
                    pos2[2] = ((pos2[2]) - (smdLine.positionZ * 100f)) / smdLine.scaleZ;

                    pos2 = RotationUtils.RotationInZ(pos2, -smdLine.angleZ);
                    pos2 = RotationUtils.RotationInY(pos2, -smdLine.angleY);
                    pos2 = RotationUtils.RotationInX(pos2, -smdLine.angleX);

                    DrawDistancePositiveX = pos2[0];
                    DrawDistancePositiveY = pos2[1];
                    DrawDistancePositiveZ = pos2[2];
                }


                var bDrawDistanceNegativeX = BitConverter.GetBytes(DrawDistanceNegativeX);

                bb[0x30] = bDrawDistanceNegativeX[0];
                bb[0x31] = bDrawDistanceNegativeX[1];
                bb[0x32] = bDrawDistanceNegativeX[2];
                bb[0x33] = bDrawDistanceNegativeX[3];

                var bDrawDistanceNegativeY = BitConverter.GetBytes(DrawDistanceNegativeY);

                bb[0x34] = bDrawDistanceNegativeY[0];
                bb[0x35] = bDrawDistanceNegativeY[1];
                bb[0x36] = bDrawDistanceNegativeY[2];
                bb[0x37] = bDrawDistanceNegativeY[3];

                var bDrawDistanceNegativeZ = BitConverter.GetBytes(DrawDistanceNegativeZ);

                bb[0x38] = bDrawDistanceNegativeZ[0];
                bb[0x39] = bDrawDistanceNegativeZ[1];
                bb[0x3A] = bDrawDistanceNegativeZ[2];
                bb[0x3B] = bDrawDistanceNegativeZ[3];

                var bDrawDistancePositiveX = BitConverter.GetBytes(DrawDistancePositiveX);

                bb[0x40] = bDrawDistancePositiveX[0];
                bb[0x41] = bDrawDistancePositiveX[1];
                bb[0x42] = bDrawDistancePositiveX[2];
                bb[0x43] = bDrawDistancePositiveX[3];


                var bDrawDistancePositiveY = BitConverter.GetBytes(DrawDistancePositiveY);

                bb[0x44] = bDrawDistancePositiveY[0];
                bb[0x45] = bDrawDistancePositiveY[1];
                bb[0x46] = bDrawDistancePositiveY[2];
                bb[0x47] = bDrawDistancePositiveY[3];

                var bDrawDistancePositiveZ = BitConverter.GetBytes(DrawDistancePositiveZ);

                bb[0x48] = bDrawDistancePositiveZ[0];
                bb[0x49] = bDrawDistancePositiveZ[1];
                bb[0x4A] = bDrawDistancePositiveZ[2];
                bb[0x4B] = bDrawDistancePositiveZ[3];

                uint FileLength = (uint)bb.Length;

                stream.Write(bb, 0, bb.Length);

                tempOffset = stream.Position;


                stream.Position = StartOffset + (i * 4);
                stream.Write(BitConverter.GetBytes(InternalOffset), 0, 4);

                stream.Position = tempOffset;

                InternalOffset += FileLength;
            }

            // tpl

            uint TplOffset = (uint)stream.Position;

            stream.Position = 8;
            stream.Write(BitConverter.GetBytes(TplOffset), 0, 4);
            stream.Position = TplOffset;

            byte[] Tpl_Padding = new byte[0x10];
            Tpl_Padding[0] = 0x10;
            stream.Write(Tpl_Padding, 0, 0x10);

            byte[] TplBytes = InternalFiles.TPL;

            stream.Write(TplBytes, 0, TplBytes.Length);

            stream.Close();

        }


        private static SmdBaseLine getGroupInfo(string GroupName) 
        {
            SmdBaseLine line = new SmdBaseLine();

            var split = GroupName.Split('#');

            try
            {
                var subSplit = split[1].Split('_');
                int id = int.Parse(subSplit[1].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                line.PmdId = id;
            }
            catch (Exception)
            {
            }

            try
            {
                var subSplit = split[2].Split('_');
                int id = int.Parse(subSplit[1].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                line.SmxId = id;
            }
            catch (Exception)
            {
            }

            try
            {
                var subSplit = split[3].Split('_');
                uint type = uint.Parse(subSplit[1].Trim(), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                line.Type = type;
            }
            catch (Exception)
            {
            }


            return line;
        }

        private static FinalBoneLine GetFinalBoneLine(SMDLineIdx smdLineIdx)
        {
            float[] values = new float[26];

            //escala
            values[0] = smdLineIdx.scaleX;
            values[1] = smdLineIdx.scaleY;
            values[2] = smdLineIdx.scaleZ;

            // position

            values[7] = smdLineIdx.positionX / 10f;
            values[8] = smdLineIdx.positionY / 10f;
            values[9] = smdLineIdx.positionZ / 10f;

            values[22] = smdLineIdx.positionX / 10f;
            values[23] = smdLineIdx.positionY / 10f;
            values[24] = smdLineIdx.positionZ / 10f;

            values[6] = 1f;
            values[10] = 1f;
            values[15] = 1f;
            values[20] = 1f;
            values[25] = 1f;
            FinalBoneLine boneLine = new FinalBoneLine(0, -1, "BONE");
            values.CopyTo(boneLine.Values, 0);
            return boneLine;
        }


    }


}
