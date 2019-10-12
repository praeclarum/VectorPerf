using Foundation;
using System;
using System.Threading.Tasks;
using UIKit;

namespace VectorPerf.iOS
{
    public partial class ViewController : UIViewController
    {
        UIButton run = UIButton.FromType(UIButtonType.RoundedRect);

        public ViewController(IntPtr handle) : base(handle)
        {
            run.SetTitle("RUN", UIControlState.Normal);
            run.TouchUpInside += async (s, e) =>
            {
                run.Enabled = false;
                await RunPerfAsync();
                run.Enabled = true;
            };
        }

        async Task RunPerfAsync()
        {
            await Task.Delay(500);
            await PerfRunner.RunAsync();
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            run.Frame = View.Bounds;
            View.AddSubview(run);

            run.Enabled = false;
            await RunPerfAsync();
            run.Enabled = true;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}