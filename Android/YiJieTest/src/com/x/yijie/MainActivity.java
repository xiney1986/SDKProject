package com.x.yijie;

import android.os.Bundle;
import android.os.Handler;

import com.snowfish.cn.ganga.helper.*;
import com.unity3d.player.*;

public class MainActivity extends UnityPlayerActivity {
	public static Handler hd = new Handler();

	protected void onCreate(Bundle arg0) {
		super.onCreate(arg0);
		SFOnlineHelper.onCreate(this, new SFOnlineInitListener() {
			public void onResponse(String tag, String value) {
				if (tag.equalsIgnoreCase("success")) {
					UnityPlayer.UnitySendMessage("SdkManager", "CreateResult",
							"success");
				} else if (tag.equalsIgnoreCase("fail")) {
					UnityPlayer.UnitySendMessage("SdkManager", "CreateResult",
							"fail");
				}
			}
		});
	}

	protected void onStop() {
		super.onStop();
		SFOnlineHelper.onStop(this);
	}

	protected void onDestroy() {
		super.onDestroy();
		SFOnlineHelper.onDestroy(this);
	}

	protected void onResume() {
		super.onResume();
		hd.postDelayed(new Runnable() {
			public void run() {
				SFOnlineHelper.onResume(MainActivity.this);
			}
		}, 1000);
	}

	protected void onPause() {
		super.onPause();
		SFOnlineHelper.onPause(this);
	}

	protected void onRestart() {
		super.onRestart();
		SFOnlineHelper.onRestart(this);
	}
}
