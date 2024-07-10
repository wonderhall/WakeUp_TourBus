using UnityEngine;


public class SerchTeleportArea : MonoBehaviour
{
    UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea ta;
    private GameObject[] colList;
    // Start is called before the first frame update
    private void Awake()
    {
        ta = this.GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>();
        colList = GameObject.FindGameObjectsWithTag("teleportArea");
        foreach (var item in colList)
        {
            if(item.GetComponent<Collider>())
            ta.colliders.Add(item.GetComponent<Collider>());
        }
        
    }
}
