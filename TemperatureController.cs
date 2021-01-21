using System.Threading.Tasks;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace TempDisplayer
{
    public class TempReadController
    {
        protected AnalogTemperature Temperature;

        public TempReadController()
        {
            InitializeTempReader();
        }

        protected void InitializeTempReader()
        {
            Temperature = new AnalogTemperature(
                device: MeadowApp.Device, 
                analogPin: MeadowApp.Device.Pins.A00,
                sensorType: AnalogTemperature.KnownSensorType.LM35);
        }

        public async Task<AtmosphericConditions> ReadTemperature()
        {
            return await Temperature.Read();
        }
    }
}