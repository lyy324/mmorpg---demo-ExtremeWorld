using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//IDeselectHandler 取消选中之后的事件
public class UIPopChar : UIWindow,IDeselectHandler
{
    public int targetId;
    public string targetName;

    public void OnDeselect(BaseEventData eventData)
    {
        //1. 将基类事件数据转换为指针事件数据（鼠标/触摸相关）
        var ed = eventData as PointerEventData;
        // 2. 检查当前鼠标悬停的所有 GameObject 列表中是否包含当前物体
        if (ed.hovered.Contains(this.gameObject))
            return;
        this.Close(WindowResult.None);
    }

    public void OnEnable()
    {//启动时自动设置为已选中的状态
        this.GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3(80, 0, 0);
    }

    public void OnChat()
    {
        ChatManager.Instance.StartPrivateChat(targetId, targetName);
        this.Close(WindowResult.No);
    }

    public void OnAddFriend()
    {

        this.Close(WindowResult.No);
    }

    public void OnInviteTeam()
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
