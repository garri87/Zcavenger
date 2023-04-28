using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    private void Awake()
    {
        instance = this;
    }


    public SkinnedMeshRenderer headRenderer;
    public SkinnedMeshRenderer torsoRenderer;
    public SkinnedMeshRenderer vestRenderer;
    public SkinnedMeshRenderer legsRenderer;
    public SkinnedMeshRenderer feetRenderer;
    public SkinnedMeshRenderer backpackRenderer;

    private Item[] currentEquipment;

    private void Start()
    {
        // Inicializamos la lista de objetos equipados
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Item[numSlots];
    }

    // Este método se encarga de equipar un objeto en la parte correspondiente del jugador
    public void Equip(Item newItem)
    {
        // Obtenemos el índice de la parte del cuerpo que se equipará
        int slotIndex = (int)newItem.equipSlot;

        // Si ya hay un objeto equipado en esa parte, lo desequipamos y lo agregamos de vuelta al inventario
        Item oldItem = Unequip(slotIndex);
        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            Inventory.instance.Add(oldItem);
        }

        // Equipamos el nuevo objeto
        currentEquipment[slotIndex] = newItem;
        switch (newItem.equipSlot)
        {
            // Asignamos el modelo y el material al SkinnedMeshRenderer correspondiente
            // Además, reasignamos los huesos del modelo del objeto al SkinnedMeshRenderer
            case EquipmentSlot.Head:
                headRenderer.sharedMesh = newItem.mesh;
                headRenderer.material = newItem.material;
                headRenderer.bones = newItem.bones;
                break;
            case EquipmentSlot.Torso:
                torsoRenderer.sharedMesh = newItem.mesh;
                torsoRenderer.material = newItem.material;
                torsoRenderer.bones = newItem.bones;
                break;
            case EquipmentSlot.Vest:
                vestRenderer.sharedMesh = newItem.mesh;
                vestRenderer.material = newItem.material;
                vestRenderer.bones = newItem.bones;
                break;
            case EquipmentSlot.Legs:
                legsRenderer.sharedMesh = newItem.mesh;
                legsRenderer.material = newItem.material;
                legsRenderer.bones = newItem.bones;
                break;
            case EquipmentSlot.Feet:
                feetRenderer.sharedMesh = newItem.mesh;
                feetRenderer.material = newItem.material;
                feetRenderer.bones = newItem.bones;
                break;
            case EquipmentSlot.Backpack:
                backpackRenderer.sharedMesh = newItem.mesh;
                backpackRenderer.material = newItem.material;
                backpackRenderer.bones = newItem.bones;
                break;
        }
    }

      // Este método se encarga de desequipar un objeto de la parte correspondiente del jugador
    public Item Unequip(int slotIndex)
    {
        // Si no hay objeto equipado en esa parte, devolvemos null
        if (currentEquipment[slotIndex] == null)
        {
            return null;
        }

        // Desquipamos el objeto y lo eliminamos de la lista de objetos equipados
        Item oldItem = currentEquipment[slotIndex];
        currentEquipment[slotIndex] = null;

        // Asignamos el modelo y el material por defecto al SkinnedMeshRenderer correspondiente
        // Además, reasignamos los huesos del modelo del jugador al SkinnedMeshRenderer
        switch (oldItem.equipSlot)
        {
            case EquipmentSlot.Head:
                headRenderer.sharedMesh = null;
                headRenderer.material = null;
                headRenderer.bones = null;
                break;
            case EquipmentSlot.Torso:
                torsoRenderer.sharedMesh = null;
                torsoRenderer.material = null;
                torsoRenderer.bones = null;
                break;
            case EquipmentSlot.Vest:
                vestRenderer.sharedMesh = null;
                vestRenderer.material = null;
                vestRenderer.bones = null;
                break;
            case EquipmentSlot.Legs:
                legsRenderer.sharedMesh = null;
                legsRenderer.material = null;
                legsRenderer.bones = null;
                break;
            case EquipmentSlot.Feet:
                feetRenderer.sharedMesh = null;
                feetRenderer.material = null;
                feetRenderer.bones = null;
                break;
            case EquipmentSlot.Backpack:
                backpackRenderer.sharedMesh = null;
                backpackRenderer.material = null;
                backpackRenderer.bones = null;
                break;
        }

        return oldItem;
    }

    public enum EquipmentSlot { Head, Torso, Vest, Legs, Feet, Backpack }

}