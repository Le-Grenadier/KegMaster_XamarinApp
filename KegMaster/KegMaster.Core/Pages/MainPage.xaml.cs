using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using Microsoft.Identity.Client;

using KegMaster.Core.Pages;
using KegMaster.Core.Features.LogOn;
using System.Text.RegularExpressions;
using System.Net;
using UIKit;
using Foundation;

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
			btnSignOut.IsEnabled = false;
		}

		async void OnManageKegs(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new KegMaster_pgView());
        }

		async void OnSignIn(object sender, EventArgs e) {
			if (IsBusy) { return; }
			IsBusy = true;
			try {
				var userContext = await authenticationService.SignInAsync();
				await UpdateSignInState(userContext);
				btnSignOut.IsEnabled = true;
			} catch (Exception ex)
            {
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                    await OnPasswordReset();
                // Alert if any exception excluding user cancelling sign-in dialog
                //else
                //    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
			IsBusy = false;
		}

		async void OnSignOut(object sender, EventArgs e) {
			if (IsBusy) { return; }
			IsBusy = true;
			try {
				var userContext = await authenticationService.SignOutAsync();
				await UpdateSignInState(userContext);
				btnSignOut.IsEnabled = false;
			} catch (Exception ex) {
				// Checking the exception message 
				// should ONLY be done for B2C
				// reset and not any other error.
				if (ex.Message.Contains("AADB2C90118"))
					await OnPasswordReset();
				// Alert if any exception excluding user cancelling sign-in dialog
				else
					await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
			}
			IsBusy = false;
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
            btnSignOut.IsEnabled = isSignedIn;
			
			/*--------------------------------------------
			Prior to fade, make buttons visible but
			opacity == 0 if transitioning to signed in. 
			--------------------------------------------*/
			labelWelcome.Opacity = !isSignedIn ? 0 : 1;
			btnSignIn.Opacity = !isSignedIn ? 0 : 1;
			btnManageBeverage.Opacity = isSignedIn ? 0 : 1;
			btnEditProfile.Opacity = isSignedIn ? 0 : 1;
			btnResetPassword.Opacity = isSignedIn ? 0 : 1;
			btnCredits.Opacity = 0.7;
			btnSupport.Opacity = 0.7;

			if (isSignedIn) {
				/* Fade welcome and credits first if signing in, else last */
				await labelWelcome.FadeTo(Convert.ToDouble(!isSignedIn), 250);
				await btnSignIn.FadeTo(Convert.ToDouble(false), 250);
				await btnCredits.FadeTo(Convert.ToDouble(false), 250);
				await btnSupport.FadeTo(Convert.ToDouble(false), 250);

				/* Make buttons visible */
				btnManageBeverage.IsVisible = isSignedIn;
				btnEditProfile.IsVisible = isSignedIn;
				btnResetPassword.IsVisible = isSignedIn;
				btnCredits.IsVisible = true;
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
				await btnCredits.FadeTo(0.7*Convert.ToDouble(false), 250);
				await btnSupport.FadeTo(0.7 * Convert.ToDouble(false), 250);

				/* Make buttons visible */
				btnManageBeverage.IsVisible = isSignedIn;
				btnEditProfile.IsVisible = isSignedIn;
				btnResetPassword.IsVisible = isSignedIn;
				btnCredits.IsVisible = true;
				btnSupport.IsVisible = true;

				/* Fade welcome first if signing in, else last */
				await labelWelcome.FadeTo(Convert.ToDouble(!isSignedIn), 250);
			}

			/* Credits button is special case, always fade in */
			await btnSignIn.FadeTo(Convert.ToDouble(!isSignedIn), 250);
			await btnCredits.FadeTo(0.7, 250);
			await btnSupport.FadeTo(0.7, 250);


		}
		async void OnCredits(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new KegMaster_Credits());
		}

		[Obsolete]
		// TODO: Figure out what I should be calling instead of 'Device.OpenUri"
		async void OnSupport(object sender, EventArgs e)
		{
			string text = "Thanks for your interest in providing feedback! To help me support you, please complete all sections below.\n\n\n**Description of Request:**\n\n**Use Case OR Steps to reproduce:**\n\n**Your contact information**\n\n";
			var subject = Regex.Replace(Title, @"[^\u0000-\u00FF]", string.Empty);
			var body = Regex.Replace(text, @"[^\u0000-\u00FF]", string.Empty);
			var email = Regex.Replace("John@JohnGrenard.com", @"[^\u0000-\u00FF]", string.Empty);
			var cr_url = "https://gitlab.johngrenard.com/JohnGrenard/kegmaster_xamarinapp/issues";
			string uri = String.Empty;

			const string SendEmail = "Send an Email";
			const string CreateCR = "Request a Change To Software";
			string action = await DisplayActionSheet("Contact Support", "Cancel", null, SendEmail, CreateCR);
			switch (action) {
				case SendEmail:
					if (Device.RuntimePlatform == Device.iOS) {
						uri = "mailto:" + email + "?subject=" + WebUtility.UrlEncode(subject).Replace("+", "%20") + "&body=" + WebUtility.UrlEncode(body).Replace("+", "%20");

						if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl(uri)))
							await Task.FromResult(UIApplication.SharedApplication.OpenUrl(new NSUrl(uri)));
						else
							await DisplayAlert("Unable to auto-open Email App.", "\nPlease email John@JohnGrenard.com with comments or questions about this app.\n\nBe sure to include a detailed description, so I may best support you.\n\nThanks!", "Back");
					} else {
						//for Android it is not necessary to code nor is it necessary to assign a destination email
						uri = "mailto:" + email + "?subject=" + Title + "&body=" + text;
						Device.OpenUri(new Uri(uri));
					}
					break;

				case CreateCR:
					Device.OpenUri(new Uri(cr_url));
					break;

				default:
					// Do Nothing 
					break;
			}
		}
	}
}