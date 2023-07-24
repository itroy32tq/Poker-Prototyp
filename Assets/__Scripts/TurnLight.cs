using UnityEngine;

namespace ProspectorPrototyp
{
    public class TurnLight : MonoBehaviour
    {
        private void Update()
        {
            transform.position = Vector3.back * 3;

            if (PokerManager.CURRENT_PLAYER == null)
            {
                return;
            }
            transform.position += PokerManager.CURRENT_PLAYER.handSlotDef.pos;
        }
    }
}
