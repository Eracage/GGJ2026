using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class Fence : MonoBehaviour
{
	private Mesh mesh;

	private Vector3 _p1 = new Vector3();
	private Vector3 _p2 = new Vector3();

	public Vector3 P1 = new Vector3();
	public Vector3 P2 = new Vector3();	

	private void Start()
	{
		// Create a new mesh instance
		mesh = new Mesh();

		// Assign the mesh to the Meshfilter
		GetComponent<MeshFilter>().mesh = mesh;

		CreateTriangle();
	}

	private void Update()
	{
		if (_p1 != P1)
		{
			_p1 = P1;
			CreateTriangle();
		}

		if (_p2 != P2)
		{
			_p2 = P2;
			CreateTriangle();
		}
	}


	private void CreateTriangle()
	{
		var profile = new Profile(new List<Vector2>
		{
			new (-2,-2),
			new (0,-3),
			new (2,-2),

			new (2,2),
			new (0,3),
			new (-2,2),
		});

		// Define arrays for vertices, UVs, and triangles
	  profile.CreateBeam(_p1, _p2, mesh);
		
	}
}

public class Profile
{
	public List<Vector2> SectionPoints { get; set; }
	public Profile(List<Vector2> sections)
	{
		SectionPoints = sections;
	}

	public void CreateBeam(Vector3 from , Vector3 to, Mesh result)
	{
	
		
		var direction = to - from;
		direction.y = 0;
		direction = Vector3.Normalize(direction);
		var side = new Vector3(-direction.z, 0, direction.x);
		var up = new Vector3(0, 1, 0);
		
		var startPoints = new List<Vector3>();
		var endPoints = new List<Vector3>();
		var startUvs = new List<Vector2>();
		var endUvs = new List<Vector2>();

		var triangles = new List<int>();
		var startEndingTriangles = new List<int>();
		var endEndingTriangles = new List<int>();


		for (var i = 0; i < SectionPoints.Count; i++) {

			startPoints.Add(from + SectionPoints[i].x * side + SectionPoints[i].y * up);
			endPoints.Add(to + SectionPoints[i].x * side + SectionPoints[i].y * up);

			startUvs.Add(new Vector2(0,0));
			endUvs.Add(new Vector2(1, 0));

			triangles.Add(i);
			triangles.Add(i + SectionPoints.Count);
			triangles.Add((i + 1) % SectionPoints.Count + SectionPoints.Count);

			triangles.Add(i);
			triangles.Add((i + 1) % SectionPoints.Count + SectionPoints.Count);
			triangles.Add((i + 1) % SectionPoints.Count);

			if (i < SectionPoints.Count - 1)
			{
				startEndingTriangles.Add(0);
				startEndingTriangles.Add((i + 1) % SectionPoints.Count);
				startEndingTriangles.Add((i + 2) % SectionPoints.Count);

				startEndingTriangles.Add((i + 2) % SectionPoints.Count + SectionPoints.Count);
				startEndingTriangles.Add((i + 1) % SectionPoints.Count + SectionPoints.Count);
				startEndingTriangles.Add(SectionPoints.Count);								
			}
		}

		triangles.AddRange(startEndingTriangles);
		triangles.AddRange(endEndingTriangles);

		var finalPoints = new List<Vector3>();
		finalPoints.AddRange(startPoints);
		finalPoints.AddRange(endPoints);

		var finalUvs = new List<Vector2>();
		finalUvs.AddRange(startUvs);
		finalUvs.AddRange(endUvs);
		var varray = finalPoints.ToArray();
		result.vertices = varray;
		result.triangles = triangles.ToArray();
		
		result.uv = finalUvs.ToArray();


	
	}
}
