using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMoveScript : MonoBehaviour
{
    Rigidbody2D rb2d;
    private float moveSpeed=3;

    public const string RIGHT = "right";
    public const string LEFT = "left";

    string buttonPressed;

    void Start()
    {
        rb2d=GetComponent<Rigidbody2D>();   //defining rb2d
    }

    
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            buttonPressed=RIGHT;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            buttonPressed=LEFT;
        }
        else
        {
            buttonPressed=null;
        }
    }


    private void FixedUpdate()
    {
        if(buttonPressed == RIGHT)
        {
            rb2d.velocity = new Vector2(moveSpeed,0);
            Debug.Log("moving right");
        }
        else if(buttonPressed == LEFT)
        {
            rb2d.velocity = new Vector2(-moveSpeed,0);
            Debug.Log("moving left");
        }
        else
        {
            rb2d.velocity = new Vector2(0,0);
        }
    }
}
