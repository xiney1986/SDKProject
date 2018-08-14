using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PageSlider : MonoBase {

public UITable table;
public int pageCountMax;
	
  int activePage;
public List<UISprite>	pagePointList;
public UISprite pointObj;	
public string	imageON="pageslider_on";
public string	imageOFF="pageslider_off";
public int 	imageSide=29;
//public UILabel anOtherDisplayMethod;//when pagecount>8 ,this will be display.
	[HideInInspector]
public string textColorHex="eeee00";
	[HideInInspector]
public int pointNumForAnOtherDisPlay=12;
	
public UILabel anOtherDisplayMethod;
	
 void Start()
	{
	 pointObj.transform.localScale=new Vector3(0.001f,0.001f,0.001f);
	}
	
	 string GetText()
	{
		   return "["+textColorHex+"]"+activePage.ToString()+"/"+pageCountMax.ToString()+"[-]";
	}
	
	
	public void initPageSlider(int count)
	{
		 
		
		
			if(pagePointList==null) pagePointList=new List<UISprite>();
			
			else
			{
				
			   foreach(UISprite each in pagePointList)
				{
					
					DestroyImmediate(each.gameObject);
				}
				
				
				pagePointList.Clear();	
			}
		
///		     if(anOtherDisplayMethod!=null)
//		         DestroyImmediate(anOtherDisplayMethod.gameObject);
		     
		         pageCountMax=count;
				
		
				if(count==0)
				{
					transform.localScale=new Vector3(0.001f,0.001f,0.001f);
					return;
				}
				else {
			       
			         transform.localScale=Vector3.one;
			
			 
			
			        if(pageCountMax>pointNumForAnOtherDisPlay)
					{ //lable



				         anOtherDisplayMethod.gameObject.SetActive(true);
				      //   table.gameObject.transform.localPosition=new Vector3(0,5,0);
				
				          
					}else
					{//point
				     
					      for(int i=0;i<count;i++)
						{
					if(anOtherDisplayMethod!=null)
							 anOtherDisplayMethod.gameObject.SetActive(false);
							UISprite tmp=Instantiate(pointObj) as UISprite;
							tmp.gameObject.name="pagePoint_"+i.ToString();
							tmp.transform.parent=table.transform;
							tmp.transform.localPosition=new Vector3(0,0,0);
							tmp.transform.localScale=new Vector3(1 ,1 ,1);
							tmp.enabled=true;
							//tmp.gameObject.layer=12;
							pagePointList.Add(tmp);
						}
				
						float width=(pointObj.width+table.padding.x)*count;
						table.repositionNow=true;
						table.gameObject.transform.localPosition=new Vector3(-width*0.5f,table.transform.localPosition.y,table.transform.localPosition.z);
						
							
					}
			
			
			          
			       
				}
			/////////////////////
					
	
 		setActivePage(1);
		//gameObject.transform.localScale=new Vector3(1,1,1);
	}
	
	
	
	public void setActivePage(int count)
	{
		if(count==0) return;
		if(pageCountMax==0)return;
		
		if(count>pageCountMax && pageCountMax>0) return;
	
		//if(count>12) return;
		if(table.transform.childCount==0) return;
		
		//if (pagePointList == null || pagePointList.Count == 0)
		//	return;
		activePage=count;
		
		if(pageCountMax<=pointNumForAnOtherDisPlay)
		{
			pagePointList[count-1].spriteName=imageON;
			
			foreach(UISprite each in pagePointList)
			{
				if(each!=pagePointList[count-1])
				{
					
					each.spriteName=imageOFF;
				}
			}
			
		}else
		{
			anOtherDisplayMethod.text=GetText();
		}
	
		
		
	}
	
		public int  getActivePage(   )
	{
		
 return activePage;
		
		
	}

	
}
