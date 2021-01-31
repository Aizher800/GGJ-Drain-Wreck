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

	// Start is called before the first frame update
	void Start()
	{

		moves = new List<Move>();
		print("Move added: " + new Vector3(0, 0, 0));
		moves.Add(new Move(Direction.Down, new Vector3(0, 0, 0)));

		// get the controller and then the spline object for the arm
		SpriteShapeController ssc = playerArm.GetComponent<SpriteShapeController>();
		spline = ssc.spline;
	}

	// Update is called once per frame
	void Update()
	{
		int numPoints = spline.GetPointCount();
		int lastPoint = numPoints - 1;

		Vector3 currentPos = spline.GetPosition(lastPoint);
		Direction newDirection = Direction.None;

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
			SetHandPos(spline, newDirection);
			Direction lastDir = GetLastDirection();

			// Update the hand sprite to follow the tip of the newly moved hand
			Vector3 handPos = ModifyZ(spline.GetPosition(spline.GetPointCount() - 1), -1.5f) + GetDirectionalStep(lastDir) * 0.75f;
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

	private Direction GetLastDirection()
    {
		if (moves.Count > 0)
        {
			return moves[moves.Count - 1].GetDirection();
        }
		return Direction.None;
    }

	private Direction GetOppositeDirection(Direction dir)
    {
		/** Returns the direction that is in the other direction to this one. */
		switch (dir)
		{
			case Direction.Down: return Direction.Up;
			case Direction.Left: return Direction.Right;
			case Direction.Up: return Direction.Down;
			case Direction.Right: return Direction.Left;
			default: return Direction.None;
		}
	}

	private void SetHandPos(Spline spline, Direction newDir)
	{
		/** Makes a moved based on the input move and the current move stack. Backtracking will remove a move.*/
		int headPoint = spline.GetPointCount() - 1;
		Direction lastDir = GetLastDirection();

		Vector3 curPos = moves[moves.Count - 1].GetLocation();
		Vector3 nextPos = curPos + GetDirectionalStep(newDir);

		if (newDir == GetOppositeDirection(lastDir))
        {
			// use this move to nuke out another
			moves.RemoveAt(moves.Count - 1);
			//moves.RemoveAt(moves.Count - 1);
        } 
		else
        {
			// Place a new move
			moves.Add(new Move(newDir, nextPos));
		}

		// Clear all spline apart from first n
		int n = 1;
		for (int i = spline.GetPointCount() - 1; i > n; i--) {
			spline.RemovePointAt(i);
		}
		//spline.Clear();

		// Set the floating head point 
		//spline.InsertPointAt(spline.GetPointCount(), new Vector3(4, 4, 0));;
			//moves[moves.Count - 1].GetLocation() + GetDirectionalStep(moves[moves.Count - 1].GetDirection()));

		//spline.InsertPointAt(spline.GetPointCount(), new Vector3(1, 1, 0));
		//spline.InsertPointAt(spline.GetPointCount(), new Vector3(6, 6, 0));

		print("There are " + moves.Count + " moves in queue");

		// Build the corners as you go 
		for (int i = 1; i < moves.Count; i++) 
		{
			Direction lastD = moves[i - 1].GetDirection();
			Direction nextD = moves[i].GetDirection();

			if (lastD != nextD)
            {
				int insertId = spline.GetPointCount();
				spline.InsertPointAt(insertId, moves[i-1].GetLocation()); // insert a point behind where we just were
				spline.SetTangentMode(insertId, ShapeTangentMode.Continuous); // set it curvey

				Vector3[] tangents = GetSmoothCorners(nextD, lastD);
				spline.SetLeftTangent(insertId, tangents[0]);
				spline.SetRightTangent(insertId, tangents[1]);

			}
        }

		int nubId = spline.GetPointCount();
		Direction nubDir = moves[moves.Count - 1].GetDirection();
		spline.InsertPointAt(nubId,
			moves[moves.Count - 1].GetLocation() + GetDirectionalStep(nubDir));

		// Set the 'nub' end of the hand to be perpendicular to the way of travel (for hand)
		spline.SetTangentMode(nubId, ShapeTangentMode.Continuous);
		spline.SetLeftTangent(nubId, -GetDirectionalStep(nubDir));

		/*
			if (newDir == lastDir)
		{
			// moving in the SAME DIRECTION 
			spline.SetPosition(headPoint, nextPos);

			print("Move added: " + nextPos);
			
		}
		else if (newDir == GetOppositeDirection(lastDir))
        {
			// We are BACKTRACKING:

			int lastMoveId = moves.Count - 1;
			int lastLastMoveId = moves.Count - 2;

			Direction lastMove = moves[lastMoveId].GetDirection();
			Direction lastLastMove = moves[lastLastMoveId].GetDirection();

			if (lastLastMove != lastMove)
            {
				// Remove a keypoint since it was made. KP id smame as 2nd last point 
				spline.RemovePointAt(spline.GetPointCount() - 2);
            }

			print("Move removed: " + moves[lastMoveId].GetLocation());
			moves.RemoveAt(lastMoveId);
			print("Move removed: " + moves[lastLastMoveId].GetLocation());
			moves.RemoveAt(lastLastMoveId);

			SetHandPos(spline, lastLastMove);

			

			// SetHandPos(spline, moves[lastMoveId].GetDirection());

			// do nothing for now 

			// remove  the last from the stack, and see if the one before that has the same direction or not

			// ensure that we always cant go too far

		}
		else
		{
			// If the direction has changed from the last, add a new keypoint 
			spline.SetPosition(headPoint, nextPos); // move the head
			spline.InsertPointAt(headPoint, lastPos); // insert a point behind where we just were
			spline.SetTangentMode(headPoint, ShapeTangentMode.Continuous); // set it curvey

			print("Move added: " + nextPos);
			moves.Add(new Move(newDir, nextPos));

			Vector3[] tangents = GetSmoothCorners(newDir, lastDir);
			spline.SetLeftTangent(headPoint, tangents[0]);
			spline.SetRightTangent(headPoint, tangents[1]);

			// Set the 'nub' end of the hand to be perpendicular to the way of travel (for hand)
			spline.SetTangentMode(headPoint + 1, ShapeTangentMode.Continuous);
			spline.SetLeftTangent(headPoint + 1, -GetDirectionalStep(newDir));
		}*/

	}

	private Vector3 GetDirectionalStep(Direction dir)
	{
		/** Get a unit vector in the direction of the given dir. */
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

	private Vector3[] GetSmoothCorners(Direction newDir, Direction lastDir, float smoothing = 0.4f)
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

	private Vector3 ModifyZ(Vector3 vec, float z)
	{
		/** Return a new Vector3 with the Z value mofidied. Useful to place objects above or behind each other in 2D. */
		return new Vector3(vec.x, vec.y, z);
	}
}