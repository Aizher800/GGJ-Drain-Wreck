using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : MonoBehaviour
{
    public float blockCheckRange = 1.0f;
    public Transform[] destHolders;   //the transforms that hold the vectors for the transforms
    public Vector3[] _dest; //the destinations for the object to move to
    private Transform blockcheckPoint;
    public Transform blockcheckPointRight;
    public Transform blockcheckPointLeft;
    public bool isBlocked = false;

    private Vector3 _v; //is just used for checking whether the crab is going left or right
    private Vector3 _old;

    public Animator anim;

    private float journeyLength; // is used for the lerp in the moving coroutine
    // Start is called before the first frame update
    void Start()
    {
        _dest = new Vector3[destHolders.Length];
        for (int i = 0; i < destHolders.Length; i++)
        {
            _dest[i] = destHolders[i].position;
        }
        StartCoroutine(MovePoint());
        _old = transform.position;
        anim  = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _v = (_old - transform.position).normalized;
        if( _v.x < 0.0f)
        {
            blockcheckPoint = blockcheckPointLeft;
        }
        if( _v.x > 0.0f)
        {
            blockcheckPoint = blockcheckPointRight;
        }
        _old = transform.position;
    }

    IEnumerator MovePoint()
    {
        for(int i = 0; i < _dest.Length; i++)
        {
            yield return 0;
            while (transform.position !=  _dest[i]) //while the objects location is not the destination do all this trash
           {
                Vector3 target = _dest[i];

                Vector3 startPosition = transform.position;
                //Debug.Log(transform.position,startPosition);
                journeyLength =  Vector3.Distance(startPosition, target);
                float startTime = Time.time; // records the start of the Lerp
                float fractionOfJourney = 0f;
                while(fractionOfJourney < 1f)
                {
                    anim.SetBool("crabWalk", true);
                    while(isBlocked)
                    {
                        yield return new WaitForFixedUpdate();
                        startTime = startTime + Time.fixedDeltaTime;
                    }
                    float distCovered = (Time.time - startTime);
                    fractionOfJourney = distCovered / journeyLength;
                    transform.position = Vector3.Lerp(startPosition, target, fractionOfJourney);    // does a lerp to grab a new position shove the crab into 
                    //rb2d.MovePosition(Vector3.Lerp(startPosition, target, fractionOfJourney));
                    yield return new WaitForFixedUpdate(); // waits until the next fixed update
                }
                StartCoroutine(MovePoint());
           }
            // ^^stuff here that moves towards it probably a lerp

            

        }
    }
    IEnumerator BlockCheck() // checks if there is a block in the trigger
    {
        while(true)
        {
            Collider2D[] objects = Physics2D.OverlapCircleAll(blockcheckPoint.position, blockCheckRange);
        
            foreach(Collider2D block in objects)
            {
                if (block.CompareTag("grabbable"))
                {
                    isBlocked = true;
                    anim.SetTrigger("crabWait");
                    anim.SetBool("crabWalk",false);
                }
            }
        }
    } 
    

}
/*have different states  Just PLANNING TRASH

    // move state (maybe a coroutine):
    loops through an array moving towards one point each frame until it reaches point 
    then starts moving towards the other

    // hold state: triggered when an blocker moves around the crab. stops the move state coroutine
    */
