using Entities;
using SkillBridge.Message;


public interface IEntityNotify
{
    void OnEntityChanged(Entity entity);
    void OnEntityRemoved();
    void OnEntityEvent(EntityEvent entityEvent, int param);
}
