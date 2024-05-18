using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.IO;



//using System.Text.RegularExpressions;
//using Microsoft;
//using Microsoft.ML;
//using static Microsoft.ML.DataOperationsCatalog;
//using Microsoft.ML.Vision;
//using Microsoft.ML.Calibrators;
//using Microsoft.ML.Runtime;
//using Microsoft.ML.Trainers;
//using System.Windows.Forms;
//using System.Text;
//using System.Threading.Tasks;
//using System.ComponentModel;



namespace MVision
{






    public class TF_DT
    {
    
     public string PathTF_ML;
     public string PathTF_ML_SAMPLES;



        // Load Name Class of MODEl
        public  string TF_CreatDirectoryPathS(/*int ID, */ string PathDt){


            try{
                int idx = 0;

                if ("DataSet" != Path.GetFileName(MLD.Path)) { Directory.CreateDirectory(Path.Combine(MLD.Path)); }

                string[] SamlCatalogPath = Directory.GetDirectories(MLD.Path);
                bool PathFind = false;

                if (SamlCatalogPath.Length != 0)
                {
                   
                    for (idx = 0; idx < SamlCatalogPath.Length ; idx++)
                    {
                      //  string value;
                      //  if (value == Path.GetFileName(SamlCatalogPath[idx])) { PathFind = true; break; }
                    }

                    if (PathFind == true)
                    {

                        PathTF_ML = SamlCatalogPath[idx];

                        if (false == Directory.Exists(PathTF_ML)) { Directory.CreateDirectory(PathTF_ML); }

                        if (false == Directory.Exists(Path.Combine(PathTF_ML, "SAMPLES")))                        { Directory.CreateDirectory(Path.Combine(PathTF_ML, "SAMPLES")); }
                        if (false == Directory.Exists(Path.Combine(PathTF_ML, "SAMPLES" + "//" + GridData.GOOD))) { Directory.CreateDirectory(Path.Combine(PathTF_ML, "SAMPLES" + "//" + GridData.GOOD)); }//створити назву "Good"
                        if (false == Directory.Exists(Path.Combine(PathTF_ML, "SAMPLES" + "//" + PathDt)))        { Directory.CreateDirectory(Path.Combine(PathTF_ML, "SAMPLES" + "//" + PathDt)); }//створити назву Img



                        PathTF_ML_SAMPLES = Path.Combine(PathTF_ML, "SAMPLES");


                    }
                }

            } catch { return null; }

                return MLD.Path;
           
        }

        
    }





        public class ResaltRgst   {
        public Bitmap Img { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }
        public float[] ValueDt { get; set; }
    }





}
    