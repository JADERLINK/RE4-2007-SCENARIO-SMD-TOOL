using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_2007_PMD_REPACK
{
    public class IntermediaryStructure
    {
        public Dictionary<string, IntermediaryMesh> Meshs { get; set; }

        public IntermediaryStructure()
        {
            Meshs = new Dictionary<string, IntermediaryMesh>();
        }
    }

    public class IntermediaryMesh 
    {
        public string MaterialName { get; set; }

        public List<IntermediaryFace> Faces { get; set; }

        public IntermediaryMesh()
        {
            Faces = new List<IntermediaryFace>();
        }
    }


    public class IntermediaryFace
    {
        public List<IntermediaryVertex> Vertexs { get; set; }
       
        public IntermediaryFace() 
        {
            Vertexs = new List<IntermediaryVertex>();
        }
    }

    public class IntermediaryVertex
    {
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public float Weight0 { get; set; }
        public float Weight1 { get; set; }

        public int BoneID0 { get; set; }
        public int BoneID1 { get; set; }

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


    public class IntermediaryNodeGroup
    {
        public string GroupName { get; set; }
        public int SkeletonIndex { get; set; }
        public string[] MaterialList { get; set; }
    }
}
