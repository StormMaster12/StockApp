using Android.App;
using Android.Gms.Auth.Api.SignIn;
using Android.Runtime;
using StockApp.HTTP;
using StockApp.StockItems;
using System;
using System.Collections.Generic;

namespace StockApp
{
    [Application]
    public class StockAppApplicaiton : Application
    {
        public GoogleSignInAccount acct { get; set; }
        public List<tescoApiJson> tescoApiList { get; set; }
        public ItemsFragment ItemsFragment { get; set; }
        public bool SignedIn { get; set; }
        private static StockAppApplicaiton instance;

        public StockAppApplicaiton(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            instance = this;
        }

        public static StockAppApplicaiton getconfig()
        {
            return instance;
        }

    }
}