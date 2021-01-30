using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ArmController : MonoBehaviour
{

	public SpriteShapeController ssc;
	public GameObject handSprite;

	private Spline spline;

    // Start is called before the first frame update
    void Start()
    {
		spline = ssc.spline;
    }

    // Update is called once per frame
    void Update()
    {

		int numPoints = spline.GetPointCount();
		//ShapeControlPoint shapePoints = ShapeTangentMode.Continuous;


		Vector3 mousePos = Camera.main.ScreenToWorldPoint(getMousePosToPipeSpace(Input.mousePosition));

        if (Input.GetKeyDown(KeyCode.Space)) {
			// Set the current head of the spline to where the mouse cursor is 
			spline.InsertPointAt(numPoints - 1, mousePos);
			spline.SetTangentMode(numPoints - 1, ShapeTangentMode.Continuous);
			numPoints += 1;
        }

		if (Input.GetMouseButton(0))
        {
			// When the user has clicked and is draggin the mouse, shift the index 0 points of the splien to their location
			Vector3 newPos = mousePos - new Vector3(0.1f, 0, 0);
			spline.SetPosition(numPoints - 1, newPos);
			
			// print("Position set to " + mousePos);
        }

		Vector3 handPosition = getMousePosToPipeSpace(spline.GetPosition(numPoints - 1), -2);
		handSprite.transform.position = handPosition;
		handSprite.transform.rotation = Quaternion.Euler(handSprite.transform.rotation.x , handSprite.transform.rotation.y, Vector3.SignedAngle(Vector3.right, currentDirection(spline), Vector3.up));

	}


	public static Vector3 currentDirection(Spline spline)
    {
		int numPoints = spline.GetPointCount();
		return spline.GetPosition(numPoints - 1) - spline.GetPosition(numPoints - 2);
    }

	public static Vector3 getMousePosToPipeSpace(Vector3 mousePos, float z=1.0f)
	{
		return new Vector3(mousePos.x, mousePos.y, z);
	}

	// Hopefully this code will allow the hand to snap the the grid that will eventually be mapped out by the pipe system
	// Stolen code form https://github.com/Draco18s/IdleArtificer/blob/master/Assets/draco18s/util/MathHelper.cs#L69-L87
	public static Vector3 snap(Vector3 pos, int v)
	{
		float x = pos.x;
		float y = pos.y;
		float z = pos.z;
		x = snap(x, v);
		y = snap(y, v);
		z = snap(z, v);
		return new Vector3(x, y, z);
	}

	public static int snap(int pos, int v)
	{
		float x = pos;
		return Mathf.FloorToInt(x / v) * v;
	}

	public static float snap(float pos, float v)
	{
		float x = pos;
		return Mathf.FloorToInt(x / v) * v;
	}
}
