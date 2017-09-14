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

using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using StockApp.UI;
using Java.Lang;

namespace StockApp.BarcodeReader
{
    class BacrodeGraphicTracker : Tracker
    {
        private GraphicOverlay mOverlay;
        private GraphicOverlay.Graphic mGraphic;

        public BacrodeGraphicTracker(GraphicOverlay overlay, GraphicOverlay.Graphic graphic)
        {
            mOverlay = overlay;
            mGraphic = graphic;
        }

        public override void OnNewItem(int id, Java.Lang.Object item)
        {
            mGraphic.mId = id;
        }

        public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
        {
            mOverlay.Add(mGraphic);
            mGraphic.updateItem((Barcode)item);
        }

        public override void OnMissing(Detector.Detections detections)
        {
            mOverlay.Remove(mGraphic);
        }

        public override void OnDone()
        {
            mOverlay.Remove(mGraphic);
        }
    }
}