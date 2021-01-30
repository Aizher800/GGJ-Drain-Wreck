using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ArmController : MonoBehaviour
{

	public SpriteShapeController ssc;
	public GameObject handSprite;

	private Spline spline;
	private enum Direction { None, Up, Right, Down, Left };

	private Direction lastDir; 

    // Start is called before the first frame update
    void Start()
    {
		spline = ssc.spline;
		lastDir = Direction.None;
    }

	// Update is called once per frame
	void Update()
	{

		int numPoints = spline.GetPointCount();
		int lastPoint = numPoints - 1;
		//ShapeControlPoint shapePoints = ShapeTangentMode.Continuous;

		Vector3 currentPos = spline.GetPosition(lastPoint);
		Direction newDirection = Direction.None;

		//Vector3 mousePos = Camera.main.ScreenToWorldPoint(getMousePosToPipeSpace(Input.mousePosition));

		// Movement controller
		if (Input.GetKeyDown(KeyCode.DownArrow))  // down
		{
			newDirection = Direction.Down;
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) // left
		{
			newDirection = Direction.Left;
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow)) // rigght
		{
			newDirection = Direction.Right;
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow)) // up
		{
			newDirection = Direction.Up;
		}

		if (newDirection != Direction.None)
		{
			setHandPos(spline, newDirection, lastDir);
			lastDir = newDirection;
		}
	}


	/*		if (Input.GetKeyDown(KeyCode.Space)) {
			// Set the current head of the spline to where the mouse cursor is 
			makeNewkeyPoint(spline, mousePos);
			numPoints += 1;
        }

		if (Input.GetMouseButton(0))
        {
			// When the user has clicked and is draggin the mouse, shift the index 0 points of the splien to their location
			Vector3 newPos = mousePos - new Vector3(0.1f, 0, 0);
			
			//spline.SetTangentMode(numPoints - 1, ShapeTangentMode.Continuous);

			Vector3 lastPos = spline.GetPosition(numPoints - 2);

			//spline.SetPosition(numPoints - 1, Vector3.Lerp(lastPos, newPos, 0.001f));

			if (Vector3.Distance(lastPos, newPos) > 1.0f)
            {
				makeNewkeyPoint(spline, newPos);
				numPoints += 1;
            }

			spline.SetPosition(numPoints - 1, newPos);

			print("Left " + spline.GetLeftTangent(numPoints - 2) + " " + spline.GetRightTangent(numPoints - 2));
			// print("Position set to " + mousePos);
		}

		Vector3 handPosition = getMousePosToPipeSpace(spline.GetPosition(numPoints - 1), -2);
		handSprite.transform.position = handPosition;
		handSprite.transform.rotation = Quaternion.Euler(handSprite.transform.rotation.x , handSprite.transform.rotation.y, Vector3.SignedAngle(Vector3.right, currentDirection(spline), Vector3.up));

	}*/


	private void setHandPos(Spline spline, Direction newDir, Direction lastDir)
    {
		int last = spline.GetPointCount() - 1;

		Vector3 lastPos = spline.GetPosition(last);
		Vector3 nextPos = lastPos + getDirectionalStep(newDir);

		if (newDir == lastDir)
        {
			spline.SetPosition(last, nextPos);
        } else
        {
			spline.SetPosition(last, nextPos);
			spline.InsertPointAt(last, lastPos);
			spline.SetTangentMode(last, ShapeTangentMode.Continuous);

			Vector3[] tangents = getSmoothCorners(newDir, lastDir);
			spline.SetLeftTangent(last, tangents[0]);
			spline.SetRightTangent(last, tangents[1]);

			//print("Left " + tangents[0] + " Right " + tangents[1]);
			print("Left " + spline.GetLeftTangent(2) + " " + spline.GetRightTangent(2));

			//spline.setR
		}

	}

	private Vector3 getDirectionalStep(Direction dir)
    {
		switch (dir) 
		{
			case Direction.Up:
				return Vector3.up;
			case Direction.Down:
				return Vector3.down;
			case Direction.Left:
				return Vector3.left;
			case Direction.Right:
				return Vector3.right;
			default:
				return Vector3.zero;
        }
    }

	private Vector3[] getSmoothCorners(Direction newDir, Direction lastDir, float smoothing=0.4f)
    {
		/** Return a set of vectors [Left, Right] for the tangent of a corner so that is it smooth */

		Vector3 left = new Vector3(smoothing,smoothing,0), right = new Vector3(-smoothing,-smoothing,0);

		bool clockwise = false;

		if ((lastDir == Direction.Down && newDir == Direction.Left) || 
				(lastDir == Direction.Left && newDir == Direction.Up) ||
				(lastDir == Direction.Up && newDir == Direction.Right) ||
				(lastDir == Direction.Right && newDir == Direction.Down)) {

			clockwise = true;

        }

		if (clockwise)
        {
			float angle = 0;

			switch (lastDir)
			{
				case Direction.Down: angle = 0; break;
				case Direction.Left: angle = -90; break;
				case Direction.Up: angle = -180; break;
				case Direction.Right: angle = -270; break;
			}

			left = Quaternion.Euler(0, 0, angle) * left;
			right = Quaternion.Euler(0, 0, angle) * right;
		} 
		else
        {

			left = new Vector3(-smoothing, smoothing, 0);
			right = new Vector3(smoothing, -smoothing, 0);

			float angle = 0;

			switch (lastDir)
			{
				case Direction.Down: angle = 0; break;
				case Direction.Right: angle = 90; break;
				case Direction.Up: angle = 180; break;
				case Direction.Left: angle = 270; break;
			}
			left = Quaternion.Euler(0, 0, angle) * left;
			right = Quaternion.Euler(0, 0, angle) * right;
		}

		return new Vector3[2] { left, right };
	} 



	private void makeNewkeyPoint(Spline spline, Vector3 keypoint)
    {
		int numPoints = spline.GetPointCount();
		spline.InsertPointAt(numPoints - 1, keypoint);
		spline.SetTangentMode(numPoints - 1, ShapeTangentMode.Continuous);
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
