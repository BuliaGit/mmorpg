using UnityEngine;

public class UICharacterView : MonoBehaviour
{
    public Transform characterModel;
    public GameObject[] characters;

    private int currentCharacter = 0;

    public int CurrectCharacter
    {
        get
        {
            return currentCharacter;
        }
        set
        {
            currentCharacter = value;
            this.UpdateCharacter();
        }
    }


    void UpdateCharacter()
    {
        for (int i = 0; i < 3; i++)
        {
            characters[i].SetActive(i == this.currentCharacter);
        }
    }
}
