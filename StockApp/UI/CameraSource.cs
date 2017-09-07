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
using Java.Lang;
using Java.Nio;
using Java;
using Android.Hardware;

namespace StockApp.UI
{
	  public class CameraSource
	  {
        public readonly static int CAMERA_FACING_BACK = (int)Android.Hardware.CameraFacing.Back;
		public readonly static int CAMERA_FACING_FRONT = (int)Android.Hardware.CameraFacing.Front;

        private readonly string TAG = "OpenCameraSource";
		
		private readonly int DUMMY_TEXTURE_NAME = 100;
		
		private readonly float  ASPECT_RATIO_TOLERANCE = 0.01f;
		
		private Context mContext;
        private static object mCameraLock = new object();
		private static Android.Hardware.Camera mCamera;
		
		private int mFacing = CAMERA_FACING_BACK;
		private int mRotation;
		private Size mPreviewSize { get; set; }
		
		private float mRequestedFps = 30.0f;
		private int mRequestedPreviewWidth = 1024;
		private int mRequestedPreviewHeight = 768;
		
		private string mFocusMode = null;
		private string mFlashMode = null;
		
		private Thread mProcessingThread;
		private static FrameProcessingRunnable mFrameProcessor;

        private static SurfaceView mDummySurfaceView;
        private static SurfaceTexture mDummySurfaceTexture;


        private Dictionary<byte[], ByteBuffer> mBytesToBuffer = new Dictionary<byte[], ByteBuffer>();
		
		public class Builder
		{
          
		  private Detector mDetector;
		  private CameraSource mCameraSource = new CameraSource();
		  
		  public Builder (Context context, Detector detector)
		  {
			if (context == null)
			{
			  throw new IllegalArgumentException("No Context Supplied");
			}
			if (detector == null)
			{
			  throw new IllegalArgumentException("No Detector Supplied");
			}
			
			mDetector = detector;
			mCameraSource.mContext = context;
		  }
		  
		  public Builder setRequestdfps(float fps)
		  {
			mCameraSource.mRequestedFps = fps;
			return this;
		  }
		  
		  public Builder setFocusMode(string mode)
		  {
			mCameraSource.mFocusMode = mode;
			return this;
		  }
		  
		  public Builder setFlashMode(string mode)
		  {
			mCameraSource.mFlashMode = mode;
			return this;
		  }
		  
		  public Builder setRequestedPreviewSize(int width, int height)
		  {
			int MAX = 1000000;
			if( (width <= 0 ) || ( width > MAX ) || ( height <= 0 ) || ( height > MAX ) )
			{
				throw new IllegalArgumentException("Invalid Preview Size");
			}
			
			mCameraSource.mRequestedPreviewWidth = width;
			mCameraSource.mRequestedPreviewHeight = height;
			
			return this;
		  }
		  
		  public Builder setFacing(int facing)
		  {
			if ( (facing != CAMERA_FACING_BACK) && (facing != CAMERA_FACING_FRONT) )
			{
				throw new IllegalArgumentException("Invalid Camera" + facing);
			}
			
			mCameraSource.mFacing = facing;
            return this;
		  }
		  
		  public CameraSource build()
		  {
			mCameraSource.mFrameProcessor = mCameraSource.new FrameProcessingRunnable(mDetector);
			return mCameraSource;
		  }
		}
	
		public void release()
		{
			lock(mCameraLock)
			{
				stop();
				mFrameProcessor.release();
			}
		}
		
		public CameraSource start()
		{
			lock(mCameraLock)
			{
				if (mCamera != null)
				{
					return this;
				}
			
				mCamera = createCamera();
				
				if(Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
				{
					mDummySurfaceTexture = new SurfaceTexture(DUMMY_TEXTURE_NAME);
					mCamera.SetPreviewTexture(mDummySurfaceTexture);
				}
				else
				{
					mDummySurfaceView = new SurfaceView(mContext);
					mCamera.SetPreviewDisplay( mDummySurfaceView.Holder);
				}
				mCamera.StartPreview();
				
				mProcessingThread = new Thread(mFrameProcessor);
				mFrameProcessor.Active = true;
				mProcessingThread.Start();
			}
			return this;
		}
		
		public CameraSource start(SurfaceHolder surfaceHolder)
		{
			lock(mCameraLock)
			{
				if(mCamera != null)
				{
					return this;
				}
				
				mCamera = createCamera();
				mCamera.SetPreviewDisplay(surfaceHolder);
				mCamera.StartPreview();
				
				mProcessingThread = new Thread(mFrameProcessor);
				mFrameProcessor.Active = true;
				mProcessingThread.Start();
			}
			return this;
		}

		public void stop()
		{
			lock(mCameraLock)
			{
				mFrameProcessor.Active = false;
				if(mProcessingThread != null)
				{
					try
					{
                        mProcessingThread.Join();
					}
					catch (InterruptedException e)
					{
						Log.Debug(TAG, "Frame processing thread interrupted on release.");
					}
					mProcessingThread = null;
				}

                mBytesToBuffer.Clear();
				
				if(mCamera != null)
				{
					mCamera.StopPreview();
					mCamera.SetPreviewCallbackWithBuffer(null);
                    try
                    {
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
                        {
                            mCamera.SetPreviewTexture(null);

                        }
                        else
                        {
                            mCamera.SetPreviewDisplay(null);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Log.e(TAG, "Failed to clear camera preview: " + e);
                    }
					mCamera.Release();
					mCamera = null;
				}
			}
		}

        public int doZoom(float scale)
        {
            lock(mCameraLock)
            {
                if(mCamera == null)
                {
                    return 0;
                }

                int currentZoom = 0;
                int maxZoom;
                var parameters = mCamera.GetParameters();

                if(!parameters.IsZoomSupported)
                {
                    Log.Warn(TAG, "Zoom is not supported");
                    return currentZoom;
                }

                maxZoom = parameters.MaxZoom;

                currentZoom = parameters.Zoom + 1;
                float newZoom;
                if(scale > 1)
                {
                    newZoom = currentZoom + scale * (maxZoom / 10);
                }
                else
                {
                    newZoom = currentZoom + scale;
                }

                currentZoom = Java.Lang.Math.Round(newZoom) - 1;

                if(currentZoom < 0)
                {
                    currentZoom = 0;
                }
                else if(currentZoom > maxZoom)
                {
                    currentZoom = maxZoom;
                }

                parameters.Zoom = currentZoom;
                mCamera.SetParameters(parameters);
                return currentZoom;
            }
        }

        public int getCameraFacing()
        {
            return mFacing;
        }

        public void takePicture(ShutterCallback shutter, PictureCallback jpeg)
        {
            lock(mCameraLock)
            {
                if(mCamera != null)
                {
                    PictureStartCallback startCallback = new PictureStartCallback();
                    startCallback.mDelegate = shutter;

                    PictureDoneCallback doneCallback = new PictureDoneCallback();
                    doneCallback.mDelegate = jpeg;
                    mCamera.TakePicture(startCallback, null, null, doneCallback);
                }
            }
        }

        public string getFocusMode()
        {
            return mFlashMode;
        }

        public bool setFocusMode(string mode)
        {
            lock(mCameraLock)
            {
                if(mCamera != null && mode != null)
                {
                    var parameters = mCamera.GetParameters();
                    if (parameters.SupportedFocusModes.Contains(mode))
                    {
                        parameters.FocusMode = mode;
                        mFocusMode = mode;
                        return true;
                    }
                }
                return false;
            }
        }

        public string getFlashMode() { return mFlashMode; }

        public bool setFlashMode(string mode)
        {
            lock(mCameraLock)
            {
                if(mCamera != null && mode != null)
                {
                    var parameters = mCamera.GetParameters();
                    if (parameters.SupportedFocusModes.Contains(mode))
                    {
                        parameters.FlashMode = mode;
                        mFlashMode = mode;
                        return true;
                    }
                }
                return false;
            }
        }

        public void autoFocus(AutoFocusCallback cb)
        {
            lock(mCameraLock)
            {
                if(mCamera != null)
                {
                    
                }
            }
        }

        public void cancelAutoFocus()
        {
            lock (mCameraLock)
            {
                if (mCamera != null)
                {
                    mCamera.CancelAutoFocus();
                }
            }
        }

        public bool setAutoFocusMoveCallback(AutoFocusCallback cb)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
            {
                return false;
            }

            lock(mCameraLock)
            {
                if(mCamera != null)
                {
                    CameraAutoFocusCallback autoFocusCallback = false;
                    if(cb != null)
                    {
                        autoFocusCallback = new CameraAutoFocusCallback();
                        autoFocusCallback.mDelegate = cb;
                    }
                    mCamera.SetAutoFocusMoveCallback(autoFocusCallback);
                }
            }
            return true;
        }

        private CameraSource() { }

        private class CameraAutoFocusCallback : Java.Lang.Object, Android.Hardware.Camera.IAutoFocusCallback
        {
            public AutoFocusCallback mDelegate;

            public void OnAutoFocus(bool success, Android.Hardware.Camera camera)
            {
                if(mDelegate != null)
                {
                    mDelegate.onAutoFocus(success);
                }
            }
        }

        private class PictureStartCallback : Java.Lang.Object, Android.Hardware.Camera.IShutterCallback
        {
            public ShutterCallback mDelegate;

            public void OnShutter()
            {
                if(mDelegate != null)
                {
                    mDelegate.onShutter();
                }
            }
        }

        private class PictureDoneCallback : Java.Lang.Object, Android.Hardware.Camera.IPictureCallback
        {
            public PictureCallback mDelegate;

            public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
            {
                if (mDelegate != null)
                {
                    mDelegate.onPictureTaken(data);
                }

                lock(mCameraLock)
                {
                    if (mCamera != null)
                    {
                        mCamera.StartPreview();
                    }
                }
                
            }
        }

        public interface ShutterCallback
        {
            void onShutter();
        }

        public interface PictureCallback
        {
            void onPictureTaken(byte[] data);
        }

        public interface AutoFocusCallback
        {
            void onAutoFocus(bool success);
        }

        public interface AutoFocusMoveCallback
        {
            void onAutoFocusMoving(bool start);
        }

        private Android.Hardware.Camera createCamera()
        {
            int requestedCameraId = getIdForRequestedCamera(mFacing);

            if(requestedCameraId == -1)
            {
                throw new RuntimeException("Could Not Find Requested Camera");
            }
            Android.Hardware.Camera camera = Android.Hardware.Camera.Open(requestedCameraId);

            SizePair sizePair = selectSizePair(camera, mRequestedPreviewWidth, mRequestedPreviewHeight);
            if (sizePair == null)
            {
                throw new RuntimeException("Could not find suitable preview size");
            }

            Size pictureSize = sizePair.pictureSize();
            mPreviewSize = sizePair.previewSize();

            int[] previewfpsRange = selectPreviewFpsRange(camera, mRequestedFps);

            if(previewfpsRange.Count() == 0)
            {
                throw new RuntimeException("Could not find suitable preview frames per second range.");
            }

            var parameters = camera.GetParameters();

            if(pictureSize != null)
            {
                parameters.SetPictureSize(pictureSize.Width, pictureSize.Height);
            }
            parameters.SetPreviewFpsRange(previewfpsRange[(int)Android.Hardware.Camera.Parameters.PreviewFpsMinIndex],
                                          previewfpsRange[(int)Android.Hardware.Camera.Parameters.PreviewFpsMaxIndex]);
            parameters.PreviewFormat = ImageFormatType.Nv21;

            setRotation(camera, parameters, requestedCameraId);

            if(mFocusMode != null)
            {
                if(parameters.SupportedFocusModes.Contains(mFocusMode))
                {
                    parameters.FocusMode = mFocusMode;
                }
                else
                {
                    Log.Info(TAG, "Camera Focus Mode: " + mFocusMode + "is not supported on this device");
                }
            }

            mFocusMode = parameters.FocusMode;
            if (mFlashMode != null)
            {
                if (parameters.SupportedFlashModes != null)
                {
                    if (parameters.SupportedFlashModes.Contains(mFlashMode))
                    {
                        parameters.FlashMode = mFlashMode;
                    }
                    else
                    {
                        Log.Info(TAG, "Camera flash mode: " + mFlashMode + " is not supported on this device.");
                    }
                }
            }

            mFlashMode = parameters.FlashMode;

            camera.SetParameters(parameters);

            camera.SetPreviewCallbackWithBuffer(new CameraPreviewCallback());
            camera.AddCallbackBuffer(createPreviewBuffer(mPreviewSize));
            camera.AddCallbackBuffer(createPreviewBuffer(mPreviewSize));
            camera.AddCallbackBuffer(createPreviewBuffer(mPreviewSize));
            camera.AddCallbackBuffer(createPreviewBuffer(mPreviewSize));

            return camera;

        }

        private SizePair selectSizePair(Android.Hardware.Camera camera, int desiredWidth, int desiredHeight)
        {
            List<SizePair> validPreviewSizes = generateVaildPreviewSizeList(camera);
            SizePair selectedPair = null;

            int minDiff = int.MaxValue;
            foreach (SizePair sizePair in validPreviewSizes)
            {
                Size size = sizePair.previewSize();
                int diff = System.Math.Abs(size.Width - desiredWidth) + System.Math.Abs(size.Height - desiredHeight);

                if(diff < minDiff)
                {
                    selectedPair = sizePair;
                    minDiff = diff;
                }
            }

            return selectedPair;
        }

        private static int getIdForRequestedCamera(int facing)
        {
            Android.Hardware.Camera.CameraInfo cameraInfo = new Android.Hardware.Camera.CameraInfo();
            for (int i =0; i < Android.Hardware.Camera.NumberOfCameras; i++)
            {
                Android.Hardware.Camera.GetCameraInfo(i, cameraInfo);
                if((int)cameraInfo.Facing == facing)
                {
                    return i;
                }
            }
            return -1;
        }

        private List<SizePair> generateVaildPreviewSizeList(Android.Hardware.Camera camera)
        {
            var parameters = camera.GetParameters();
            List<Android.Hardware.Camera.Size> supportedPreviewSizes = (List<Android.Hardware.Camera.Size>)parameters.SupportedPreviewSizes;
            List<Android.Hardware.Camera.Size> supportedPictureSizes = (List<Android.Hardware.Camera.Size>)parameters.SupportedPictureSizes;
            List<SizePair> validPreviewSizes = new List<SizePair>();

            foreach (Android.Hardware.Camera.Size previewSize in supportedPreviewSizes)
            {
                float previewAspectRatio = (float)previewSize.Width / (float)previewSize.Height;
                
                foreach (Android.Hardware.Camera.Size pictureSize in supportedPictureSizes)
                {
                    float pictureAspectRatio = (float)pictureSize.Width / (float)pictureSize.Height;
                    if (System.Math.Abs(previewAspectRatio - pictureAspectRatio) < ASPECT_RATIO_TOLERANCE)
                    {
                        validPreviewSizes.Add(new SizePair(previewSize, pictureSize));
                        break;
                    }
                }
            }

            if (validPreviewSizes.Count == 0)
            {
                foreach (Android.Hardware.Camera.Size previewSize in supportedPreviewSizes)
                {
                    validPreviewSizes.Add(new SizePair(previewSize, null));
                }
            }

            return validPreviewSizes;
        }

        private class SizePair
        {
            private Size mPreview;
            private Size mPicture;

            public SizePair (Android.Hardware.Camera.Size previewSize, Android.Hardware.Camera.Size pictureSize)
            {
                mPreview = new Size(previewSize.Width, previewSize.Height);
                if(pictureSize != null)
                {
                    mPicture = new Size(pictureSize.Width, pictureSize.Height);
                }
            }

            public Size previewSize()
            {
                return mPreview;
            }

            public Size pictureSize()
            {
                return mPicture;
            }


        }

        private void setRotation(Android.Hardware.Camera camera, Android.Hardware.Camera.Parameters parameters, int cameraId )
        {
            IWindowManager windowManager = (IWindowManager)mContext.GetSystemService(Context.WindowService);
            int degrees = 0;
            int rotation = (int)windowManager.DefaultDisplay.Rotation;

            switch (rotation)
            {
                case (int)SurfaceOrientation.Rotation0:
                    degrees = 0;
                    break;
                case (int)SurfaceOrientation.Rotation90:
                    degrees = 90;
                    break;
                case (int)SurfaceOrientation.Rotation180:
                    degrees = 180;
                    break;
                case (int)SurfaceOrientation.Rotation270:
                    degrees = 270;
                    break;
                default:
                    Log.Error(TAG, "Bad Rotation Value:" + rotation);
                    break;
            }
            Android.Hardware.Camera.CameraInfo cameraInfo = new Android.Hardware.Camera.CameraInfo();
            Android.Hardware.Camera.GetCameraInfo(cameraId, cameraInfo);

            int angle;
            int displayAngle;
            if(cameraInfo.Facing == Android.Hardware.Camera.CameraInfo.CameraFacingFront)
            {
                angle = (cameraInfo.Orientation + degrees) % 360;
                displayAngle = (360 - angle) % 360;
            }
            else
            {
                angle = (cameraInfo.Orientation - degrees + 360) % 360;
                displayAngle = angle;
            }

            mRotation = angle / 90;

            camera.SetDisplayOrientation(displayAngle);
            parameters.SetRotation(angle);
        }

        private int[] selectPreviewFpsRange(Android.Hardware.Camera camera, float desiredPreviewFps)
        {
            int desiredPreviewFpsScaled = (int)(desiredPreviewFps * 1000.0f);

            int[] selectedFpsRange = null;
            int minDiff = int.MaxValue;
            List<int[]> previewFpsRangeList = (List<int[]>) camera.GetParameters().SupportedPreviewFpsRange;

            foreach (int[] range in previewFpsRangeList)
            {
                int deltaMin = desiredPreviewFpsScaled - range[(int)Android.Hardware.Preview.FpsMinIndex];
                int deltaMax = desiredPreviewFpsScaled - range[(int)Android.Hardware.Preview.FpsMaxIndex];
                int diff = System.Math.Abs(deltaMin) + System.Math.Abs(deltaMax);
                if (diff < minDiff)
                {
                    selectedFpsRange = range;
                    minDiff = diff;
                }
            }
            return selectedFpsRange;
        }

		private byte[] createPreviewBuffer(Size previewSize)
		{
			int bitsPerPixel = ImageFormat.getBitsPerPixel(ImageFormat.NV21);
			long sizeInBits = previewSize.getHeight() * previewSize.getWidth() * bitsPerPixel;
			int bufferSize = (int)System.Math.Ceil(sizeInBits/8.0d) + 1;
			
			byte[] byteArray = new byte[bufferSize];
			ByteBuffer buffer = ByteBuffer.wrap(byteArray);
			if(!buffer.hasarray() || buffer.array() != byteArray)
			{
				throw new IllegalStateException("Failed to create valid buffer for camera source.");
			}
			
			mBytesToByteBuffer.put(byteArray, buffer);
			return byteArray;
		}
		
        private class CameraPreviewCallback : Java.Lang.Object, Android.Hardware.Camera.IPreviewCallback
        {
            public void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera)
            {
                mFrameProcessor.setNextFrame(data, camera);
            }
        }
		
        private class FrameProcessingRunnable : Java.Lang.Object, IRunnable
        {
			private Detector mDetector;
			private long mStartTimeMillis = SystemClock.elapsedRealtime():
			private object mLock = new object();
			private bool mActive = true;
			private long mPendingTimeMillis;
			private int mPendingFrameId = 0;
			private ByteBuffer mPendingFrameData;
			
			public FrameProcessingRunnable(Detector detector)
			{
				mDetector = detector;
			}
			
			public void release()
			{
				assert(mProcessingThread.getState() == State.TERMINATED);
				mDetector.release();
				mDetector = null;
			}
			
			private void setActive(bool active)
			{
				lock(mLock)
				{
					mActive = active
					mLock.notifyAll();
				}
			}
			
			private void setNextFrame(byte[] data, Android.Hardware.Camera camera)
			{
				lock(mLock)
				{
					if(mPendingFrameData != null)
					{
						camera.addCallbackBuffer(mPendingFrameData.array());
						mPendingFrameData = null;
					}
					
					if (!mBytesToByteBuffer.containsKey(data))
					{
						Log.Debug("Skipping Frame, Could not Find ByteBuffer Associated with image");
						return
					}
					
					mPendingTimeMillis = SystemClock.elapsedRealtime() - mStartTimeMillis;
					mPendingFrameId ++;
					mLock.notifyAll();
			
            public void Run()
            {
                Frame outputFrame;
				ByteBuffer data;
				
				while (true)
				{
					lock(mLock)
					{
						while(mActive && mPendingFrameData == null)
						{
							try
							{
								mLock.wait();
							{
							catch (InterruptedException e)
							{
								Log.Debug(TAG, "Frame Processing Loop Terminated", e);
								return;
							}
						}
						
						if(!mAcitve) { return; }
						
						outputFrame = new Frame.Builder().setImageData(mPendingFrameData, mPreviewSize.getWidth(), mPreviewSize.getHeight(), ImageFormat.NV21)
														  .setId(mPendingFrameId)
														  .setTimestamp(mPendingTimeMillis)
														  .setRotation(mRotation)
														  .build();
														  
						data = mPendingFrameData;
						mPendingFrameData = null;
					}
					
					try
					{
						mDetector.receiveFrame(outputFrame);
					{
					catch(Throwable t)
					{
						Log.Exception(TAG, "Exception thrown from Reciever", t);
					}
					finaly
					{
						mCamera.addCallBackBuffer(data.array);
					}
				}
			}		
        }
    }
}
    
