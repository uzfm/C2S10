using System;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.IO;


//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace MVision
{
    class Flow
    {

       
        public static bool PotocStartPredict;
        public static bool PotocStartCamera;
       public static bool PotocStartHID;

        static Thread PotocPredict;
        static Thread PotocCamera;
        static Thread PotocHID;

        





        //HID READ
        static public void StartPotocHID() {

         if( (PotocHID==null) ||  (PotocHID.ThreadState==System.Threading.ThreadState.Stopped  ) ) { 
                    PotocHID = new Thread(PotocHIDFunction);
                    PotocHID.Priority = ThreadPriority.Lowest;
                    PotocHID.Name = "HID";
                    // запускаем поток
                    PotocHID.Start();
                    PotocStartHID = true;
        }
    }

        static void PotocHIDFunction() {
            USB_HID.HID_Read();

        }



        static public void StartPotocCamera() {
            PotocCamera = new Thread(PotocFlowCamera);
            PotocCamera.Priority = ThreadPriority.AboveNormal;
            PotocCamera.Name = "Camera";
            // запускаем поток
            PotocCamera.Start();
            PotocStartCamera = true;

            //StartPotocHID();
           
        }



         public void StartPotocPredict()
        {
            PotocPredict = new Thread(PotocFlowPredict);
            PotocPredict.Priority = ThreadPriority.AboveNormal;
            PotocPredict.Name = "Predict";
            // запускаем поток
            PotocPredict.Start();
            PotocStartPredict = true;

         

        }


        static FlowCamera FlowCamera = new FlowCamera();

        static FlowAnalis FlowAnalis = new FlowAnalis();

        static void PotocFlowCamera(){
               FlowCamera.СheckAnalis();
        }

         void PotocFlowPredict()
        {
           // FlowAnalis.FindClass();
            AnalisPredict analisPredict = new AnalisPredict();
            analisPredict.FindClass();

        }

        static public void StopPotocCamera() { PotocStartCamera = false;}
        static public void StopPotocPredict() { PotocStartPredict = false; }
        static public void StopPotocHID() { PotocStartHID = false; }
 





      

  











        static System.Diagnostics.Process _process = null;


        static public void ProcessLerningFunction(string DataPath) {
            try
            {

               //flowCamera.Stop_GPU();

                System.Diagnostics.ProcessStartInfo startInfo = new ProcessStartInfo();
                _process = null;

                startInfo = new System.Diagnostics.ProcessStartInfo( @"../../../../../MachineLearning\bin\Debug\net5.0-windows\TenserflowKeras.exe");

                startInfo.ArgumentList.Add("C2S10");
                startInfo.ArgumentList.Add(DataPath);

                //startInfo.Arguments = DataPath;
                _process = System.Diagnostics.Process.Start(startInfo);

                
      
            }catch { }
        }



      
        static public void ProcessLoadImagesFunction(bool Run){
  
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(
                @"../../../../LoadImage\WindowsFormsApp1\bin\Debug\LoadImage.exe");


          

            if (Run) { _process = System.Diagnostics.Process.Start(startInfo); } else{
                _process.Kill();
                _process.Close();
            }
        }





    }
}
