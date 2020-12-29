using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieColliders : MonoBehaviour
{
    public Animator animator;
    public Collider headCollider;
    public List<Collider> bodyColliders;
    public System.Action onDied;

    public float health;
    public bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        health = 100f;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkCollision(Collider collider, float damage) {//???
        if (headCollider == collider) {
            gotHeadShot();
            
        }
        else if (bodyColliders.Contains(collider)){
            gotBodyShot(damage);
        }
    }


    public void gotBodyShot(float damage) {
        Debug.Log("got body shot");

        health -= damage;
        if(health <= 0)
        {
            animator.SetTrigger("headShot");
            health = 0;
            Died();
        }
        else{
            animator.SetTrigger("bodyShot");
        }
        
    }
    public void gotHeadShot()
    {
        health = 0;
        animator.SetTrigger("headShot");
        Died();
    }
    public void Died() {
        if (isAlive) {
            onDied?.Invoke();
            isAlive = false;
        }
        
    }

}
