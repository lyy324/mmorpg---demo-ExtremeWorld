using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 坐骑控制器。
/// 【重建文件】原文件在工程快照中缺失，按 EntityController 的实际调用
/// （SetRider(this) / OnEntityEvent(entityEvent, param)）重建最小实现。
/// 注意：坐骑预制体也已缺失，若有教程源码请对照补全骑乘动画等原始逻辑。
/// </summary>
public class RideController : MonoBehaviour {

    public Animator animator;

    private EntityController rider;

    // Use this for initialization
    void Start () {
        if (this.animator == null)
        {
            this.animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// 绑定骑乘者（EntityController.Ride() 中调用）。
    /// </summary>
    public void SetRider(EntityController rider)
    {
        this.rider = rider;
    }

    /// <summary>
    /// 透传实体事件（由 EntityController.OnEntityEvent 转发）。
    /// </summary>
    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        if (this.animator == null)
        {
            return;
        }
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                this.animator.SetBool("Move", false);
                this.animator.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
            case EntityEvent.MoveBack:
                this.animator.SetBool("Move", true);
                break;
        }
    }
}
