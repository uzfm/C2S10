using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;


namespace MVision
{










    public class SV
    {

        [Serializable()]
        public class Report
        {
            /*****  REPORT  ***********/
            public string NameReport { get; set; }
            public string SempleTyp { get; set; }
            public string CreatedBy { get; set; }
            public string Comments { get; set; }
            public string PathFileSave { get; set; }
            public int FullWeight { get; set; }      // вага тестового зразка грам
            public int PiecesWeighed { get; set; }      // кількість шт тестового зразка

            public double SemplWeight { get; set; }      //середня вага однієї гранули


        }








        [Serializable()]
        public class Device{

            

            public int ID { get; set; }
            public int MasterID { get; set; }
            public int SlaveID { get; set; }
            // нормальний режим швиткості
            public int PWM_Table { get; set; }
            public int Hz_Table { get; set; }
            // на старті пришвидшений режим руху частинок
     

            public string URL_Databases { get; set; }
            public string URL_XLSX { get; set; }
            public string FilesSetingCamMaster { get; set; }
            public string FilesSetingCamSlave { get; set; }
            public int OutputDelay { get; set; }   //час затримка на вимкненя flaps off Timer.

            public int AutoStop { get; set; }      // виключення системи післі закінчення аналізу    

            public int ShowGoodPCS { get; set; }

            public bool LiveView { get; set; } // off/on Live View

        }

            [Serializable()]
            public class ColourMin
            {
                public double[] B = new double[2];
                public double[] G = new double[2];
                public double[] R = new double[2];
            };

           [Serializable()]
            public class ColourMax
            {
                public double[] B = new double[2];
                public double[] G = new double[2];
                public double[] R = new double[2];
            };


        [Serializable()]
        public class Analys {

            public int[] ConturMax = new int[2];
            public int[] ConturMin = new int[2];
                                 
            public int DoubleFlaps;  //Відступ від країв видимості ширини поля в (пікселях по осі(Х))
            public int Shifting { get; set; }   // Deviation along "X" which defines the trajectory to determine the doubling.
            public bool SelectPS;
            public int PositionErrorX;

            // шлях папкі картинок для симуляції
            public string PathSimulation; //Pash Simulation for Images
            public bool AutoReportPDF;    // Автоматичне генеровання репорта після сортування

            public List<ClasSempl> ClasSempl = new List<ClasSempl>();
        }

    


    public class ClasSempl
    {

        /*****  REPORT  ***********/
        public string Name;
        public int SampleSize;
        public string NameSmaller ="_S";
        public string NameLarged  ="_L";
        public bool   SubGroups;    // Активація під груп
        public int   IdxGrp;       //індикс першого значення підгрупи в Image List.(для індексації Mosaic)

 static   public short   IdxGrpleng;         // довжина List з групами  та активованими підгрупами.

            //public bool   TypeOn;

        }

}



    [Serializable()]
    public class SaveClassDT{

         public SV.Analys     Analys    = new SV.Analys();
         public SV.ColourMax  ColourMax = new SV.ColourMax();
         public SV.ColourMin  ColourMin = new SV.ColourMin();
         public SV.Device     Device    = new SV.Device();
         public SV.Report     Report    = new SV.Report();

        public USB_HID.SENSOR.SensorDT SensorsDT = new USB_HID.SENSOR.SensorDT();
        public USB_HID.MOTOR.DT[]      Motor     = new USB_HID.MOTOR.DT[2];
        public _DLS.DT                 DALSA     = new _DLS.DT();

    }





    [Serializable()]
    public class SAV
    {
     static public SaveClassDT DT = new SaveClassDT();

        string FileName = "Settings.json";

        public bool Serialize(string url)
        {


            try
            {
                string filePath = Path.Combine(url, FileName);

                string json = JsonConvert.SerializeObject(DT, Formatting.Indented); File.WriteAllText(filePath, json);
              

                Console.WriteLine("Serialize JSON OK");
                return true;
            }
            catch
            {

                Console.WriteLine("Serialize JSON ERROR");
                return false;
            }
        }

        public bool Deserialize(string url)
        {
            try
            {
                string filePath = Path.Combine(url, FileName);
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    DT = JsonConvert.DeserializeObject<SaveClassDT>(json);
                    Console.WriteLine("Deserialize JSON OK");
                    return true;
                }
            }
            catch
            {
                Console.WriteLine("Deserialize JSON ERROR");
            }


            return false;
        }

    }

















    /// <summary>
    /// ///////////////////////////////////  SAVE SETINGS  The First Ones Files   /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>





    [Serializable()]
     public  class Data
        {
            public string     URL_Models       { get; set; } // шлях для Моделі
            public string     Name_Model       { get; set; }
            public string     Password         { get; set; }
            public StringCollection SampleType { get; set; } 
            public StringCollection CreatedBy  { get; set; }  // назви відів семплів (назва папки)
            
        }




    class STGS
    {

        static public Data DT = new Data();


        private const string FileName = "settings.json";

        public bool Save( )
        {
            string url = System.Windows.Forms.Application.StartupPath;

            try{
                string filePath = Path.Combine(url, FileName);
            
                string json = JsonConvert.SerializeObject(DT, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Console.WriteLine("Serialize JSON OK");
                return true;
            }
            catch
            {
                Console.WriteLine("Serialize JSON ERROR");
                return false;
            }
        }

        public bool Read()
        {
            string url = System.Windows.Forms.Application.StartupPath;
            try
            {
                string filePath = Path.Combine(url, FileName);
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    DT = JsonConvert.DeserializeObject<Data>(json);
                 
                    Console.WriteLine("Deserialize JSON OK");
                    return true;
                }
            }
            catch
            {
                Console.WriteLine("Deserialize JSON ERROR");
            }

            return false;
        }
    }








}

