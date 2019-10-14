using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamarinEntry;
using XamarinEntry.Droid;

using KegMaster.Core.Features;

using Android.Content;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace XamarinEntry.Droid
{
	public class CustomEntryRenderer : EntryRenderer
	{
		public CustomEntryRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);
			if (Control != null)
			{
				Control.Background = new ColorDrawable(Android.Graphics.Color.Transparent);
			}
		}
	}
}