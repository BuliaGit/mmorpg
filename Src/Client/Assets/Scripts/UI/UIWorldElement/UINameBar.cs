using Entities;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour
{
    public Text avaverName;

    public Creature character;

    // Update is called once per frame
    void Update()
    {
        if (this.character != null)
        {
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            if (name != this.avaverName.text)
            {
                this.avaverName.text = name;
            }
        }
    }
}
