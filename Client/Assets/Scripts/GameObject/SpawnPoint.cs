using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnPoint : MonoBehaviour
{
    Mesh mesh = null;
    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Vector3 pos = this.transform.position + Vector3.up * this.transform.localScale.y * 0.5f;
        Gizmos.color = Color.yellow;
        if (this.mesh != null)
            Gizmos.DrawWireMesh(this.mesh,pos,this.transform.rotation,this.transform.localScale);
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.ArrowHandleCap(0, pos, this.transform.rotation, 1f, EventType.Repaint);
        UnityEditor.Handles.Label(pos,"SpawnPoint:" + this.ID);
    }
#endif
}
