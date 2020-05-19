using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MemoryPool : IEnumerable, System.IDisposable
{
    class Item
    {
        public bool isActive;
        public GameObject objItem;
    }
    Item[] table;

    public void Dispose()
    {
        if (table == null)
            return;

        int count = table.Length;

        for (int i = 0; i < count; i++)
        {
            Item item = table[i];
            GameObject.Destroy(item.objItem);
        }
        table = null;
    }

    public IEnumerator GetEnumerator()
    {
        if (table == null)
            yield break;

        int count = table.Length;

        for (int i = 0; i < count; i++)
        {
            Item item = table[i];
            if (item.isActive)
                yield return item.objItem;
        }
    }

    public void Create(Object original, int count, GameObject parentgameObject)
    {
        Dispose();
        table = new Item[count];

        for (int i = 0; i < count; i++)
        {
            Item item = new Item();
            item.isActive = false;
            item.objItem = GameObject.Instantiate(original) as GameObject;
            item.objItem.transform.parent = parentgameObject.transform;
            item.objItem.SetActive(false);
            table[i] = item;
        }
    }

    public GameObject NewItem()
    {
        if (table == null)
            return null;

        int count = table.Length;

        for (int i = 0; i < count; i++)
        {
            Item item = table[i];
            if (item.isActive == false)
            {
                item.isActive = true;
                item.objItem.SetActive(true);
                return item.objItem;
            }
        }
        return null;
    }

    public void RemoveItem(GameObject gameObject)
    {
        if (table == null || gameObject == null)
        {
            return;
        }

        int count = table.Length;
        for (int i = 0; i < count; i++)
        {
            Item item = table[i];
            if (item.objItem == gameObject)
            {
                item.isActive = false;
                item.objItem.SetActive(false);
                break;
            }
        }
    }

    public void ClearItem()
    {
        if (table == null)
            return;

        int count = table.Length;
        for (int i = 0; i < count; i++)
        {
            Item item = table[i];
            if (item != null && item.isActive)
            {
                item.isActive = false;
                item.objItem.SetActive(false);
            }
        }
    }
}
