using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace KegMaster.Core.Database
{
	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class KegItem
	{
		/* Database values */
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

		/* Constructor - These values cannot be null when inserting data */
		public KegItem()
		{
			this.TapNo = 0;
			this.Id = Guid.NewGuid().ToString("N");
			this.DateAvail = DateTime.Now;
			this.DateKegged = DateTime.Now;
			this.CreatedAt = DateTimeOffset.Now.ToString();
			this.Deleted = "False";
		}
		/*
         * Get/Set Properties - Json for Query
         *
         * One database row per keg, updated until replaced with new keg. Other
         * (future) tables to contain history for individual kegs.
         */
		[JsonProperty(PropertyName = "Id")]
		public string Id { get { return id; } set { id = value; } }
		[JsonProperty(PropertyName = "Alerts")]
		public string Alerts { get { return alerts; } set { alerts = value; } }
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
		[Version]
		public string Version { get; set; }
		[CreatedAt]
		public string CreatedAt { get; set; }
		[UpdatedAt]
		public string UpdatedAt { get; set; }
		[Deleted]
		public string Deleted { get; set; }

		/* My special values specific to implimentation of this app */
		[JsonIgnoreAttribute]
		public float QtyCaution { get { return qtyReserve * 2.0f; } }
	}

}
