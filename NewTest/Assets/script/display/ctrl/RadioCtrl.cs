using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 广播控制组件
/// </summary>
public class RadioCtrl : MonoBehaviour
{

	/** 滚动的预制件 */
	public GameObject labPrefab;
	private List<UILabel> freeLabs;
	private List<UILabel> runLabs;
	/** 定时器 */
	private Timer timer;
	/** 定时器数量 */
	private int timerCount;
	private int screenWidth;
	private Vector3 startPosition;
	private Vector3 endPosition;
	/** 广播类型 */
	public int radioType = 0;
	public int gap=100;
	public float duration=12;
	public float speed=70;
	public int maxLabWidth=500;
	public int startPosOffsetX=100;
	public int endPosOffsetX=100;

	void Awake()
	{
		freeLabs=new List<UILabel>();
		runLabs=new List<UILabel>();
	}
	void OnEnable()
	{
		screenWidth=Screen.width;
		startPosition=new Vector3(screenWidth/2+startPosOffsetX,0,0);
		endPosition=new Vector3(-screenWidth/2-endPosOffsetX,0,0);
		timerCount=0;
		timer=TimerManager.Instance.getTimer(2*1000);
		timer.addOnTimer(M_onTimer);
		timer.start();		
	}
	void OnDisable()
	{
		timer.stop();
		iTween.Stop(gameObject,true);
		int i=0,length=runLabs.Count;
		for(;i<length;i++)
		{
			Destroy(runLabs[i].gameObject);
		}
		runLabs.Clear();

		for(i=0,length=freeLabs.Count;i<length;i++)
		{
			Destroy(freeLabs[i].gameObject);
		}
		freeLabs.Clear();
	}
	private void M_onTimer()
	{
		timerCount++;
		bool check=true;
		UILabel curLab;
		if(runLabs.Count>0)
		{
			curLab=runLabs[runLabs.Count-1];
			if(curLab.transform.localPosition.x+curLab.width>screenWidth/2-gap)
			{
				check=false;
			}
		}
		if(check)
		{
			M_addNewMsg();
		}
	}

	private UILabel M_getNewLab()
	{

		UILabel label;
		if(freeLabs.Count>0)
		{
			label=freeLabs[0];
			freeLabs.RemoveAt(0);
		}else
		{
			GameObject newLab=Instantiate(labPrefab) as GameObject;
			newLab.transform.parent=transform;
			label=newLab.GetComponent<UILabel>();
		}
		label.gameObject.SetActive(true);
		label.transform.localPosition=startPosition;
		label.transform.localScale=Vector3.one;
		return label;
	}
	private void M_addNewMsg()
	{
		string newMsg=RadioManager.Instance.M_getLastRadioMsg(radioType);
		if(string.IsNullOrEmpty(newMsg)) {
			newMsg=RadioManager.Instance.M_getLastRadioTipMsg(radioType);
			if(string.IsNullOrEmpty(newMsg)) {
				if(timerCount%10==0) {
					newMsg=RadioManager.Instance.M_getRandomTip(radioType);	
				} else {
					return;
				}
			}
		}
		UILabel curLab;
		curLab=M_getNewLab();
		curLab.text=newMsg;
		runLabs.Add(curLab);
		Vector3 endP=endPosition-new Vector3(curLab.width,0,0);
		Hashtable args=new Hashtable();
		args.Add("position", endP);
		args.Add("islocal",true);
		args.Add("oncomplete", "M_onMoveCmp");
		args.Add("oncompletetarget",gameObject);
		args.Add("speed", speed);
		args.Add("easetype", iTween.EaseType.linear);
		iTween.MoveTo(curLab.gameObject,args);
	}
	public void M_onMoveCmp()
	{
		UILabel _target=runLabs[0];
		_target.gameObject.SetActive(false);
		runLabs.Remove(_target);
		freeLabs.Add(_target);
	}
}

