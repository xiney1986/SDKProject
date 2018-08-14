using System;
/**
 * erlang 列表数据结构
 * 规定格式:[{K,V},...] 或者[integer,....]=string 这个列表的结尾肯定是106 ErlString没有106结尾符
 * @author longlingquan
 * */
using System.Collections.Generic;

public class ErlList:ErlType
{
	public const int TAG = 0x6c;
	private ErlType[] _value;
	private bool isString=true;
	
	public ErlList (ErlType[] array)
	{
		if (array == null) {
		} else {
			_value = array;
		}
	}
	
	public ErlType[] Value {
		get {
			return _value;
		}
		set {
			_value = value;
		}
	}

    public ErlList addList(ErlList li)
    {
        List<ErlType> teamList = new List<ErlType>();
        if (_value == null)
        {

            for (int i = 0; i < li.Value.Length; i++)
            {
                teamList.Add(li.Value[i]);
            }

        }
        else
        {
            for (int i = 0; i < _value.Length; i++)
            {
                teamList.Add(_value[i]);
            }
            for (int ii = 0; ii < li.Value.Length; ii++)
            {
                teamList.Add(li.Value[ii]);
            }
            // return 
        }
        return new ErlList(teamList.ToArray());
    }

    override public bool isTag (int tag)
	{ 
		return TAG == tag;
	}
	
	override public void bytesRead (ByteBuffer data)
	{
		base.bytesRead (data);
		if (isTag (_tag)) {
			int length = data.readInt ();
			_value = new ErlType[length];
			ErlType erl;
			for (int i =0; i<length; i++) { 
				// length==3  ErlList的循环
				erl = ByteKit.natureAnalyse (data);
				if(!(erl is ErlByte || erl is ErlInt))
					isString=false;
				_value [i] = erl;
			}
			data.readUnsignedByte ();// 读取列表结尾的空列表的tag标记 
		} 
	}
	
	override public void bytesWrite (ByteBuffer data)
	{
		base.bytesWrite (data);
		if (_value == null || _value.Length < 1) {
			new ErlNullList ().bytesWrite (data);
		} else {
			data.writeByte (TAG);
			data.writeInt (_value.Length);
			ErlBytesWriter erlBW;
			for (int i=0; i<_value.Length; i++) {
				erlBW = _value [i] as ErlBytesWriter;
				if (erlBW == null) {
					erlBW = new ErlNullList ();
				}
				erlBW.bytesWrite (data);
			}
			new ErlNullList ().bytesWrite (data); 
		}
	}
	
	override public void writeToJson (object key, object jsonObj)
	{
		
	}
	
	override public string getString (object key)
	{
		string str = '"' + key.ToString () + '"' + ':';
		ErlType erlType;
		if (_value.Length == 0) {
			return "[]";
		}
		string s = transNumber ();
		if (s != null) {
			str += '"' + s + '"';
			return str;
		} else {
			str += "[";
			for (int i=0; i<_value.Length; i++) {
				erlType = _value [i] as ErlType;
				if (erlType == null)
					continue; 
				if (erlType as ErlArray != null) {
					str += (erlType as ErlArray).getListArray ();// 针对直接嵌套在列表内的元组的特殊处理
				} else
					str += erlType.getValueString ();
				if (i < (_value.Length - 1))
					str += ",";
			}
			str += "]";
			return str;
		} 
	}
	
	/** 检查列表内的所有元素都是数字，并且在0-16#80000000范围之间，条件内的列表数据转换成字符串返回 */
	public string transNumber ()
	{
		if (!isString)
			return null;
		ByteBuffer byteArray = new ByteBuffer ();
		ErlByte erlByte = null;
		ErlInt erlInt = null;
		string str = null;
		string s = null;
		for (int j = 0; j<_value.Length; j++) {
			erlByte = _value [j] as ErlByte;
			//			if (j == 0 && erlByte != null && erlByte.Value == 0) {// 如果列表第一位是0，则不执行转换为字符串的操作
			//				return null;
			//			}
			if (erlByte != null && erlByte.Value > 0) {
				s = checkTrans (erlByte.Value);
				if (str == null) {
					str = "";
				}
				if (s != null) {
					str += s;
				} else {// 否则把byteArray中已写入的字节转换成字符串并连接到str中
					//						byteArray.clear();
					//						byteArray.writeByte(erlByte.value);// 不是转义数字则继续写到字节数组中
					//						byteArray.position=0;
					//						if(byteArray.bytesAvailable!=0)
					//						{
					//							str+=byteArray.readMultiByte(byteArray.bytesAvailable,"//unicode");
					//						}
					//						str+=String.fromCharCode(erlByte.Value); 暂时采用下面的写法
					str += new string (new char[]{(char)(erlByte.Value)}); 
				} 
				continue;
			} else {
				erlInt = _value [j] as ErlInt;
				if (erlInt != null && erlInt.Value > 0 && erlInt.Value < int.MaxValue) {
					s = checkTrans (erlInt.Value);
					if (str == null)
						str = "";
					if (s != null) {
						str += s;
					} else {
						//						byteArray.clear();
						//						byteArray.writeInt(erlInt.value);// 不是转义数字则继续写到字节数组中
						//						byteArray.position=0;
						//						if(byteArray.bytesAvailable!=0)
						//						{
						//							trace("字符串转换  byteArray.bytesAvailable="+byteArray.bytesAvailable);
						//							str+=byteArray.readMultiByte(byteArray.bytesAvailable,"unicode");
						//						}
						//str+=String.fromCharCode(erlByte.Value); 暂时采用下面的写法
//						str += new string (new char[]{(char)(erlInt.Value)});
						str +=  char.ConvertFromUtf32(erlInt.Value);
					}
					continue;
				}
			}
			str = null;
			return str;
		}
		return str; 
	}
	
	/** 检查指定数字是否是特殊转义字符，如果是则返回转义字符串，否则返回空 */
	public string checkTrans (int num)
	{
		string str = null;
		if (num == 34)
			str += '\"';
		else if (num == 92)
			str += '\\';
		else if (num == 8)
			str += '\b';
		else if (num == 9)
			str += '\t';
		else if (num == 10)
			str += '\n';
		else if (num == 12)
			str += '\f';
		else if (num == 13)
			str += '\r';
		return str; 
	}
	
	public override string getValueString ()
	{
		string s = transNumber (); 
		string str = ""; 
		if (s != null) {
			
			//str+='"'+s+'"'; 去掉引号 
			str += s;
			return str;
		} else {
			return getListString ();
		}
	}
	
	public string getListString ()
	{
		string str = "[";
		ErlType erlType;
		for (int i = 0; i < _value.Length; i++) {
			erlType = _value [i] as ErlType;
			if (erlType == null)
				continue; 
			if (erlType as ErlArray != null) {
				str += (erlType as ErlArray).getValueString ();// 针对直接嵌套在列表内的元组的特殊处理
			} else
				str += erlType.getValueString ();
			if (i < (_value.Length - 1)) {
				str += ",";
			}
		}
		str += "]";
		return str;
	}
} 

