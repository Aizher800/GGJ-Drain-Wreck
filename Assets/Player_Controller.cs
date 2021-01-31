using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Player_Controller : MonoBehaviour
{

	private class Move
    {

		private Direction direction;
		private Vector3 location;

		public Move(Direction direction, Vector3 location)
        {
			this.direction = direction;
			this.location = location;
        }

		public Direction GetDirection()
        {
			return direction;
        }

		public Vector3 GetLocation()
        {
			return location;
        }
    }

	public GameObject playerArm;
	public GameObject playerHand;

	private List<Move> moves;

	private Spline spline;
	private enum Direction { None, Up, Right, Down, Left };

	private Direction lastDir;

	// Start is called before the first frame update
	void Start()
	{

		//moves = new List<Move>();
		//moves.Add(new Move(Direction.None, ))


		SpriteShapeController ssc = playerArm.GetComponent<SpriteShapeController>();
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

			// Update the hand sprite to follow the tip of the newly moved hand
			Vector3 handPos = modifyZ(spline.GetPosition(spline.GetPointCount() - 1), -1.5f) + getDirectionalStep(lastDir) * 0.75f;
			playerHand.transform.position = handPos;

			float angle = 0;

			switch (lastDir)
			{
				case Direction.Down: angle = 270; break;
				case Direction.Left: angle = 180; break;
				case Direction.Up: angle = 90; break;
				case Direction.Right: angle = 0; break;
			}

			playerHand.transform.rotation = Quaternion.Euler(0, 0, angle);
		}
	}

	private void setHandPos(Spline spline, Direction newDir, Direction lastDir)
	{
		int last = spline.GetPointCount() - 1;

		Vector3 lastPos = spline.GetPosition(last);
		Vector3 nextPos = lastPos + getDirectionalStep(newDir);

		if (newDir == lastDir)
		{
			spline.SetPosition(last, nextPos);
		}
		else
		{
			spline.SetPosition(last, nextPos);
			spline.InsertPointAt(last, lastPos);
			spline.SetTangentMode(last, ShapeTangentMode.Continuous);

			Vector3[] tangents = getSmoothCorners(newDir, lastDir);
			spline.SetLeftTangent(last, tangents[0]);
			spline.SetRightTangent(last, tangents[1]);

			// Set the 'nub' end of the hand to be perpendicular to the way of travel (for hand)
			spline.SetTangentMode(last + 1, ShapeTangentMode.Continuous);
			spline.SetLeftTangent(last + 1, -getDirectionalStep(newDir));
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

	private Vector3[] getSmoothCorners(Direction newDir, Direction lastDir, float smoothing = 0.4f)
	{
		/** Return a set of vectors [Left, Right] for the tangent of a corner so that is it smooth */

		Vector3 left = new Vector3(smoothing, smoothing, 0), right = new Vector3(-smoothing, -smoothing, 0);

		bool clockwise = false;

		if ((lastDir == Direction.Down && newDir == Direction.Left) ||
				(lastDir == Direction.Left && newDir == Direction.Up) ||
				(lastDir == Direction.Up && newDir == Direction.Right) ||
				(lastDir == Direction.Right && newDir == Direction.Down))
		{

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


	private Vector3 modifyZ(Vector3 vec, float z)
	{
		return new Vector3(vec.x, vec.y, z);
	}
}