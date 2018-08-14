using System;

/// <summary>
/// 秘宝一些常数类
/// </summary>
public class MagicWeaponType{
    public const int HP = 1;//血量
	public const int ATTACK = 2;//攻击
	public const int DF = 3;//防御
	public const int MAGIC = 4;//魔力
    public const int AGILE= 5;//敏捷
	public const int DUKANG = 6;//毒抗性
    public const int PUTON = 7;//穿戴类型
    public const int STRENG = 8;//强化类型
    public const int FROM_CARD_BOOK_HAVE_M = 9;//从人物界面过去的(有秘宝)
    public const int FROM_CARD_BOOK_NOT_M = 10;//从人物界面过去的(没有秘宝)
    public const int ON_USED = 1;//使用中的
    public const int FORM_MINE = 11;//自己进入
    public const int FORM_OTHER = 12;//看别人的 就只能看了
    public const int STRENGG = 100;//进入强化界面
    public const int PHASE = 200;//进入锻造界面
    public MagicWeaponType()
	{
	}
}
