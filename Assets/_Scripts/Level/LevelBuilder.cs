using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Street 
{

     [NonReorderable] public GameObject prefab;
   public enum StreetType
    {
        Road,
        Pavement,
        Decoration,
    }

    [NonReorderable]  public StreetType streetType;

    public enum StreetShape
    {
        straight,
        cornerLeft,
        cornerRight,
    }
    [NonReorderable]  public StreetShape streetShape;

}

public class LevelBuilder : MonoBehaviour
{
    private BuildingGenerator buildGenerator;
    public int blockWidth;
    public int levelSize;

    #region Prefabs  

    public List<Street> prefabs;

    #endregion


}
