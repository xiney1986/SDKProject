using UnityEngine;
using System.Collections;

/**
 * 星星组件
 * 应用于进化的星级控制,每使用一次进化就填满一个星
 * @author 汤琦
 * */ 
public class StarGrade : MonoBase
{ 
	private const string solidStarName = "star";
	private const string hollowStarName = "star_b";
	public UISprite starName;
	public UISprite[] star;
	
	public void Initialize (int evolveCount)
	{

		int starCount = 4 - evolveCount;
		
		for (int i = 0; i < star.Length; i++) {
			if (i < starCount) {
				star [i].spriteName = solidStarName;
			} else {
				star [i].spriteName = hollowStarName;
			}
		}
	}
}
