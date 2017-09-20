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
using Android.Util;
using Android.Graphics;
using Java.Util;
using Android.Gms.Vision.Barcodes;


namespace StockApp.UI
{
    [Register("stockapp.stockapp.ui.GraphicOverlay")]
    class GraphicOverlay : View
    {
        private Object mLock = new object();
        private int mPreviewWidth { get; set; }
        private float mWidthScaleFactor { get; set; } = 1.0f;
        private int mPreviewHeight;
        private float mHeightScaleFactor { get; set; } = 1.0f;
        private int mFacing { get; set; } = (int)CameraFacing.Back;
        private HashSet<GraphicOverlay.Graphic> mGraphics = new HashSet<GraphicOverlay.Graphic>();

        public GraphicOverlay(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public void Clear()
        {
            lock(mLock)
            {
                mGraphics.Clear();
            }
        }

        public void Add(Graphic graphic)
        {
            lock(mLock)
            {
                mGraphics.Add(graphic);
            }
            PostInvalidate();
        }

        public void Remove(Graphic graphic)
        {
            lock(mLock)
            {
                mGraphics.Remove(graphic);
            }
            PostInvalidate();
        }

        public List<Graphic> getGraphics()
        {
            lock(mLock)
            {
                return mGraphics.ToList();
            }
        }

        public void setCameraInfo(int previewWidth, int previewHeight, int facing)
        {
            lock(mLock)
            {
                mPreviewHeight = previewHeight;
                mPreviewWidth = previewWidth;
                mFacing = facing;
            }

            PostInvalidate();
        }

        protected void onDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            lock(mLock)
            {
                if(mPreviewWidth !=0 && mPreviewHeight !=0)
                {
                    mWidthScaleFactor = (float)canvas.Width / (float)mPreviewWidth;
                    mHeightScaleFactor = (float)canvas.Height / (float)mPreviewHeight;
                }

                foreach (Graphic graphic in mGraphics)
                {
                    graphic.Draw(canvas);
                }
            }
        }

        public class Graphic
        {
            private GraphicOverlay mOverlay;
            public int mId { get; set; }

            private int[] ColourChoices = new int[] { Color.Blue, Color.Cyan, Color.Green };

            private int mCurrentColourIndex = 0;
            private Paint mRecPaint;
            private Paint mTextPaint;
            private volatile Barcode mbarcode;

            public Graphic(GraphicOverlay overlay)
            {
                mOverlay = overlay;
                mCurrentColourIndex = (mCurrentColourIndex + 1) % ColourChoices.Length;
                int selectedColour = ColourChoices[mCurrentColourIndex];

                mRecPaint = new Paint();
                mRecPaint.Color = new Android.Graphics.Color(selectedColour);
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

            public void Draw(Canvas canvas)
            {
                Barcode barcode = mbarcode;
                if (barcode == null)
                {
                    return;
                }

                RectF rect = new RectF(barcode.BoundingBox);
                rect.Left = translateY(rect.Left);
                rect.Right = translateY(rect.Right);
                rect.Top = translateY(rect.Top);
                rect.Bottom = translateY(rect.Bottom);

                canvas.DrawText(barcode.RawValue, rect.Left, rect.Bottom, mTextPaint);
            }

            public float scaleX(float horizontal) { return horizontal * mOverlay.mWidthScaleFactor; }
            public float scaleY(float vertical) { return vertical * mOverlay.mHeightScaleFactor; }

            public float translateX(float x)
            {
                float scale = scaleX(x);

                if(mOverlay.mFacing == (int)CameraFacing.Front)
                {
                    return mOverlay.Width - scale;
                }
                else
                {
                    return scale;
                }
            }

            public float translateY(float y) { return scaleY(y); }

            public void postInvalidate() { mOverlay.PostInvalidate(); }
        }



    }
}
