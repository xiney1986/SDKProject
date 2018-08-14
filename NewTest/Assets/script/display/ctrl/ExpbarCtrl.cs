using UnityEngine;
using System.Collections;

//玩家获得exp面板exp显示控制器 
// @author 李程


public class ExpbarCtrl : barCtrl
{
	/** 老等级多出的经验 */
	long ExpNow;
	/** 新等级多出的经验 */
	long ExpTarget;
	/** 增量经验 */
	long expAll;
	/** 增量经验,用于数字变换底数 */
	long expTime;
	/** 滑块条的增长步长 */
	int sp;
	/** 新等级多出的经验百分点 */
	float finishPoint;
	/** 老等级多出的经验百分点 */
	float startPoint;
	/** 升了多少级 */
	int levelcount;
	/** 升级前的等级 */
	public int LevelNow;
	public UILabel  expDistance;

	/** 参数 */
	public int arg1;


	CallBack<int,int,bool> callbackNew;
	CallBack<int> callback;

	public CallBack endCall;
	bool beginExpAnim = false;
	private bool isUp = false;

	//jordenwu add 进度回调
	private CallBack<int> mProgressCallBack=null;
	public void SetProcessCallBack(CallBack<int> pc){
		mProgressCallBack=pc;
	}
	//
	
	public void init (LevelupInfo info)
	{
		if (SliderBar != null)
			SliderBar.value = 0;
		if (FrontBar != null)
			FrontBar.fillAmount = 0;
		
		ExpNow = 0;
		hasTrigger=false;

		expAll = info.newExp - info.oldExp;
		expTime = expAll;
		
		LevelNow = info.oldLevel;
		// 老等级升级所需要的经验
		long length = info.oldExpUp - info.oldExpDown;
		long newLength = length;
		// 老等级多出的经验
		ExpNow = info.oldExp - info.oldExpDown;
		// 新等级多出的经验
		ExpTarget = info.newExp - info.newExpDown;
		levelcount = info.newLevel - info.oldLevel;
		if (levelcount > 0) {
			//新等级升级所需要的经验
			newLength = info.newExpUp - info.newExpDown;
		}
		
		if (newLength == 0) {
			newLength = ExpTarget;
			finishPoint = 1;
		} else {
			finishPoint = (float)ExpTarget / (float)newLength;
		}
		
		//如果满了 那么显示条直接填满
		if (finishPoint >= 1) {
			finishPoint = 0.99f;
			SliderBar.value = 1;
		}

		if (length == 0)
			return;
		startPoint = (float)ExpNow / (float)length;
		if (startPoint == 1)//起始点不可能是1
			startPoint = 0;
		
		if (SliderBar != null)
			SliderBar.value = startPoint;
		if (FrontBar != null)
			FrontBar.fillAmount = startPoint;
		
		beginExpAnim = true;
		if (levelcount > 1) {
			sp = (int)((float)expAll / (levelcount * 4f));
		} else {
			sp = (int)((float)expAll / 4);
		}
	}

	public void setLevelUpCallBack (CallBack<int,int,bool> fun)
	{
		callbackNew = fun;
	}

	public void setLevelUpCallBack(CallBack<int> fun)
	{
		callback=fun;
	}


	public bool getIsUp ()
	{
		return isUp;
	}
	private bool hasTrigger=false;
	// Update is called once per frame
	void updateSliderBar ()
	{
		//满了升一级
		if (SliderBar.value >= 1) {
			levelcount -= 1;
			LevelNow += 1;
			SliderBar.value = 0;
			
			if (callback != null)
				callback (LevelNow);
			if(callbackNew!=null)
				callbackNew(LevelNow,arg1,hasTrigger);
			hasTrigger=true;
			isUp = true;
		}
		
		
		if (levelcount > 0) {
			//还大于0 ，还要升级几轮，直接搞满
			SliderBar.value += 4f * Time.deltaTime; 
			
		} else {
		//最后一轮了
			if (SliderBar.value < finishPoint) { 
				SliderBar.value = Mathf.Lerp (SliderBar.value, finishPoint, Time.deltaTime); 

			} 

			if (SliderBar.value - finishPoint >= 0)
			{
				SliderBar.value = finishPoint;
			} else if (SliderBar.value <= 0) {
				SliderBar.value = 0;
			}

			if (endCall != null)
			{
				endCall();
				endCall=null;
			}
		}
	}

	void updateFrontBar ()
	{
		
		if (FrontBar.fillAmount >= 1) {
			FrontBar.fillAmount = 0;
			levelcount -= 1;
			LevelNow += 1;

			if (callback != null)
				callback (LevelNow);
			if(callbackNew!=null)
				callbackNew(LevelNow,arg1,hasTrigger);

			hasTrigger=true;
			isUp = true;
		}
		
		
		if (levelcount > 0) {
			//大于0 ，还要升级几轮，直接搞满
			FrontBar.fillAmount += 4f * Time.deltaTime; 
			
		} else {
		
			if (FrontBar.fillAmount < finishPoint) { 
				
				FrontBar.fillAmount = Mathf.Lerp (FrontBar.fillAmount, finishPoint, Time.deltaTime); 
				
			} 
			

			if (FrontBar.fillAmount - finishPoint >= 0)
				FrontBar.fillAmount = finishPoint;
			
		}
	}
	
	protected override void updateBar ()
	{
		if (expDistance != null && expTime > 0) {
			//步进 减 
			expTime -= sp;
			expDistance .text = (int)expTime + "";
		}
		
		if (beginExpAnim == false)
			return;
		
		if (SliderBar != null) {
			updateSliderBar ();
			return;
		}
 
		if (FrontBar != null) {
			updateFrontBar ();
			return;
		}

		//jordenwu 通知步进
		if(mProgressCallBack!=null)mProgressCallBack(sp);
		//

	}
}
