using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    const string LEFT = "left"; //stops typos and from us writing it over and over
    const string RIGHT = "right";

    [SerializeField]
    Transform castPos;
    
    [SerializeField]
    float baseCastDist; //the distance the enemy can see/sensitivity

    string facingDirection;

    Vector3 baseScale;//storing the base scale

    Rigidbody2D rb2d;
    float moveSpeed = 5;


    void Start()
    {
        baseScale = transform.localScale;
        rb2d = GetComponent<Rigidbody2D>();
        facingDirection = RIGHT; //whatever direction its facing by default
    }

    
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        float vX = moveSpeed;

        if(facingDirection == LEFT)
        {
            vX = -moveSpeed;
        }
        //move the game object
        rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);

        if(IsHittingWall() || IsNearEdge())
        {
            if(facingDirection == LEFT)
            {
                ChangeFacingDirection(RIGHT);
            }
            else if (facingDirection == RIGHT)
            {
                ChangeFacingDirection(LEFT);
            }
        }
    }

    void ChangeFacingDirection(string newDirection)
    {
        Vector3 newScale = baseScale;

        if(newDirection == LEFT)
        {
            newScale.x = -baseScale.x;
        }

        else if (newDirection == RIGHT)
        {
            newScale.x = baseScale.x;
        }

        transform.localScale = newScale;

        facingDirection = newDirection;
    }

    bool IsHittingWall()
    {
        bool val = false;

        float castDist = baseCastDist;

        //define the cast distance for left and right
        if(facingDirection == LEFT)
        {
            castDist = -baseCastDist;
        }
        else
        {
            castDist = baseCastDist;
        }

        //determin the target distination based on the cast distance
        Vector3 targetPos = castPos.position;
        targetPos.x += castDist;

        Debug.DrawLine(castPos.position, targetPos, Color.blue);

        //draw the raw. uses bitwise convention to find a layermask. shoots x to y and sees if it touches layer
        if(Physics2D.Linecast(castPos.position, targetPos, 1 << LayerMask.NameToLayer("Terrain")))
        {
            val = true; //we have hit a wall
        }
        else
        {
            val = false;
        }

        return val;
    }


    bool IsNearEdge()
    {
        bool val = true;

        float castDist = baseCastDist;

        //determine the target distination based on the cast distance
        Vector3 targetPos = castPos.position;
        targetPos.y += castDist; //shoots down


        Debug.DrawLine(castPos.position, targetPos, Color.red);


        //draw the raw. uses bitwise convention to find a layermask. shoots x to y and sees if it touches layer
        if(Physics2D.Linecast(castPos.position, targetPos, 1 << LayerMask.NameToLayer("Terrain")))
        {
            val = false; //opposite of wall
        }
        else
        {
            val = true;
        }

        return val;
    }

}
