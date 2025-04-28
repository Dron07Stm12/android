using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.CSharp.RuntimeBinder;
//using InTheHand.Bluetooth;
using InTheHand.Net.Sockets;
using System.Collections.Generic;
using System;
using System.IO.Ports;
using System.Collections.ObjectModel;
using InTheHand.Net.Bluetooth;
using System.Runtime.InteropServices;
using static Microsoft.Maui.ApplicationModel.Permissions;
using Microsoft.Maui.Controls;
using DronApp1;
using System.Net.Sockets;
//using Windows.Devices.Bluetooth;
#if ANDROID


using Android.Bluetooth;
using Android.Content;
using Application = Android.App.Application;
using Permissions = Microsoft.Maui.ApplicationModel.Permissions;
using System.Collections.ObjectModel;
#endif

namespace DronApp1
{
    public partial class MainPage : ContentPage
    {
        #if ANDROID
        BluetoothAdapter _adapter;
        DeviceReceiver _receiver;
        Android.Bluetooth.BluetoothSocket? socket_global;
        ObservableCollection<DeviceInfo> _devices = new();
        DeviceInfo _selectedDevice;
        #endif

        public MainPage()
        {
            InitializeComponent();

#if ANDROID
            DevicesList.ItemsSource = _devices;

            _adapter = BluetoothAdapter.DefaultAdapter;

            _receiver = new DeviceReceiver(deviceInfo =>
            {
                if (!_devices.Any(d => d.Address == deviceInfo.Address))
                {
                    _devices.Add(deviceInfo);
                }
            });

            Application.Context.RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound));
#endif
        }

        private async Task<bool> CheckPermissions()
        {
#if ANDROID
            var statusLoc = await RequestAsync<LocationWhenInUse>();
            var statusConnect = await RequestAsync<Bluetooth>();

            return statusLoc == PermissionStatus.Granted && statusConnect == PermissionStatus.Granted;
#else
            return true;
#endif
        }

        private async void OnScanClicked(object sender, EventArgs e)
        {
#if ANDROID
            if (!await CheckPermissions())
            {
                await DisplayAlert("Ошибка", "Нет разрешений для работы с Bluetooth", "OK");
                return;
            }

            _devices.Clear();

            if (_adapter == null)
            {
                await DisplayAlert("Ошибка", "Bluetooth не поддерживается", "OK");
                return;
            }

            if (!_adapter.IsEnabled)
            {
                await DisplayAlert("Ошибка", "Bluetooth выключен", "OK");
                return;
            }

            if (_adapter.IsDiscovering)
            {
                _adapter.CancelDiscovery();
            }

            await DisplayAlert("Поиск", "Начинаем сканирование устройств...", "OK");
            _adapter.StartDiscovery();
#endif
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
#if ANDROID
            try
            {
                Application.Context.UnregisterReceiver(_receiver);
            }
            catch (Exception)
            {
                // Уже отписались
            }
#endif
        }

        private async void OnDeviceSelected(object sender, SelectionChangedEventArgs e)
        {
#if ANDROID
    await DisplayAlert("Клик!", "Вы кликнули на устройство!", "OK");

    _selectedDevice = e.CurrentSelection.FirstOrDefault() as DeviceInfo;

    if (_selectedDevice == null)
    {
        await DisplayAlert("Ошибка", "Устройство не выбрано!", "OK");
        return;
    }

    await DisplayAlert("Вы выбрали устройство", $"{_selectedDevice.Name} [{_selectedDevice.Address}]", "OK");

    bool connectResult = await ConnectToDeviceAsync(_selectedDevice);

    if (connectResult)
    {
        await DisplayAlert("Успех", $"Подключение к {_selectedDevice.Name} установлено!", "OK");
    }
    else
    {
        await DisplayAlert("Ошибка", $"Не удалось подключиться к {_selectedDevice.Name}.", "OK");
    }
#endif
        }



#if ANDROID
        private async Task<bool> ConnectToDeviceAsync(DeviceInfo deviceInfo)
        {
            try
            {
                var device = _adapter.GetRemoteDevice(deviceInfo.Address);

                _adapter.CancelDiscovery();

                socket_global = device.CreateRfcommSocketToServiceRecord(
                    Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));

                await Task.Run(() => socket_global.Connect());

                return socket_global.IsConnected;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка подключения", $"Ошибка: {ex.Message}", "OK");
                return false;
            }
        }

        class DeviceReceiver : BroadcastReceiver
        {
            private readonly Action<DeviceInfo> _onDeviceFound;

            public DeviceReceiver(Action<DeviceInfo> onDeviceFound)
            {
                _onDeviceFound = onDeviceFound;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action == BluetoothDevice.ActionFound)
                {
                    BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    if (device != null)
                    {
                        string name = string.IsNullOrEmpty(device.Name) ? "Неизвестное устройство" : device.Name;
                        string address = device.Address;

                        DeviceInfo deviceInfo = new DeviceInfo
                        {
                            Name = name,
                            Address = address
                        };

                        _onDeviceFound?.Invoke(deviceInfo);
                    }
                }
            }
        }

        public class DeviceInfo
        {
            public string Name { get; set; }
            public string Address { get; set; }

            public override string ToString()
            {
                return $"{Name} [{Address}]";
            }

            public override bool Equals(object obj)
            {
                return obj is DeviceInfo other && Address == other.Address;
            }

            public override int GetHashCode()
            {
                return Address.GetHashCode();
            }
        }
#endif
    }
}










/////////////////////////////////////////////////////////////////////////////////
//BluetoothClient client = new BluetoothClient();

////BluetoothClient client = new BluetoothClient();
////DisplayAlert("Button Clicked", "Scan_Clicked!", "BluetoothClient");
//////DeviceList = new ObservableCollection<IDevice>();
//foreach (var dev in client.DiscoverDevices())
//{

//    DisplayAlert("Button Clicked", "Scan_Clicked!", "for");
//    if (dev.DeviceName == null)
//    {
//        // Check if the device name contains "Bike"
//        DisplayAlert("Button Clicked", "Scan_Clicked!", "dev.DeviceName");

//        //if (dev.DeviceName.Contains("98:D3:34:90:DC:F2"))
//        //{
//        //    DisplayAlert("Button Clicked", "Scan_Clicked!", "98:D3:34:90:DC:F2");
//        //    device = dev;
//        //    //DeviceList.Add(device);
//        //    //DeviceSelected = device;
//        //    //IsDeviceListEmpty = false;
//        //    //IsScanning = false;
//        //    break;
//        //}

//    }
//    // Display the name and address of each discovered device
//    //   Console.WriteLine($"Device: {dev.DeviceName}, Address: {dev.DeviceAddress}");
//    break;
//}


//if (!device.Authenticated)
//{
//    BluetoothSecurity.PairRequest(device.DeviceAddress, "1234");
//}
//device.Refresh();



//client.Connect(device.DeviceAddress, BluetoothService.SerialPort);
//////////////////////////////////////////////////////////////////////////
///
/////////////////////////////////////
//BluetoothClient client = new BluetoothClient();
//DisplayAlert("Button Clicked", "Scan_Clicked!", "BluetoothClient");
////DeviceList = new ObservableCollection<IDevice>();
//foreach (var dev in client.DiscoverDevices())
//{

//    DisplayAlert("Button Clicked", "Scan_Clicked!", "for");
//    if (dev.DeviceName == null)
//    {
//        // Check if the device name contains "Bike"
//        DisplayAlert("Button Clicked", "Scan_Clicked!", "dev.DeviceName");

//        //if (dev.DeviceName.Contains("98:D3:34:90:DC:F2"))
//        //{
//        //    DisplayAlert("Button Clicked", "Scan_Clicked!", "98:D3:34:90:DC:F2");
//        //    device = dev;
//        //    //DeviceList.Add(device);
//        //    //DeviceSelected = device;
//        //    //IsDeviceListEmpty = false;
//        //    //IsScanning = false;
//        //    break;
//        //}

//    }
//    // Display the name and address of each discovered device
//    //   Console.WriteLine($"Device: {dev.DeviceName}, Address: {dev.DeviceAddress}");
//    break;

//}


//if (!device.Authenticated)
//{
//    BluetoothSecurity.PairRequest(device.DeviceAddress, "0000");
//}
//device.Refresh();



//client.Connect(device.DeviceAddress, BluetoothService.SerialPort);




//////////////////////////////////////////////////////////////////////////////////////////////////









// Handle the button click event here
// For example, you can navigate to another page or perform some action
//  DisplayAlert("Button Clicked", "Button 1 was clicked!", "OK");
//var profiles = Connectivity.Current.ConnectionProfiles;

//if (profiles.Contains(ConnectionProfile.Bluetooth))
//{
//    Console.WriteLine("Bluetooth is connected!");
//}
//else
//{
//    DisplayAlert("Button Clicked", "Button 1 was clicked!", "OK");
//}


//NetworkAccess accessType = Connectivity.Current.NetworkAccess;

//if (accessType == NetworkAccess.Internet)
//{





//    // Connection to internet is available
//}
///////////////////////////////////////////////////

//string deviceId = "98:D3:34:90:DC:F2"; // Replace with actual device ID(98:D3:34:90:DC:F2)
//var scanner = new BluetoothScanner();


// Use null-conditional operator and null check to handle possible null value
//BluetoothDevice? device = BluetoothDevice.FromIdAsync(deviceId).Result;

//if (device != null)
//{

//    DisplayAlert(device.Name, device.Id, "OK");
//    //   BluetoothSecurity.PairRequest(device.DeviceAddress, "0000");


//    //  client.Connect(device.DeviceAddress, BluetoothService.SerialPort);
//    // Use the device object to perform Bluetooth operations

//}
//else
//{
//    DisplayAlert("Error", "Failed to retrieve Bluetooth device.", "OK");
//}
////////////////////////////////////////////////////////////////////
//string deviceId = "98:D3:34:90:DC:F2";
//var profiles = Connectivity.Current.ConnectionProfiles;

//if (profiles.Contains(ConnectionProfile.Bluetooth))
//{

//    DisplayAlert("Button Clicked", "Bluetooth is connected!", "OK");
//}
//else
//{
//    DisplayAlert("Button Clicked", "no_Bluetooth is connected!", "OK");
//}


//////получение данных по блютуз HC-06
//client.Connect(device.DeviceAddress, BluetoothService.SerialPort);
////Start a new Thread after connection:
//Thread tr = new(ReceiveData);
//tr.IsBackground = true;
//tr.Start();

//private void ReceiveData()
//{
//    var stream = client.GetStream();
//    byte[] receive = new byte[1024];

//    while (true)
//    {
//        Array.Clear(receive, 0, receive.Length);
//        var readMessage = "";
//        do
//        {
//            Thread.Sleep(1000);
//            stream.Read(receive, 0, receive.Length);
//            readMessage += Encoding.ASCII.GetString(receive);
//        }
//        while (stream.DataAvailable);
//    }
//}



/////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////
///
//IEnumerable<ConnectionProfile> profiles = Connectivity.Current.ConnectionProfiles;

//if (profiles.Contains(ConnectionProfile.WiFi))
//{
//    // Bluetooth connection.
// //   DisplayAlert("Button Clicked", "Scan_Clicked!", "OK");

//    // Active Wi-Fi connection.
//}

//if (profiles.Contains(ConnectionProfile.Bluetooth))
//{
//    // Bluetooth connection.
//    DisplayAlert("Button Clicked", "Scan_Clicked!", "OK");

//    // Active Wi-Fi connection.
//}


///////////////////////////////////////////////////////
//#if ANDROID
//    var bluetoothManager = Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService) as Android.Bluetooth.BluetoothManager;

//    if (bluetoothManager != null)
//    {
//        var bluetoothAdapter = bluetoothManager.Adapter;

//        if (bluetoothAdapter != null)
//        {
//            if (bluetoothAdapter.IsEnabled)
//            {
//                // Use the newer API for Android 33 and later
//                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
//                {
//                    // Redirect user to Bluetooth settings to disable Bluetooth
//                    var intent = new Android.Content.Intent(Android.Provider.Settings.ActionBluetoothSettings);
//                    intent.AddFlags(Android.Content.ActivityFlags.NewTask);
//                    Android.App.Application.Context.StartActivity(intent);
//                }
//                else
//                {
//                    // Disable Bluetooth for older Android versions
//                    DisplayAlert(bluetoothAdapter.Name, " No_Connect", "OK");
//                    // Use reflection to call the Disable method for Android < 33
//                    try
//                    {
//                        var disableMethod = bluetoothAdapter.Class.GetMethod("disable");
//                        disableMethod.Invoke(bluetoothAdapter);
//                    }
//                    catch (Exception ex)
//                    {
//                        DisplayAlert("Error", $"Failed to disable Bluetooth: {ex.Message}", "OK");
//                    }
//                }
//            }
//            else
//            {
//                // Use the newer API for Android 33 and later
//                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
//                {
//                    // Redirect user to Bluetooth settings to enable Bluetooth
//                    var intent = new Android.Content.Intent(Android.Provider.Settings.ActionBluetoothSettings);
//                    intent.AddFlags(Android.Content.ActivityFlags.NewTask);
//                    Android.App.Application.Context.StartActivity(intent);
//                }
//                else
//                {
//                    // Enable Bluetooth for older Android versions
//                    DisplayAlert(bluetoothAdapter.Name, "Connect", "OK");
//                    bluetoothAdapter.Enable();
//                }
//            }
//        }
//    }
//#endif


/////////////////////////////////////////

