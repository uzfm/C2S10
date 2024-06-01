
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections.Concurrent;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;


namespace MVision
{


    class FlowCamera
    {

        public static ConcurrentQueue<Image<Bgr, byte>> BoxM = new ConcurrentQueue<Image<Bgr, byte>>();
        public static ConcurrentQueue<Image<Bgr, byte>> BoxS = new ConcurrentQueue<Image<Bgr, byte>>();
        public static ConcurrentQueue<Image<Bgr, byte>[]> BoxImgs = new ConcurrentQueue<Image<Bgr, byte>[]>();

        public static ConcurrentQueue<CutImg> BuferIMG = new ConcurrentQueue<CutImg>();


        public class CutImg
        {
            public Mat[] Img = new Mat[2];
            public Rectangle[] ROI = new Rectangle[2];
            public double[] SizeCorect = new double[2];
            public double[] Aria = new double[2];
            public Elongated[] Elong = new Elongated[2];
            public int ID { get; set; }
        }

        static Image<Bgr, byte>[] imgDT = new Image<Bgr, byte>[2];
        //static int RestartCam = 0;
        static bool RestartCamStop = false;
        public static bool START_tImg = false;



        static public int CameraEroor;

        static   FlowAnalis flowAnalis = new FlowAnalis();



       public static ML_Net  ML_NetM = new ML_Net();
       public static ML_Nets ML_NetS = new ML_Nets();

        public ML_Net.PDT PDTm = new ML_Net.PDT();
        public ML_Net.PDT PDTs = new ML_Net.PDT();




        public void Stop_GPU()
        {
            ML_NetM.Stop_GPU();

        }


        public  static ML_Net.PDT  FlowSlave (Mat x) { return ML_NetM.PredictImage(x); }
        public  static ML_Net.PDT  FlowMaster(Mat x) { return ML_NetM.PredictImage(x); }

        public static ML_Net.PDT[]   Predict(Image<Bgr, byte>[] M, Image<Bgr, byte>[] S) { return ML_NetM.PredictImage(M,S); }


        static   public void InstML()
        {
            try{
                ML_NetM.Main();
                //ML_NetS.Main();

                ML_NetM.PredictImage("Load.JPG");
            } catch { Help.ErrorMesag("Failed to load the model"); }
        }

        public void СheckAnalis(){

            while (Flow.PotocStartCamera){
                
                if (((BoxM.Count >= 1) && (BoxS.Count >= 1))){

                    BoxM.TryDequeue(out imgDT[0]);
                    BoxS.TryDequeue(out imgDT[1]);

                    flowAnalis.FindConturs(imgDT);
                    imgDT = new Image<Bgr, byte>[2];

                }
                //     FlowCamera.CameraCountFPS[0] = FlowCamera.BoxS.Count;
               // Thread.Sleep(1);
           }

        }

    }



    struct Elongated
    {
        public int Height;
        public int Width;
    }


    class FlowAnalis {



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //==============================                                 ВИЗНАЧАЄМ ТА МАЛЮЄМО КОНТУРИ з FOTO                                                      ==============================//
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


       public static bool LiveViewSetings = true;
        static Mat NewMat;
        static Image<Bgr, byte> img;
        static Image<Bgr, byte>[] Old_IMG = new Image<Bgr, byte>[2];
        //static Image<Bgr, byte>[] Main_IMG = new Image<Bgr, byte>[2];

        // Live Wive
        public static PictureBox LiveViewTvMaster = new PictureBox();
        static public void LiveViewTV(PictureBox LiveView) { LiveViewTvMaster = LiveView; }


        // Live Wive
        public static PictureBox LiveViewTvSlave = new PictureBox();
        static public void LiveViewTV2(PictureBox LiveView) { LiveViewTvSlave = LiveView; }

        List<CutImg> ListCutImg = new List<CutImg>();
        CutImg CutImgClass = new CutImg();

        static ConcurrentQueue<CutList>   BuferList = new ConcurrentQueue<CutList>();
        public static int ErrorX; //розбіжність семла з камер Master Slave по X
        public static int ErrorY; //розбіжність семла з камер Master Slave по Y
        public static int ErrorCount; //розбіжність семла з камер Master Slave по Y



        class CutImg
        {
            public Mat[] Img = new Mat[2];
            public double[] Aria = new double[2];
            public Elongated [] Elong = new Elongated[2];
            public double[] SizeCorect = new double[2];
            public Rectangle [] ROI = new Rectangle[2];
            public int ID { get; set; }
        }



        class CutList
        {
            public CutImg ListDT = new CutImg();
            public int Nam { get; set; }
        }



 
       public class SizeApertur {
         public static  int Height = 0;
         public static  int Width = 0;

        }



      public static  Rectangle ROI_Main;


        public bool FindConturs (Image<Bgr, byte>[] ImagsCam)
        {
            const int Rosolution = 10;
            Stopwatch watch;
            watch = Stopwatch.StartNew();

            CutImg TempColl = new CutImg();

            if (EMGU.StartAnalis == true)
            {
                EMGU.StartAnalis = false;
                CountContact[Master] = 0;
                CountContact[Slave] = 0;
                ImagContact[Slave] = null;
                ImagContact[Master] = null;

            }




            ROI_Main = ImagsCam[0].ROI;

            for (int ID = 0; ID < 2; ID++) {
                //зжимаємо фото
                img = ImagsCam[ID].Resize(ImagsCam[ID].Width/ Rosolution, ImagsCam[ID].Height/ Rosolution, Inter.Linear);
         
                //mg = imgtest1;
                NewMat = new Mat();

                // склейка фото 3 трьох частитн
                if (CountContact[ID] >= 2) { ImagContact[ID].ROI = new Rectangle(0, (img.Height), img.Width, (img.Height)); }
                else { CountContact[ID]++; }

                if (ImagContact[ID] != null)
                {
                    CvInvoke.VConcat(ImagContact[ID], img, NewMat);
                } else { NewMat = img.Mat; }


                //    NewMat = img.Mat;
                ImagContact[ID] = NewMat.ToImage<Bgr, byte>();
            }

                Image<Bgr, byte> [] imgAVT = new Image<Bgr, byte>[2];
           
                imgAVT[Master] = ImagContact[Master].Clone();
                imgAVT[Slave]  = ImagContact[Slave].Clone();


            if (ImagContact[Master].Height == (img.Height * 2)){

                SizeApertur.Height = ImagsCam[Master].Height;
                SizeApertur.Width   = ImagsCam[Master].Width;



                //**----------   визначаєм контори  -----------------**//
                CutCTR CutCTR_SV1 = FindBlobMini(ImagContact[0], 0);
                CutCTR CutCTR_SV2 = FindBlobMini(ImagContact[1], 0);


         
                // watch.Stop();
                // var ElapsedMs = watch.ElapsedMilliseconds;
                //Debug.WriteLine("Predict:--" + "PCS-" + CountBF.ToString() + "Time-" + elapsedMs.ToString() + " ms"); ;

                // Запускаємо функції в різних потоках
                // Task<CutCTR> task1 = Task.Run(() => FindBlobMini(ImagContact[0], 0));
                // Task<CutCTR> task2 = Task.Run(() => FindBlobMini(ImagContact[1], 1));

                // Очікуємо завершення обох завдань
                // Task.WaitAll(task1, task2);

                for (int ID = 0; ID < 2; ID++)
                {
                   //if (Old_IMG[ID] == null) { Old_IMG[0] = new Image<Bgr, byte>(ImagsCam[0].Width, ImagsCam[0].Height); }

                    CutCTR CutCTR_SV= new CutCTR();
                    if (ID==Master) {  CutCTR_SV = CutCTR_SV1; } else { CutCTR_SV = CutCTR_SV2; }
                   
                 
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    for (int i = 0; i < CutCTR_SV.CUT.Length; i++)
                    {

                        int scale = 0;
                        /****************************/
                        // відступ центрування зразків
                        /****************************/
                        int ScaleNorm = 22;

                      
                        if ((CutCTR_SV.ROI[i].Width >= ScaleNorm) ||  (CutCTR_SV.ROI[i].Height >= ScaleNorm))  {

                            if (CutCTR_SV.ROI[i].Width > CutCTR_SV.ROI[i].Height)
                            {
                                scale = (CutCTR_SV.ROI[i].Width - CutCTR_SV.ROI[i].Height) / 2;
                                CutCTR_SV.ROI[i].Y = CutCTR_SV.ROI[i].Y - scale;
                                CutCTR_SV.ROI[i].Height = CutCTR_SV.ROI[i].Width;
                            }

                            if (CutCTR_SV.ROI[i].Height > CutCTR_SV.ROI[i].Width)
                            {
                                scale = (CutCTR_SV.ROI[i].Height - CutCTR_SV.ROI[i].Width) / 2;
                                CutCTR_SV.ROI[i].X = CutCTR_SV.ROI[i].X - scale;
                                CutCTR_SV.ROI[i].Width = CutCTR_SV.ROI[i].Height;
                            }

                            }else {
                           

                                scale = (ScaleNorm - CutCTR_SV.ROI[i].Height) / 2;
                                CutCTR_SV.ROI[i].Y = CutCTR_SV.ROI[i].Y - scale;
                                CutCTR_SV.ROI[i].Height = ScaleNorm;


                                scale = (ScaleNorm - CutCTR_SV.ROI[i].Width) / 2;
                                CutCTR_SV.ROI[i].X = CutCTR_SV.ROI[i].X - scale;
                                CutCTR_SV.ROI[i].Width = ScaleNorm;

                             }



                        Rectangle ROI = CutCTR_SV.ROI[i];

                        if (CutCTR_SV.CUT[i]){
                        
                            ImagsCam[0].ROI = ROI_Main;
                            int CatHeight = imgAVT[ID].Height / 2;
                            int CatLeng = CutCTR_SV.ROI[i].Y + CutCTR_SV.ROI[i].Height;

                            
                            //===========  IMG CATING =============================
                            if ((CatLeng >= CatHeight) && (CutCTR_SV.ROI[i].Y < CatHeight))
                            {
                                 Elongated Elong_Save = new Elongated();

                                ROI = CutCTR_SV.ROI[i];

                                //Top IMG
                                int catW = CatHeight - CutCTR_SV.ROI[i].Y;
                                ROI.Height = ROI.Height - catW;
                                ROI.Y = CatHeight;
                                Rectangle ROIT = ROI;
                               
                                ROI.X *= Rosolution;
                                ROI.Y = 0;
                                ROI.Width *= Rosolution;
                                ROI.Height *= Rosolution;
                                //CvInvoke.Rectangle(ImagsCam[0], ROI, new Bgr(Color.Blue).MCvScalar, Rosolution);
                                ImagsCam[ID].ROI = ROI;
                                
                                //ROI fo Save
                                Elong_Save.Height = ROI.Height;
                                Elong_Save.Width = ROI.Width;

                                //Bottom IMG
                                ROI = CutCTR_SV.ROI[i];
                                ROI.Height = CatHeight - CutCTR_SV.ROI[i].Y;
                                Rectangle ROIB = ROI;

                                ROI.X *= Rosolution;
                                ROI.Y *= Rosolution;
                                ROI.Size *= Rosolution;
                                if (Old_IMG[ID] == null) { Old_IMG[ID] = new Image<Bgr, byte>(ROI_Main.Width, ROI_Main.Height);  }

                                Old_IMG[ID].ROI = ROI;
                                //ROI fo Save
                                Elong_Save.Height = Elong_Save.Height + ROI.Height;
                                Elong_Save.Width = ROI.Width;



                                Mat NewMatAI = new Mat();
                                CvInvoke.VConcat(Old_IMG[ID], ImagsCam[ID], NewMatAI);


                          
                                TempColl = new CutImg();
                                TempColl.Aria[ID]    = (CutCTR_SV.Aria[i] * (double)Rosolution);
                                TempColl.Img[ID] = NewMatAI.ToImage<Bgr, byte>().Copy().Resize(64, 64, Inter.Linear).Mat;
                                TempColl.ROI[ID] = ROI;
                                TempColl.Elong[ID] = Elong_Save;

                                if (ID==Master) {
                                    if ((SAV.DT.Analys.SelectPS) && (SAV.DT.Device.LiveView)) { 
                                    CvInvoke.Rectangle(imgAVT[ID].Mat, ROIT, new Bgr(Color.Blue).MCvScalar, 1);
                                    CvInvoke.Rectangle(imgAVT[ID].Mat, ROIB, new Bgr(Color.Red).MCvScalar, 1); }
                             
                                    ListCutImg.Add(TempColl); } else {

                                    for (int j = 0; j < ListCutImg.Count; j++)
                                    {
                                        Rectangle roi1 = TempColl.ROI[Slave];
                                        Rectangle roi2 = ListCutImg[j].ROI[Master];

                                        // Перевірка на перетин за зазначеним порогом (5 пікселів)
                                        if ((roi1.X <= roi2.X && Math.Abs(roi1.X - roi2.X) < SAV.DT.Analys.PositionErrorX) ||
                                            (roi1.X >= roi2.X && Math.Abs(roi1.X - roi2.X) < SAV.DT.Analys.PositionErrorX))
                                        {
                                            if (ListCutImg[j].Img[Slave] == null)
                                            {
                                                if ((SAV.DT.Analys.SelectPS)&& (SAV.DT.Device.LiveView)) { 
                                                CvInvoke.Rectangle(imgAVT[ID].Mat, ROIT, new Bgr(Color.Blue).MCvScalar, 1);
                                                CvInvoke.Rectangle(imgAVT[ID].Mat, ROIB, new Bgr(Color.Red).MCvScalar, 1);}
                                                ListCutImg[j].Aria[Slave] = TempColl.Aria[Slave];
                                                ListCutImg[j].Elong[Slave] = Elong_Save;
                                                ListCutImg[j].Img[Slave] = TempColl.Img[Slave];
                                                ListCutImg[j].ROI[Slave] = TempColl.ROI[Slave];
                                                break;
                                            }
                                        } 
                                        } 
                                        }



                

                            }
                            else
                            {


                                //===========  IMG FUL =============================

                                CatHeight = imgAVT[ID].Height / 2;
                                CatLeng = CutCTR_SV.ROI[i].Y + CutCTR_SV.ROI[i].Height;

                                if ((CutCTR_SV.ROI[i].Y >= CatHeight) && (CatLeng < imgAVT[ID].Height)){


                                        ROI = CutCTR_SV.ROI[i];

                                  //Top IMG
                                  ROI.Y -= imgAVT[ID].Height / 2;
                                  ROI.Y *= Rosolution;
                                  ROI.X *= Rosolution;
                                  ROI.Width *= Rosolution;
                                  ROI.Height *= Rosolution;

                                    ImagsCam[ID].ROI = ROI;
                                    TempColl = new CutImg();
                                    TempColl.Aria[ID] = (CutCTR_SV.Aria[i] * (double)Rosolution);
                                    TempColl.Img[ID] = ImagsCam[ID].Copy().Resize(64, 64, Inter.Linear).Mat;
                                    TempColl.ROI[ID] = ROI;
                                    TempColl.Elong[ID].Height = ROI.Height;
                                    TempColl.Elong[ID].Width = ROI.Width;


                                    /************************    Тут вирізає картинки з Master CAM    *****************************************/
                                    if (ID == Master) {

                                        if ((SAV.DT.Analys.SelectPS)&& (SAV.DT.Device.LiveView)) { CvInvoke.Rectangle(imgAVT[ID].Mat, CutCTR_SV.ROI[i], new Bgr(Color.LightSeaGreen).MCvScalar, 1); }

                                        ListCutImg.Add(TempColl);
                                    } else {

                                        /************************    Тут вирізає картинки з Slave CAM                   *****************************************/
                                        /************************    Знаходить найбилищий зразок до семла з Master CAM   *****************************************/
                                        
                                        int MinX = SAV.DT.Analys.PositionErrorX;
                                        int MinY = SAV.DT.Analys.PositionErrorX;
                                        int IdxJ = 0;
                                        for (int j = 0; j < ListCutImg.Count; j++)
                                        {

                                            if (ListCutImg[j].Img[Slave] == null)
                                            {
                                                Rectangle roi1 = TempColl.ROI[Slave];
                                                Rectangle roi2 = ListCutImg[j].ROI[Master];

                                                var MathAbsX = Math.Abs(roi1.X - roi2.X);
                                                var MathAbsY = roi1.Y - roi2.Y;

                                                 //===========  IMG CATING ============================= //
                                                if ((MathAbsX <= MinX) && (MathAbsY <= MinX) ) {


                                                        MinX = MathAbsX; IdxJ = j;  MinY = MathAbsY; 

                                                    }
                          
                                            }
                                        }
                                        ErrorX = MinX;
                                        ErrorY = MinY;

                                        // Перевірка на перетин за зазначеним порогом (5 пікселів)
                                        if (ListCutImg.Count > 0){ 
                                        if (MinX < SAV.DT.Analys.PositionErrorX)
                                            {

                                                if ((SAV.DT.Analys.SelectPS)&&(SAV.DT.Device.LiveView)) { CvInvoke.Rectangle(imgAVT[ID].Mat, CutCTR_SV.ROI[i], new Bgr(Color.LightSeaGreen).MCvScalar, 1); }

                                                    ListCutImg[IdxJ].Aria[Slave] = TempColl.Aria[Slave];
                                                    ListCutImg[IdxJ].Img[Slave]  = TempColl.Img [Slave];
                                                    ListCutImg[IdxJ].ROI[Slave]  = TempColl.ROI [Slave];
                                                    ListCutImg[IdxJ].Elong[Slave].Height = TempColl.ROI[Slave].Height;
                                                    ListCutImg[IdxJ].Elong[Slave].Width  = TempColl.ROI[Slave].Width;


                                                ImagsCam[ID].ROI = ROI_Main;
                                                    //CvInvoke.Rectangle(ImagsCam[Slave], ROI, new Bgr(Color.Blue).MCvScalar, 9);
                                                    //break;
                                                } else {
                                           ErrorCount++;  
                                            } }

                                    } }else {
                                
                                    if ((CutCTR_SV.ROI[i].Y < CatHeight) && (CatLeng < imgAVT[ID].Height) && ( ID==Slave ))
                                    {

                                        ROI = CutCTR_SV.ROI[i];
                                        ROI.Y *= Rosolution;
                                        ROI.X *= Rosolution;
                                        ROI.Width *= Rosolution;
                                        ROI.Height *= Rosolution;

                                        Old_IMG[ID].ROI = ROI;
                                        TempColl = new CutImg();
                                        TempColl.Img[ID] = Old_IMG[ID].Copy().Resize(64, 64, Inter.Linear).Mat;
                                        TempColl.ROI[ID] = ROI;




                           

                                        for (int j = 0; j < ListCutImg.Count; j++)
                                            {
                                                Rectangle roi1 = TempColl.ROI[Slave];
                                                Rectangle roi2 = ListCutImg[j].ROI[Master];

                                                // Перевірка на перетин за зазначеним порогом (5 пікселів)
                                                if ((roi1.X <= roi2.X && Math.Abs(roi1.X - roi2.X) < SAV.DT.Analys.PositionErrorX) ||
                                                    (roi1.X >= roi2.X && Math.Abs(roi1.X - roi2.X) < SAV.DT.Analys.PositionErrorX))
                                                {


                                                    if (ListCutImg[j].Img[Slave] == null)  {
                                                    if (ErrorCount !=0) { ErrorCount--; };
                                                    if ((SAV.DT.Analys.SelectPS)&& (SAV.DT.Device.LiveView)) { CvInvoke.Rectangle(imgAVT[ID].Mat, CutCTR_SV.ROI[i], new Bgr(Color.LightYellow).MCvScalar, 2); }
                                                    
                                                        ListCutImg[j].Aria[Slave] = TempColl.Aria[Slave];
                                                        ListCutImg[j].Img[Slave] = TempColl.Img[Slave];
                                                        ListCutImg[j].ROI[Slave] = TempColl.ROI[Slave];
                                                        ListCutImg[j].Elong[Slave].Height = TempColl.ROI[Slave].Height;
                                                        ListCutImg[j].Elong[Slave].Width = TempColl.ROI[Slave].Width;

                                                    Old_IMG[ID].ROI = ROI_Main;
                                                        //CvInvoke.Rectangle(ImagsCam[Slave], ROI, new Bgr(Color.Blue).MCvScalar, 9);
                                                        break;
                                                    }
                                                }
                                            }

                          

                                    }

                                }

                            }
                     
                        }
                   
                    }//перебираєм усі знайдені контури



                }





         


                if ((ListCutImg.Count != 0) && ((ListCutImg[0].Img[Master] != null) && (ListCutImg[0].Img[Slave] != null)) ){
                DTLimg[] DTLimgs = new DTLimg[1];

                for (int idx = 0; idx < ListCutImg.Count; idx++){
                     DTLimgs[0] = new DTLimg();
                   if ( (ListCutImg[idx].Img[Master] == null) || (ListCutImg[idx].Img[Slave] == null) ) { break; }



                        FlowCamera.CutImg cutImg = new FlowCamera.CutImg();

                        cutImg.Img[0] = ListCutImg[idx].Img[0];
                        cutImg.Img[1] = ListCutImg[idx].Img[1];
                        cutImg.ROI[0] = ListCutImg[idx].ROI[0];
                        cutImg.ROI[1] = ListCutImg[idx].ROI[1];
                        cutImg.Aria[0] = ListCutImg[idx].Aria[0];
                        cutImg.Aria[1] = ListCutImg[idx].Aria[1];
                        cutImg.Elong[0] = ListCutImg[idx].Elong[0];
                        cutImg.Elong[1] = ListCutImg[idx].Elong[1];
                        cutImg.SizeCorect[0] = ListCutImg[idx].SizeCorect[0];
                        cutImg.SizeCorect[1] = ListCutImg[idx].SizeCorect[1];


                        if (LiveViewSetings){ 
                        IProducerConsumerCollection<FlowCamera.CutImg> tmp = FlowCamera.BuferIMG;
                        tmp.TryAdd(cutImg);  }

                    }

                 }


                CutList cutList = new CutList();

                cutList.Nam = 1;

   

                if (Old_IMG[0] != null){

                    ImagsCam[Master].ROI = ROI_Main;
                    ImagsCam[Slave].ROI = ROI_Main;
                    Old_IMG[0].ROI = ImagsCam[0].ROI;
                   

                }else { Old_IMG[0] = new Image<Bgr, byte>(ImagsCam[0].Width, ImagsCam[0].Height);   }


                try{

             if (SAV.DT.Device.LiveView) {
                LiveViewTvMaster.Image = imgAVT[Master].ToBitmap();
                LiveViewTvSlave.Image  = imgAVT[Slave].ToBitmap();
                } }catch {  }
                
                Old_IMG[0] = ImagsCam[Master];
                Old_IMG[1] = ImagsCam[Slave];






                //FindClass(ImagContact);
            }

                //ДЛЯ ПРОГОРТУВАННЯ
            if ((EMGU.ListMast.Count < MaxImageSave) && (IMG_Test_Viwe == IMG_Test_Viwe_SET.Save))
            {
                EMGU.ListMast.Add(ImagsCam[Master].Clone().ToBitmap());
                EMGU.ListSlav.Add(ImagsCam[Slave].Clone().ToBitmap());
            }


             ///ЗБЕРІГАЄМ ФОТО ДЛЯ СИМУЛЯЦІЇ 
            if ((EMGU.ListSlav.Count < MaxImageSave*3) && (IMG_Test_Viwe == IMG_Test_Viwe_SET.Simulation))
            {
                EMGU.ListMast.Add(imgAVT[Master].ToBitmap());
                EMGU.ListSlav.Add(imgAVT[Slave].ToBitmap());
                
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.WriteLine("FIND:--" + elapsedMs.ToString() + " ms");


            ListCutImg.Clear();

       



            return true;

        }


















        static HashSet<int> Treker = new HashSet<int>();
        HashSet<int> TrekerRW = new HashSet<int>();
        int CountCNT = new int();

        /********************   пошук контурів   **************************************/
        public CutCTR FindBlobMini(Image<Bgr, byte> ImagAI, int ID)
        {
            Size FildOfWive = new Size(ImagAI.Width, ImagAI.Height);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            //var _img = ImagAI.Convert<Gray, byte>().ThresholdBinary(new Gray(SAV.DT.ColourMin.R[ID]), new Gray(255));
            Mat hierarchy = new Mat();

            var _img = ~ImagAI.InRange(new Bgr(SAV.DT.ColourMin.B[ID], SAV.DT.ColourMin.G[ID], SAV.DT.ColourMin.R[ID]), new Bgr(SAV.DT.ColourMax.B[ID], SAV.DT.ColourMax.G[ID], SAV.DT.ColourMax.R[ID]));  //маска

            //var cont =CvInvoke.FindContourTree(~_img.Mat, contours, ChainApproxMethod.ChainApproxSimple);/* визначаються контури які не торкраються краю картинки*/
            if (false) { CvInvoke.FindContours( _img, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple); }
                  else { CvInvoke.FindContours(~_img, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple); }


            //LiveViewTv.Image = _img.ToBitmap();

            CutImages CutImage = new CutImages();
         // Image<Bgr, byte> ImageAN = new Image<Bgr, byte>(ImagAI.Width, ImagAI.Height);
            Rectangle boxROI;

            //ВИЗНАЧИТИ ЧИ ПРОХОДИТЬ ЗНАЙДЕНИЙ КОНТУР ПО РОЗМІРУ
            int CnSize = 0;

            for (int i = 0; i < contours.Size; i++)
            {
                double CnturSize = CvInvoke.ContourArea(contours[i]);
                //    ВИЗНАЧИТИ ЧИ ПРОХОДИТЬ ЗНАЙДЕНИЙ КОНТУР ПО РОЗМІРУ
                if (((CnturSize >= SAV.DT.Analys.ConturMin[ID]) &&
                     (CnturSize < SAV.DT.Analys.ConturMax[ID])))
                { CnSize++; }
            }


            CutCTR cutCTR = new CutCTR();
            cutCTR.ROI = new Rectangle[CnSize];
            cutCTR.Aria = new double[CnSize];
            cutCTR.CUT = new bool[CnSize];
            cutCTR.NULL = new bool[CnSize];
            CnSize = 0;

            //  ***********ВИЗЧИТИ ЗАДВОЄННЯ ТА ЗАХВАТ КОНТОРУ  *************************//
            for (CountCNT = 0; CountCNT < contours.Size; CountCNT++)
            {
                double CnturSize = CvInvoke.ContourArea(contours[CountCNT]);

                //    ВИЗНАЧИТИ ЧИ ПРОХОДИТЬ ЗНАЙДЕНИЙ КОНТУР ПО РОЗМІРУ
                if (((CnturSize >= SAV.DT.Analys.ConturMin[ID]) && (CnturSize < SAV.DT.Analys.ConturMax[ID])))
                {
                    boxROI = CvInvoke.BoundingRectangle(contours[CountCNT]);
            

                    ///-----------ДОБАВИТИ ЗНАЙДЕНИЙ КОНТУР ДЛЯ АНАЛІЗУ---------------//
         
                            // CvInvoke.DrawContours(ImageAN, contours, CountCNT, new MCvScalar(50, 255, 50), 1, LineType.FourConnected);
                            if (!(Treker.Contains(boxROI.X))) { TrekerRW.Add(boxROI.X); }
                            ImagAI.ROI = boxROI;
                            cutCTR.ROI[CnSize] = boxROI;
                            cutCTR.NULL[CnSize] = false;
                            cutCTR.CUT[CnSize] = true;
                            cutCTR.Aria[CnSize++] = CnturSize;

                }
            }

            Treker = new HashSet<int>(TrekerRW);
            TrekerRW.Clear();

            return cutCTR;
        }




        public class CutCTR {
            public Rectangle[] ROI { get; set; }
            public double[] Aria { get; set; }
            public bool[] CUT { get; set; }
            public bool[] NULL { get; set; }
        };





















        public enum IMG_Test_Viwe_SET { No, Save, Simulation }



   
        // public static ConcurrentQueue<Img> Box = new ConcurrentQueue<Img>();
        public static IMG_Test_Viwe_SET IMG_Test_Viwe = IMG_Test_Viwe_SET.No;
        public static bool SelectDoubl = false;
        public static bool SelectionSample = false;
        public const int MaxImageSave = 1000; //максимальна кількість фоток  яку можна додавати в "List" та зберігати та відтворювати в симуляторі
        public static bool Setings = false;  //признак симуляції 
        public static int[] Count_Contur = new int[2];
        private static CutImages[] ListCut = new CutImages[2];
        private static Trace[] Trace = new Trace[2];
        private const int Master = 0;
        private const int Slave = 1;
        private static Image<Bgr, byte>[] ImagContact = new Image<Bgr, byte>[2];

        public static int[] CountContact = new int[2] { 0, 0 };










        public void resetValue()
        {
            ImagContact[0] = null;
            ImagContact[1] = null;
            CountContact[0] = 0;
            CountContact[1] = 0;

            EMGU.ListMast.Clear();
            EMGU.ListSlav.Clear();
        }












        static int[] OUTPUT_BIT = new int[3];
        static int[] OUTPUT_BIT2 = new int[3];   //зроблено для спрацювання трьох електоро тяг (коли боб падає між двома лопатками)
        //static Image<Bgr, byte>[] img_Camera = new Image<Bgr, byte>[2];
        public static double [] ContourSize= new double[2];
        public static bool ShowContourErors = false;






    }



  public  class AnalisReadData { 
   


   public int BuferIMG_DTCT_Count()
      {



        return FlowCamera.BuferIMG.Count;

       }
    
    





     private class InnerClass{
    
      static  private int _value;

        public int Value
        {
            get => _value;
            set => _value = value;
        }
    }

    public void SetMLtime(int value)
    {
        var innerClass = new InnerClass();
            if (value != 0)
            {
                innerClass.Value = value;
            }
    }


    public int GetMLtim()
    {
        var innerClass = new InnerClass();
        return innerClass.Value;
    }

    }


    class AnalisPredict {


        static int[] OUTPUT_BIT = new int[3];
        static int[] OUTPUT_BIT2 = new int[3];   //зроблено для спрацювання трьох електоро тяг (коли боб падає між двома лопатками)
                                                 //static Image<Bgr, byte>[] img_Camera = new Image<Bgr, byte>[2];



        public static double[] ContourSize = new double[2];
        public static bool ShowContourErors = false;

        public static bool TestFlaps = false;
        public static bool FreeRun;

        private static CutImages[] ListCut = new CutImages[2];
        public delegate void Mosaics(DTLimg[] dTLimg);
        static public event Mosaics MosaicaEvent;

        public  ML_Net ML_NetM = new ML_Net();
        public  ML_Nets ML_NetS = new ML_Nets();

        public ML_Net.PDT PDTm = new ML_Net.PDT();
        public ML_Net.PDT PDTs = new ML_Net.PDT();

        public  ML_Net.PDT FlowSlave(Mat x) { return ML_NetM.PredictImage(x); }
        public  ML_Net.PDT FlowMaster(Mat x) { return ML_NetM.PredictImage(x); }

        public  ML_Net.PDT[] Predict(Image<Bgr, byte>[] M, Image<Bgr, byte>[] S) { return ML_NetM.PredictImage(M, S); }


        public void InstML()
        {
            try
            {
                ML_NetM.Main();
                //ML_NetS.Main();

                ML_NetM.PredictImage("Load.JPG");
            }
            catch { Help.ErrorMesag("Failed to load the model"); }
        }

        private int TimPredictML;


        void SetTimML(int dt) {

            TimPredictML = TimPredictML = dt;
        }


         int ReadIng = 0;
        AnalisReadData analisReadData = new AnalisReadData();

        public bool FindClass(){

                  


            InstML();
             EMGU EMGU = new EMGU();
            while (Flow.PotocStartPredict)
            {

                if ((FlowCamera.BuferIMG.Count >= 1) ){
                

                    FlowCamera.CutImg ListDT = new FlowCamera.CutImg();
                    int CountBF = FlowCamera.BuferIMG.Count;

                    IMG_Cut IMG_CUT = new IMG_Cut();
                    IMG_CUT.ImgMaster = new Image<Bgr, byte>[CountBF];
                    IMG_CUT.ImgSlave = new Image<Bgr, byte>[CountBF];
                    IMG_CUT.ROI_Master = new Rectangle[CountBF];
                    IMG_CUT.ROI_Slave  = new Rectangle[CountBF];
                    IMG_CUT.AriaM = new double[CountBF];
                    IMG_CUT.AriaS = new double[CountBF];
                    IMG_CUT.ElongM = new Elongated[CountBF];
                    IMG_CUT.ElongS = new Elongated[CountBF];

                    ReadIng = 0;

                    //------  ЧИТАТИ БУФЕР   ------///
                    for (int i = 0; i < CountBF; i++){

                        FlowCamera.BuferIMG.TryDequeue(out ListDT);

                        if (ListDT != null)
                        {
                            ReadIng++;
                            IMG_CUT.ImgMaster[i]  = ListDT.Img[0].ToImage<Bgr, byte>();
                            IMG_CUT.ImgSlave[i]   = ListDT.Img[1].ToImage<Bgr, byte>();
                            IMG_CUT.ROI_Master[i] = ListDT.ROI[0];
                            IMG_CUT.ROI_Slave[i]  = ListDT.ROI[1];
                            IMG_CUT.AriaM[i] = ListDT.Aria[0];
                            IMG_CUT.AriaS[i] = ListDT.Aria[1];
                            IMG_CUT.ElongM[i] = ListDT.Elong[0];
                            IMG_CUT.ElongS[i] = ListDT.Elong[1];
                        }
                        else { break; };

                    }



                    /////////////////////////////////////////////////////////////////////////
                    ///    ------------------------Analysis-------------------------------------- -//
                    ////////////////////////////////////////////////////////////////////////
                    //***********************************************************************//




                    if (ReadIng != 0)
                    {
                        CountBF = ReadIng;

                      //   ******************Визначити вид****************************//
                        Stopwatch WatchML;
                        WatchML = Stopwatch.StartNew();

                        DTLimg[] DTLimg = new DTLimg[CountBF];
                        for (int i = 0; i < CountBF; i++)
                        {
                            DTLimg[i] = new DTLimg();
                            DTLimg[i].Value = new float[2][];

                          //  ---наповнюєм-- -
                           DTLimg[i].Img[0]  = IMG_CUT.ImgMaster[i];
                           DTLimg[i].Img[1]  = IMG_CUT.ImgSlave[i];
                           DTLimg[i].Aria[0] = Large_mm(Math.Max(IMG_CUT.ElongM[i].Width, IMG_CUT.ElongM[i].Height));
                           DTLimg[i].Aria[1] = Large_mm(Math.Max(IMG_CUT.ElongS[i].Width, IMG_CUT.ElongS[i].Height));
                        }


                        if (!FreeRun)
                        {

                           //-----------***************  PREDICT ************------------------

                            var PDT = Predict(IMG_CUT.ImgMaster, IMG_CUT.ImgSlave);

                           // -----------*************** PREDICT ************------------------

                            for (int i = 0; i < CountBF; i++)
                            {

                               // ---наповнюєм-- -

                                DTLimg[i].Name[0] = PDT[i].Nema;
                                DTLimg[i].Value[0] = new float[PDT[i].Value.Length];
                                DTLimg[i].Value[0] = PDT[i].Value;
                                DTLimg[i].IdxName[0] = PDT[i].ID;

                              //  для слідуючого індикса добавляєм зміщення CountBF
                                DTLimg[i].Name[1] = PDT[i + CountBF].Nema; ;
                                DTLimg[i].Value[1] = new float[PDT[i + CountBF].Value.Length];
                                DTLimg[i].Value[1] = PDT[i + CountBF].Value;
                                DTLimg[i].IdxName[1] = PDT[i + CountBF].ID; ;

                                WatchML.Stop();
                                var elapsedMs = WatchML.ElapsedMilliseconds;
                                analisReadData.SetMLtime(((int)elapsedMs));
                                //Debug.WriteLine("Predict:--" + "PCS-" + CountBF.ToString() + "Time-" + elapsedMs.ToString() + " ms"); ;


                                if ((DTLimg[i].Name[0] != GridData.GOOD)||(TestFlaps))

                                {
                                    OUTPUT_BIT = EMGU.SeparationChenal( IMG_CUT.ROI_Master[i].X, IMG_CUT.ROI_Master[i].Width);
                                    USB_HID.FLAPS.SET((USB_HID.FLAPS.Select)OUTPUT_BIT[0]);
                                    USB_HID.FLAPS.SET((USB_HID.FLAPS.Select)OUTPUT_BIT[1]);
                                    USB_HID.FLAPS.SET((USB_HID.FLAPS.Select)OUTPUT_BIT[2]);


                                    //OUTPUT_BIT2 = EMGU.SeparationChenal(1, ListCut[1].ROI[i].X, ListCut[1].ROI[i].Width, false);
                                    //USB_HID.FLAPS.SET((USB_HID.FLAPS.Select)OUTPUT_BIT[0]);
                                    //USB_HID.FLAPS.SET((USB_HID.FLAPS.Select)OUTPUT_BIT[1]);
                                }
                                else
                                {
                                    if (DTLimg[i].Name[1] != GridData.GOOD)
                                    {
                                        OUTPUT_BIT = EMGU.SeparationChenal( IMG_CUT.ROI_Master[i].X, IMG_CUT.ROI_Master[i].Width);
                                        USB_HID.FLAPS.SET((USB_HID.FLAPS.Select)OUTPUT_BIT[0]);
                                        USB_HID.FLAPS.SET((USB_HID.FLAPS.Select)OUTPUT_BIT[1]);
                                        USB_HID.FLAPS.SET((USB_HID.FLAPS.Select)OUTPUT_BIT[2]);


                                        //OUTPUT_BIT2 = EMGU.SeparationChenal(1, ListCut[1].ROI[i].X, ListCut[1].ROI[i].Width, false);
                                        //USB_HID.FLAPS.SET((USB_HID.FLAPS.Select)OUTPUT_BIT[0]);
                                       // USB_HID.FLAPS.SET((USB_HID.FLAPS.Select)OUTPUT_BIT[1]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //для тестування флепсів
                            for (int i = 0; i < CountBF; i++)
                            {

                                DTLimg[i].Name[0] = GridData.GOOD;
                                DTLimg[i].Value[0] = new float[1];
                                DTLimg[i].Value[0][0] = 1;

                                DTLimg[i].Name[1] = GridData.GOOD;
                                DTLimg[i].Value[1] = new float[1];
                                DTLimg[i].Value[1][0] = 1;
                            }
                        }
                        //************Mosaica ADD****************//
                           MosaicaEvent(DTLimg);
                        //--------------------------------------------//
                    }




                    if (SAV.DT.Device.OutputDelay != 0) { USB_HID.FLAPS.APPLY(); } 


                    ListCut[1] = null;
                    ListCut[1] = new CutImages();

                    ListCut[0] = null;
                    ListCut[0] = new CutImages();

                }

                Thread.Sleep(1);
            }
            ////////////////
            Thread.Sleep(1);
            return true;
        }


          double widthInPixels = 8192; // ширина поля в пікселях
           double widthInCm = 200; // ширина поля в см
         double Areamm2(double Aria)
        {

           double  data1=  ( widthInCm/ widthInPixels );
            double data3  = Aria * data1;


            // Знайдемо площу в мм²
           // double    data=Aria * (widthInPixels / widthInCm) ;      // 1 см = 10 мм
            
       
            return data3;
        }


        double Large_mm(double Aria)
        {

            double data1 = (widthInCm / widthInPixels);
            double data3 = Aria * data1;

            // Знайдемо площу в мм²
            // double    data=Aria * (widthInPixels / widthInCm) ;      // 1 см = 10 мм

            return Math.Round(data3, 2); ;
        }



    }





    class FlowAnalisTST
    {



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //==============================                                 ВИЗНАЧАЄМ ТА МАЛЮЄМО КОНТУРИ з FOTO                                                      ==============================//
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public delegate void Mosaics(DTLimg [] dTLimg);
        static public event Mosaics MosaicaEvent;

        private const int Master = 0;
        private const int Slave = 1;

        public static ML_Net ML_NET = new ML_Net();

      static  public void InstML()
        {
            try
            {
                ML_NET.Main();
            }
            catch { Help.ErrorMesag("Failed to load the model"); }
        }


        /// find kik  100x100 IMG  ////
      static  public bool FindClassTestImg(Image<Bgr, byte>[] Imag1, int ID)
        {



            //наповнюєм наними 
            DTLimg []DTLimgs = new DTLimg[1];
            DTLimgs[0] = new DTLimg();
            DTLimgs[0].Value = new float[2][];
    

            DTLimgs[0].Img[0] = Imag1[Master];
            DTLimgs[0].Img[1] = Imag1[Slave];



            if (MLD.Names != null)
            {


                //******************  Slave  ****************************//
                    var PDTm = ML_NET.PredictImage(Imag1[Master].Mat);

                    DTLimgs[0].Name[0] = PDTm.Nema;
                    DTLimgs[0].Value[0] = new float[PDTm.Value.Length];
                    DTLimgs[0].Value[0] = PDTm.Value;

                    DTLimgs[0].Name[1] = PDTm.Nema;
                    DTLimgs[0].Value[1] = new float[PDTm.Value.Length];
                    DTLimgs[0].Value[1] = PDTm.Value;
          


            }

            MosaicaEvent(DTLimgs);





            return true;
        }







    












































    }


}
