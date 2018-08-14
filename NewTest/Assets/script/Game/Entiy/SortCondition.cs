using System;

/**
 * 筛选和排序条件类型
 * @author 汤琦
 * */
public class SortCondition
{
	public Condition[] siftConditionArr;//筛选条件集合
	public Condition	sortCondition;//排序条件
	public SortCondition ()
	{

	}
	
	public void clearSortCondition ()
	{
		siftConditionArr = null;
		sortCondition = null;
	}
	
	public void addSiftCondition(Condition condition){
		if (siftConditionArr == null || siftConditionArr.Length < 1)
			siftConditionArr = new Condition[]{condition};
		else {
			Condition[] temp = new Condition[siftConditionArr.Length + 1];
			temp[0] = condition;
			Array.Copy(siftConditionArr,0,temp,1,siftConditionArr.Length);
		}
	}
} 

