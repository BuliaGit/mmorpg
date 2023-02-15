using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Sprite activeImage;
    public Sprite normalImage;

    public TabView tabView;

    public int tabIndex = 0;

    private Image tabImage;

    // Use this for initialization
    void Start()
    {
        tabImage = this.GetComponent<Image>();

        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Select(bool select)
    {
        if (tabImage == null)
            return;

        tabImage.overrideSprite = select ? activeImage : normalImage;
    }

    void OnClick()
    {
        this.tabView.SelectTab(this.tabIndex);
    }
}
