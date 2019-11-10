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
				if (arg.TapNo >= kegs.Count) {
					kegs.Add(arg);
				} else {
					kegs.RemoveAt(arg.TapNo);
					kegs.Insert(arg.TapNo, arg);
				}
			});

			MyListView.ItemsSource = kegs;

			/* Register refresh command */
			MyListView.RefreshCommand = new Command( async() => {
				await LoadKegsBackground();

			});
			MyListView.RefreshCommand.Execute(this);
		}

		private async Task LoadKegsBackground()
		{
			/* Set the 'busy' state */
			IsBusy = true;
			MyListView.IsRefreshing = true;
			await kegModBtns.FadeTo(0.1, 100);
			numTaps = 0;
			KegItem keg = await getKeg(numTaps);

			while (keg != null)  {
				enableDelete = true;
				if (kegs.Count > numTaps) {
					kegs[numTaps] = keg;
				} else {
					kegs.Add(keg);
				}
				OnPropertyChanged();

				numTaps++;
				keg = await getKeg(numTaps);
			}

			await kegModBtns.FadeTo(1, 500);
			IsBusy = false;
			MyListView.IsRefreshing = false;
		}

		/*----------------------------------------------------------------------
         Get all kegs that have been created, no defined limit currently exists
        ----------------------------------------------------------------------*/
		async Task<KegItem> getKeg(int idx)
        {
			var keg = await manager.GetActiveKegAsync(idx);
            return (keg);
        }


		private void bleh(object sender, EventArgs e)
		{
			Task.Run(async() => await LoadKegsBackground());
			MyListView.EndRefresh();
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
		Create Keg
		----------------------------------------------------------------------*/
		async void OnAddKegBtnClicked(object sender, EventArgs args)
		{
			/* Disable while still loading kegs */
			if (IsBusy) { return; }

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
			/* Disable while still loading kegs */
			if (IsBusy) { return; }

			string s = await DisplayActionSheet(string.Format("Kegs must be deleted in descending order. \nDelete Keg {0}?", numTaps), "Cancel", "Delete Keg");
			if( s.Contains("Delete") && kegs.Count >= numTaps) {
				await manager.DeleteKegAsync(kegs[numTaps - 1]);

				kegs.RemoveAt(numTaps - 1);
				numTaps--;
				enableDelete = numTaps > 0;
			}
		}
	}
}
