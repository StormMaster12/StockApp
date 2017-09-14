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
using Android.Graphics;
using Android.Gms.Vision.Barcodes;

using StockApp.UI;

namespace StockApp.BarcodeReader
{
    class BarcodeGraphic : GraphicOverlay.Graphic
    {
        public int mId { get; set; }

        private int[] ColourChoices = new int[] { Color.Blue, Color.Cyan, Color.Green };

        private int mCurrentColourIndex = 0;
        private Paint mRecPaint;
        private Paint mTextPaint;
        private volatile Barcode mbarcode;

        public BarcodeGraphic(GraphicOverlay overlay) : base(overlay)
        {
            mCurrentColourIndex = (mCurrentColourIndex + 1) % ColourChoices.Length;
            int selectedColour = ColourChoices[mCurrentColourIndex];

            mRecPaint = new Paint();
            mRecPaint.Color = new Android.Graphics.Color (selectedColour);
            mRecPaint.SetStyle(Paint.Style.Stroke);
            mRecPaint.StrokeWidth = 4.0f;

            mTextPaint = new Paint();
            mTextPaint.Color = new Android.Graphics.Color(selectedColour);
            mTextPaint.StrokeWidth = 36.0f;
        }

        public Barcode GetBarcode()
        {
            return mbarcode;
        }

        public void updateItem(Barcode barcode)
        {
            mbarcode = barcode;
            postInvalidate();
        }

        //public override void Draw(Canvas canvas)
        //{
        //    Barcode barcode = mbarcode;
        //    if(barcode == null)
        //    {
        //        return;
        //    }

        //    RectF rect = new RectF(barcode.BoundingBox);
        //    rect.Left = translateY(rect.Left);
        //    rect.Right = translateY(rect.Right);
        //    rect.Top = translateY(rect.Top);
        //    rect.Bottom = translateY(rect.Bottom);

        //    canvas.DrawText(barcode.RawValue, rect.Left, rect.Bottom, mTextPaint);
        //}


    }
}
