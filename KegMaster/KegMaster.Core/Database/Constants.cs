﻿using System;
namespace KegMaster.Core.Database
{
    public class Constants
    {
        public static string ApplicationURL = @"https://kegmasterappservice.azurewebsites.net";
        public const string FunctionURL    = @"https://kegmasterfunc.azurewebsites.net/api/Function2?code=nTSzJgPLwTIMEkje7vugzFjJ8AmnbwWkLe3ZlaqkjEfF75GHuYq06Q==";

		public const string ListenConnectionString = "Endpoint=sb://kegmasternotifications.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=DMSA/O+pbDnKekgbseWrnBNlhiBVDHQhlxrQDAVS6UM=";
		public const string NotificationHubName = "KegMaster_NotificationHub";
	}
}
