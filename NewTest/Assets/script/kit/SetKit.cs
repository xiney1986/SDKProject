﻿/**
 * Coypright 2013 by 刘耀鑫<xiney@youkia.com>.
 */
using System;

/**
 * @author 刘耀鑫
 */
public class SetKit
{

	/* static fields */

	/** 快速排序的起步常数 */
	public const int QUICK_SORT_LIMIT = 20;
	/** 堆栈长度 */
	private const int STACK_LENGTH = 32;
	/** 快速排序的中值划分的阀值 */
	private const int THRESHOLD = 7;

	/* static methods */
	/**
	 * 对象数组排序方法， 默认为升序，数组长度小于指定长度用希尔排序，否则用快速排序，
	 * 
	 * @param array 为要排序的对象数组，
	 * @param comparator 为对象比较器，
	 */
	public static void sort (object[] array, Comparator comparator)
	{
		sort (array, 0, array.Length, comparator, false);
	}
	/**
	 * 对象数组排序方法， 默认为升序，数组长度小于指定长度用希尔排序，否则用快速排序，
	 * 
	 * @param array 为要排序的对象数组，
	 * @param offset 为排序的起始偏移位置，
	 * @param len 为排序的长度，
	 * @param comparator 为对象比较器，
	 */
	public static void sort (object[] array, int offset, int len,
		Comparator comparator)
	{
		sort (array, offset, len, comparator, false);
	}
	/**
	 * 对象数组排序方法， 数组长度小于指定长度用希尔排序，否则用快速排序，
	 * 
	 * @param array 为要排序的对象数组，
	 * @param comparator 为对象比较器，
	 * @param descending 表示是否为降序，
	 */
	public static void sort (object[] array, Comparator comparator,
		bool descending)
	{
		sort (array, 0, array.Length, comparator, descending);
	}
	/**
	 * 对象数组排序方法， 数组长度小于指定长度用希尔排序，否则用快速排序，
	 * 
	 * @param array 为要排序的对象数组，
	 * @param offset 为排序的起始偏移位置，
	 * @param len 为排序的长度，
	 * @param comparator 为对象比较器，
	 * @param descending 表示是否为降序，
	 */
	public static void sort (object[] array, int offset, int len,
		Comparator comparator, bool descending)
	{
		// 对小数组排序，希尔排序更好一些
		if (array.Length < QUICK_SORT_LIMIT)
			shellSort (array, offset, len, comparator, descending);
		else
			quickSort (array, offset, len, comparator, descending);
	}
	/* 希尔排序 */
	public static void shellSort (object[] array, int offset, int len,
		Comparator comparator, bool descending)
	{
		if (len <= 0)
			return;
		if (offset < 0)
			offset = 0;
		if (offset + len > array.Length)
			len = array.Length - offset;
		// 定义排序方向
		int comp = descending ? ComparatorKit.COMP_LESS : ComparatorKit.COMP_GRTR;
		// 定义及计算增量值
		int inc = 1;
		for (int n=len/9; inc<=n; inc=3*inc+1)
			;
		object o;
		int j;
		for (; inc>0; inc/=3) {
			for (int i=inc,n=offset+len; i<n; i+=inc) {
				o = array [i];
				j = i;
				while ((j>=inc)&&(comparator.compare(array[j-inc],o)==comp)) {
					array [j] = array [j - inc];
					j -= inc;
				}
				array [j] = o;
			}
		}
	}
	/* 快速排序 */
	public static void quickSort (object[] array, int offset, int len,
		Comparator comparator, bool descending)
	{
		if (len <= 0)
			return;
		if (offset < 0)
			offset = 0;
		if (offset + len > array.Length)
			len = array.Length - offset;
		// 定义排序方向
		int comp = descending ? ComparatorKit.COMP_GRTR : ComparatorKit.COMP_LESS;
		// 创建左右堆栈
		int size = STACK_LENGTH;
		int[] lefts = new int[size];
		int[] rights = new int[size];
		int top = 0;
		// 三元素比较，取中值，中值划分的最小数据长度
		const int threshold = THRESHOLD;
		// 左右堆栈的大小
		int lsize, rsize;
		// 左右初始值
		int l = offset, r = offset + len - 1;
		// 中间，左，右，枢轴指针
		int mid, scanl, scanr, pivot;
		object temp;
		// 主循环
		while (true) {
			while (r>l) {
				// 中值划分，取左右和中间位置的元素进行比较，
				// 取值为中间的元素作为枢轴
				if ((r - l) > threshold) {
					mid = (l + r) / 2;
					if (comparator.compare (array [mid], array [l]) == comp) {
						temp = array [mid];
						array [mid] = array [l];
						array [l] = temp;
					}
					if (comparator.compare (array [r], array [l]) == comp) {
						temp = array [r];
						array [r] = array [l];
						array [l] = temp;
					}
					if (comparator.compare (array [r], array [mid]) == comp) {
						temp = array [mid];
						array [mid] = array [r];
						array [r] = temp;
					}
					pivot = r - 1;
					temp = array [mid];
					array [mid] = array [pivot];
					array [pivot] = temp;
					scanl = l + 1;
					scanr = r - 2;
				} else {
					pivot = r;
					scanl = l;
					scanr = r - 1;
				}
				while (true) {
					// 扫描左边元素是否大于等于枢轴
					while ((scanl<r)
						&&(comparator.compare(array[scanl],array[pivot])==comp))
						scanl++;
					// 扫描右边元素是否小于等于枢轴
					while ((scanr>l)
						&&(comparator.compare(array[pivot],array[scanr])==comp))
						scanr--;
					// 如果左右扫描会合，则退出内层循环
					if (scanl >= scanr)
						break;
					// 交换元素
					temp = array [scanl];
					array [scanl] = array [scanr];
					array [scanr] = temp;
					if (scanl < r)
						scanl++;
					if (scanr > l)
						scanr--;
				}
				// 交换最后元素
				temp = array [scanl];
				array [scanl] = array [pivot];
				array [pivot] = temp;
				// 记录最大的分段到相应的堆栈中
				lsize = scanl - l;
				rsize = r - scanl;
				if (lsize > rsize) {
					if (lsize != 1) {
						top++;
						if (top == size)
							throw new Exception (typeof(SetKit).ToString ()
								+ " quickSort, stack overflow");
						lefts [top] = l;
						rights [top] = scanl - 1;
					}
					if (rsize == 0)
						break;
					l = scanl + 1;
				} else {
					if (rsize != 1) {
						top++;
						if (top == size)
							throw new Exception (typeof(SetKit).ToString ()
								+ " quickSort, stack overflow");
						lefts [top] = scanl + 1;
						rights [top] = r;
					}
					if (lsize == 0)
						break;
					r = scanl - 1;
				}
			}
			if (top == 0)
				break;
			l = lefts [top];
			r = rights [top--];
		}
	}

	/* constructors */
	private SetKit ()
	{
	}

}
