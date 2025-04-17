using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using Android.Bluetooth;
using Android.Content;
using System.Collections.Generic;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace DronApp1
{

    //public class BluetoothScanner
    //{

    //    private BluetoothAdapter _adapter = null;
    //    private BluetoothSocket _device = null;

    //    private List<string> _devices = new List<string>();

    //    public BluetoothScanner()
    //    {
    //        _adapter = BluetoothAdapter.DefaultAdapter;
    //    }

    //    public void StartScan()
    //    {
    //        if (_adapter != null && _adapter.IsEnabled)
    //        {
    //            _adapter.StartDiscovery();
    //        }
    //    }

    //    public List<string> GetPairedDevices()
    //    {
    //        var pairedDevices = _adapter.BondedDevices;
    //        foreach (var device in pairedDevices)
    //        {
    //            if (device.Name.Contains("HC-06"))
    //            {
    //                _devices.Add(device.Name + " - " + device.Address);
    //            }
    //        }
    //        return _devices;
    //    }
    //}




    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
       

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.R && ActivityCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothConnect) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, new string[] { Android.Manifest.Permission.BluetoothConnect }, 102);
            }
            if (Build.VERSION.SdkInt <= Android.OS.BuildVersionCodes.R && ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Bluetooth) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, new string[] { Android.Manifest.Permission.Bluetooth }, 102);

            }
            ///////////////////////////////////////////////////////////////////////////
            //BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            //if (!bluetoothAdapter.IsEnabled)
            //{
            //    Intent intent = new Intent(BluetoothAdapter.ActionRequestEnable);
            //    StartActivity(intent);
            //    IntentFilter filter = new IntentFilter(BluetoothAdapter.ActionStateChanged); 
                
            //}
              


            //BluetoothScanner bluetoothScanner = new BluetoothScanner();
            //bluetoothScanner.StartScan();
            //var devices = bluetoothScanner.GetPairedDevices();
            //foreach (var device in devices)
            //{
            //    System.Diagnostics.Debug.WriteLine(device);
            //}   

        }

        /////////////////////////////////////////////////////////////////////////////////////////


        //////////////////////////////////////////////////////////////////////////////////////////
    }




   
 

    






}
