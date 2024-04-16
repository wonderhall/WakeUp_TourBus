using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SerchTeleportArea : MonoBehaviour
{
    TeleportationArea ta;
    private GameObject[] colList;
    // Start is called before the first frame update
    private void Awake()
    {
        ta = this.GetComponent<TeleportationArea>();
        colList = GameObject.FindGameObjectsWithTag("teleportArea");
        foreach (var item in colList)
        {
            if(item.GetComponent<Collider>())
            ta.colliders.Add(item.GetComponent<Collider>());
        }
        
    }
}
