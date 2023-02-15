using UnityEngine;
using UnityEngine.UI;

public class UILoading : MonoBehaviour
{
    public Slider progressBar;
    public Text progressNumber;
    //public Image bg;
    //public Animator animator;

    void Start()
    {
        progressBar.value = 0;
        progressNumber.text = "0%";
        SceneManager.Instance.onProgress = SetProgressValue;
    }

    private void SetProgressValue(float value)
    {
        if (progressBar == null || progressNumber == null) return;

        progressBar.value = value;
        progressNumber.text = string.Format("{0}%", progressBar.value);
    }
}
