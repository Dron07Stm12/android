// Пространство имён для специфичных настроек MAUI на iOS (здесь пока не используется, но подключено для кроссплатформенности)
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
// Для выполнения привязки во время выполнения между типами (dynamic binding) в C# (не используется сейчас, можно удалить)
using Microsoft.CSharp.RuntimeBinder;
//using InTheHand.Bluetooth;
// Библиотека InTheHand для работы с Bluetooth (используется на других платформах, не Android)
using InTheHand.Net.Sockets;
using System.Collections.Generic;
using System;
// Специализированные коллекции, такие как ObservableCollection (важно для привязки данных в MVVM)
using System.Collections.ObjectModel;
// Работа с Bluetooth через библиотеку InTheHand.Net
using InTheHand.Net.Bluetooth;
// Для взаимодействия с неуправляемым кодом (например, вызов функций WinAPI)
using System.Runtime.InteropServices;
// Статический импорт всех разрешений MAUI для упрощённого обращения
using static Microsoft.Maui.ApplicationModel.Permissions;
// Библиотека элементов управления MAUI (например, ContentPage, Button и т.д.)
using Microsoft.Maui.Controls;
using DronApp1;
// Работа с сокетами на сетевом уровне TCP/IP (для сетевых соединений)
using System.Net.Sockets;
using System.ComponentModel;

#if ANDROID// --- Только для Android (директива препроцессора) ---

// Работа с Bluetooth через Android API
using Android.Bluetooth;
// Работа с контекстом приложений Android
using Android.Content;
// Переименование Android.App.Application, чтобы не конфликтовать с Microsoft.Maui.Application
using Application = Android.App.Application;
// Работа с разрешениями через MAUI
using Permissions = Microsoft.Maui.ApplicationModel.Permissions;

#endif

namespace DronApp1
{
    public partial class MainPage : ContentPage
    {
#if ANDROID
        BluetoothAdapter _adapter;  // BluetoothAdapter - основной класс Android для работы с Bluetooth (включение/отключение, сканирование устройств)
        DeviceReceiver _receiver;  // Приёмник событий BroadcastReceiver для отлавливания событий поиска Bluetooth-устройств

        Android.Bluetooth.BluetoothSocket? socket_global; // Переменная для хранения глобального сокета подключения к выбранному устройству
        ObservableCollection<DeviceInfo> _devices = new(); // Наблюдаемая коллекция всех найденных устройств (обновляется автоматически на UI при изменении)
        DeviceInfo _selectedDevice;// Выбранное пользователем устройство из списка
#endif

        public string receivedData2 = null;



        public MainPage()
        {
            InitializeComponent(); // Инициализация всех компонентов страницы, созданных в XAML

#if ANDROID
            DevicesList.ItemsSource = _devices;// Устанавливаем источник данных для списка устройств — нашу коллекцию _devices


            _adapter = BluetoothAdapter.DefaultAdapter;// Получаем ссылку на стандартный Bluetooth-адаптер устройства


             // Действие при обнаружении нового устройства:
              // Проверяем, нет ли уже устройства с таким же адресом (уникальный идентификатор MAC)
            _receiver = new DeviceReceiver(deviceInfo =>
            {
                if (!_devices.Any(d => d.Address == deviceInfo.Address))
                {
                  // Если устройство не найдено в списке, добавляем его
                    _devices.Add(deviceInfo);
                }
            });

            //////////////////////////////можно использовать  делегат
            //_receiver = new DeviceReceiver(delegate(DeviceInfo deviceInfo){
                  
            //       if(! _devices.Any(delegate(DeviceInfo deviceInfo2){
                   
            //       return deviceInfo2.Address == deviceInfo.Address;    
            //       }))
            //       {
                     
            //         _devices.Add(deviceInfo);  
            //       }
               
            //   });


            /////////////////////////////
             // Регистрируем наш приемник на события нахождения новых Bluetooth устройств
            Application.Context.RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound));
#endif
        }


        // --- Метод для проверки всех необходимых разрешений --
        private async Task<bool> CheckPermissions()
        {
#if ANDROID
            // Запрашиваем разрешение на использование геолокации (требуется для поиска Bluetooth-устройств в Android)
            var statusLoc = await RequestAsync<LocationWhenInUse>();
             // Запрашиваем разрешение на использование Bluetooth
            var statusConnect = await RequestAsync<Bluetooth>();
             // Возвращаем true, если оба разрешения получены
            return statusLoc == PermissionStatus.Granted && statusConnect == PermissionStatus.Granted;
#else
            return true;
#endif
        }

        // --- Метод для обработки нажатия кнопки "Сканировать" ---
        private async void OnScanClicked(object sender, EventArgs e)
        {
#if ANDROID

            if (!await CheckPermissions())
            {
                await DisplayAlert("Ошибка", "Нет разрешений для работы с Bluetooth", "OK");
                return;
            }

            _devices.Clear();// Очищаем список устройств перед началом нового сканирования

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
             // Запускаем процесс поиска Bluetooth-устройств
            _adapter.StartDiscovery();
#endif
        }

        // --- Переопределение метода, вызываемого при уходе со страницы ---
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
#if ANDROID
            try
            {
                  // Отписываемся от приёмника событий при уходе со страницы (освобождаем ресурсы)
                Application.Context.UnregisterReceiver(_receiver);
            }
            catch (Exception)
            {
                // Если приемник уже отписан, игнорируем ошибку
            }
#endif
        }

        // Updated method to fix CS8601: Possible null reference assignment.
        //        private async void OnDeviceSelected(object sender, SelectionChangedEventArgs e)
        //        {
        //#if ANDROID
        //            // Получаем текущее выбранное устройство
        //            var clickedDevice = e.CurrentSelection.FirstOrDefault() as DeviceInfo;

        //            // Если кликнули повторно по уже выбранному — снимаем выбор
        //            if (_selectedDevice != null && clickedDevice != null && _selectedDevice.Address == clickedDevice.Address)
        //            {
        //                DevicesList.SelectedItem = null;   // Сброс визуального выделения
        //                _selectedDevice = null;            // Обнуляем переменную выбранного устройства

        //                // Если ты используешь цвет/рамку для визуального выделения — здесь можно сбросить IsSelected (если такое есть)
        //                if (clickedDevice != null)
        //                {
        //                    clickedDevice.IsSelected = false; // если ты используешь такую логику
        //                }

        //                // Обновим привязку, чтобы визуально обновилось
        //                DevicesList.ItemsSource = null;
        //                DevicesList.ItemsSource = _devices;

        //                await DisplayAlert("Отмена", "Выбор устройства отменён", "OK");
        //                return;
        //            }

        //            // Снимаем выделение с предыдущего устройства, если есть
        //            if (_selectedDevice != null)
        //            {
        //                _selectedDevice.IsSelected = false;
        //            }

        //            // Обновляем текущий выбор
        //            if (clickedDevice != null) // Ensure clickedDevice is not null
        //            {
        //                _selectedDevice = clickedDevice;

        //                // Устанавливаем новый флаг выбора (если используется для визуального отображения)
        //                _selectedDevice.IsSelected = true;

        //                // Обновим визуальное выделение
        //                DevicesList.ItemsSource = null;
        //                DevicesList.ItemsSource = _devices;

        //                await DisplayAlert("Вы выбрали устройство", $"{_selectedDevice.Name} [{_selectedDevice.Address}]", "OK");

        //                bool connectResult = await ConnectToDeviceAsync(_selectedDevice);
        //                await DisplayAlert("Ожидайте подключения", $"{_selectedDevice.Name} [{_selectedDevice.Address}]", "OK");

        //                if (connectResult)
        //                {
        //                    await DisplayAlert("Успех", $"Подключение к {_selectedDevice.Name} установлено!", "OK");
        //                }
        //                else
        //                {
        //                    await DisplayAlert("Ошибка", $"Не удалось подключиться к {_selectedDevice.Name}.", "OK");
        //                }
        //            }
        //            else
        //            {
        //                await DisplayAlert("Ошибка", "Устройство не выбрано!", "OK");
        //            }
        //#endif
        //        }










        //        private async void OnDeviceSelected(object sender, SelectionChangedEventArgs e)
        //        {
        //#if ANDROID
        //    // Получаем текущее выбранное устройство
        //    var clickedDevice = e.CurrentSelection.FirstOrDefault() as DeviceInfo;

        //    // Если кликнули повторно по уже выбранному — снимаем выбор
        //    if (_selectedDevice != null && clickedDevice != null && _selectedDevice.Address == clickedDevice.Address)
        //    {
        //        DevicesList.SelectedItem = null;   // Сброс визуального выделения
        //        _selectedDevice = null;            // Обнуляем переменную выбранного устройства

        //        // Если ты используешь цвет/рамку для визуального выделения — здесь можно сбросить IsSelected (если такое есть)
        //        if (clickedDevice != null)
        //        {
        //            clickedDevice.IsSelected = false; // если ты используешь такую логику
        //        }

        //        // Обновим привязку, чтобы визуально обновилось
        //        DevicesList.ItemsSource = null;
        //        DevicesList.ItemsSource = _devices;

        //        await DisplayAlert("Отмена", "Выбор устройства отменён", "OK");
        //        return;
        //    }

        //    // Снимаем выделение с предыдущего устройства, если есть
        //    if (_selectedDevice != null)
        //    {
        //        _selectedDevice.IsSelected = false;
        //    }

        //    // Обновляем текущий выбор
        //    _selectedDevice = clickedDevice;

        //    if (_selectedDevice == null)
        //    {
        //        await DisplayAlert("Ошибка", "Устройство не выбрано!", "OK");
        //        return;
        //    }

        //    // Устанавливаем новый флаг выбора (если используется для визуального отображения)
        //    _selectedDevice.IsSelected = true;

        //    // Обновим визуальное выделение
        //    DevicesList.ItemsSource = null;
        //    DevicesList.ItemsSource = _devices;

        //    await DisplayAlert("Вы выбрали устройство", $"{_selectedDevice.Name} [{_selectedDevice.Address}]", "OK");

        //    bool connectResult = await ConnectToDeviceAsync(_selectedDevice);
        //    await DisplayAlert("Ожидайте подключения", $"{_selectedDevice.Name} [{_selectedDevice.Address}]", "OK");

        //    if (connectResult)
        //    {
        //        await DisplayAlert("Успех", $"Подключение к {_selectedDevice.Name} установлено!", "OK");
        //    }
        //    else
        //    {
        //        await DisplayAlert("Ошибка", $"Не удалось подключиться к {_selectedDevice.Name}.", "OK");
        //    }
        //#endif
        //        }





        //        private async void OnDeviceSelected(object sender, SelectionChangedEventArgs e)
        //        {
        //#if ANDROID
        //    // Получаем текущее выбранное устройство
        //    var clickedDevice = e.CurrentSelection.FirstOrDefault() as DeviceInfo;

        //    // Если кликнули повторно по уже выбранному — снимаем выбор
        //    if (_selectedDevice != null && clickedDevice != null && _selectedDevice.Address == clickedDevice.Address)
        //    {
        //        DevicesList.SelectedItem = null;   // Сброс выделения
        //        _selectedDevice = null;            // Обнуляем переменную
        //        await DisplayAlert("Отмена", "Выбор устройства отменён", "OK");
        //        return;
        //    }

        //    // Обновляем текущий выбор
        //    _selectedDevice = clickedDevice;

        //    if (_selectedDevice == null)
        //    {
        //        await DisplayAlert("Ошибка", "Устройство не выбрано!", "OK");
        //        return;
        //    }

        //    await DisplayAlert("Вы выбрали устройство", $"{_selectedDevice.Name} [{_selectedDevice.Address}]", "OK");

        //    bool connectResult = await ConnectToDeviceAsync(_selectedDevice);
        //    await DisplayAlert("Ожидайте подключения", $"{_selectedDevice.Name} [{_selectedDevice.Address}]", "OK");

        //    if (connectResult)
        //    {
        //        await DisplayAlert("Успех", $"Подключение к {_selectedDevice.Name} установлено!", "OK");
        //    }
        //    else
        //    {
        //        await DisplayAlert("Ошибка", $"Не удалось подключиться к {_selectedDevice.Name}.", "OK");
        //    }
        //#endif
        //        }








        private async void OnDeviceSelected(object sender, SelectionChangedEventArgs e)
        {
#if ANDROID
                     //   await DisplayAlert("Клик!", "Вы кликнули на устройство!", "OK");
                      // Получаем выбранное пользователем устройство
                        _selectedDevice = e.CurrentSelection.FirstOrDefault() as DeviceInfo;
                      //  if( e.CurrentSelection.FirstOrDefault() is DeviceInfo){ _selectedDevice = (DeviceInfo)e.CurrentSelection.FirstOrDefault();}


                        if (_selectedDevice == null)
                        {
                            await DisplayAlert("Ошибка", "Устройство не выбрано!", "OK");
                            return;
                        }

                        await DisplayAlert($"Вы выбрали устройство", $"{_selectedDevice.Name} [{_selectedDevice.Address}]", "OK");

                        bool connectResult = await ConnectToDeviceAsync(_selectedDevice);
                         await DisplayAlert("Ожидайте подключения", $"{_selectedDevice.Name} [{_selectedDevice.Address}]", "OK");
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

              // Добавляем флаг для отслеживания выделенного состояния
          //    public bool IsSelected { get; set; }

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


        // public class DeviceInfo : INotifyPropertyChanged
        //{
        //    private bool _isSelected;

        //    public string Name { get; set; }
        //    public string Address { get; set; }

        //    public bool IsSelected
        //    {
        //        get => _isSelected;
        //        set
        //        {
        //            if (_isSelected != value)
        //            {
        //                _isSelected = value;
        //                OnPropertyChanged(nameof(IsSelected));
        //            }
        //        }
        //    }

        //    public event PropertyChangedEventHandler PropertyChanged;

        //    protected void OnPropertyChanged(string propertyName) =>
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //    public override string ToString() => $"{Name} [{Address}]";

        //    public override bool Equals(object obj) => obj is DeviceInfo other && Address == other.Address;

        //    public override int GetHashCode() => Address.GetHashCode();
        //}









#endif










        public async void ReceiverData(object sender, EventArgs e)//Bluetooth connect
        {

            Stream? _inputStream = null; // Initialize the variable to avoid CS0165
            byte[] buffer = new byte[2048];
#if ANDROID

                    try
                    {
                        _inputStream = socket_global.InputStream;
                        // Use the input stream to read data from the Bluetooth device
                        // For example, you can read data like this:
                        while (true)
                        {
                            await Task.Delay(200);
                              
                            int bytesRead = await _inputStream.ReadAsync(buffer, 0, buffer.Length);
                            receivedData2 = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            label4.Text = $"data hc-06:{Environment.NewLine}{receivedData2}";
                            SemanticScreenReader.Announce(label4.Text);
                            receivedData2 = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to connect: {ex.Message}", "OK");
                        return;
                    }

#endif

        }





        private async void TransmitterData(object sender, EventArgs e)
        {
            Stream? _outputStream = null;
            byte[] buffer = new byte[2048];
#if ANDROID

                    try
                    {
                        _outputStream = socket_global.OutputStream;
                      //   entry1.Text = null; 

                     //   while (true)
                    //    {
                            await Task.Delay(100);
                            buffer = System.Text.Encoding.ASCII.GetBytes(entry1.Text);//  buffer = System.Text.Encoding.ASCII.GetBytes("g");
                            await _outputStream.WriteAsync(buffer, 0, buffer.Length);
                        //    await DisplayAlert("Button Clicked", "Button 1 was clicked!", "OK");

                      //  }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to connect: {ex.Message}", "OK");
                        return;
                    }

#endif


        }



        public async void On_Off_Bluetooth(object sender, EventArgs e)//Bluetooth connect
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
                                        DisplayAlert(bluetoothAdapter.Name, " No_Connect", "OK");
                                        // Use reflection to call the Disable method for Android < 33
                                        try
                                        {
                                            var disableMethod = bluetoothAdapter.Class.GetMethod("disable");
                                            disableMethod.Invoke(bluetoothAdapter);
                                        }
                                        catch (Exception ex)
                                        {
                                            DisplayAlert("Error", $"Failed to disable Bluetooth: {ex.Message}", "OK");
                                        }
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
                                        DisplayAlert(bluetoothAdapter.Name, "Connect", "OK");
                                        bluetoothAdapter.Enable();
                                    }
                                }
                            }
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

