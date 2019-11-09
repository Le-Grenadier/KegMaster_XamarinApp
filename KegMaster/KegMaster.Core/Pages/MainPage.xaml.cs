using System;
using System.Net.Http;
using Microsoft.Identity.Client;
using KegMaster.Core.Features.LogOn;
using Xamarin.Forms;

using KegMaster.Core.Pages.ManageKegs_Views;
using System.Diagnostics;
using System.Threading.Tasks;
using KegMaster.Core.Pages;

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

		async void OnCredits(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new KegMaster_Credits());
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

			/* Hide Credits page to reload it nicely below */
			btnCredits.Opacity = !isSignedIn ? 0 : 1;
			btnCredits.IsVisible = true;

			/*--------------------------------------------
			Prior to fade, make buttons visible but
			opacity == 0 if transitioning to signed in. 
			--------------------------------------------*/
			labelWelcome.Opacity = !isSignedIn ? 0 : 1;
			btnManageBeverage.Opacity = isSignedIn ? 0 : 1;
			btnEditProfile.Opacity = isSignedIn ? 0 : 1;
			btnResetPassword.Opacity = isSignedIn ? 0 : 1;
			btnCredits.Opacity = isSignedIn ? 0 : 1;

			if (isSignedIn) {
				/* Fade welcome and credits first if signing in, else last */
				await labelWelcome.FadeTo(Convert.ToDouble(!isSignedIn), 250);
				await btnCredits.FadeTo(Convert.ToDouble(false), 250);

				/* Make buttons visible */
				btnManageBeverage.IsVisible = isSignedIn;
				btnEditProfile.IsVisible = isSignedIn;
				btnResetPassword.IsVisible = isSignedIn;
				btnCredits.IsVisible = true;
			}

			/* Fade Buttons */
			await btnManageBeverage.FadeTo(Convert.ToDouble(isSignedIn), 250);
			await btnEditProfile.FadeTo(Convert.ToDouble(isSignedIn), 250);
			await btnResetPassword.FadeTo(Convert.ToDouble(isSignedIn), 250);

			/*--------------------------------------------
			Post fade, make buttons invisible but
			opacity == 0 if transitioning to signed in. 
			--------------------------------------------*/
			if (!isSignedIn) {
				/* Fade credits first if signing in, else last -- prior to changing number of elements on page though */
				await btnCredits.FadeTo(Convert.ToDouble(false), 250);

				/* Make buttons visible */
				btnManageBeverage.IsVisible = isSignedIn;
				btnEditProfile.IsVisible = isSignedIn;
				btnResetPassword.IsVisible = isSignedIn;
				btnCredits.IsVisible = true;

				/* Fade welcome first if signing in, else last */
				await labelWelcome.FadeTo(Convert.ToDouble(!isSignedIn), 250);
			}

			/* Credits button is special case, always fade in */
			await btnCredits.FadeTo(1, 250);

		}
	}
}