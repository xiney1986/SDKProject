using UnityEngine;
using System.Collections;

/**   
 * 指引点位信息模板
 *@author 汤琦
 **/
public class GuidePointSample : Sample
{
	public GuidePointSample ()
	{
		
	}

	/** 点位类型（1 3D 2 2D） */
	public int pointType;
	/** 指向的目标点的路径 */
	public string targetPath;
	/** 文字说明的位置（0 无 1 上 2 中 3 下） */
	public int texLocal;
	/** 指向目标点的箭头的旋转（0 向右 90 向上 -90 向下 180向左） */
	public int arrowRot;
	/** 事件触控类型（1 按钮 2 全屏 3 滑动） */
	public int clickType;

	override public void parse (int sid, string str)
	{ 
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 5);

		//strArr[0] is sid  
		//strArr[1] pointType
		this.pointType = StringKit.toInt (strArr [1]);
		//strArr[2] targetPath
		this.targetPath = strArr [2]; 
		//strArr[3] texLocal
		this.texLocal = StringKit.toInt (strArr [3]); 
		//strArr[4] arrowRot
		this.arrowRot = StringKit.toInt (strArr [4]); 
		//strArr[5] clickType
		this.clickType = StringKit.toInt (strArr [5]);

	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}

public class GuidePointType
{
	public const int POINT3D = 1;//3D点位
	public const int POINT2D = 2;//2D点位
}

public class GuideTexLocalType
{
	/** 无 */
	public const int NO = 0;
	/** 上 */
	public const int TOP = 1;
	/** 中 */
	public const int CENTER = 2;
	/** 下 */
	public const int BOTTOM = 3;
}

public class GuideClickType
{
	/** 按钮 */
	public const int BUTTON = 1;
	/** 全屏 */
	public const int SCREEN = 2;
	/** 滑动 */
	public const int SLIDE = 3;
	/** 友善按钮 */
	public const int FRIENDLY_BUTTON = 4;
}
