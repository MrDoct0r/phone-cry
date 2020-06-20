using Android.App;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using Java.Lang;
using System.Timers;

namespace PhoneCry
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        AccelerometerReader accelerometerReader = new AccelerometerReader();
        private MediaPlayer mp = new MediaPlayer();
        private int rollingTime = 0;
        private readonly int timing = 300;
        private bool firstRoll = true;
        private int nbClashes = 0;
        private int globalTimer = 0;
        private bool playFirstWell = true;
        private int lastWell = 0;
        private int lastEventTime = 0;

        private RadioGroup rgVoice;
        private RadioButton rbChecked;

        private double inclinaisonX,
                       inclinaisonY,
                       inclinaisonZ;
        TextView tvValue;

        private bool isStarted = false;

        public static readonly int[] voiceShort = new int[10]
        {
            Resource.Raw.Voice01_01,
            Resource.Raw.Voice01_02,
            Resource.Raw.Voice01_03,
            Resource.Raw.Voice01_04,
            Resource.Raw.Voice01_05,
            Resource.Raw.Voice01_06,
            Resource.Raw.Voice01_07,
            Resource.Raw.Voice01_08,
            Resource.Raw.Voice01_09,
            Resource.Raw.Voice01_10
        };

        public static readonly int[] voiceLong = new int[10]
        {
            Resource.Raw.Voice02_01,
            Resource.Raw.Voice02_02,
            Resource.Raw.Voice02_03,
            Resource.Raw.Voice02_04,
            Resource.Raw.Voice02_05,
            Resource.Raw.Voice02_06,
            Resource.Raw.Voice02_07,
            Resource.Raw.Voice02_08,
            Resource.Raw.Voice02_09,
            Resource.Raw.Voice02_10
        };

        public static readonly int[] voiceOff = new int[10]
        {
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        };

        private int[] selectedVoice = voiceShort;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // ---- UI ----
            tvValue = FindViewById<TextView>(Resource.Id.tvValue);
            rgVoice = FindViewById<RadioGroup>(Resource.Id.rgVoice);

            rgVoice.CheckedChange += RgVoice_CheckedChange;

            accelerometerReader.ToggleAccelerometer();
            play();
        }

        private void RgVoice_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            rbChecked = FindViewById<RadioButton>(rgVoice.CheckedRadioButtonId);
            switch (rbChecked.Text)
            {
                case "Voice 01":
                    selectedVoice = voiceShort;
                    break;
                case "Voice 02":
                    selectedVoice = voiceLong;
                    break;
                case "Voice off":
                    selectedVoice = voiceOff;
                    break;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void play()
        {
            Timer Timer1 = new Timer();
            Timer1.Start();
            Timer1.Interval = timing;
            Timer1.Enabled = true;
            Timer1.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                RunOnUiThread(() =>
                {
                    globalTimer += timing;
                    checkData();
                });
            };
        }

        private void checkData()
        {
            double x = accelerometerReader.valX;
            double y = accelerometerReader.valY;
            double z = accelerometerReader.valZ;
            double norm_Of_g = Math.Sqrt(x * x + y * y + z * z);

            x /= norm_Of_g;
            y /= norm_Of_g;
            z /= norm_Of_g;

            inclinaisonX = (int)Math.Round(Math.ToDegrees(Math.Acos(x)));
            inclinaisonY = (int)Math.Round(Math.ToDegrees(Math.Acos(y)));
            inclinaisonZ = (int)Math.Round(Math.ToDegrees(Math.Acos(z)));

            tvValue.Text = $"x : {inclinaisonX} - y : {inclinaisonY} - z : {inclinaisonZ}";

            if (!isStarted)
            {
                mustStarted();
            }
            // On attends que le jeu ait commencé depuis au moins 4 secondes avant de traiter quoi que ce soit
            // pour être sûr de laisser le temps de remettre le téléphone à plat
            else if (globalTimer >= 4000)
            {
                checkRoll();
                // checkClash();
                checkWell();
            }
            if (globalTimer >= 36000)
            {

            }
        }

        private void mustStarted()
        {
            if (inclinaisonZ > 5)
            {
                isStarted = true;
                playSound(selectedVoice[0]);
            }
        }

        private void playSound(int sound)
        {
            if (!mp.IsPlaying && sound != 0)
            {
                mp = MediaPlayer.Create(this, sound);
                mp.Start();
            }
        }

        private void checkRoll()
        {
            if (inclinaisonX <= 75 || inclinaisonY >= 100 || inclinaisonY <= 80 || inclinaisonX >= 105)
            {
                rollingTime += timing;
                lastEventTime = globalTimer;
                if (rollingTime < 3000 && firstRoll)
                {
                    playSound(Resource.Raw.Voice01_02);
                    playSound(selectedVoice[1]);
                    firstRoll = false;
                }
                else if (rollingTime < 3000)
                {
                    playSound(selectedVoice[3]);
                }
                else
                {
                    playSound(selectedVoice[2]);
                }
            }
            else
            {
                rollingTime = 0;
            }
        }

        private void checkClash()
        {
            if (nbClashes == 0)
            {
                playSound(selectedVoice[4]);
                nbClashes++;
            }
            else if (nbClashes == 1)
            {
                playSound(selectedVoice[5]);
                nbClashes++;
            }
            else
            {
                playSound(selectedVoice[6]);
                nbClashes++;
            }
        }

        private void checkWell()
        {
            if (lastWell != lastEventTime)
            {
                int delayWithoutProblem = globalTimer - lastEventTime;
                if (delayWithoutProblem >= 5000 && playFirstWell)
                {
                    playSound(selectedVoice[7]);
                    playFirstWell = !playFirstWell;
                    lastEventTime = globalTimer;
                    lastWell = globalTimer;
                }
                else if (delayWithoutProblem >= 10000 && !playFirstWell)
                {
                    playSound(selectedVoice[8]);
                    playFirstWell = !playFirstWell;
                    lastEventTime = globalTimer;
                    lastWell = globalTimer;
                }
            }
        }


    }
}