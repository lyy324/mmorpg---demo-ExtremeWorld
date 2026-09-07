using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Managers;
using System.Runtime.Hosting;
using System.Deployment.Internal;
using Models;
using UnityEditor;
using SkillBridge.Message;

public class NpcController : MonoBehaviour
{
    SkinnedMeshRenderer render;
    public int npcID;
    Animator anim;
    Color orignColor;

    private bool inInteractive = false;

    NpcDefine npc;
    NpcQuestStatus questStatus;
    // Start is called before the first frame update
    void Start()
    {
        render =  this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        anim = this.gameObject.GetComponent<Animator>();
        orignColor = render.sharedMaterial.color;
        npc =  NpcManager.Instance.GetNpcDefine(npcID);
        this.StartCoroutine(Actions());
        RefreshNpcStatus();
        QuestManager.Instance.OnQuestStatusChanged += OnQuestStatusChanged;
    }
    void OnQuestStatusChanged(Quest quest)
    {
        this.RefreshNpcStatus();
    }
    void RefreshNpcStatus()
    {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(npcID);
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform,questStatus);
    }

    private void OnDestroy()
    {
        QuestManager.Instance.OnQuestStatusChanged -= OnQuestStatusChanged;
        if(UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
    }

    IEnumerator Actions()
    {
        while(true)
        {
            if(inInteractive)
                yield return new WaitForSeconds(2f);
            else 
                yield return new WaitForSeconds(Random.Range(5f,10f));

            this.Relax();
        }
    }
    void Relax()
    {
        anim.SetTrigger("Relax");
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void Interactive()
    {
        //这里是为了避免连续点击 协程过程中点击不响应
        if(!inInteractive)
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }
    IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();
        if(NpcManager.Instance.Interactive(npc))
        {
            anim.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(3f);
        inInteractive = false;
    }
    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while(Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward,faceTo)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward,faceTo,Time.deltaTime * 5f);
            yield return null;
        }
    }

    void OnMouseDown()
    {
        Interactive();
    }

    private void OnMouseOver()
    {
        Highlight(true);
    }
    private void OnMouseEnter()
    {
        Highlight(true);
    }
    private void OnMouseExit()
    {
        Highlight(false);
    }

    void Highlight(bool hightlight)
    {
        if(hightlight)
        {
            if(render.sharedMaterial.color != Color.white)
                render.sharedMaterial.color = Color.white;
        }
        else
        {
            if(render.sharedMaterial.color != orignColor)
                render.sharedMaterial.color = orignColor;
        }
    }



}
