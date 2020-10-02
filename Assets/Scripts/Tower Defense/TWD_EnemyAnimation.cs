using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TWD_EnemyAnimation : MonoBehaviour
{

    [SerializeField] private TWD_Enemy enemyScript;
    [SerializeField] private Sprite NEanimation;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {   

    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = transform.localRotation * Quaternion.Inverse(transform.rotation);
        if (enemyScript.animationDir == 1)
        {   
            spriteRenderer.sprite = NEanimation;
        }

    }
    private void LateUpdate()
    {
        
    }
}
