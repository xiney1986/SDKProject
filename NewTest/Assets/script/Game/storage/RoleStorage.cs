using System; 
using System.Collections;
 
/**
 * 角色仓库
 * */
public class RoleStorage:Storage
{
	public RoleStorage ()
	{
	
	}

	//获得指定品质
	public ArrayList getAllRoleOfQuality(int qualityId)
	{
		ArrayList cards=getStorageProp();
		ArrayList temp = new ArrayList();
		for (int i = 0; i < cards.Count; i++) {
			Card c=cards[i] as Card;
			if(c.getQualityId() == qualityId)
			{
				c.index=i;//重置索引
				temp.Add(c);
			}
		}
		return temp;
	}

	public ArrayList getRoleBySid(int sid)
	{
		ArrayList cards=getStorageProp();
		ArrayList temp = new ArrayList();
		for (int i = 0; i < cards.Count; i++) {
			Card c=cards[i] as Card;
			if(c.sid == sid)
			{
				c.index=i;//重置索引
				temp.Add(c);
			}
		}
		return temp;
	}

	//获得非祭品卡片
	public ArrayList getAllRoleByNotToEat ()
	{
		ArrayList cards=getStorageProp();
		ArrayList temp = new ArrayList();
		for (int i = 0; i < cards.Count; i++) {
			Card c=cards[i] as Card;
			if (!ChooseTypeSampleManager.Instance.isToEat(c,ChooseTypeSampleManager.TYPE_ADDON_NUM) && !ChooseTypeSampleManager.Instance.isToEat(c,ChooseTypeSampleManager.TYPE_MONEY_NUM)
			    && !ChooseTypeSampleManager.Instance.isToEat(c,ChooseTypeSampleManager.TYPE_SKILL_EXP)) {
				c.index=i;//重置索引
				temp.Add(c);
			}
		}
		return temp;
	}

	//获得献祭祭品卡片
	public ArrayList getAllRoleToSacrifice(int qualityId)
	{
		ArrayList cards=getStorageProp();
		ArrayList temp = new ArrayList();
		for (int i = 0; i < cards.Count; i++) {
			Card c=cards[i] as Card;
			if (ChooseTypeSampleManager.Instance.isToEat(c,ChooseTypeSampleManager.TYPE_SKILL_EXP)&&c.getQualityId() == qualityId) {
				c.index=i;//重置索引			
				temp.Add(c);
			}
		}
		return temp;
	}

	
	public override void parse (ErlArray arr)
	{ 
		ErlArray ea1 = arr.Value [1] as ErlArray;
		if (ea1.Value.Length <= 0) {
			init (StringKit.toInt (arr.Value [0].getValueString ()), null);
		} else {
			ArrayList al = new ArrayList ();
			Card card;
			for (int i=0; i < ea1.Value.Length; i++) {
				//uid,sid,exp,hplevel,attlevel,deflevel,magiclevel,agilelevel,被动技能,开场技能,主动技能,装备,状态
				//"ea2":[1,1,0,0,0,0,0,0,[],[],[[21001,0],[21004,0]],[],0]
				card=CardManagerment.Instance.createCard();
				card.bytesRead(0,ea1.Value [i] as ErlArray);
				al.Add (card);
			}
			init (StringKit.toInt (arr.Value [0].getValueString ()), al);
		}
	}
} 

