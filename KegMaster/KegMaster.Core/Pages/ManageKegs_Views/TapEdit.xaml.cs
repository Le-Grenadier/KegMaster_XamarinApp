﻿using System;
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
using System.Threading;

namespace KegMaster.Core.Pages.ManageKegs_Views
{
    public partial class TapEdit : ContentPage
    {
		private const string FunctionURL = Constants.FunctionURL;
		private ConnectionManager manager = ConnectionManager.DefaultManager;
		private KegItem kegTapData;

		public TapEdit()
        {
			/* This back button change will take effect on any pages loaded from this one */
			InitializeComponent();

			/* This is to set the current page state */
			MessagingCenter.Subscribe<KegMaster_pgView, KegItem>(this, "KegItem_EditContext", (sender, arg) => { this.kegTapData = arg; this.RefreshData(); });
		}

		void RefreshData()
        {
			/* Update page title */
			Page.Title = string.Format("Tap {0}", this.kegTapData.TapNo+1);

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
            this.btnPourEn.IsToggled = this.kegTapData.PourEn;
            this.btnPresEn.IsToggled = this.kegTapData.PressureEn;

			Boolean isNewKeg = (this.kegTapData.CreatedAt == null);
			btnPresUpdt.Text = isNewKeg ? "Create New Keg" : "Update Keg";

			PageLoading.IsVisible = false;
			PageContent.Opacity = 1;
		}

		async void OnUpdateBtnClicked(object sender, EventArgs args)
		{
			PageLoading.IsVisible = true;
			PageContent.Opacity = 0.5;
			PageContent.IsEnabled = false;

			if( this.kegTapData.CreatedAt == null ) {
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

			/* These are a little counter-intuitive, they display the opposite of the current state -- e.g. Pressure is 'on' if button displays 'off' */
			this.kegTapData.PourEn = await updateColumnBool("PourEn", this.kegTapData.PourEn, this.btnPourEn.IsToggled);
			this.kegTapData.PressureEn = await updateColumnBool("PressureEn", this.kegTapData.PressureEn, this.btnPresEn.IsToggled);

			this.kegTapData = await manager.GetActiveKegAsync(this.kegTapData.TapNo);

			/* Make robust in the event a user also presses the back button */
			Device.StartTimer(TimeSpan.FromMilliseconds(2000), () =>
			{
				if (Page.IsVisible) {
					Navigation.PopAsync();
				}
				return false;
			});

			MessagingCenter.Send<TapEdit, KegItem>(this, "KegItem_Updated", (Database.KegItem)this.kegTapData);
		}

		protected override void OnDisappearing()
		{
			Page.IsVisible = false;
		}

		async Task<bool> updateColumnBool(string key, bool current, bool challenge)
		{
			bool ret = current;

			if (current != challenge) {
				ret = challenge;
				await updateColumn_sendToDb(key, challenge.ToString());
			}

			return (ret);
		}

		async Task<float> updateColumnFloat(string key, float current, float challenge)
		{
			float epsilon = 0.01f;
			float ret = current;

			if (Math.Abs(current - challenge) > epsilon) {
				ret = challenge;
				await updateColumn_sendToDb(key, challenge.ToString());
			}

			return (ret);
		}

		async Task<DateTime> updateColumnDateTime(string key, DateTime current, DateTime challenge)
		{
			DateTime ret = current;

			if( (current.Year != challenge.Year)
		     || (current.Month != challenge.Month)
			 || (current.Day != challenge.Day) ){
				ret = challenge;

				/* Enable Keg at 12:30 PM by default */
				await updateColumn_sendToDb(key, challenge.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
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
				ret = challenge;
				await updateColumn_sendToDb(key, challenge);
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
