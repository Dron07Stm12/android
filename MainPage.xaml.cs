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
//using Android.Bluetooth;
//using Android.Bluetooth;
//using Android;
//using Android.Bluetooth;


namespace DronApp1
{
    public partial class MainPage : ContentPage
    {
     //  private BluetoothDeviceInfo device = null;
     //   List<BluetoothDeviceInfo> DeviceList = new List<BluetoothDeviceInfo>();

        //  private BluetoothDeviceInfo device;

        public MainPage()
        {
            InitializeComponent();
        }



        private void Button_Clicked(object sender, EventArgs e)
        {
          #if ANDROID
                var bluetoothManager = Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService) as Android.Bluetooth.BluetoothManager;

                if (bluetoothManager != null)
                {
                    var bluetoothAdapter = bluetoothManager.Adapter;

                    if (bluetoothAdapter != null)
                    {
                        if (bluetoothAdapter.IsEnabled)
                        {
                            // Use the newer API for Android 33 and later
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
                            {
                                // Redirect user to Bluetooth settings to disable Bluetooth
                                var intent = new Android.Content.Intent(Android.Provider.Settings.ActionBluetoothSettings);
                                intent.AddFlags(Android.Content.ActivityFlags.NewTask);
                                Android.App.Application.Context.StartActivity(intent);
                            }
                            else
                            {
                                // Disable Bluetooth for older Android versions
                                bluetoothAdapter.Disable(); // This is still valid for Android < 33
                            }
                        }
                        else
                        {
                            // Use the newer API for Android 33 and later
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
                            {
                                // Redirect user to Bluetooth settings to enable Bluetooth
                                var intent = new Android.Content.Intent(Android.Provider.Settings.ActionBluetoothSettings);
                                intent.AddFlags(Android.Content.ActivityFlags.NewTask);
                                Android.App.Application.Context.StartActivity(intent);
                            }
                            else
                            {
                                // Enable Bluetooth for older Android versions
                                bluetoothAdapter.Enable();
                            }
                        }
                    }
                }
            #endif
        }

        private void Scan_Clicked(object sender, EventArgs e)
        {


          #if ANDROID
            var enable = new Android.Content.Intent(Android.Bluetooth.BluetoothAdapter.ActionRequestEnable);
            enable.SetFlags(Android.Content.ActivityFlags.NewTask);

            var disable = new Android.Content.Intent(Android.Bluetooth.BluetoothAdapter.ActionRequestDiscoverable);
            disable.SetFlags(Android.Content.ActivityFlags.NewTask);

            var bluetoothManager = (Android.Bluetooth.BluetoothManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService);
            var bluetoothAdapter = bluetoothManager.Adapter;

            if (bluetoothAdapter.IsEnabled == true)
            {
                Android.App.Application.Context.StartActivity(disable);
                // Disable the Bluetooth;
            }
            else
            {
                // Enable the Bluetooth
                Android.App.Application.Context.StartActivity(enable);
            }
           #endif



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



        }




        private void Connect_Bluetooth(object sender, EventArgs e)
        {


          #if ANDROID
            var _devices = new List<string>();
            var bluetoothadapter = Android.Bluetooth.BluetoothAdapter.DefaultAdapter;
           


            if (bluetoothadapter.IsEnabled)
            {
                // Bluetooth is enabled
                // Perform your Bluetooth operations here

                bluetoothadapter.StartDiscovery();
                var pairedDevices = bluetoothadapter.BondedDevices;
                if (pairedDevices != null)
                {
                    DisplayAlert(bluetoothadapter.Name, "Connect", "OK");
                    // Loop through paired devices
                    //foreach (var device in pairedDevices)
                    //{
                    //    // Display the name and address of each paired device
                    //    Console.WriteLine($"Device: {device.Name}, Address: {device.Address}");
                    //}
                }
               // DisplayAlert("Button Clicked", "Bluetooth is enable!", "OK");
                foreach (var dev in pairedDevices)
                {

                   
                    if (dev.Name.Contains("HC-06"))
                    {
                        _devices.Add(dev.Name + " - " + dev.Address);
                        
                    }
                }
               

                // BluetoothDevice device = bluetoothadapter.GetRemoteDevice(deviceAddress);
                //BluetoothSocket socket = device.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));

                //_adapter.CancelDiscovery();
                //socket.Connect();


            }
            else
            {
                // Bluetooth is disabled
                // Prompt the user to enable Bluetooth or handle accordingly
                DisplayAlert("Button Clicked", "Bluetooth is disabled!", "OK");
            }
            // BluetoothDevice device = bluetoothadapter.GetRemoteDevice("98:D3:34:90:DC:F2"); 

            Android.Bluetooth.BluetoothDevice device = bluetoothadapter.GetRemoteDevice("98:D3:34:90:DC:F2");
            Android.Bluetooth.BluetoothSocket socket = device.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
            bluetoothadapter.CancelDiscovery();
            socket.Connect();   

#endif





        }


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
