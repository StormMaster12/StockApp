using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Support.V7.App;
using Android.Gms.Common;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using Android.Widget;
using StockApp.StockItems;

namespace StockApp.SignIn
{
    [Activity(Label = "SignIn")]
    public class SigninClass : AppCompatActivity,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, IResultCallback
    {
        private const string KEY_IS_RESOLVING = "is_resolving";
        private const string KEY_SHOULD_RESOLVE = "should_resolve";
        private const int RC_SIGN_IN = 9001;

        private GoogleApiClient mGoogleApiClient;
        private GoogleSignInOptions gso;
        private bool mIsResolving = false;
        private bool mShouldResolve = false;

        private TextView mStatusText;
        private SignInButton signInButton;
        private Button signOutButton;
        private Button disconnectButton;
        private ProgressDialog mProgressDialog;
        private IOnSignIn onSignIn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ItemsFragment itemsFragment;
            itemsFragment = ((StockAppApplicaiton)this.Application).ItemsFragment;
            onSignIn = itemsFragment;

            SetContentView(Resource.Layout.SignIn); // Create Layout

            if (savedInstanceState != null)
            {
                mIsResolving = savedInstanceState.GetBoolean(KEY_IS_RESOLVING);
                mShouldResolve = savedInstanceState.GetBoolean(KEY_SHOULD_RESOLVE);
            }

            signInButton = FindViewById(Resource.Id.sign_in_button) as SignInButton;
            mStatusText = FindViewById(Resource.Id.status) as TextView;
            signOutButton = FindViewById(Resource.Id.sign_out_button) as Button;
            disconnectButton =  FindViewById(Resource.Id.disconnect_button) as Button;

            signOutButton.Click += async delegate 
            {
                var result = await Auth.GoogleSignInApi.SignOut(mGoogleApiClient);
                UpdateUI(false);
                ((StockAppApplicaiton)this.Application).acct = null;
                ((StockAppApplicaiton)this.Application).SignedIn = false;
                onSignIn.update();
            };

            disconnectButton.Click += async delegate
            {
                var result = await Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient);
                UpdateUI(false);
                ((StockAppApplicaiton)this.Application).acct = null;
                ((StockAppApplicaiton)this.Application).SignedIn = false;
                onSignIn.update();
            };

            signInButton.Click += delegate {
                Console.WriteLine("I have been Clicked");
                Intent signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
                Console.WriteLine("I have Created the Intent");
                StartActivityForResult(signInIntent, RC_SIGN_IN);
                Console.WriteLine("I have started the Activity");
            };
            signInButton.SetSize(SignInButton.SizeWide);
            signInButton.Enabled = false;

            gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestIdToken("918121048883-kt94ua84h8cfus3dejf64okgqvngagm3.apps.googleusercontent.com")
                .RequestEmail()
                .Build();
            mGoogleApiClient = new GoogleApiClient.Builder(this)
                .EnableAutoManage(this, this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                .AddScope(new Scope(Scopes.Profile))
                .Build();
        }

        private void UpdateUI(bool isSignedIn)
        {
            if(isSignedIn)
            {
                signInButton.Visibility = ViewStates.Gone;
                FindViewById(Resource.Id.sign_out_and_disconnect).Visibility = ViewStates.Visible;
            }
            else
            {
                mStatusText.Text = GetString(Resource.String.signed_out);
                FindViewById(Resource.Id.sign_out_and_disconnect).Visibility = ViewStates.Gone;
                signInButton.Visibility = ViewStates.Visible;
                signInButton.Enabled = true;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == RC_SIGN_IN)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                handleSignInResult(result);
            }
        }

        private void handleSignInResult(GoogleSignInResult result)
        {
            if (result.IsSuccess)
            {
                GoogleSignInAccount acct = result.SignInAccount;
                mStatusText.Text = GetString(Resource.String.signed_in_fmt, acct.DisplayName);
                ((StockAppApplicaiton)this.Application).acct = acct;
                UpdateUI(true);
            }
            else
            {
                Console.WriteLine("Sign In Failed, Error Code: {0}", result.Status.StatusCode);
                UpdateUI(false);
            }
            onSignIn.update();
        }

        public void OnConnected(Bundle connectionHint)
        {
            UpdateUI(true);
        }

        protected override void OnStart()
        {
            base.OnStart();
            OptionalPendingResult opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
            if (opr.IsDone)
            {
                GoogleSignInResult result = opr.Get() as GoogleSignInResult;
                handleSignInResult(result);
            }
            else
            {
                showProgressDialog();
                opr.SetResultCallback(this);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            hideProgressDialog();
        }

        protected override void OnStop()
        {
            base.OnStop();
            if(mProgressDialog != null)
            {
                mProgressDialog.Dismiss();
            }
        }

        private void showProgressDialog()
        {
            if (mProgressDialog == null)
            {
                mProgressDialog = new ProgressDialog(this);
                mProgressDialog.SetMessage("Loading");
                mProgressDialog.Indeterminate = true;
            }

            mProgressDialog.Show();
        }

        private void hideProgressDialog()
        {
            if (mProgressDialog != null && mProgressDialog.IsShowing)
            {
                mProgressDialog.Hide();
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            if(!mIsResolving && mShouldResolve)
            {
                if(result.HasResolution)
                {
                    try
                    {
                        result.StartResolutionForResult(this, RC_SIGN_IN);
                        mIsResolving = true; 
                    }
                    catch
                    {
                        mIsResolving = false;
                        mGoogleApiClient.Connect();
                    }
                }
            }
            else
            {
                UpdateUI(false);
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            Console.Write("onConnection Suspended: {0}", cause);
        }

        public void OnResult(Java.Lang.Object result)
        {
            hideProgressDialog();
            handleSignInResult(result as GoogleSignInResult);
        }        
    }
}