using System;
using System.Net.Http;
using Microsoft.Identity.Client;
using KegMaster.Core.Features.LogOn;
using Xamarin.Forms;

using KegMaster.Core.Pages.ManageKegs_Views;

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
                if (btnSignInSignOut.Text == "Sign in")
                {
                    var userContext = await authenticationService.SignInAsync();
                    UpdateSignInState(userContext);
                    UpdateUserInfo(userContext);
                }
                else
                {
                    var userContext = await authenticationService.SignOutAsync();
                    UpdateSignInState(userContext);
                    UpdateUserInfo(userContext);
                }
            }
            catch (Exception ex)
            {
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                    OnPasswordReset();
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
                UpdateSignInState(userContext);
                UpdateUserInfo(userContext);
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
                UpdateSignInState(userContext);
                UpdateUserInfo(userContext);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }
        async void OnPasswordReset()
        {
            try
            {
                var userContext = await authenticationService.ResetPasswordAsync();
                UpdateSignInState(userContext);
                UpdateUserInfo(userContext);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        void UpdateSignInState(UserContext userContext)
        {
            var isSignedIn = userContext.IsLoggedOn;
            btnSignInSignOut.Text = isSignedIn ? "Sign out" : "Sign in";
            btnEditProfile.IsVisible = isSignedIn;
            slUser.IsVisible = isSignedIn;
            lblApi.Text = "";

            btnManageBeverage.IsVisible = isSignedIn ? true : false;
        }
        public void UpdateUserInfo(UserContext userContext)
        {
            lblName.Text = userContext.Name;
            lblJob.Text = userContext.JobTitle;
            lblCity.Text = userContext.City;
        }
    }
}