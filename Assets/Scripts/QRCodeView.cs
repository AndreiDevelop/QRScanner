using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QRCodeView : MonoBehaviour 
{
	private const string BARCODE = "Barcode";

	[SerializeField]
	private Text _qrCodeText = null;

	[SerializeField]
	private VuforiaScanner _vuforiaScanner = null;

	void OnEnable()
	{
		_vuforiaScanner.OnQRCodeDetected += ChangeQRCodeViewText;
	}

	void OnDisable()
	{
		_vuforiaScanner.OnQRCodeDetected -= ChangeQRCodeViewText;
	}

	// Use this for initialization
	private void ChangeQRCodeViewText (string newQRCode) 
	{
		_qrCodeText.text = BARCODE + " : " + newQRCode;
	}
}
