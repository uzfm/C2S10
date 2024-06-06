
using Emgu.CV;
using Emgu.CV.CvEnum;
//using Emgu.CV.Shape;
//using Emgu.Util;
//using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
//using DirectShowLib;
//using System.Threading;
//using System.Linq;
//using Emgu.CV.ImgHash;
//using Emgu.Util.TypeEnum;
//using System.Collections.Concurrent;
//using Emgu.CV.Stitching;
//using Emgu.CV.Cuda;
//using System.IO;
//using System.Drawing.Imaging;
//using System.Diagnostics;

//using Emgu.CV.Dnn;
//using Emgu.CV.UI;
//using Emgu.CV.Util;
//using Emgu.CV.CvEnum;





namespace MVision
{


    class EMGU{


        static public List<Bitmap> ListMast = new List<Bitmap>();
        static public List<Bitmap> ListSlav = new List<Bitmap>();
        static public ImageList[] MosaicsAnalis = new ImageList[2];



        public Bitmap ToHSV(Bitmap img) {

            Mat imgHSV = new Mat(100, 100, DepthType.Cv16U, 1);
            CvInvoke.CvtColor(img.ToImage<Rgb, byte>(), imgHSV, ColorConversion.Rgb2Hsv);
            return imgHSV.ToBitmap();
        }
        public static void InstMosaics()
        {

            MosaicsAnalis[Master] = new ImageList();
            MosaicsAnalis[Slave] = new ImageList();

        }


        public delegate void Mosaics(int ID, Bitmap Imag);
        //static public event Mosaics Mosaica;
        static public bool[,] AnalysisMode = new bool[2, 5] { { false, false, false, false, false }, { false, false, false, false, false } };
        public const int Master = 0;
        public const int Slave = 1;
        VideoCapture capture = null;

        //============================== подія захвата кадра ====================================//
        private void Capture_ImageGrabbed(Mat mat1)
        {
            Mat mat = new Mat();
            capture.Retrieve(mat);
            Image<Rgb, byte> test = mat.ToImage<Rgb, byte>().Flip(FlipType.Horizontal);

            FindContur(test);
        }
        //======================================================================================//


        //==============================ВИЗНАЧАЄМ ТА МАЛЮЄМО КОНТУРИ з відео потоку ==============================//
        public Bitmap FindContur(Image<Rgb, byte> img)
        {
            //переводим в чорно білу картинку з певним затемненням
            Image<Gray, byte> btmaptest = img.Convert<Gray, byte>().ThresholdBinary(new Gray(100), new Gray(255));
            //створюємо пустий контур
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarhy = new Mat();
            //шукаємо контури
            CvInvoke.FindContours(btmaptest, contours, hierarhy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
            // малюємо знайдені контури
            CvInvoke.DrawContours(img, contours, -1, new MCvScalar(10000, 1, 2), 1, LineType.FourConnected);
            //вивисти картинку на екран

            return img.ToBitmap();
            //  panAndZoomPictureBox1.Image = img.AsBitmap();
        }
        //========================================================================================================//




        //============================== Загрузити Image для подальшоъ обробки ==============================//
        static public Image<Bgr, byte>[] Origenal_img_Camera = new Image<Bgr, byte>[2];
        static public Image<Bgr, byte>[] Origenal_Img_Mosaics = new Image<Bgr, byte>[2];

        public static int[] SetingsID = new int[2];
        public static bool[] SelectConturs = new bool[2] { false, false }; // дозволяє виділити глоби




        ///static public Image<Bgr, byte> Origenal_img;
        public Bitmap Origenal_Image(int ID) {  return Origenal_img_Camera[ID].Mat.ToBitmap();  }
        public void Load_Image(Bitmap img, int ID) { Origenal_img_Camera[ID] = img.ToImage<Bgr, byte>(); }  // (new ToBitmap(img)); 
        public void Load_Image_Mosaics(Bitmap img, int ID) { Origenal_Img_Mosaics[ID] = img.ToImage<Bgr, byte>(); }






        //***********************************************************************//
        //***********************************************************************//
        //************ ОСНОВНЕ НАЛАШТУВАННЯ КОЛІРУ            *******************//
        //***********************************************************************//
        public static bool[] OrigenalFotoIDX = new bool[2];
        public Bitmap FiltreColour(int ID)
        {
            if (OrigenalFotoIDX[ID] == false)
            {
                //ВИБРАТИ ФОТО Origenal_img_Camera[]
                if (Origenal_img_Camera[ID] != null){
                    //++ налаштування маски begraund
                    Image<Bgr, byte> BegraundImg;
                    BegraundImg = new Image<Bgr, byte>(Origenal_img_Camera[ID].Data);
                    Image<Gray, Byte> GrayImg = BegraundImg.Convert<Gray, Byte>();
                    GrayImg = BegraundImg.InRange(new Bgr(SAV.DT.ColourMin.B[ID], SAV.DT.ColourMin.G[ID], SAV.DT.ColourMin.R[ID]),
                    new Bgr(SAV.DT.ColourMax.B[ID], SAV.DT.ColourMax.G[ID], SAV.DT.ColourMax.R[ID]));
                    return GrayImg.Mat.ToBitmap();
                }
            }
            else
            {

                // ВИБИРАЄМ ФОТО З МОЗАЇКИ
                if (Origenal_Img_Mosaics[ID] != null)
                {
                    Image<Gray, Byte> GrayImg = Origenal_Img_Mosaics[ID].Convert<Gray, Byte>();
                    GrayImg = Origenal_Img_Mosaics[ID].InRange(new Bgr(SAV.DT.ColourMin.B[ID], SAV.DT.ColourMin.G[ID], SAV.DT.ColourMin.R[ID]),
                                                               new Bgr(SAV.DT.ColourMax.B[ID], SAV.DT.ColourMax.G[ID], SAV.DT.ColourMax.R[ID]));


                    return GrayImg.Mat.ToBitmap();
                }
            }




            return null;


        }
        //***********************************************************************//
        public Bitmap ResivColBackgMin(int ID)
        {
            if (Origenal_img_Camera[ID] != null)
            {
                Image<Bgr, byte> Background = new Image<Bgr, byte>(100, 100, new Bgr(SAV.DT.ColourMin.B[ID], SAV.DT.ColourMin.G[ID], SAV.DT.ColourMin.R[ID]));
                return Background.Mat.ToBitmap();
            }
            return null;
        }
        public Bitmap ResivColBackgMax(int ID)
        {
            if (Origenal_img_Camera[ID] != null)
            {
                Image<Bgr, byte> Background = new Image<Bgr, byte>(100, 100, new Bgr(SAV.DT.ColourMax.B[ID], SAV.DT.ColourMax.G[ID], SAV.DT.ColourMax.R[ID]));
                return Background.Mat.ToBitmap();
            }
            return null;
        }




        // Склеює дві картинки
        public Bitmap ContactImags(Bitmap ImagM, Bitmap ImagS) {
            Mat NewMat = new Mat();
            Image<Bgr, byte> imgM = ImagM.ToImage<Bgr, byte>();
            Image<Bgr, byte> imgS = ImagS.ToImage<Bgr, byte>();
            CvInvoke.HConcat(imgS, imgM, NewMat);
            return NewMat.ToBitmap();
        }

        // склеює дві картинки і додає білий контур
        public Bitmap ContactImagsWithContur(Bitmap ImagM, Bitmap ImagS)
        {
            Mat NewMat = new Mat();
            Image<Bgr, byte> imgM = ImagM.ToImage<Bgr, byte>();
            Image<Bgr, byte> imgS = ImagS.ToImage<Bgr, byte>();

            // Склеювання зображень
            CvInvoke.HConcat(imgS, imgM, NewMat);

            // Обведення контуру
            Rectangle ROI = NewMat.ToImage<Bgr, byte>().ROI;
            ROI.Height = ROI.Size.Height - 1;
            ROI.Width = ROI.Size.Width - 1;

            CvInvoke.Rectangle(NewMat, ROI, new Bgr(Color.LightBlue).MCvScalar, 1);

            return NewMat.ToBitmap();
        }

  
        //========================================================================================//
        /// <summary>






        public Bitmap SetingsShiftingDetect(int Shiftin)
        {

            Rectangle Rect = new Rectangle(0, 0, 0, 0);

            MCvScalar drawingColor = new Bgr(Color.Red).MCvScalar;

            Image<Bgr, byte> img = new Image<Bgr, byte>(FlowAnalis.SizeApertur.Width, FlowAnalis.SizeApertur.Height);

            if (Origenal_img_Camera[0] != null)
            {
                img = new Image<Bgr, byte>(Origenal_img_Camera[0].Data);
            }

            Rect = new Rectangle(FlowAnalis.SizeApertur.Height*2, 1 ,Shiftin, FlowAnalis.SizeApertur.Height);
            CvInvoke.Rectangle(img, Rect, drawingColor, 5);

            return img.ToBitmap();
        }

        //Green img
        public Bitmap bgrImgGreen(Bitmap img)
        {

            Image<Bgr, Byte> img1 = new Image<Bgr, Byte>(img.Width, img.Height, new Bgr(0, 255, 0));

            return img1.ToBitmap();
        }

        //Визначення кaналу для відправки на контролер
        const int ShoulderPic = 20;
        static public int[] SeparationChenal( int Position, int Length) {
        int[] Output = new int[3];

            int Shoulder = ((/*SAV.DT.Analys.AperWidth*/ FlowAnalis.SizeApertur.Width) / ShoulderPic);
            int X = SAV.DT.Analys.DoubleFlaps;

            Output[0] = ((Position +  (Length / 2) + ((Shoulder/100)* SAV.DT.Analys.DoubleFlaps)) ) / (Shoulder);
            Output[1] = (Position +   (Length / 2) ) / (Shoulder);
            Output[2] = ((Position +  (Length / 2) - ((Shoulder / 100) * SAV.DT.Analys.DoubleFlaps)) ) / (Shoulder);

            return Output;
        }






        public static int[] Count_Contur = new int[2];
        static Image<Bgr, byte>[] ImagContact = new Image<Bgr, byte>[2];


        static int[,,] LocationImg_1 = new int[2, 20, 3]; // ID ,H,W
        static int[,,] LocationImg_2 = new int[2, 20, 3]; // ID ,H,W
        static byte[,] LocationImgIDX = new byte[2, 2];

        public static bool StartAnalis = false;    // синхронізація і початок аналізу

        private static Image<Bgr, byte>[] imgROI = new Image<Bgr, byte>[2];      //вирізання img
                                                                                 //  private static int             [] imgCount = new int[2] { 0, 0 };        //підраховуємо кадри для синхронізації
        private static int[] PeletsCount = new int[2] { 0, 0 };     //підраховуємо частинок для синхронізації
                                                                    //  static int                     [] ContName = new int[2];

        static public int   CameraBufer = 0;
        static public int   CameraEroor = 0;













        #region BackgroundSetings

        public Image<Bgr, byte> WriteTextINimg(Image<Bgr, byte> img, string Text, Rectangle ROI, Color colour)
        {

            //   public Rectangle[] ROI { get; set; }


            // Write information next to marked object створити точку в центрі для запису інформації
            Point center = new Point(ROI.X + ROI.Width / 2, ROI.Y + ROI.Height / 2);


            //створюєм інформацію для запису
            var info = new string[] { Text, $"Position: {center.X}, {center.Y}" };
            // for (int i = 0; i < lines.Length; i++)
            // {
            //  int y = i * 10 + origin.Y; // Moving down on each line

            // створюємо колір
            MCvScalar drawingColor = new Bgr(colour).MCvScalar;

            //записуєм строчку на вирізаний малюнок
            CvInvoke.PutText(img, info[0], center, FontFace.HersheyPlain, 2, drawingColor, 1);
            // CvInvoke.PutText(img, "?????", new System.Drawing.Point(50, 50), FontFace.HersheyComplex, 1.0, new Bgr(0, 255, 0).MCvScalar);
            //вивисти картинку на екран
            // Mosaica(ID, imgROI[ID].Bitmap);




            return img;
        }

        //================================================== ВИРІВНЮВАННЯ ФОНУ ================================================================================================//

        static public bGMSK BGMSK = new bGMSK(); //DATA


        [Serializable()]
        public class bGMSK
        {
            //  МАСКА ФОНУ
            public Image<Bgr, byte>[] BackgroundLevel      = new Image<Bgr, byte>[2];
           // public Image<Bgr, byte>[] BackgroundLevelRLTim = new Image<Bgr, byte>[2];

            //для виривнюваня фону
            public  int[]  Darken = new int[2] { 0, 0 };
            public  int[]  Lighter = new int[2] { 0, 0 };
            public bool[]  OriginImage = new bool[2] { false, false };
        }    // Class ДЛЯ СЕРЕЛІЗАЦІЇ



        //ЗАГРУЗИТИ МАСКУ ФОНУ В БУФЕР
        public bool BackgroundLoadImag(int ID)
        {

            if (Origenal_img_Camera[ID] != null)
            {
                BGMSK.BackgroundLevel[ID] = new Image<Bgr, byte>(Origenal_img_Camera[ID].Data);
                BGMSK.BackgroundLevel[ID] = Origenal_img_Camera[ID];

               // BGMSK.BackgroundLevelRLTim[ID] = new Image<Bgr, byte>(Origenal_img_Camera[ID].Data);
               // BGMSK.BackgroundLevelRLTim[ID] = BGMSK.BackgroundLevel[ID];
            }
            else { return false; }

            return true;
        }

        //ОЧИСТИТИ БУФЕР МАСКИ
        public bool BackgroundClean(int ID){
         
            if (/*(BGMSK.BackgroundLevelRLTim == null)||*/(BGMSK.BackgroundLevel==null)) {
                // BGMSK.BackgroundLevelRLTim = new Image<Bgr, byte>[2];
                   BGMSK.BackgroundLevel      = new Image<Bgr, byte>[2];
            }

            if (BGMSK.BackgroundLevel[ID] != null){
                   BGMSK.BackgroundLevel[ID] = null;
                 //BGMSK.BackgroundLevelRLTim[ID] = null;
                return true;
            }

            return false;
        }


        //ВИРІВНЮВАННЯ Background І ПОВЕРНУТИ (Bitmap)
        public Bitmap BackgroundApply(int ID, out bool Eroor)
        {

            if (BGMSK.BackgroundLevel[ID] != null)
            {
                if (Origenal_img_Camera[ID] == null) { Eroor = false; return null; }
                Image<Bgr, byte> BackgroundLevel2;
                Image<Bgr, byte> Mask = new Image<Bgr, byte>(BGMSK.BackgroundLevel[ID].Size.Width, BGMSK.BackgroundLevel[ID].Size.Height, new Bgr(BGMSK.Darken[ID], BGMSK.Darken[ID], BGMSK.Darken[ID]));
                Image<Bgr, byte> Mask_2 = new Image<Bgr, byte>(BGMSK.BackgroundLevel[ID].Size.Width, BGMSK.BackgroundLevel[ID].Size.Height, new Bgr(BGMSK.Lighter[ID], BGMSK.Lighter[ID], BGMSK.Lighter[ID]));

                BackgroundLevel2 = BGMSK.BackgroundLevel[ID] - Mask;
                Eroor = false;

                if ((Origenal_img_Camera[ID].Height != Mask_2.Height)) { Eroor = true; return null; }

                return ((Origenal_img_Camera[ID] - BackgroundLevel2) + Mask_2).ToBitmap();
            }
            Eroor = true;
            return null;
        }


        //ВИРІВНЯТИ Background І ПОВЕРНУТИ  (Image)
        //public Image<Bgr, byte> BackgroundApply(Image<Bgr, byte> Img, int ID, out bool Eroor)
        //{

        //    if (BGMSK.BackgroundLevel[ID] != null)
        //    {
        //        Image<Bgr, byte> BackgroundLevel2;
        //        Image<Bgr, byte> Mask = new Image<Bgr, byte>(BGMSK.BackgroundLevel[ID].Size.Width, BGMSK.BackgroundLevel[ID].Size.Height, new Bgr(BGMSK.Darken[ID], BGMSK.Darken[ID], BGMSK.Darken[ID]));
        //        Image<Bgr, byte> Mask_2 = new Image<Bgr, byte>(BGMSK.BackgroundLevel[ID].Size.Width, BGMSK.BackgroundLevel[ID].Size.Height, new Bgr(BGMSK.Lighter[ID], BGMSK.Lighter[ID], BGMSK.Lighter[ID]));
        //        BackgroundLevel2 = BGMSK.BackgroundLevel[ID] - Mask;
        //        if (Img.Height != Mask_2.Height) { Eroor = false; return null; }
        //        Eroor = true;
        //        return ((Img - BackgroundLevel2) + Mask_2);
        //    }
        //    Eroor = false;
        //    return null;
        //}


   static     public Image<Bgr, byte> BackgroundApply (Image<Bgr, byte> Img, int ID, out bool Eroor)
        {

            if (BGMSK.BackgroundLevel[ID] != null)
            {

                Image<Bgr, byte> BackgroundLevel2;
                Image<Bgr, byte> Mask = new Image<Bgr, byte>(BGMSK.BackgroundLevel[ID].Size.Width, Img.Size.Height, new Bgr(BGMSK.Darken[ID], BGMSK.Darken[ID], BGMSK.Darken[ID]));
                Image<Bgr, byte> Mask_2 = new Image<Bgr, byte>(BGMSK.BackgroundLevel[ID].Size.Width, Img.Size.Height, new Bgr(BGMSK.Lighter[ID], BGMSK.Lighter[ID], BGMSK.Lighter[ID]));
                Bitmap Imag = new Bitmap(BGMSK.BackgroundLevel[ID].ToBitmap());// fail 100x50pix  

                 Image<Bgr, byte> NewMat = Imag.ToImage<Bgr, byte>();
                 BackgroundLevel2 = NewMat - Mask;
                Eroor = true;
                return ((Img - BackgroundLevel2) + Mask_2);

            }
            Eroor = false;
            return null;
        }

        //ВИРІВНЯТИ Background З МОЗАЇКИ 100x100 Cline begraund (Image)
        public Image<Bgr, byte> BackgroundApplyMosaic(Image<Bgr, byte> Img, int ID, out bool Eroor)
        {

            if (BGMSK.BackgroundLevel[ID] != null)
            {
                Image<Bgr, byte> Imag = new Image<Bgr, byte>(100, 100);
                Imag = new Bitmap(BGMSK.BackgroundLevel[ID].ToBitmap(), 100, 100).ToImage<Bgr, byte>();// fail 100x50pix 
                Image<Bgr, byte> BackgroundLevel2;
                Image<Bgr, byte> Mask = new Image<Bgr, byte>(100, 100, new Bgr(BGMSK.Darken[ID], BGMSK.Darken[ID], BGMSK.Darken[ID]));
                Image<Bgr, byte> Mask_2 = new Image<Bgr, byte>(100, 100, new Bgr(BGMSK.Lighter[ID], BGMSK.Lighter[ID], BGMSK.Lighter[ID]));
                BackgroundLevel2 = Imag - Mask;
                Eroor = true;
                return ((Img - BackgroundLevel2) + Mask_2);
            }
            Eroor = false;
            return null;
        }

        public void ref_BGMSK(int ID, int DarkenValue, int LighterValue, bool OriginImage)
        {
            BGMSK.Darken[ID] = DarkenValue;
            BGMSK.Lighter[ID] = LighterValue;
            BGMSK.OriginImage[ID] = OriginImage;
        }
        public void ref_BGMSK(int ID, out int DarkenValue, out int LighterValue, out bool OriginImage)
        {
            DarkenValue = BGMSK.Darken[ID];
            LighterValue = BGMSK.Lighter[ID];
            OriginImage = BGMSK.OriginImage[ID];
        }

        #endregion BackgroundSetings
        //================================================== *************************** ================================================================================================//



        Image<Bgr, byte> Resolt;
        Mat NewMatM = new Mat();

        public Bitmap CleanBegraund(Image img, int ID)
        {

            Image<Bgr, Byte> Data = new Image<Bgr, byte>(img.Width, img.Height);
            Image<Bgr, Byte> Data2 = new Image<Bgr, byte>(img.Width, img.Height);

            Image<Bgr, byte> Datasf = new Image<Bgr, byte>(100, 100);

            Datasf = new Bitmap(img).ToImage<Bgr, byte>();

            bool Eroor = false;

            Data = BackgroundApplyMosaic(Datasf, ID, out Eroor);

            if (Eroor == false) { Data= new Bitmap(img, 100, 100).ToImage<Bgr, byte>(); }// fail 100x50pix
            Data2 = new Bitmap(img, 100, 100).ToImage<Bgr, byte>();
            /// byte[,,] data = Data.Data;

            //маскування певних кольорів білим//
            Image<Bgr, byte> Resolt = new Image<Bgr, byte>(img.Width, img.Size.Height, new Bgr(255, 255, 255));
            Image<Gray, byte> Maska = Data.Convert<Gray, byte>();

            Maska = Data.InRange(new Bgr(SAV.DT.ColourMin.B[ID], SAV.DT.ColourMin.G[ID], SAV.DT.ColourMin.R[ID]),
                                  new Bgr(SAV.DT.ColourMax.B[ID], SAV.DT.ColourMax.G[ID], SAV.DT.ColourMax.R[ID]));
            CvInvoke.BitwiseAnd(Data2, Data2, Resolt, ~Maska);
            // CvInvoke.Imshow("Clin Begraund", Resolt);
            // CvInvoke.Imshow("CleanBegraund", Resolt);
            return Resolt.ToBitmap();
        }

        public Image<Bgr, Byte> CleanBegraundROI(Image img, int ID)
        {

            Image<Bgr, Byte> Data = new Image<Bgr, byte>(img.Width, img.Height);
            Image<Bgr, Byte> Data2 = new Image<Bgr, byte>(img.Width, img.Height);


            Data = new Bitmap(img, 100, 100).ToImage<Bgr, byte>(); // fail 100x50pix
            Data2 = new Bitmap(img, 100, 100).ToImage<Bgr, byte>();
            /// byte[,,] data = Data.Data;

            //маскування певних кольорів білим//
            Resolt = new Image<Bgr, byte>(100, 100, new Bgr(255, 255, 255));
            Image<Gray, byte> Maska = Data.Convert<Gray, byte>();
          
            Maska = Data.InRange(new Bgr(   SAV.DT.ColourMin.B[ID], SAV.DT.ColourMin.G[ID], SAV.DT.ColourMin.R[ID]),
                                  new Bgr(SAV.DT.ColourMax.B[ID], SAV.DT.ColourMax.G[ID], SAV.DT.ColourMax.R[ID]));
            CvInvoke.BitwiseAnd(Data2, Data2, Resolt, ~Maska);
         

            //CvInvoke.Imshow("CleanBegraund", Resolt);
            return Resolt;
        }


        public  static Image<Bgr, byte> CLAHE(Image<Bgr, byte> image) {
           // Image<Bgr, byte> image = new Image<Bgr, byte>("your_image_path.jpg");
            Image<Lab, byte> labImage = new Image<Lab, byte>(image.Width, image.Height);
            CvInvoke.CvtColor(image, labImage, ColorConversion.Bgr2Lab);

            // Розділити канали Lab
            Image<Gray, byte>[] labChannels = labImage.Split();


            var R= labChannels[0].Size;
            var G = labChannels[1].Size;
            var B = labChannels[2].Size;

            // Застосувати глобальне вирівнювання освітлення до каналу L
            // CvInvoke.EqualizeHist(labChannels[0], labChannels[0]);

            // Застосувати CLAHE до каналів L
            CvInvoke.CLAHE(labChannels[0], clipLimit: 20, tileGridSize: new Size(2, 2), labChannels[0] as IInputOutputArray);


            // Об'єднати канали Lab
            VectorOfMat labChannelsVector = new VectorOfMat(); //labChannels.Length
            foreach (var channel in labChannels)
            {
                labChannelsVector.Push(channel.Mat);
            }

            // Об'єднати канали Lab
            Image<Lab, byte> processedLabImage = new Image<Lab, byte>(labImage.Width, labImage.Height);
            CvInvoke.Merge(labChannelsVector, processedLabImage);

            // Перетворити назад у формат Bgr
            Image<Bgr, byte> outputImage = new Image<Bgr, byte>(processedLabImage.Width, processedLabImage.Height);
            CvInvoke.CvtColor(processedLabImage, outputImage, ColorConversion.Lab2Bgr);
            return outputImage;
        }

        public static Image<Bgr, byte> ImageCLAHE(Image<Bgr, byte> image)
        {
            // Image<Bgr, byte> image = new Image<Bgr, byte>("your_image_path.jpg");
            Image<Lab, byte> labImage = new Image<Lab, byte>(image.Width, image.Height);
            CvInvoke.CvtColor(image, labImage, ColorConversion.Bgr2Lab);

            // Розділити канали Lab
            Image<Gray, byte>[] labChannels = labImage.Split();


            var R = labChannels[0].Size;
            var G = labChannels[1].Size;
            var B = labChannels[2].Size;

            // Застосувати глобальне вирівнювання освітлення до каналу L
            // CvInvoke.EqualizeHist(labChannels[0], labChannels[0]);

            // Застосувати CLAHE до каналів L
            CvInvoke.CLAHE(labChannels[0], clipLimit: 10, tileGridSize: new Size(2, 2), labChannels[0] as IInputOutputArray);


            // Об'єднати канали Lab
            VectorOfMat labChannelsVector = new VectorOfMat(); //labChannels.Length
            foreach (var channel in labChannels)
            {
                labChannelsVector.Push(channel.Mat);
            }

            // Об'єднати канали Lab
            Image<Lab, byte> processedLabImage = new Image<Lab, byte>(labImage.Width, labImage.Height);
            CvInvoke.Merge(labChannelsVector, processedLabImage);

            // Перетворити назад у формат Bgr
            Image<Bgr, byte> outputImage = new Image<Bgr, byte>(processedLabImage.Width, processedLabImage.Height);
            CvInvoke.CvtColor(processedLabImage, outputImage, ColorConversion.Lab2Bgr);
            return outputImage;
        }











        //void TestImgBlb()
        //{
        //    string PshData = Path.Combine(PachML.Text, "Data"); //створити шлях до каталога "Data"
        //    string PshSempls = Path.Combine(PshData, STGS.Data.ML_NAME, comboBoxImgTypTest.Text); //створити шлях до каталога "SAMPLES"


        //    try
        //    {
        //        string urlMaster = PshSempls + "\\" + "Image" + IdxShouTest++ + ".jpg";
        //        files = Directory.GetFiles(@PshSempls, "*.jpg");

        //        int count = files.Length;

        //        if (files != null)
        //        {
        //            if (IdxShouTest <= files.Length)
        //            {
        //                Bitmap imM = new Bitmap(files[IdxShouTest]);
        //                textBox3.Text = IdxShouTest.ToString();

        //                imOriginalM = imM.ToImage<Bgr, byte>().Resize(64, 64, interpolationType: Inter.Linear).Mat;

        //                Stopwatch watch = Stopwatch.StartNew();



        //                Image<Bgr, byte> ImagesViw = new Image<Bgr, byte>(100, 100);
        //                if (AnalysisTest.Checked)
        //                {
        //                    ImagesViw = vision.DetectBlob(imOriginalM, labelDectContur);
        //                }
        //                else
        //                {
        //                    ImagesViw = vision.DetectBlobBlack(imOriginalM, labelDectContur);
        //                }



        //                watch.Stop();
        //                var elapsedMs = watch.ElapsedMilliseconds;
        //                toolStripStatusLabel5.Text = elapsedMs.ToString();
        //                // Console.WriteLine("First Prediction took: " + elapsedMs + " ms");

        //                pictureBox1.Image = ImagesViw.ToBitmap();
        //                pictureBox2.Image = imOriginalM.ToBitmap();



        //            }
        //        }
        //    }
        //    catch { }
        //}










    }












    class IMG_Cut {

        public Image<Bgr, byte>[] ImgMaster;
        public Image<Bgr, byte>[] ImgSlave ;
        public Rectangle[] ROI_Master ;
        public Rectangle[] ROI_Slave;
        public double   [] AriaM ;
        public double   [] AriaS;
        public AnalisSMP[] AnalSMP_M;
        public AnalisSMP[] AnalSMP_S;

    }

    class CutImages
    {

        public Image <Bgr,byte>[] Img { get; set; }
        public int[] X { get; set; }
        public int[] Y { get; set; }


        public int[] Width { get; set; }
        public int[] Height { get; set; }


        public Rectangle[] ROI { get; set; }
        public bool DONE { get; set; }
        public byte Count { get; set; }

        public double [] ContuorSize { get; set; }

    }
    class Trace
    {
        public Rectangle[] ROI { get; set; }
        public int Frame { get; set; }
        public bool [] Found { get; set; }
    }
    class Img {
        public Bitmap[] Bmap { get; set; } = new Bitmap[2];

       
    }


    class DTLimg{
         public Image <Bgr,Byte>   []   Img       = new Image< Bgr, Byte> [2]; // зображення з двох сторін
         public     int      ID ;                          // індефікатор масива назва семпла по сторонам
         public String  []   Name      = new string[2];    // назва семпла по сторонам
         public float   [][] Value ;                       // детальна значеня по усіх назвах сеплів для двох сторін
         public int          IdxOut;                       // ідекс розташування в таблиці
         public int       [] IdxName = new int [2] ;                       // ідекс розташування в таблиці
        
        public double[] SizeCorect = new double[2];
       // public double[] Aria = new double[2];    //
        public AnalisSMP[] AnalisSMP = new AnalisSMP[2];    //

        static public List<int> SelectITM = new List<int>(); // вибираєм семпли та заносим в даний ліст для подальшої обробки.
    };

    public  class FPN {


    static  public Image<Bgr, byte> Set(Image<Bgr, byte> image){
                    Stopwatch watch;
            watch = Stopwatch.StartNew();

            // Отримання розміру зображення та розрахунок відстані від кожного пікселя до центру
            int centerX = image.Width / 2;
            int centerY = (image.Height / 2);
            int maxDistance = Math.Max(centerX, centerY);
            //int maxDistance = centerX;
            double brightnessFactor = 1; // Коефіцієнт збільшення яскравості (змінюйте його для досягнення потрібного ефекту)

            // Обробка пікселів зображення паралельно
            Parallel.For(0, image.Height, y => {
                Parallel.For(0, image.Width, x => {
                    // Обчислення відстані до центру
                    int distanceX = Math.Abs(x - centerX);
                    //int distanceY = Math.Abs(y - centerY);

                    // Обчислення коефіцієнта збільшення яскравості на основі відстані до центру
                    double brightnessMultiplier = 1.2;
                    brightnessMultiplier += (distanceX / (double)maxDistance) * brightnessFactor;
                    //brightnessMultiplier += (distanceY / (double)maxDistance) * brightnessFactor;

                    // Збільшення яскравості пікселя
                    Bgr pixel = image[y, x];
                    pixel.Blue = (byte)Math.Min(pixel.Blue * brightnessMultiplier, 255);
                    pixel.Green = (byte)Math.Min(pixel.Green * brightnessMultiplier, 255);
                    pixel.Red = (byte)Math.Min(pixel.Red * brightnessMultiplier, 255);
                    image[y, x] = pixel;

                });
            });


            watch.Stop();
            Debug.WriteLine("FPN: -- " + watch.ElapsedMilliseconds + " ms");
            return image;
        }


        static public Bitmap Test(string imagePath) {
            Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath);




    


            // Отримання розміру зображення та розрахунок відстані від кожного пікселя до центру
            // Визначення відступу від центру

            Stopwatch watch;
            watch = Stopwatch.StartNew();
            // Отримання розміру зображення та розрахунок відстані від кожного пікселя до центру
            int centerX = image.Width / 2;
            int centerY = (image.Height / 2) ;
            int maxDistance = Math.Max(centerX, centerY);
            //int maxDistance = centerX;
            double brightnessFactor = 1; // Коефіцієнт збільшення яскравості (змінюйте його для досягнення потрібного ефекту)


            // Обробка пікселів зображення паралельно
            Parallel.For(0, image.Height, y =>{
                Parallel.For(0, image.Width, x =>{
                    // Обчислення відстані до центру
                        int distanceX = Math.Abs(x - centerX); 
                        int distanceY = Math.Abs(y - centerY);

                        // Обчислення коефіцієнта збільшення яскравості на основі відстані до центру
                        double brightnessMultiplier = 1.2;
                        brightnessMultiplier += (distanceX / (double)maxDistance) * brightnessFactor;
                        //brightnessMultiplier += (distanceY / (double)maxDistance) * brightnessFactor;

                        // Збільшення яскравості пікселя
                    Bgr pixel = image[y, x];
                        pixel.Blue  = (byte)Math.Min(pixel.Blue * brightnessMultiplier, 255);
                        pixel.Green = (byte)Math.Min(pixel.Green * brightnessMultiplier, 255);
                        pixel.Red   = (byte)Math.Min(pixel.Red * brightnessMultiplier, 255);
                        image[y, x] = pixel;
              
                });
            });

            watch.Stop();
            Debug.WriteLine("FPN: -- " + watch.ElapsedMilliseconds + " ms");
            return image.ToBitmap();
        }


    }


}
