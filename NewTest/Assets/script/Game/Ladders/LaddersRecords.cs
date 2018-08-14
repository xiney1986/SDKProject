using System;
using System.Collections.Generic;

/// <summary>
/// 天梯战报记录存储
/// </summary>
public class LaddersRecords
{
	private List<LaddersRecordInfo> records;


	public LaddersRecords ()
	{
		records=new List<LaddersRecordInfo>();
	}
	public void M_addRecord(LaddersRecordInfo _record)
	{
		records.Add(_record);
	}
	public List<LaddersRecordInfo> M_getRecords()
	{
		return records;
	}
	public LaddersRecordInfo M_getLastRecord()
	{
		if(records.Count>0)
		{
			return records[0];
		}else
		{
			return null;
		}
	}
	public void M_clear()
	{
		records.Clear();
	}
}


