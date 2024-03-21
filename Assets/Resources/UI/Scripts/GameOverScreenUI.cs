using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreenUI : MonoBehaviour
{
    public UIDocument gameOverScreenUIDocument;
    public Label gameOverlabel;
    public string gameOverText = "YOU ARE DEAD";

    private void OnEnable()
    {
        gameOverScreenUIDocument = GetComponent<UIDocument>();

        VisualElement root = gameOverScreenUIDocument.rootVisualElement;

        gameOverlabel = root.Q<Label>();

        gameOverlabel.text = gameOverText;

        gameOverlabel.AddToClassList("LabelOff");
    }

    public void ShowGameOver()
    {
        gameOverlabel.EnableInClassList("LabelOff",false);
    }
}
