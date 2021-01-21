using System;
using System.IO;
using System.Reflection;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Foundation.Displays;
using SimpleJpegDecoder;
using Color = Meadow.Foundation.Color;

namespace TempDisplayer
{
    public class DisplayController
    {
        protected St7789 Display;
        protected GraphicsLibrary Graphics;
        protected AtmosphericConditions Conditions;

        protected bool IsRendering = false;
        protected object RenderLock = new object();

        public DisplayController()
        {
            InitializeDisplay();
        }

        protected void InitializeDisplay()
        {
            var config = new SpiClockConfiguration(12000, SpiClockConfiguration.Mode.Mode3);

            Display = new St7789(device: MeadowApp.Device,
                spiBus: MeadowApp.Device.CreateSpiBus(MeadowApp.Device.Pins.SCK, MeadowApp.Device.Pins.MOSI,
                    MeadowApp.Device.Pins.MISO, config),
                chipSelectPin: null,
                dcPin: MeadowApp.Device.Pins.D01,
                resetPin: MeadowApp.Device.Pins.D00,
                width: 240, height: 240);

            Graphics = new GraphicsLibrary(Display)
            {
                CurrentFont = new Font12x20()
            };

            Graphics.Clear(true);
        }

        public void UpdateDisplay(AtmosphericConditions conditions)
        {
            Conditions = conditions;
            Render();
        }

        protected void Render()
        {
            lock (RenderLock)
            {
                if (IsRendering)
                {
                    Console.WriteLine("Already in render loop");
                    return;
                }

                IsRendering = true;
            }

            Graphics.Clear(false);

            Graphics.Stroke = 1;

            Graphics.DrawRectangle(
                x: 0, y: 0, 
                width: (int) Display.Width - 10, 
                height: (int) Display.Height- 10, 
                Color.White);

            Graphics.DrawCircle( 
                centerX: (int) Display.Width / 2 - 5,
                centerY: (int) Display.Height / 2 - 5,
                radius: (int)(Display.Width/2)-10,
                Color.FromHex("#23abe3"),
                filled: true);

            DisplayJPG(55, 40);

            string text = $"{Conditions.Temperature?.ToString("##.#")}°C";
            Graphics.CurrentFont = new Font12x20();
            Graphics.DrawText(
                x: (int) (Display.Width - text.Length * 24) / 2,
                y: 140,
                text: text,
                color: Color.Black,
                scaleFactor: GraphicsLibrary.ScaleFactor.X2);

            Graphics.Rotation = GraphicsLibrary.RotationType._180Degrees;

            Graphics.Show();

            IsRendering = false;
        }

        protected void DisplayJPG(int x, int y)
        {
            var jpgData = LoadResource("meadow2.jpg");
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            int imageX = 0;
            int imageY = 0;
            byte r, g, b;

            for (int i = 0; i < jpg.Length; i += 3)
            {
                r = jpg[i];
                g = jpg[i + 1];
                b = jpg[i + 2];

                Graphics.DrawPixel(imageX + x, imageY + y, Color.FromRgb(r, g, b));

                imageX++;
                if (imageX % decoder.Width == 0)
                {
                    imageY++;
                    imageX = 0;
                }
            }

            Display.Show();
        }

        protected byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"TempDisplayer.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }

}