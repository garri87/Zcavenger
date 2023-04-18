using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyStr
{
    public KeyCode keyCode;
    public string keyName;

    public KeyStr(KeyCode keyCode, string keyName)
    {
        this.keyCode = keyCode;
        this.keyName = keyName;
    }

}

public class KeyAssignments : MonoBehaviour
{
    private static KeyAssignments _instance;
    public static KeyAssignments Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<KeyAssignments>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<KeyAssignments>();
                }
            }
            return _instance;
        }
    }
    
    public KeyStr leftKey = new KeyStr          (KeyCode.A, "left");
    public KeyStr rightKey = new KeyStr         (KeyCode.D, "right");
    public KeyStr upKey = new KeyStr            (KeyCode.W, "up");
    public KeyStr downKey = new KeyStr          (KeyCode.S, "down");
    public KeyStr jumpKey = new KeyStr          (KeyCode.Space, "jump");
    public KeyStr walkKey = new KeyStr          (KeyCode.LeftShift, "walk");
    public KeyStr crouchKey = new KeyStr        (KeyCode.C, "crouch");
    public KeyStr proneKey = new KeyStr         (KeyCode.Z, "prone");
    public KeyStr attackKey = new KeyStr        (KeyCode.Mouse0, "attack");
    public KeyStr aimBlockKey = new KeyStr      (KeyCode.Mouse1, "aim/block");
    public KeyStr useKey = new KeyStr           (KeyCode.E, "use");
    public KeyStr reloadKey = new KeyStr        (KeyCode.R, "reload");
    public KeyStr flashLightKey = new KeyStr    (KeyCode.F, "flashlight");
    public KeyStr inventoryKey = new KeyStr     (KeyCode.Tab, "inventory");
    public KeyStr pauseKey = new KeyStr         (KeyCode.Escape, "pause");
    public KeyStr primaryKey = new KeyStr       (KeyCode.Alpha1, "draw primary");
    public KeyStr secondaryKey = new KeyStr     (KeyCode.Alpha2, "draw secondary");
    public KeyStr meleeKey = new KeyStr         (KeyCode.Alpha3, "draw melee");
    public KeyStr throwableKey = new KeyStr     (KeyCode.Alpha4, "draw throwable");


    public KeyStr[] keys;
    private int keyCodesLength;

    private int keybindChildCount;
    private void OnValidate()
    {
     keys = new KeyStr[]
        {
            jumpKey,
            walkKey,
            crouchKey,
            proneKey,
            attackKey,
            aimBlockKey,
            useKey,
            reloadKey,
            flashLightKey,
            inventoryKey,
            primaryKey,
            secondaryKey,
            meleeKey,
            throwableKey,
        };
        keyCodesLength = keys.Length;

    }

    private void Awake()
    {
        try
        {
            foreach (var key in keys)
            {
                key.keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(nameof(key)));
            } 

        }
        catch
        {
            foreach (var key in keys)
            {
                PlayerPrefs.SetString(nameof(key), key.keyCode.ToString());
            }
        }

    
    }



    /// <summary>
    /// Update the key assignment for a given key name
    /// </summary>
    /// <param name="keyCodeName"></param>
    public void UpdateKeyBinding(KeyStr key, KeyCode newKeyCode)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i] == key)
            {
             keys[i].keyCode = newKeyCode;
                Debug.Log("The " + keys[i].keyName + " key was successfully rebinded to" + keys[i].keyCode.ToString());
            }

        }
    }
}

