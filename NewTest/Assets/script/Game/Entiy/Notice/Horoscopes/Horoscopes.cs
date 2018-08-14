using UnityEngine;
using System.Collections;

public class Horoscopes
{
	public Horoscopes(int type, string describe, string name, int iconId , string _date , string _spriteName,string _Sdescribe,string _Pdescribe)
    {
        this.type = type;
        this.name = name;
        this.describe = describe;
		this.imageID = iconId;
		this.date = _date;
		this.spriteName = _spriteName;

		this.Sdescribe=_Sdescribe;
		this.Pdescribe=_Pdescribe;
    }
    
    private int type; //星座类型
    private string describe; //星座描述


	private string Cdescribe; //性格
	private string Sdescribe; //技能
	private string Pdescribe; //被动

    private string name; // 星座名字
	private string date; //日期
    private int imageID;//形象
	private string spriteName;//星座小图标

    public int getType()
    {
        return this.type;
    }

    public string getDescribe()
    {
        return this.describe;
    }

	public string getSkillDescribe()
	{
		return this.Sdescribe;
	}

	public string getPassiveDescribe()
	{
		return this.Pdescribe;
	}
	
	public string getName()
    {
        return this.name;
    }

    public int getImageID()
    {
        return this.imageID;
    }

	public string getDate()
	{
		return this.date;
	}

	public string getSpriteName()
	{
		return this.spriteName;
	}
}
