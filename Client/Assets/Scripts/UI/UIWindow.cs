using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow window,WindowResult result);
    public event CloseHandler OnClose;
    public virtual System.Type Type { get { return this.GetType(); } }
    public GameObject Root;
    public enum WindowResult
    {
        None = 0,
        Yes,
        No,
    }

    public void Close(WindowResult result = WindowResult.None)
    {
        UIManager.Instance.Close(this.Type);
        if(this.OnClose != null)
            this.OnClose(this, result);
        this.OnClose = null;   
    }
    //正常关闭的写法
    public virtual void OnCloseClick()
    {
        this.Close();
    }
    //有 Yes 之后要处理的逻辑（事件） 的关闭写法
    public virtual void OnYesClick()
    {
        this.Close(WindowResult.Yes);
    }
    //有 No 之后要处理的逻辑（事件） 的关闭写法
    public virtual void OnNoClick()
    {
        this.Close(WindowResult.No);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
