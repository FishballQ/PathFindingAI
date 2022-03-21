using System;
using System.Collections;
using System.Collections.Generic;

// define heap structure interface
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}

// design and constraint on the binary heap
public class Heap<T> where T : IHeapItem<T>
{
    T[] items;            // list
    int currentItemCount; // current counts
    // specify heap counts
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    // add a item and sort up
    public void Add(T item)
    {
        // add item as last item 
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        // sort up the item
        SortUp(item);
        currentItemCount++;
    }

    // remove first item and sort down
    public T RemoveFirst()
    {
        // first item has minimum value
        T firstItem = items[0];
        currentItemCount--;
        // change the last item to first 
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }
    // update heap order
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    // return current number of items 
    public int Count()
    {
        return currentItemCount;
    }

    // if the item exist
    public bool Contains(T item)
    {
        // if the new item equals find item
        return Equals(items[item.HeapIndex], item);
    }

    // sort down order
    void SortDown(T item)
    {
        while (true)
        {
            //left child branch
            int childIndexLeft = item.HeapIndex * 2 + 1;
            // right child branch
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;
            // swap if left branch value < parent item
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                // swap if right branch value < parent item
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) > 0)
                    {
                        swapIndex = childIndexRight; // smaller item
                    }
                }
                //a.compareto(b) a<b=-1 a=b=0 a>b=1
                if (item.CompareTo(items[swapIndex]) > 0)
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

    // sort up order
    void SortUp(T item)
    {
        // define parent item
        int parentIndex = (int)((item.HeapIndex - 1) *0.5f);

        while (true)
        {
            T parentItem = items[parentIndex];
            // swap if the current item value < parent item value
            if (item.CompareTo(parentItem)<0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            // continue comparing
            parentIndex= (int)((item.HeapIndex - 1) * 0.5f);
        }
    }

    // swap items and values order
    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}
