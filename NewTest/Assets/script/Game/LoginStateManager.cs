using UnityEngine;
using System.Collections;

public class LoginStateManager : MonoBehaviour
{
	private bool _isResourceCache;
	private bool _isConnectOK;
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_isResourceCache && _isConnectOK) {
			//isResourceCache=false;
			_isConnectOK=false;
			UserManager.Instance.gotoMainWindow();
		}
	}

	public bool isResourceCache {
		set {
			_isResourceCache = value;
		}
		get {
			return _isResourceCache;
		}
	}

	public bool isConnectOK {
		set {
			MaskWindow.LockUI();
			_isConnectOK = value;
		}
		get {
			return _isConnectOK;
		}
	}
}

