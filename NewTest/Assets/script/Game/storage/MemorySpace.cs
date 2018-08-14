using System;
using System.Collections;

/**
 * 存储空间，准备不用了
 * @author longlingquan
 * */
public class MemorySpace
{
		public int num = 0;//当前已开启数量
		public ArrayList space ;

		public MemorySpace ()
		{
		}

		public void init (int num, ArrayList list)
		{
				this.num = num;
				if (list == null)
						space = new ArrayList ();
				else
						space = new ArrayList (list);
		}

		//设置当前已开启数量
		public void set_num (int num)
		{
				this.num = num;
		}

		//更新当前已开启数量
		public void update_num (int _num)
		{
				num += _num;
		}

		public virtual void parse (ErlArray arr)
		{

		}

		public void clear ()
		{
				if (space != null)
						space.Clear ();
				this.num = 0;
				space = null;
		}
}

