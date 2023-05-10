using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : NetworkBehaviour
{
    [SerializeField] private Image healthbar;
    [SerializeField] private Image background;
    [SerializeField] private GameObject player;
    public float maxHealth;
    public float currentHealth;
    

    private Vector3 offset = new Vector3(0f, -0.5f, 0f);

    private void Awake() {
        currentHealth = maxHealth; 
    }

    //Ajdust healthbar rotation 
    private void Update() {
        healthbar.transform.position = player.transform.position - offset;
        background.transform.position = healthbar.transform.position;     
    }

   

    [ClientRpc]
    public void DamageTakenClientRpc(float damage) {
        currentHealth -= damage;
        healthbar.fillAmount = currentHealth / maxHealth;
    }
}
