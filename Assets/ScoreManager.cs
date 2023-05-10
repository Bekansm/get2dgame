using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public int score;
    private TextMeshProUGUI scoreText;
    private void Awake() {
        scoreText = GetComponent<TextMeshProUGUI>();
    }


    public void UpdateScore() {
        UpdateScoreServerRpc();
    }

    [ServerRpc]
    public void UpdateScoreServerRpc() {
        score++;
        scoreText.text = "Score: " + score.ToString();
    }
}
