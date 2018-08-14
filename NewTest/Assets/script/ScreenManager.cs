using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
  * 场景切换管理
  * @author 李程
  * */
public class ScreenManager:MonoBase
{

	public static ScreenManager Instance;
	int _index = -1;
	float _progress = 0;
	float _lastProgress = 0;
	AsyncOperation asy1;
	AsyncOperation asy2;
	float res1;
	float res2	;
	int phase = 0;//读取的阶段
	bool waitCache;

	public CallBack beginInCallBack;
	public CallBack begOutCallBack;

	public float progress {
		get {
			return _progress;
		}
		set {
			
			_progress = value;
		}
	}


	
	public void loadScreen (int p_index, CallBack _beginIn, CallBack _beginOut  )
	{
		if(p_index==1)
			waitCache=false;
		else
			waitCache=true;

		_index = p_index;
		UiManager.Instance.openDialogWindow<LoadingWindow> (
			(win)=>{
			//回调，cache必须有，该回调内要调用hideloadingwindow
			//如果beginOut有，那么在hideloadingwindow的时候会调用
			beginInCallBack = _beginIn;
			begOutCallBack = _beginOut;
			ScreenManager.Instance.beginLoad();
		});


		
	}
	
	public void loadScreen (int p_index)
	{

		loadScreen (p_index, null, null);
	}

	public void beginLoad ()
	{
		StartCoroutine (ie_loadScreen (_index));

	
	}

	void Update ()
	{
		
		if (UiManager .Instance.ActiveLoadingWindow == null || UiManager .Instance.ActiveLoadingWindow .justLoading==true)
			return;
			
		if (phase == 1) {
			if (asy1 != null && asy1.progress > res1)
				res1 = asy1.progress;
			progress = res1 * 0.2f;

		}
		
		if (phase == 2) {
			
			if (asy2 != null && asy2.progress > res2)
				res2 = asy2.progress;
			progress = 0.2f + res2 * 0.5f + ResourcesManager.Instance.cacheProgress * 0.3f;
		}

		
		if (progress < _lastProgress)
			return;

		_lastProgress = Mathf.Lerp (_lastProgress, progress, Time.deltaTime * 20);
		
		if (_lastProgress > 0.98f)
			_lastProgress = 1;

		UiManager .Instance.ActiveLoadingWindow.setProgress( _lastProgress);
	}
	
	private	IEnumerator ie_loadScreen (int p_index)
	{
		if(beginInCallBack!=null)
			beginInCallBack();

		phase = 1;
		asy1 = Application.LoadLevelAsync (2);
		//这里处于中间screen
	 
		phase = 2;
		ResourcesManager.Instance.releaseScreenResources ();
		asy2 = Application.LoadLevelAsync (p_index);	
		yield return asy2;


		if(waitCache==false)
			ResourcesManager.Instance.cacheProgress=1;
		while (ResourcesManager.Instance.cacheProgress<1) {
			yield return 0;
		}
		
		while (_lastProgress<1f) {
			yield return 0;
		}

		ResourcesManager.Instance.cacheProgress = 0;
		_lastProgress = 0;
		res1 = 0;
		res2 = 0;

		if(begOutCallBack!=null)
			begOutCallBack();
		UiManager.Instance.hideLoading ();			
		reset();

		//切换背景音乐		
		int mid = 1;
		if (p_index == 3) {

			if (BattleManager.lastMissionEvent != null && BattleManager.lastMissionEvent.eventType == MissionEventType.BOSS)
				mid = 4;
			else
				mid = Random.Range(2,4);
		}
		AudioManager.Instance.PlayMusic(mid);
	}

	void reset()
	{
		_index = -1;
	}

}
