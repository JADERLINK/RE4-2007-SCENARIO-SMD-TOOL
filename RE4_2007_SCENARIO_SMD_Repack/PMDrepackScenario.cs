using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_2007_SCENARIO_SMD_Repack;

namespace RE4_PMD_Repack
{
    public static partial class PMDrepack
    {
        public static void CreateScenarioPMD(string pmdPath, 
            StartStructure startStructure, 
            IntermediaryNodeGroup intermediaryNodeGroup,
            FinalBoneLine boneLine, 
            Dictionary<string, FinalMaterialLine> UseMaterial,
            SMDLineIdx smdLine
            ) 
        {
            // estrutura intermediaria
            IntermediaryStructure intermediaryStructure = MakeIntermediaryStructure(startStructure, smdLine);

            // estrutura final
            FinalStructure finalStructure = MakeFinalStructure(intermediaryStructure, new IntermediaryNodeGroup[] { intermediaryNodeGroup }, true);

            //finaliza e cria o arquivo pmd
            MakeFinalPmdFile(pmdPath, finalStructure, new FinalBoneLine[] { boneLine }, UseMaterial);
        }



    }
}
