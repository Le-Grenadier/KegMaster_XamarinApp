namespace KegMaster.Core.Features.LogOn
{
    public static class B2CConstants
    {
        // Azure AD B2C Coordinates
        public static string Tenant = "AADJohnG.onmicrosoft.com";
        public static string AzureADB2CHostname = "AADJohnG.b2clogin.com";
        public static string ClientID = "d933e53c-1a4b-40b6-9f8f-f9e034fba73a";
        public static string PolicySignUpSignIn = "b2c_1_signupsignin1";
        public static string PolicyEditProfile = "b2c_1_profileediting1";
        public static string PolicyResetPassword = "b2c_1_passwordreset1";

        public static string[] Scopes = { "https://AADJohnG.onmicrosoft.com/kegmaster/read", "https://AADJohnG.onmicrosoft.com/kegmaster/user_impersonation" };

        public static string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static string AuthoritySignInSignUp = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";
        public static string IOSKeyChainGroup = "com.microsoft.adalcache";
    }
}