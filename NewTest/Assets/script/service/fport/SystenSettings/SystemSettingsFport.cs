using UnityEngine;
using System.Collections;

public class SystemSettingsFport : BaseFPort {

	private CallBack mCallback;

	public void GetInfo (CallBack callback) {
		mCallback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.SYSTEMSETTINGS_GETINFO);
		access (message);
	}

	public void Submit (CallBack callback, string args) {
		mCallback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.SYSTEMSETTINGS_SUBMIT);
		message.addValue ("info", new ErlString (args));
		access (message);
	}

	public override void read (ErlKVMessage message) {
		if (message.getValue ("msg") is ErlAtom) {
			string data = (message.getValue ("msg") as ErlAtom).Value;
			if (data == "ok") {
				if (mCallback != null)
					mCallback ();
			}
			else {
				MessageWindow.ShowAlert (data);
			}
		}
		else if (message.getValue ("msg") is ErlArray) {
			bool[] settings = SystemSettingsSampleManager.Instance.getDefualtSettings ();
			ErlType[] data = (message.getValue ("msg") as ErlArray).Value as ErlType[];
			if (data != null) {
				for (int i = 0; i < data.Length; i++) {
					ErlArray item = data [i] as ErlArray;
					settings [StringKit.toInt (item.Value [0].getValueString ()) - 1] = item.Value [1].getValueString () == "1";
				}
			}
            settings[settings.Length-1]=PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1)==1;
			SystemSettingsManagerment.Instance.UpdateSettings (settings);
			if (mCallback != null)
				mCallback ();
		}
	}
}
