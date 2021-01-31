using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabbableScript : MonoBehaviour
{
    public bool grabbed;
    RaycastHit2D hit;
    public float distance = 2f;
    public Transform holdpoint;
    public float throwforce;
    public LayerMask notgrabbed;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            if(!grabbed)
            {//grab
                Physics2D.queriesStartInColliders=false;

                hit = Physics2D.Raycast(transform.position, Vector2.right*transform.localScale.x, distance);
                
                if(hit.collider!=null && hit.collider.tag=="grabbable")
                {
                    grabbed=true;
                    Debug.Log("object grabbed");
                }
            }
            else if(!Physics2D.OverlapPoint(holdpoint.position, notgrabbed)) //stops you from dropping objects in the terrain/garbbing the terrain
            {//throw
                grabbed=false;
                Debug.Log("object released");
                if(hit.collider.gameObject.GetComponent<Rigidbody2D>()!=null)
                {//angle of throw. faces same direction as play + up.
                    hit.collider.gameObject.GetComponent<Rigidbody2D>().velocity=new Vector2(transform.localScale.x,1)*throwforce;
                    
                }
            }

        }

        if(grabbed)
        {
            hit.collider.gameObject.transform.position=holdpoint.position; //whatever we grab is held in this poisiton
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position,transform.position+Vector3.right*transform.localScale.x*distance);
    }
}
