using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Get item.");
            anim.SetBool("Get", true);
            if (gameObject.CompareTag("Gem"))
            {
                AudioManager.PlayPickGem();
            }else if (gameObject.CompareTag("Cherry"))
            {
                AudioManager.PlayPickCherry();
            }
            Invoke("GetItem",0.35f);
        }
    }

    void GetItem()
    {
        Destroy(gameObject);
    }
}
