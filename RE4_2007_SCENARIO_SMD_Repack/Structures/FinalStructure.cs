using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_PMD_Repack
{
    public class FinalStructure
    {
        public Dictionary<string, FinalNode> Nodes { get; set; }

        public FinalStructure()
        {
            Nodes = new Dictionary<string, FinalNode>();
        }
    }

    public class FinalNode
    {
        public string NodeName { get; set; }
        public int SkeletonIndex { get; set; }
        public int[] BonesIDs { get; set; }
        public FinalMesh[] Meshs { get; set; }

        public FinalNode() 
        {
            BonesIDs = new int[0];
            Meshs = new FinalMesh[0];
        }

    }

    public class FinalMesh
    {
        public string MaterialName { get; set; }
        public List<ushort> IndexList { get; set; }
        public List<FinalVertex> VertexList { get; set; }

        public FinalMesh()
        {
            IndexList = new List<ushort>();
            VertexList = new List<FinalVertex>();
        }

    }

    public struct FinalVertex
    {
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public float Weight0 { get; set; }
        public float Weight1 { get; set; }

        public float BoneID0 { get; set; }
        public float BoneID1 { get; set; }

        public float NormalX { get; set; }
        public float NormalY { get; set; }
        public float NormalZ { get; set; }

        public float TextureU { get; set; }
        public float TextureV { get; set; }

        public float ColorR { get; set; }
        public float ColorG { get; set; }
        public float ColorB { get; set; }
        public float ColorA { get; set; }

    }


    public class FinalMaterialLine
    {
        public string TextureName { get; set; }
        public float[] TextureData { get; set; }
        public int TextureUnknown { get; set; }
    }

    public class FinalBoneLine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Parent { get; set; }
        public float[] Values { get; private set; }

        public FinalBoneLine(int id, int parent, string name)
        {
            ID = id;
            Name = name;
            Parent = parent;
            Values = new float[26];
        }
    }


}
