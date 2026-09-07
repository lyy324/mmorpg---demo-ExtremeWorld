using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;

public class TeleporterObject : MonoBehaviour
{
    public int ID;
    Mesh mesh = null;
    // Start is called before the first frame update
    void Start()
    {
        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //碰到角色之后 触发传送逻辑
	void OnTriggerEnter(Collider collider)
	{
		PlayerInputController playerInput = collider.GetComponent<PlayerInputController>();


		if (playerInput !=null && playerInput.isActiveAndEnabled)
		{
			TeleporterDefine tdDefine = DataManager.Instance.Teleporters[this.ID];

			if (tdDefine == null)
			{
				Debug.LogErrorFormat("TeleportObject: Chacater :{0}  Enter Teleport{1} but TeleporterDefine is not existed", playerInput.character.Info.Name , this.ID);
				return;
			}
			Debug.LogFormat("TeleportObject: Chacater : [{0}]  Enter Teleport [{1}:{2}] ", playerInput.character.Info.Name, this.ID, tdDefine.Name);
			if (tdDefine.LinkTo > 0)
			{
				if (DataManager.Instance.Teleporters.ContainsKey(tdDefine.LinkTo))
				{
					MapService.Instance.SendMapTeleport(this.ID);
                    
				}
				else
				{
					Debug.LogErrorFormat("teleporter id {0}  link id {1}  error", this.ID, tdDefine.LinkTo);
				}
			}
		}
	}

#if UNITY_EDITOR
    //编辑器的拓展开发
    void OnDrawGizmos()
    {
		Gizmos.color = Color.green;
		if (this.mesh != null)
		{
			Gizmos.DrawWireMesh(this.mesh , this.transform.position + Vector3.up * this.transform.localScale.y * 0.5f, this.transform.rotation,this.transform.localScale);
		}
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.ArrowHandleCap(0, this.transform.position,this.transform.rotation, 1f, EventType.Repaint );
	}
#endif
}
