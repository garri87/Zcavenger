using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemAttributeField : MonoBehaviour
{
  public Slider attributeBar;
  public TextMeshProUGUI attributeNameTMP;
  public TextMeshProUGUI attributeValueTMP;
  
  private void OnEnable()
  {
    attributeBar.value = Int32.Parse(attributeValueTMP.text);

    if (attributeBar.value <= 0)
    {
      gameObject.SetActive(false);
    }
  }
}
