using UnityEngine;
using System.Collections.Generic;

public class HappySundayContent : MonoBehaviour
{
    public HappySundayContentItem UI_ItemTemplate;
    public Transform UI_ItemContainer;

	public UILabel Timelabel;
	public UILabel activeDesc;

    private NoticeWindow mFatherWindow;
	public HappySundayNotice notice;


    private void Start()
    { 
        

    }

    private void onReceiveInit()
    {
        System.Collections.Hashtable table = HappySundaySampleManager.Instance.samples;
        System.DateTime date = TimeKit.getDateTime(ServerTimeKit.getSecondTime());
        int onlineDay = (ServerTimeKit.getSecondTime() - ServerTimeKit.onlineTime) / 3600 / 24;
        foreach (System.Collections.DictionaryEntry item in table)
        {
            HappySundaySample sample = item.Value as HappySundaySample;
			ActiveTime activeTime = ActiveTime.getActiveTimeByID(sample.timeID);
			if (sample.OnlineDay > onlineDay||activeTime.getIsFinish())
                continue;
            HappySundayContentItem target = Instantiate(UI_ItemTemplate) as HappySundayContentItem;
            target.transform.parent = UI_ItemContainer;
            target.transform.localScale = Vector3.one;
            target.SetData(sample, mFatherWindow);
            
        }
        UI_ItemContainer.GetComponent<UIGrid>().Reposition();
    }



    public void initContent(NoticeWindow win,Notice notice)
    {
		this.notice = notice as HappySundayNotice;
        mFatherWindow = win;
		updateUI();
        HappySundayManagerment.Instance.InitData(onReceiveInit);
    }

	public void updateUI()
	{
		Timelabel.text = notice.getStartTime()+"-"+notice.getEndTime();
		activeDesc.text = notice.getSample().activiteDesc;
	}
    




}

