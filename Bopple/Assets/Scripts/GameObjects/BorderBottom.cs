using Bopple.Core.Utilities;
using UnityEngine;

namespace Scripts.GameObjects
{
    public class BorderBottom : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D actor)
        {
            Bopple bopple = actor.GetComponent<Bopple>();
            if (!bopple)
            {
                LogUtil.Log("Not Bopple");
                return;
            }

            Destroy(bopple.gameObject);
        }
    }
}
