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
    class BarcodeTrackerFactory : MultiProcessor.IFactory
    {
        private GraphicOverlay<BarcodeGraphic> mgraphicOverlay;


        public BarcodeTrackerFactory(GraphicOverlay<BarcodeGraphic> graphicOverlay)
        {
            mgraphicOverlay = graphicOverlay;
        }

        public IntPtr Handle => throw new NotImplementedException();

        public Tracker Create(Java.Lang.Object obj)
        {
            BarcodeGraphic graphic = new BarcodeGraphic(mgraphicOverlay);
            return new BacrodeGraphicTracker(mgraphicOverlay, graphic);
        }

        public void Dispose()
        {
        }
    }
}