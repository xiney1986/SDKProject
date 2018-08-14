using UnityEngine;
using System.Collections;

public class TalkCtrl : MonoBase
{

	public TalkObjAnimCtrl animCtrl;
	public UILabel talkText;
	public UILabel talkerName;
	public UITexture image;
	//public UISprite talkPen;
	public GameObject talkArrow;
	public UISprite talkJiantou;
	public UISprite talkQuan1;
	public UISprite talkQuan2;
	public UISprite frame;
	//public UISprite shadow; 
//	public UISprite background;
	public TalkWindow fatherWindow;

	public void changeDepth(bool isTop)
	{
		if(isTop){
			talkText.depth=210;
			talkerName.depth=210;
			image.depth=200;
			//talkPen.depth=215;
			//GameObject.Find
			talkArrow.SetActive(true);
			talkJiantou.depth =215;
			talkQuan1.depth = 214;
			talkQuan2.depth = 214;

			frame.depth=203;
			//shadow.depth=204;
//			background.depth=205;
		}else{
			talkText.depth=10;
			talkerName.depth=10;
			image.depth=0;
			//talkPen.depth=15;
			talkArrow.SetActive(false);
			talkJiantou.depth =15;
			talkQuan1.depth = 14;
			talkQuan2.depth = 14;
			frame.depth=3;
			//shadow.depth=4;
//			background.depth=5;
		}


	}

	void move (Vector3 data)
	{
		transform.localPosition = data;
	}

	public void changeBlack ()
	{
		foreach (Transform each in animCtrl.transform) {
			UIWidget wid = each.GetComponent<UIWidget> ();
			if (wid == null)
				continue;
			//if (wid.name == shadow.name)   //防止对话框上面的阴影的透明度被改变
			//	continue;
			if (wid.name == talkerName.name)  //不改变对话人名字的颜色
				continue;
			wid.color = new Color (0.2f, 0.2f, 0.2f, 1);
		}
		//talkPen.alpha=0;
		talkArrow.SetActive(false);


	}
	
	public void changeLight ()
	{
		foreach (Transform each in animCtrl.transform) {
			UIWidget wid = each.GetComponent<UIWidget> ();
			if (wid == null)
				continue;
			//if (wid.name == shadow.name)   //防止对话框上面的阴影的透明度被改变
			//	continue;
			if (wid.name == talkerName.name)  //不改变对话人名字的颜色
				continue;
			wid.color = new Color (1f, 1f, 0.9f, 1f);
		}
		//talkText.color=new Color(1f,0.8f,0.52f);
		//talkerName.color=new Color(1f,0.9f,0.8f);
	
	}

	public void talkIn (int intoLoc)
	{
		switch (intoLoc) {
		case 1:
			animCtrl.AnimType = ObjAnimType.TopIn;
			break;
		case 2:
			animCtrl.AnimType = ObjAnimType.RightIn;
			break;
		case 3:
			animCtrl.AnimType = ObjAnimType.BottomIn;
			break;
		case 4:
			animCtrl.AnimType = ObjAnimType.LeftIn;
			break;
		}
		animCtrl.ObjAnimBegin ();

	}

	public void 	talkOut (int outLoc)
	{
		switch (outLoc) {
		case 1:
			animCtrl.AnimType = ObjAnimType.TopOut;
			break;
		case 2:
			animCtrl.AnimType = ObjAnimType.RightOut;
			break;
		case 3:
			animCtrl.AnimType = ObjAnimType.BottomOut;
			break;
		case 4:
			animCtrl.AnimType = ObjAnimType.LeftOut;
			break;
		}
		animCtrl.ObjAnimBegin ();
	}	

	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
