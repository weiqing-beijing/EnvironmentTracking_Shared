using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Net;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EnvironmentTracking
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int DATA_PIN = 24;
        private const int SCK_PIN = 23;

        private GpioPin pin;

        // Timer
        private DispatcherTimer ReadSensorTimer;
        // SHT15 Sensor
        private SHT15 sht15 = null;

        // Sensor values
        public static double TemperatureC = 0.0;
        public static double TemperatureF = 0.0;
        public static double Humidity = 0.0;

        public string sensorID = "RPi2First";
        public string sensorLocation = "Beijing";

        //int counter = 0; // dummy temp counter value;

        int uploadHelper = 0;
        int uploadspac = 10;

        ConnectTheDotsHelper ctdHelper;

        /// <summary>
        /// Main page constructor
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            InitGPIO();

            // Hard coding guid for sensors. Not an issue for this particular application which is meant for testing and demos
            List<ConnectTheDotsSensor> sensors = new List<ConnectTheDotsSensor>
            {
                new ConnectTheDotsSensor(),
            };

            //the below settings need to be changed to your own Azure setting
            ctdHelper = new ConnectTheDotsHelper(
               serviceBusNamespace: "XXX",
               eventHubName: "XXX",
               keyName: "XXX",
               key: "XXX",
               displayName: "YOUR_DEVICE_NAME",
               organization: "YOUR_ORGANIZATION_OR_SELF",
               location: "YOUR_LOCATION",
               sensorList: sensors);

            // Start Timer every 1 seconds
            ReadSensorTimer = new DispatcherTimer();
            ReadSensorTimer.Interval = TimeSpan.FromMilliseconds(1000);
            ReadSensorTimer.Tick += Timer_Tick;
            ReadSensorTimer.Start();

            Unloaded += MainPage_Unloaded;

            InitializeSensor(DATA_PIN, SCK_PIN);

            // Initialize and Start HTTP Server
            HttpServer WebServer = new HttpServer();

            WebServer.RecivedMeg += (meg, eve) =>
            {
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    //tbmeg.Text = meg.ToString();
                }).AsTask();

            };

            var asyncAction = ThreadPool.RunAsync((w) => { WebServer.StartServer(); });
        }


        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                //tbmeg.Text = "There is no GPIO controller on this device.";
                return;
            }


            // Show an error if the pin wasn't initialized properly
            if (pin == null)
            {
                //tbmeg.Text = "There were problems initializing the GPIO pin.";
                return;
            }

            pin.Write(GpioPinValue.High);
            pin.SetDriveMode(GpioPinDriveMode.Output);

            //tbmeg.Text = "GPIO pin initialized correctly.";
        }

        private void MainPage_Unloaded(object sender, object args)
        {
            // Cleanup Sensor
            sht15.Dispose();
        }

        // Timer Ro
        private void Timer_Tick(object sender, object e)
        {
            // Read Raw Temperature and Humidity
            int RawTemperature = sht15.ReadRawTemperature();

            TemperatureC = sht15.CalculateTemperatureC(RawTemperature);
            TemperatureF = sht15.CalculateTemperatureF(RawTemperature);
            Humidity = sht15.ReadHumidity(TemperatureC);

            StringBuilder _temperature = new StringBuilder();
            _temperature.AppendLine(MainPage.TemperatureC.ToString(".00") + "℃");

            StringBuilder _humidity = new StringBuilder();
            _humidity.AppendLine(MainPage.Humidity.ToString(".00") + "%");

            StringBuilder _time = new StringBuilder();
            _time.AppendLine(DateTime.Now.ToString("yyyy.MM.dd" + "  " + "HH:mm:ss"));

            Time.Text = _time.ToString();
            Temp.Text = _temperature.ToString();
            Hum.Text = _humidity.ToString();
            SensorID.Text = sensorID.ToString();


            uploadHelper++;
            if (uploadHelper >= uploadspac)
            {
                ConnectTheDotsSensor sensor = ctdHelper.sensors[0];
                sensor.guid = Guid.NewGuid().ToString();
                sensor.location = sensorLocation;
                sensor.deviceid = sensorID; //sensor id for wq
                sensor.temperatureC = TemperatureC.ToString();
                sensor.temperatureF = TemperatureF.ToString();
                sensor.humidity = Humidity.ToString();
                //upload Data To EventHub
                ctdHelper.SendSensorData(sensor);
                uploadHelper = 0;
            }
         }

        private void InitializeSensor(int datapin, int sckpin)
        {
            sht15 = new SHT15(DATA_PIN, SCK_PIN);
        }

    }

}
