using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_2007_SCENARIO_SMD_EXTRACT
{
    public class PmdMaterialPart
    {
        public string TextureName;
        public int TextureEnable;
        public float[] TextureData; // new float[17];

        public override int GetHashCode()
        {
            return TextureName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PmdMaterialPart))
            {
                return false;
            }

            var m = obj as PmdMaterialPart;

            for (int i = 0; i < 17; i++)
            {
                string v1 = m.TextureData[i].ToString("f", System.Globalization.CultureInfo.InvariantCulture);
                string v2 = TextureData[i].ToString("f", System.Globalization.CultureInfo.InvariantCulture);

                if (v1 != v2)
                {
                    return false;
                }
            }
            return m.TextureName == TextureName
                && m.TextureEnable == TextureEnable;
        }
    }
}
