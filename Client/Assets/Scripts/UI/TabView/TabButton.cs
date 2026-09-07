using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour 
{

    public Sprite activeImage;
    public Sprite normalImage;

    public TabView tabView;

    public int tabIndex = 0;
    public bool selected = false;

    public Image tabImage;

	// Use this for initialization
	void Start () {
        
        this.GetComponent<Button>().onClick.AddListener(OnClick);
	}

    public void Select(bool select)
    {
        tabImage.overrideSprite = select ? activeImage : normalImage;
    }

    void OnClick()
    {
        this.tabView.SelectTab(this.tabIndex);
    }

}
