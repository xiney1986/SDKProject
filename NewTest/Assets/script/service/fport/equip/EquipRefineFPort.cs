

/**
 * 装备精炼端口
 * */
public class EquipRefineFPort : BaseFPort
{

	private CallBack<string> callback;
    private string chooseEquipUid;//需要精炼装备的uid
    private int sid;//使用的精炼道具sid
    private int num;//使用的精炼道具的数量

	//uid1 -> uid2
    public void access(string equipUid, int propSid, int num, CallBack<string> callback)
	{ 
		this.callback = callback;
        this.chooseEquipUid = equipUid;
        this.sid = propSid;
        this.num = num;
        ErlKVMessage message = new ErlKVMessage(FrontPort.EQUIP_REFINE);
        message.addValue("equip_uid", new ErlString(equipUid));//老的
        message.addValue("sid", new ErlInt(sid));//新的
        message.addValue("num", new ErlInt(num));//需要交换的类型（装备,星魂,...）
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;

		if (callback != null) {
            callback(type.getValueString());
		}
	}

}
