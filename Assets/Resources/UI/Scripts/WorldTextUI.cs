using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldTextUI : MonoBehaviour
{
    public UIDocument uiDocument;
    
    public Transform targetTransform; //Transform to place the text
    public string text = "Place_text";
    public Vector2 offsetPosition;


    public VisualElement _frame;
    private Label _label;
    private VisualElement root;
    
    private Camera _camera;

    public bool uIEnabled;

    private void Awake()
    {
      
    }

    private void OnEnable()
    {
        if (!targetTransform)
        {
            targetTransform = transform;
        }
        uiDocument = GetComponent<UIDocument>();
        _camera = Camera.main;
        root = uiDocument.rootVisualElement;
        _frame = root.Q<VisualElement>("Frame");
        _label = root.Q<Label>("Text");
        _frame.style.display = DisplayStyle.Flex;

    }

    private void LateUpdate()
    {
        if (uIEnabled && targetTransform != null)
        {
            SetWorldTextUI(targetTransform,text);
        }
        if (uIEnabled)
        {
            _frame.style.display = DisplayStyle.Flex;
        }
        else
        {
            _frame.style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// Places the element in the target position
    /// </summary>
    public void SetWorldTextUI(Transform newTarget, string newText)
    {
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
            _frame.panel, newTarget.position, _camera);
        newPosition.x = (newPosition.x  - _frame.layout.width / 2 * offsetPosition.x);
        newPosition.y = (newPosition.y  - _frame.layout.height / 2 * offsetPosition.y);
        _frame.transform.position = newPosition;
        _label.text = newText;

    }
}