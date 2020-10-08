using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class TWD_BaseAnimation : MonoBehaviour
{
    [SerializeField] GameObject tower;
    public Sprite sprite1; // Drag your first sprite here
    public Sprite sprite2; // Drag your second sprite here
    bool shoot = false;


    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        //GameObject TrueRangerTower = GameObject.Find("TrueRangerTower");
        //TWD_Tower towerScript = TrueRangerTower.GetComponent<TWD_Tower>();
        ////shoot = towerScript.shootCheck;
        //shoot = true;

        spriteRenderer = GetComponent<SpriteRenderer>(); // we are accessing the SpriteRenderer that is attached to the Gameobject
        if (spriteRenderer.sprite == null) // if the sprite on spriteRenderer is null then
            spriteRenderer.sprite = sprite1; // set the sprite to sprite1


    }

    // Update is called once per frame
    void Update()
    {
        TWD_Tower towerScript = tower.GetComponent<TWD_Tower>();
        shoot = towerScript.sCheck;
        if (shoot==true) // If the space bar is pushed down
        {
            ChangeTheDamnSprite(); // call method to change sprite
            shoot = false;
        }

    }
    void ChangeTheDamnSprite()
    {
        //GameObject.Find("TrueRangerTower").GetComponent<move>().speed
        if (spriteRenderer.sprite == sprite1) // if the spriteRenderer sprite = sprite1 then change to sprite2
        {
            spriteRenderer.sprite = sprite2;
        }
        else
        {
            spriteRenderer.sprite = sprite1; // otherwise change it back to sprite1
        }
    }
}


