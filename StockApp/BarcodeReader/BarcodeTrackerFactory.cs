using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Vision.Barcodes;
using Android.Gms.Vision;
using Java.Lang;

using StockApp.UI;

namespace StockApp.BarcodeReader
{
    class BarcodeTrackerFactory : Java.Lang.Object, MultiProcessor.IFactory
    {
        private GraphicOverlay mgraphicOverlay;


        public BarcodeTrackerFactory(GraphicOverlay graphicOverlay)
        {
            mgraphicOverlay = graphicOverlay;
        }

        public Tracker Create(Java.Lang.Object obj)
        {
            BarcodeGraphic graphic = new BarcodeGraphic(mgraphicOverlay);
            return new BacrodeGraphicTracker(mgraphicOverlay, graphic);
        }

    }
}