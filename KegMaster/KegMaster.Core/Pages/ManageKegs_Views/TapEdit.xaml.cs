using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

using KegMaster.Core.Database;
using System.Globalization;
using System.Net;
using System.Text;
using System.IO;

namespace KegMaster.Core.Pages.ManageKegs_Views
{
    public partial class TapEdit : ContentPage
    {
		private const string FunctionURL = Constants.FunctionURL;
		private ConnectionManager manager = ConnectionManager.DefaultManager;

		private KegItem kegTapData;

		public TapEdit()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<KegMaster_pgView, KegItem>(this, "KegItem_EditContext", (sender, arg) => { this.kegTapData = arg; this.RefreshData(); });
		}

		void RefreshData()
        {
			/* Update page title */
			Page.Title = string.Format("Edit Tap {0}", this.kegTapData.TapNo+1);

            /* Update ViewModel Data */
            this.entryKegName.Text = string.Format("{0}", this.kegTapData.Name);
            this.entryKegStyle.Text = string.Format("{0}", this.kegTapData.Style);
            this.entryDescription.Text = string.Format("{0}", this.kegTapData.Description);
            this.startDatePicker.Date = this.kegTapData.DateKegged;
			this.endDatePicker.MinimumDate = this.kegTapData.DateKegged;
            this.endDatePicker.Date = this.kegTapData.DateAvail;

            this.entryPressureCurrent.Text = this.kegTapData.PressureCrnt.ToString("N2");
            this.entryPressureDsrd.Text = this.kegTapData.PressureDsrd.ToString("N2");
			this.entryPourQtyGlass.Text = this.kegTapData.PourQtyGlass.ToString("N2");
			this.entryPourQtySample.Text = this.kegTapData.PourQtySample.ToString("N2");
			this.entryQtyRemain.Text = this.kegTapData.QtyAvailable.ToString("N2");
            this.entryQtyReserve.Text = this.kegTapData.QtyReserve.ToString("N2");
            this.btnPourEn.Text = string.Format("{0}", this.kegTapData.PourEn ? "Lock Tap" : "Unlock Tap");
            this.btnPresEn.Text = string.Format("{0}", this.kegTapData.PressureEn ? "Turn CO2 Off" : "Turn CO2 On");

			PageLoading.IsVisible = false;
			PageContent.IsVisible = true;
		}

		/*
         * Pressure Button Pressed
         */
		void OnPressureBtnClicked(object sender, EventArgs args)
        {
			this.btnPresEn.Text = string.Format("{0}", this.btnPresEn.Text.ToLower().Equals("turn co2 on") ? "Turn CO2 Off" : "Turn CO2 On");
		}

		/*
         * Pour Button Pressed
         */
		void OnPourBtnClicked(object sender, EventArgs args)
        {
			this.btnPourEn.Text = string.Format("{0}", this.btnPourEn.Text.ToLower().Equals("unlock tap") ? "Lock Tap" : "Unlock Tap");
		}

		async void OnUpdateBtnClicked(object sender, EventArgs args)
		{
			PageLoading.IsVisible = true;
			PageContent.IsVisible = false;

			if(this.kegTapData.CreatedAt == null) {
				this.kegTapData.CreatedAt = DateTimeOffset.Now.ToString();
				await manager.CreateKegAsync(this.kegTapData);
			}

			this.kegTapData.Name = await updateColumnString("Name", this.kegTapData.Name, this.entryKegName.Text);
			this.kegTapData.Style = await updateColumnString("Style", this.kegTapData.Style, this.entryKegStyle.Text);
			this.kegTapData.Description = await updateColumnString("Description", this.kegTapData.Description, this.entryDescription.Text);

			/* Date kegged/avail and Pour Qty */
			this.kegTapData.DateKegged = await updateColumnDateTime("DateKegged", this.kegTapData.DateKegged, this.startDatePicker.Date);
			this.kegTapData.DateAvail = await updateColumnDateTime("DateAvail", this.kegTapData.DateAvail, this.endDatePicker.Date);
			var f_pg = float.Parse(this.entryPourQtyGlass.Text, CultureInfo.InvariantCulture.NumberFormat);
			var f_ps = float.Parse(this.entryPourQtySample.Text, CultureInfo.InvariantCulture.NumberFormat);

			this.kegTapData.PourQtyGlass = await updateColumnFloat("PourQtyGlass", this.kegTapData.PourQtyGlass, f_pg);
			this.kegTapData.PourQtySample = await updateColumnFloat("PourQtySample", this.kegTapData.PourQtySample, f_ps);

			/* Pressure and Qty */
			var f_pc = float.Parse(this.entryPressureCurrent.Text, CultureInfo.InvariantCulture.NumberFormat);
			var f_pd = float.Parse(this.entryPressureDsrd.Text, CultureInfo.InvariantCulture.NumberFormat);
			var f_qa = float.Parse(this.entryQtyRemain.Text, CultureInfo.InvariantCulture.NumberFormat);
			var f_qr = float.Parse(this.entryQtyReserve.Text, CultureInfo.InvariantCulture.NumberFormat);

			this.kegTapData.PressureCrnt = await updateColumnFloat("PressureCrnt", this.kegTapData.PressureCrnt, f_pc);
			this.kegTapData.PressureDsrd = await updateColumnFloat("PressureDsrd", this.kegTapData.PressureDsrd, f_pd);
			this.kegTapData.QtyAvailable = await updateColumnFloat("QtyAvailable", this.kegTapData.QtyAvailable, f_qa);
			this.kegTapData.QtyReserve = await updateColumnFloat("QtyReserve", this.kegTapData.QtyReserve, f_qr);

			/* These are a littl counter-intuitive, they display the opposite of the current state -- e.g. Pressure is 'on' if button displays 'off' */
			this.kegTapData.PourEn = await updateColumnBool("PourEn", this.kegTapData.PourEn, !this.btnPourEn.Text.ToLower().Contains("unlock"));
			this.kegTapData.PressureEn = await updateColumnBool("PressureEn", this.kegTapData.PressureEn, this.btnPresEn.Text.ToLower().Equals("turn co2 off"));

			this.kegTapData = await manager.GetActiveKegAsync(this.kegTapData.TapNo);
			
			MessagingCenter.Send<TapEdit, KegItem>(this, "KegItem_Updated", (Database.KegItem)this.kegTapData);

			await Navigation.PopAsync();
		}

		async Task<bool> updateColumnBool(string key, bool current, bool challenge)
		{
			bool ret = current;

			if (current != challenge) {
				await updateColumn_sendToDb(key, challenge.ToString());
				ret = challenge;
			}
			return (ret);
		}

		async Task<float> updateColumnFloat(string key, float current, float challenge)
		{
			float epsilon = 0.01f;
			float ret = current;

			if (Math.Abs(current - challenge) > epsilon) {
				await updateColumn_sendToDb(key, challenge.ToString());
				ret = challenge;
			}
			return(ret);
		}

		async Task<DateTime> updateColumnDateTime(string key, DateTime current, DateTime challenge)
		{
			DateTime ret = current;

			if( (current.Year != challenge.Year)
		     || (current.Month != challenge.Month)
			 || (current.Day != challenge.Day) ){
				await updateColumn_sendToDb(key, challenge.ToString());
				ret = challenge;
			}
			return (ret);
		}

		async Task<string> updateColumnString(string key, string current, string challenge)
		{
			string ret = current;
			if (challenge == null) {
				return ret;
			}

			// Ignore time, only compare date
			if( (current == null && challenge != null)
			  ||(!current.Equals(challenge))) {
				await updateColumn_sendToDb(key, challenge);
				ret = challenge;
			}
			return (ret);
		}

		async Task updateColumn_sendToDb(string key, string val)
		{
			Exception error = null;

			// Row accessed by ID and Tap Number
			string json = string.Format("Id:\"{0}\", TapNo:{1}, qrySelect:\"{2}\", qryValue:\"{3}\"", this.kegTapData.Id.ToString(), this.kegTapData.TapNo.ToString(), key, val);
			// This does nothing for now, but is planned for use to allow access/control for multiple different systems.
			json = string.Format("{{ReqId:\"\", {0}}}", json);

			try {
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(FunctionURL);
				req.Method = "POST";
				req.ContentType = "application/json";
				Stream stream = req.GetRequestStream();
				byte[] buffer = Encoding.UTF8.GetBytes(json);
				stream.Write(buffer, 0, buffer.Length);
				HttpWebResponse result = (HttpWebResponse)req.GetResponse();
			} catch (Exception ex) {
				error = ex;
			}
			if (error != null) {
				await DisplayAlert("There was an error", error.Message, "OK");
			}
		}
	}
}
