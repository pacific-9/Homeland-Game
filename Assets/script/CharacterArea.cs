
using UnityEngine;

public class CharacterArea : MonoBehaviour
{
	public Character character;
	public MeshCollider characterMeshCollider;

	private void Awake()
	{
		characterMeshCollider = gameObject.AddComponent<MeshCollider>();
	}
}