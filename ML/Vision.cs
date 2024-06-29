using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util.TypeEnum;
using Emgu.Util;
using Emgu.CV.Util;
using System.Drawing;
using Emgu.CV.XObjdetect;
using Emgu.CV.Features2D;
using System.Windows.Forms;

namespace MVision
{
    public class VIS
    {
        public static DATA_Save Data = new DATA_Save();

        [Serializable()]
        public class DATA_Save
        {
            //для виривнюваня фону

            public byte blurA { get; set; }
            public byte ThresholdA { get; set; }


            public byte blurB { get; set; }
            public byte ThresholdB { get; set; }
            public int ArcLengthB { get; set; }

            public int ArcLengthTest { get; set; }

            public int WhiteBekgraun { get; set; }

        }

        static public bool ArcLengTestChecd = false;
        Image<Bgr, byte> ColorBlobimg = new Image<Bgr, byte>(100, 100, new Bgr(255, 0, 0));
        VectorOfVectorOfPoint conturs = new VectorOfVectorOfPoint();


        public Image<Bgr, byte> DetectBlob(Mat img_Dtc, Label labelDectContur) {

            Mat ouput = new Mat();

            ////Середня фільтрація Blur
            CvInvoke.Blur(img_Dtc, ouput, new Size(Data.blurA, Data.blurA), new Point(-1, -1));
            var _imgGry = ouput.ToImage<Gray, byte>();
            Image<Bgr, byte> _img = ouput.ToImage<Bgr, byte>();

            // Визначення середнього значення інтенсивності
            MCvScalar meanIntensity = CvInvoke.Mean(_imgGry);
            int Threshld = (int)(meanIntensity.V0);

            //___________________________________

            // Визначення мінімального та максимального значення інтенсивності
            double MinValue = 0.0;
            double MaxValue = 0.0;
            Point minLocation = new Point();
            Point maxLocation = new Point();

            // Виконання адаптивної порогової обробки
            // Визначення мінімального та максимального значення інтенсивності
            CvInvoke.MinMaxLoc(_imgGry, ref MinValue, ref MaxValue, ref minLocation, ref maxLocation);

            var test = 255 / (Threshld - MinValue); //2.47
            var test2 = (Threshld - MinValue) / test; //41.6

            CvInvoke.Threshold(_imgGry, _imgGry, MinValue - Data.ThresholdA + test2, 255, ThresholdType.Binary);


            Mat hierarchy = new Mat();

            CvInvoke.FindContours(~_imgGry, conturs, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            //Test img
            ColorBlobimg = new Image<Bgr, byte>(img_Dtc.Width, img_Dtc.Height, new Bgr(10, 10, 10));

            // Фільтрування та відображення закритих контурів
            for (int i = 0; i < conturs.Size; i++) {

                double perimeter = CvInvoke.ArcLength(conturs[i], true);

                // Якщо контур є закритим, вивести його шукаєм більш менш кругляшкі
                if (conturs[i].Size >= 1) {

                    CvInvoke.DrawContours(ColorBlobimg, conturs, i, new MCvScalar(20, 20, 255), 1); //RED
                    labelDectContur.Text = "Detected";
                    labelDectContur.ForeColor = Color.Red;
                    return ColorBlobimg;
                }
                else { CvInvoke.DrawContours(ColorBlobimg, conturs, -1, new MCvScalar(20, 255, 20), 1); //GEEN
                }
            }
            labelDectContur.Text = "Not detected";
            labelDectContur.ForeColor = Color.Green;
            return ColorBlobimg;
        }



        public DT _DetectBlob(Mat img_Dtc)
        {
            DT Dt = new DT();

            Mat ouput = new Mat();

            ////Середня фільтрація Blur
            CvInvoke.Blur(img_Dtc, ouput, new Size(Data.blurA, Data.blurA), new Point(-1, -1));
            var _imgGry = ouput.ToImage<Gray, byte>();
            Image<Bgr, byte> _img = ouput.ToImage<Bgr, byte>();

            // Визначення середнього значення інтенсивності
            MCvScalar meanIntensity = CvInvoke.Mean(_imgGry);
            int Threshld = (int)(meanIntensity.V0);

            //___________________________________

            // Визначення мінімального та максимального значення інтенсивності
            double MinValue = 0.0;
            double MaxValue = 0.0;
            Point minLocation = new Point();
            Point maxLocation = new Point();

            // Виконання адаптивної порогової обробки
            // Визначення мінімального та максимального значення інтенсивності
            CvInvoke.MinMaxLoc(_imgGry, ref MinValue, ref MaxValue, ref minLocation, ref maxLocation);

            var test = 255 / (Threshld - MinValue); //2.47
            var test2 = (Threshld - MinValue) / test; //41.6

            CvInvoke.Threshold(_imgGry, _imgGry, MinValue - Data.ThresholdA + test2, 255, ThresholdType.Binary);
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(~_imgGry, conturs, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            //Test img
            ColorBlobimg = new Image<Bgr, byte>(img_Dtc.Width, img_Dtc.Height, new Bgr(10, 10, 10));



            // Фільтрування та відображення закритих контурів
            for (int i = 0; i < conturs.Size; i++) {
                double perimeter = CvInvoke.ArcLength(conturs[i], true);
                if (conturs[i].Size >= 1) {
                    Dt.SizeCNT = SizeAriaDetect(conturs, i);
                    Dt.Detect = true;
                    return Dt;
                }
            }

            Dt.Detect = false;
            return Dt;
        }










        public DT _DetectBlobBlack(Mat img_Dtc) {

            DT dT = new DT();
            Mat ouput = new Mat();
            ////Середня фільтрація Blur
            CvInvoke.Blur(img_Dtc, ouput, new Size(Data.blurB, Data.blurB), new Point(-1, -1));

            var _imgGry = ouput.ToImage<Gray, byte>();

            Image<Bgr, byte> _img = ouput.ToImage<Bgr, byte>();

            CvInvoke.Threshold(_imgGry, _imgGry, Data.ThresholdB, 255, ThresholdType.Binary);

            Mat hierarchy = new Mat();

            CvInvoke.FindContours(~_imgGry, conturs, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            // Фільтрування та відображення закритих контурів
            for (int i = 0; i < conturs.Size; i++)
            {
                double perimeter = CvInvoke.ArcLength(conturs[i], true);

                // Zise contur Check
                if ((perimeter >= Data.ArcLengthB) && (!ArcLengTestChecd)) {
                    dT.SizeCNT = SizeAriaDetect(conturs, i);
                    dT.Detect = true;
                    return dT;
                }
                else {

                    // Zise contur Check Test
                    if ((perimeter >= Data.ArcLengthTest) && (ArcLengTestChecd)) {
                        dT.SizeCNT = SizeAriaDetect(conturs, i);
                        dT.Detect = true;
                        return dT;
                    } }

            }

            dT.Detect = false;
            return dT;
        }



        public Image<Bgr, byte> DetectBlobBlack(Mat img_Dtc, Label labelDectContur) {

            Mat ouput = new Mat();

            ////Середня фільтрація Blur
            CvInvoke.Blur(img_Dtc, ouput, new Size(Data.blurB, Data.blurB), new Point(-1, -1));

            var _imgGry = ouput.ToImage<Gray, byte>();

            Image<Bgr, byte> _img = ouput.ToImage<Bgr, byte>();

            CvInvoke.Threshold(_imgGry, _imgGry, Data.ThresholdB, 255, ThresholdType.Binary);

            Mat hierarchy = new Mat();

            CvInvoke.FindContours(~_imgGry, conturs, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            //Test img
            ColorBlobimg = new Image<Bgr, byte>(img_Dtc.Width, img_Dtc.Height, new Bgr(10, 10, 10));

            // Фільтрування та відображення закритих контурів
            for (int i = 0; i < conturs.Size; i++)
            {
                double perimeter = CvInvoke.ArcLength(conturs[i], true);
                //VectorOfPoint approx = new VectorOfPoint();
                // CvInvoke.ApproxPolyDP(conturs[i], approx, 0.05 * perimeter, true);

                // Zise contur Check
                if ((perimeter >= Data.ArcLengthB) && (!ArcLengTestChecd))
                {
                    CvInvoke.DrawContours(ColorBlobimg, conturs, i, new MCvScalar(20, 20, 255), 1); // RED 
                    labelDectContur.Text = "Detected";
                    labelDectContur.ForeColor = Color.Red;
                    return ColorBlobimg;
                } else {

                    // Zise contur Check
                    if ((perimeter >= Data.ArcLengthTest) && (ArcLengTestChecd))
                    {
                        CvInvoke.DrawContours(ColorBlobimg, conturs, i, new MCvScalar(20, 20, 255), 1); // RED 
                        labelDectContur.Text = "Detected";
                        labelDectContur.ForeColor = Color.Red;
                        return ColorBlobimg;
                    } } }





            CvInvoke.DrawContours(ColorBlobimg, conturs, -1, new MCvScalar(20, 255, 20), 1); // GRIN
            labelDectContur.Text = "Not detected";
            labelDectContur.ForeColor = Color.Green;
            return ColorBlobimg;
        }






































        public Image<Bgr, byte>[] DetectBlobTEST(Mat img_Dtc)
        {
            Mat ouput = new Mat();

            ////Середня фільтрація
            CvInvoke.Blur(img_Dtc, ouput, new Size(Data.blurA, Data.blurA), new Point(-1, -1));

            Image<Bgr, byte>[] _img = new Image<Bgr, byte>[2];
            _img[0] = new Image<Bgr, byte>(img_Dtc.Width, img_Dtc.Height);
            _img[1] = new Image<Bgr, byte>(img_Dtc.Width, img_Dtc.Height);

            var _imgGry = ouput.ToImage<Gray, byte>();
            _img[0] = ouput.ToImage<Bgr, byte>();


            //___________________________________


            // Тепер ви можете використовувати 'averageGradient' для подальших обчислень або візуалізації

            // Обчисліть середнє значення інтенсивності
            //  MCvScalar meanIntensity = CvInvoke.Mean(averageGradient);

            // Визначення середнього значення інтенсивності
            MCvScalar meanIntensity = CvInvoke.Mean(_imgGry);

            // Відображення результату
            //CvInvoke.Imshow("Average Gradient", averageGradient);
            //CvInvoke.WaitKey(0);

            int Threshld = (int)(meanIntensity.V0);

            //___________________________________

            // Визначення мінімального та максимального значення інтенсивності
            double MinValue = 0.0;
            double MaxValue = 0.0;
            Point minLocation = new Point();
            Point maxLocation = new Point();

            // Визначення мінімального та максимального значення інтенсивності
            CvInvoke.MinMaxLoc(_imgGry, ref MinValue, ref MaxValue, ref minLocation, ref maxLocation);

            var test = 255 / (Threshld - MinValue); //2.47
            var test2 = (Threshld - MinValue) / test; //41.6

            CvInvoke.Threshold(_imgGry, _imgGry, MinValue - Data.ThresholdA + test2, 255, ThresholdType.Binary);


            // Виконання адаптивної порогової обробки
            /*  */
            // CvInvoke.AdaptiveThreshold(_imgGry, _imgGry, 5, AdaptiveThresholdType.MeanC, ThresholdType.Binary, threshold, maxValue);

            Mat hierarchy = new Mat();
            CvInvoke.FindContours(~_imgGry, conturs, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);



            // Застосуйте алгоритм Кені для знаходження контурів
            //Mat cannyEdges = new Mat();
            //CvInvoke .Canny(_imgGry, cannyEdges, threshold, maxValue,l2Gradient:false,apertureSize: 3); // Параметри порогу
            // _img[1] = cannyEdges.ToImage<Bgr, byte>();

            ColorBlobimg = new Image<Bgr, byte>(img_Dtc.Width, img_Dtc.Height, new Bgr(10, 10, 10));

            // Фільтрування та відображення закритих контурів
            for (int i = 0; i < conturs.Size; i++)
            {
                double perimeter = CvInvoke.ArcLength(conturs[i], true);
                VectorOfPoint approx = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(conturs[i], approx, 0.04 * perimeter, true);

                // Якщо контур є закритим, вивести його шукаєм більш менш кругляшкі
                if (approx.Size >= 4)
                {
                    CvInvoke.DrawContours(ColorBlobimg, conturs, i, new MCvScalar(20, 20, 255), 1);
                }
                else { CvInvoke.DrawContours(ColorBlobimg, conturs, -1, new MCvScalar(20, 255, 20), 1); }
            }


            /**/


            _img[1] = ColorBlobimg;
            return _img;
        }






        double SizeAriaDetect(VectorOfVectorOfPoint contours, int idx) {






            double temp = CvInvoke.ContourArea(contours[idx]);

            double CameraWidthVisibility = 293; // ширина видимості камери в міліметрах
            double CameraWidthPixl = 8192;       // розширення камери в пікселях
            double SizePixl = CameraWidthVisibility / CameraWidthPixl;

            double SizeCt = 0;
            if (temp != 0) {
                SizeCt = ((temp * SizePixl) * (double)1000.0);
            } else { SizeCt = (SizePixl / 2) * (double)1000.0; }

            return SizeCt;
        }





        public class DT {
            public double SizeCNT;
            public bool Detect;
        }






        public Image<Bgr, byte> DetectBlobWhiteBekgraund(Mat img_Dtc, Label labelDectContur)
        {

            Mat ouput = new Mat();

            ////Середня фільтрація Blur
            CvInvoke.Blur(img_Dtc, ouput, new Size(Data.blurA, Data.blurA), new Point(-1, -1));
            var _imgGry = ouput.ToImage<Gray, byte>();
            Image<Bgr, byte> _img = ouput.ToImage<Bgr, byte>();

            // Визначення середнього значення інтенсивності
            MCvScalar meanIntensity = CvInvoke.Mean(_imgGry);
            int Threshld = (int)(meanIntensity.V0);

            //___________________________________

            // Визначення мінімального та максимального значення інтенсивності
            double MinValue = 0.0;
            double MaxValue = 0.0;
            Point minLocation = new Point();
            Point maxLocation = new Point();

            // Виконання адаптивної порогової обробки
            // Визначення мінімального та максимального значення інтенсивності
            CvInvoke.MinMaxLoc(_imgGry, ref MinValue, ref MaxValue, ref minLocation, ref maxLocation);

            var test = 255 / (Threshld - MinValue); //2.47
            var test2 = (Threshld - MinValue) / test; //41.6

            CvInvoke.Threshold(_imgGry, _imgGry, MinValue - Data.ThresholdA + test2, 255, ThresholdType.Binary);


            Mat hierarchy = new Mat();

            CvInvoke.FindContours(~_imgGry, conturs, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            //Test img
            ColorBlobimg = new Image<Bgr, byte>(img_Dtc.Width, img_Dtc.Height, new Bgr(10, 10, 10));

            // Фільтрування та відображення закритих контурів
            for (int i = 0; i < conturs.Size; i++)
            {

                double perimeter = CvInvoke.ArcLength(conturs[i], true);

                // Якщо контур є закритим, вивести його шукаєм більш менш кругляшкі
                if (conturs[i].Size >= 1)
                {

                    CvInvoke.DrawContours(ColorBlobimg, conturs, i, new MCvScalar(20, 20, 255), 1); //RED
                    labelDectContur.Text = "Detected";
                    labelDectContur.ForeColor = Color.Red;
                    return ColorBlobimg;
                }
                else
                {
                    CvInvoke.DrawContours(ColorBlobimg, conturs, -1, new MCvScalar(20, 255, 20), 1); //GEEN
                }
            }
            labelDectContur.Text = "Not detected";
            labelDectContur.ForeColor = Color.Green;
            return ColorBlobimg;
        }






        public Image<Bgr, byte> WhiteBackground1(Mat image, int intensity) {

            // Перетворення зображення в сірий колір
            Mat grayImage = new Mat();
            CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

            // Застосування порогової обробки
            Mat threshImage = new Mat();
            CvInvoke.Threshold(grayImage, threshImage, intensity, 255, ThresholdType.Binary);

            // Пошук контурів
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(threshImage, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            // Знайти найбільший контур за площею
            int largestContourIndex = 0;
            double largestContourArea = 0;

            for (int i = 0; i < contours.Size; i++)
            {
                double contourArea = CvInvoke.ContourArea(contours[i]);
                if (contourArea > largestContourArea)
                {
                    largestContourArea = contourArea;
                    largestContourIndex = i;
                }
            }

            // Створити маску для найбільшого контуру
            Mat mask = new Mat(image.Size, DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(0));
            //CvInvoke.DrawContours(mask, contours, largestContourIndex, new MCvScalar(200), 1);


            // Згладити контур для точнішого вирізання
            VectorOfPoint largestContour = contours[largestContourIndex];
            VectorOfPoint approxContour = new VectorOfPoint();
            double epsilon = 0.001 * CvInvoke.ArcLength(largestContour, true);
            CvInvoke.ApproxPolyDP(largestContour, approxContour, epsilon, true);


            // Створити маску для найбільшого контуру
            CvInvoke.DrawContours(mask, new VectorOfVectorOfPoint(approxContour), -1, new MCvScalar(200), -1);


            // Вирізати область за контуром
            Mat cutout = new Mat();
            image.CopyTo(cutout, mask);

            // Знайти обмежуючу рамку для найбільшого контуру
            Rectangle boundingRect = CvInvoke.BoundingRectangle(contours[largestContourIndex]);

            // Вирізати область зображення за обмежуючою рамкою
            Mat croppedImage = new Mat(cutout, boundingRect);

            // Змінити розмір вирізаного зображення на 64x64
            Mat resizedImage = new Mat();
            CvInvoke.Resize(croppedImage, resizedImage, new System.Drawing.Size(image.Width, image.Height), 0, 0, Inter.Linear);

            // Створити білий фон 64x64
            Mat whiteBackground = new Mat(image.Width, image.Height, DepthType.Cv8U, 3);
            whiteBackground.SetTo(new MCvScalar(200, 200, 200));

            // Створити маску для вставки вирізаного зображення
            Mat maskResized = new Mat();
            CvInvoke.Resize(mask, maskResized, new Size(image.Width, image.Width), 0, 0, Inter.Linear);

            // Скопіювати вирізане зображення на білий фон з урахуванням маски
            resizedImage.CopyTo(whiteBackground, maskResized);

            // Повернути результат
            return whiteBackground.ToImage<Bgr, byte>();
        }


        public Image<Bgr, byte> ContaminationZise(Mat img, int intensity, int intensityGry, out double diameterMm, double ZipImg)
        {


            int test = (int)Math.Sqrt((double)(64 * 64) * ZipImg);
            Mat image = img.ToImage<Bgr, byte>().Resize(test, test, Inter.Linear).Mat;


            // Перетворення зображення в сірий колір
            Mat grayImage = new Mat();
            CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

            // Застосування порогової обробки
            Mat threshImage = new Mat();
            CvInvoke.Threshold(grayImage, threshImage, intensity, 255, ThresholdType.Binary);

            // Пошук контурів
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(threshImage, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            // Знайти найбільший контур за площею
            int largestContourIndex = 0;
            double largestContourArea = 0;

            for (int i = 0; i < contours.Size; i++)
            {
                double contourArea = CvInvoke.ContourArea(contours[i]);
                if (contourArea > largestContourArea)
                {
                    largestContourArea = contourArea;
                    largestContourIndex = i;
                }
            }

            // Створити маску для найбільшого контуру
            Mat mask = new Mat(image.Size, DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(0)); // встановити чорний фон для маски
            CvInvoke.DrawContours(mask, contours, largestContourIndex, new MCvScalar(20), -1); // намалювати білий контур

            // Стягнути контур на 2 пікселі
            Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Erode(mask, mask, element, new Point(-1, -1), 2, BorderType.Default, new MCvScalar(0));



            // Створити результуюче зображення з білим фоном
            Mat result = new Mat(image.Size, DepthType.Cv8U, 3);
            result.SetTo(new MCvScalar(200, 200, 200)); // встановити білий фон

            // Використовувати маску для копіювання тільки контурів на білий фон
            Mat maskedImage = new Mat();
            CvInvoke.BitwiseAnd(image, image, maskedImage, mask);

            // Скопіювати область з оригінального зображення на білий фон
            maskedImage.CopyTo(result, mask);

            return BlackContur(result, intensityGry, out diameterMm, ZipImg);
        }





        public Image<Bgr, byte> BlackContur(Mat image, int intensityGry, out double diameterMm , double ZipImg)
        {  


            // Перетворення зображення в сірий колір
            Mat grayImage = new Mat();
            CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

            // Застосування порогової обробки
            Mat threshImage = new Mat();
            CvInvoke.Threshold(grayImage, threshImage, intensityGry, 255, ThresholdType.Binary);

            // Пошук контурів
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(~threshImage, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            // Знайти найбільший контур за площею
            int largestContourIndex = 0;
            double largestContourArea = 0;

            for (int i = 0; i < contours.Size; i++)
            {
                double contourArea = CvInvoke.ContourArea(contours[i]);
                if (contourArea > largestContourArea)
                {
                    largestContourArea = contourArea;
                    largestContourIndex = i;
                }
            }




            // Створити маску для найбільшого контуру
            CvInvoke.DrawContours(image, contours, largestContourIndex, new MCvScalar(0,0,255), 1); // намалювати білий контур

            diameterMm = 0;

            if (largestContourArea > 0)
            {
                // Перетворення площі з пікселів в квадратні міліметри
                double largestContourAreaMm2 = largestContourArea * Math.Pow(STGS.DT.   PixelToMm, 2);

                // Обчислення діаметра з площі
                   diameterMm  = 2 * Math.Sqrt(largestContourAreaMm2 / Math.PI);
            }
            if (diameterMm == 0) { diameterMm = 0.001; }

                // Додати текст з діаметром на зображення
                string diameterText = $"{diameterMm:F3} mm";
            Point textPosition = new Point(5, 20); // Позиція тексту на зображенні
            CvInvoke.PutText(image, diameterText, textPosition, Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.8, new MCvScalar(100, 0, 100), 1);



            return image.ToImage<Bgr, byte>();
        }




    }
}
