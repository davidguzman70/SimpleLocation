﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using PerpetualEngine.Location;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Demo.Droid
{
    [Activity(
        Label = "SimpleLocation.Droid",
        Icon = "@drawable/icon",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        App app;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);
            FormsMaterial.Init(this, savedInstanceState);

            SimpleLocationManager.SetContext(this);
            SimpleLocationManager.HowOftenShowUseLocationDialog = SimpleLocationManager.ShowUseLocationDialog.Once;
            SimpleLocationManager.HandleLocationPermission = true;
            SimpleLocationManager.ShouldShowRequestPermissionRationale = true;

            app = new App();
            app.SimpleLocationManager.LocationUpdatesStarted += () => StartService(new Intent(this, typeof(BackgroundLocationService)));
            app.SimpleLocationManager.LocationUpdatesStopped += () => StopService(new Intent(this, typeof(BackgroundLocationService)));
            app.SimpleLocationManager.ShowRequestPermissionRationale += () => ShowRequestPermissionRationale();

            BackgroundLocationService.SimpleLocationManager = app.SimpleLocationManager;

            LoadApplication(app);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            app.SimpleLocationManager.HandleResultForLocationPermissionRequest(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            app.SimpleLocationManager.HandleResolutionResultForLocationSettings(requestCode, resultCode);

            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void ShowRequestPermissionRationale()
        {
            var alert = new AlertDialog.Builder(this);
            alert.SetTitle("Permission explanation");
            alert.SetMessage("Location permission is necessary to get location information");
            alert.SetPositiveButton("OK", delegate
            {
                app.SimpleLocationManager.RequestPermission();
            });
            alert.Create().Show();
        }
    }
}

