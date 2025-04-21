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
//using Android;
//using Android.Bluetooth;


namespace DronApp1
{
    public partial class MainPage : ContentPage
    {

        int count = 0; // Initialize the variable before using it
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

        }




        private void Connect_Bluetooth(object sender, EventArgs e)
        {


            #if ANDROID
            

            var _devices = new List<string>();
            var bluetoothadapter = Android.Bluetooth.BluetoothAdapter.DefaultAdapter;
           


            if (bluetoothadapter.IsEnabled)
            {
                // Bluetooth is enabled
               
                bluetoothadapter.StartDiscovery();
                var pairedDevices = bluetoothadapter.BondedDevices;
                if (pairedDevices != null)
                {
                    DisplayAlert(bluetoothadapter.Name, "Connect", "OK");
                   
                }             
                foreach (var dev in pairedDevices)
                {
                 
                    if (dev.Name.Contains("HC-06"))
                    {
                        _devices.Add(dev.Name + " - " + dev.Address);
                        
                    }
                }

                             
            }
            else
            {
                // Bluetooth is disabled
                // Prompt the user to enable Bluetooth or handle accordingly
                DisplayAlert("Button Clicked", "Bluetooth is disabled!", "OK");
            }
           
            Android.Bluetooth.BluetoothDevice device = bluetoothadapter.GetRemoteDevice("98:D3:34:90:DC:F2");
            Android.Bluetooth.BluetoothSocket socket = device.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
            bluetoothadapter.CancelDiscovery();
            socket.Connect();   

            #endif

        }


        private async void Send_Data(object sender, EventArgs e)
        {
            string receivedData = "h";
#if ANDROID
           Stream? _inputStream = null; // Initialize the variable to avoid CS0165  
           byte[] buffer = new byte[1024];  
           var bluetoothadapter = Android.Bluetooth.BluetoothAdapter.DefaultAdapter;  

           if (bluetoothadapter == null)  
           {  
               await DisplayAlert("Error", "Bluetooth adapter is not available.", "OK");  
               return;  
           }  

           Android.Bluetooth.BluetoothDevice? device = bluetoothadapter.GetRemoteDevice("98:D3:34:90:DC:F2");  

           if (device == null)  
           {  
               await DisplayAlert("Error", "Bluetooth device not found.", "OK");  
               return;  
           }  

           Android.Bluetooth.BluetoothSocket? socket = device.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));  

           if (socket == null)  
           {  
               await DisplayAlert("Error", "Failed to create Bluetooth socket.", "OK");  
               return;  
           }  

           bluetoothadapter.CancelDiscovery();  

           try  
           {  
               await socket.ConnectAsync();
                _inputStream = socket.InputStream;
                // Use the input stream to read data from the Bluetooth device
                // For example, you can read data like this:
                while (true)
                {
                  await Task.Delay(1000);
                 int bytesRead = await _inputStream.ReadAsync(buffer, 0, buffer.Length);
                  receivedData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    // Process the received data as needed
                    
                    Counter.Text = $"{receivedData}";
            
                    SemanticScreenReader.Announce(Counter.Text);
                
                }   
                

           }  
           catch (Exception ex)  
           {  
               await DisplayAlert("Error", $"Failed to connect: {ex.Message}", "OK");  
               return;  
           }  
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