//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.Remoting.Messaging;
//using System.Text;
//using System.Threading.Tasks;

using System.Windows.Forms;

namespace MVision.Resources
{
    class Class1
    {


        PictureBox Load = new PictureBox();



       public void LoadPicture() {

           Load.SizeMode = PictureBoxSizeMode.StretchImage;
  
            Load.ClientSize= new System.Drawing.Size(100,10);
            Load.Location = new  System.Drawing.Point(14, 17);
            Load.SizeMode = PictureBoxSizeMode.CenterImage;
Load.ImageLocation= @"C:\Users\Yurii\Pictures\Untitled.png";


            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {

                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.png)|*.png|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;



                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    Load.Image = new System.Drawing.Bitmap(openFileDialog.FileName);
                }

            }

 }


       System.Diagnostics.Process _process = null;

           //public void LoadImages( bool Run)
           //{
           //    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("LoadImage.exe");

           //    if (Run)
           //    {
           //       _process = System.Diagnostics.Process.Start(startInfo);
           //    }
           //    else
           //    {
           //      _process.Kill(); 
           //      _process.Close();
           //    }
             
           //}

        public void ProgresImages(bool Run)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("ProgressLearning.exe");
        

            if (Run)
            {
                _process = System.Diagnostics.Process.Start(startInfo);
            }
            else
            {
                _process.Kill();
                _process.Close();
            }

        }

        //    Load.Load("C:\\Users\\Yurii\\Pictures\\Untitled.png");


        //Name = "pictureBox",
        //  Size = new Size(100, 50),
        //Location = new Point(14, 17),
        //ImageLocation = @"c:\Images\test.jpg",
        //SizeMode = PictureBoxSizeMode.CenterImage



        // Stretches the image to fit the pictureBox.







        //CvInvoke.Imshow("imgROI", imgROI);

        //Resolt.ROI = box;
        // CvInvoke.Imshow("Resolt", Resolt);
        // CvInvoke.Imshow("Maska", Maska);

        //вивисти картинку на екран
        // Mosaica(ID, Resolt.Bitmap);

        //маскування певних кольорів білим//
        // CvInvoke.BitwiseAnd
        // low_green = np.array([25, 52, 72])
        // high_green = np.array([102, 255, 255])
        // green_mask = cv2.inRange(hsv_frame, low_green, high_green)
        //  green = cv2.bitwise_and(frame, frame, mask = green_mask)




        //public void TestANN(Bitmap Image)
        //{

        //    Image<Bgr, Byte> original = new Image<Bgr, byte>(Image);


        //    original.Bitmap = new Bitmap(Image, 20, 20);// fail 100x50pix
        //    CvInvoke.Imshow("img25", original);
        //    byte[,,] data = original.Data;

        //    Image<Bgr, Byte> Test = new Image<Bgr, byte>(500, 100);
        //    int[,,] ColorRowCols = new int[20, 20, 3];
        //    byte[,,] autImg = new byte[10, original.Cols, 3];



        //    // Сумуємо усі стовпці Image


        //    Console.WriteLine(" Start " + "/" + DateTime.Now.Minute + "/" + DateTime.Now.Millisecond);

        //    /*----------------------------------------------------------------*/
        //    int[,] contpix = new int[2500, 3];
        //    int[,] contpixSum = new int[2500, 3];
        //    int[] contpixLeng = new int[2500];
        //    int idx = 0;
        //    int Cof = 20;

        //    //ЗВЕДЕННЯ КОЛЬОРУ ПО СХОЖОСТІ І ВИЗЕАЧЕННЯ КІЛЬКОСТІ СХОЖИХ КОЛЬОРІВ
        //    for (int i = 0; i < original.Rows; i++)
        //    {
        //        for (int w = 0; w < original.Rows; w++)
        //        {
        //            for (int j = 0; j < (idx + 1); j++)    // 5>3  5<7 
        //            {
        //                if (((contpix[j, 0] > (data[i, w, 0] - Cof)) && (contpix[j, 0] < (data[i, w, 0] + Cof))) || (idx == j))
        //                {
        //                    if (((contpix[j, 1] > (data[i, w, 1] - Cof)) && (contpix[j, 1] < (data[i, w, 1] + Cof))) || (idx == j))
        //                    {
        //                        if (((contpix[j, 2] > (data[i, w, 2] - Cof)) && (contpix[j, 2] < (data[i, w, 2] + Cof))) || (idx == j))
        //                        {

        //                            if (idx == j) { idx++; }
        //                            contpixLeng[j]++;
        //                            contpix[j, 1] = data[i, w, 1]; contpix[j, 2] = data[i, w, 2]; contpix[j, 0] = data[i, w, 0];
        //                            contpixSum[j, 1] += data[i, w, 1]; contpixSum[j, 2] += data[i, w, 2]; contpixSum[j, 0] += data[i, w, 0];
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    for (int w = 0; w < idx; w++)
        //    {
        //        contpix[w, 0] = contpixSum[w, 0] / contpixLeng[w];
        //        contpix[w, 1] = contpixSum[w, 1] / contpixLeng[w];
        //        contpix[w, 2] = contpixSum[w, 2] / contpixLeng[w];
        //    }






        //    byte[,] contpixQ = new byte[2500, 3];
        //    int[,] contpixD = new int[2500, 3];

        //    int[] contpixLengQ = new int[2500];
        //    int[] contpixLengD = new int[2500];

        //    int[] IDXF = new int[1000];


        //    int idxQ = 0;
        //    int idxD = 0;
        //    int CofQ = 10;





        //    //for (int w = 0; w < (idx + 1); w++)    // 5>3  5<7 
        //    //{
        //    //    contpixQ[w, 1] = (byte)contpix[w, 1];
        //    //    contpixQ[w, 2] = (byte)contpix[w, 2];
        //    //    contpixLengQ[w] = contpixLeng[w];
        //    //    idxQ++;
        //    //}
        //    ////////////for (int i = 0; i < idx ; i++)
        //    ////////////{
        //    ////////////    ///зводим до найближчого кольору зменшуєм матрицю
        //    ////////////    if ((contpixLeng[i] <= 2) && (contpixLeng[i] > 0))
        //    ////////////    {
        //    ////////////        for (int w = 0; w < (idx - i); w++)    // 5>3  5<7 
        //    ////////////        {
        //    ////////////            if (idxD >= (idx - idxD -100) {
        //    ////////////                contpixD[idxD, 0] = contpix[i, 0];
        //    ////////////                contpixD[idxD, 1] = contpix[i, 1];
        //    ////////////                contpixD[idxD, 2] = contpix[i, 2];
        //    ////////////                contpixLengD[idxD] = contpixLeng[i];
        //    ////////////                idxD++;
        //    ////////////                break; }
        //    ////////////            contpixD[idxD, 0] = contpix[i, 0];
        //    ////////////            contpixD[idxD, 1] = contpix[i, 1];
        //    ////////////            contpixD[idxD, 2] = contpix[i, 2];
        //    ////////////            if (((contpix[i, 0] > (contpix[w, 0] - CofQ)) && (contpix[w, 0] < (contpix[w, 0] + CofQ))) || (idx == w))
        //    ////////////            {
        //    ////////////                if (((contpix[i, 1] > (contpix[w, 1] - CofQ)) && (contpix[w, 1] < (contpix[w, 1] + CofQ))) || (idx == w))
        //    ////////////                {
        //    ////////////                    if (((contpix[i, 2] > (contpix[w, 2] - CofQ)) && (contpix[w, 2] < (contpix[w, 2] + CofQ))) || (idx == w))
        //    ////////////                    {
        //    ////////////                        if ((contpix[w, 0] != 0) && (contpix[w, 1] != 0) && (contpix[w, 2] != 0))
        //    ////////////                        {
        //    ////////////                            contpixD[idxD, 0] = (contpixD[idxD, 0]+contpix[w, 0])/2;
        //    ////////////                            contpixD[idxD, 1] = (contpixD[idxD, 1]+contpix[w, 1])/2;
        //    ////////////                            contpixD[idxD, 2] = (contpixD[idxD, 2]+contpix[w, 2])/2;
        //    ////////////                            contpixLengD[idxD] = contpixLeng[w];
        //    ////////////                            idxD++;
        //    ////////////                            contpix[w, 0] = 0;
        //    ////////////                            contpix[w, 1] = 0;
        //    ////////////                            contpix[w, 2] = 0;
        //    ////////////                            contpixLeng[w] = 0;
        //    ////////////                        }
        //    ////////////                    }
        //    ////////////                }
        //    ////////////            }
        //    ////////////        }
        //    ////////////    }else
        //    ////////////    {
        //    ////////////        if ((contpix[i, 0] != 0) && (contpix[i, 1] != 0) && (contpix[i, 2] != 0))
        //    ////////////        {
        //    ////////////            contpixD[idxD, 0] = contpix[i, 0];
        //    ////////////            contpixD[idxD, 1] = contpix[i, 1];
        //    ////////////            contpixD[idxD, 2] = contpix[i, 2];
        //    ////////////            contpixLengD[idxD] = contpixLeng[i];
        //    ////////////            idxD++;
        //    ////////////        }
        //    ////////////        }
        //    ////////////}






















        //    //for (int w = 0; w < idxD; w++)    // 5>3  5<7 
        //    //{



        //    //  contpixQ[w, 0] = (byte)contpix[w, 0];
        //    //  contpixQ[w, 1] = (byte)contpixD[w, 1];
        //    //  contpixQ[w, 2] = (byte)contpixD[w, 2];
        //    //  contpixLengQ[w] = contpixLengD[w];


        //    //}

        //    //idxQ = idxD;














        //    //for (int i = 0; i < idx; i++)
        //    //{


        //    //    ///зводим до найближчого кольору зменшуєм матрицю
        //    //    if (contpixLeng[i] <= CofQ)
        //    //    {


        //    //        for (int w = 0; w < (idx + 1); w++)    // 5>3  5<7 
        //    //        {
        //    //            if (((contpixQ[i, 0] > (contpix[w, 0] - CofQ)) && (contpixQ[i, 0] < (contpix[w, 0] + CofQ))) || (idx == w))
        //    //            {
        //    //                if (((contpixQ[i, 1] > (contpix[w, 1] - CofQ)) && (contpixQ[i, 1] < (contpix[w, 1] + CofQ))) || (idx == w))
        //    //                {
        //    //                    if (((contpixQ[i, 2] > (contpix[w, 2] - CofQ)) && (contpixQ[i, 2] < (contpix[w, 2] + CofQ))) || (idx == w))
        //    //                    {

        //    //                        if ((contpix[w, 0] != 0) && (contpix[w, 1] != 0) && (contpix[w, 2] != 0))
        //    //                        {
        //    //                            contpixQ[idxQ, 0] = (byte)contpix[w, 0];
        //    //                            contpixQ[idxQ, 1] = (byte)contpix[w, 1];
        //    //                            contpixQ[idxQ, 2] = (byte)contpix[w, 2];
        //    //                            contpix[idxQ, 0] = 0;
        //    //                            contpix[w, 1] = 0;
        //    //                            contpix[w, 2] = 0;
        //    //                            contpixLengQ[idxQ] = contpixLeng[i];
        //    //                            idxQ++;
        //    //                        }

        //    //                    }
        //    //                }
        //    //            }
        //    //        }


        //    //    }
        //    //    else
        //    //    {
        //    //        if ((contpix[i, 0] != 0) && (contpix[i, 1] != 0) && (contpix[i, 2] != 0))
        //    //        {
        //    //            contpixQ[idxQ, 0] = (byte)contpix[i, 0];
        //    //            contpixQ[idxQ, 1] = (byte)contpix[i, 1];
        //    //            contpixQ[idxQ, 2] = (byte)contpix[i, 2];
        //    //            contpix[i, 0] = 0;
        //    //            contpix[i, 1] = 0;
        //    //            contpix[i, 2] = 0;
        //    //            contpixLengQ[idxQ] = contpixLeng[i];
        //    //            idxQ++;
        //    //        }
        //    //    }


        //    //}











        //    //for (int Q = 0; Q < 30; Q++)
        //    //{   

        //    for (int i = 0; i < idx; i++)
        //    {
        //        int Q = 0;
        //        for (int S = 0; Q < contpixLeng[i]; i++)
        //        {


        //            for (S = 0; S < 100; S++)
        //            {
        //                if (Q >= contpixLeng[i]) { break; }

        //                Test.Data[S, i, 0] = (byte)contpix[i, 0];
        //                Test.Data[S, i, 1] = (byte)contpix[i, 1];
        //                Test.Data[S, i, 2] = (byte)contpix[i, 2];
        //                Q++;
        //            }




        //        }
        //    }

        //    // }




        //    //for (int Q = 0; Q < 30; Q++)
        //    //{

        //    //    for (int i = 0; i < idxQ; i++)
        //    //    {
        //    //        Test.Data[Q, i, 0] = contpixQ[i, 0];
        //    //        Test.Data[Q, i, 1] = contpixQ[i, 1];
        //    //        Test.Data[Q, i, 2] = contpixQ[i, 2];
        //    //    }

        //    //}




        //    //CvInvoke.Imshow("img", Test);
        //    //ЗВЕДЕННЯ КОЛЬОРУ ПО СХОЖОСТІ І ВИЗЕАЧЕННЯ КІЛЬКОСТІ СХОЖИХ КОЛЬОРІВ
        //    for (int i = 0; i < original.Rows; i++)
        //    {


        //    }






        //    Console.WriteLine(" Finish " + "/" + DateTime.Now.Minute + "/" + DateTime.Now.Millisecond);

        //    /** для тестового виводу на екран та додаваня в базу масок **/
        //    //for (
        //    //    int Q = 0; Q < 10; Q++)
        //    //{
        //    //    for (int i = 0; i < original.Cols; i++)
        //    //    {
        //    //        autImg[Q, i, 0] = Convert.ToByte(ColorRowCols[i, 0, 0] / original.Cols);
        //    //        autImg[Q, i, 1] = Convert.ToByte(ColorRowCols[i, 0, 1] / original.Cols);
        //    //        autImg[Q, i, 2] = Convert.ToByte(ColorRowCols[i, 0, 2] / original.Cols);
        //    //    }
        //    //}


        //    /////////////////////////////////////////////////////////////////////////////////////////////////////

        //    ///*  додаєм в базу масок */
        //    //int Cols;
        //    //for (Cols = 0; Cols < original.Cols; Cols++)
        //    //{
        //    //    StudiMask[StudiMaskQunty, Cols, 0] = autImg[0, Cols, 0];
        //    //    StudiMask[StudiMaskQunty, Cols, 1] = autImg[0, Cols, 1];
        //    //    StudiMask[StudiMaskQunty, Cols, 2] = autImg[0, Cols, 2];

        //    //    //  ImgLN[0].ImagMask[StudiMaskQunty, Cols, 0] = autImg[0, Cols, 0];
        //    //    //   ImgLN[0].ImagMask[StudiMaskQunty, Cols, 0] = autImg[0, Cols, 0];
        //    //    //  ImgLN[0].ImagMask[StudiMaskQunty, Cols, 0] = autImg[0, Cols, 0];


        //    //}


        //    StudiMaskQunty++;
        //    Test.Data = autImg;
        //    Mosaics[0].Images.Add(Test.Bitmap);
        //    // CvInvoke.Imshow("img", Test);

        //}






        ///////////////////////      НАВЧАННЯ   1.1    /////////////////////////////

        //    public void TestANN(Bitmap Image)
        //    {
        //        Image = EMGU.CleanBegraund(Image, 0);
        //        Layer Lyr = new Layer();


        //        double[] HandIPCTidx = new double[400];
        //        double BufIPCT = 0.0;


        //        double[] Buf = new double[400];
        //        byte[] Handle = new byte[3];
        //        int Activate = 0;
        //        bool[] HandDoble = new bool[400];


        //        for (int Q = 1; Q < 100; Q++)
        //        {  // ЗМІНЮЄМ НАВЧАЛЬНУ КАРТИНКУ
        //            if (BufImagAnn[Q] == null) { break; }



        //            double[] HandIPCT = new double[400];
        //            bool[] BufDoble = new bool[400];

        //            double[][] HandXXX = new double[400][];


        //            for (int i = 0; i < 400; i++)  // ПЕРЕБОР УСІХ ПІКСЕЛІВ Hand 
        //            {
        //                HandXXX[i] = new double[3];

        //                HandXXX[i][0] = HandImgAnn[i][0];
        //                HandXXX[i][1] = HandImgAnn[i][1];
        //                HandXXX[i][2] = HandImgAnn[i][2];

        //            }

        //            for (int w = 0; w < 400; w++)  // ПЕРЕБОР УСІХ ПІКСЕЛІВ Hand 
        //            {
        //                Activate = 0;


        //                for (int i = 0; i < 400; i++)// ПЕРЕБОР УСІХ ПІКСЕЛІВ З ПРИКЛАДУ КАРТИНКІ Buf
        //                {
        //                    if (BufImagAnn[Q][w] == null) { break; }


        //                    BufIPCT = Lyr.NormPix(BufImagAnn[Q][w], HandXXX[w]);


        //                    if ((BufIPCT > HandIPCT[w]) && (HandDoble[w] == false) && (BufIPCT > 1) && (BufImagAnn[Q][w][0] != 255))
        //                    {
        //                        HandIPCT[w] = BufIPCT;
        //                        Activate = w;
        //                        BufDoble[w] = true;

        //                    }

        //                    if ((BufIPCT == 1) && (BufImagAnn[Q][w][0] != 255))
        //                    {
        //                        HandIPCT[w] = BufIPCT;
        //                        Activate = w;
        //                        BufDoble[w] = true; break;
        //                    } // true поставити після циклу

        //                }



        //                if (Activate != 0)
        //                {
        //                    HandDoble[Activate] = true;
        //                    HandImgAnn[w][0] = BufImagAnn[Q][Activate][0]; //(BufImagAnn[Q][Activate][0] + HandImgAnn[w][0]) / 2;
        //                    HandImgAnn[w][1] = BufImagAnn[Q][Activate][1]; //(BufImagAnn[Q][Activate][1] + HandImgAnn[w][1]) / 2;
        //                    HandImgAnn[w][2] = BufImagAnn[Q][Activate][2]; //(BufImagAnn[Q][Activate][2] + HandImgAnn[w][2]) / 2;
        //                }
        //            }


        //            //  HandIPCT[w] = 0;
        //        }

        //        byte[,,] fotoShou = new byte[20, 20, 3];

        //        if (HandImgAnn[0] != null)
        //        {
        //            int g = 0;
        //            for (int h = 0; h < 20; h++)
        //            {
        //                for (int r = 0; r < 20; r++)
        //                {

        //                    fotoShou[h, r, 0] = (byte)HandImgAnn[g][0];
        //                    fotoShou[h, r, 1] = (byte)HandImgAnn[g][1];
        //                    fotoShou[h, r, 2] = (byte)HandImgAnn[g][2];
        //                    g++;
        //                }
        //            }
        //            Image<Bgr, Byte> test = new Image<Bgr, byte>(20, 20);
        //            test.Data = fotoShou;
        //            test.Bitmap = new Bitmap(test.Bitmap, 100, 100);// fail 100x50pix
        //            CvInvoke.Imshow("img5", test);
        //        }


        //    }
        //}






        //            red = np.uint8([[[0, 0, 255]]]) 
        //redHSV = cv2.cvtColor(red, cv2.COLOR_BGR2HSV)
        //print redHSV

        ////////Mat imgOriginal = new Mat();

        ////////Image<Bgr, Byte> imageCV = new Image<Bgr, byte>(Image); //Image Class from Emgu.CV
        ////////Image<Bgr, Byte> imageDD = new Image<Bgr, byte>(Image); //Image Class from Emgu.CV
        ////////imgOriginal = imageCV.Mat;
        ////////    Mat imgGrayscale = new Mat(imgOriginal.Size, DepthType.Cv16S, 1);
        ////////Mat imgBlurred = new Mat(imgOriginal.Size, DepthType.Cv16U, 1);
        ////////Mat imgCanny = new Mat(20, 20, DepthType.Cv8U, 1);
        ////////CvInvoke.CvtColor(imgOriginal, imgGrayscale, ColorConversion.Bgr2Hsv);
        ////////    Bgr gerge = new Bgr(255, 255, 255);
        ////////Hsv gerge1 = new Hsv(255, 255, 255);

        ////////// CvInvoke.CvtColor (gerge, gerge1, ColorConversion.Bgr2Hsv);

        ////////byte[,,] Dat = new byte[100, 100, 3];

        ////////imageDD.Bitmap = imgGrayscale.Bitmap;

        ////////    for (int r = 0; r<imageDD.Rows; r++)
        ////////    {

        ////////        for (int i = 0; i<imageDD.Cols; i++)
        ////////        {
        ////////            //imageDD.Data[i, r, 1] = 255;
        ////////            //imageDD.Data[i, r, 2] = 200;



        ////////        }
        ////////    }




        // склейка фото 3 трьох частитн
        //Image<Bgr, byte> imgL = new Image<Bgr, byte>(100, 100);
        //Image<Bgr, byte> imgR = new Image<Bgr, byte>(100, 100);
        //Mat NewMatf = new Mat();
        //Bitmap imgE = new Bitmap(ListCut[1][0].Img[0], 100, 100);// fail 100x50pix  
        //Bitmap imgW = new Bitmap(ListCut[0][0].Img[0], 100, 100);// fail 100x50pix \
        //imgL.Bitmap = imgE;
        //imgR.Bitmap = imgW;
        // CvInvoke.VConcat(imgL, imgR, NewMat);
        //Mosaica(0, NewMat.Bitmap);




        //   Image image1 ;
        //   image1 = report.img;

        ////   byte[] Imgbytes = ImageToByteArray(image1);



        //public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        imageIn.Save(ms, imageIn.RawFormat);
        //        return ms.ToArray();
        //    }
        //}




        // Convert bitmap to byte array
        //Bitmap bitmap = new Bitmap(report.img);
        //bitmap.Save("test5.bmp");
        //while (true)
        //{
        //    if (File.Exists("test5.bmp"))
        //        break;
        //}
        //section.AddImage("test5.bmp");




        // CvInvoke.Imshow(MY_ML.DTK.AutName + MY_ML.DTK.AutSimile.ToString(), imgr);
        // CvInvoke.WaitKey(0);  //Wait for the key pressing event

    }
}
