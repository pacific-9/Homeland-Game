using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public bool player = false;
	public string characterName;
	public Color color;
	public Material material;
	public List<Character> attackedCharacters = new List<Character>();

	
	public int startAreaPoints = 50;
	public float startAreaRadius = 3f;
	public float minPointDistance = 0.1f;
	
	public CharacterArea area;
	public List<Vector3> areaVertices = new List<Vector3>();
	public List<Vector3> newAreaVertices = new List<Vector3>();
	private MeshRenderer areaMeshRenderer;
	private MeshFilter areaFilter;
	public float areaSize=0.0f;

	protected Rigidbody rb;
	protected Vector3 curDir;
	protected Quaternion targetRot;

	public float speed = 2f;
	public float turnSpeed = 10f;
	public TrailRenderer trailRenderer;
	public GameObject trailCollidersHolder;
	public List<BoxCollider> trailCollider = new List<BoxCollider>();



	public virtual void Start()
	{
		rb = GetComponent<Rigidbody>();
		trailRenderer = transform.Find("Trail").GetComponent<TrailRenderer>();
		trailRenderer.material.color = new Color(color.r, color.g, color.b, 0.65f);
		GetComponent<MeshRenderer>().material.color = new Color(color.r * 1.3f, color.g * 1.3f, color.b * 1.3f);
		area = new GameObject().AddComponent<CharacterArea>();
		area.name = characterName + "Area";
		area.character = this;
		Transform areaTrans = area.transform;

		areaFilter = area.gameObject.AddComponent<MeshFilter>();
		areaMeshRenderer = area.gameObject.AddComponent<MeshRenderer>();
		areaMeshRenderer.material = material;
		areaMeshRenderer.material.color = color;


		float step = 360f / startAreaPoints;
		for (int i = 0; i < startAreaPoints; i++)
		{
			areaVertices.Add(transform.position + Quaternion.Euler(new Vector3(0, step * i, 0)) * Vector3.forward * startAreaRadius);
		}
		UpdateArea();

		trailCollidersHolder = new GameObject();
		trailCollidersHolder.transform.SetParent(areaTrans);
		trailCollidersHolder.name = characterName + "TrailCollidersHolder";
		trailCollidersHolder.layer = 8;
	}

	public virtual void Update()
	{
		var trans = transform;
		var transPos = trans.position;
		trans.position = Vector3.ClampMagnitude(transPos, 25f);
		bool outsideOfPolygon = !GameManager.IsPointInside(new Vector2(transPos.x, transPos.z), Vertices2D(areaVertices));
		int count = newAreaVertices.Count;

		if (outsideOfPolygon)
		{
			if (count == 0 || !newAreaVertices.Contains(transPos) && (newAreaVertices[count - 1] - transPos).magnitude >= minPointDistance)
			{
				count++;
				newAreaVertices.Add(transPos);

				int trailCollsCount = trailCollider.Count;
				float trailWidth = trailRenderer.startWidth;
				BoxCollider lastColl = trailCollsCount > 0 ? trailCollider[trailCollsCount - 1] : null;
				if (!lastColl || (transPos - lastColl.center).magnitude > trailWidth)
				{
					BoxCollider trailCollider = trailCollidersHolder.AddComponent<BoxCollider>();
					trailCollider.center = transPos;
					trailCollider.size = new Vector3(trailWidth , trailWidth, trailWidth );
					trailCollider.isTrigger = true;
					trailCollider.enabled = false;
					this.trailCollider.Add(trailCollider);

					if (trailCollsCount > 1)
					{
						this.trailCollider[trailCollsCount - 2].enabled = true;
					}
				}
			}

			if (!trailRenderer.emitting)
			{
				trailRenderer.Clear();
				trailRenderer.emitting = true;
			}
		}
		else if (count > 0)
		{
			GameManager.ChangeCharacterArea(this, newAreaVertices);
			
			foreach(var character in attackedCharacters)
			{
				List<Vector3> newCharacterAreaVertices = new List<Vector3>();
				foreach(var vertex in newAreaVertices)
				{
					if (GameManager.IsPointInside(new Vector2(vertex.x, vertex.z), Vertices2D(character.areaVertices)))
					{
						newCharacterAreaVertices.Add(vertex);
					}
				}

				GameManager.ChangeCharacterArea(character, newCharacterAreaVertices);
			}
			attackedCharacters.Clear();
			newAreaVertices.Clear();

			if (trailRenderer.emitting)
			{
				trailRenderer.Clear();
				trailRenderer.emitting = false;
			}			
			foreach (var trailColl in trailCollider)
			{
				Destroy(trailColl);
			}
			trailCollider.Clear();

		}
		
	}

	public virtual void FixedUpdate()
	{
		rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

		if (curDir != Vector3.zero)
		{
			targetRot = Quaternion.LookRotation(curDir);
			if(rb.rotation != targetRot)
			{
				rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRot, turnSpeed);
			}
		}
	}

	public void UpdateArea()
	{
		if (areaFilter)
		{
			Mesh areaMesh = GenerateMesh(areaVertices, characterName);
			areaFilter.mesh = areaMesh;
		
			area.characterMeshCollider.sharedMesh = areaMesh;//sharedMesh for collision detection
			areaSize = AreaOfPolygon();
			
		}
	}

	private Mesh GenerateMesh(List<Vector3> vertices, string meshName)
	{
		Triangulator triangulator = new Triangulator(Vertices2D(vertices));
		int[] indices = triangulator.Triangulate();

		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.name = meshName + "Mesh";
		return mesh;
	}
	private float AreaOfPolygon()
	{
		int n = areaVertices.Count;
		float A = 0.0f;
		for (int a = n - 1, b = 0; b < n; a = b++)
		{
			Vector3 pval = areaVertices[a];
			Vector3 qval = areaVertices[b];
			A += pval.x * qval.z - qval.x * pval.z;
		}
		//Debug.Log("area" +A);
		return (-A * 0.5f);
	}

	private Vector2[] Vertices2D(List<Vector3> vertices)
	{
		List<Vector2> areaVertices2D = new List<Vector2>();
		foreach (Vector3 vertex in vertices)
		{
			areaVertices2D.Add(new Vector2(vertex.x, vertex.z));
		}

		return areaVertices2D.ToArray();
	}

	public int GetClosestAreaVertice(Vector3 fromPos)
	{
		int closest = -1;
		float closestDist = Mathf.Infinity;
		for (int i = 0; i < areaVertices.Count; i++)
		{
			float dist = (areaVertices[i] - fromPos).magnitude;
			if (dist < closestDist)
			{
				closest = i;
				closestDist = dist;
			}
		}

		return closest;
	}

	private void OnTriggerEnter(Collider other)
	{
		CharacterArea characterArea = other.GetComponent<CharacterArea>();
		if (characterArea && characterArea != area && !attackedCharacters.Contains(characterArea.character))
		{
			attackedCharacters.Add(characterArea.character);
		}

		if (other.gameObject.layer == 8)
		{
			characterArea = other.transform.parent.GetComponent<CharacterArea>();
			GameManager.gameManager.winPoint++;
			//Debug.Log(GameManager.gameManager.winPoint);
			characterArea.character.Die();
		}

	}

	public void Die()
	{
		if (player)
		{
			GameManager.gameManager.GameOver();
		}
		else
		{
			
			
			

			Destroy(area.gameObject);
		
			Destroy(gameObject);
		}
	}

}