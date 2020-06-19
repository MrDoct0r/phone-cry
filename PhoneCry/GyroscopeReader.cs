using System;
using Xamarin.Essentials;

namespace PhoneCry
{
    public class GyroscopeReader
    {
        SensorSpeed speed = SensorSpeed.UI;

        public float valX = 0;
        public float valY = 0;
        public float valZ = 0;

        public GyroscopeReader()
        {
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
        }

        void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            GyroscopeData data = e.Reading;
            valX = data.AngularVelocity.X;
            valY = data.AngularVelocity.Y;
            valZ = data.AngularVelocity.Z;
        }

        public void ToggleGyroscope()
        {
            try
            {
                if (Gyroscope.IsMonitoring)
                    Gyroscope.Stop();
                else
                    Gyroscope.Start(speed);
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