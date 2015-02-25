using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Core;
namespace K2.LegoRobot
{
    public class K2D2
    {

        public static Brick brick;
        public static string _ControllerFQDN;
        public static int _BallDistance;

        //public async static void MoveMe()
        //{
        //    Brick brick = new Brick(new K2.LegoRobot.UsbCommunication());
        //    brick.BrickChanged +=brick_BrickChanged;
        //    await brick.ConnectAsync();
        //    await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B, 50);

        //}




        public async static void  StartWifi(string IPAddress,string ControllerFQDN)
        {
            string Message = string.Empty;
            try
            {
                brick = new Brick(new K2.LegoRobot.NetworkCommunication(IPAddress));
                brick.BrickChanged += brick_BrickChanged;

                _ControllerFQDN = ControllerFQDN;

                //Setting the sensors

                brick.Ports[InputPort.One].SetMode(TouchMode.Touch);
                brick.Ports[InputPort.Three].SetMode(InfraredMode.Proximity);
                brick.Ports[InputPort.Two].SetMode(ColorMode.Color);



                //Building the event handlers for the sensors
                brick.Ports[InputPort.One].PropertyChanged += K2D2_Touch_PropertyChanged;
                brick.Ports[InputPort.Three].PropertyChanged += K2D2_Red_PropertyChanged;
                brick.Ports[InputPort.Two].PropertyChanged += K2D2_Colour_PropertyChanged;


                await brick.ConnectAsync();
                await brick.DirectCommand.SetLedPatternAsync(LedPattern.GreenPulse);

                Message = "I am Connected";
            }
            catch (Exception ex)
            {
                Message = "Help, I can't connect" + ex.Message.ToString();
            }

           
        }

        public async static void Disconnect()
        {
            await brick.DirectCommand.ClearAllDevicesAsync();
             brick.Disconnect();
        }

        public async static void brick_BrickChanged(object sender, BrickChangedEventArgs e)
        {

        }

        public async static void K2D2_Colour_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (brick.Ports[InputPort.Two].SIValue < 20) //Max 100 = 70cm
            {
                //SensorLog log = new SensorLog();
                //log.Sensor = "Colour";
                //log.SensorValue = brick.Ports[InputPort.Two].SIValue.ToString(); 
                //log.Create();

                //Found Something Red so going to do something!!
                if (brick.Ports[InputPort.Two].SIValue == 5)
                {
                    brick.BatchCommand.PlaySound(100, "../prjs/myapp/K2D2Laugh");
                    brick.BatchCommand.SetLedPattern(LedPattern.GreenFlash);
                    await brick.BatchCommand.SendCommandAsync();

                    //K2D2Communication.SendMessage("K2D2: Red Ball dectected", _ControllerFQDN);
                    //records the distance to the red ball
                    _BallDistance = Convert.ToInt32(brick.Ports[InputPort.Three].SIValue);
                    MoveForwards(50);
                
                }

            }


        }


        /// <summary>
        /// Checks the proximity to an object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void K2D2_Red_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            try
            {
                if (brick.Ports[InputPort.Three].SIValue > 0)
                {
                    //SensorLog log = new SensorLog();
                    //log.Sensor = "Proximity";
                    //log.SensorValue = brick.Ports[InputPort.Three].SIValue.ToString();
                    //log.Create();
                }
            }
            catch (Exception ex)
            { 
            
            }
            
        
        }

        //Response to touch Sensor
        public async static void K2D2_Touch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //1 touch button is pressed
            if (brick.Ports[InputPort.One].SIValue == 1)
            {
                try
                {
                    brick.BatchCommand.PlaySound(100, "../prjs/myapp/K2D2Laugh");
                    brick.BatchCommand.SetLedPattern(LedPattern.RedPulse);
                    await brick.BatchCommand.SendCommandAsync();
                    MoveBackwards(50);

                    //SensorLog log = new SensorLog();
                    //log.Sensor = "Touch";
                    //log.SensorValue = brick.Ports[InputPort.One].SIValue.ToString();
                    //log.Create();
                }
                catch (Exception ex)
                { 
                
                }
            
            }
        }
       
        public async static void StartBlueTooth(string port)
        {
            try
            {
                brick = new Brick(new K2.LegoRobot.BluetoothCommunication(port));
              

                brick.Ports[InputPort.One].SetMode(TouchMode.Touch);
                brick.Ports[InputPort.Three].SetMode(InfraredMode.Proximity);
                brick.Ports[InputPort.Two].SetMode(ColorMode.Color);



                //Building the event handlers for the sensors
                brick.Ports[InputPort.One].PropertyChanged += K2D2_Touch_PropertyChanged;
                brick.Ports[InputPort.Three].PropertyChanged += K2D2_Red_PropertyChanged;
                brick.Ports[InputPort.Two].PropertyChanged += K2D2_Colour_PropertyChanged;


                await brick.ConnectAsync();
                await brick.DirectCommand.SetLedPatternAsync(LedPattern.GreenPulse);

            }
            catch (Exception ex)
            { 
            
            }
        }

        public async static void Start()
        {
            try
            {
                brick = new Brick(new K2.LegoRobot.UsbCommunication());

                brick.Ports[InputPort.One].SetMode(TouchMode.Touch);
                brick.Ports[InputPort.Three].SetMode(InfraredMode.Proximity);
                brick.Ports[InputPort.Two].SetMode(ColorMode.Color);



                //Building the event handlers for the sensors
                brick.Ports[InputPort.One].PropertyChanged += K2D2_Touch_PropertyChanged;
                brick.Ports[InputPort.Three].PropertyChanged += K2D2_Red_PropertyChanged;
                brick.Ports[InputPort.Two].PropertyChanged += K2D2_Colour_PropertyChanged;


                await brick.ConnectAsync();
                await brick.DirectCommand.SetLedPatternAsync(LedPattern.GreenPulse);

            }
            catch (Exception ex)
            { 
            
            }
        }
 

        public async static void MoveBackwards(int Power)
        {
            try
            {
                brick.BatchCommand.TurnMotorAtSpeedForTime(OutputPort.B, Power, 1000, false);
                brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, Power, 1000, false);

                brick.BatchCommand.PlayTone(50, 1000, 500);
                await brick.BatchCommand.SendCommandAsync();
            }
            catch (Exception ex)
            { }
        }

        public async static void MoveForwards(int Power)
        {
            try
            {
                int power = (Power - (Power * 2));
                int x = -Power;

                brick.BatchCommand.TurnMotorAtSpeedForTime(OutputPort.B, power, 1000, false);
                brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, power, 1000, false);
                brick.BatchCommand.PlayTone(50, 1000, 500);
                await brick.BatchCommand.SendCommandAsync();
            }
            catch (Exception ex)
            { }
        }


        public async static void TurnLeft()
        {

            try
            {

                await brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B, 50, 1000, false);

            }
            catch (Exception ex)
            { 
            
            }

        }


        public async static void TurnRight()
        {

            try
            {

                await brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.C, 50, 1000, false);
            }
            catch (Exception ex)
            { }


        }

        public async static void TurnHeadLeft(int Power)
        {
            try
            {
                await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.A, Power);
            }
            catch (Exception ex)
            { 
            }
        }

        public async static void PlaySound()
        {
            //await brick.SystemCommand.CopyFileAsync("K2D2Laugh.rsf", "../prjs/myapp/K2D2Laugh.rsf");
            try
            {
                await brick.DirectCommand.PlaySoundAsyncInternal(100, "../prjs/myapp/K2D2Laugh");
            }
            catch (Exception ex)
            { 
            }
        
        }

     
        /// <summary>
        /// Displays a message on the screen
        /// </summary>
        /// <param name="Message"></param>
        public async void DisplayMessage(string Message)
        {
            try
            {
                Lights(LedPattern.OrangePulse);
                await brick.DirectCommand.DrawTextAsync(Color.Foreground, 0, 0, Message);
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// Displays a image on the screen
        /// </summary>
        /// <param name="ImageName"></param>
        public async void DisplayImage(string ImageName)
        {
            try
            {
                await brick.DirectCommand.DrawImageAsync(Color.Foreground, 0, 0, "../prjs/myapp/" + ImageName);
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="SoundName"></param>
        public async static void PlaySound(string SoundName)
        {
            //await brick.SystemCommand.CopyFileAsync("K2D2Laugh.rsf", "../prjs/myapp/K2D2Laugh.rsf");
            try
            {
                await brick.DirectCommand.PlaySoundAsyncInternal(100, "../prjs/myapp/" + SoundName);
            }
            catch
            { }
        }


        /// <summary>
        /// Controls pattern of Leds
        /// </summary>
        /// <param name="led"></param>
        public async static void Lights(LedPattern led)
        {
            try
            {
                await brick.DirectCommand.SetLedPatternAsync(led);
            }
            catch(Exception ex)
            { }
            
        }


       

    }
}