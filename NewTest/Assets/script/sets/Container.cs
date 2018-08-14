/**
 * Coypright 2013 by 刘耀鑫<xiney@youkia.com>.
 */

/**
 * @author 刘耀鑫
 */
public interface Container
{

	/* properties */
	/** 获得容器的大小 */
	int size();
	/** 判断容器是否为空 */
	bool isEmpty();
	/** 判断容器是否已满 */
	bool isFull();
	/* methods */
	/** 判断对象是否在容器中 */
	bool contain(object obj);
	/** 将对象放入到容器中 */
	bool add(object obj);
	/** 检索容器中的对象 */
	object get();
	/** 从容器中移除对象 */
	object remove();
	/** 清除容器 */
	void clear();
}

