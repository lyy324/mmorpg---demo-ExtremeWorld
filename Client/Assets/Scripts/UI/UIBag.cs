using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    public Text money;

    public Transform[] pages;
    public GameObject bagItem;
    List<Image> slots;

    // Start is called before the first frame update
    void Start()
    {
        SetTitle(User.Instance.CurrentCharacter.Gold.ToString());
        if(slots == null)
        {
            slots = new List<Image>();
            for(int page = 0; page < pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        StartCoroutine(InitBags());
    }

    IEnumerator InitBags()
    {
        for(int i = 0;i <BagManager.Instance.Items.Length;i ++)
        {
            var item = BagManager.Instance.Items[i];
            if(item.ItemId > 0)
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);
                var ui = go.GetComponent<UIIconItem>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon,item.Count.ToString());
            }
            yield return null;
        }
        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
            yield return null;
        }
    }

    public void SetTitle(string title)
    {
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
        this.Clear();
        StartCoroutine(InitBags());
    }

    void Clear()
    {
        for(int i = 0;i < slots.Count;i ++)
        {
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
