using System;

/**
 * 卡牌训练
 * @author huangzhenghan
 * */
public class InitCardTraining : BaseFPort
{
    CallBack<int, int> callback;


    public void access(CallBack<int, int> callback)
    {
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.CARDTRAINING_INIT);
        access(message);
    }

    public override void read(ErlKVMessage message)
    {
        ErlArray arr = message.Value[1] as ErlArray;
        if (arr == null) return;

        for (int i = 0; i < arr.Value.Length; i++)
        {
            ErlArray item = arr.Value[i] as ErlArray;
            int index = StringKit.toInt(item.Value[0].getValueString());
            int time = StringKit.toInt(item.Value[1].getValueString());

            if (index == 1) index = 2;
            else if (index == 2) index = 1;
            else if (index == 3) index = 0;

            callback(index, time);
        }

    }










}

