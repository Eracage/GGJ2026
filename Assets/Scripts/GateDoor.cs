using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GateDoor: MonoBehaviour
{
	private Mesh mesh;

	public bool leftHanded;

	private float _width;
	public float width;

	private float _height;
	public float height;

	private void Start()
	{
		_height = height;
		_width = width;
		//CreateDoorMesh();
		
	}

    private void CreateDoorMesh()
    {
		mesh = new Mesh();
		var fenceProfile = new Profile(new List<Vector2>
		{
			new (-0.025f,-0.075f),
			new (0.025f,-0.075f),

			new (0.025f,0.075f),
			new (-0.025f,0.075f),
		});

		fenceProfile.CreateBeam(new Vector3(width, 0,0), new Vector3(0, 0, 0), mesh, 20f);
		fenceProfile.CreateBeam(new Vector3(width, height,0), new Vector3(0, height, 0), mesh, 20f);
		fenceProfile.CreateBeam(new Vector3(width, 0, 0), new Vector3(0, height, 0), mesh, 20f);
		fenceProfile.CreateBeam(new Vector3(width, height, 0), new Vector3(0, 0, 0), mesh, 20f);
		fenceProfile.CreateColumn(new Vector3(0.075f, 0, 0),new Vector3(width - 0.075f, 0, 0), 0,height, mesh, 20f);
		fenceProfile.CreateColumn(new Vector3(0.075f, 0, 0),new Vector3(width - 0.075f, 0, 0), 1, height, mesh, 20f);

		var collider = gameObject.GetComponent<BoxCollider>();
		collider.center = new Vector3(width / 2f, height/2f, 0);
		collider.size = new Vector3(width, height, 0.1f);
		GetComponent<MeshFilter>().mesh = mesh;
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			if (_height != height || _width != width)
			{
				_height = height;
				_width = width;
				CreateDoorMesh();
			}
		}
	}
}
