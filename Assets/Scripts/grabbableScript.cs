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

                Vector2 dir = new Vector2(1, 0);
                Vector3 angle = transform.rotation.eulerAngles;

                print("euler angles " + angle);

                if (Mathf.RoundToInt(angle.z) == 0) { dir = new Vector2(1, 0);  }
                else if (Mathf.RoundToInt(angle.z) == -90) { dir = new Vector2(0, -1); }
                else if (Mathf.RoundToInt(angle.z) == -180) { dir = new Vector2(-1, 0); }
                else if (Mathf.RoundToInt(angle.z) == -270) { dir = new Vector2(0, 1); }

                Vector2 back = -distance * 0.5f * dir;
                Vector3 backOffset = new Vector3(back.x, back.y, 0);

                hit = Physics2D.Raycast(transform.position, dir*transform.localScale.x, distance);
                
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
