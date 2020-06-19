using System;
using Xamarin.Essentials;

namespace PhoneCry
{
    class AccelerometerReader
    {
        SensorSpeed speed = SensorSpeed.UI;

        public float valX = 0;
        public float valY = 0;
        public float valZ = 0;

        public AccelerometerReader()
        {
            // Register for reading changes, be sure to unsubscribe when finished
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }

        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            AccelerometerData data = e.Reading;
            valX = data.Acceleration.X;
            valY = data.Acceleration.Y;
            valZ = data.Acceleration.Z;
        }

        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
                else
                    Accelerometer.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred
            }
        }
    }
}