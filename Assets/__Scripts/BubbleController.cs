using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProspectorPrototyp
{
    public class BubbleController : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI _bubbleText;
        [SerializeField]
        private Image _bubbleForm;
        [SerializeField]
        private Image _arrowL;
        [SerializeField]
        private Image _arrowR;
        [SerializeField]
        private RectTransform _rectTransform;

        private List<RectTransform> playerUIData = new();
        public List<RectTransform> PlayerUIData { get => playerUIData; set => playerUIData = value; }
        public bool isActiCoroutine = false;
        
        public void SubscribeBubble()
        {
            foreach (Player pl in PokerManager.Manager.players)
            {
                pl.OnPlayerActionEventFin += PlayerActionFinHandler;
                pl.OnPlayerActionEventStart += PlayerActionStartHandler;
            }
        }

        private string GetText(GamePhase gamePhase)
        {
            switch (gamePhase)
            { 
                case GamePhase.first_rate:
                    return "моя первоночальная ставка " + PokerManager.CURRENT_PLAYER.blind.ToString();
                case GamePhase.first_auction:
                case GamePhase.final_auction:
                    return PokerManager.CURRENT_PLAYER.action.ToString();
                case GamePhase.card_exchange:
                    return "сбрасываю " + PokerManager.CURRENT_PLAYER.discardCardsInHand.Count.ToString();
                case GamePhase.showdown:
                    return PokerManager.CURRENT_PLAYER.combination.ToString();
            }
            return string.Empty;
        }

        public void PlayerActionFinHandler(Player player) 
        {
            if (PokerManager.Manager.TurnPhase == TurnPhase.waiting) OnShowBubble(player);
            if (PokerManager.Manager.TurnPhase == TurnPhase.idle) OnCloseBubble();
        }

        public void PlayerActionStartHandler(Player player)
        {
            if (PokerManager.Manager.TurnPhase == TurnPhase.waiting) OnShowBubble(player, "думаю...");
        }

        public void OnShowBubble(Player player, string text = "")
        {

            if (text == string.Empty)
            {
                GamePhase gamePhase = PokerManager.Manager.GamePhase;
                _bubbleText.text = GetText(gamePhase);
            }
            else _bubbleText.text = text;

            if (player.playerNum == 3)
            {
                _arrowL.gameObject.SetActive(false);
                _arrowR.gameObject.SetActive(true);
            }
            else 
            {
                _arrowL.gameObject.SetActive(true);
                _arrowR.gameObject.SetActive(false);
            }
            
            if (_rectTransform.localScale == Vector3.zero)
            {
                _rectTransform.localPosition = playerUIData[player.playerNum-1].localPosition;
                StartCoroutine(OpenBubble(_rectTransform, 0.3f));
            }
        }

        public void OnCloseBubble()
        {
            StartCoroutine(CloseBubble(_rectTransform, 0.3f));
        }

        public IEnumerator OpenBubble(RectTransform rectTransform, float time)
        {

            isActiCoroutine = true;
            float currentTime = 0f;
            Vector3 startPosition = Vector3.zero;
            Vector3 endPosition = Vector3.one;

            while (currentTime < time)
            {
                rectTransform.localScale = Vector3.Lerp(startPosition, endPosition, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.localScale = endPosition;
            isActiCoroutine = false;
        }

        public IEnumerator CloseBubble(RectTransform rectTransform, float time)
        {

            isActiCoroutine = true;
            float currentTime = 0f;
            Vector3 startPosition = rectTransform.localScale;
            Vector3 endPosition = Vector3.zero;
            while (currentTime < time)
            {
                rectTransform.localScale = Vector3.Lerp(startPosition, endPosition, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;
                yield return null;
            }
            yield return null;
            rectTransform.localScale = endPosition;
            isActiCoroutine = false;
        }
    }

}
