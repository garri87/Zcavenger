using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreenUI : MonoBehaviour
{
    public UIDocument gameOverScreen;
    public Label gameOverlabel;
    public string gameOverText = "YOU ARE DEAD";

    private void OnEnable()
    {
        gameOverScreen = GetComponent<UIDocument>();

        VisualElement root = gameOverScreen.rootVisualElement;

        gameOverlabel = root.Q<Label>();

        gameOverlabel.text = gameOverText;

        gameOverlabel.AddToClassList("LabelOff");
    }

    public void ShowGameOver()
    {
        gameOverlabel.EnableInClassList("LabelOff",false);
    }
}
