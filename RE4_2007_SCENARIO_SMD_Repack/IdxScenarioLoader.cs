using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_2007_SCENARIO_SMD_Repack
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

                    if (line.TrimStart().StartsWith(":") || line.TrimStart().StartsWith("#") || line.TrimStart().StartsWith("/"))
                    {
                        continue;
                    }
                    else if (split.Length >= 2)
                    {
                        string key = split[0].ToUpper().Trim();

                        if (!pair.ContainsKey(key))
                        {
                            pair.Add(key, split[1]);
                        }

                    }

                }
            }

            //----

            IdxScenario idxScenario = new IdxScenario();

            int smdAmount = 0;

            //SMDAMOUNT
            try
            {
                string value = RE4_PMD_Repack.Utils.ReturnValidDecValue(pair["SMDAMOUNT"]);
                smdAmount = int.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }

            //SMDFILENAME
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



            //---

            SMDLineIdx[] smdLines = new SMDLineIdx[smdAmount];

            BinRenderBox[] boxes = new BinRenderBox[smdAmount];

            for (int i = 0; i < smdAmount; i++)
            {

                string key1 = i.ToString("D3") + "_DRAWDISTANCENEGATIVEX";
                string key2 = i.ToString("D3") + "_DRAWDISTANCENEGATIVEY";
                string key3 = i.ToString("D3") + "_DRAWDISTANCENEGATIVEZ";

                string key4 = i.ToString("D3") + "_DRAWDISTANCEPOSITIVEX";
                string key5 = i.ToString("D3") + "_DRAWDISTANCEPOSITIVEY";
                string key6 = i.ToString("D3") + "_DRAWDISTANCEPOSITIVEZ";

                string key7 = i.ToString("D3") + "_SCALEX";
                string key8 = i.ToString("D3") + "_SCALEY";
                string key9 = i.ToString("D3") + "_SCALEZ";

                string key11 = i.ToString("D3") + "_POSITIONX";
                string key12 = i.ToString("D3") + "_POSITIONY";
                string key13 = i.ToString("D3") + "_POSITIONZ";

                string key14 = i.ToString("D3") + "_ANGLEX";
                string key15 = i.ToString("D3") + "_ANGLEY";
                string key16 = i.ToString("D3") + "_ANGLEZ";

                SMDLineIdx smdline = new SMDLineIdx();
                BinRenderBox box = new BinRenderBox();


                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key1]);
                    box.DrawDistanceNegativeX = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key2]);
                    box.DrawDistanceNegativeY = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key3]);
                    box.DrawDistanceNegativeZ = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }



                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key4]);
                    box.DrawDistancePositiveX = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key5]);
                    box.DrawDistancePositiveY = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key6]);
                    box.DrawDistancePositiveZ = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }


                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key7]);
                    smdline.scaleX = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    smdline.scaleX = 1f;
                }
                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key8]);
                    smdline.scaleY = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    smdline.scaleY = 1f;
                }
                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key9]);
                    smdline.scaleZ = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    smdline.scaleZ = 1f;
                }


                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key11]);
                    smdline.positionX = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key12]);
                    smdline.positionY = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key13]);
                    smdline.positionZ = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }


                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key14]);
                    smdline.angleX = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key15]);
                    smdline.angleY = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
                try
                {
                    string value = RE4_PMD_Repack.Utils.ReturnValidFloatValue(pair[key16]);
                    smdline.angleZ = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }


                boxes[i] = box;
                smdLines[i] = smdline;
            }

            // ----

            idxScenario.SmdAmount = smdAmount;
            idxScenario.BinRenderBoxes = boxes;
            idxScenario.SmdLines = smdLines;

            //---
            idxFile.Close();


            return idxScenario;
        }


    }



    public class IdxScenario
    {
        public int SmdAmount = 0;

        public SMDLineIdx[] SmdLines;

        public BinRenderBox[] BinRenderBoxes;

        public string SmdFileName = "null.smd"; 

        public string PmdBaseName = "null"; 
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
