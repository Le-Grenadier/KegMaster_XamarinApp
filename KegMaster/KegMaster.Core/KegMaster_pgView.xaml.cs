using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using KegMaster.Core.Database;
using System.Collections.Generic;
using KegMaster.Core.Pages.ManageKegs_Views;

namespace KegMaster.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KegMaster_pgView : ContentPage
    {
		private ObservableCollection<KegItem> kegs = new ObservableCollection<KegItem>();
		private ConnectionManager manager = ConnectionManager.DefaultManager;
		private int numTaps;
		private bool enableDelete;

		/* Constructor assumes kegs exist for each index up to n */
		public KegMaster_pgView()
        {

			InitializeComponent();

			MessagingCenter.Subscribe<TapEdit, KegItem>(this, "KegItem_Updated", (sender, arg) => {
				if (arg.TapNo > kegs.Count) {
					kegs.Add(arg);
				} else {
					kegs.RemoveAt(arg.TapNo);
					kegs.Insert(arg.TapNo, arg);
				}
			});

			DisplayLoading.IsVisible = true;

			MyListView.ItemsSource = kegs;
			MyListView.IsVisible = false;
			kegModBtns.IsVisible = false;
			Task.Run(async() => await loadKegsBackground());
        }

		async Task loadKegsBackground()
		{
			numTaps = 0;
			KegItem keg = await getKeg(numTaps);

			while (keg != null)  {
				enableDelete = true;

				kegs.Add(keg);
				OnPropertyChanged();

				numTaps++;
				keg = await getKeg(numTaps);
			}

			DisplayLoading.IsVisible = false;
			MyListView.IsVisible = true;
			kegModBtns.IsVisible = true;
		}

		/*----------------------------------------------------------------------
         Get all kegs that have been created, no defined limit currently exists
        ----------------------------------------------------------------------*/
		async Task<KegItem> getKeg(int idx)
        {
			var keg = await manager.GetActiveKegAsync(idx);
            return (keg);
        }

        /*----------------------------------------------------------------------
        Set callback for when on-screen item has been selected
        ----------------------------------------------------------------------*/
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await Navigation.PushAsync(new Pages.ManageKegs_Views.TapEdit());
            MessagingCenter.Send<KegMaster_pgView, KegItem>(this, "KegItem_EditContext", (Database.KegItem)e.Item);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

		/*----------------------------------------------------------------------
		Add Keg
		----------------------------------------------------------------------*/
		async void OnAddKegBtnClicked(object sender, EventArgs args)
		{
			KegItem keg = new KegItem();
			keg.TapNo = numTaps;
			numTaps++;
			enableDelete = numTaps > 0;

			await Navigation.PushAsync(new Pages.ManageKegs_Views.TapEdit());
			MessagingCenter.Send<KegMaster_pgView, KegItem>(this, "KegItem_EditContext", keg);
		}

		/*----------------------------------------------------------------------
		Remove keg
		----------------------------------------------------------------------*/
		async void OnRemoveKegBtnClicked(object sender, EventArgs args)
		{
			string s = await DisplayActionSheet(string.Format("Kegs must be deleted in reverse order. \nDelete Keg {0}?", numTaps), "Cancel", "Delete Keg");
			if( s.Contains("Delete") && kegs.Count >= numTaps) {
				await manager.DeleteKegAsync(kegs[numTaps - 1]);

				kegs.RemoveAt(numTaps - 1);
				numTaps--;
				enableDelete = numTaps > 0;
			}
		}
	}
}
