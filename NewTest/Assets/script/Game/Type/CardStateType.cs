using System;

/**
 * 卡片状态实体类
 * @author 汤琦
 * */
public class CardStateType
{
	public const int STATE_INIT = 0, //初始状态
		STATE_USING = 1, //使用中
		STATE_LOCK = 2, //保护中
		STATE_LOCKANDUSING = 3,//既使用又保护
        STATE_MINING = 4;//采矿中
	public const int NO_REMOVE = -1;//不移除
}
