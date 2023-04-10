using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldTextUI : MonoBehaviour
{
    public UIDocument uiDocument;
    
    public Transform target;
    public string text;
    
    public VisualElement _frame;
    private Label _label;
    private VisualElement root;
    
    private Camera _camera;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _camera = Camera.main;
        root = uiDocument.rootVisualElement;
        _frame = root.Q<VisualElement>("Frame");
        _label = root.Q<Label>("Text");
        _label.text = text;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            SetPosition();
        }
    }

    public void SetPosition()
    {
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
            _frame.panel, target.position, _camera);
        _frame.transform.position = new Vector2(target.transform.position.x, target.transform.position.y);
    }
}