using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Leds;
using System.Threading;
using Meadow.Foundation;

namespace TempDisplayer
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        private TempReadController _temp = new TempReadController();
        private DisplayController _display = new DisplayController();
        private OnBoardLED _onboardLed = new OnBoardLED();

        public MeadowApp()
        {
            while (true)
            {
                _onboardLed.DisplayColor(Color.Red);
                ReadTempAndUpdateDisplay();
                _onboardLed.DisplayColor(Color.Blue);
            }
        }

        private void ReadTempAndUpdateDisplay()
        {
            var task = _temp.ReadTemperature();
            task.Wait();
            _display.UpdateDisplay(task.Result);
        }
    }
}
