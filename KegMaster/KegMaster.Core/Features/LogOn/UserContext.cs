using System;

namespace KegMaster.Core.Features.LogOn
{
	/* Static okay here -- only one user allowed at a time */
	public static class User
	{
		public static UserContext data;

		public static UserContext InitUser(UserContext u)
		{
			data = u.Clone();

			return(data);
		}
	}

		public class UserContext
    {
        public string Name { get; internal set; }
        public string UserIdentifier { get; internal set; }
        public bool IsLoggedOn { get; internal set; }
        public string GivenName { get; internal set; }
        public string FamilyName { get; internal set; }
        public string Province { get; internal set; }
        public string PostalCode { get; internal set; }
        public string Country { get; internal set; }
        public string EmailAddress { get; internal set; }
        public string JobTitle { get; internal set; }
        public string StreetAddress { get; internal set; }
        public string City { get; internal set; }
        public string AccessToken { get; internal set; }

		public Boolean IsAdmin { get { return(JobTitle != null && JobTitle.Equals("Admin")); } }

		public UserContext Clone()
		{
			return ((UserContext)this.MemberwiseClone());
		}

	}
}