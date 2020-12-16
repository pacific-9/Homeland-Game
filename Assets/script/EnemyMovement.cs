using UnityEngine;

public class EnemyMovement : Character
{
	public int positionsUptoReturn = 2;
	private int cursorPosition;
	private Vector3 targetPos;

	public override void Start()
	{
		targetPos = transform.position;

		base.Start();
	}

	public override void Update()
	{
		var transPos = transform.position;
		if ((targetPos - transPos).magnitude < .5f)
		{
			if (cursorPosition < positionsUptoReturn)
			{
				cursorPosition++;
				var targetPos2D = 25f * Random.insideUnitCircle;
				targetPos = new Vector3(targetPos2D.x, 0, targetPos2D.y);
			}
			else
			{
				cursorPosition = 0;
				targetPos = areaVertices[GetClosestAreaVertice(transPos)];
			}
		}

		curDir = (targetPos - transPos).normalized;

		base.Update();
	}
}