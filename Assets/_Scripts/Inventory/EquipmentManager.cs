using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class EquipmentManager : MonoBehaviour
{
    public SkinnedMeshRenderer playerModelRenderer;

    public void Equip(GameObject itemGO)
    {
        Item item = itemGO.GetComponent<Item>();
        SkinnedMeshRenderer itemRenderer = item.itemModelGO.GetComponent<SkinnedMeshRenderer>();
        itemRenderer.bones = playerModelRenderer.bones;
        itemRenderer.rootBone = playerModelRenderer.rootBone;
    }

    public void Unequip(GameObject itemGO)
    {
        Item item = itemGO.GetComponent<Item>();
        SkinnedMeshRenderer itemRenderer = item.itemModelGO.GetComponent<SkinnedMeshRenderer>();
        itemRenderer.bones = null;
        itemRenderer.rootBone = null;
    }

}
