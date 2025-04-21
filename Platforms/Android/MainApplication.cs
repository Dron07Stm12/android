using Android.App;
using Android.Runtime;
using Android.Bluetooth;
using Android;

//[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]

namespace DronApp1
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
