using System;
using System.Net.Http;
using Microsoft.Identity.Client;
using KegMaster.Core.Features.LogOn;
using Xamarin.Forms;

using KegMaster.Core.Pages.ManageKegs_Views;
using System.Diagnostics;
using System.Threading.Tasks;

namespace KegMaster.Core
{
    public partial class MainPage : ContentPage
    {
        protected readonly IAuthenticationService authenticationService;

        public MainPage()
        {
            InitializeComponent();

			/* Grab an instance of the IAuthenticationService using DependencyService.
             * 
             * NOTE: this will give us an instance of B2CAuthenticationService 
             * because we registered that class in App.xaml.cs
             * 
             * */

			authenticationService = DependencyService.Get<IAuthenticationService>();

        }

        async void OnManageKegs(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new KegMaster_pgView());
        }

        async void OnSignInSignOut(object sender, EventArgs e)
        {
            try
            {
                if (btnSignInSignOut.Text.ToLower().Equals("sign in"))
                {
                    var userContext = await authenticationService.SignInAsync();
                    await UpdateSignInState(userContext);
                }
                else
                {
                    var userContext = await authenticationService.SignOutAsync();
                    await UpdateSignInState(userContext);
                }
            }
            catch (Exception ex)
            {
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                    await OnPasswordReset();
                // Alert if any exception excluding user cancelling sign-in dialog
                else
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }
        async void OnEditProfile(object sender, EventArgs e)
        {
            try
            {
                var userContext = await authenticationService.EditProfileAsync();
                await UpdateSignInState(userContext);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }
        async void OnResetPassword(object sender, EventArgs e)
        {
            try
            {
                var userContext = await authenticationService.ResetPasswordAsync();
                await UpdateSignInState(userContext);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }
        async Task OnPasswordReset()
        {
            try
            {
                var userContext = await authenticationService.ResetPasswordAsync();
                await UpdateSignInState(userContext);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        async Task UpdateSignInState(UserContext userContext)
        {
			/* Update Sign In/Out button text */
            var isSignedIn = userContext.IsLoggedOn;
            btnSignInSignOut.Text = isSignedIn ? "Sign out" : "Sign in";

			/* Show/Hide Reset Password */
			btnResetPassword.Opacity = !isSignedIn ? 0 : 100;
			await btnResetPassword.FadeTo(Convert.ToDouble(!isSignedIn), 500);
			btnResetPassword.IsVisible = !isSignedIn;

			/* Show/Hide Manage Kegs */
			btnManageBeverage.Opacity = isSignedIn ? 0 : 100;
			btnManageBeverage.IsVisible = isSignedIn;

			/* Show/Hide Edit Profile */
			btnEditProfile.Opacity = isSignedIn ? 0 : 100;
			btnEditProfile.IsVisible = isSignedIn;

			/* Fade Buttons */
			await btnManageBeverage.FadeTo(Convert.ToDouble(isSignedIn), 500);
			await btnEditProfile.FadeTo(Convert.ToDouble(isSignedIn), 1000);
		}
	}
}