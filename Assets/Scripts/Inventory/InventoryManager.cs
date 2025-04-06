using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : VerticalLayoutGroup
{
    [SerializeField]
    private InventoryElement inventoryPrefab;
    [SerializeField]
    private RectTransform list;

    private List<InventoryElement> inventoryList = new List<InventoryElement>();

    public void AddItem(GameObject obj)
    {
        var i = Instantiate(inventoryPrefab, list);
        i.SetObject(obj);
        inventoryList.Add(i);

        LayoutRebuilder.MarkLayoutForRebuild(list);
    }
}
