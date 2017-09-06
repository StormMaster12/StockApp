using Android.Manifest;
using Android.annotation.SuppressLint;
using Android.annotation.TargetApi;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera;
using Android.Build;
using Android.View;
using Android.Support.Annotation;
using Android.Gms.Common.Images;
using Android.Gms.Vision;


namespace StockApp.UI
{
	  public class CameraSource
	  {
		public readonly int CAMERA_FACING_BACK = CameraInfo.CAMERA_FACING_BACK;
		public readonly int CAMERA_FACING_FRONT = CameraInfo.CAMERA_FACING_FRONT;
		
		private readonly string TAG = "OpenCameraSource";
		
		private readonly int DUMMY_TEXTURE_NAME = 100;
		
		private readonly float = ASPECT_RATIO_TOLERANCE = 0.01f;
		
		private Context mContext;
		private Camera mCamera;
		
		private int mFacing = CAMERA_FACING_BACK;
		pricate int mRotation;
		private Size mPreviewSize;
		
		private float mRequestedFps = 30.0f;
		private int mRequestedPreviewWidth = 1024;
		private int mRequestedPreviewHeight = 768;
		
		private string mfocusMode = null;
		private string mFlashMode = null;
		
		private Thread mProcessingThread;
		private FrameProcessingRunnable mFrameProcessor;
		
		private Map<byte[], ByteBuffer> mBytesToBuffer = new HashMap<>();
		
		public class Builder
		{
		  private Detector<?> mDetector;
		  private CameraSource mCameraSource = new CameraSource();
		  
		  public Builder (Context context, Detector<?> detector)
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
		  
		  public Builder setRequestdfps(float fps_
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
			readonly int MAX = 1000000;
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
			if ( (facing != CAMERA_FACING_BACK) && (facing != CAMERA_FACING_FRONT)
			{
				throw new IllegalArgumentException("Invalid Camera" + facing);
			}
			
			mCameraSource.mFacing = facing;
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
				
				if(Build.VERSION.SDK_INT >= Build.VESRSON_CODES.HONEYCOMB)
				{
					mDummySurfaceTexture = new SurfaceTexture(DUMMY_TEXTURE_NAME);
					mCamera.PreviewTexture = mDummySurfaceTexture;
				}
				else
				{
					mDummySurfaceView = new SurfaceView(mContext);
					mCamera.PreviewDisplay = mDummySurfaceView.Holder;
				}
				mCamera.startPreview();
				
				mProcessingThread = new Thread(mFrameProcessor);
				mFrameProcessor.Active = true;
				mProcessingThread.start();
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
				mCamera.setPreviewDisplay(surfaceHolder);
				mCamera.startPreview();
				
				mProcessingThread = new Thread(mFrameProcessor);
				mFrameProcessor.Active = true;
				mProcessingThread.start();
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
						mProcessingThread.join()
					}
					catch (InterruptedException e)
					{
						Log.d(TAG, ""Frame processing thread interrupted on release.");
					}
					mProcessingThread = null;
				}
				
				mBytesToByteBuffer.clear();
				
				if(mCamera != null)
				{
					mCamera.stopPreview();
					mCamera.PreviewCallbackWithBuffer = null;
					try
					{
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB) {
							mCamera.PreviewTexture = null;

						} else 
						{
							mCamera.PreviewDisplay = null;
						}
					catch (Exception e)
					{
						Log.e(TAG, "Failed to clear camera preview: " + e);
					}
					mCamera.release();
					mCamera = null;
				}
			}
		}
	}
}
    
