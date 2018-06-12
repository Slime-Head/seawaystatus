using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.IO;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.System.Threading;
using System.Threading;


namespace SeawayChecker
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;

        private const int GREEN_LED_PIN = 3;
        private const int RED_LED_PIN = 6;
        private GpioPin greenPin;
        private GpioPin redPin;

        private ThreadPoolTimer timer;

        private const string url = "http://www.greatlakes-seaway.com/R2/jsp/mMaiBrdgStatus.jsp?language=E";
        private bool isOpen;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            InitGPIO();
            GetStatus(null);
            timer = ThreadPoolTimer.CreatePeriodicTimer(GetStatus, TimeSpan.FromMinutes(10));
        }

        private void InitGPIO()
        {
            greenPin = GpioController.GetDefault().OpenPin(GREEN_LED_PIN);
            greenPin.Write(GpioPinValue.High); // OFF
            greenPin.SetDriveMode(GpioPinDriveMode.Output);

            redPin = GpioController.GetDefault().OpenPin(RED_LED_PIN);
            redPin.Write(GpioPinValue.High); // OFF
            redPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private async void GetStatus(ThreadPoolTimer timer)
        {
            HttpClient client = new HttpClient();
            string html = await client.GetStringAsync(url);
            html = html.Substring(622); // Remove text before start of html node

            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);
            var htmlNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div/div[2]/ul/li[2]/span[3]/font");
            string availability = htmlNode.InnerText.Trim();

            if (availability == "Available")
                isOpen = true;
            else
                isOpen = false;

            UpdateLEDs();
        }

        private void UpdateLEDs()
        {
            if (isOpen)
            {
                greenPin.Write(GpioPinValue.Low); // Green ON
                redPin.Write(GpioPinValue.High); // Red OFF
            }
            else
            {
                redPin.Write(GpioPinValue.Low); // Green ON
                greenPin.Write(GpioPinValue.High); // Red OFF
            }   
        }
    }
}

