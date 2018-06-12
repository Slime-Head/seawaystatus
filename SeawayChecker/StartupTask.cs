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
        private GpioPinValue value = GpioPinValue.High;
        private const int GREEN_LED_PIN = 3;
        private const int RED_LED_PIN = 5;
        private GpioPin greenPin;
        private GpioPin redPin;
        private ThreadPoolTimer timer;
        private bool isOpened;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            InitGPIO();

            timer = ThreadPoolTimer.CreatePeriodicTimer(GetStatus, TimeSpan.FromMinutes(10));
        }

        private void InitGPIO()
        {
            greenPin = GpioController.GetDefault().OpenPin(GREEN_LED_PIN);
            greenPin.Write(GpioPinValue.High);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);

            redPin = GpioController.GetDefault().OpenPin(RED_LED_PIN);
            redPin.Write(GpioPinValue.Low);
            redPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private async void GetStatus(ThreadPoolTimer timer)
        {
            HttpClient client = new HttpClient();
            string url = "http://www.greatlakes-seaway.com/R2/jsp/mMaiBrdgStatus.jsp?language=E";
            HttpResponseMessage response = await client.GetAsync(url);
            string html = response.Content.ToString();
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);
            var htmlNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div/div[2]/ul/li[2]/span[3]/font");
            if (htmlNode.InnerText.Trim() == "Available")
                isOpened = true;
            else
                isOpened = false;
            UpdateLEDs();
        }

        private void UpdateLEDs()
        {
            if (isOpened)
            {
                greenPin.Write(GpioPinValue.High);
                redPin.Write(GpioPinValue.Low);
            }
            else
            {
                redPin.Write(GpioPinValue.High);
                greenPin.Write(GpioPinValue.Low);
            }
                
        }
    }
}

