using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI settingsPlayerName;
    [SerializeField] private TextMeshProUGUI lobbyPlayerName;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject lobbyUI;



    public void ApplyPlayerSettings() {
        lobbyPlayerName.text = settingsPlayerName.text;
        settingsUI.SetActive(false);
        lobbyUI.SetActive(true);

    }
}
