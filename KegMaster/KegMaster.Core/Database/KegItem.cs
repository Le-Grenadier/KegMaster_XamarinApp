using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace KegMaster.Core.Database
{
    public class KegItem
    {
        string id;
        string alerts;
        int tapNo;
        string name;
        string style;
        string description;
        DateTime dateKegged;
        DateTime dateAvail;
        bool pourEn;
        bool pourNotification;
        float pourQtyGlass;
        float pourQtySample;
        float pressureCrnt;
        float pressureDsrd;
        float pressureDwellTime;
        bool pressureEn;
        float qtyAvailable;
        float qtyReserve;

        /*
         * Get/Set Properties - Json for Query
         *
         * One database row per keg, updated until replaced with new keg. Other
         * (future) tables to contain history for individual kegs.
         */
        [JsonProperty(PropertyName = "Id")]        public string Id { get { return id; } set { id = value; } }
        [JsonProperty(PropertyName = "Alerts")]
        public string Alerts { get { return alerts; } set { alerts = value;  } }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get { return name; } set { name = value; } }
        [JsonProperty(PropertyName = "Style")]
        public string Style { get { return style; } set { style = value; } }
        [JsonProperty(PropertyName = "Description")]
        public string Description { get { return description; } set { description = value; } }
        [JsonProperty(PropertyName = "DateKegged")]
        public DateTime DateKegged { get { return dateKegged; } set { dateKegged = value; } }
        [JsonProperty(PropertyName = "DateAvail")]
        public DateTime DateAvail { get { return dateAvail; } set { dateAvail = value; } }
        [JsonProperty(PropertyName = "PourEn")]
        public bool PourEn { get { return pourEn; } set { pourEn = value; } }
        [JsonProperty(PropertyName = "PourNotification")]
        public bool PourNotification { get { return pourNotification; } set { pourNotification = value; } }
        [JsonProperty(PropertyName = "PourQtyGlass")]
        public float PourQtyGlass { get { return pourQtyGlass; } set { pourQtyGlass = value; } }
        [JsonProperty(PropertyName = "PourQtySample")]
        public float PourQtySample { get { return pourQtySample; } set { pourQtySample = value; } }
        [JsonProperty(PropertyName = "PressureCrnt")]
        /* Doesn't make sense to 'set' the current pressure from the app.... */
        public float PressureCrnt { get { return pressureCrnt; } set { pressureCrnt = value; } }
        [JsonProperty(PropertyName = "PressureDsrd")]
        public float PressureDsrd { get { return pressureDsrd; } set { pressureDsrd = value; } }
        [JsonProperty(PropertyName = "PressureDwellTime")]
        /* Doesn't make sense to 'set' the pressure dwell time from the app... */
        public float PressureDwellTime { get { return pressureDwellTime; } set { pressureDwellTime = value; } }
        [JsonProperty(PropertyName = "PressureEn")]
        public bool PressureEn { get { return pressureEn; } set { pressureEn = value; } }
        [JsonProperty(PropertyName = "QtyAvailable")]
        /* Setting the quantity available seems sub-optimal, should probably be a calibration instead */
        public float QtyAvailable { get { return qtyAvailable; } set { qtyAvailable = value; } }
        [JsonProperty(PropertyName = "QtyReserve")]
        public float QtyReserve { get { return qtyReserve; } set { qtyReserve = value; } }
        [JsonProperty(PropertyName = "TapNo")]
        public int TapNo { get { return tapNo; } set { tapNo = value; } }
        [Version]        public string Version { get; set; }
    }
}
