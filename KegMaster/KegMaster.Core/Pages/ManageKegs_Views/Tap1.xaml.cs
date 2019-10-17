using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

using KegMaster.Core.Database;
using System.Globalization;

namespace KegMaster.Core.Pages.ManageKegs_Views
{
    public partial class Tap1 : ContentPage
    {
        KegItem kegTapData;
        KegItem updatedKeg;
        
        public Tap1()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<KegMaster, KegItem>(this, "KegItem_TabContext", async (sender, arg) =>
            { this.kegTapData = arg; this.RefreshData(); });

            entryKegName.TextChanged        += async (sender, args) => { await ItemChanged("Name",        args.NewTextValue); this.updatedKeg.Name       = args.NewTextValue; };
            entryKegStyle.TextChanged       += async (sender, args) => { await ItemChanged("Style",       args.NewTextValue); this.updatedKeg.Style      = args.NewTextValue; };
            entryDescription.TextChanged    += async (sender, args) => { await ItemChanged("Description", args.NewTextValue); this.updatedKeg.Description = args.NewTextValue; };

            entryQtyRemain.TextChanged      += async (sender, args) => { await ItemChanged("QtyAvailable", args.NewTextValue); this.updatedKeg.QtyAvailable = float.Parse(args.NewTextValue, CultureInfo.InvariantCulture.NumberFormat); };
            entryQtyReserve.TextChanged     += async (sender, args) => { await ItemChanged("QtyReserve",   args.NewTextValue); this.updatedKeg.QtyReserve = float.Parse(args.NewTextValue, CultureInfo.InvariantCulture.NumberFormat); };
            entryPressureDsrd.TextChanged   += async (sender, args) => { await ItemChanged("PressureDsrd", args.NewTextValue); this.updatedKeg.PressureDsrd = float.Parse(args.NewTextValue, CultureInfo.InvariantCulture.NumberFormat); };
            
        }

        public async void RefreshData()
        {
            /* Prepare for potential data updates */ 
            updatedKeg = kegTapData;

            /* Update ViewModel Data */
            this.labelKegName.Text = string.Format("Name: {0}", this.kegTapData.Name);
            this.labelKegStyle.Text = string.Format("Style: {0}", this.kegTapData.Style);
            this.labelDescription.Text = string.Format("Description: {0}", this.kegTapData.Description);
            this.labelDateKegged.Text = string.Format("Keg Date: {0:ddd, MMM d, yyyy}", this.kegTapData.DateKegged);
            this.labelDateAvailable.Text = string.Format("Serve Date: {0:ddd, MMM d, yyyy}", this.kegTapData.DateAvail);

            this.labelPressureCrnt.Text = string.Format("CrntPressure: {0:G2}", this.kegTapData.PressureCrnt);
            this.labelPressureDsrd.Text = string.Format("DsrdPressure: {0:G2}", this.kegTapData.PressureDsrd);
            this.labelQtyRemain.Text = string.Format("Qty Remain: {0:G2}", this.kegTapData.QtyAvailable);
            this.labelQtyReserve.Text = string.Format("Qty Reserve: {0:G2}", this.kegTapData.QtyReserve);
            this.btnPourEn.Text = string.Format("{0}", this.kegTapData.PourEn ? "Lock Tap" : "Unlock Tap");
            this.btnPresEn.Text = string.Format("{0}", this.kegTapData.PressureEn ? "Turn CO2 Off" : "Turn CO2 On");

        }

        public async Task ItemChanged(string key, string value)
        {
            MessagingCenter.Send<ContentPage, KegItem>(this, "KegItem_KegUpdate", updatedKeg );
            MessagingCenter.Send<ContentPage, string>(this, "KegItem_PushEmbeddedUpdate", string.Format("Id:\"{0}\", TapNo:{1}, qrySelect:\"{2}\", qryValue:\"{3}\"", kegTapData.Id.ToString(), kegTapData.TapNo.ToString(), key, value));
        }

        /*
         * Kegged Date Changed
         */
        async void OnKegDateSelected(object sender, DateChangedEventArgs args)
        {
            this.updatedKeg.DateKegged = args.NewDate;
            await ItemChanged("DateKegged", args.NewDate.ToString());
        }

        /*
         * Keg Serve Date Changed
         */
        async void OnServeDateSelected(object sender, DateChangedEventArgs args)
        {
            this.updatedKeg.DateAvail = args.NewDate;
            await ItemChanged("DateAvail", args.NewDate.ToString());
        }

        /*
         * Pressure Button Pressed
         */
        async void OnPressureBtnClicked(object sender, EventArgs args)
        {
            this.updatedKeg.PressureEn = !this.kegTapData.PressureEn;
            await ItemChanged("PressureEn", this.updatedKeg.PressureEn.ToString());
        }

        /*
         * Pour Button Pressed
         */
        async void OnPourBtnClicked(object sender, EventArgs args)
        {
            this.updatedKeg.PourEn = !this.kegTapData.PourEn;
            await ItemChanged("PourEn", this.updatedKeg.PourEn.ToString());
        }
    }
}
