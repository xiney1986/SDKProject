using UnityEngine;
using System.Collections;

/**
 * 进化星星管理
 * @author 陈世惟
 * */
public class EvolutionStarView : MonoBase
{

	public UISprite[] star;
	public UISprite[] starLine;

	public EffectCtrl[] effect_Small;
	public EffectCtrl[] effect_Medium;
	public EffectCtrl[] effect_Big;

	public EffectCtrl[] openEffect_Small;
	public EffectCtrl[] openEffect_Medium;
	public EffectCtrl[] openEffect_Big;

	public const int TYPE_SMALL = 0;
	public const int TYPE_MEDIUM = 1;
	public const int TYPE_BIG = 2;

	//0小、1中、2大
	private int[] star1 = new int[]{1,2,1,2,0};
	private int[] star2 = new int[]{1,2,2,1,0};
	private int[] star3 = new int[]{1,2,0,0,1};

	public void initStar (int group, int _surLv, int _shownum)
	{
		for (int i = 0; i<star.Length; i++) {
			if (star[i].transform.childCount > 0) {
				for (int j = 0; j < star[i].transform.childCount; j++) {
					Destroy(star[i].transform.GetChild(j).gameObject);
				}
			}
			if (i < _shownum) {
				if (i < starLine.Length)
					starLine [i].color = getColor (4);
					NGUITools.AddChild (star [i].gameObject, getOpenEffect (getBigOrSmall(group)[i], _surLv).gameObject);
					NGUITools.AddChild (star [i].gameObject, getEffect (getBigOrSmall(group)[i], _surLv).gameObject);
			}
		}
	}

	private int[] getBigOrSmall(int group)
	{
		switch (group) {
		case 0:
			return star1;
		case 1:
			return star2;
		case 2:
			return star3;
		default:
			return star3;
		}
	}

	private Color getColor (int lv)
	{
		switch (lv) {
		case 3:
			return Colors.EVOLUTIONCOLOR_BLUE;
		case 4:
			return Colors.EVOLUTIONCOLOR_PURPLE;
		case 5:
			return Colors.EVOLUTIONCOLOR_YELLOW;
		default:
			return Colors.EVOLUTIONCOLOR_YELLOW;
		}
	}

	/** 根据体积和等级获得相应颜色的升级特效 */
	private EffectCtrl getOpenEffect (int str, int lv)
	{
		if (str == TYPE_SMALL)
			return getOpenEffectByLv (lv) [0];
		else if (str == TYPE_MEDIUM)
			return getOpenEffectByLv (lv) [1];
		else if (str == TYPE_BIG)
			return getOpenEffectByLv (lv) [2];
		else
			return getOpenEffectByLv (lv) [2];
	}

	/** 根据等级获得相应颜色的升级特效组 */
	private EffectCtrl[] getOpenEffectByLv (int lv)
	{
		EffectCtrl[] effs = new EffectCtrl[3];
		switch (lv) {
		case 3:
			effs [0] = openEffect_Small [0];
			effs [1] = openEffect_Medium [0];
			effs [2] = openEffect_Big [0];
			return effs;
		case 4:
			effs [0] = openEffect_Small [1];
			effs [1] = openEffect_Medium [1];
			effs [2] = openEffect_Big [1];
			return effs;
		case 5:
			effs [0] = openEffect_Small [2];
			effs [1] = openEffect_Medium [2];
			effs [2] = openEffect_Big [2];
			return effs;
		default:
			effs [0] = openEffect_Small [2];
			effs [1] = openEffect_Medium [2];
			effs [2] = openEffect_Big [2];
			return effs;
		}
	}

	/** 根据体积和等级获得相应颜色的特效 */
	private EffectCtrl getEffect (int str, int lv)
	{
		if (str == TYPE_SMALL)
			return getEffectByLv (lv) [0];
		else if (str == TYPE_MEDIUM)
			return getEffectByLv (lv) [1];
		else if (str == TYPE_BIG)
			return getEffectByLv (lv) [2];
		else
			return getEffectByLv (lv) [2];
	}

	/** 根据等级获得相应颜色的特效组 */
	private EffectCtrl[] getEffectByLv (int lv)
	{
		EffectCtrl[] effs = new EffectCtrl[3];
		switch (lv) {
		case 3:
			effs [0] = effect_Small [0];
			effs [1] = effect_Medium [0];
			effs [2] = effect_Big [0];
			return effs;
		case 4:
			effs [0] = effect_Small [1];
			effs [1] = effect_Medium [1];
			effs [2] = effect_Big [1];
			return effs;
		case 5:
			effs [0] = effect_Small [2];
			effs [1] = effect_Medium [2];
			effs [2] = effect_Big [2];
			return effs;
		default:
			effs [0] = effect_Small [2];
			effs [1] = effect_Medium [2];
			effs [2] = effect_Big [2];
			return effs;
		}
	}
}
