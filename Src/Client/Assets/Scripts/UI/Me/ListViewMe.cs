using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ListViewMe : MonoBehaviour
{
    //选中列表项事件
    public UnityAction<ListViewItemMe> OnItemSelect;
    //列表项集合
    private List<ListViewItemMe> viewItems = new List<ListViewItemMe>();

    //选中的列表项
    private ListViewItemMe selectItem = null;
    public ListViewItemMe SelectItem
    {
        get { return selectItem; }
        set
        {
            if (selectItem != null && selectItem != value)
            {
                selectItem.Selected = false;
            }
            selectItem = value;
            if (OnItemSelect != null)
            {
                OnItemSelect(value);
            }
        }
    }
    public class ListViewItemMe : MonoBehaviour, IPointerClickHandler
    {
        private bool selected;
        [HideInInspector]
        public ListViewMe owner;

        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                OnSelect(selected);
            }
        }
        public virtual void OnSelect(bool val) { }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Selected)
            {
                Selected = true;
            }
            if (owner != null)
            {
                owner.SelectItem = this;
            }
        }
    }
    public void AddItem(ListViewItemMe item)
    {
        item.owner = this;
        viewItems.Add(item);
    }

    public void RemoveAll()
    {
        if (viewItems != null)
        {
            foreach (var item in viewItems)
            {
                Destroy(item.gameObject);
            }
            viewItems.Clear();
        }
    }
}
