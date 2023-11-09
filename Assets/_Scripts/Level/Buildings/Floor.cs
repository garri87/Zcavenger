using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public int width;
    public int height;
    public int depth = 2;


    public enum FloorType
    {
        Base,
        Middle,
        Roof
    }
    public FloorType floorType;

    public enum StairsPosition
    {
        Left,
        Right,
        Middle,
    }
    public StairsPosition stairsPosition;

    public enum ExitDoorPosition
    {
        Front,
        LeftRight,
        FrontLeft,
        FrontRight,
        FrontLeftRight,
        None
    }
    public ExitDoorPosition exitDoorPosition;
    public MeshCollider meshCollider;
    public MeshFilter meshFilter; 


    private void OnValidate()
    {
       
    }

    private void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        if (meshCollider)
        {
            if (meshFilter)
            {
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }
     
    }
}
