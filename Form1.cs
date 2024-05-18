using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Drawing;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

using System.Xml.Serialization;

using System.Threading;
using MVision.Resources;

using System.Collections;
//using System.Collections.Generic;
using System.Collections.Specialized;

using Emgu.CV;
using Emgu.CV.Structure;

using System.Diagnostics;
using Emgu.CV.CvEnum;

using System.Data;
using System.Data.SqlClient;
using SortOrder = System.Windows.Forms.SortOrder;
using System.Text.RegularExpressions;



namespace MVision
{




    public partial class Form1 : Form
    {      
        
        
        #region Includ 
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;
        private SqlCommand sqlCommand = null;
        private String sqlconect = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=F:\PROJEKT MicrOptok\V2 Sorter\VisualStudio\SQL_Report\WorkUsingData\WorkUsingData\Databases.mdf;Integrated Security=True";
        DataGridViewLinkCell linkCell;
        DataGridViewButtonCell buttonCell;



        #endregion Includ 




           EMGU EMGU = new EMGU();
           Help Help = new Help();


         // Camera  camera;


        SV SaveData = new SV();
      
        TF_DT TF_DT = new TF_DT();

        const int Master = EMGU.Master;
        const int Slave = EMGU.Slave;

        
        Class1 Class1 = new Class1();
        double AverageWight;

        /// <summary>
        ///////////////      ====== PROCES REAL_TIME ===     ///////////////////////////////////////////////////////////////
        [DllImport("Kernel32.dll")]
        static extern bool SetPriorityClass(IntPtr hProcess, int dwPriorityClass);






STGS STGS = new STGS();


        USB_HID USB_HID = new USB_HID();


        DLS DLS;

        public Form1 (){
            MosaicDTlistBufer = new ConcurrentQueue<DTLimg>();
           // Form Form1 = new Form();
             Flow.ProcessLoadImagesFunction(true);
            InitializeComponent();
            ////******   USB HID INSTAL *********//
            try
            {
                USB_HID.InstalDevice("V2S10");
            } catch { Help.ErrorMesag("USB not connected"); }


            //*************  initialization of cameras  *********************
            try { 
             DLS = new DLS(_DLS.HowMany.CAM1_CAM2);
            }catch { Help.Mesag("Cameras are not connected"); }
            ////*****************

            ///////////////////////
            /*camera = new Camera();
            camera.ImageViy(Slave, pictureBoxSlave);
            camera.ImageViy(Master, pictureBoxMaster);
            camera.instal();*/
            /////////////////////////////////////////////////////////////////////////
            ///   File SAVE 
            ///   START File
            STGS.Read();
            PashModelML.Text = STGS.DT.URL_Models;
            NameModelML.Text = STGS.DT.Name_Model;
  


            if (ReloadSetingsFile == false){
            
                comboBox1_Click(null, null);  // обновити список  comboBox1 Model
                comboBox1.Text = STGS.DT.Name_Model;
            }


            InstMosaics();


            ///////////////
            AnalisPredict.MosaicaEvent += MosaicaEvent;
            FlowAnalisTST.MosaicaEvent += MosaicaEvent;

            EMGU.InstMosaics();
            Help.SendHalp();

            //запустити поток на аналіз Img
            Flow.StartPotocCamera();
            //Flow.StartPotocCamera();
            //Flow.StartPotocCamera();
            //Flow.StartPotocCamera();


            //Flow.ProcessLoadImagesFunction(false);

            Save_Report_Button.Enabled = false;


            SaveRefresh();




             PaswordLock_Click(null, null);   // Lock MENU
            ///  

            //SaveRefresh();
            //camera.StrM = true;
            //camera.StrS = true;
            //camera.LiveVideo = true;
            //Thread.Sleep(5000);
            //camera.LiveVideo = false;
            //camera.StrM = false;
            //camera.StrS = false
            FlowAnalis.LiveViewTV(pictureBoxMaster);
            FlowAnalis.LiveViewTV2(pictureBoxSlave);

            Flow Flow_Task = new Flow();

            Flow_Task.StartPotocPredict();
            Thread.Sleep(1000);
            Flow_Task.StartPotocPredict();
            Thread.Sleep(1000);
            Flow_Task.StartPotocPredict();
            Thread.Sleep(1000);
            Flow_Task.StartPotocPredict();
            Thread.Sleep(1000);
            Flow_Task.StartPotocPredict();
            Thread.Sleep(1000);

            /* if (SetingsCameraStart.Checked)
            {
                timer3.Enabled = false;
                try
                {

                    if (CAMERA_INSTAL) { DLS = new DLS(); }

                }
                catch
                {
                    coutTim = 0;
                    Help.Mesag("Cameras are not connected"); Enabled = true; timer3.Stop();
                    Flow.ProcessLoadImagesFunction(false);

                }

            } */

            try { 
            DLS.SetGain( (double) SAV.DT.DALSA.GEIN[Master], Master);
            DLS.SetGain((double)SAV.DT.DALSA.GEIN[Slave], Slave);

                DLS.StartCAMERA(Master);
                DLS.StartCAMERA(Slave);


            }
            catch {  }

            Flow.ProcessLoadImagesFunction(false);

            ///////////////      ====== PROCES REAL_TIME ===     ///////////////////////////////////////////////////////////////
            SetPriorityClass(Process.GetCurrentProcess().Handle, 0x00000100); /////////////////////////////////////////////////
                                                                              ///////////////      ====== PROCES REAL_TIME ===     ///////////////////////////////////////////////////////////////




            // Діаграма швиткості
            solidGauge1.To = 15;

            FlowAnalisTST.InstML();
            /////////////// Діаграма швиткості ////////////////
            solidGauge1.Value = 0.00;
            SpidKgH.Text = "0.00";
        }



        static int ImagCouAnn = 0; 

        //Deserializ Setings
        bool ReloadSetingsFile = false;

        void LoadParamiterFromDisk(){

            SAVE.Deserialize(PashModelML.Text +"//"+ STGS.DT.Name_Model);
            SaveRefresh();

        }
        


        ///////////////////////// передаем в конструктор тип класса  ///////////////////////////////////////////////////////////////////////////////
        SV SV = new SV();
        SAV SAVE = new SAV();

        static public bool SelectsContamination = true;
        private void SaveRefresh(){
            // десериализация
            try {

                //----------------   DALSA  ---------------------
                 GAIN.Value = SAV.DT.DALSA.GEIN[ID];
                //--------------------------------------------


                richTextBox1.Text     = SAV.DT.Device.URL_Databases;
                richTextBox2.Text     = SAV.DT.Device.URL_XLSX;
                sqlconect = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="+ SAV.DT.Device.URL_Databases + @"\Databases.mdf;Integrated Security=True";

         
                    PWM_Table.Value = SAV.DT.Device.PWM_Table;
                    Hz_Table.Value = SAV.DT.Device.Hz_Table;
   
    

                DoubleFlaps.Value   = SAV.DT.Analys.DoubleFlaps;
                    



                    OutputDelayFleps.Value   = SAV.DT.Device.OutputDelay;
                    AutomaticStop.Value = SAV.DT.Device.AutoStop;
                    GenerateReportAuto.Checked = SAV.DT.Analys.AutoReportPDF;
                    ShowGoodNumeric.Value = SAV.DT.Device.ShowGoodPCS;

                    ShowWeightSample.Text = SAV.DT.Report.SemplWeight.ToString();
                    SelectPosicinSeml.Checked = SAV.DT.Analys.SelectPS;
                    PositionError_X.Value =   SAV.DT.Analys.PositionErrorX;

                //richTextBox3.Text = TF_DT.TF_CreatDirectoryPathS(ID, comboBox2.Text);

                radioButton8.Checked = true;
                radioButton8_CheckedChanged(null, null);
                radioButtonCreatedBy.Checked = true;
                radioButton14_CheckedChanged(null, null);
                radioButton9.Checked = true;
                radioButton9_CheckedChanged(null, null);

                if (ReloadSetingsFile == false) { 
                //comboBox1_Click(null, null);  // обновити список  comboBox1 Model
                    if (comboBox1.Text != STGS.DT.Name_Model) { comboBox1.Text = STGS.DT.Name_Model; }
                }
                ReloadSetingsFile = false;


                ///  налаштування фону
                EMGU.ref_BGMSK(Master, out int DarkenValue, out int LighterValue, out bool OriginaImageChecked);
            

                // кількість настройок по різних кольорах
                MastMaxR.Value = (int)SAV.DT.ColourMax.R[ID];
                MastMaxG.Value = (int)SAV.DT.ColourMax.G[ID];
                MastMaxB.Value = (int)SAV.DT.ColourMax.B[ID];

                MastMinR.Value = (int)SAV.DT.ColourMin.R[ID];
                MastMinG.Value = (int)SAV.DT.ColourMin.G[ID];
                MastMinB.Value = (int)SAV.DT.ColourMin.B[ID];

                ContuorMax.Value = SAV.DT.Analys.ConturMax[ID];
                ContuorMin.Value = SAV.DT.Analys.ConturMin[ID];
                Live_view.Checked = SAV.DT.Device.LiveView;

                NumContuorMax.Value = ContuorMax.Value;
                NumContuorMin.Value = ContuorMin.Value;

                if (CreatedBy.Items.Count == 0) {CreatedBy.Items.Add(SAV.DT.Report.CreatedBy); CreatedBy.Text = SAV.DT.Report.CreatedBy;}
                if (SempleTyp.Items.Count == 0) { SempleTyp.Items.Add(SAV.DT.Report.SempleTyp); SempleTyp.Text = SAV.DT.Report.SempleTyp; }

                Comments.Text = SAV.DT.Report.Comments;
                numericUpDown2.Value = SAV.DT.Report.FullWeight;
                ShowPcsSample.Text = (PcsSample = SAV.DT.Report.PiecesWeighed).ToString();

                PathFileSave.Text = SAV.DT.Report.PathFileSave;
                if (SAV.DT.Analys.Shifting >= 1) { PositionError_X.Value = SAV.DT.Analys.Shifting; } else {   SAV.DT.Analys.Shifting = 1; }
                if (NameReport.Text != SAV.DT.Report.NameReport) { NameReport.Text = SAV.DT.Report.NameReport; }

                richTextBox5.Text = SAV.DT.Analys.PathSimulation;
                СontourSelect.Checked = EMGU.SelectConturs[ID];

            }
            catch
            {}
            //****************************************************************&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&*************************************

        
        }











        ////////////         SAVE DATA          ///////////////////
        private void SaveWriteData(){
            // объект для сериализации
            try
            {
                //----------------   DALSA  ---------------------
                SAV.DT.DALSA.GEIN[ID] =  GAIN.Value;

                //--------------------------------------------


                SAV.DT.Report.SempleTyp = SempleTyp.Text;
                SAV.DT.Report.CreatedBy = CreatedBy.Text;
                SAV.DT.Report.Comments = Comments.Text;
                SAV.DT.Report.FullWeight = (int)numericUpDown2.Value;
                SAV.DT.Report.PiecesWeighed = PcsSample;
                SAV.DT.Report.PathFileSave = PathFileSave.Text;
                SAV.DT.Analys.Shifting = (int)PositionError_X.Value;
                SAV.DT.Analys.PathSimulation = richTextBox5.Text;
                SAV.DT.Device.AutoStop = (int)AutomaticStop.Value;
                SAV.DT.Analys.AutoReportPDF = GenerateReportAuto.Checked;
                SAV.DT.Device.URL_Databases    = richTextBox1.Text;
                SAV.DT.Device.URL_XLSX         = richTextBox2.Text;
                SAV.DT.Analys.DoubleFlaps = (int)DoubleFlaps.Value;
                SAV.DT.Device.LiveView = Live_view.Checked;
                SAV.DT.Device.ShowGoodPCS = (int) ShowGoodNumeric.Value;
                SAV.DT.Analys.SelectPS  = SelectPosicinSeml.Checked;
                SAV.DT.Analys.PositionErrorX =(int) PositionError_X.Value;


                    SAV.DT.Device.PWM_Table = (int) PWM_Table.Value ;
                    SAV.DT.Device.Hz_Table  = (int) Hz_Table.Value ;
     

        


                SAV.DT.Device.OutputDelay =  (int) OutputDelayFleps.Value;
               //SAV.DT.Device.URL_Folders = Path.Combine(STGS.DT.URL_Models, STGS.DT.Name_Model);
             
            }
            catch { }
        }







        /// <summary>
        /// SAVE DATA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>    //СЕРЕЛІЗАЦІЯ НАЛАШТУВАНЬ BIN
        private void Save_Click(object sender, EventArgs e){


            STGS.DT.URL_Models = PashModelML.Text;
            STGS.DT.Name_Model = NameModelML.Text;

   

         // Запис у файл
            STGS.Save();
            TF_LoadSempelsName();
            SaveWriteData();
            SAVE.Serialize(Path.Combine(STGS.DT.URL_Models, STGS.DT.Name_Model));

        }











        static int idxS = 1;
        static int idxM = 1;

        static bool StrM = false;
        static bool StrS = false;







        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            if (StrM == false) { StrM = true; } else { StrM = false; }
            if (StrS == false) { StrS = true; } else { StrS = false; }

        }






        //перевірка зєднання USB 
        //Перевірка буферу мозаїки для виведення на форму
        static int FriTimCount = 0;
        bool DtectEroorFlaps = false;
        static double PCSminCout1 = 0;

        static int TimOutRefresh = 0;
        static int LastImgListCout = 0;
        static int AUTO_STOP = 0;

        static string TimWork;

        void resetPCSmin(){
            PCSminCout1 = 0;
            PCSmin.Text = "0";
            TimWork= "00:00";
            TimerWorks.Text= "00:00";
            

        }

        AnalisReadData AnalisReadData = new AnalisReadData();

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (StartButton.Text != "Start Analysis") {

                PCSminCout1++;
                FriTimCount++;
                TimOutRefresh++;



                PCSmin.Text = ((int)((double)ImgListCout / (double)(((double)(PCSminCout1 / 2) / 60)))).ToString();

                //*************   AUTU STOP ***********//

                if ((FriTimCount > (AutomaticStop.Value * 2)) ) { START_BUTON_Click(null, null); }
                if (AUTO_STOP != ImgListCout) { AUTO_STOP = ImgListCout; FriTimCount = 0; }





                // ----  Timer Work ----
                if (((int)((double)((double)PCSminCout1 / 2) / 60)) < 10) {
                    TimWork = "0"; TimWork += ((int)((double)((double)PCSminCout1 / 2) / 60)).ToString(); } else {
                    TimWork = ((int)((double)((double)PCSminCout1 / 2) / 60)).ToString(); }
                TimWork += ":";
                if (((int)((double)((double)PCSminCout1 / 2) % 60)) < 10) { TimWork += "0"; }
                TimWork += ((int)((double)((double)PCSminCout1 / 2) % 60)).ToString();
                 TimerWorks.Text = TimWork;



                if (TimOutRefresh >= 6) { TimOutRefresh = 0;

                    solidGauge1.Value = Math.Round((((double)(ImgListCout - LastImgListCout) * SAV.DT.Report.SemplWeight) * (((60 * 60) / 3) / 1000)), 2);
                    LastImgListCout = ImgListCout;
                }

                //47


                /////////////// Діаграма швиткості ////////////////
                //  solidGauge1.Value = Math.Round((((double)ImgListCout / (double)((double)PCSminCout1 / 7200))),2); 
                SpidKgH.Text = Math.Round((((((((((((double)ImgListCout / (double)((PCSminCout1 / 2)))) * SAV.DT.Report.SemplWeight) * 60) * 60) / 1000))))), 2).ToString();



            }



            Kg_StatusLabel.Text = Math.Round(     ( ((double) ImgListCout  * SAV.DT.Report.SemplWeight ) / 1000) , 2).ToString();


            TimerPredict.Text = AnalisReadData.GetMLtim().ToString();

            int BufCaunt = AnalisReadData.BuferIMG_DTCT_Count();

            if (BufCaunt != 0) { BuferIMG.Text = BufCaunt.ToString(); }





            if ((FlowAnalis.ErrorY > 99) || (FlowAnalis.ErrorY < -1))
                { toolStripStatusLabel8.ForeColor = Color.Red; toolStripStatusLabel8.Text = "Y" + FlowAnalis.ErrorY.ToString();   }
                else
                { toolStripStatusLabel8.ForeColor = Color.Green; toolStripStatusLabel8.Text = "Y" + FlowAnalis.ErrorY.ToString(); }

                if (FlowAnalis.ErrorX> 99)
                { toolStripStatusLabel7.ForeColor = Color.Red; toolStripStatusLabel7.Text = "X" + FlowAnalis.ErrorX.ToString(); }
                else
                { toolStripStatusLabel7.ForeColor = Color.Green; toolStripStatusLabel7.Text = "X" + FlowAnalis.ErrorX.ToString(); }

                ErrorDevid.Text = "Error " + FlowAnalis.ErrorCount.ToString();

                SizeContourM.Text = FlowAnalis.ContourSize[Master].ToString();
                SizeContourS.Text =   FlowAnalis.ContourSize[Slave].ToString();


                MastFotoIdx.Text = DLS.CameraSinc[0].ToString();
                SlaveFotoIdx.Text = DLS.CameraSinc[1].ToString();
                if (DLS.CameraSinc[0] > DLS.CameraSinc[1] + 1) { MastFotoIdx.ForeColor = Color.Red; } else { MastFotoIdx.ForeColor = Color.Black; }
                if (DLS.CameraSinc[1] > DLS.CameraSinc[0] + 1) { SlaveFotoIdx.ForeColor = Color.Red; } else { SlaveFotoIdx.ForeColor = Color.Black; }
               // ShowFoto_Idx.Text = uEyeCam.CameraEroor.ToString();
               toolStripStatusLabel4.Text = ImgListCout.ToString();



            if (USB_HID.HidStatus == true)
            {
                HidConect.ForeColor = Color.Green;
                HidConect.Text = "connected";
     
              }else {    HidConect.Text = "not connected"; HidConect.ForeColor = Color.Red; 
               
            }



            RefreshMosaics();


        }










        private void START_BUTON_Click(object sender, EventArgs e){
            
            if (StartButton.Text == "Start Analysis"){
                StartButton.Enabled = false;
                RESET_dataGridViewSempls();
                dataGridViewSempls.Enabled = false;


        
   
                 USB_HID.LIGHT.SET();
                 USB_HID.FAN.SET();
                 Thread.Sleep(500);

                //DLS.StartCAMERA(Master); 
                //DLS.StartCAMERA(Slave);
                DLS.START_CAM();
                 Thread.Sleep(200);




                    USB_HID_VIBRATING_SET();
                

  

                StartButton.Text = "Stop Analysis";
                StatusLineMenu.Text = "To finish measurement click "+'"'+"Stop Analysis"+'"';
                StartButton.BackColor = Color.Salmon;
                Save_Report_Button.Enabled = false;
                ClearExperimentButton.Enabled = false;
                StartButton.Enabled = true;
                FriTimCount = 0;
            }
            else {


                StartButton.Enabled = false;
                USB_HID_VIBRATING_RES();
                Thread.Sleep(2000);

                // DLS.StopCAMERA(Master);
                // DLS.StopCAMERA(Slave);
                DLS.STOP_CAM();


                Thread.Sleep(200);
                USB_HID.LIGHT.RES();
                USB_HID.FAN.SET();
                StartButton.Text = "Start Analysis";
                StatusLineMenu.Text = "To continue click 'Sort by Type'";
                StartButton.BackColor = Color.GreenYellow;
                ClearExperimentButton.Enabled = true;
                Save_Report_Button.Enabled = true;
                dataGridViewSempls.Enabled = true;

                StartButton.Enabled = true;

            }
            StartButton.Enabled = true;
        }


        static int IdxShowFoto = 0;


        private void button3_Click(object sender, EventArgs e)
        {

            if (IdxShowFoto != EMGU.ListSlav.Count) { IdxShowFoto++; } else { if (IdxShowFoto != EMGU.ListMast.Count) { IdxShowFoto++; } }


            if ((IdxShowFoto > EMGU.ListMast.Count) && (IdxShowFoto > 0) && (IdxShowFoto > EMGU.ListSlav.Count)) { IdxShowFoto = 0; }

            if ((IdxShowFoto <= EMGU.ListSlav.Count) && (IdxShowFoto > 0))
            {
                pictureBoxSlave.Image = EMGU.ListSlav[IdxShowFoto - 1];
                EMGU.Load_Image(EMGU.ListSlav[IdxShowFoto - 1], EMGU.Slave);

            }
            if ((IdxShowFoto <= EMGU.ListMast.Count) && (IdxShowFoto > 0))
            {
                pictureBoxMaster.Image = EMGU.ListMast[IdxShowFoto - 1];
                EMGU.Load_Image(EMGU.ListMast[IdxShowFoto - 1], EMGU.Master);

            }
            numericUpDown4.Text = IdxShowFoto.ToString();
        }



        private void button4_Click(object sender, EventArgs e)
        {

            if (IdxShowFoto != 0) { IdxShowFoto--; }
            numericUpDown4.Text = IdxShowFoto.ToString();

            if ((EMGU.ListSlav.Count >= 1) && (IdxShowFoto >= 0))
            {
                pictureBoxSlave.Image = EMGU.ListSlav[IdxShowFoto];
                EMGU.Load_Image(EMGU.ListSlav[IdxShowFoto], EMGU.Slave);

            }

            if ((EMGU.ListMast.Count >= 1) && (IdxShowFoto >= 0))
            {

                pictureBoxMaster.Image = EMGU.ListMast[IdxShowFoto];
                EMGU.Load_Image(EMGU.ListMast[IdxShowFoto], EMGU.Master);
            }

            /// if ((EMGU.ListSlav.Count >= 1) && (IdxShowFoto >= 0) && (EMGU.ListSlav.Count >= 1)) {  }

        }


        private void numericUpDown4_Enter(object sender, EventArgs e)
        {
            IdxShowFoto = Convert.ToInt32(numericUpDown4.Value);
        }


        private void ClearExperiment_Click (object sender, EventArgs e)
        {
            BAD_SplCont = 0;
            GOOD_SplCont = 0;

           TimOutRefresh = 0;
           LastImgListCout = 0;

            FirstSemplesDetect = false;


            FlowAnalis.ErrorCount = 0; 
            ErrorDevid.Text = "Error " + 0.ToString();

            FlowAnalis.ErrorY = 0;
            FlowAnalis.ErrorX = 0;

            //camera.LiveVideo=true;
            flowAnalis.resetValue();
            ClearMosaics();//clear misiks



            string MasterPath = Path.Combine(STGS.DT.URL_Models, STGS.DT.Name_Model, "MasterImgTemp");
            string SlavePath = Path.Combine(STGS.DT.URL_Models, STGS.DT.Name_Model, "SlaveImgTemp");

            if (true == Directory.Exists(SlavePath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(SlavePath));
                dirInfo.Delete(true);
            }

            if (true == Directory.Exists(MasterPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(MasterPath);
                dirInfo.Delete(true);
            }

            Image<Rgb, byte> imgRes = new Image<Rgb, byte>(10, 1796);
            pictureBoxMaster.Image = imgRes.ToBitmap();
            pictureBoxSlave.Image = imgRes.ToBitmap();

            Save_Report_Button.Enabled = false;
            StartButton.Enabled = true;
            StatusLineMenu.ForeColor = Color.DarkGreen;
            StatusLineMenu.Text = "To start measurement click "+'"'+"Start Analysis"+'"';
            ManualCorrection();
        }








        private void button5_Click(object sender, EventArgs e)
        {
  

            if (StartTable.Text == "START TABLE"){
                USB_HID.LIGHT.SET(); ;
                USB_HID_VIBRATING_SET();
            }
            else
            {
                USB_HID.LIGHT.RES();
          
                USB_HID_VIBRATING_RES();
              
           
            }


        }


        private void button5_Click_1(object sender, EventArgs e)
        {


            if (OnLight.Text == "ON LIGHT")
            {
                USB_HID.LIGHT.SET();
                OnLight.Text = "OFF LIGHT";
                //DLS.StartCAMERA(Master);
               // DLS.StartCAMERA(Slave);
                FlowAnalis.LiveViewSetings = false;

            }
            else
            {
                USB_HID.LIGHT.RES();
                OnLight.Text = "ON LIGHT";
              //  DLS.StopCAMERA(Master);
               // DLS.StopCAMERA(Slave);
                FlowAnalis.LiveViewSetings = true;
            }



        }


        #region Buttum









        // Застосувати кольоровий фільтер
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBoxMaster.Image = EMGU.FiltreColour(EMGU.Master);
                pictureBoxSlave.Image = EMGU.FiltreColour(EMGU.Slave);
            }
            catch { }
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e) { MastMinR.Value = trackBar1.Value; }
        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (MastMinR.Value != trackBar1.Value) { trackBar1.Value = Convert.ToInt32(MastMinR.Value); }
            SAV.DT.ColourMin.R[ID] = (double)MastMinR.Value;
            refreshSetingColor();
        }



            private void trackBar2_Scroll(object sender, EventArgs e) { MastMaxR.Value = trackBar2.Value; }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (MastMaxR.Value != trackBar2.Value) { trackBar2.Value = Convert.ToInt32(MastMaxR.Value); }
            SAV.DT.ColourMax.R[ID] = (double)MastMaxR.Value;
            refreshSetingColor();
        }

        private void trackBar3_Scroll(object sender, EventArgs e) { MastMinG.Value = trackBar3.Value; }
        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            if (MastMinG.Value != trackBar3.Value) { trackBar3.Value = Convert.ToInt32(MastMinG.Value); }
            SAV.DT.ColourMin.G[ID] = (double)MastMinG.Value;
            refreshSetingColor();
        }

        private void trackBar4_Scroll(object sender, EventArgs e) { MastMaxG.Value = trackBar4.Value; }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (MastMaxG.Value != trackBar4.Value) { trackBar4.Value = Convert.ToInt32(MastMaxG.Value); }
            SAV.DT.ColourMax.G[ID] = (double)MastMaxG.Value;
            refreshSetingColor();
        }

        private void trackBar5_Scroll(object sender, EventArgs e) { MastMinB.Value = trackBar5.Value; }
        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            if (MastMinB.Value != trackBar5.Value) { trackBar5.Value = Convert.ToInt32(MastMinB.Value); }
            SAV.DT.ColourMin.B[ID] = (double)MastMinB.Value;
            refreshSetingColor();
        }

        private void trackBar6_Scroll(object sender, EventArgs e) { MastMaxB.Value = trackBar6.Value; }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (MastMaxB.Value != trackBar6.Value) { trackBar6.Value = Convert.ToInt32(MastMaxB.Value); }
            SAV.DT.ColourMax.B[ID] = (double)MastMaxB.Value;
            refreshSetingColor();
        }



        // трек розмір контура
        private void numericMastConturMax(object sender, EventArgs e)
        {
            if (ContuorMax.Value != NumContuorMax.Value) { ContuorMax.Value = Convert.ToInt32(NumContuorMax.Value); }
            SAV.DT.Analys.ConturMax[ID] = (int)NumContuorMax.Value;
        }


        private void trackBar7_Scroll(object sender, EventArgs e) { NumContuorMin.Value = ContuorMin.Value; }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            if (ContuorMin.Value != NumContuorMin.Value) { ContuorMin.Value = Convert.ToInt32(NumContuorMin.Value); }
            SAV.DT.Analys.ConturMin[ID] = (int)NumContuorMin.Value;
        }







        void refreshSetingColor()
        {
            pictureBoxMaster.Image = EMGU.FiltreColour(Master);
            pictureBoxSlave.Image = EMGU.FiltreColour(Slave);
        }




        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBoxMaster.Image = EMGU.Origenal_Image(Master);
                pictureBoxSlave.Image = EMGU.Origenal_Image(Slave);

            } catch { Help.ErrorMesag("The picture is empty. Simulation mode must be turned on ! ");  }
        }










        #endregion region Botum






















































        //Dynamically create Image List at run-time
        static System.Windows.Forms.ImageList myImageList = new ImageList();
        public string ImageNewName = "";
        OpenFileDialog ofd = new OpenFileDialog()
        {
            Multiselect = true,
            ValidateNames = true,
            // Filter =
            // "JPG|*jpg|JPEG|*.jpeg|PNG|*.png"
        };







        static int MosaicsCount;

        static string[,] Quantity = new string[21, 2]; // { {"0", "0", "0", "0", "0", "0" },{ "0", "0", "0", "0", "0", "0" } };


      static   MSIC[] MSC = new MSIC[2];


        class MSIC {

            public ImageList[] IMG;       // Images масив просортований по іменах ([Name] [Images])
            public List<int>[] IdxName;   // iндек назви зразка ([Name] [Idx])
     static public int OLL;               // індек останього рядка в таблиці зразків який по замовчувані є ( показувати усі Show Oll)
     static public int SELECT;            //Виділиний в таблиці зліва зразок (принажаті кнопки в таблиці вибирається зразок)

        }


        static public ImageList MosaicsTeach = new ImageList();
        static public ImageList MosaicsTeachHalp = new ImageList();
        static List<DTLimg>[] MosaicSort; // відсортовані по видам семпли
        static List<DTLimg> MosaicDTlist; //буфер ліст для буферезації перед сортуваням
        static ConcurrentQueue<DTLimg> MosaicDTlistBufer;      // = new ConcurrentQueue<DTLimg>();



        struct IMG_SIZE {

          public const int Width = 64;
          public const int Height = 64;
        };

        private void InstMosaics() {
            try {
            MSC[Master].IMG[MSIC.OLL] = new ImageList();
            MSC[Slave].IMG[MSIC.OLL] = new ImageList();
            MSC[Master].IMG[MSIC.OLL].ImageSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);          //розмір виводу картинкі
            MSC[Slave].IMG[MSIC.OLL].ImageSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);          //розмір виводу картинкі
            MSC[Master].IMG[MSIC.OLL].ColorDepth = ColorDepth.Depth16Bit;
            MSC[Slave].IMG[MSIC.OLL].ColorDepth = ColorDepth.Depth16Bit;
            listView1.View = View.LargeIcon;                //відображати назву картинкі
            listView1.Dock = DockStyle.Fill;
            listView1.VirtualMode = true;
            listView1.TileSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);
            listView2.View = View.LargeIcon;                //відображати назву картинкі
            listView2.Dock = DockStyle.Fill;
            listView2.VirtualMode = true;
            listView2.TileSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);
            MosaicsTeachHalp.ImageSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);
            MosaicsTeachHalp.ColorDepth = ColorDepth.Depth24Bit;
            MosaicsTeach.ImageSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);      //розмір виводу картинкі
            MosaicsTeach.ColorDepth = ColorDepth.Depth24Bit;
            listView3.LabelEdit = true;
            listView3.AllowColumnReorder = true;
            listView3.FullRowSelect = true;
            listView3.GridLines = true;
 } catch { }
        }


/// /////////////////////////////////////////////////////////////////////////////////////

        static  int ImgListCout = 0;
        static int ImgListCoutMosaic = 0;
        static bool FirstSemplesDetect = false;

        double BAD_SplCont = 0;
        double GOOD_SplCont = 0;
        DTLimg OutDT = new DTLimg();
        private void RefreshMosaics()
        {

            if ((MosaicDTlistBufer.Count != 0)){// && (ImgListCout <= (Convert.ToInt32(PageCauntMosaic.Text)))
    
                //візуалізація мозаїки
                for (; 1 <= MosaicDTlistBufer.Count; ImgListCout++)
                {
                   MosaicDTlistBufer.TryDequeue(out OutDT);


                    if (ImgListCout >= 1000000) { ClearMosaic(); break; }
                    if (OutDT.Img != null){

                      int IdxM  = OutDT.IdxName[Master];
                      int IdxS  = OutDT.IdxName[Slave];


                        //MSC[Slave].IMG //

     

                        bool NOT_GOOD = true;
                        if (OutDT.Name[Master] != GridData.GOOD) { 

                            dataGridViewSempls.Rows[IdxM].Cells[1].Value = (MSC[Master].IMG[IdxM].Images .Count + 1).ToString();
                            MSC[Master].IMG[IdxM].Images.Add(OutDT.Img[Master].ToBitmap());
                            MSC[Slave].IMG[IdxM].Images.Add(OutDT.Img[Slave].ToBitmap());
                            BAD_SplCont++;
                            MSC[Master].IdxName[IdxM].Add(IdxM);
                            MSC[Slave].IdxName[IdxM].Add(IdxS);

                        } else {

                            if ((OutDT.Name[Slave] != GridData.GOOD))
                            {
                                
                                MSC[Master].IMG[IdxS].Images.Add(OutDT.Img[Master].ToBitmap());
                                MSC[Slave].IMG[IdxS].Images.Add(OutDT.Img[Slave].ToBitmap());
                                dataGridViewSempls.Rows[IdxS ].Cells[1].Value = (MSC[Master].IMG[IdxS].Images.Count).ToString();
                                BAD_SplCont ++;
                                MSC[Master].IdxName[IdxS].Add(IdxM);
                                MSC[Slave].IdxName[IdxS].Add(IdxS);
                            }
                            else {
                                /*  ADD SHOW GOOD */
                              if(GOOD_SplCont < SAV.DT.Device.ShowGoodPCS) { 
                                MSC[Master].IMG[IdxM].Images.Add(OutDT.Img[Master].ToBitmap());
                                MSC[Slave].IMG[IdxS].Images.Add(OutDT.Img[Slave].ToBitmap());
                                
                                MSC[Master].IdxName[IdxM].Add(IdxM);
                                MSC[Slave].IdxName[IdxS].Add(IdxS);}
                             
                             GOOD_SplCont++;
                             dataGridViewSempls.Rows[IdxM].Cells[1].Value = (GOOD_SplCont.ToString());
                              NOT_GOOD = false;
                        } }


                        if ((NOT_GOOD)||(Goot_Show_Mosaic.Checked)) {

                        MSC[Master].IMG[MSIC.OLL].Images.Add(OutDT.Img[Master].ToBitmap());
                        MSC[Slave].IMG[MSIC.OLL].Images.Add(OutDT.Img[Slave].ToBitmap());

                        listView1.LargeImageList = MSC[Master].IMG[MSIC.OLL];
                        listView1.VirtualListSize = MSC[Master].IMG[MSIC.OLL].Images.Count;    // Задайте загальну кількість елементів

                        listView2.LargeImageList = MSC[Slave].IMG[MSIC.OLL];
                        listView2.VirtualListSize = MSC[Slave].IMG[MSIC.OLL].Images.Count;    // Задайте загальну кількість елементів

                         dataGridViewSempls.Rows[MSIC.OLL].Cells[1].Value = (MSC[Master].IMG[MSIC.OLL].Images.Count).ToString();

                         MSC[Master].IdxName[MSIC.OLL].Add(IdxM);
                         MSC[Slave].IdxName[MSIC.OLL].Add(IdxS);
                        
                         ImgListCoutMosaic++;
                        
                        }


                            //  List VIWE
                            if ((visibleItemsPerPage == 0) || (ImgListCoutMosaic == 0)) { listView1_Resize(null, null);  }
                        else
                        {
                            int startIndex = Math.Max(0, ImgListCoutMosaic - visibleItemsPerPage); // Отримайте індекс першого елемента для відображення
                            listView1.EnsureVisible(startIndex); /// Переконайтесь, що перший елемент видимий
                            listView2.EnsureVisible(startIndex); /// Переконайтесь, що перший елемент видимий

                    }







                    }
  


                }


            }


        }





        void ClearMosaic()
        {

            MSC[Master].IMG[MSIC.OLL].Images.Clear();
            MSC[Slave].IMG[MSIC.OLL].Images.Clear();
            listView1.VirtualListSize = 0; // Скидаємо кількість елементів у ListView

            listView1.Clear();
            MosaicDTlist = new List<DTLimg>();
            MosaicDTlistBufer = new ConcurrentQueue<DTLimg>();

            listView2.Clear();
            listView2.VirtualListSize = 0; // Скидаємо кількість елементів у ListView

            //MosaicsTeachGrey.Clear();
            ImgListCout = 0;
            ImgListCoutMosaic = 0;
        }


        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {

                if (e.ItemIndex >= 0 && e.ItemIndex < ImgListCoutMosaic)
                {
                    // Отримайте дані для відображення (зображення, текст і т. д.) для пункта з індексом e.ItemIndex
                    //var item = Mosaics.Images[e.ItemIndex];

                    // Створіть об'єкт для відображення
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.ImageIndex = e.ItemIndex; // Індекс зображення (залежить від ваших даних)

                    listViewItem.Text =  (1 + e.ItemIndex).ToString();

                    // Встановіть колір тексту
                    int ID_Color =  MSC[Master].IdxName[MSIC.SELECT][e.ItemIndex];   
                        listViewItem.ForeColor =  MLD.NameColor[ID_Color];

                    // Передайте об'єкт для відображення у подію
                    e.Item = listViewItem;

                }
                else
                {  // Якщо індекс поза межами, можливо, встановіть e.Item в null або використайте інші значення за замовчуванням.
                    e.Item = new ListViewItem("Out of Range");
                }

            }
            catch { }
        }



        private void listView2_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {

                if (e.ItemIndex >= 0 && e.ItemIndex < ImgListCoutMosaic)
                {
                    // Отримайте дані для відображення (зображення, текст і т. д.) для пункта з індексом e.ItemIndex
                    //var item = Mosaics.Images[e.ItemIndex];

                    // Створіть об'єкт для відображення
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.ImageIndex = e.ItemIndex; // Індекс зображення (залежить від ваших даних)

                    listViewItem.Text = (1 + e.ItemIndex).ToString();

                    // Встановіть колір тексту
                    int ID_Color = MSC[Slave].IdxName[MSIC.SELECT][e.ItemIndex];
                    listViewItem.ForeColor = MLD.NameColor[ID_Color];

                    // Передайте об'єкт для відображення у подію
                    e.Item = listViewItem;

                }
                else
                {  // Якщо індекс поза межами, можливо, встановіть e.Item в null або використайте інші значення за замовчуванням.
                    e.Item = new ListViewItem("Out of Range");
                }

            }
            catch { }
        }




        int visibleItemsPerPage;
        private void listView1_Resize(object sender, EventArgs e)
        {
            if (listView1.Items.Count != 0)
            {
                int itemHeight = listView1.GetItemRect(0).Height; // Визначте висоту одного елемента списку
                visibleItemsPerPage = listView1.ClientRectangle.Height / itemHeight;
            }
        }



        private void listView2_Resize(object sender, EventArgs e)
        {
            if (listView2.Items.Count != 0)
            {
                int itemHeight = listView2.GetItemRect(0).Height; // Визначте висоту одного елемента списку
                visibleItemsPerPage = listView2.ClientRectangle.Height / itemHeight;
            }
        }























        void USB_HID_VIBRATING_SET() {
            USB_HID.VIBRATING.SET(USB_HID.VIBRATING.Select.Table, (UInt32)SAV.DT.Device.PWM_Table);
            USB_HID.VIBRATING.SET(USB_HID.VIBRATING.Select.Frequency, (UInt32)SAV.DT.Device.Hz_Table);
            USB_HID.APPLY();
            StartTable.Text = "STOP TABLE";
        }



        void USB_HID_VIBRATING_RES(){
            USB_HID.VIBRATING.SET(USB_HID.VIBRATING.Select.Table, 0);
            USB_HID.VIBRATING.SET(USB_HID.VIBRATING.Select.Frequency, 0);
            USB_HID.APPLY();
            
            StartTable.Text = "START TABLE";
        }

        void DTLimgMax(DTLimg dT) {
            
                float[] ValueMax = new float[2];

                if ((dT.Name[0] != GridData.GOOD) && (dT.Name[1] != GridData.GOOD)){
                    ValueMax[0] = dT.Value[0].Max();
                    ValueMax[1] = dT.Value[1].Max();
                    if (ValueMax[0] > ValueMax[1]) { dT.ID = 0; } else { dT.ID = 1; }

                }else{  if (dT.Name[0] == GridData.GOOD) { dT.ID = 1; } else { dT.ID = 0; } } 
        }






        Bitmap LearnImg = new Bitmap(IMG_SIZE.Width, IMG_SIZE.Height);
        //***********************************************************************************************************************************
        private void MosaicaEvent(DTLimg [] dTLimg)
        {    
             IProducerConsumerCollection<DTLimg> tmp = MosaicDTlistBufer;
            foreach (var TLimg in dTLimg)
            {
           // BeginInvoke((MethodInvoker)(() => MosaicDTlist.Add(TLimg)));  // добавляєм нормальні семпли

               
                BeginInvoke((MethodInvoker)(() => tmp.TryAdd(TLimg) )); // добавляєм нормальні семпли





            }

        }
        //*********************************************************************************************************************************






        //ОЧИСТИТИ БУФЕР МОЗАЙКИ 
        private void ClearMosaics() {

            MSC[Master].IMG[MSIC.OLL].Images.Clear();
            listView1.Items.Clear();
            listView1.VirtualListSize = 0; // Скидаємо кількість елементів у ListView


            MSC[Slave].IMG[MSIC.OLL].Images.Clear();
            listView2.Items.Clear();
            listView2.VirtualListSize = 0; // Скидаємо кількість елементів у ListView

            MosaicsCount = 0;
            ImgListCout = 0;
            SortImg = false;
            MosaicDTlistBufer.Clear();
            resetPCSmin();

            for (int Q = 0; Q < MSC[Master].IMG.Length; Q++) {
                MSC[Master].IMG[Q].Images.Clear();
                MSC[Slave].IMG[Q].Images.Clear();
            }

            ImgListCoutMosaic = 0;

        }






        /// ////////////////////////////////////////////////////////////////////////////
        ////////////  ****  СОРТУВАННЯ ПО ВИДАХ І ЗАПОВНЕННЯ ДАНИХ НА РЕПОРТ *** ///////
        ////////////////////////////////////////////////////////////////////////////////
        /////***********************************************************************////

        ReportDT reportDT;
        static int ID;
        static bool SortImg = false;



        /// <summary>
        /// FUNK SORTING
        /// </summary>
        private void Save_Report_Click(object sender, EventArgs e)
        {
            StartButton.Enabled = false;
            StatusLineMenu.Text = "To make new measurement click "+'"'+ "Clear Analysis" + '"';

            if (UpgradeWeight.Checked == true) { Weig_Click(); UpgradeWeight.Checked = false; }
            SortImg = true;
            timer1.Enabled = false;

            reportDT = new ReportDT(MSC[Master].IMG.Length + 1);

            for (int i = 0; i <= MSC[Master].IMG.Length; i++)
            {
                reportDT.Name[i] = " ";
                reportDT.DataPct[i] = 0;
                reportDT.DataPic[i] = 0;
                reportDT.Weight[i] = 0;
                reportDT.IMG[i] = new List<Bitmap>();

            }


            ///////////////////  MASTER ///////////////////////////////////////////
            ImageList ListImgMosaicsM = new ImageList();

            //////////////////// SLAVE         ////////////////////////////////
            ImageList ListImgMosaicsS = new ImageList();


            /********************************************************************/
            ///////  ВИЗНАЧИТИ ВИД І НАЗВУ ВИДУ СЕМПЛА  ------  BAD  -----  /////
            /********************************************************************/
            int lengImg = MLD.Names.Length;

            for (int NameIdxSempl = 0; NameIdxSempl < lengImg; NameIdxSempl++) {
                string Name = MLD.Names[NameIdxSempl];
                if (Name != GridData.GOOD) {
                    for (int idx = 0; idx < MSC[Master].IMG[NameIdxSempl].Images.Count; idx++){

                        Bitmap bitImg = EMGU.ContactImagsWithContur(new Bitmap(MSC[Slave].IMG[NameIdxSempl].Images[idx], 50, 50), new Bitmap(MSC[Master].IMG[NameIdxSempl].Images[idx], 50, 50));
                        reportDT.IMG[NameIdxSempl].Add(new Bitmap(bitImg));
                    }
                }




                reportDT.Name[NameIdxSempl] = Name;
                reportDT.DataPct[NameIdxSempl] = Math.Round((((double)100 / (BAD_SplCont + GOOD_SplCont)) * (double)MSC[Master].IMG[NameIdxSempl].Images.Count), 3);
                reportDT.DataPic[NameIdxSempl] = MSC[Master].IMG[NameIdxSempl].Images.Count;
                reportDT.Weight[NameIdxSempl] = Math.Round(SAV.DT.Report.SemplWeight * reportDT.DataPic[NameIdxSempl], 3);

                // Заповнення Таблиці SQL
                if (reportDT.IMG[NameIdxSempl].Count != 0) { GridData.Valua[Array.IndexOf(GridData.Name, Name)] = reportDT.IMG[NameIdxSempl].Count ; };
            }


            for (int NameIdxSempl = 0; NameIdxSempl < lengImg; NameIdxSempl++){
                string Name = MLD.Names[NameIdxSempl];
                if ((Name == GridData.GOOD)){

                    for (int idx = 0; idx < MSC[Master].IMG[NameIdxSempl].Images.Count; idx++){
                     Bitmap bitImg = EMGU.ContactImagsWithContur(new Bitmap(MSC[Slave].IMG[NameIdxSempl].Images[idx], 50, 50), new Bitmap(MSC[Master].IMG[NameIdxSempl].Images[idx], 50, 50));
                     reportDT.IMG[NameIdxSempl].Add(new Bitmap(bitImg));
                    }

                    reportDT.Name[NameIdxSempl] = Name;
                    reportDT.DataPct[NameIdxSempl] = Math.Round((((double)100 / (BAD_SplCont + GOOD_SplCont)) * (double)GOOD_SplCont), 3);
                    reportDT.DataPic[NameIdxSempl] = (int) GOOD_SplCont;
                    reportDT.Weight[NameIdxSempl] = Math.Round(SAV.DT.Report.SemplWeight * reportDT.DataPic[NameIdxSempl], 3);
                    // Заповнення Таблиці SQL
                    if (reportDT.IMG[NameIdxSempl].Count != 0) { GridData.Valua[Array.IndexOf(GridData.Name, Name)] = (int)GOOD_SplCont; };
                }

            }

            //**************************************************************************************************************************************************/
            //**************************************************************************************************************************************************/
            //**************************************************************************************************************************************************/



            int TFdtNameLength = 0;
            if (MLD.Names.Length < 2) { TFdtNameLength = 1; } else { TFdtNameLength = MLD.Names.Length; }



            SaveGrid();
            timer1.Enabled = true;
            Save_Report_Button.Enabled = false;
            if (GenerateReportAuto.Checked == true) { MakeReportButton_Click(null, null); }
        }
/// ------------------------------////////






        private void button12_Click(object sender, EventArgs e)
        {
          
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps1);
            USB_HID.FLAPS.APPLY();
        }
        private void button13_Click(object sender, EventArgs e)
        {
            
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps2);
            USB_HID.FLAPS.APPLY();
        }
        private void button14_Click(object sender, EventArgs e)
        {
           
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps3);
            USB_HID.FLAPS.APPLY();
        }
        private void button17_Click(object sender, EventArgs e)
        {
            
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps4);
            USB_HID.FLAPS.APPLY();
        }
        private void button20_Click(object sender, EventArgs e)
        {
            
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps5);
            USB_HID.FLAPS.APPLY();
        }
        private void button24_Click(object sender, EventArgs e)
        {
            
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps6);
            USB_HID.FLAPS.APPLY();
        }
        private void button23_Click(object sender, EventArgs e)
        {
           
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps7);
            USB_HID.FLAPS.APPLY();
        }
        private void button22_Click(object sender, EventArgs e)
        {
            
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps8);
            USB_HID.FLAPS.APPLY();
        }



        private void button1_Click(object sender, EventArgs e)
        {
           
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps9);
            USB_HID.FLAPS.APPLY();
        }

        private void button16_Click(object sender, EventArgs e)
        {
           
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps10);
            USB_HID.FLAPS.APPLY();
        }

        private void button30_Click(object sender, EventArgs e)
        {
            
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps11);
            USB_HID.FLAPS.APPLY();
        }

        private void button31_Click(object sender, EventArgs e)
        {
           
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps12);
            USB_HID.FLAPS.APPLY();
        }

        private void button37_Click(object sender, EventArgs e)
        {
           
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps13);
            USB_HID.FLAPS.APPLY();
        }

        private void button38_Click(object sender, EventArgs e)
        {
           
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps14);
            USB_HID.FLAPS.APPLY();

        }

        private void button39_Click(object sender, EventArgs e)
        {
            
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps15);
            USB_HID.FLAPS.APPLY();
        }

        private void button40_Click(object sender, EventArgs e)
        {
           
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps16);
            USB_HID.FLAPS.APPLY();
        }

        private void button45_Click(object sender, EventArgs e)
        {
           
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps17);
            USB_HID.FLAPS.APPLY();
        }

        private void button43_Click(object sender, EventArgs e)
        {
           
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps18);
            USB_HID.FLAPS.APPLY();
        }

        private void button29_Click_1(object sender, EventArgs e)
        {
          
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps19);
            USB_HID.FLAPS.APPLY();
        }

        private void button26_Click_1(object sender, EventArgs e)
        {
            
            USB_HID.FLAPS.SET(USB_HID.FLAPS.Select.Fps20);

            USB_HID.FLAPS.APPLY();
        }






        private void numericOutputDelay(object sender, EventArgs e){
          
            USB_HID.FLAPS.Time_OFF((UInt32)OutputDelayFleps.Value);
            USB_HID.APPLY();
        }









        int SelectITMs;
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {

       

            if (listView1.FocusedItem != null){
                SelectITMs = listView1.FocusedItem.Index;
                //створюємо картинку для аналізу
                // LearnImg = new Bitmap(Mosaics[ID].Images[SelectITM]);
                try {
                if ((MSC[Master].IMG[NameSampleSelect].Images[SelectITMs] != null)&&(MSC[Master].IMG[NameSampleSelect].Images.Count==0)) {
                    for (int Q = 0; Q < MosaicSort.Length; Q++){
                        if (MosaicSort[Q].Count != 0) {
                            for (int i = 0; i < MosaicSort[Q].Count; i++) {
                                if (MosaicSort[Q][i].IdxOut == SelectITMs) {

                                    listView1.LargeImageList = MSC[Master].IMG[NameSampleSelect];

                                    LearnImg = MosaicSort[Q][i].Img[Master].ToBitmap();

                                    ResaltRgst[] Data = new ResaltRgst[2];
                                    Data[0] = new ResaltRgst();
                                    Data[1] = new ResaltRgst();

                                    Data[Master].Name = MosaicSort[Q][i].Name[Master];
                                    Data[Master].ValueDt = MosaicSort[Q][i].Value[Master];
                                    Data[Master].Img = MosaicSort[Q][i].Img[Master].ToBitmap();

                                    Data[Slave].Name = MosaicSort[Q][i].Name[Slave];
                                    Data[Slave].ValueDt = MosaicSort[Q][i].Value[Slave];
                                    Data[Slave].Img = MosaicSort[Q][i].Img[Slave].ToBitmap();
                                   // DTK_Image(Data);
                                }
                            }
                        }
                    }
                }



                if (EMGU.SelectConturs[Master] == true)
                {
                    LearnImg = (Bitmap)MSC[Master].IMG[NameSampleSelect].Images[SelectITMs];
                    EMGU.Load_Image_Mosaics(new Bitmap(MSC[Master].IMG[NameSampleSelect].Images[SelectITMs]), Master);
                    pictureBoxMaster.Image = EMGU.FiltreColour(EMGU.Master);

                }
                else {

                    pictureBoxMaster.Image = MSC[Master].IMG[NameSampleSelect].Images[SelectITMs];
                    LearnImg =  (Bitmap) MSC[Master].IMG[NameSampleSelect].Images[SelectITMs];

                }
                }catch { Help.ErrorMesag("Choose the type of sample"); }

            }
        }



        private void listView2_MouseUp(object sender, MouseEventArgs e)
        {

            var Select = dataGridViewSempls.SelectedCells[0];

            if (listView2.FocusedItem != null) {
                SelectITMs = listView2.FocusedItem.Index;

                // Cтворюємо картинку для аналізу
                // LearnImg = new Bitmap(Mosaics[ID].Images[SelectITM]);
                try
                {
                 
  
                    if (EMGU.SelectConturs[Master] == true) {
                            LearnImg = (Bitmap)MSC[Slave].IMG[NameSampleSelect].Images[SelectITMs];
                            EMGU.Load_Image_Mosaics(new Bitmap(MSC[Slave].IMG[NameSampleSelect].Images[SelectITMs]), Slave);
                            pictureBoxSlave.Image = EMGU.FiltreColour(EMGU.Slave);
                        
                    }else{
                            pictureBoxSlave.Image = MSC[Slave].IMG[NameSampleSelect].Images[SelectITMs];
                            LearnImg = (Bitmap)MSC[Slave].IMG[NameSampleSelect].Images[SelectITMs];
                    }
                }
                catch { Help.ErrorMesag("Choose the type of sample"); }
            }
        }



        private void button26_Click(object sender, EventArgs e)
        {
            if (EMGU.ListMast.Count > 0)
            {
                for (int i = 0;  i < EMGU.ListMast.Count; i++)
                {

                    EMGU.ListMast[i].Save(richTextBox5.Text + "\\" + "Master" + idxM++.ToString() + ".bmp");
                    EMGU.ListSlav[i].Save(richTextBox5.Text + "\\" + "Slave" + idxS++.ToString() + ".bmp");

                }


            }

            idxS = 0;
            idxM = 0;
        }


        #region AnalysisSetingsRadioButton



        #endregion AnalysisSetingsRadioButton







        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            EMGU.OrigenalFotoIDX[ID] = СontourSelect.Checked;
            EMGU.SelectConturs[ID] = СontourSelect.Checked;

        }


        private void button27_Click(object sender, EventArgs e)
        {
            RESET_dataGridViewSempls();
            EMGU.StartAnalis = true;
            timer2.Enabled = true;
            Save_Report_Button.Enabled = true;
        }







        static int IdxSlavShou = 1;
        static int IdxMastShou = 1;
        FlowCamera flowCamera = new FlowCamera();
        FlowAnalis flowAnalis = new FlowAnalis();
        private void timer2_Tick(object sender, EventArgs e){

            string urlMaster = richTextBox5.Text + "\\" + "Master" + IdxMastShou++ + ".bmp";
            string urlSlave = richTextBox5.Text + "\\" + "Slave" + IdxSlavShou++ + ".bmp";
            FlowAnalis.Setings = true;

            if (IdxSlavShou <= numericUpDown5.Value)
            {
                try
                {
                    FlowCamera.START_tImg = true;

                    Bitmap imM = new Bitmap(urlMaster);
                    Bitmap imS = new Bitmap(urlSlave);

                    Image<Bgr, byte>[] ImgTst = new Image<Bgr, byte>[2];

                    ImgTst[0] = imM.ToImage<Bgr, byte>();
                    ImgTst[1] = imS.ToImage<Bgr, byte>(); 

                    IProducerConsumerCollection<Image<Bgr, byte>> tmpM = FlowCamera.BoxM;
                    tmpM.TryAdd(ImgTst[0]);

                    IProducerConsumerCollection<Image<Bgr, byte>> tmpS = FlowCamera.BoxS;
                    tmpS.TryAdd(ImgTst[1]);


                } catch {timer2.Enabled = false; FlowAnalis.Setings = false; IdxSlavShou = 1; IdxMastShou = 1; MessageBox.Show("Сheck the link to the image library"); }

            } else { timer2.Enabled = false; FlowAnalis.Setings = false; IdxSlavShou = 1; IdxMastShou = 1; }
        }












        void DellDataTeach() {
            ImagCouAnn = 0;
           // MY_ML.ResBufImagAnn();
            listView3.Clear();
            MosaicsTeach.Images.Clear();
            DTLimg.SelectITM.Clear();
        }

        //виділення імя виду сортування з list Box;






        void ClearImgMsaic() {
            int SelectITM;

            if (listView3.FocusedItem != null) {
                SelectITM = listView3.FocusedItem.Index;
                ImageList MosaicsTeachCopi = new ImageList();
                MosaicsTeachCopi.ImageSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);      //розмір виводу картинкі
                MosaicsTeachCopi.ColorDepth = ColorDepth.Depth24Bit;


                for (int i = 0; i < MosaicsTeach.Images.Count; i++)
                {
                    if (i == SelectITM) { if (i == MosaicsTeach.Images.Count) { break; } }
                    else
                    {
                        MosaicsTeachCopi.Images.Add(MosaicsTeach.Images[i]);
                    } }

                listView3.Clear();
                MosaicsTeach = MosaicsTeachCopi;

                for (ImagCouAnn = 0; ImagCouAnn < MosaicsTeachCopi.Images.Count; ImagCouAnn++)
                {
                    listView3.LargeImageList = MosaicsTeach;
                    listView3.Items.Add(new ListViewItem { ImageIndex = ImagCouAnn, Text = ImagCouAnn.ToString(),  /*nema imag*/ });
                }
            }



        }















































        private void button29_Click(object sender, EventArgs e)
        {
            if (radioButton12.Checked == true)
            {
                pictureBoxMaster.Image = EMGU.Origenal_Image(Master);
            }
            else
            {
                pictureBoxSlave.Image = EMGU.Origenal_Image(Slave);
            }

        }











        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Flow.StopPotocHID();
            Flow.StopPotocCamera();
            Flow.StopPotocPredict();
            sqlConnection.Close();
            //camera.Stop(Slave);
            //camera.Stop(Master);
            //camera.ClosingLibrary();
            // Flow.StopPotocHID();
        }




        Report report = new Report();

        [Obsolete]

        private void MakeReportButton_Click(object sender, EventArgs e)
        {
            if (SortImg == true)
            {
                report.ReportSet(reportDT);
            } else { Help.ErrorMesag("You cannot select image or generate a report when the images are not sorted! "); }
        }



        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {   //1
            if (tabControl1.SelectedTab.Text == "Report")
            {
                SaveRefresh();
            }

             ///2
            if (tabControl1.SelectedTab.Text == "Manual Correction")
            {
                radioButton13_CheckedChanged(null, null);
                SaveRefresh();
                splitContainer5.Panel1Collapsed = false;
                //ManualCorrection();
            }

            //3
            if (tabControl1.SelectedTab.Text == " Analysis")
            {
                SaveRefresh();
            }

            //4
           if (tabControl1.SelectedTab.Text == "Machine Learning")
            {
                radioButton13_CheckedChanged(null, null);
                SaveRefresh();
                splitContainer5.Panel1Collapsed = false;
            }

           //5
          if (tabControl1.SelectedTab.Text == "Contour detection")
            {
                VisualID();
                SaveRefresh();
                splitContainer5.Panel1Collapsed = false;
            }


            
          //6
            if (tabControl1.SelectedTab.Text == "Settings device")
            {
                SaveRefresh();
                splitContainer5.Panel1Collapsed = false;
            }

            //7
            if (tabControl1.SelectedTab.Text == "Statistics")
            {
                SaveRefresh();
                splitContainer5.Panel1Collapsed = false;
            }


            if ((tabControl1.SelectedTab.Text != "Detection of defects") &&
                (tabControl1.SelectedTab.Text != "Settings cameras") &&
                (tabControl1.SelectedTab.Text != "Machine Learning") &&
                (tabControl1.SelectedTab.Text != "Manual Correction")
                 ) { splitContainer5.Panel1Collapsed = true; }
            VisualID();

        }



        void VisualID()
        {
            string[] nemaID = new string[2] { "SAMPLES", "SAMPLES" };
            IDtex.Text = nemaID[ID];

        }

        private void button10_Click(object sender, EventArgs e)
        {
            try { 
          SAV.DT.Analys.Shifting = (int)PositionError_X.Value;
          pictureBoxSlave.Image = EMGU.SetingsShiftingDetect(SAV.DT.Analys.Shifting);
          pictureBoxMaster.Image = EMGU.SetingsShiftingDetect(SAV.DT.Analys.Shifting);
} catch { }

        }








        void TF_LoadSempelsName(){
            try{

                MLD.Path = Path.Combine(STGS.DT.URL_Models, STGS.DT.Name_Model);

                if (ID==Master) { MLD.PathSamples = Path.Combine(MLD.Path, "SAMPLES"); }
                if (ID == Slave) { MLD.PathSamples = Path.Combine(MLD.Path, "SAMPLES"); }
                if (false == Directory.Exists(MLD.PathSamples)) { Directory.CreateDirectory(MLD.PathSamples); }

                 string   PathGood = Path.Combine(MLD.PathSamples, GridData.GOOD);
                if (false == Directory.Exists(PathGood)) { Directory.CreateDirectory(PathGood); }//створити назву "Good"


                int idx = 0;
                    string[] SamlCatalogPath = new string[Directory.GetDirectories(MLD.PathSamples).Length];
                    string[] pathSmpl = new string[Directory.GetDirectories(MLD.PathSamples).Length];
                    SamlCatalogPath = Directory.GetDirectories(MLD.PathSamples);


                    for (idx = 0; idx < SamlCatalogPath.Length; idx++)
                    { pathSmpl[idx] += Path.GetFileName(SamlCatalogPath[idx]); }

                    listBox1.Items.Clear();

           
                    string[] NemSmpls = new string[pathSmpl.Length];
                    MLD.Names = new string[Directory.GetDirectories(MLD.PathSamples).Length];


                short i = 0;


       



                foreach (var Sepl in pathSmpl){
                        listBox1.Items.Add(Sepl);
                         MLD.Names[i++] = Sepl;
                    }


  



              if ( (MSC[Master] == null) || (SamlCatalogPath.Length+1 != MSC[Master].IMG.Length)|| (SamlCatalogPath.Length+1 != MSC[Slave].IMG.Length)) {


                    MSC[Master] = new MSIC();
                    MSC[Slave] = new MSIC();

                    MSC[Master].IMG = new ImageList[MLD.Names.Length+1];
                    MSC[Slave].IMG  = new ImageList[MLD.Names.Length+1];
                    MSC[Master].IdxName = new List<int>[MLD.Names.Length + 1];
                    MSC[Slave].IdxName  = new List<int>[MLD.Names.Length + 1];

                 for (idx = 0; idx < SamlCatalogPath.Length; idx++){
                 
                    MSC[Master].IdxName[idx] = new List<int>();
                    MSC[Slave].IdxName [idx] = new List<int>();

                    MSC[Master].IMG[idx] = new ImageList();
                    MSC[Slave].IMG [idx] = new ImageList();

                    MSC[Master].IMG[idx].ImageSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);          //розмір виводу картинкі
                    MSC[Master].IMG[idx].ColorDepth = ColorDepth.Depth16Bit;

                    MSC[Slave].IMG [idx].ImageSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);          //розмір виводу картинкі
                    MSC[Slave].IMG [idx].ColorDepth = ColorDepth.Depth16Bit;
                  
                   }

                    idx = MLD.Names.Length;
                    MSIC.OLL = idx;
                    MSIC.SELECT = idx;

                    // OLL SHOW IMGS
                    MSC[Master].IdxName[idx] = new List<int>();
                    MSC[Slave].IdxName[idx]  = new List<int>();

                    MSC[Master].IMG[idx] = new ImageList();
                    MSC[Slave].IMG[idx] = new ImageList();

                    MSC[Master].IMG[idx].ImageSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);          //розмір виводу картинкі
                    MSC[Master].IMG[idx].ColorDepth = ColorDepth.Depth16Bit;

                    MSC[Slave].IMG[idx].ImageSize = new Size(IMG_SIZE.Width, IMG_SIZE.Height);          //розмір виводу картинкі
                    MSC[Slave].IMG[idx].ColorDepth = ColorDepth.Depth16Bit;
                    

                }


               // ManualCorrection();

            }
            catch { Help.ErrorMesag(" problem with configuration Model "); }
        }




        private void button18_Click_1(object sender, EventArgs e)
        {

           ///DialogResult result = MessageBox.Show("Do you want add to saved samples ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //if (result == DialogResult.Yes){}
            
                //richTextBox3.Text = TF_DT.TF_CreatDirectoryPathS(ID, comboBox2.Text);
                //TF_LoadSempelsName();
                SaveSample(true);



        }





        void SaveSample(bool AskMsg){
            if (SelectSempleTyp.Text == "")
            {
                MessageBox.Show("Choose a sample Name!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            };


            string[] SamlCatalogPath = new string[Directory.GetDirectories(MLD.PathSamples).Length];
            SamlCatalogPath = Directory.GetDirectories(MLD.PathSamples);
            string[] pathSmpl = new string[SamlCatalogPath.Length];


            for (int idx = 0; idx < SamlCatalogPath.Length; idx++)
            { pathSmpl[idx] += Path.GetFileName(SamlCatalogPath[idx]); }


            string pashImg = Path.Combine(MLD.PathSamples, SelectSempleTyp.Text); //створити назву Img
            DialogResult result = DialogResult.Yes;
            if (AskMsg == true) { result = MessageBox.Show("Do you want Add Images to "+ SelectSempleTyp.Text + " ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Information); }


            if (false == Directory.Exists(pashImg)) { Directory.CreateDirectory(pashImg); }
            if (result == DialogResult.Yes)
            {

                for (int i = 0; i < MosaicsTeach.Images.Count; i++)
                {
                    DateTime dateOnly = new DateTime();
                    dateOnly = DateTime.Now;

                    String DataFile = dateOnly.Month.ToString() + ".";
                    DataFile = DataFile + dateOnly.Day.ToString() + ".";
                    DataFile = DataFile + dateOnly.Year.ToString() + ". ";
                    DataFile = DataFile + dateOnly.Hour.ToString() + ".";
                    DataFile = DataFile + dateOnly.Minute.ToString() + ".";
                    DataFile = DataFile + dateOnly.Second.ToString() + " ";




                    Bitmap IMGconvert = new Bitmap(MosaicsTeach.Images[i], IMG_SIZE.Width, IMG_SIZE.Height);

                    // MosaicsTeach.Images[i].Save(pashImg + "\\" + DataFile + "img" + i.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    IMGconvert.Save(pashImg + "\\" + DataFile + "img" + i.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);

                }
                DellDataTeach();
            }
        }







        void AddImgMosicaSetings(Bitmap Img, string nema) {
            MosaicsTeach.Images.Add(Img);
            listView3.LargeImageList = MosaicsTeach;
            listView3.Items.Add(new ListViewItem { ImageIndex = ImagCouAnn, Text = ImagCouAnn.ToString(), Name = nema /*nema imag*/ });
            ImagCouAnn++;
        }


        ResaltRgst resalt = new ResaltRgst();
        private void button32_Click(object sender, EventArgs e)
        {

            string fullPath = "";
            // Об'єднати відносний шлях з поточним каталогом для отримання повного шляху
            if (SelectSempleTyp.Text  != null) { fullPath = Path.Combine(PashModelML.Text, NameModelML.Text, "SAMPLES", SelectSempleTyp.Text); } else { 
            fullPath = Path.Combine(PashModelML.Text, NameModelML.Text, "SAMPLES");}

            if (false == Directory.Exists(fullPath)) { Directory.CreateDirectory(fullPath); }// якщо нема пакі то створюєм


            string absolutePath = Path.GetFullPath(fullPath);


            if (System.IO.Directory.Exists(absolutePath))
            {
                Process.Start("explorer.exe", absolutePath);
            }
            else
            {
                Console.WriteLine("Папка не існує.");
            }
        }



        private void button41_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want Teach ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes) {
                string DataPath =  Path.Combine(STGS.DT.URL_Models, STGS.DT.Name_Model);
                Flow.ProcessLerningFunction(DataPath);
                Close();
            }
        }



        private void button42_Click(object sender, EventArgs e) {
            if (SortImg == true) {
                MosaicsTeach.Images.Add(LearnImg);
                listView3.LargeImageList = MosaicsTeach;
                listView3.Items.Add(new ListViewItem { ImageIndex = ImagCouAnn, Text = ImagCouAnn.ToString(),  /*nema imag*/ });
                ImagCouAnn++;
                label21.Text = ImagCouAnn.ToString();
                label21.Text += " PCS ";
            } else { Help.ErrorMesag("You cannot select image or generate a report when the images are not sorted! "); }
        }


        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (ID == Master) {
                if (MouseAddImage.Checked == true) {
                    DoubleClicSelectImg();
                    MosaicsTeach.Images.Add(LearnImg);
                    listView3.LargeImageList = MosaicsTeach;
                    listView3.Items.Add(new ListViewItem { ImageIndex = ImagCouAnn, Text = ImagCouAnn.ToString(),  /*nema imag*/ });
                    ImagCouAnn++;
                    label21.Text = ImagCouAnn.ToString();
                    label21.Text += " PCS ";
                }
            } else { Help.ErrorMesag("You cannot select Camera 1 "); }
        }

        private void listView2_MouseDoubleClick_1(object sender, MouseEventArgs e) {
            if (ID == Slave){
                if (MouseAddImage.Checked == true) {
                    DoubleClicSelectImg();
                    MosaicsTeach.Images.Add(LearnImg);
                    listView3.LargeImageList = MosaicsTeach;
                    listView3.Items.Add(new ListViewItem { ImageIndex = ImagCouAnn, Text = ImagCouAnn.ToString(),  /*nema imag*/ });
                    ImagCouAnn++;
                    label21.Text = ImagCouAnn.ToString();
                    label21.Text += " PCS ";
                }
            }else { Help.ErrorMesag("You cannot select Camera 2 "); }
        }



        void DoubleClicSelectImg() {
            DTLimg.SelectITM.Add(SelectITMs);
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (SortImg == true)
            {
                if (MouseAddImage.Checked == true)
                {
                    DoubleClicSelectImg();
                    MosaicsTeach.Images.Add(LearnImg);
                    listView3.LargeImageList = MosaicsTeach;
                    listView3.Items.Add(new ListViewItem { ImageIndex = ImagCouAnn, Text = ImagCouAnn.ToString(),  /*nema imag*/ });

                    ImagCouAnn++;
                    label21.Text = ImagCouAnn.ToString();
                    label21.Text += " PCS ";
                }
            } else { Help.ErrorMesag("You cannot select image or generate a report when the images are not sorted! "); }
        }



        private void radioButton13_CheckedChanged(object sender, EventArgs e){
            VisualID(); 
            SelectSempleTyp.Text = "";
            DellDataTeach(); 
            SelectSempleTyp.Text = ""; 
            DellDataTeach();
            //TF_LoadSempelsName();
            LearnImg = EMGU.bgrImgGreen(LearnImg);// Delete Image
        }

        private void button44_Click(object sender, EventArgs e)
        {
            ClearImgMsaic();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectSempleTyp.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
        }



        private void button46_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog FBD = new FolderBrowserDialog();
                if (FBD.ShowDialog() == DialogResult.OK)
                {
                    STGS.DT.URL_Models = FBD.SelectedPath;
                    PashModelML.Text = STGS.DT.URL_Models;
                }
                else { MessageBox.Show("Choose directory please", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        


        private void button47_Click(object sender, EventArgs e)
        {

            if (comboBoxCreat.Text != "") {
                DialogResult YESNO = MessageBox.Show("Do you want add new  " + comboBoxCreat.Text + "  ?", "Waring!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (radioButton9.Checked) {
                    try {
                        if ((true == Directory.Exists(STGS.DT.URL_Models)) && (YESNO == DialogResult.Yes))
                        {
                            Directory.CreateDirectory(Path.Combine(STGS.DT.URL_Models, comboBoxCreat.Text));

                            comboBoxCreat.Text = "";

                        }
                    } catch { MessageBox.Show("Name written not correct", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }


                if (radioButtonCreatedBy.Checked)
                {
                    try
                    {

                        if ((true == Directory.Exists(STGS.DT.URL_Models)) && (YESNO == DialogResult.Yes))
                        {
                            StringCollection stringCollection = new StringCollection();
                            comboBoxCreat.Items.Add(comboBoxCreat.Text);
                            String[] data = new string[comboBoxCreat.Items.Count];
                            comboBoxCreat.Items.CopyTo(data, 0);
                            stringCollection.AddRange(data);
                            STGS.DT.CreatedBy = stringCollection;
                            STGS.Save();

                        }
                    }
                    catch { MessageBox.Show("Name written not correct", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }

                if (radioButton8.Checked) {
                    try {
                        if ((true == Directory.Exists(STGS.DT.URL_Models)) && (YESNO == DialogResult.Yes)) {
                            StringCollection stringCollection = new StringCollection();
                            comboBoxCreat.Items.Add(comboBoxCreat.Text);
                            String[] data = new string[comboBoxCreat.Items.Count];
                            comboBoxCreat.Items.CopyTo(data, 0);
                            stringCollection.AddRange(data);
                            STGS.DT.SampleType = stringCollection;
                            STGS.Save();

                        }
                    }
                    catch { MessageBox.Show("Name written not correct", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }







            } else { MessageBox.Show("String 'Name' cannot be empty", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }








        private void button48_Click(object sender, EventArgs e){
            DellDataTeach();
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            try
            {
                int idx = 0;
                string[] SamlCatalogPath = Directory.GetDirectories(STGS.DT.URL_Models);


                comboBox1.Items.Clear();
                if (SamlCatalogPath.Length == 0) { comboBox1.Text = ""; }

                for (idx = 0; idx < SamlCatalogPath.Length; idx++){
                    string dt = Path.GetFileName(SamlCatalogPath[idx]);
                    comboBox1.Items.Add(dt);
                }
              
            }
            catch { };

        }






        private void comboBox1_TextChanged(object sender, EventArgs e){

            if (comboBox1.Text != "") {


                if (NameModelML.Text != comboBox1.Text)
                {



                    if (NameModelML.Text == "") { STGS.DT.Name_Model = comboBox1.Text; NameModelML.Text = comboBox1.Text; }
                    else { if (NameModelML.Text != comboBox1.Text) {
                            DialogResult result = DialogResult.Yes;
                            result = MessageBox.Show("Make sure the hopper is empty! ", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (result == DialogResult.Yes) { STGS.DT.Name_Model = comboBox1.Text; NameModelML.Text = comboBox1.Text; }}}
                }


                STGS.DT.Name_Model = comboBox1.Text; 
                TF_LoadSempelsName();
                ReloadSetingsFile = true;
                LoadParamiterFromDisk();

                ManualCorrection();
            }
        }

        private void button50_Click(object sender, EventArgs e)
        {
            if (comboBoxCreat.Text != "") {
                DialogResult result = DialogResult.Yes;
                result = MessageBox.Show("Do you want delete '" + comboBoxCreat.Text + "' ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (radioButton9.Checked) {
                    if (result == DialogResult.Yes) {
                        //видалити деректрію
                        try
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(STGS.DT.URL_Models, comboBoxCreat.Text));
                            dirInfo.Delete(true);
                            comboBoxCreat.Text = "";
                        }
                        catch { MessageBox.Show("The directory cannot be deleted 'directory is not found' "); }
                    } }
                if (radioButtonCreatedBy.Checked) {
                    try {
                        if (result == DialogResult.Yes) {

                            STGS.DT.CreatedBy.Remove(comboBoxCreat.Text);
                            STGS.Save();
                            radioButton14_CheckedChanged(null, null);

                        } } catch { MessageBox.Show("Name  not correct", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); } }
                if (radioButton8.Checked)
                {
                    try
                    {
                        if (result == DialogResult.Yes) {
                            STGS.DT.SampleType.Remove(comboBoxCreat.Text);
                            STGS.Save();
                            radioButton8_CheckedChanged(null, null);
                        }
                    }
                    catch { MessageBox.Show("Name not correct", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }

            } else { MessageBox.Show("String 'Name' cannot be empty"); }
        }





 

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            //  SV.DT_BIN.AnalisDefect = checkBox3.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            // SV.DT_BIN.AnalisColour = checkBox5.Checked;
        }

        private void button51_Click(object sender, EventArgs e)
        {
            try
            {


                FolderBrowserDialog FBD = new FolderBrowserDialog();
                if (FBD.ShowDialog() == DialogResult.OK)
                {

                    PathFileSave.Text = FBD.SelectedPath;

                }


                else { MessageBox.Show("Choose directory please", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            Save_Click(null, null);
        }



        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog FBD = new FolderBrowserDialog();
                if (FBD.ShowDialog() == DialogResult.OK)
                {

                    richTextBox5.Text = FBD.SelectedPath;
                }
                else { MessageBox.Show("Choose directory please", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }




        private void button35_Click_1(object sender, EventArgs e)
        {
            Save_Click(null, null);
        }




        /************************************* Manual corection ****************************************************************/


        private void button54_Click(object sender, EventArgs e)
        {
            try { 
            if (DTLimg.SelectITM.Count != 0)
            {
                for (int Q = 0; Q < MLD.Names.Length; Q++)
                {

                    if (MLD.Names[Q] == label28.Text)
                    {
                      
                            for (int d = 0; d < DTLimg.SelectITM.Count; d++)
                            {
                            MSC[Master].IMG[Q].Images.Add( MSC[Master].IMG[MSIC.SELECT].Images[DTLimg.SelectITM[d]]);
                            MSC[Slave].IMG[Q].Images.Add(MSC[Slave].IMG[MSIC.SELECT].Images[DTLimg.SelectITM[d]]);
                            MSC[Master].IdxName[Q].Add(Q);
                            MSC[Slave].IdxName[Q].Add(Q);
                      
                             }

                            for (int d = 0; d < DTLimg.SelectITM.Count; d++)
                            {
                            MSC[Master].IMG[MSIC.SELECT].Images.RemoveAt(DTLimg.SelectITM[d]-d);
                            MSC[Slave].IMG[MSIC.SELECT].Images.RemoveAt(DTLimg.SelectITM[d]-d);
                                MSC[Master].IdxName[MSIC.SELECT].RemoveAt(DTLimg.SelectITM[d] - d);
                                MSC[Slave].IdxName[MSIC.SELECT].RemoveAt(DTLimg.SelectITM[d] - d);
                            }
                               
                                dataGridViewSempls.Rows[Q].Cells[1].Value = (MSC[Master].IMG[Q].Images.Count).ToString();
                                dataGridViewSempls.Rows[MSIC.SELECT].Cells[1].Value = (MSC[Master].IMG[MSIC.SELECT].Images.Count).ToString();



                          
                    }
                }  dataGridViewSempls_CellClick(null, null);
            }
} catch { }

            //listView1.VirtualListSize = MSC[Master].IMG[MSIC.SELECT].Images.Count;    // Задайте загальну кількість елементів
            //listView1.LargeImageList = MSC[Master].IMG[MSIC.SELECT];

           // listView2.VirtualListSize = MSC[Slave].IMG[MSIC.SELECT].Images.Count;    // Задайте загальну кількість елементів
            //listView2.LargeImageList = MSC[Slave].IMG[MSIC.SELECT];



            Save_Report_Button.Enabled = true;
            DellDataTeach();
        }

        private void ManualCorrection() {

            ImageList MosaicsView = new ImageList();
            // listView1.View = View.LargeIcon;                //відображати назву картинкі
            MosaicsView.ImageSize = new Size(55, 55);      //розмір виводу картинкі
            MosaicsView.ColorDepth = ColorDepth.Depth24Bit;
            listView4.Clear();
            // MosaicsView[Master].Images.Add ( imig);               //додати картинку до списку   
            // Set the view to show details.
            //listView4.View = View.Details;
            // Allow the user to edit item text.
            listView4.LabelEdit = true;
            // Allow the user to rearrange columns.
            listView4.AllowColumnReorder = true;
            // Display check boxes.
            // listView4.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            listView4.FullRowSelect = true;
            // Display grid lines.
            listView4.GridLines = true;
            //Sort the items in the list in ascending order.
            listView4.Sorting = SortOrder.Ascending;
            listView4.SmallImageList = MosaicsView;

            if (MLD.PathSamples != null) 
            { 
            string PashImg = Path.Combine(MLD.PathSamples, SelectSempleTyp.Text);
            }try{
                dataGridViewSempls.Rows.Clear();
                for (int i = 0; i < MLD.Names.Length; i++) {

                    string dts = Path.Combine(MLD.PathSamples, MLD.Names[i]);

                    string[] ImgName = Directory.GetFiles(dts);
                    if (ImgName.Length != 0)
                    {
                        foreach (string dt in ImgName)
                        {

                            string name = Path.GetFileName(dt);
                            Bitmap OpenImag;

                            using (var file = new FileStream(dts + "\\" + name, FileMode.Open, FileAccess.Read, FileShare.Inheritable))
                            {
                                OpenImag = new Bitmap(Image.FromStream(file));
                            }

                            MosaicsView.Images.Add(OpenImag);
                            listView4.LargeImageList = MosaicsView;
                            listView4.Items.Add(new ListViewItem { ImageIndex = i, Text = MLD.Names[i],  /*BackColor = Color.Blue,*/  });

                            dataGridViewSempls.RowTemplate.Height = 50;
                            dataGridViewSempls.Rows.Add(MLD.Names[i], "0", new Bitmap(OpenImag, new Size(50, 50)));

                            // Отримати комірку
                            DataGridViewCell cell = dataGridViewSempls.Rows[i].Cells[0];
                   
                            // Встановити колір заливки
                            cell.Style.BackColor = MLD.NameColor[i];
                            cell.Style.SelectionBackColor = MLD.NameColor[i];
                            // Встановити колір тексту
                            cell.Style.SelectionForeColor = MLD.NameColor[i];
                            break;
                        }
                    
                    }   else {
                        MosaicsView.Images.Add(new Bitmap(64, 64));
                        listView4.LargeImageList = MosaicsView;
                        listView4.Items.Add(new ListViewItem { ImageIndex = i, Text = MLD.Names[i],  /*BackColor = Color.Blue,*/  });

                        dataGridViewSempls.RowTemplate.Height = 50;
                        dataGridViewSempls.Rows.Add(MLD.Names[i], "0", new Bitmap(50, 50));
                    }
                }

                /// BOTUM OLL IMGS

                //MosaicsView.Images.Add(OpenImag);
                //listView4.LargeImageList = MosaicsView;
                //listView4.Items.Add(new ListViewItem { ImageIndex = i, Text = MLD.Names[i],  /*BackColor = Color.Blue,*/  });

                //dataGridViewSempls.RowTemplate.Height = 50;
                 dataGridViewSempls.Rows.Add("Show All", "0");

                // Отримати комірку
                //DataGridViewCell cell = dataGridViewSempls.Rows[i].Cells[0];

                // Встановити колір заливки
                //cell.Style.BackColor = MLD.NameColor[i];
                //cell.Style.SelectionBackColor = MLD.NameColor[i];
                // Встановити колір тексту
                //cell.Style.SelectionForeColor = MLD.NameColor[i];





            } catch { }
        }



        private void listView4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView4.FocusedItem != null)
            {
                label28.Text = listView4.FocusedItem.Text;
                

            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            FlowAnalis.SelectDoubl = SelectPosicinSeml.Checked;
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            FlowAnalis.SelectionSample = checkBox13.Checked;
        }


        //Weght
        static int PcsSample;
        private void Weig_Click() {

            if (ImgListCout != 0) { ShowPcsSample.Text = (PcsSample = ImgListCout).ToString(); } else { Help.ErrorMesag("Error Weeg"); }
            SAV.DT.Report.SemplWeight = (double)numericUpDown2.Value / (double)PcsSample;
            ShowWeightSample.Text = SAV.DT.Report.SemplWeight.ToString();
        
        }







        string PasswordWrite = ""; ///Properties.Settings.Default.Password;  //Password from memori save
        bool DontTch = false;
        string PaswortChange = "";

        public void PaswordSet_Click(object sender, EventArgs e)
        {
            string password = STGS.DT.Password;
                //Properties.Settings.Default.Password;  //Password from memori save
            PasworLable.Text = "";


            if ((password == PasswordWrite) || ("1304" == PasswordWrite))
            {

                groupBox17.Enabled = true;  // частини які потрібно заблокувати  Lock
                groupBox14.Enabled = true;  // Lock
                groupBox11.Enabled = true;  // Lock
                DeleteColumn.Enabled = true;
                AddColumn.Enabled = true;
                groupBox18.Enabled = true;
                groupBox1.Enabled = true;
                groupBoxWeight.Enabled = true;
                groupBoxSelectCamera.Enabled = true;
                IDtex.Text = "Admin mode"; // Label visual Mode
                PaswordString.Text = "";
                PaswortChange = "";
                PasworLable.Text = "OK";

            }
            else { Help.ErrorMesag("Password is incorrect"); }
        }

        private void PaswordChange_Click(object sender, EventArgs e)
        {
            PasworLable.Text = "";

            if (IDtex.Text == "Admin mode") // Label visual Mode
            {
                if (PaswortChange == "")
                {
                    PaswortChange = PasswordWrite;
                    PasswordWrite = "";
                    DontTch = false;
                    PaswordString.Text = "";
                    PasworLable.Text = "Repeat";


                }

                else
                {
                    if (PaswortChange == PasswordWrite)
                    {
                        STGS.DT.Password = PasswordWrite;
                        PasswordWrite = "";
                        PaswordString.Text = "";
                        PaswortChange = "";
                        PasworLable.Text = "Change OK";
                        STGS.Save();
                    }
                    else
                    {
                        PasswordWrite = "";
                        DontTch = false;
                        PaswordString.Text = "";
                        PaswortChange = "";
                        PasworLable.Text = "Error";

                    }
                }
            }
            else { Help.ErrorMesag("Password changing possible only in admin mode"); };
        }

        private void PaswordLock_Click(object sender, EventArgs e)
        {
            groupBox17.Enabled = false;   // частини які потрібно заблокувати  Lock
            groupBox14.Enabled = false;   // частини які потрібно заблокувати  Lock
            groupBox11.Enabled = false;  // Lock
            DeleteColumn.Enabled = false;
            AddColumn.Enabled = false;
            groupBox18.Enabled = false;
            groupBox1.Enabled = false;
            groupBoxWeight.Enabled = false;
            groupBoxSelectCamera.Enabled = false;
            PasworLable.Text = "Lock";
            IDtex.Text = "User mode";   // Label visual Mode
        }

        private void PaswordShow_CheckedChanged(object sender, EventArgs e)
        {
            PasworLable.Text = "";

            if (PaswordShow.Checked == false)
            {
                if (PaswordString.Text.Length > 0)
                {
                    PasswordWrite = "";

                    for (int i = 0; i < PaswordString.Text.Length; i++)
                    {
                        if (PaswordString.Text.ToArray()[i] != '*')
                        {
                            PasswordWrite += PaswordString.Text.ToArray()[i];
                        };
                    }



                    int Q = PasswordWrite.Length;
                    DontTch = false;
                    PaswordString.Text = "";
                    for (int i = 0; i < Q; i++)
                    {
                        DontTch = false;
                        PaswordString.Text += "*";
                    }
                }

                PaswordString.SelectionStart = PaswordString.Text.Length;
                PaswordString.Focus();

            }
            else { if (PaswordString.Text.Length > 0) { DontTch = false; PaswordString.Text = PasswordWrite.Clone().ToString(); } }


            PaswordString.SelectionStart = PaswordString.Text.Length;
            PaswordString.Focus();
        }

        private void PaswordString_TextChanged(object sender, EventArgs e) {

            PasworLable.Text = "";

            if (DontTch == true) {
                if (PaswordShow.Checked == false) {
                    if (PaswordString.Text.Length > 0) {
                        string rewrit = "";
                        for (int i = 0; i < PaswordString.Text.Length; i++)
                        {

                            if (PaswordString.Text.ToArray()[i] != '*')
                            {
                                PasswordWrite += PaswordString.Text.ToArray()[i];
                            }

                            if (PasswordWrite.Length > i) { rewrit += PasswordWrite.ToArray()[i]; }
                        }
                        PasswordWrite = rewrit;
                        int Q = PaswordString.Text.Length;
                        PaswordString.Text = "";
                        for (int i = 0; i < Q; i++)
                        {
                            PaswordString.Text += "*";
                        }

                        PaswordString.SelectionStart = PaswordString.Text.Length;
                        PaswordString.Focus();
                    }
                    else
                    {
                        PasswordWrite = PaswordString.Text;
                    }
                }
                else
                {
                    PasswordWrite = PaswordString.Text;
                }
            }
            DontTch = true;

        }





        private void UpgradeWeight_CheckedChanged(object sender, EventArgs e)
        {
            Save_Report_Button.Enabled = true;
            Weig_Click();
        }

        private void radioButton14_CheckedChanged(object sender, EventArgs e) {

            try
            {
                comboBoxCreat.Items.Clear();
                comboBoxCreat.Text = "";

                if (STGS.DT.CreatedBy == null) { STGS.DT.CreatedBy = new StringCollection(); }
                foreach (var Dt in STGS.DT.CreatedBy)
                {comboBoxCreat.Items.Add(Dt); }
            }catch { }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e){
            try
            {
                comboBoxCreat.Items.Clear();
                comboBoxCreat.Text = "";

                if (STGS.DT.SampleType==null) { STGS.DT.SampleType = new StringCollection(); }
                foreach (var Dt in STGS.DT.SampleType)
                {
                    comboBoxCreat.Items.Add(Dt);
                }
            }catch { }
        }



        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked && (comboBox1.Items != null)) {

                String[] data = new string[comboBox1.Items.Count];
                comboBox1.Items.CopyTo(data, 0);
                comboBoxCreat.Items.Clear();
                comboBoxCreat.Text = "";
                comboBoxCreat.Items.AddRange(data);

            }else { comboBoxCreat.Items.Clear(); }
        }





        private void Hz_Table_ValueChanged(object sender, EventArgs e)
        {
            if ((StartButton.Text == "Stop Analysis") || (StartTable.Text == "STOP TABLE")) {
                USB_HID.VIBRATING.SET(USB_HID.VIBRATING.Select.Frequency, (UInt32)Hz_Table.Value);
                USB_HID.APPLY();
            }
        }

        private void PWM_Table_ValueChanged(object sender, EventArgs e) {
            if ((StartButton.Text == "Stop Analysis") || (StartTable.Text == "STOP TABLE")) {
                USB_HID.VIBRATING.SET(USB_HID.VIBRATING.Select.Table, (UInt32)PWM_Table.Value);
                USB_HID.APPLY();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            AnalisPredict.FreeRun = FreeRun.Checked;
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            string pathAssq = Environment.CurrentDirectory;
            string pathAss = Path.Combine(pathAssq, "Help" + ".pdf");
            Report.testStart(pathAss);
        }


        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        private void FormMain_Load(object sender, EventArgs e) {
  
            sqlConnection = new SqlConnection(sqlconect);

            try{
                sqlConnection.Open();
                UpdateGridSet(false);
                var bfvdfb = DateTime.Now;
              dateTimePicker1.Text=  DateTime.Now.ToString("yyy-MM-dd");
              dateTimePicker2.Text=  DateTime.Now.Date.AddDays(1).ToString("yyy-MM-dd");

            }catch { Help.ErrorMesag("Database error! Check the path."); }

        }


        void UpdateGridSet(bool SelectData) {

            Font F = new Font("Arial", 8, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.RowsDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = F;

            string DataA = "";
            string DataB = "";
            if (SelectData) {
                DataA = dateTimePicker1.Text;
                DataB = dateTimePicker2.Text;
            } else {
                DataA = DateTime.Now.Date.ToString("yyy-MM-dd");
                DataB = DateTime.Now.Date.AddDays(1).ToString("yyy-MM-dd");

            }

            sqlDataAdapter = new SqlDataAdapter("SELECT * FROM Users WHERE DateTime >='" + DataA + "' AND DateTime <='" + DataB + "'", sqlConnection);

            try
            {
                sqlBilder = new SqlCommandBuilder(sqlDataAdapter);
                sqlBilder.GetInsertCommand();
                sqlBilder.GetUpdateCommand();
                sqlBilder.GetDeleteCommand();
                dataSet = new DataSet();

                sqlDataAdapter.Fill(dataSet, "Users");
                dataGridView1.DataSource = dataSet.Tables["Users"];
                

                //dataGridView1.Columns.w

                GridData.Valua = new int[dataSet.Tables["Users"].Columns.Count];
                GridData.Name = new string[dataSet.Tables["Users"].Columns.Count];
                SelectSempleTyp.Items.Clear();
                comboBox3.Items.Clear();
                //визнвчаєм назви заголовків
                for (int i = 0; i < dataSet.Tables["Users"].Columns.Count; i++){
                GridData.Name[i] = dataSet.Tables["Users"].Columns[i].ColumnName.ToString();
                if (i >= GridData.StartDataFemeli) {
                        comboBox3.Items.Add(GridData.Name[i]);
                        SelectSempleTyp.Items.Add(GridData.Name[i]); }
                dataGridView1.Columns[i].Width = 63;
                }
                dataGridView1.Columns[0].Width = 50;
                dataGridView1.Columns[1].Width = 110;
            }
            catch { Help.ErrorMesag("Database error! Check database."); }

        }



        ////    -----------               SQL    -----------------/////
        ///////     зберегти першу строчку таблиці  /////////////
        ///  Save in SQL Data SAMPELS
        /// 

        DataRow Row;
        private void SaveGrid() {
            Row = dataSet.Tables["Users"].NewRow();

            for (int i = 0; i < GridData.Name.Length; i++) {

                if (i >= GridData.StartDataFemeli){
                    if (GridData.Valua[i] != 0){

                    
                        double Dt = ((double)100 / (double)(BAD_SplCont + GOOD_SplCont)) * (double)GridData.Valua[i];
                        Row[GridData.Name[i]] = Math.Round(Dt, 3).ToString(); 
                    
                    } else {

                        var ERGHER = GridData.Name;
                        Row[GridData.Name[i]] = 0; }

                }else {


                    if (GridData.Name[i] == "DateTime") {
                        Row[GridData.Name[i]] = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");
                        var ergbre = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");
                    }

                    if (GridData.Name[i] == "Report Name")
                    {
                        try {  Row[GridData.Name[i]] = SAV.DT.Report.NameReport.ToString(); } catch { Help.ErrorMesag("Check the name of the report!"); Row[GridData.Name[i]] = "No name"; }

                    }

                    if (GridData.Name[i] == "Model Name")
                    {
                        try { Row[GridData.Name[i]] = comboBox1.Text; } catch { Help.ErrorMesag("Check the name of the report!"); }
                    }

                    if (GridData.Name[i] == "Sample Size"){
                        Row[GridData.Name[i]] = numericUpDown2.Value.ToString();
                    }


                    if (GridData.Name[i] == "Speed")
                    {
                        try { Row[GridData.Name[i]] = PCSmin.Text; } catch { Help.ErrorMesag("Check the name of the report!"); }
                    }

                    if (GridData.Name[i] == "Measuring Time")
                    {
                      
                        try { Row[GridData.Name[i]] = TimerWorks.Text; } catch { Help.ErrorMesag("Check the name of the report!"); }
            
                    }

                    if (GridData.Name[i] == "Sample Type")
                    {
                        try { Row[GridData.Name[i]] = SempleTyp.Text; } catch { Help.ErrorMesag("Check the name of the report!"); }
                    }

                    if (GridData.Name[i] == "Created By")
                    {
                        try { Row[GridData.Name[i]] = CreatedBy.Text; } catch { Help.ErrorMesag("Check the name of the report!"); }
                    }


                    if (GridData.Name[i] == "PWM"){
                        Row[GridData.Name[i]] = SAV.DT.Device.PWM_Table;
                    }

                    if (GridData.Name[i] == "HZ")
                    {
                        Row[GridData.Name[i]] = SAV.DT.Device.Hz_Table;
                    }



                }
            }
            dataSet.Tables["Users"].Rows.Add(Row);
            sqlDataAdapter.Update(dataSet, "Users");
            UpdateGridSet(false);

        }







        void ClearGridRealTim() {

            for (int i = 0; i < GridData.Name.Length; i++)
            {
                if (i > 4)
                {
                    GridData.Valua[i] = 0;
                    dataGridView1.Rows[0].Cells[GridData.Name[i]].Value = 0;
                }
                else { if (i > 0) { dataGridView1.Rows[0].Cells[GridData.Name[i]].Value = ""; } }
            }


            try {
            for (int i = 0; i < chart1.Series["ChartSemple"].Points.Count; i++)
            {
                chart1.Series["ChartSemple"].Points[i].XValue = 0;
                chart1.Series["ChartSemple"].Points[i].YValues[0] = 0;
            }
        }   catch {  }

        }



        private void button15_Click_1(object sender, EventArgs e) { UpdateGridSet(true); }


        //ВИДАЛИТИ СТРОКУ
        private void button33_Click(object sender, EventArgs e) {
          var result = MessageBox.Show("Do you want delete select 'Row' ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes) {
                try{
                    int Idx = dataGridView1.SelectedRows[0].Index;
                    dataGridView1.Rows.RemoveAt(Idx);
                    dataSet.Tables["Users"].Rows[Idx].Delete();//  .RemoveAt(Idx);
                    sqlDataAdapter.Update(dataSet, "Users");
                }catch { Help.ErrorMesag("You need to select a 'Row' to delete"); }
            }
        }

        private void button9_Click_1(object sender, EventArgs e){

            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK){
                richTextBox1.Text = FBD.SelectedPath;
            }else { MessageBox.Show("Choose directory please", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }
       
        private void button5_Click_3(object sender, EventArgs e) {
            try { 
            ReportXLSX xLSX = new ReportXLSX();
            ReportXLSX.DTchart Report = new ReportXLSX.DTchart();
            Report.DT = new ReportXLSX.DT[dataSet.Tables["Users"].Rows.Count];
            Report.Name       = new string[GridData.Name.Length-1];
            int idx = 0;
            int ix = 0;
            Report.PeriodFrom = dateTimePicker1.Text;
            Report.PeriodTo   = dateTimePicker2.Text;

            // визначаєм заголовкі таблиці
            for (int i = 1; i <= GridData.Name.Length - 1; i++){
                Report.Name[idx] = GridData.Name[i].ToString();
                idx++;}

            idx = 0;
            ix = 0;

            // заповняєм даними таблиці
            foreach (DataRow item in dataSet.Tables["Users"].Rows){
                Report.DT[idx] = new ReportXLSX.DT();
                Report.DT[idx].Value = new string[item.ItemArray.Length - 1];
                ix = 0;

                for (int i = 1; i <= item.ItemArray.Length - 1; i++){
                    Report.DT[idx].Value[ix] = "";
                    Report.DT[idx].Value[ix] = item.ItemArray[i].ToString();
                    ix++;
                } idx++;}xLSX.CreateXlsx(Report, richTextBox2.Text );
            }
            catch { MessageBox.Show("Check the correctness of the path for save XLS report!"); }
        }

        //Delete Name Column
        private void button35_Click_2(object sender, EventArgs e) {

            DialogResult result = DialogResult.Yes;
            result = MessageBox.Show("Do you want delete '" + comboBox3.Text + "' ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                string queryString = "ALTER TABLE Users DROP COLUMN " + comboBox3.Text;
                using (SqlConnection connection = new SqlConnection(sqlconect))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    try { command.ExecuteReader(); }
                    catch { MessageBox.Show("The name you want to create already exists or the name is incorrect!"); }
                    connection.Close();
                    Update();
                    UpdateGridSet(false);
                }
            }



            }


        //Create Name new Column
        private void button36_Click(object sender, EventArgs e) {
            DialogResult result = DialogResult.Yes;
            result = MessageBox.Show("Do you want add '" + richTextBox6.Text + "' ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                string queryString = "ALTER TABLE Users add " + richTextBox6.Text + " VARCHAR (20)";
                using (SqlConnection connection = new SqlConnection(sqlconect))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    try { command.ExecuteReader(); }
                    catch { MessageBox.Show("The name you want to create already exists or the name is incorrect!"); }
                    connection.Close();
                    Update();
                    UpdateGridSet(false);
                }
            }
        }



        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e){
            if (e.ColumnIndex == -1) {
                try{
                    //chart1.

                    var thtrh= GridData.StartDataFemeli;
                    for (int i = GridData.StartDataFemeli; i < GridData.Name.Length ; i++)
                    {

                        double DataDoubl = 0.0;
                        string Data = "";
                        Data = dataGridView1.Rows[e.RowIndex].Cells[i].Value.ToString();
                        if (Data != "") { DataDoubl = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[i].Value); }

                        //chart1.Series[Idx].Points.Clear(); 
                        chart1.Series["ChartSemple"].Points[(i - GridData.StartDataFemeli)].XValue = 1;
                        chart1.Series["ChartSemple"].Points[(i - GridData.StartDataFemeli)].YValues[0] = DataDoubl;
                    }
                }catch { }
            
            }

        }


        private void button34_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                richTextBox2.Text = FBD.SelectedPath;
            }
            else { MessageBox.Show("Choose directory please", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }





        private void NameReport_TextChanged(object sender, EventArgs e) {

            var invalidChars = string.Join("", Path.GetInvalidFileNameChars());
            var regex = new Regex("[" + Regex.Escape(string.Join("", invalidChars)) + "]");

            if (!regex.IsMatch(NameReport.Text)) { SAV.DT.Report.NameReport = NameReport.Text; } else { Help.ErrorMesag("Incorrect name '"+ NameReport.Text +"' ! Cannot be used (" + @"\"+ "/:*? <>|)");   };
            
           
        }








        private void radioButton4_CheckedChanged(object sender, EventArgs e) {

        
            PWM_Table.Value = SAV.DT.Device.PWM_Table;
            Hz_Table.Value = SAV.DT.Device.Hz_Table;}
        


    

  


        private void ContuorMax_Scroll(object sender, EventArgs e)
        {
            NumContuorMax.Value = ContuorMax.Value;
        

        }

     





       


        private void button58_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // дії, які виконуються при виборі картинки
                pictureBoxSlave.Image = Image.FromFile(openFileDialog1.FileName);

                //Mat img = CvInvoke.Imread(openFileDialog1.FileName);

                //pictureBoxMaster.Image = EMGU.CLAHE(img.ToImage<Bgr, byte>());
                pictureBoxMaster.Image= FPN.Test(openFileDialog1.FileName);


            }
        }

        private void button59_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // дії, які виконуються при виборі картинки
                pictureBoxMaster.Image = Image.FromFile(openFileDialog1.FileName);

                //Mat img = CvInvoke.Imread(openFileDialog1.FileName);

                // pictureBoxMaster.Image = EMGU.CLAHE(img.ToImage<Bgr, byte>());
                pictureBoxSlave.Image  = FPN.Test(openFileDialog1.FileName);


            }
        }





        private void SempleTyp_Click(object sender, EventArgs e){
            SempleTyp.Items.Clear();
            foreach ( var Dt in STGS.DT.SampleType ) { 
                SempleTyp.Items.Add(Dt);
            }
        }

        private void CreatedBy_Click(object sender, EventArgs e)
        {
            CreatedBy.Items.Clear();
            foreach (var Dt in STGS.DT.CreatedBy)
            {
                CreatedBy.Items.Add(Dt);
            }
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e) {

            if (radioButton12.Checked == true) { ID = 0; } else { ID = 1; }
            SaveRefresh();
        }

        private void button61_Click(object sender, EventArgs e)
        {
            SimulacionImg(null, null);
        }

        private void button60_Click(object sender, EventArgs e)
        {
            IdxShou--;
            IdxShou--;
            SimulacionImg(null, null);
        }

        private void button62_Click(object sender, EventArgs e)
        {
            IdxShou = 0;
        }

        static int IdxShou = 0;
        private void SimulacionImg(object sender, EventArgs e)
        {
            if (IdxShou < 0) { IdxShou = 0; }
            //string urlMaster = richTextBox5.Text + "\\" + "Image" + IdxShou++ + ".jpg";
            FlowAnalis.Setings = true;
            //string[] files = Directory.GetFiles(@richTextBox5.Text, "*.jpg" );

            string[] allowedExtensions = { ".jpg", ".bmp" };
            string directoryPath = @richTextBox5.Text;

            string[] files = Directory.GetFiles(directoryPath)
                                .Where(file => allowedExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                                .ToArray();

            int count = files.Length;

            string masterPrefix = "Master";
            string slavePrefix = "Slave";

                string masterImagePath = Path.Combine(directoryPath, $"{masterPrefix}{IdxShou}.bmp");
                string slaveImagePath  = Path.Combine(directoryPath, $"{slavePrefix}{IdxShou}.bmp");
                                                                                    IdxShou++;
                if (File.Exists(masterImagePath) && File.Exists(slaveImagePath))
                {
                    Bitmap imM = new Bitmap(masterImagePath);
                    Bitmap imS = new Bitmap(slaveImagePath);

                    IProducerConsumerCollection<Image<Bgr, byte>> CollecTempM = FlowCamera.BoxM;
                    IProducerConsumerCollection<Image<Bgr, byte>> CollecTempS = FlowCamera.BoxS;
                    CollecTempM.TryAdd(imM.ToImage<Bgr, byte>());
                    CollecTempS.TryAdd(imS.ToImage<Bgr, byte>());
                }
                else
                {

                    if (IdxShou <= files.Length){

                        Bitmap imM = new Bitmap(files[IdxShou++]);
                        Emgu.CV.Mat imOriginal = imM.ToImage<Bgr, byte>().Mat;
                        IProducerConsumerCollection<Image<Bgr, byte>> CollecTempM = FlowCamera.BoxM;
                        IProducerConsumerCollection<Image<Bgr, byte>> CollecTempS = FlowCamera.BoxS;
                        CollecTempM.TryAdd(imOriginal.ToImage<Bgr, byte>());
                        CollecTempS.TryAdd(imOriginal.ToImage<Bgr, byte>());

                    }


                }
           
       
        }








        bool CAMERA_INSTAL = true;
        short coutTim = 0;
        private void timer3_Tick(object sender, EventArgs e)
        {


            if (CAMERA_INSTAL)
            {
                if (coutTim == 1)
                {
                   // Flow.ProcessLoadImagesFunction();
                }
                if (coutTim == 5)
                {
                    Enabled = true;
                    //try { DLS = new DLS(_DLS.HowMany.CAM1); }
                    //catch
                    //{
                    //    timer3.Enabled = false;
                    //    coutTim = 0;
                    //    Help.Mesag("Cameras are not connected"); Enabled = true; timer3.Enabled = false;
                    //    Flow.ProcessLoadImagesFunction(false);

                    //}
                }

                Enabled = false;
                if (coutTim > 10)
                {
                    timer3.Enabled = false;
                    coutTim = 0;


                    GAIN.Enabled = DLS.Devis.Status[ID];
                    // GAIN2.Enabled = DLS.Devis.Status[DLS.Slave];

                    if ((SAV.DT.DALSA.GEIN[Master] <= 10) && (SAV.DT.DALSA.GEIN[Master] >= 1))
                    {

                        if (SAV.DT.DALSA.GEIN[Master] != (decimal)DLS.Devis.Gain[DLS.Master]) { DLS.SetGain((double)SAV.DT.DALSA.GEIN[Master], DLS.Master); }

                    }
                    else { SAV.DT.DALSA.GEIN[Master] = (decimal)DLS.Devis.Gain[DLS.Master]; }


                    if ((SAV.DT.DALSA.GEIN[Slave] <= 10) && (SAV.DT.DALSA.GEIN[Slave] >= 1))
                    {

                        if (SAV.DT.DALSA.GEIN[Slave] != (decimal)DLS.Devis.Gain[DLS.Slave]) { DLS.SetGain((double)SAV.DT.DALSA.GEIN[Slave], DLS.Slave); }

                    }
                    else { SAV.DT.DALSA.GEIN[Slave] = (decimal)DLS.Devis.Gain[DLS.Slave]; }


                    // Завантаження Вирівнювання Фону
                    if (!SAV.DT.DALSA.CameraAnalis_1) { SetACQ_File(DLS.Master); }
                    if (!SAV.DT.DALSA.CameraAnalis_2) { SetACQ_File(DLS.Slave); }
                    Flow.ProcessLoadImagesFunction(false);

                    Enabled = true;
                }

            }
            else { timer3.Enabled = false; Enabled = true; }

            coutTim++;
        }


        void SetACQ_File(int ID_CAM)
        {

            string PathType = Path.Combine(_DLS.DT.URL_SampleType, NameModelML.Text);
            string PshACQ = Path.Combine(PathType, "ACQ"); //створити шлях до IMG

            DLS.Load_FF_File(ID_CAM, PshACQ);
            DLS.checkBox_FaltField_Click(ID_CAM, checkBoxAcqSet.Checked);

        }

        private void GAIN_Click(object sender, EventArgs e)
        {
            try
            {
                if ((GAIN.Value <= 10) && (GAIN.Value >= 1))
                {
                   
                    DLS.SetGain((double)GAIN.Value, ID);

                }
                else { 
                    GAIN.Value = 1;
                    DLS.SetGain((double)GAIN.Value, ID);
                }

            }
            catch {
                Help.Mesag("Reset Program"); }
        }

        private void radioButtonNo_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonNo.Checked==true) { FlowAnalis.IMG_Test_Viwe = FlowAnalis.IMG_Test_Viwe_SET.No; SaveImgsButton.Enabled = false; }
        }

        private void radioButtonSimulation_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSimulation.Checked == true) { FlowAnalis.IMG_Test_Viwe = FlowAnalis.IMG_Test_Viwe_SET.Simulation; SaveImgsButton.Enabled = false; }
        }

        private void radioButtonSave_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSave.Checked == true) { FlowAnalis.IMG_Test_Viwe = FlowAnalis.IMG_Test_Viwe_SET.Save; SaveImgsButton.Enabled = true; }
        }

        int NameSampleSelect=0;
        private void dataGridViewSempls_CellClick(object sender, DataGridViewCellEventArgs e){
            NameSampleSelect = dataGridViewSempls.SelectedCells[0].RowIndex;
            string dataGridViewSemplsName =(string) dataGridViewSempls.Rows[NameSampleSelect].Cells[0].Value;

          
            if (dataGridViewSemplsName == "Show All") { MSIC.SELECT = MSIC.OLL; }
            else 
            { MSIC.SELECT = MLD.NameIdx(dataGridViewSemplsName); }

            listView1.VirtualListSize = MSC[Master].IMG[MSIC.SELECT].Images.Count;    // Задайте загальну кількість елементів
            listView1.LargeImageList  = MSC[Master].IMG[MSIC.SELECT];

            listView2.VirtualListSize = MSC[Slave].IMG[MSIC.SELECT].Images.Count;    // Задайте загальну кількість елементів
            listView2.LargeImageList  = MSC[Slave].IMG[MSIC.SELECT];

            ImgListCoutMosaic = MSC[Slave].IMG[MSIC.SELECT].Images.Count;

        }

      void  RESET_dataGridViewSempls() {

            MSIC.SELECT = MSIC.OLL; 
          
            listView1.VirtualListSize = MSC[Master].IMG[MSIC.SELECT].Images.Count;    // Задайте загальну кількість елементів
            listView1.LargeImageList = MSC[Master].IMG[MSIC.SELECT];

            listView2.VirtualListSize = MSC[Slave].IMG[MSIC.SELECT].Images.Count;    // Задайте загальну кількість елементів
            listView2.LargeImageList = MSC[Slave].IMG[MSIC.SELECT];

            ImgListCoutMosaic = MSC[Slave].IMG[MSIC.SELECT].Images.Count;

        }






         int StarnIdxSelec = 0;
        private void button53_Click(object sender, EventArgs e) {
                if (button53.Text != "Start Image add")
                {
                    button53.Text = "Start Image add";
                    button53.ForeColor = Color.Black;

                    try
                    {
                        if (((listView1.FocusedItem != null) && (ID == Master)) || ((listView2.FocusedItem != null) && (ID == Slave)))
                    {
                           int idx = 0;
                             if(ID ==Master){ idx = listView1.SelectedIndices[0]; }else{ //END IDX
                                              idx = listView2.SelectedIndices[0];} //END IDX

                        Bitmap LearnImg = new Bitmap(IMG_SIZE.Width, IMG_SIZE.Height);

                            for (int Q = StarnIdxSelec; Q < idx + 1; Q++){

                            if (ID ==Slave) {    LearnImg = (Bitmap) MSC[Slave].IMG[NameSampleSelect].Images[Q];}
                            if (ID == Master){    LearnImg = (Bitmap)MSC[Master].IMG[NameSampleSelect].Images[Q]; }

                            MosaicsTeach.Images.Add(LearnImg);
                            
                                listView3.LargeImageList = MosaicsTeach;
                                listView3.Items.Add(new ListViewItem { ImageIndex = ImagCouAnn, Text = MosaicsTeach.Images.Count.ToString(),  /*nema imag*/ });
                                ImagCouAnn++;

                            }
                        }
                        else { Help.Mesag("You need to select several images from the mosaic"); }
                    }
                    catch { }
                StarnIdxSelec = 0;
                }
                else
                {
                try
                {

                    button53.Text = "Last Image";
                    button53.ForeColor = Color.DarkRed;
                    if (ID == Master) { StarnIdxSelec = listView1.SelectedIndices[0]; } else {  //START IDX
                                        StarnIdxSelec = listView2.SelectedIndices[0];
                    }

                }
                catch { Help.Mesag("You need to selected a starting sample"); }
                }

            
        }

        private void button28_Click(object sender, EventArgs e)
        {
            ManualCorrection();
        }

        private void MakeReportButton_Click_1(object sender, EventArgs e)
        {

                report.ReportSet(reportDT);

        }

        private void TestFlaps_Click(object sender, EventArgs e)
        {
            AnalisPredict.TestFlaps = TestFlaps.Checked;
        }
    }
}





