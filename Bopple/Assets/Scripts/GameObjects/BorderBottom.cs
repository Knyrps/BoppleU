using System.Linq;
using UnityEngine;

public class BorderBottom : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D actor)
    {
        Bopple bopple = actor.GetComponent<Bopple>();
        if (bopple == null)
        {
            Debug.Log("Not Bopple");
            return;
        }

        Destroy(bopple.gameObject);
    }
}
