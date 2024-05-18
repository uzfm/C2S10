using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using HidLibrary;
using HidSharp;
using System.Threading;
using System.IO;

using System.Windows.Forms;
using System.Drawing;


namespace MVision
{
    public class USB_HID
    {


        #region CODE

        //Standart
        ushort DEVICE_VID = 1155;
        ushort DEVICE_PID = 22352;

        //Devices
        ushort V1_PID = 22351;
        ushort C1_PID = 22352;
        ushort CMS_PID = 22353;
        ushort GA_PID = 22354;
        ushort GA_V2 = 22355;

        const byte PAGE = 64;
        byte REPORT_ID_READ = 1;
        byte REPORT_ID_WRITE = 2;

        static byte[] Buffer_USB_RX = new byte[PAGE];
        static byte[] Buffer_USB_TX = new byte[PAGE];

        static Byte OUTPUT0_BIT = 0;
        static Byte OUTPUT1_BIT = 0;
        static Byte OUTPUT2_BIT = 0;
        static Byte OUTPUT3_BIT = 0;


        const byte BIT0_RES = 0xFE;
        const byte BIT1_RES = 0xFD;
        const byte BIT2_RES = 0xFB;
        const byte BIT3_RES = 0xF7;
        const byte BIT4_RES = 0xEF;
        const byte BIT5_RES = 0xDF;
        const byte BIT6_RES = 0xBF;
        const byte BIT7_RES = 0x7F;

        const byte BIT0_SET = 0x01;
        const byte BIT1_SET = 0x02;
        const byte BIT2_SET = 0x04;
        const byte BIT3_SET = 0x08;
        const byte BIT4_SET = 0x10;
        const byte BIT5_SET = 0x20;
        const byte BIT6_SET = 0x40;
        const byte BIT7_SET = 0x80;

        public struct BIT
        {
            public const byte I0 = 0x01;
            public const byte I1 = 0x02;
            public const byte I2 = 0x04;
            public const byte I3 = 0x08;
            public const byte I4 = 0x10;
            public const byte I5 = 0x20;
            public const byte I6 = 0x40;
            public const byte I7 = 0x80;
        }

        const Int16 REG_1 = 1;
        const Int16 REG_2 = 2;
        const Int16 REG_3 = 3; //PWM Channels-1
        const Int16 REG_4 = 4; //PWM Channels-1
        const Int16 REG_5 = 5; //PWM Channels-2
        const Int16 REG_6 = 6; //PWM Channels-2
        const Int16 REG_7 = 7; //PWM Channels-3
        const Int16 REG_8 = 8; //PWM Channels-3
        const Int16 REG_9 = 9; //PWM Channels-4
        const Int16 REG_10 = 10; //PWM Channels-4
        const Int16 REG_11 = 11; //PWM Channels-5
        const Int16 REG_12 = 12; //PWM Channels-5
        const Int16 REG_13 = 13; //PWM Channels-6
        const Int16 REG_14 = 14; //PWM Channels-6
        const Int16 REG_15 = 15; //PWM Channels-7
        const Int16 REG_16 = 16; //
        const Int16 REG_17 = 17; //

        const Int16 REG_30 = 30;

        const Int16 REG_31 = 31;
        const Int16 REG_32 = 32; // LED6
        const Int16 REG_33 = 33; // LED5
        const Int16 REG_34 = 34; // LED4
        const Int16 REG_35 = 35; // LED1
        const Int16 REG_36 = 36; // LED2
        const Int16 REG_37 = 37; // LED3
        const Int16 REG_38 = 38;
        const Int16 REG_39 = 39; //IMPUT1
        const Int16 REG_40 = 40; //IMPUT2
        const Int16 REG_41 = 41; //IMPUT3
        const Int16 REG_42 = 42; //IMPUT4
        const Int16 REG_43 = 43; //IMPUT5
        const Int16 REG_44 = 44; //IMPUT6
        const Int16 REG_45 = 45; //Frequency PWM Channels-1
        const Int16 REG_46 = 46; //Frequency PWM Channels-1
        const Int16 REG_47 = 47; //Frequency PWM Channels-4
        const Int16 REG_48 = 48; //Frequency PWM Channels-4


        ///  RSS 485 Motor.....  //// iNTERFICE fO send COMAND   //////////////////////
        const Int16 REG_49 = 49; //Довжина даних

        //Motor
        const Int16 REG_50 = 50; //
        const Int16 REG_51 = 51;
        const Int16 REG_52 = 55;
        const Int16 REG_53 = 57;
        const Int16 REG_54 = 59;
        //---------------------//
        //const Int16 REG_55 = 55;
        // const Int16 REG_56 = 56;
        // const Int16 REG_57 = 57;
        // const Int16 REG_58 = 58;
        const Int16 REG_59 = 59;
        const Int16 REG_60 = 60;
        const Int16 REG_61 = 61;
        const Int16 REG_62 = 62;
        const Int16 REG_63 = 63;
        const Int16 REG_64 = 64;

        const UInt16 ON = 0xFFFF;
        const UInt16 OFF = 0;

        #endregion CODE


        public static DATA_Save Data = new DATA_Save();
        [Serializable()]
        public class DATA_Save
        {
            //для виривнюваня фону
            public decimal PWM_Table { get; set; }
            public decimal Hz_Table { get; set; }
            public decimal Fleps_Time_OFF { get; set; }


            // LIGHT LOCK   -- TOP SPOT BEAK IR---
            public bool Light_Top { get; set; }
            public bool Light_Bottom { get; set; }
            public bool Light_Back { get; set; }
            public bool Light_IR { get; set; }

        }


        public class PLC_C2S10
        {

            public class FLAPS
            {
                /// <summary>
                /// Types of on
                /// </summary>
                public enum Typ { Fps1, Fps2, Fps3, Fps4, Fps5, Fps6, Fps7, Fps8, Fps9, Fps10, Fps11, Fps12, Fps13, Fps14, Fps15, Fps16, Fps17 }

                static Button Flaps1 = new Button();
                static Button Flaps2 = new Button();
                static Button Flaps3 = new Button();
                static Button Flaps4 = new Button();
                static Button Flaps5 = new Button();
                static Button Flaps6 = new Button();
                static Button Flaps7 = new Button();
                static Button Flaps8 = new Button();
                static Button Flaps9 = new Button();
                static Button Flaps10 = new Button();
                static Button Flaps11 = new Button();
                static Button Flaps12 = new Button();
                static Button Flaps13 = new Button();
                static Button Flaps14 = new Button();
                static Button Flaps15 = new Button();
                static Button Flaps16 = new Button();
                static Button Flaps17 = new Button();

                static System.Timers.Timer TimerFlepsColour = new System.Timers.Timer();


                static public void FlepsLightInstal(
            Button Flaps_1, Button Flaps_2, Button Flaps_3, Button Flaps_4,
            Button Flaps_5, Button Flaps_6, Button Flaps_7, Button Flaps_8,
            Button Flaps_9, Button Flaps_10, Button Flaps_11, Button Flaps_12,
            Button Flaps_13, Button Flaps_14, Button Flaps_15, Button Flaps_16, Button Flaps_17
            )
                {
                    Flaps1 = Flaps_1;
                    Flaps2 = Flaps_2;
                    Flaps3 = Flaps_3;
                    Flaps4 = Flaps_4;
                    Flaps5 = Flaps_5;
                    Flaps6 = Flaps_6;
                    Flaps7 = Flaps_7;
                    Flaps8 = Flaps_8;
                    Flaps9 = Flaps_9;
                    Flaps10 = Flaps_10;
                    Flaps11 = Flaps_11;
                    Flaps12 = Flaps_12;
                    Flaps13 = Flaps_13;
                    Flaps14 = Flaps_14;
                    Flaps15 = Flaps_15;
                    Flaps16 = Flaps_16;
                    Flaps17 = Flaps_17;

                    // Встановлення інтервалу в мілісекундах (100 мілісекунд)
                    TimerFlepsColour.Interval = 100;
                    // Додавання обробника події Tick
                    TimerFlepsColour.Elapsed += TimerFlepsColourTick;
                    // Активація таймера
                    TimerFlepsColour.Enabled = true;
                    TimerFlepsColour.Start();

                }



                // Обробник події, який буде викликатися при кожному таймауті
                static private void TimerFlepsColourTick(object sender, EventArgs e)
                {
                    FlepsLightRES();

                }


                static public void FlepsLightSET(Typ FlapsSet)
                {
                    switch (FlapsSet)
                    {
                        case Typ.Fps1: Flaps1.BackColor = Color.Salmon; break;
                        case Typ.Fps2: Flaps2.BackColor = Color.Salmon; break;
                        case Typ.Fps3: Flaps3.BackColor = Color.Salmon; break;
                        case Typ.Fps4: Flaps4.BackColor = Color.Salmon; break;
                        case Typ.Fps5: Flaps5.BackColor = Color.Salmon; break;
                        case Typ.Fps6: Flaps6.BackColor = Color.Salmon; break;
                        case Typ.Fps7: Flaps7.BackColor = Color.Salmon; break;
                        case Typ.Fps8: Flaps8.BackColor = Color.Salmon; break;
                        case Typ.Fps9: Flaps9.BackColor = Color.Salmon; break;
                        case Typ.Fps10: Flaps10.BackColor = Color.Salmon; break;
                        case Typ.Fps11: Flaps11.BackColor = Color.Salmon; break;
                        case Typ.Fps12: Flaps12.BackColor = Color.Salmon; break;
                        case Typ.Fps13: Flaps13.BackColor = Color.Salmon; break;
                        case Typ.Fps14: Flaps14.BackColor = Color.Salmon; break;
                        case Typ.Fps15: Flaps15.BackColor = Color.Salmon; break;
                        case Typ.Fps16: Flaps16.BackColor = Color.Salmon; break;
                        case Typ.Fps17: Flaps17.BackColor = Color.Salmon; break;

                    }
                }

                static public void FlepsLightRES()
                {

                    if ((SetFlapsColourDelay) && (SetFlapsColour))
                    {

                        Flaps1.BackColor = Color.ForestGreen;
                        Flaps2.BackColor = Color.ForestGreen;
                        Flaps3.BackColor = Color.ForestGreen;
                        Flaps4.BackColor = Color.ForestGreen;
                        Flaps5.BackColor = Color.ForestGreen;
                        Flaps6.BackColor = Color.ForestGreen;
                        Flaps7.BackColor = Color.ForestGreen;
                        Flaps8.BackColor = Color.ForestGreen;
                        Flaps9.BackColor = Color.ForestGreen;
                        Flaps10.BackColor = Color.ForestGreen;
                        Flaps11.BackColor = Color.ForestGreen;
                        Flaps12.BackColor = Color.ForestGreen;
                        Flaps13.BackColor = Color.ForestGreen;
                        Flaps14.BackColor = Color.ForestGreen;
                        Flaps15.BackColor = Color.ForestGreen;
                        Flaps16.BackColor = Color.ForestGreen;
                        Flaps17.BackColor = Color.ForestGreen;
                        SetFlapsColourDelay = false;
                        SetFlapsColour = false;
                    }
                    else { if (SetFlapsColour) { SetFlapsColourDelay = true; } }

                }



                static bool SetFlaps = false;
                static bool SetFlapsColour = false;
                static bool SetFlapsColourDelay = false;


                /// <summary>
                ///  Selected fleps to on);
                /// </summary>
                /// <param name="Type"></param>
                /// <param name="Data"></param>
                static public void SET(Typ Type)
                {
                    switch (Type)
                    {
                        case Typ.Fps1: OUTPUT0_BIT |= BIT0_SET; break;
                        case Typ.Fps2: OUTPUT0_BIT |= BIT1_SET; break;
                        case Typ.Fps3: OUTPUT0_BIT |= BIT2_SET; break;
                        case Typ.Fps4: OUTPUT0_BIT |= BIT3_SET; break;
                        case Typ.Fps5: OUTPUT0_BIT |= BIT4_SET; break;
                        case Typ.Fps6: OUTPUT0_BIT |= BIT5_SET; break;
                        case Typ.Fps7: OUTPUT0_BIT |= BIT6_SET; break;
                        case Typ.Fps8: OUTPUT0_BIT |= BIT7_SET; break;
                        case Typ.Fps9: OUTPUT1_BIT |= BIT0_SET; break;
                        case Typ.Fps10: OUTPUT1_BIT |= BIT1_SET; break;
                        case Typ.Fps11: OUTPUT1_BIT |= BIT2_SET; break;
                        case Typ.Fps12: OUTPUT1_BIT |= BIT5_SET; break;
                        case Typ.Fps13: OUTPUT1_BIT |= BIT6_SET; break;
                        case Typ.Fps14: OUTPUT1_BIT |= BIT7_SET; break;
                        case Typ.Fps15: OUTPUT2_BIT |= BIT0_SET; break;
                        case Typ.Fps16: OUTPUT2_BIT |= BIT1_SET; break;
                        case Typ.Fps17: OUTPUT2_BIT |= BIT2_SET; break;
                    }
                    FlepsLightSET(Type);
                    TimerFlepsColour.Start();
                    SetFlaps = true;
                    SetFlapsColour = true;
                }

                static public void SET()
                {

                    RUN();
                }

                static public void SET(Typ Type, bool Set)
                {
                    switch (Type)
                    {
                        case Typ.Fps1: OUTPUT0_BIT |= BIT0_SET; break;
                        case Typ.Fps2: OUTPUT0_BIT |= BIT1_SET; break;
                        case Typ.Fps3: OUTPUT0_BIT |= BIT2_SET; break;
                        case Typ.Fps4: OUTPUT0_BIT |= BIT3_SET; break;
                        case Typ.Fps5: OUTPUT0_BIT |= BIT4_SET; break;
                        case Typ.Fps6: OUTPUT0_BIT |= BIT5_SET; break;
                        case Typ.Fps7: OUTPUT0_BIT |= BIT6_SET; break;
                        case Typ.Fps8: OUTPUT0_BIT |= BIT7_SET; break;
                        case Typ.Fps9: OUTPUT1_BIT |= BIT0_SET; break;
                        case Typ.Fps10: OUTPUT1_BIT |= BIT1_SET; break;
                        case Typ.Fps11: OUTPUT1_BIT |= BIT2_SET; break;
                        //case Typ.Fps12: OUTPUT1_BIT |= BIT3_SET; break;
                        //case Typ.Fps13: OUTPUT1_BIT |= BIT4_SET; break;
                        case Typ.Fps12: OUTPUT1_BIT |= BIT5_SET; break;
                        case Typ.Fps13: OUTPUT1_BIT |= BIT6_SET; break;
                        case Typ.Fps14: OUTPUT1_BIT |= BIT7_SET; break;
                        case Typ.Fps15: OUTPUT2_BIT |= BIT0_SET; break;
                        case Typ.Fps16: OUTPUT2_BIT |= BIT1_SET; break;
                        case Typ.Fps17: OUTPUT2_BIT |= BIT2_SET; break;
                    }
                    FlepsLightSET(Type);
                    TimerFlepsColour.Start();
                    SetFlaps = true;
                    SetFlapsColour = true;
                    if (Set) { RUN(); }

                }

                /// <summary>
                /// Send data to "ON" selected FLEPS
                /// </summary>
                static public void RUN()
                {


                    if (SetFlaps == true)
                    {

                        SetFlaps = false;
                        //Buffer_USB_RX[REG_30] = OUTPUT0_BIT;
                        //Buffer_USB_RX[REG_31] = OUTPUT1_BIT;
                        //Buffer_USB_RX[REG_32] = OUTPUT2_BIT; //32

                        HID_Write();
                        OUTPUT0_BIT = 0;
                        OUTPUT1_BIT = 0;
                        OUTPUT2_BIT &= BIT0_RES;
                        OUTPUT2_BIT &= BIT1_RES;
                        OUTPUT2_BIT &= BIT2_RES;
                        //Buffer_USB_RX[REG_30] = OUTPUT0_BIT;
                        //Buffer_USB_RX[REG_31] = OUTPUT1_BIT;
                        //Buffer_USB_RX[REG_32] = OUTPUT2_BIT; //32


                    }
                }

                /// <summary>
                /// Automatic turn-off time of the flaps
                /// </summary>
                /// <param name="Data"></param>
                static public void Time_OFF(decimal Data)
                {
                    if (Data >= 1)
                    {
                        USB_HID.Data.Fleps_Time_OFF = Data;
                        byte[] ConvArray = new byte[4];
                        ConvArray = BitConverter.GetBytes((int)Data);
                        ConvArray = BitConverter.GetBytes((int)Data);
                        Buffer_USB_RX[27] = ConvArray[0];
                        Buffer_USB_RX[28] = ConvArray[1];
                        HID_Write();

                    }
                }

            }

            public class LIGHT
            {


                // ON - OFF  RED
                static public void RED_ERROR(bool ON_OFF)
                {
                    if (ON_OFF) { OUTPUT3_BIT |= BIT7_SET; } else { OUTPUT3_BIT &= BIT7_RES; }
                    Buffer_USB_RX[REG_33] = OUTPUT3_BIT;
                    HID_Write();
                }

                // ON - OFF  YELLO
                static public void YELLO_ERROR(bool ON_OFF)
                {
                    if (ON_OFF) { OUTPUT3_BIT |= BIT6_SET; } else { OUTPUT3_BIT &= BIT6_RES; }
                    Buffer_USB_RX[REG_33] = OUTPUT3_BIT;
                    HID_Write();
                }

                // ON - OFF  GREEN
                static public void GREEN_ERRO(bool ON_OFF)
                {
                    if (ON_OFF) { OUTPUT3_BIT |= BIT5_SET; } else { OUTPUT3_BIT &= BIT5_RES; }
                    Buffer_USB_RX[REG_33] = OUTPUT3_BIT;
                    HID_Write();
                }


                // ON   SOUND на одну секунду
                static public void SOUND_ERRO(bool ON)
                {
                    if (ON) { OUTPUT3_BIT |= BIT4_SET; } else { OUTPUT3_BIT &= BIT4_RES; }
                    Buffer_USB_RX[REG_33] = OUTPUT3_BIT; //25 OUT
                    HID_Write();
                }


                static public void IR(bool ON)
                {
                    if (ON) { OUTPUT3_BIT |= BIT3_SET; } else { OUTPUT3_BIT &= BIT3_RES; }
                    Buffer_USB_RX[REG_33] = OUTPUT3_BIT; //25 OUT
                    HID_Write();

                }

                static public void Top(bool ON)
                {
                    if (ON) { OUTPUT3_BIT |= BIT2_SET; } else { OUTPUT3_BIT &= BIT2_RES; }
                    Buffer_USB_RX[REG_33] = OUTPUT3_BIT;   //26 OUT
                    HID_Write();
                }

                static public void Bottom(bool ON)
                {
                    if (ON) { OUTPUT3_BIT |= BIT1_SET; } else { OUTPUT3_BIT &= BIT1_RES; }
                    Buffer_USB_RX[REG_33] = OUTPUT3_BIT;   //27 OUT
                    HID_Write();
                }

                static public void Back(bool ON)
                {
                    if (ON) { OUTPUT3_BIT |= BIT0_SET; } else { OUTPUT3_BIT &= BIT0_RES; }
                    Buffer_USB_RX[REG_33] = OUTPUT3_BIT;   //28 OUT
                    HID_Write();
                }



                static public void ON()
                {
                    if (!Data.Light_Back) { OUTPUT3_BIT |= BIT0_SET; }
                    if (!Data.Light_Top) { OUTPUT3_BIT |= BIT1_SET; }
                    if (!Data.Light_Bottom) { OUTPUT3_BIT |= BIT2_SET; }
                    if (!Data.Light_IR) { OUTPUT3_BIT |= BIT3_SET; }


                    // Buffer_USB_RX[REG_33] = OUTPUT3_BIT;   //28 OUT

                    //Buffer_USB_RX[30] = 0;
                    //Buffer_USB_RX[31] = 0;
                    //Buffer_USB_RX[32] &= BIT0_RES;
                    //Buffer_USB_RX[32] &= BIT1_RES;
                    //Buffer_USB_RX[32] &= BIT2_RES;

                    HID_Write();
                }




                static public void OFF()
                {

                    OUTPUT3_BIT &= BIT0_RES;
                    OUTPUT3_BIT &= BIT1_RES;
                    OUTPUT3_BIT &= BIT2_RES;
                    OUTPUT3_BIT &= BIT3_RES;

                    //Buffer_USB_RX[REG_30] = 0;
                    //Buffer_USB_RX[REG_31] = 0;
                    //OUTPUT2_BIT &= BIT0_RES;
                    //OUTPUT2_BIT &= BIT1_RES;
                    //OUTPUT2_BIT &= BIT2_RES;
                    //Buffer_USB_RX[REG_33] = OUTPUT3_BIT;   //28 OUT

                    HID_Write();
                }



            }


            public class CAMERA
            {
                //22 OUT
                static public void OFF()
                {
                    OUTPUT2_BIT &= BIT5_RES;
                    //Buffer_USB_RX[REG_32] = OUTPUT2_BIT;
                    HID_Write();

                }

                static public void ON()
                {
                    OUTPUT2_BIT |= BIT5_SET;
                    //Buffer_USB_RX[REG_32] = OUTPUT2_BIT;
                    HID_Write();
                }



            }


            public class SEPARATOR
            {
                //24 OUT
                static public void OFF()
                {
                    OUTPUT2_BIT &= BIT7_RES;
                    //Buffer_USB_RX[REG_32] = OUTPUT2_BIT;
                    HID_Write();

                }

                static public void ON()
                {
                    OUTPUT2_BIT |= BIT7_SET;
                    //Buffer_USB_RX[REG_32] = OUTPUT2_BIT;
                    HID_Write();
                }

            }


            public class AUTOLOADER
            {  //out 23

                static public void OFF()
                {
                    OUTPUT2_BIT &= BIT6_RES;
                    //Buffer_USB_RX[REG_32] = OUTPUT2_BIT;
                    HID_Write();

                }

                static public void ON()
                {
                    OUTPUT2_BIT |= BIT6_SET;
                    //Buffer_USB_RX[REG_32] = OUTPUT2_BIT;
                    HID_Write();
                }
            }

            public class COOLING
            {  //out 21

                static public void OFF()
                {
                    OUTPUT2_BIT &= BIT4_RES;
                    //Buffer_USB_RX[REG_32] = OUTPUT2_BIT;
                    HID_Write();

                }

                static public void ON()
                {
                    OUTPUT2_BIT |= BIT4_SET;
                    // Buffer_USB_RX[REG_32] = OUTPUT2_BIT;
                    HID_Write();
                }

            }




            public class VIBRATING
            {
                /// <summary>
                /// Types of control
                /// </summary>
                public enum Typ { PWM,/* Table2, Valve, Valve2,*/ Frequency, OFF, ON }

                /// <summary>
                /// Vibrating intensity  0 min - 500 maх ( 0= off ) ( 500= off) ( 255= on);
                /// </summary>
                /// <param name=" VIBRATING"></param>
                /// <returns></returns>
                static public void SET(Typ Type, decimal Data)
                {
                    byte[] ConvArray = new byte[4];
                    ConvArray = BitConverter.GetBytes((Int32)Data);
                    switch (Type)
                    {
                        //** C1 ***//
                        case Typ.PWM: if (Data != 0) { USB_HID.Data.PWM_Table = Data; } ConvArray = BitConverter.GetBytes((Int32)Data); Buffer_USB_RX[9] = ConvArray[0]; Buffer_USB_RX[10] = ConvArray[1]; break;
                        case Typ.Frequency: if (Data != 0) { USB_HID.Data.Hz_Table = Data; } ConvArray = BitConverter.GetBytes((Int32)Data); Buffer_USB_RX[REG_47] = ConvArray[0]; Buffer_USB_RX[REG_48] = ConvArray[1]; break;
                    }
                    HID_Write();
                }



                static public void SET(Typ Type)
                {

                    byte[] ConvArray = new byte[4];
                    Int32 Data = 0;
                    switch (Type)
                    {
                        //** C1 ***//
                        case Typ.OFF: Data = 0; ConvArray = BitConverter.GetBytes((Int32)Data); Buffer_USB_RX[9] = ConvArray[0]; Buffer_USB_RX[10] = ConvArray[1]; break;
                        case Typ.ON:
                            Data = (Int32)USB_HID.Data.PWM_Table; ConvArray = BitConverter.GetBytes((Int32)Data); Buffer_USB_RX[9] = ConvArray[0]; Buffer_USB_RX[10] = ConvArray[1];
                            Data = (Int32)USB_HID.Data.Hz_Table; ConvArray = BitConverter.GetBytes((Int32)Data); Buffer_USB_RX[REG_47] = ConvArray[0]; Buffer_USB_RX[REG_48] = ConvArray[1]; break;
                    }


                    HID_Write();
                }


            }





            void OutputHRD_Set(int OUTPUT_BIT)
            {
                switch (OUTPUT_BIT)
                {
                    //case 15: OUTPUT2_BIT |= BIT0_SET; break; // Flaps 15
                    case 18: OUTPUT2_BIT |= BIT1_SET; break;
                    case 19: OUTPUT2_BIT |= BIT2_SET; break; // Ionizer
                    case 20: OUTPUT2_BIT |= BIT3_SET; break; // 
                    case 21: OUTPUT2_BIT |= BIT4_SET; break; // Cooling
                    case 22: OUTPUT2_BIT |= BIT5_SET; break; // Cameras
                    case 23: OUTPUT2_BIT |= BIT6_SET; break; // Autoloader
                    case 24: OUTPUT2_BIT |= BIT7_SET; break; // Metal separator
                    case 25: OUTPUT3_BIT |= BIT0_SET; break; // Lighte IR
                    case 26: OUTPUT3_BIT |= BIT1_SET; break; // Lighte White
                    case 27: OUTPUT3_BIT |= BIT2_SET; break; // Lighte White
                    case 28: OUTPUT3_BIT |= BIT3_SET; break; // Lighte White
                    case 29: OUTPUT3_BIT |= BIT4_SET; break; // Error sound
                    case 30: OUTPUT3_BIT |= BIT5_SET; break; // Light error green
                    case 31: OUTPUT3_BIT |= BIT6_SET; break; // Light error yellow
                    case 32: OUTPUT3_BIT |= BIT7_SET; break; // Light error red

                }
            }

            void OutputHRD_Res(int OUTPUT_BIT)
            {

                switch (OUTPUT_BIT)
                {
                    //case 15: OUTPUT2_BIT &= BIT0_RES; break;
                    case 18: OUTPUT2_BIT &= BIT1_RES; break; // Lighte
                    case 19: OUTPUT2_BIT &= BIT2_RES; break; // Ionizer
                    case 20: OUTPUT2_BIT &= BIT3_RES; break; // 
                    case 21: OUTPUT2_BIT &= BIT4_RES; break; // Cooling
                    case 22: OUTPUT2_BIT &= BIT5_RES; break; // Cameras
                    case 23: OUTPUT2_BIT &= BIT6_RES; break; // Autoloader
                    case 24: OUTPUT2_BIT &= BIT7_RES; break; // Metal separator
                    case 25: OUTPUT3_BIT &= BIT0_RES; break;
                    case 26: OUTPUT3_BIT &= BIT1_RES; break;
                    case 27: OUTPUT3_BIT &= BIT2_RES; break;
                    case 28: OUTPUT3_BIT &= BIT3_RES; break;
                    case 29: OUTPUT3_BIT &= BIT4_RES; break; // Error sound
                    case 30: OUTPUT3_BIT &= BIT5_RES; break; // Light error green
                    case 31: OUTPUT3_BIT &= BIT6_RES; break; // Light error yellow
                    case 32: OUTPUT3_BIT &= BIT7_RES; break; // Light error red

                }
            }

            //Buffer_USB_RX[REG_30] = OUTPUT0_BIT;
            //Buffer_USB_RX[REG_31] = OUTPUT1_BIT;
            //Buffer_USB_RX[REG_32] = OUTPUT2_BIT;
            //Buffer_USB_RX[REG_33] = OUTPUT3_BIT;




            public class STATUS
            {



                public bool SensorHigh { get; }
                public bool SensorLow { get; }
                public bool Stop { get; }

                public string Door { get; }
                public bool DoorStatus { get; }


                public STATUS()
                {
                    SensorHigh = (((Buffer_USB_TX[30] & BIT.I1)) == 0); // Прочитати значення верхнього сенсора
                    SensorLow = (((byte)(Buffer_USB_TX[30] & BIT.I0)) == 0); // Прочитати значення нижнього сенсора
                    Stop = (((byte)(Buffer_USB_TX[31] & BIT.I4)) != 0); // Прочитати значення STOP

                    DoorStatus = false;
                    Door = "";
                    if (((byte)(Buffer_USB_TX[30] & BIT.I2)) != 0) { DoorStatus = true; Door = Door + "- 1"; }

                    if (((byte)(Buffer_USB_TX[30] & BIT.I3)) != 0) { DoorStatus = true; Door = Door + "- 2"; }

                    if (((byte)(Buffer_USB_TX[30] & BIT.I4)) != 0) { DoorStatus = true; Door = Door + "- 3"; }

                    if (((byte)(Buffer_USB_TX[30] & BIT.I5)) != 0) { DoorStatus = true; Door = Door + "- 4"; }

                    if (((byte)(Buffer_USB_TX[30] & BIT.I6)) != 0) { DoorStatus = true; Door = Door + "- 5"; }

                    if (((byte)(Buffer_USB_TX[30] & BIT.I7)) != 0) { DoorStatus = true; Door = Door + "- 6"; }

                    if (((byte)(Buffer_USB_TX[31] & BIT.I0)) != 0) { DoorStatus = true; Door = Door + "- 7"; }

                    if (((byte)(Buffer_USB_TX[31] & BIT.I1)) != 0) { DoorStatus = true; Door = Door + "- 8"; }




                    if (DoorStatus == true) { string DoorR = "DOOR IS OPEN  "; Door = DoorR + Door; } else { Door = ""; }

                }



            }
























        }









        /*
        public void HID_Send_Comand(Int16 Comand, UInt32 Data)
        {
            byte[] ConvArray = new byte[4];

            ConvArray = BitConverter.GetBytes(Data);



            // OUTPUT ON-OFF 
            // Buffer_USB_RX[REG_30] = 0;// 1. 2. 3. 4. 5. 6. 7. 8
            //Buffer_USB_RX[REG_31] = 0;// 9.10.11.12.13.14.15.16
            // Buffer_USB_RX[REG_32] = 0;//17.18.19.20.21.22.23.24
            //------    COMANDA --------//
            Buffer_USB_RX[REG_40] = 0;
            // MOTOR
            Buffer_USB_RX[50] = 0;  //Comand


            switch (Comand)
            {
                //PWM - OUTPUT Light- vibration //Static
                case REG_1: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[1] = ConvArray[0]; Buffer_USB_RX[2] = ConvArray[1]; break;
                case REG_2: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[3] = ConvArray[0]; Buffer_USB_RX[4] = ConvArray[1]; break;
                case REG_3: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[5] = ConvArray[0]; Buffer_USB_RX[6] = ConvArray[1]; break;
                case REG_4: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[7] = ConvArray[0]; Buffer_USB_RX[8] = ConvArray[1]; break;
                case REG_5: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[9] = ConvArray[0]; Buffer_USB_RX[10] = ConvArray[1]; break;
                case REG_6: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[11] = ConvArray[0]; Buffer_USB_RX[12] = ConvArray[1]; break;
                case REG_7: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[13] = ConvArray[0]; Buffer_USB_RX[14] = ConvArray[1]; break;
                case REG_8: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[15] = ConvArray[0]; Buffer_USB_RX[16] = ConvArray[1]; break;  //16 Byt
                //------------------------------------------------------------------------------------------------------------------------------//
                case REG_9: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[17] = ConvArray[0]; Buffer_USB_RX[18] = ConvArray[1]; break; //--

                // (SENSOR 1) 
                //case REG_20: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[19] = ConvArray[0]; Buffer_USB_RX[20] = ConvArray[1]; break; //--
                //case REG_21: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[19] = ConvArray[0]; Buffer_USB_RX[20] = ConvArray[1]; break; //--
                //case REG_22: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[19] = ConvArray[0]; Buffer_USB_RX[20] = ConvArray[1]; break; //--
                //case REG_23: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[19] = ConvArray[0]; Buffer_USB_RX[20] = ConvArray[1]; break; //--

                //  (SENSOR 2) 
                //case REG_24: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[19] = ConvArray[0]; Buffer_USB_RX[20] = ConvArray[1]; break; //--
                //case REG_25: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[19] = ConvArray[0]; Buffer_USB_RX[20] = ConvArray[1]; break; //--
                //case REG_26: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[19] = ConvArray[0]; Buffer_USB_RX[20] = ConvArray[1]; break; //--
                //case REG_27: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[19] = ConvArray[0]; Buffer_USB_RX[20] = ConvArray[1]; break; //--


                //TIMER (OUTPUT ON-OFF) //Static
                case REG_14: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[27] = ConvArray[0]; Buffer_USB_RX[28] = ConvArray[1]; break;
                case REG_15: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[29] = ConvArray[0]; Buffer_USB_RX[30] = ConvArray[1]; break;

                //Frequency PWM Channels1 //Static
                case REG_16: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[REG_45] = ConvArray[0]; Buffer_USB_RX[REG_46] = ConvArray[1]; break;
                //Frequency PWM Channels2
                case REG_17: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[REG_47] = ConvArray[0]; Buffer_USB_RX[REG_48] = ConvArray[1]; break;



                // OUTPUT ON-OFF 
                case REG_30: Buffer_USB_RX[REG_30] = Convert.ToByte(OUTPUT0_BIT); break;// 1. 2. 3. 4. 5. 6. 7. 8
                case REG_31: Buffer_USB_RX[REG_31] = Convert.ToByte(OUTPUT1_BIT); break;// 9.10.11.12.13.14.15.16
                case REG_32: Buffer_USB_RX[REG_32] = Convert.ToByte(OUTPUT2_BIT); break;//17.18.19.20.21.22.23.24
                case REG_33: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[REG_33] = Convert.ToByte(OUTPUT3_BIT); break;
                case REG_34: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[REG_34] = ConvArray[0]; break;



                //------    COMANDA --------//
                case REG_35: Buffer_USB_RX[REG_37] = ConvArray[0]; break;
                case REG_36: Buffer_USB_RX[REG_37] = ConvArray[0]; break;
                case REG_37: Buffer_USB_RX[REG_37] = ConvArray[0]; break;
                case REG_38: Buffer_USB_RX[REG_38] = ConvArray[0]; break;
                case REG_39: Buffer_USB_RX[REG_39] = ConvArray[0]; break;
                case REG_40: Buffer_USB_RX[REG_40] = ConvArray[0]; break;
                case REG_41: Buffer_USB_RX[REG_41] = ConvArray[0]; break;
                case REG_42: Buffer_USB_RX[REG_42] = ConvArray[0]; break;
                case REG_43: Buffer_USB_RX[REG_43] = ConvArray[0]; break;
                case REG_44: Buffer_USB_RX[REG_44] = ConvArray[0]; break;
                case REG_47: Buffer_USB_RX[REG_47] = ConvArray[0]; break;
                case REG_48: Buffer_USB_RX[REG_48] = ConvArray[0]; break;
                case REG_49: Buffer_USB_RX[REG_49] = ConvArray[0]; break;



                //MOTOR
                case REG_50:
                    //Buffer_USB_RX[50] = ConvArray[0];  //Comand
                    //ConvArray = BitConverter.GetBytes(Convert.ToInt32(numericUpDown11.Value));
                    //Buffer_USB_RX[51] = ConvArray[0];
                    //Buffer_USB_RX[52] = ConvArray[1];
                    //Buffer_USB_RX[53] = ConvArray[2];
                    //Buffer_USB_RX[54] = ConvArray[3];       //send position
                    //if (numericUpDown12.Value > numericUpDown13.Value) { numericUpDown12.Value = numericUpDown13.Value; }
                    //ConvArray = BitConverter.GetBytes(Convert.ToInt32(numericUpDown12.Value));
                    //Buffer_USB_RX[55] = ConvArray[0];
                    //Buffer_USB_RX[56] = ConvArray[1];       //SPID_MAX
                    //if (numericUpDown13.Value < numericUpDown12.Value) { numericUpDown13.Value = numericUpDown12.Value; }
                    //ConvArray = BitConverter.GetBytes(Convert.ToInt32(numericUpDown13.Value));
                    //Buffer_USB_RX[57] = ConvArray[0];
                    //Buffer_USB_RX[58] = ConvArray[1];       //SPID_MIN
                    //ConvArray = BitConverter.GetBytes(Convert.ToInt32(numericUpDown14.Value));
                    //Buffer_USB_RX[59] = ConvArray[0];
                    //Buffer_USB_RX[60] = ConvArray[1];       //acceleration //deacceleration
                    //Buffer_USB_RX[61] = Convert.ToByte(comboBox1.Text); //ID MOTOR
           

                    break;

            }

            HID_Write();
        }
*/
        /// <summary>
        /// Timer Refresh
        /// </summary>
        /// <param name="TimerRef"></param>
        public static void TIMERREF(System.Windows.Forms.Timer TimerRef)
        {
            TinerRef = TimerRef;
            TinerRef.Tick += new System.EventHandler(timer_Tick);
            TinerRef.Interval = 100;
            TinerRef.Enabled = true;
            TinerRef.Start();
        }

        /// <summary>
        /// Timer Refresh
        /// </summary>
        /// <param name="TimerRef"></param>
        public static void TIMERREF()
        {
            TinerRef.Tick += new System.EventHandler(timer_Tick);
            TinerRef.Interval = 100;
            TinerRef.Enabled = true;
            TinerRef.Start();
        }


        private static System.Windows.Forms.Timer TinerRef = new System.Windows.Forms.Timer();
        static private void timer_Tick(object sender, EventArgs e)
        {

            // SENSOR.Refresh_Data();
            // POCICION_READ_UPDATE
            MOTOR.Position_Update();
            SENSOR_TEMP.Refresh_Data();

        }






        //  static private SaveBin SaveBin = new SaveBin();


        static public void APPLY()
        {
            HID_Write();
        }



        public class FLAPS
        {

            /// <summary>
            /// Types of on
            /// </summary>
            public enum Select { Fps1, Fps2, Fps3, Fps4, Fps5, Fps6, Fps7, Fps8, Fps9, Fps10, Fps11, Fps12, Fps13, Fps14, Fps15, Fps16, Fps17, Fps18, Fps19, Fps20 }

            /// <summary>
            ///  Selected fleps to on);
            /// </summary>
            /// <param name="Type"></param>
            /// <param name="Data"></param>
            static public void SET(Select Type)
            {
                switch (Type)
                {
                    case Select.Fps1: OUTPUT0_BIT |= BIT0_SET; break;
                    case Select.Fps2: OUTPUT0_BIT |= BIT1_SET; break;
                    case Select.Fps3: OUTPUT0_BIT |= BIT2_SET; break;
                    case Select.Fps4: OUTPUT0_BIT |= BIT3_SET; break;
                    case Select.Fps5: OUTPUT0_BIT |= BIT4_SET; break;
                    case Select.Fps6: OUTPUT0_BIT |= BIT5_SET; break;
                    case Select.Fps7: OUTPUT0_BIT |= BIT6_SET; break;
                    case Select.Fps8: OUTPUT0_BIT |= BIT7_SET; break;

                    case Select.Fps9: OUTPUT1_BIT |= BIT0_SET; break;
                    case Select.Fps10: OUTPUT1_BIT |= BIT1_SET; break;
                    case Select.Fps11: OUTPUT1_BIT |= BIT2_SET; break;
                    case Select.Fps12: OUTPUT1_BIT |= BIT5_SET; break;
                    case Select.Fps13: OUTPUT1_BIT |= BIT6_SET; break;
                    case Select.Fps14: OUTPUT1_BIT |= BIT7_SET; break;

                    case Select.Fps15: OUTPUT2_BIT |= BIT0_SET; break;
                    case Select.Fps16: OUTPUT2_BIT |= BIT1_SET; break;
                    case Select.Fps17: OUTPUT2_BIT |= BIT2_SET; break;
                    case Select.Fps18: OUTPUT2_BIT |= BIT3_SET; break;
                    case Select.Fps19: OUTPUT2_BIT |= BIT4_SET; break;
                    case Select.Fps20: OUTPUT2_BIT |= BIT5_SET; break;




                }
            }

            /// <summary>
            /// Send data to "ON" selected FLEPS
            /// </summary>
            static public void APPLY()
            {
                if ((OUTPUT0_BIT != 0) || (OUTPUT1_BIT != 0) || (OUTPUT2_BIT != 0))
                {
                    Buffer_USB_RX[REG_30] = OUTPUT0_BIT;
                    Buffer_USB_RX[REG_31] = OUTPUT1_BIT;
                    Buffer_USB_RX[REG_32] = OUTPUT2_BIT;
                    HID_Write();
                    OUTPUT0_BIT = 0;
                    OUTPUT1_BIT = 0;
                    OUTPUT2_BIT = 0;
                }
            }

            /// <summary>
            /// Automatic turn-off time of the flaps
            /// </summary>
            /// <param name="Data"></param>
            static public void Time_OFF(UInt32 Data)
            {
                byte[] ConvArray = new byte[4];
                ConvArray = BitConverter.GetBytes(Data);
                ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[27] = ConvArray[0]; Buffer_USB_RX[28] = ConvArray[1];
                HID_Write();
            }



        }
        public class LIGHT
        {
            /// <summary>
            /// Types of light to control
            /// </summary>
            public enum Select { Top, Back, Front, Spot, Frequency }

            /// <summary>
            /// Set Light intensity  0 min - 500 maх ( 0= off ) ( 500= on);
            /// </summary>
            /// <param name="ID_Sensor"></param>
            /// <returns></returns>
            static public void SET()
            {

                Buffer_USB_RX[33] |= BIT0_SET;
                HID_Write();
            }


            static public void RES()
            {

                Buffer_USB_RX[33] &= BIT0_RES;
                HID_Write();
            }
        }

        public class FAN
        {

            /// <summary>
            /// Set Fan ( on off );
            /// </summary>
            /// <param name="ID_Sensor"></param>
            /// <returns></returns>

            static public void SET()
            {
                Buffer_USB_RX[33] |= BIT1_SET;
                HID_Write();
            }


            static public void RES()
            {
                Buffer_USB_RX[33] &= BIT1_RES;
                HID_Write();
            }
        }



        public class VIBRATING
        {
            /// <summary>
            /// Types of control
            /// </summary>
            public enum Select { Table, Table2, Valve, Valve2, Frequency }

            /// <summary>
            /// Vibrating intensity  0 min - 500 maх ( 0= off ) ( 500= off) ( 255= on);
            /// </summary>
            /// <param name="ID_Sensor"></param>
            /// <returns></returns>
            static public void SET(Select Type, UInt32 Data)
            {

                byte[] ConvArray = new byte[4];
                ConvArray = BitConverter.GetBytes(Data);
                switch (Type)
                {
                    case Select.Table: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[9] = ConvArray[0]; Buffer_USB_RX[10] = ConvArray[1]; break;
                    case Select.Table2: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[11] = ConvArray[0]; Buffer_USB_RX[12] = ConvArray[1]; break;
                    case Select.Valve: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[13] = ConvArray[0]; Buffer_USB_RX[14] = ConvArray[1]; break;
                    case Select.Valve2: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[15] = ConvArray[0]; Buffer_USB_RX[16] = ConvArray[1]; break;
                    case Select.Frequency: ConvArray = BitConverter.GetBytes(Data); Buffer_USB_RX[REG_47] = ConvArray[0]; Buffer_USB_RX[REG_48] = ConvArray[1]; break;   //Frequency PWM Channels2 //Static

                }
                //HID_Write();
            }
        }

        public class MOTOR
        {
            SAV Save = new SAV();

            public MOTOR()
            {



                //if (  false != Save.Deserialize (Path.Combine(STGS.DT.URL_Models, STGS.DT.Name_Model))  ) {

                //     Data = SAV.DT.Device.MotorDT;  }


                // else
                // {

                //     for (int i = 0; i < 2; i++)
                //     {

                //         Data[i].PositionRead = 0; ;
                //         Data[MotorID].PositionSet = 0;
                //         Data[i].SPID_MAX = 0; ;
                //         Data[i].SPID_MIN = 0; ;
                //         Data[i].Acceleration = 0; ;
                //         Data[i].ID = 0;
                //     }
                // }



            }

            public enum COMAND : byte
            {
                JPUS = 1,
                STOP = 2,
                JMUS = 3,
                ABORT = 4,
                INC = 5,
                ABS = 6,
                POWER_OFF = 8,
                POWER_ON = 7,
                POSIT_RES = 9

            };


            //public enum STATUS  {
            //    ACCEL,
            //    DECEL,
            //    RUN,
            //    STOP
            //};

            static string[] STATUS = new string[] { "ACCEL", "DECEL", "RUN", "STOP", "LimitP", "LimitM" };





            /// <summary>
            /// LIST update Windows Form
            /// </summary>
            ///example(
            /// USB_HID.MOTOR.LIST.POCICION_READ = numericUpDown15;
            /// USB_HID.MOTOR.LIST.POCICION_SET = numericUpDown11;
            /// USB_HID.MOTOR.LIST.SPID_MAX = numericUpDown12;
            /// USB_HID.MOTOR.LIST.SPID_MIN = numericUpDown13;
            /// USB_HID.MOTOR.LIST.ACCLER = numericUpDown14;  ) 
            public struct LIST
            {
                static public System.Windows.Forms.NumericUpDown SPID_MAX
                {
                    set
                    {
                        SpidMax = value;
                        SpidMax.Value = Convert.ToDecimal(SAV.DT.Motor[SAV.DT.Motor[0].ID].SPID_MAX);
                        _SPID_MAX = SAV.DT.Motor[SAV.DT.Motor[MotorID].ID].SPID_MAX;

                    }
                }
                static public System.Windows.Forms.NumericUpDown SPID_MIN
                {
                    set
                    {
                        SpidMin = value;
                        SpidMin.Value = Convert.ToDecimal(SAV.DT.Motor[SAV.DT.Motor[0].ID].SPID_MIN);
                        _SPID_MIN = SAV.DT.Motor[SAV.DT.Motor[MotorID].ID].SPID_MIN;

                    }
                }
                static public System.Windows.Forms.NumericUpDown ACCLER
                {
                    set
                    {
                        Acceleration = value;
                        Acceleration.Value = Convert.ToDecimal(SAV.DT.Motor[SAV.DT.Motor[0].ID].Acceleration);
                        _Acceleration = SAV.DT.Motor[SAV.DT.Motor[MotorID].ID].Acceleration;

                    }
                }
                static public System.Windows.Forms.NumericUpDown POCICION_SET
                {
                    set
                    {
                        PosicionSet = value;
                        ///PosicionSet.Value = Convert.ToDecimal(SAV.DT.Motor[SAV.DT.Motor[0].ID].PositionSet);
                        _Position = (int)PosicionSet.Value;

                    }
                }
                static public System.Windows.Forms.NumericUpDown POCICION_READ
                {
                    set
                    {
                        PosicionRead = value;
                        PosicionRead.Value = Convert.ToDecimal(SAV.DT.Motor[MotorID].PositionRead);


                    }
                }

                static public System.Windows.Forms.Label STATUS
                {
                    set
                    {

                        StatusMotor = value;


                    }
                }




            }




            static System.Windows.Forms.NumericUpDown PosicionRead = new System.Windows.Forms.NumericUpDown();
            static System.Windows.Forms.NumericUpDown PosicionSet = new System.Windows.Forms.NumericUpDown();
            static System.Windows.Forms.NumericUpDown SpidMax = new System.Windows.Forms.NumericUpDown();
            static System.Windows.Forms.NumericUpDown SpidMin = new System.Windows.Forms.NumericUpDown();
            static System.Windows.Forms.NumericUpDown Acceleration = new System.Windows.Forms.NumericUpDown();
            static System.Windows.Forms.Label StatusMotor = new System.Windows.Forms.Label();




            static byte MotorID = 0;


            static public void SetMotorID(byte ID)
            {
                MotorID = ID;
                // PosicionSet.Value  = (decimal)((double) SAV.DT.Motor[ID].PositionSet / Step_mm); 


            }


            [Serializable()]
            public struct DT
            {

                public Int32 PositionSet;
                public Int32 PositionRead;
                public Int32 SPID_MAX;
                public Int32 SPID_MIN;
                public Int32 Acceleration;
                public Int32 ID;

            }






            /// <summary>
            /// function run motor
            /// (Read  Pothucion)
            ///  </summary>
            /// <param name="CMD">comand </param> 
            /// <param name="ID">id motors.</param> 
            public static int PosicionMotorSet(int ID)
            {

                return SAV.DT.Motor[ID].PositionSet;
            }



            /// <summary>
            /// function run motor
            /// (Set  ZERO Posicion )
            ///  </summary>
            /// <param name="CMD">comand </param> 
            /// <param name="ID">id motors.</param> 
            public static void POSITION_RES()
            {
                _COMAND_SET = (byte)COMAND.POSIT_RES;
                HID_Write();
            }



            /// <summary>
            /// function run motor
            /// (Set  POWER MOTOR )
            ///  </summary>
            /// <param name="CMD">comand </param> 
            /// <param name="ID">id motors.</param> 
            public static void POWER_ON()
            {
                _COMAND_SET = (byte)COMAND.POWER_ON;
                HID_Write();
            }

            public static void POWER_OFF()
            {
                _COMAND_SET = (byte)COMAND.POWER_OFF;
                HID_Write();
            }


            /// <summary>
            /// function run motor
            /// (read parameter from  LIST System.Windows.Forms.NumericUpDown)
            ///  </summary>
            /// <param name="CMD">comand </param> 
            /// <param name="ID">id motors.</param> 
            public static void SET(COMAND CMD, byte ID)
            {
                MotorID = ID;
                _COMAND_SET = (byte)CMD;
                _Position = (int)PosicionSet.Value;
                _SPID_MAX = (int)SpidMax.Value;
                _SPID_MIN = (int)SpidMin.Value;
                _Acceleration = (int)Acceleration.Value;
                _ID = MotorID;
                HID_Write();
            }

            /// <summary>
            /// function run motor
            /// </summary>
            /// <param name="CMD"></param> comand
            /// <param name="ID"></param> id motors
            /// <param name="Position"></param> position motor in step
            /// <param name="SPID_MAX"></param> spid max axl deaxl
            /// <param name="SPID_MIN"></param> spid min axl deaxl
            /// <param name="Acceleration"></param> axl
            public static void SET(COMAND CMD, int ID, int Position, int SPID_MAX, int SPID_MIN, int Acceleration)
            {
                MotorID = (byte)ID;
                _COMAND_SET = (byte)CMD;
                _Position = Position;
                _SPID_MAX = SPID_MAX;
                _SPID_MIN = SPID_MIN;
                _Acceleration = Acceleration;
                _ID = ID;
                HID_Write();

            }

            /// <summary>
            /// function run motor
            /// </summary>
            /// <param name="CMD"></param> comand
            /// <param name="ID"></param> id motors
            /// <param name="SPID_MAX"></param> spid max axl deaxl
            /// <param name="SPID_MIN"></param> spid min axl deaxl
            /// <param name="Acceleration"></param> axl
            public static void SET(COMAND CMD, int ID, int SPID_MAX, int SPID_MIN, int Acceleration)
            {
                MotorID = (byte)ID;
                _COMAND_SET = (byte)CMD;
                _Position = (int)((double)PosicionSet.Value * Step_mm);
                SAV.DT.Motor[MotorID].PositionSet = (int)((double)PosicionSet.Value * Step_mm);
                _SPID_MAX = SPID_MAX;
                _SPID_MIN = SPID_MIN;
                _Acceleration = Acceleration;
                _ID = ID;
                HID_Write();
            }


            /// <summary>
            /// Called to update the position of the motor in steps (automatically updates the indicators on the form)
            /// </summary>
            /// <returns></returns>
            /// 

            // double OborotMotor = 200;
            // double StepDriver  = 16;
            // double StepSkru = 2;

            public static double Step_mm = ((200 * 16) / 2);

            static bool ResetLimitTrek = false;
            static public int Position_Update()
            {
                try
                {
                    if (MotorID == 0) { double Value = BitConverter.ToInt32(Buffer_USB_TX, 52); PosicionRead.Value = (decimal)((Value / Step_mm)); }
                    if (MotorID == 1) { double Value = BitConverter.ToInt32(Buffer_USB_TX, 56); PosicionRead.Value = (decimal)((Value / Step_mm)); }
                    if (MotorID == 2) { double Value = BitConverter.ToInt32(Buffer_USB_TX, 60); PosicionRead.Value = (decimal)((Value / Step_mm)); }
                    if (MotorID == 3) { double Value = BitConverter.ToInt32(Buffer_USB_TX, 48); PosicionRead.Value = (decimal)((Value / Step_mm)); }
                    StatusMotor.Text = STATUS[Buffer_USB_TX[47]];
                    //Auto posicion zero
                    if ((StatusMotor.Text == "LimitM") && (ResetLimitTrek == false)) { ResetLimitTrek = true; POSITION_RES(); } else { if (StatusMotor.Text != "LimitM") { ResetLimitTrek = false; } }
                }
                catch { };

                return (int)PosicionRead.Value;
            }

            static byte _COMAND_SET
            {
                set { Buffer_USB_RX[50] = value; }
            }
            static int _Position
            {
                set
                {
                    var ConvArray = BitConverter.GetBytes(Convert.ToInt32(value));
                    Buffer_USB_RX[51] = ConvArray[0];
                    Buffer_USB_RX[52] = ConvArray[1];
                    Buffer_USB_RX[53] = ConvArray[2];
                    Buffer_USB_RX[54] = ConvArray[3];       //send position

                }
            }
            static int _SPID_MAX
            {
                set
                {
                    var ConvArray = BitConverter.GetBytes(Convert.ToInt32(value));
                    Buffer_USB_RX[55] = ConvArray[0];
                    Buffer_USB_RX[56] = ConvArray[1];


                }
            }
            static int _SPID_MIN
            {
                set
                {
                    var ConvArray = BitConverter.GetBytes(Convert.ToInt32(value));
                    Buffer_USB_RX[57] = ConvArray[0];
                    Buffer_USB_RX[58] = ConvArray[1];


                }
            }
            static int _Acceleration
            {
                set
                {
                    var ConvArray = BitConverter.GetBytes(Convert.ToInt32(value));
                    Buffer_USB_RX[59] = ConvArray[0];
                    Buffer_USB_RX[60] = ConvArray[1];


                }
            }
            static int _ID
            {
                set
                {
                    var ConvArray = BitConverter.GetBytes(Convert.ToInt32(value));
                    Buffer_USB_RX[61] = ConvArray[0];
                }
            }

        }

        public class RS_485
        {

            const int WRITE = 20;
            const int READ = 20;
            const int CMD = 50;
            const int DATA = 51;
            const int Length = 49;

            private static byte[] Buffer_RS_485;
            public static bool Read_Status = false;


            static public byte[] READ_DATA
            {
                get
                {
                    Read_Status = false;
                    return Buffer_RS_485;
                }
            }


            static public bool REFRESH_DATA
            {
                get
                {
                    byte idx = 0;
                    if (Buffer_USB_TX[CMD] == READ)
                    {
                        Buffer_RS_485 = new byte[Buffer_USB_TX[Length]];
                        while ((Buffer_USB_TX[Length] > idx) || (idx > 12)) { Buffer_RS_485[idx] = Buffer_USB_TX[DATA + idx]; idx++; }
                        Read_Status = true;
                        Buffer_USB_RX[CMD] = 200; //NULL
                        HID_Write();
                        return true;

                    }; return false;
                }
            }

            static public byte[] SET
            {
                set
                {
                    Buffer_USB_RX[50] = WRITE;
                    byte idx = 0;
                    while ((value.Length > idx) || (idx > 12)) { Buffer_USB_RX[51 + idx] = value[idx]; idx++; }
                    Buffer_USB_RX[49] = idx;
                    HID_Write();
                }
            }


        }
        public class SENSOR
        {
            SAV Save = new SAV();

            public SENSOR()
            { }

            static byte[] START_DATA = new byte[2] { 20, 25 };


            [Serializable()]
            public struct SensorDT
            {
                public double[] Tara;
                public bool[] TaraSet;
                public double[] Unit;
                public double[] CalibrationConst;
                public Int32[] DataInt;
            }
            /////  static SensorDT SAV.DT.SensorsDT = new SensorDT();

            static System.Windows.Forms.NumericUpDown SensorData1 = new System.Windows.Forms.NumericUpDown();
            static System.Windows.Forms.NumericUpDown SensorData2 = new System.Windows.Forms.NumericUpDown();
            static System.Windows.Forms.NumericUpDown SensorData11 = new System.Windows.Forms.NumericUpDown();
            static System.Windows.Forms.NumericUpDown SensorData22 = new System.Windows.Forms.NumericUpDown();

            static public System.Windows.Forms.NumericUpDown SENSOR1_unit
            { get { return SensorData1; } set { SensorData1 = value; } }

            static public System.Windows.Forms.NumericUpDown SENSOR2_unit
            { get { return SensorData2; } set { SensorData2 = value; } }


            static public System.Windows.Forms.NumericUpDown SENSOR1_Analog
            { get { return SensorData11; } set { SensorData11 = value; } }
            static public System.Windows.Forms.NumericUpDown SENSOR2_Analog
            { get { return SensorData22; } set { SensorData22 = value; } }


            static public void Refresh_Data()
            {
                //Masa signal
                SensorData1.Value = Convert.ToDecimal(READ_DATA(0));
                SensorData2.Value = Convert.ToDecimal(READ_DATA(1));
                //Analog signal
                SensorData11.Value = Convert.ToDecimal(AnalogData[0]);
                SensorData22.Value = Convert.ToDecimal(AnalogData[1]);
            }

            static public double READ_DATA(byte ID_Sensor)
            {
                byte[] Buffer_Data = new byte[4];
                Buffer_Data[0] = Buffer_USB_TX[START_DATA[ID_Sensor]];
                Buffer_Data[1] = Buffer_USB_TX[START_DATA[ID_Sensor] + 1];
                Buffer_Data[2] = Buffer_USB_TX[START_DATA[ID_Sensor] + 2];
                Buffer_Data[3] = Buffer_USB_TX[START_DATA[ID_Sensor] + 3];
                SAV.DT.SensorsDT.DataInt[ID_Sensor] = BitConverter.ToInt32(Buffer_Data, 0);

                if (SAV.DT.SensorsDT.TaraSet[ID_Sensor])
                { SAV.DT.SensorsDT.Tara[ID_Sensor] = SAV.DT.SensorsDT.DataInt[ID_Sensor]; SAV.DT.SensorsDT.TaraSet[ID_Sensor] = false; }

                double Weight = (SAV.DT.SensorsDT.DataInt[ID_Sensor] - SAV.DT.SensorsDT.Tara[ID_Sensor]) / SAV.DT.SensorsDT.Unit[ID_Sensor];
                if (double.IsNaN(Weight)) { return 0.000; };
                if (double.IsInfinity(Weight)) { return 0.000; };
                return Weight; ;

            }

            /// <summary>
            ///(Sensor1 = 0 )
            ///(Sensor2 = 1 )
            /// </summary>
            public byte Tara { set { SAV.DT.SensorsDT.TaraSet[value] = true; } }

            public void CALIBRATION(byte ID_Sensor, double value)
            {
                SAV.DT.SensorsDT.CalibrationConst[ID_Sensor] = value;
                SAV.DT.SensorsDT.Unit[ID_Sensor] = (SAV.DT.SensorsDT.DataInt[ID_Sensor] - SAV.DT.SensorsDT.Tara[ID_Sensor]) / SAV.DT.SensorsDT.CalibrationConst[ID_Sensor];
            }

            static public Int32[] AnalogData { get { return SAV.DT.SensorsDT.DataInt; } }



        }

        public class SENSOR_TEMP
        {

            static byte[] START_DATA = new byte[2] { 30, 31 };


            static System.Windows.Forms.NumericUpDown SensorTmp1 = new System.Windows.Forms.NumericUpDown();
            static System.Windows.Forms.NumericUpDown SensorTmp2 = new System.Windows.Forms.NumericUpDown();

            /// <summary>
            /// System.Windows.Forms.NumericUpDown SENSOR1_TEMP1
            /// </summary>
            static public System.Windows.Forms.NumericUpDown SENSOR1_TEMP1
            { get { return SensorTmp1; } set { SensorTmp1 = value; } }

            /// <summary>
            /// System.Windows.Forms.NumericUpDown SENSOR1_TEMP2
            /// </summary>
            static public System.Windows.Forms.NumericUpDown SENSOR1_TEMP2
            { get { return SensorTmp2; } set { SensorTmp2 = value; } }


            /// <summary>
            /// Refresh data in /* System.Windows.Forms.NumericUpDown SensorTmp1 - SensorTmp2 */ 
            /// </summary>

            static public void Refresh_Data()
            {
                /// Temperature C`
                SensorTmp1.Value = READ_DATA(Select.Sensor1);
                SensorTmp2.Value = READ_DATA(Select.Sensor2);

            }

            public enum Select { Sensor1 = 1, Sensor2 = 2 }

            /// <summary>
            /// Read Data from  sensor ID = 1 or 2;
            /// </summary>
            /// <param name="ID_Sensor"></param>
            /// <returns></returns>
            static public short READ_DATA(Select ID_Sensor)
            {
                try
                {
                    if (ID_Sensor == Select.Sensor1) { return Buffer_USB_TX[START_DATA[0]]; }
                    if (ID_Sensor == Select.Sensor2) { return Buffer_USB_TX[START_DATA[1]]; }
                }
                catch { };

                return 0;
            }
        }




        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>

        /************************  Core 5.0 ******************************************/
        public static bool HidStatus;
        static HidStream HIDstream;
        static readonly HidDeviceLoader HidLoader = new HidDeviceLoader();

        public void HIDinst()
        {
            RefreshConnection();
            DeviceList.Local.Changed += Local_Changed;
        }

        void RefreshConnection()
        {
            //var Devise = DeviceList.Local.GetHidDevices();
            if (HidLoader.GetDeviceOrDefault(DEVICE_VID, DEVICE_PID) != null)
            {
                device = HidLoader.GetDeviceOrDefault(DEVICE_VID, DEVICE_PID);
                HIDstream = device.Open();
                HidStatus = true;
            }
            else { HidStatus = false; }
        }



        private void Local_Changed(object sender, DeviceListChangedEventArgs e)
        {
            InstalDevice("V2S10");
        }


        public string[] InstalDevice(string NemaDevice)
        {
            const int LengTyp = 5;
            devices = new HidDevice[LengTyp];
            int idx = 0;
            int LengTypXXX = 0;
            devices[0] = HidLoader.GetDeviceOrDefault(DEVICE_VID, V1_PID);
            devices[1] = HidLoader.GetDeviceOrDefault(DEVICE_VID, C1_PID);
            devices[2] = HidLoader.GetDeviceOrDefault(DEVICE_VID, CMS_PID);
            devices[3] = HidLoader.GetDeviceOrDefault(DEVICE_VID, GA_PID);
            devices[4] = HidLoader.GetDeviceOrDefault(DEVICE_VID, GA_V2);
            while (idx < LengTyp) { if (devices[idx++] != null) { LengTypXXX++; } }
            string[] StringDevice = new string[LengTypXXX];
            idx = 0;
            if ((devices[4] != null) && (NemaDevice == "V2")) { StringDevice[idx] = "V2"; idx++; device = devices[4]; HIDstream = device.Open(); HidStatus = true; }
            else
            {
                if ((devices[3] != null) && (NemaDevice == "GLA")) { StringDevice[idx] = "GLA"; idx++; device = devices[3]; HIDstream = device.Open(); HidStatus = true; }
                else
                {
                    if ((devices[2] != null) && (NemaDevice == "CMS")) { StringDevice[idx] = "CMS"; idx++; device = devices[2]; HIDstream = device.Open(); HidStatus = true; }
                    else
                    {
                        if ((devices[1] != null) && (NemaDevice == "C1")) { StringDevice[idx] = "C1"; idx++; device = devices[1]; HIDstream = device.Open(); HidStatus = true; }
                        else
                        {

                            if ((devices[1] != null) && (NemaDevice == "V2S10")) { StringDevice[idx] = "V2S10"; idx++; device = devices[1]; HIDstream = device.Open(); HidStatus = true; }
                            else
                            {

                                if ((devices[0] != null) && (NemaDevice == "V1")) { StringDevice[idx] = "V1"; idx++; device = devices[0]; HIDstream = device.Open(); HidStatus = true; }
                                else
                                {
                                    HidStatus = false; return null;
                                }
                            }
                        }
                    }
                }
            }

            DeviceList.Local.Changed += Local_Changed;
            Buffer_USB_RX = new byte[PAGE];
            HID_Write();
            return StringDevice;

        }




        private static async void HID_Write()
        {
            Buffer_USB_RX[0] = (byte)2;
            try
            {
                if (HidStatus == true)
                {
                    await HIDstream.WriteAsync(Buffer_USB_RX, 0, 64);
                }
                else { if (DLS.DLS_HowManyCAMERAS != _DLS.HowMany.NO) { Help.ErrorMesag("device is disconnected"); } }
            }
            catch
            {
                Help.ErrorMesag("device problem");
            }

            Buffer_USB_RX[30] = 0;
            Buffer_USB_RX[31] = 0;
            Buffer_USB_RX[32] = 0;
        }


        private static async void HID_WriteBuf(byte[] Bufer)
        {
            Bufer[0] = (byte)2;
            // Random.NextBytes(Bufer);
            if (HidStatus == true)
            {
                await HIDstream.WriteAsync(Bufer, 0, 64);
            }
            else { if (DLS.DLS_HowManyCAMERAS != _DLS.HowMany.NO) { Help.ErrorMesag("device is disconnected"); } }
            Buffer_USB_RX[30] = 0;
            Buffer_USB_RX[31] = 0;

        }

        //==================================================  USB =========================================================================================================
        public static bool CONECT;
        private static HidDevice device;
        private static HidDevice[] devices;
        private const int ReportLength = 64;




        ///Включити Подію читання USB
        public static void HID_Read()
        {

            try
            {
                // Асинхронне читання з потоку
                if (Buffer_USB_TX == null) { Buffer_USB_TX = new byte[device.GetMaxInputReportLength()]; }
                CancellationTokenSource cts = new CancellationTokenSource();
            }
            catch { };

            while (Flow.PotocStartHID && HidStatus)
            {
                try
                {
                    // Читання з HID-потоку
                    int bytesRead = HIDstream.Read(Buffer_USB_TX);


                }
                catch
                {
                    // Закриття потоку та пристрою
                    //   HIDstream.Close();

                }

            }



        }
























    }
}
