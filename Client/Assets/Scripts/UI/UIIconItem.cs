using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    public Image mainImage;
    public Image SecondImage;
    public Text mianText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //设置图标名 和 文本
    public void SetMainIcon(string iconName,string text)
    {
        this.mainImage.overrideSprite = Resources.Load<Sprite>(iconName);
        this.mianText.text = text;
    }
}
