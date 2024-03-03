using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_2007_SCENARIO_SMD_REPACK
{
    public class IdxScenarioLoader
    {
        public static IdxScenario Loader(StreamReader idxFile)
        {
            Dictionary<string, string> pair = new Dictionary<string, string>();

            string line = "";
            while (line != null)
            {
                line = idxFile.ReadLine();
                if (line != null && line.Length != 0)
                {
                    var split = line.Trim().Split(new char[] { ':' });

                    if (line.TrimStart().StartsWith(":") || line.TrimStart().StartsWith("#") || line.TrimStart().StartsWith("/") || line.TrimStart().StartsWith("\\"))
                    {
                        continue;
                    }
                    else if (split.Length >= 2)
                    {
                        string key = split[0].ToUpper().Trim();

                        if (!pair.ContainsKey(key))
                        {
                            pair.Add(key, split[1].Trim());
                        }

                    }

                }
            }

            //----

            IdxScenario idxScenario = new IdxScenario();

            int smdAmount = 0;

            //SmdAmount
            try
            {
                string value = RE4_2007_PMD_REPACK.Utils.ReturnValidDecValue(pair["SMDAMOUNT"]);
                smdAmount = int.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }

            //SmdFileName
            try
            {
                string value = pair["SMDFILENAME"].Trim();
                value = value.Replace('/', '\\')
              .Replace(":", "").Replace("*", "").Replace("\"", "").Replace("|", "")
              .Replace("<", "").Replace(">", "").Replace("?", "").Replace(" ", "_");

                value = value.Split('\\').Last();

                if (value.Length == 0)
                {
                    value = "null";
                }

                var fileinfo = new FileInfo(value);
                idxScenario.SmdFileName = fileinfo.Name.Remove(fileinfo.Name.Length - fileinfo.Extension.Length, fileinfo.Extension.Length) + ".SMD";
            }
            catch (Exception)
            {
            }

            //PmdBaseName
            try
            {
                string value = pair["PMDBASENAME"].Trim();
                value = value.Replace('/', '\\')
              .Replace(":", "").Replace("*", "").Replace("\"", "").Replace("|", "")
              .Replace("<", "").Replace(">", "").Replace("?", "").Replace(" ", "_");

                value = value.Split('\\').Last();

                if (value.Length == 0)
                {
                    value = "null";
                }
                idxScenario.PmdBaseName = value.Trim('_');
            }
            catch (Exception)
            {
            }

            //UseIdxPmdMaterial
            try
            {
                string value = pair["USEIDXPMDMATERIAL"];
                idxScenario.UseIdxPmdMaterial = bool.Parse(value);
            }
            catch (Exception)
            {
            }


            //---

            SMDLineIdx[] smdLines = new SMDLineIdx[smdAmount];

            BinRenderBox[] boxes = new BinRenderBox[smdAmount];

            for (int i = 0; i < smdAmount; i++)
            {

                #region SMDLineIdx
                string scaleXkey = i.ToString("D3") + "_SCALEX";
                string scaleYkey = i.ToString("D3") + "_SCALEY";
                string scaleZkey = i.ToString("D3") + "_SCALEZ";

                string positionXkey = i.ToString("D3") + "_POSITIONX";
                string positionYkey = i.ToString("D3") + "_POSITIONY";
                string positionZkey = i.ToString("D3") + "_POSITIONZ";

                string angleXkey = i.ToString("D3") + "_ANGLEX";
                string angleYkey = i.ToString("D3") + "_ANGLEY";
                string angleZkey = i.ToString("D3") + "_ANGLEZ";

                SMDLineIdx smdline = new SMDLineIdx();

                smdline.scaleX = GetFloat(ref pair, scaleXkey, 1f);
                smdline.scaleY = GetFloat(ref pair, scaleYkey, 1f);
                smdline.scaleZ = GetFloat(ref pair, scaleZkey, 1f);
                smdline.positionX = GetFloat(ref pair, positionXkey, 0f);
                smdline.positionY = GetFloat(ref pair, positionYkey, 0f);
                smdline.positionZ = GetFloat(ref pair, positionZkey, 0f);
                smdline.angleX = GetFloat(ref pair, angleXkey, 0f);
                smdline.angleY = GetFloat(ref pair, angleYkey, 0f);
                smdline.angleZ = GetFloat(ref pair, angleZkey, 0f);

                smdLines[i] = smdline;
                #endregion


                #region boxes
                BinRenderBox box = new BinRenderBox();

                string key1 = i.ToString("D3") + "_DRAWDISTANCENEGATIVEX";
                string key2 = i.ToString("D3") + "_DRAWDISTANCENEGATIVEY";
                string key3 = i.ToString("D3") + "_DRAWDISTANCENEGATIVEZ";

                string key4 = i.ToString("D3") + "_DRAWDISTANCEPOSITIVEX";
                string key5 = i.ToString("D3") + "_DRAWDISTANCEPOSITIVEY";
                string key6 = i.ToString("D3") + "_DRAWDISTANCEPOSITIVEZ";

                box.DrawDistanceNegativeX = GetFloat(ref pair, key1, -327675f);
                box.DrawDistanceNegativeY = GetFloat(ref pair, key2, -327675f);
                box.DrawDistanceNegativeZ = GetFloat(ref pair, key3, -327675f);

                box.DrawDistancePositiveX = GetFloat(ref pair, key4, 655350f);
                box.DrawDistancePositiveY = GetFloat(ref pair, key5, 655350f);
                box.DrawDistancePositiveZ = GetFloat(ref pair, key6, 655350f);

                boxes[i] = box;
                #endregion

            }

            // ----

            idxScenario.SmdAmount = smdAmount;
            idxScenario.BinRenderBoxes = boxes;
            idxScenario.SmdLines = smdLines;

            //---
            idxFile.Close();


            return idxScenario;
        }

        public static float GetFloat(ref Dictionary<string, string> pair, string key, float DefaultValue)
        {
            float res = DefaultValue;

            if (pair.ContainsKey(key))
            {
                try
                {
                    string value = RE4_2007_PMD_REPACK.Utils.ReturnValidFloatValue(pair[key]);
                    res = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    res = DefaultValue;
                }
            }

            return res;
        }

    }



    public class IdxScenario
    {
        public int SmdAmount = 0;

        public SMDLineIdx[] SmdLines;

        public BinRenderBox[] BinRenderBoxes;

        public string SmdFileName = "null.smd"; 

        public string PmdBaseName = "null";

        public bool UseIdxPmdMaterial = false;
    }

    public class SMDLineIdx
    {
        public float positionX;
        public float positionY;
        public float positionZ;

        public float angleX;
        public float angleY;
        public float angleZ;

        public float scaleX;
        public float scaleY;
        public float scaleZ;
    }


    public class BinRenderBox
    {
        public float DrawDistanceNegativeX;
        public float DrawDistanceNegativeY;
        public float DrawDistanceNegativeZ;

        public float DrawDistancePositiveX;
        public float DrawDistancePositiveY;
        public float DrawDistancePositiveZ;

    }
}
