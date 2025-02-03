using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_2007_PMD_REPACK;

namespace RE4_2007_SCENARIO_SMD_REPACK
{
    public static class PMDrepackScenario
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
            IntermediaryStructure intermediaryStructure = PMDrepackIntermediary.MakeIntermediaryStructure(startStructure, smdLine);

            // estrutura final
            FinalStructure finalStructure = PMDrepackFinal.MakeFinalStructure(intermediaryStructure, new IntermediaryNodeGroup[] { intermediaryNodeGroup }, true);

            //finaliza e cria o arquivo pmd
            PMDrepackFinal.MakeFinalPmdFile(pmdPath, finalStructure, new FinalBoneLine[] { boneLine }, UseMaterial);
        }

    }
}
