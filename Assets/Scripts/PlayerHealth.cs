using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public Text healthText;
    public float health = 100f;
    public int zombiesLayerNumber;
    public Image mainPanel;

    public bool rejuvenate = false;
    public float rejuvenationRate = 1f;
    string healthTextString = "";

    public static PlayerHealth thisPlayerHealth;
    // Start is called before the first frame update
    void UpdateUI() {
        if ((int)health + "" != healthTextString) {
            healthTextString = (int)health + "";
            healthText.text = healthTextString;
        }
    }
    
    void Start()
    {
        thisPlayerHealth = this;
        UpdateUI();
    }
    void TakeDamage(float damage) {
        health -= damage;
        healthText.text = (int)health + "";
        Color tp = mainPanel.color;
        tp.a = 0.5f;
        mainPanel.color = tp;

        if (health <= 0) {
            tp.a = 1f;
            mainPanel.color = tp;
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (mainPanel.color.a > 0 && health > 0) {
            Color tp = mainPanel.color;
            tp.a -= 0.5f * Time.deltaTime;
            mainPanel.color = tp;
        }
        if (rejuvenate && health < 100f) {
            health += rejuvenationRate * Time.deltaTime;
        }
        UpdateUI();
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("On trigger enter: " + collider.gameObject.layer +", "+ zombiesLayerNumber);

        if (collider.gameObject.layer == zombiesLayerNumber)
        {
            Debug.Log("On zombie trigger enter");
            ZombieMovement zm = collider.gameObject.GetComponentInParent<ZombieMovement>();
            if(zm != null && zm.isAlive)
                TakeDamage(1f);//TODO per damahge
        }
    }
    private void OnTriggerStay(Collider collider)
    {

    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //hit.collider
        //if (hit.collider.gameObject.layer == zombieColliderLayer)
        //{
        //    Debug.Log("On zombie collision enter");
        //    TakeDamage(5f);
        //}
    }
}
