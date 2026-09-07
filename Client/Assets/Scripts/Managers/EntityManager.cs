using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Entities;
using SkillBridge.Message;


class EntityManager : Singleton<EntityManager>, IDisposable
{
    Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
    Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

    public EntityManager()
    {
        
    }

    public void Dispose()
    {
        
    }

    //不用事件 不用委托 通过接口来存储函数
    public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
    {
        this.notifiers[entityId] = notify;
    }

    public void AddEntity(Entity entity)
    {
        this.entities[entity.entityId] = entity;
    }

    public void RemoveEntity(NEntity entity)
    {
        if (this.entities.ContainsKey(entity.Id))
        {
            this.entities.Remove(entity.Id);
            if (this.notifiers.ContainsKey(entity.Id))
            {
                this.notifiers[entity.Id].OnEntityRemoved();
                this.notifiers.Remove(entity.Id);
            }
        }
    }

    public void OnEntitySync(NEntitySync entitySync)
    {
        Entity entity = null;
        if (this.entities.TryGetValue(entitySync.Id, out entity))
        {
            entity.EntityData = entitySync.Entity;
            IEntityNotify notify = null;
            if (this.notifiers.TryGetValue(entitySync.Id, out notify))
            {
                notify.OnEntityEvent(entitySync.Event, entitySync.Param);
            }
        }

    }
}
