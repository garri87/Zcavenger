using UnityEngine;
using UnityEngine.UIElements;


public class LoadingScreenUI : MonoBehaviour
{
   public UIDocument loadingScreenUIDocument;
   public VisualElement root;

   public VisualElement progressBar;
   public Label hintsLabel;
   public HintsText hintsText; // Obtener los hints desde este script
   public string currentHint;

    private float timer;
    public float hintChangeTime = 2f;

   private void OnEnable()
   {
      loadingScreenUIDocument = GetComponent<UIDocument>();
      root = loadingScreenUIDocument.rootVisualElement;

        progressBar = root.Q<VisualElement>("Foreground");
      hintsLabel = root.Q<Label>("Hints");
        timer = hintChangeTime;
        currentHint = GetHint();

    }

    private void Update()
    {

        hintsLabel.text = currentHint;

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            currentHint = GetHint();
            timer = hintChangeTime;
        }

    }

    private string GetHint()
    {
        string hint = hintsText.hints[Random.Range(0,hintsText.hints.Count)];

        if  (hint == currentHint)
        {
            hint = GetHint();
        }
       
        return hint;

    }
}
