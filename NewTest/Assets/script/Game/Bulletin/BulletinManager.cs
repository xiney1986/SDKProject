using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 宣传栏管理器
/// </summary>
public class BulletinManager {

	/** 宣传列表 */
	private List<Bulletin> bulletinList;

	public static BulletinManager Instance {
		get{ return SingleManager.Instance.getObj ("BulletinManager") as BulletinManager;}
	}

	/// <summary>
	/// 获得宣传列表
	/// </summary>
	public List<Bulletin> getButtletinList () {
		return bulletinList;
	}

	public void creatButtletinList (ErlKVMessage message) {
		//[[{sid:1},{top:测试活动公告},{state:1},{type:1},{title:二次元},{content:fuck~草泥马~好样的},{switch:0}],...]
		ErlType list = message.getValue ("msg") as ErlType;
		if (list != null && list is ErlList) {
			ErlList list_i = list as ErlList;
			string erlAttr;
			string erlName;
			int sid = 0;
			int state = 0;
			int type = 0;
			string name = "";
			string title = "";
			string desc = "";
			ErlList list_ii;

			bulletinList = new List<Bulletin> ();

			for (int i = 0; i < list_i.Value.Length; i++) {
				list_ii = list_i.Value [i] as ErlList;
				for (int j=0; j <list_ii.Value.Length; j++) {
					erlName = ((list_ii.Value [j] as ErlArray).Value [0] as ErlType).getValueString ();
					erlAttr = ((list_ii.Value [j] as ErlArray).Value [1] as ErlType).getValueString ();
					if (erlName == "sid") {
						sid = StringKit.toInt (erlAttr);
					}
					else if (erlName == "state") {
						state = StringKit.toInt (erlAttr);
					}
					else if (erlName == "type") {
						type = StringKit.toInt (erlAttr);
					}
					else if (erlName == "top") {
						name = erlAttr;
					}
					else if (erlName == "title") {
						title = erlAttr;
					}
					else if (erlName == "content") {
						desc = erlAttr;
					}
				}
				bulletinList.Add (new Bulletin (sid, state, type, name, title, desc));
			}

			if (bulletinList != null && bulletinList.Count > 0) {
				bulletinList.Sort ((Bulletin c1,Bulletin c2) => {
					return c1.sid > c2.sid ? 1 : -1;
				});
			}
		}
	}
}
