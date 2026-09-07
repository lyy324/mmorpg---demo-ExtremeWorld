using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Common.Data;
using System.Runtime.InteropServices;


public class UIEquipItem : MonoBehaviour, IPointerClickHandler
{
    public Image Icon;
    public Text title;
    public Text level;
    public Text limitClass;
    public Text limitCategory;

    public Image background;
    public Sprite normalBg;
    public Sprite selectorBg;

    private bool selected;
    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            this.selected = value;
            background.overrideSprite = selected ? selectorBg : normalBg;
        }
    }

    public int index { get; set; }
    private UICharEquip owner;
    private Item item;

    bool isEquiped = false;



    internal void SetEquipItem(int Id, Item item, UICharEquip owner, bool isEquiped)
    {
        this.owner = owner;
        this.index = Id;
        this.item = item;
        this.isEquiped = isEquiped;

        if (this.title != null) this.title.text = this.item.Define.Name;
        if (this.level != null) this.level.text = this.item.Define.Level.ToString();
        if(this.limitClass != null) this.limitClass.text = this.item.Define.LimitClass;
        if(this.limitCategory != null) this.limitCategory.text = this.item.EquipInfo.Category.ToString();
        if (this.Icon != null) this.Icon.overrideSprite = Resources.Load<Sprite>(this.item.Define.Icon);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(this.isEquiped)
        {
            UnEquip();
        }
        else
        {
            if (this.selected)
            {
                DoEquip();
                this.Selected = false;
            }
            else
                this.selected = true;
        }
    }

    private void DoEquip()
    {
        var msg = MessageBox.Show(string.Format("要装备 {0} 吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            if (oldEquip != null)
            {
                var newmsg = MessageBox.Show(string.Format("要替换掉 {0} 吗?", oldEquip.Define.Name), "确认", MessageBoxType.Confirm);
                newmsg.OnYes = () =>
                {
                    this.owner.DoEquip(this.item);
                };


            }
            else
                this.owner.DoEquip(this.item);
            
        };
    }

    private void UnEquip()
    {
        var msg = MessageBox.Show(string.Format("要取下装备 {0} 吗?", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = ( ) =>
        {
            this.owner.UpEquip(this.item);
        };
    }

    public void OnSelect(BaseEventData eventData)
    {
        throw new NotImplementedException();
    }
}
