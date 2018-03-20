using UnityEngine;
using System;
using System.Collections;

using Vuforia;

using System.Threading;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;


[AddComponentMenu("System/VuforiaScanner")]
public class VuforiaScanner : MonoBehaviour
{   
	public delegate void QRCodeDetectedHandler(string qrcode);
	public event QRCodeDetectedHandler OnQRCodeDetected;

	private const float CUSTOM_UPDATE_SPEED = 0.2f;
    private BarcodeReader barCodeReader;

	private Image.PIXEL_FORMAT _mPixelFormat = Image.PIXEL_FORMAT.UNKNOWN_FORMAT;
	private RGBLuminanceSource.BitmapFormat _formatBit = RGBLuminanceSource.BitmapFormat.Unknown;

	private bool _mFormatRegistered = false;

    void Start()
    {      
		#if UNITY_EDITOR
			_mPixelFormat = Image.PIXEL_FORMAT.GRAYSCALE;			//Need Grayscale for Editor
			_formatBit = RGBLuminanceSource.BitmapFormat.Gray8;		//Need Grayscale for Editor
		#else
			_mPixelFormat = Image.PIXEL_FORMAT.RGB888;				//Need RGB888 for mobile
			_formatBit = RGBLuminanceSource.BitmapFormat.RGB32;		//Need RGB32 for mobile
		#endif

        barCodeReader = new BarcodeReader();

		VuforiaARController.Instance.RegisterVuforiaStartedCallback (OnVuforiaStarted);
		//VuforiaARController.Instance.RegisterTrackablesUpdatedCallback (OnTrackableUpdated);
		VuforiaARController.Instance.RegisterOnPauseCallback (OnPause);
    }

    private void OnVuforiaStarted()
    {
		_mFormatRegistered = CameraDevice.Instance.SetFrameFormat(_mPixelFormat, true);

		Debug.Log("FormatSet : " + _mFormatRegistered);

		if (_mFormatRegistered) 
		{
			// Force autofocus.
			var isAutoFocus = CameraDevice.Instance.SetFocusMode (CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
			if (!isAutoFocus) 
			{
				CameraDevice.Instance.SetFocusMode (CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
			}
			Debug.Log ("AutoFocus : " + isAutoFocus);
		}
    }

	public void StartScan()
	{
		if (_mFormatRegistered)
		{
			StartCoroutine ("CustomScanUpdate");
		}
	}

	public void StopScan()
	{
		if (_mFormatRegistered) 
		{
			StopCoroutine ("CustomScanUpdate");
		}
	}

	private IEnumerator CustomScanUpdate()
	{
		while (true) 
		{
			yield return new WaitForSeconds (CUSTOM_UPDATE_SPEED);
			Debug.Log ("Update");
			OnTrackableUpdated ();
		}
	}

    private void OnTrackableUpdated()
    {
		if (_mFormatRegistered) 
		{
			try 
			{
				var cameraFeed = CameraDevice.Instance.GetCameraImage (_mPixelFormat);

				if (cameraFeed == null) 
				{
					return;
				}
				var data = barCodeReader.Decode (cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight, _formatBit);
				if (data != null) 
				{
					// QRCode detected.
					Debug.Log (data.Text);

					if(OnQRCodeDetected != null)
						OnQRCodeDetected(data.Text);
				} 
				else 
				{
					Debug.Log ("No QR code detected !");
				}
			} 
			catch (Exception e) 
			{
				Debug.LogError (e.Message);
			}
		}
    }  

	private void OnPause(bool paused)
	{
		if (paused) 
		{
			Debug.Log ("App is paused");
		} 
		else 
		{
			Debug.Log ("App is runing");
		}	
	}
}
