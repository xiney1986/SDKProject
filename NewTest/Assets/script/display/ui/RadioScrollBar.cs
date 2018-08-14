using UnityEngine;
using System.Collections;

public class RadioScrollBar : MonoBehaviour {

	public UIAtlas atlas;
	public string normalSpriteName = "pageslider_off";
	public string checkSpriteName = "pageslider_on";
	public UIPanel scrollPanel;
	public int pageCount;
	public int spriteDepth;
	public int space = 10;

	float lastOffsetX = 100000000;
	int lastCheckIndex = -1;
	UISprite[] buttons;

	// Use this for initialization
	void Start () {
        if (pageCount > 0)
        {
            Init(pageCount);
        }
	}

    public void Init(int pageCount)
    {
        this.pageCount = pageCount;
        Utils.DestoryChilds(gameObject);
        buttons = new UISprite[pageCount];
        float width = 0;
        for (int i = 0; i < pageCount; i++) {
            UISprite us = NGUITools.AddChild<UISprite>(gameObject);
            us.atlas = atlas;
            us.spriteName = normalSpriteName;
            us.depth = spriteDepth;
            us.MakePixelPerfect();
            if(width == 0)
                width = (us.width + space) * pageCount - space;
            float x = (us.width + space) * i - width / 2 + us.width / 2;
            us.transform.localPosition = new Vector3(x,0,0);
            buttons[i] = us;
        }
    }
	
	// Update is called once per frame
	void Update () {

		if (scrollPanel != null && pageCount > 0) {
			float x = scrollPanel.clipOffset.x;
			int i = (int)Mathf.Round (x / 615);
			check(i);
//			if (x != lastOffsetX) {
//				lastOffsetX = x;
//				int index = (int)Mathf.FloorToInt(x / scrollPanel.finalClipRegion.z);
//				check(index);
//			}
		}
	}

	public void check(int index)
	{
		if (index == lastCheckIndex)
			return;
		lastCheckIndex = index;
		if (index < 0 || index >= buttons.Length)
			return;
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i].spriteName = i == index ? checkSpriteName : normalSpriteName;
		}

		if(onChangePage!=null)
		{
			onChangePage(index);
		}
	}
	public CallBack<int> onChangePage;

}
