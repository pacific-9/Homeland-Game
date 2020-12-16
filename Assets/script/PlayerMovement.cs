
using UnityEngine;

public class PlayerMovement : Character
{
	public float sensitivity = 300f;
	public float turnTreshold = 15f;
	private Vector3 mouseStartPostion;

	public override void Update()
	{
		var mousePos = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
		{
			mouseStartPostion = mousePos;
		}
		else if (Input.GetMouseButton(0))
		{
			float distance = (mousePos - mouseStartPostion).magnitude;
			if (distance > turnTreshold)
			{
				if (distance > sensitivity)
				{
					mouseStartPostion = mousePos - (curDir * sensitivity / 2f);
				}

				var curDir2D = -(mouseStartPostion - mousePos).normalized;
				curDir = new Vector3(curDir2D.x, 0, curDir2D.y);
			}
		}
		else
		{
			curDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		}

		base.Update();
	}

}