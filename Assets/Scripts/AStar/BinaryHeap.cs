using System.Collections;
using UnityEngine;
using System;

public class BinaryHeap<T> where T : IHeapItem<T>
{
    private T[] items;
    private int currentItemCount;

    public BinaryHeap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    /// <summary>
    /// Adds item to Heap
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    /// <summary>
    /// Removes first indexed item from array, then  then returns
    /// </summary>
    /// <returns firstItem></returns>
    public T RemoveFirstItem()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    /// <summary>
    /// Tests to if item is equal to the item in heap index
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    /// <summary>
    /// Returns the current item count
    /// </summary>
    public int Count
    {
        get { return currentItemCount; }
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    /// <summary>
    /// Compares item with parent, then swaps if parent is larger then item
    /// </summary>
    /// <param name="item"></param>
    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];

            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    /// <summary>
    /// Swaps indexes of items
    /// </summary>
    /// <param name="itemA"></param>
    /// <param name="itemB"></param>
    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
