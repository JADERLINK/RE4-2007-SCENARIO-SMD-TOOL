using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_2007_PMD_REPACK
{
    public static class PMDrepackFinal
    {
        public static FinalStructure MakeFinalStructure(IntermediaryStructure intermediaryStructure, IntermediaryNodeGroup[] nodeGroups, bool isScenarioPmd)
        {
            FinalStructure finalStructure = new FinalStructure();

            var meshMaterial = intermediaryStructure.Meshs.Keys.ToArray();

            Dictionary<string, string> Dic_MaterialName_NodeName = new Dictionary<string, string>();
            Dictionary<string, int> Dic_NodeName_SkeletonIndex = new Dictionary<string, int>();

            //faz as lista que define o SkeletonIndex de cada node
            //e define qual material é de qual node.
            for (int i = 0; i < meshMaterial.Length; i++)
            {
                var nodeGroupObj = (from obj in nodeGroups
                                    where obj.MaterialList.Contains(meshMaterial[i])
                                    select obj).FirstOrDefault();

                if (nodeGroupObj != null && !Dic_NodeName_SkeletonIndex.ContainsKey(nodeGroupObj.GroupName))
                {
                    Dic_MaterialName_NodeName.Add(meshMaterial[i], nodeGroupObj.GroupName);
                    Dic_NodeName_SkeletonIndex.Add(nodeGroupObj.GroupName, nodeGroupObj.SkeletonIndex);
                }
                else if (nodeGroupObj != null)
                {
                    Dic_MaterialName_NodeName.Add(meshMaterial[i], nodeGroupObj.GroupName);
                }
                else
                {
                    Dic_MaterialName_NodeName.Add(meshMaterial[i], meshMaterial[i]);
                    Dic_NodeName_SkeletonIndex.Add(meshMaterial[i], 0);
                }
            }

            // nome do node/grupo
            Dictionary<string, FinalNode> nodes = new Dictionary<string, FinalNode>();

            foreach (var item in intermediaryStructure.Meshs)
            {
                FinalMesh mesh = new FinalMesh();

                FinalNode node = null;

                List<int> boneIDs = new List<int>();

                string nodeName = Dic_MaterialName_NodeName[item.Key];
                if (nodes.ContainsKey(nodeName))
                {
                    node = nodes[nodeName];
                    boneIDs.AddRange(node.BonesIDs);
                }
                else
                {
                    node = new FinalNode();
                    node.NodeName = nodeName;
                    node.SkeletonIndex = Dic_NodeName_SkeletonIndex[nodeName];
                    nodes.Add(nodeName, node);
                }

                //--------

                mesh.MaterialName = item.Value.MaterialName;

                ushort indexCounter = 0;
                bool flip = false;

                for (int i = 0; i < item.Value.Faces.Count; i++)
                {

                    //vertices
                    for (int iv = 0; iv < item.Value.Faces[i].Vertexs.Count; iv++)
                    {
                        FinalVertex vertex = new FinalVertex();

                        vertex.PosX = item.Value.Faces[i].Vertexs[iv].PosX;
                        vertex.PosY = item.Value.Faces[i].Vertexs[iv].PosY;
                        vertex.PosZ = item.Value.Faces[i].Vertexs[iv].PosZ;

                        vertex.NormalX = item.Value.Faces[i].Vertexs[iv].NormalX;
                        vertex.NormalY = item.Value.Faces[i].Vertexs[iv].NormalY;
                        vertex.NormalZ = item.Value.Faces[i].Vertexs[iv].NormalZ;

                        vertex.TextureU = item.Value.Faces[i].Vertexs[iv].TextureU;
                        vertex.TextureV = item.Value.Faces[i].Vertexs[iv].TextureV;

                        vertex.ColorR = item.Value.Faces[i].Vertexs[iv].ColorR;
                        vertex.ColorG = item.Value.Faces[i].Vertexs[iv].ColorG;
                        vertex.ColorB = item.Value.Faces[i].Vertexs[iv].ColorB;
                        vertex.ColorA = item.Value.Faces[i].Vertexs[iv].ColorA;

                        if (isScenarioPmd)
                        {
                            vertex.Weight0 = 1f;
                            vertex.Weight1 = 0f;
                            vertex.BoneID0 = 0f;
                            vertex.BoneID1 = 0f;
                        }
                        else
                        {
                            vertex.Weight0 = item.Value.Faces[i].Vertexs[iv].Weight0;

                            int boneID0 = item.Value.Faces[i].Vertexs[iv].BoneID0;
                            if (!boneIDs.Contains(boneID0))
                            {
                                boneIDs.Add(boneID0);
                            }

                            vertex.BoneID0 = boneIDs.IndexOf(boneID0) * 3;

                            if (vertex.Weight0 != 1f)
                            {
                                vertex.Weight1 = item.Value.Faces[i].Vertexs[iv].Weight1;

                                int boneID1 = item.Value.Faces[i].Vertexs[iv].BoneID1;
                                if (!boneIDs.Contains(boneID1))
                                {
                                    boneIDs.Add(boneID1);
                                }

                                vertex.BoneID1 = boneIDs.IndexOf(boneID1) * 3;
                            }

                        }

                        mesh.VertexList.Add(vertex);

                    }

                    //indices
                    flip = false;
                    indexCounter += 2;
                    for (int io = 2; io < item.Value.Faces[i].Vertexs.Count; io++)
                    {
                        if (flip)
                        {
                            mesh.IndexList.Add((ushort)(indexCounter));
                            mesh.IndexList.Add((ushort)(indexCounter - 1));
                            mesh.IndexList.Add((ushort)(indexCounter - 2));
                            flip = false;
                        }
                        else
                        {
                            mesh.IndexList.Add((ushort)(indexCounter - 2));
                            mesh.IndexList.Add((ushort)(indexCounter - 1));
                            mesh.IndexList.Add((ushort)(indexCounter));
                            flip = true;
                        }

                        indexCounter++;
                    }


                }



                //---------
                node.BonesIDs = boneIDs.ToArray();
                var nodeMeshs = node.Meshs.ToList();
                nodeMeshs.Add(mesh);
                node.Meshs = nodeMeshs.ToArray();
                nodes[nodeName] = node;
            }

            //ordenação
            nodes = (from obj in nodes
                    orderby obj.Key
                    orderby obj.Value.SkeletonIndex
                    select obj).ToDictionary(k => k.Key, v => v.Value);
            foreach (var node in nodes) 
            {
                node.Value.Meshs = (from mesh in node.Value.Meshs
                                   orderby mesh.MaterialName
                                   select mesh).ToArray();
            }

            finalStructure.Nodes = nodes;
            return finalStructure;
        }

        public static void MakeFinalPmdFile(string pmdPath, FinalStructure finalStructure, FinalBoneLine[] boneLines, Dictionary<string, FinalMaterialLine> UseMaterial)
        {
            // arruma indices dos materiais
            List<string> materialIndex = new List<string>();

            //cria arquivo
            var fileInfo = new FileInfo(pmdPath);
            var pmd = fileInfo.Create();

            byte[] header = new byte[] { 0x36, 0x00, 0x00, 0x00,
                0x76, 0x65, 0x72, 0x74, 0x65, 0x78, 0x28, 0x78, 0x2C, 0x79, 0x2C, 0x7A, 0x2C, 0x77, 0x30, 0x2C, 0x77, 0x31, 0x2C, 0x69, 0x30, 0x2C,
                0x69, 0x31, 0x2C, 0x6E, 0x78, 0x2C, 0x6E, 0x79, 0x2C, 0x6E, 0x7A, 0x2C, 0x74, 0x75, 0x2C, 0x74, 0x76, 0x2C, 0x72, 0x2C, 0x67, 0x2C,
                0x62, 0x2C, 0x61, 0x29, 0x2C, 0x69, 0x6E, 0x64, 0x65, 0x78 };

            pmd.Write(header, 0, header.Length);

            //quantidade de nome de bones

            int boneLenght = boneLines.Length;
            byte[] boneLenghtBytes = BitConverter.GetBytes(boneLenght);
            pmd.Write(boneLenghtBytes, 0, boneLenghtBytes.Length);

            for (int i = 0; i < boneLenght; i++)
            {
                int nameLength = boneLines[i].Name.Length;
                byte[] nameLengthBytes = BitConverter.GetBytes(nameLength);
                pmd.Write(nameLengthBytes, 0, nameLengthBytes.Length);

                byte[] nameBytes = Encoding.ASCII.GetBytes(boneLines[i].Name);
                pmd.Write(nameBytes, 0, nameBytes.Length);

                byte[] idBytes = BitConverter.GetBytes(i);
                pmd.Write(idBytes, 0, idBytes.Length);
            }

            // quantidade de nome de nodes

            int nodesAmount = finalStructure.Nodes.Count;
            byte[] nodesAmountBytes = BitConverter.GetBytes(nodesAmount);
            pmd.Write(nodesAmountBytes, 0, nodesAmountBytes.Length);

            var nodeKeys = finalStructure.Nodes.Keys.ToArray();

            for (int i = 0; i < nodesAmount; i++)
            {
                int nameLength = nodeKeys[i].Length;
                byte[] nameLengthBytes = BitConverter.GetBytes(nameLength);
                pmd.Write(nameLengthBytes, 0, nameLengthBytes.Length);

                byte[] nameBytes = Encoding.ASCII.GetBytes(nodeKeys[i]);
                pmd.Write(nameBytes, 0, nameBytes.Length);

                byte[] idBytes = BitConverter.GetBytes(i);
                pmd.Write(idBytes, 0, idBytes.Length);
            }

            // 4 zeros
            pmd.Write(new byte[4], 0, 4);

            //quantidade de bones
            pmd.Write(boneLenghtBytes, 0, boneLenghtBytes.Length);

            for (int i = 0; i < boneLenght; i++)
            {
                int parent = boneLines[i].Parent;
                byte[] parentBytes = BitConverter.GetBytes(parent);
                pmd.Write(parentBytes, 0, parentBytes.Length);

                for (int iv = 0; iv < boneLines[i].Values.Length; iv++)
                {
                    byte[] valueBytes = BitConverter.GetBytes(boneLines[i].Values[iv]);
                    pmd.Write(valueBytes, 0, valueBytes.Length);
                }

            }

            // obtem os valores corretos pra os bones que ficam dentro dos nodes
            var subBone = GetSubBonePos(boneLines);

            // quantidade de nodes
            pmd.Write(nodesAmountBytes, 0, nodesAmountBytes.Length);


            foreach (var item in finalStructure.Nodes.Values)
            {
                //item.SkeletonIndex
                byte[] skeletonIndexBytes = BitConverter.GetBytes(item.SkeletonIndex);
                pmd.Write(skeletonIndexBytes, 0, skeletonIndexBytes.Length);

                //32 bytes com zeros
                byte[] zeros32bytes = new byte[32];
                pmd.Write(zeros32bytes, 0, zeros32bytes.Length);

                // quantidade de id de texturas = quantidade de mesh
                //item.Meshs.Length;
                byte[] meshLenghtBytes = BitConverter.GetBytes(item.Meshs.Length);
                pmd.Write(meshLenghtBytes, 0, meshLenghtBytes.Length);

                for (int i = 0; i < item.Meshs.Length; i++)
                {

                    if (!materialIndex.Contains(item.Meshs[i].MaterialName))
                    {
                        materialIndex.Add(item.Meshs[i].MaterialName);
                    }

                    int index = materialIndex.IndexOf(item.Meshs[i].MaterialName);

                    byte[] indexBytes = BitConverter.GetBytes(index);
                    pmd.Write(indexBytes, 0, indexBytes.Length);

                }

                // 4 zeros
                pmd.Write(new byte[4], 0, 4);

                // quantiade de meshs
                pmd.Write(meshLenghtBytes, 0, meshLenghtBytes.Length);

                for (int i = 0; i < item.Meshs.Length; i++)
                {
                    //fix0x40000000
                    pmd.WriteByte(0x40);
                    pmd.WriteByte(0x00);
                    pmd.WriteByte(0x00);
                    pmd.WriteByte(0x00);

                    //quantidade de index(orders) do mesh
                    int indexCount = item.Meshs[i].IndexList.Count;

                    byte[] indexCountBytes = BitConverter.GetBytes(indexCount);
                    pmd.Write(indexCountBytes, 0, indexCountBytes.Length);

                    byte[] indexListBytes = new byte[indexCount * 2];

                    int offsetindex = 0;
                    for (int io = 0; io < indexCount; io++)
                    {
                        var indexU = BitConverter.GetBytes(item.Meshs[i].IndexList[io]);
                        indexListBytes[offsetindex] = indexU[0];
                        indexListBytes[offsetindex + 1] = indexU[1];
                        offsetindex += 2;
                    }

                    pmd.Write(indexListBytes, 0, indexListBytes.Length);

                    // vertex amount

                    int vertexAmount = item.Meshs[i].VertexList.Count;

                    byte[] vertexAmountBytes = BitConverter.GetBytes(vertexAmount);
                    pmd.Write(vertexAmountBytes, 0, vertexAmountBytes.Length);

                    for (int iv = 0; iv < vertexAmount; iv++)
                    {
                        byte[] vertice = new byte[64];

                        var posX = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].PosX);
                        var posY = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].PosY);
                        var posZ = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].PosZ);

                        var weight0 = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].Weight0);
                        var weight1 = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].Weight1);

                        var boneID0 = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].BoneID0);
                        var boneID1 = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].BoneID1);

                        var normalX = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].NormalX);
                        var normalY = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].NormalY);
                        var normalZ = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].NormalZ);

                        var textureU = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].TextureU);
                        var textureV = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].TextureV);

                        var colorR = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].ColorR);
                        var colorG = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].ColorG);
                        var colorB = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].ColorB);
                        var colorA = BitConverter.GetBytes(item.Meshs[i].VertexList[iv].ColorA);

                        posX.CopyTo(vertice, 0);
                        posY.CopyTo(vertice, 4);
                        posZ.CopyTo(vertice, 8);
                        weight0.CopyTo(vertice, 12);
                        weight1.CopyTo(vertice, 16);
                        boneID0.CopyTo(vertice, 20);
                        boneID1.CopyTo(vertice, 24);
                        normalX.CopyTo(vertice, 28);
                        normalY.CopyTo(vertice, 32);
                        normalZ.CopyTo(vertice, 36);
                        textureU.CopyTo(vertice, 40);
                        textureV.CopyTo(vertice, 44);
                        colorR.CopyTo(vertice, 48);
                        colorG.CopyTo(vertice, 52);
                        colorB.CopyTo(vertice, 56);
                        colorA.CopyTo(vertice, 60);

                        pmd.Write(vertice, 0, vertice.Length);
                    }

                }

                // partes com os bones por node

                // quantidade

                int BonesIDsLenght = item.BonesIDs.Length;
                byte[] BonesIDsLenghtBytes = BitConverter.GetBytes(BonesIDsLenght);
                pmd.Write(BonesIDsLenghtBytes, 0, BonesIDsLenghtBytes.Length);

                for (int i = 0; i < BonesIDsLenght; i++)
                {
                    //id do bone
                    byte[] BonesIDBytes = BitConverter.GetBytes(item.BonesIDs[i]);
                    pmd.Write(BonesIDBytes, 0, BonesIDBytes.Length);

                    // conjunto de floats
                    var floatsBytes = NodeBoneFloats(subBone[item.BonesIDs[i]]);
                    pmd.Write(floatsBytes, 0, floatsBytes.Length);
                }

            }

            // coloca materials

            int materialAmount = materialIndex.Count;
            byte[] materialAmountBytes = BitConverter.GetBytes(materialAmount);
            pmd.Write(materialAmountBytes, 0, materialAmountBytes.Length);

            for (int i = 0; i < materialAmount; i++)
            {
                var material = UseMaterial[materialIndex[i]];

                byte[] floatsData = new byte[68];

                int _offset = 0;
                for (int f = 0; f < material.TextureData.Length; f++)
                {
                    byte[] fBytes = BitConverter.GetBytes(material.TextureData[f]);
                    fBytes.CopyTo(floatsData, _offset);
                    _offset += 4;
                }

                pmd.Write(floatsData, 0, floatsData.Length);

                byte[] TextureUnknownBytes = BitConverter.GetBytes(material.TextureEnable);
                pmd.Write(TextureUnknownBytes, 0, TextureUnknownBytes.Length);

                int nameLength = material.TextureName.Length;
                byte[] nameLengthBytes = BitConverter.GetBytes(nameLength);
                pmd.Write(nameLengthBytes, 0, nameLengthBytes.Length);

                byte[] nameBytes = Encoding.ASCII.GetBytes(material.TextureName);
                pmd.Write(nameBytes, 0, nameBytes.Length);

            }

            pmd.Close();
        }

        private static (float x, float y, float z)[] GetSubBonePos(FinalBoneLine[] boneLines)
        {
            (float x, float y, float z)[] arr = new (float x, float y, float z)[boneLines.Length];

            for (int i = 0; i < boneLines.Length; i++)
            {
                arr[i].x = -boneLines[i].Values[7];
                arr[i].y = -boneLines[i].Values[8];
                arr[i].z = -boneLines[i].Values[9];

                if (boneLines[i].Parent != -1)
                {
                    arr[i].x = arr[boneLines[i].Parent].x - boneLines[i].Values[7];
                    arr[i].y = arr[boneLines[i].Parent].y - boneLines[i].Values[8];
                    arr[i].z = arr[boneLines[i].Parent].z - boneLines[i].Values[9];
                }
            }

            return arr;
        }

        private static byte[] NodeBoneFloats((float x, float y, float z) pos)
        {
            byte[] res = new byte[68]; // 17 *4

            byte[] number1 = BitConverter.GetBytes(1f);

            number1.CopyTo(res, 4);
            number1.CopyTo(res, 24);
            number1.CopyTo(res, 44);
            number1.CopyTo(res, 64);

            byte[] bx = BitConverter.GetBytes(pos.x);
            byte[] by = BitConverter.GetBytes(pos.y);
            byte[] bz = BitConverter.GetBytes(pos.z);

            bx.CopyTo(res, 52);
            by.CopyTo(res, 56);
            bz.CopyTo(res, 60);

            return res;
        }

    }
}
