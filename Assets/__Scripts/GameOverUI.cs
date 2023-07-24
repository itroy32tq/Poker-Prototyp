using TMPro;
using UnityEngine;

namespace ProspectorPrototyp
{
    public class GameOverUI : MonoBehaviour
    {
        private TextMeshProUGUI _txt;

        private void Awake()
        {
            _txt = GetComponent<TextMeshProUGUI>();
            _txt.text = string.Empty;
        }
        private void Update()
        {
            if (PokerManager.Manager.GamePhase != GamePhase.gameover) return;
           
            if (PokerManager.VICTORY_PLAYERS == null) return;
            else
            {
                if (PokerManager.VICTORY_PLAYERS.Count == 1 & PokerManager.VICTORY_PLAYERS[0].type == PlayerType.human) _txt.text = "Вы выиграли!";
                else if (PokerManager.VICTORY_PLAYERS.Count == 1) _txt.text = "Победил игрок " + PokerManager.VICTORY_PLAYERS[0].playerNum.ToString();
                else _txt.text = "Ничья!!";
            }
        }

    }
}
