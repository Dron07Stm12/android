using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using Android.Bluetooth;
using Android.Content;
using System.Collections.Generic;
using static Microsoft.Maui.ApplicationModel.Permissions;
using InTheHand.Net.Sockets;
//using AndroidX.Core.App;
using AndroidX.Core.Content;
using System.Collections.ObjectModel;
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;

namespace DronApp1
{

    //public class BluetoothScanner
    //{
    //    // private readonly ContentPage _page; // Add a reference to the page
    //    string[] people = { "Tom", "Tim", "Bob", "Sam" };
    //    private BluetoothAdapter _adapter = null;
    //    private BluetoothSocket _device = null;
    //    //private DeviceReceiver _receiver;
    //    private ObservableCollection<string> _devices2 = new();
    //    private BluetoothDevice _device2 = null;
    //    //partial void OnBluetoothDeviceDiscovered(BluetoothDevice device)
    //    //{
    //    //    // Handle the discovered device
    //    //    System.Diagnostics.Debug.WriteLine($"Discovered device: {device.Name} - {device.Address}");
    //    //}

    //    private List<string> _devices = new List<string>();

    //    public BluetoothScanner()
    //    {
    //        _adapter = BluetoothAdapter.DefaultAdapter;
    //        if (people.Any())
    //        {
    //            // Handle the case when Bluetooth is not supported
    //            System.Diagnostics.Debug.WriteLine("Bluetooth is not supported on this device.");
    //            return;
    //        }
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

        public static MainActivity Instance { get; private set; }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //🔸 Что происходит:
            //Build.VERSION.SdkInt — текущая версия Android API устройства.
            //Android.OS.BuildVersionCodes.R — это API 30, т.е.Android 11.
            //Условие: если Android > 11(то есть Android 12 и выше)
            //🔹 Общая цель:
            //Эта строка(ActivityCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothConnect) != Permission.Granted) проверяет,
            //выдал ли пользователь приложению разрешение на подключение к Bluetooth - устройствам на Android 12 +.
            //this,                               // текущая Activity (контекст выполнения)
            //Manifest.Permission.BluetoothConnect // нужное разрешение
            //Это строковая константа, представляющая имя разрешения, которое нужно приложению.
            //Она равна "android.permission.BLUETOOTH_CONNECT" — то есть новое разрешение, введённое с Android 12(API 31).
            //!= Permission.Granted
            //Permission.Granted — это просто 0(код, означающий "разрешение предоставлено").
            //Если CheckSelfPermission(...) возвращает не Permission.Granted, значит: 👉 разрешение ещё не предоставлено,
            //и его нужно запросить через RequestPermissions(...).

            //и если разрешение BluetoothConnect не предоставлено, тогда:

            if (Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.R && ActivityCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothConnect) != Permission.Granted)
            {


                //🔸 Запрашиваем разрешение на BluetoothConnect
                //🔸 Что происходит:
                //Вызывается метод RequestPermissions, чтобы запросить разрешение BluetoothConnect.
                //102 — это произвольный код запроса(можешь использовать его позже в OnRequestPermissionsResult).
                ActivityCompat.RequestPermissions(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, new string[] { Android.Manifest.Permission.BluetoothConnect }, 102);
            }
            if (Build.VERSION.SdkInt <= Android.OS.BuildVersionCodes.R && ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Bluetooth) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, new string[] { Android.Manifest.Permission.Bluetooth }, 102);

            }

            //🔹 Дополнительная проверка для Android 12+ (SDK >= S, т.е. API 31)
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            {

                //🔸 Запрашиваем разрешение на BluetoothScan и BluetoothConnect

                if (ContextCompat.CheckSelfPermission(Platform.CurrentActivity, Manifest.Permission.BluetoothScan) != Android.Content.PM.Permission.Granted ||
                    ContextCompat.CheckSelfPermission(Platform.CurrentActivity, Manifest.Permission.BluetoothConnect) != Android.Content.PM.Permission.Granted)
                {
                    //Запрашиваются оба разрешения одновременно.
                    ActivityCompat.RequestPermissions(Platform.CurrentActivity, new string[]
                    {
                Manifest.Permission.BluetoothScan,
                Manifest.Permission.BluetoothConnect
                    }, 1);
                }
            }

           
        }

       
    }

    //public class BluetoothHC06Reader
    //{
    //    private BluetoothAdapter _adapter;
    //    private BluetoothSocket _socket;
    //    private Stream _inputStream;

    //    BluetoothClient client = new BluetoothClient();
    //    BluetoothDevice bluetoothDevice = null;
    //    BluetoothManager bluetoothManager = null;
    //    BroadcastReceiver broadcastReceiver = null;
    //    Task task = null;   
    //   // Application

    //    public BluetoothHC06Reader()
    //    {
    //        _adapter = BluetoothAdapter.DefaultAdapter;
    //    }

    //    public async Task ConnectToHC06(string deviceAddress)
    //    {
    //        try
    //        {
    //            BluetoothDevice device = _adapter.GetRemoteDevice(deviceAddress);
    //            _socket = device.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));

    //            _adapter.CancelDiscovery();
    //            await _socket.ConnectAsync();

    //            _inputStream = _socket.InputStream;
    //            Console.WriteLine("Подключено к HC-06");
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Ошибка подключения: {ex.Message}");
    //        }
    //    }

    //    public async Task<string> ReadDataAsync()
    //    {
    //        try
    //        {

    //            byte[] buffer = new byte[1024];
    //            int bytesRead = await _inputStream.ReadAsync(buffer, 0, buffer.Length);
    //            return System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Ошибка чтения данных: {ex.Message}");
    //            return null;
    //        }
    //    }
    //}


}



// Existing code...  

//if (Build.VERSION.SdkInt >= BuildVersionCodes.S) // Android 31 (API level 31) and later  
//{
//    if (ContextCompat.CheckSelfPermission(Platform.CurrentActivity!, Manifest.Permission.BluetoothScan) != Android.Content.PM.Permission.Granted ||
//        ContextCompat.CheckSelfPermission(Platform.CurrentActivity!, Manifest.Permission.BluetoothConnect) != Android.Content.PM.Permission.Granted)
//    {
//        ActivityCompat.RequestPermissions(Platform.CurrentActivity!, new string[]
//        {
//           Manifest.Permission.BluetoothScan,
//           Manifest.Permission.BluetoothConnect
//        }, 1);
//    }
//}





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

