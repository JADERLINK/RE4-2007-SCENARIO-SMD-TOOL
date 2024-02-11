using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_2007_SCENARIO_SMD_Repack;

namespace RE4_PMD_Repack
{
    // editado
    public static partial class PMDrepack
    {
        private static IntermediaryStructure MakeIntermediaryStructure(StartStructure startStructure, SMDLineIdx smdLine)
        {
            IntermediaryStructure intermediary = new IntermediaryStructure();

            foreach (var item in startStructure.FacesByMaterial)
            {
                IntermediaryMesh mesh = new IntermediaryMesh();

                for (int i = 0; i < item.Value.Faces.Count; i++)
                {
                    IntermediaryFace face = new IntermediaryFace();

                    for (int iv = 0; iv < item.Value.Faces[i].Count; iv++)
                    {
                        IntermediaryVertex vertex = new IntermediaryVertex();

                        float[] pos1 = new float[3];// 0 = x, 1 = y, 2 = z
                        pos1[0] = item.Value.Faces[i][iv].Position.X * 100f;
                        pos1[1] = item.Value.Faces[i][iv].Position.Y * 100f;
                        pos1[2] = item.Value.Faces[i][iv].Position.Z * 100f;

                        pos1[0] = ((pos1[0]) - (smdLine.positionX * 100f)) / smdLine.scaleX;
                        pos1[1] = ((pos1[1]) - (smdLine.positionY * 100f)) / smdLine.scaleY;
                        pos1[2] = ((pos1[2]) - (smdLine.positionZ * 100f)) / smdLine.scaleZ;

                        pos1 = RotationUtils.RotationInZ(pos1, -smdLine.angleZ);
                        pos1 = RotationUtils.RotationInY(pos1, -smdLine.angleY);
                        pos1 = RotationUtils.RotationInX(pos1, -smdLine.angleX);

                        vertex.PosX = pos1[0] / 1000f;
                        vertex.PosY = pos1[1] / 1000f;
                        vertex.PosZ = pos1[2] / 1000f;

                        float[] normal1 = new float[3];// 0 = x, 1 = y, 2 = z
                        normal1[0] = item.Value.Faces[i][iv].Normal.X;
                        normal1[1] = item.Value.Faces[i][iv].Normal.Y;
                        normal1[2] = item.Value.Faces[i][iv].Normal.Z;

                        normal1 = RotationUtils.RotationInZ(normal1, -smdLine.angleZ);
                        normal1 = RotationUtils.RotationInY(normal1, -smdLine.angleY);
                        normal1 = RotationUtils.RotationInX(normal1, -smdLine.angleX);

                        vertex.NormalX = normal1[0];
                        vertex.NormalY = normal1[1];
                        vertex.NormalZ = normal1[2];

                        vertex.TextureU = item.Value.Faces[i][iv].Texture.U;
                        vertex.TextureV = item.Value.Faces[i][iv].Texture.V;

                        vertex.ColorR = item.Value.Faces[i][iv].Color.R;
                        vertex.ColorG = item.Value.Faces[i][iv].Color.G;
                        vertex.ColorB = item.Value.Faces[i][iv].Color.B;
                        vertex.ColorA = item.Value.Faces[i][iv].Color.A;

                        vertex.BoneID0 = item.Value.Faces[i][iv].WeightMap.BoneID1;
                        vertex.BoneID1 = item.Value.Faces[i][iv].WeightMap.BoneID2;

                        vertex.Weight0 = item.Value.Faces[i][iv].WeightMap.Weight1;
                        vertex.Weight1 = item.Value.Faces[i][iv].WeightMap.Weight2;

                        face.Vertexs.Add(vertex);
                    }

                    mesh.Faces.Add(face);
                }

                mesh.MaterialName = item.Key.ToUpperInvariant();
                intermediary.Meshs.Add(mesh.MaterialName, mesh);
            }

            return intermediary;
        }

    }
}
