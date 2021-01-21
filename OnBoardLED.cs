using Meadow.Foundation;
using Meadow.Foundation.Leds;

namespace TempDisplayer
{
    public class OnBoardLED
    {
        private RgbPwmLed _onBoardLED = new RgbPwmLed(device: MeadowApp.Device,
            redPwmPin: MeadowApp.Device.Pins.OnboardLedRed,
            greenPwmPin: MeadowApp.Device.Pins.OnboardLedGreen,
            bluePwmPin: MeadowApp.Device.Pins.OnboardLedBlue);

        public OnBoardLED()
        {
            
        }

        public void DisplayColor(Color color)
        {
            _onBoardLED.SetColor(color);
        }
    }

}
