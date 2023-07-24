using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProspectorPrototyp
{

    public class UIComponent : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _plOneCyrBlind;
        [SerializeField]
        private TextMeshProUGUI _plOneCyrCash;
        [SerializeField]
        private TextMeshProUGUI _plTwoCyrBlind;
        [SerializeField]
        private TextMeshProUGUI _plTwoCyrCash;
        [SerializeField]
        private TextMeshProUGUI _plThreeCyrBlind;
        [SerializeField]
        private TextMeshProUGUI _plThreeCyrCash;
        [SerializeField]
        private TextMeshProUGUI _plCyrBlind;
        [SerializeField]
        private TextMeshProUGUI _plCyrCash;
        [SerializeField]
        private Button _passTurnButton;
        [SerializeField]
        private Button _cardsExConfirmButton;

        [SerializeField]
        private TMP_Dropdown _playerActionDropdown;
        [SerializeField]
        private Slider _playerCurRateSlider;
        [SerializeField]
        private TextMeshProUGUI _sliderValueText;

        private PlayerAction _humanPlayerAction;
        private int _playerCurRate = 0;

        private void Awake()
        {
            _passTurnButton.gameObject.SetActive(false);
            _cardsExConfirmButton.gameObject.SetActive(false);
            _playerActionDropdown.gameObject.SetActive(false);
            _playerCurRateSlider.gameObject.SetActive(false);  

        }

        private void Update()
        {
            if (PokerManager.CURRENT_PLAYER != null)
            {
               
                if (PokerManager.CURRENT_PLAYER.type == PlayerType.human & (PokerManager.Manager.GamePhase == GamePhase.first_rate 
                                                        || PokerManager.Manager.GamePhase == GamePhase.first_auction || 
                                                        PokerManager.Manager.GamePhase == GamePhase.final_auction)) _passTurnButton.gameObject.SetActive(true);
                else _passTurnButton.gameObject.SetActive(false);

                if (PokerManager.CURRENT_PLAYER.type == PlayerType.human & PokerManager.Manager.GamePhase == GamePhase.card_exchange) _cardsExConfirmButton.gameObject.SetActive(true);
                else _cardsExConfirmButton.gameObject.SetActive(false);

                if (PokerManager.CURRENT_PLAYER.type == PlayerType.human & (PokerManager.Manager.GamePhase == GamePhase.final_auction || PokerManager.Manager.GamePhase == GamePhase.first_auction))
                {
                    _playerActionDropdown.gameObject.SetActive(true);

                    if (_playerCurRateSlider.isActiveAndEnabled) _sliderValueText.text = _playerCurRate.ToString();
                }
                else
                {
                    _playerActionDropdown.gameObject.SetActive(false);
                    _playerCurRateSlider.gameObject.SetActive(false);
                } 

            }

            _plCyrBlind.text = "текущая ставка " + PokerManager.Manager.players[0].curRate.ToString();
            _plOneCyrBlind.text = "текущая ставка " + PokerManager.Manager.players[1].curRate.ToString();
            _plTwoCyrBlind.text = "текущая ставка " + PokerManager.Manager.players[2].curRate.ToString();
            _plThreeCyrBlind.text = "текущая ставка " + PokerManager.Manager.players[3].curRate.ToString();

            _plCyrCash.text = "остаток наличных " + PokerManager.Manager.players[0].cache.ToString();
            _plOneCyrCash.text = "остаток наличных " + PokerManager.Manager.players[1].cache.ToString();
            _plTwoCyrCash.text = "остаток наличных " + PokerManager.Manager.players[2].cache.ToString();
            _plThreeCyrCash.text = "остаток наличных " + PokerManager.Manager.players[3].cache.ToString();
        }

        public void OnChengeButtonClick()
        { 
            PokerManager.Manager.PassTurnClick = true;
            Debug.Log("сделал ставку");

            if (PokerManager.Manager.GamePhase == GamePhase.final_auction || PokerManager.Manager.GamePhase == GamePhase.first_auction)
            {
                PokerManager.CURRENT_PLAYER.action = _humanPlayerAction;
                PokerManager.CURRENT_PLAYER.insRate = _playerCurRate;
            } 

            _passTurnButton.gameObject.SetActive(false);

        }

        public void OnCardsExConfirmClick()
        {
            PokerManager.Manager.CardsExConfirmClick = true;
            Debug.Log("подтвердил обмен");
            _cardsExConfirmButton.gameObject.SetActive(false);

        }

        public void OnCardActionDropdown(TMP_Dropdown tMP_Dropdown) 
        {
            switch (tMP_Dropdown.value)
            {
                case 0:
                    _humanPlayerAction = PlayerAction.pass;
                    break;
                case 1:
                    _humanPlayerAction = PlayerAction.coll;
                    break;
                case 2:
                    _humanPlayerAction = PlayerAction.raise;
                    _playerCurRateSlider.gameObject.SetActive(true);
                    _playerCurRateSlider.maxValue = PokerManager.CURRENT_PLAYER.cache;
                    break;
                case 3:
                    _humanPlayerAction = PlayerAction.allin;
                    break;
            }
        }
        public void OnPlayerCurRateSliderValueChanged()
        {
            _playerCurRate =(int)_playerCurRateSlider.value;
        }
    }
}
