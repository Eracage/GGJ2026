using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class Fence : MonoBehaviour
{
	private Mesh mesh;

	private List<Vector3> _points = new List<Vector3>();
	


	private List<Transform> Transforms;
	public float startOffsetOfColumn = 0;
	public float columnHeight = 1.8f;
	public float textVerticalStep = 100;
	public float textHorizontalStep = 1;
	public float columnSpacing = 1;

	private void Start()
	{
		Transforms = new List<Transform>();
		// Create a new mesh instance
		mesh = new Mesh();
		foreach (Transform t in transform) {
			Transforms.Add(t);
				}
		// Assign the mesh to the Meshfilter
		GetComponent<MeshFilter>().mesh = mesh;
		Update();
	}

	private void Update()
	{		
		
		if (_points.Count != Transforms.Count)
		{
			_points = Transforms.Select(o => o.transform.localPosition).ToList();
		}

		for (var i=0; i < _points.Count; i++)
		{
			if (_points[i] != Transforms[i].position)
			{
				_points = Transforms.Select(o => o.transform.localPosition).ToList();
				CreateTriangle();
				break;
			}
		}
		
	}


	private void CreateTriangle()
	{
		var fenceProfile = new Profile(new List<Vector2>
		{
			new (-0.025f,-0.075f),			
			new (0.025f,-0.075f),

			new (0.025f,0.075f),			
			new (-0.025f,0.075f),
		});

		var columnScale = 0.02f;
		var columnProfile = new Profile(new List<Vector2>
		{
			new (-2*columnScale,-2*columnScale),
			new (0,-3*columnScale),
			new (2*columnScale,-2*columnScale),
			new (2*columnScale,2*columnScale),
			new (0,3*columnScale),
			new (-2*columnScale,2*columnScale),
		});

		// Define arrays for vertices, UVs, and triangles

		mesh.Clear();

		if (_points.Count < 2)
		{
			return;
		}

		for (var p = 0; p < _points.Count-1;p++) {

			var p1 = _points[p];
			var p2 = _points[p+1];

			fenceProfile.CreateBeam(p1 + new Vector3(0, 0.4f * columnHeight, 0), p2 + new Vector3(0, 0.4f * columnHeight, 0), mesh, textHorizontalStep);
			fenceProfile.CreateBeam(p1 + new Vector3(0, 0.9f * columnHeight, 0), p2 + new Vector3(0, 0.9f * columnHeight, 0), mesh, textHorizontalStep);

			var direction = p2 - p1;
			var side = Vector3.Normalize(new Vector3(-direction.z, 0, direction.x));

			var length = direction.magnitude;
			var columnCount = Mathf.Max(2, Mathf.Ceil(length / columnSpacing));
			var offset = columnSpacing/2;
			var step = (length-offset*2) / (columnCount - 1);

			var currentLocation = offset/length * length;
			var conversion = 0f;
			for (var i = 0f; i < columnCount; i++)
			{
				conversion = currentLocation / length;
				columnProfile.CreateColumn(p1 + new Vector3(0, startOffsetOfColumn, 0)+ side* 0.02f, p2 + new Vector3(0, startOffsetOfColumn, 0) + side * 0.02f, conversion, columnHeight, mesh, textVerticalStep);
				currentLocation += step;
			}

			var boxC = Transforms[p].gameObject.GetComponent<BoxCollider>();

			if (boxC == null)
			{
				Transforms[p].gameObject.AddComponent<BoxCollider>();
				boxC = Transforms[p].gameObject.GetComponent<BoxCollider>();
			}

			Transforms[p].LookAt(Transforms[p + 1]);
			
			boxC.center = new Vector3(0,0,((p2 - p1) * 0.5f).magnitude);
			var sx = 0.2f;
			var sy = columnHeight;
			var sz = ((p1 - p2)).magnitude;
			boxC.size = new Vector3(sx, sy, sz);

		}

		GetComponent<MeshFilter>().mesh = mesh;
	}
}

public class Profile
{
	public List<Vector2> SectionPoints { get; set; }
	public Profile(List<Vector2> sections)
	{
		SectionPoints = sections;
	}

	public void CreateColumn(Vector3 from, Vector3 to, float location, float height, Mesh result, float textVerticalStep)
	{
		var firstId = result.vertices.Length > 0 ? result.vertices.Length : 0;
		var direction = to - from;
		var length = direction.magnitude;

		var orginalDirection = Vector3.Normalize(direction);
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

		


		for (var i = 0; i < SectionPoints.Count; i++)
		{

			startPoints.Add(from + location * length * orginalDirection + SectionPoints[i].x * side + SectionPoints[i].y * direction);
			endPoints.Add(from + location * length * orginalDirection + SectionPoints[i].x * side + SectionPoints[i].y * direction + up * height);

			startUvs.Add(new Vector2(0, ((float)i) / SectionPoints.Count * length / textVerticalStep));
			endUvs.Add(new Vector2(1, ((float)i) / SectionPoints.Count * length / textVerticalStep));

			
			
			triangles.Add(firstId + (i + 1) % SectionPoints.Count + SectionPoints.Count);
			triangles.Add(firstId + i + SectionPoints.Count);
			triangles.Add(firstId + i);

			
			triangles.Add(firstId + (i + 1) % SectionPoints.Count);
			triangles.Add(firstId + (i + 1) % SectionPoints.Count + SectionPoints.Count);
			triangles.Add(firstId + i);

			if (i < SectionPoints.Count - 1)
			{
								
				startEndingTriangles.Add(firstId + (i + 2) % SectionPoints.Count);
				startEndingTriangles.Add(firstId + (i + 1) % SectionPoints.Count);
				startEndingTriangles.Add(firstId + 0);
								
				startEndingTriangles.Add(firstId + SectionPoints.Count);
				startEndingTriangles.Add(firstId + (i + 1) % SectionPoints.Count + SectionPoints.Count);
				startEndingTriangles.Add(firstId + (i + 2) % SectionPoints.Count + SectionPoints.Count);
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

		if (firstId > 0)
		{
			finalPoints.InsertRange(0, result.vertices);
			triangles.InsertRange(0, result.triangles);
			finalUvs.InsertRange(0, result.uv);
		}
		var varray = finalPoints.ToArray();
		result.vertices = varray;
		result.triangles = triangles.ToArray();
		result.uv = finalUvs.ToArray();
	}

	public void CreateBeam(Vector3 from, Vector3 to, Mesh result, float textHorizontalStep)
	{

		var firstId = result.vertices.Length > 0 ? result.vertices.Length : 0;
		var direction = to - from;
		var l = direction.magnitude;
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


		for (var i = 0; i < SectionPoints.Count; i++)
		{

			startPoints.Add(from + SectionPoints[i].x * side + SectionPoints[i].y * up);
			endPoints.Add(to + SectionPoints[i].x * side + SectionPoints[i].y * up);

			startUvs.Add(new Vector2(0, ((float)i) / SectionPoints.Count * l / textHorizontalStep));
			endUvs.Add(new Vector2(1, ((float)i) / SectionPoints.Count * l / textHorizontalStep));

			triangles.Add(firstId + i);
			triangles.Add(firstId + i + SectionPoints.Count);
			triangles.Add(firstId + (i + 1) % SectionPoints.Count + SectionPoints.Count);

			triangles.Add(firstId + i);
			triangles.Add(firstId + (i + 1) % SectionPoints.Count + SectionPoints.Count);
			triangles.Add(firstId + (i + 1) % SectionPoints.Count);

			if (i < SectionPoints.Count - 1)
			{
				startEndingTriangles.Add(firstId + 0);
				startEndingTriangles.Add(firstId + (i + 1) % SectionPoints.Count);
				startEndingTriangles.Add(firstId + (i + 2) % SectionPoints.Count);

				startEndingTriangles.Add(firstId + (i + 2) % SectionPoints.Count + SectionPoints.Count);
				startEndingTriangles.Add(firstId + (i + 1) % SectionPoints.Count + SectionPoints.Count);
				startEndingTriangles.Add(firstId + SectionPoints.Count);
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

		if (firstId > 0)
		{
			finalPoints.InsertRange(0, result.vertices);
			triangles.InsertRange(0, result.triangles);
			finalUvs.InsertRange(0, result.uv);
		}
		var varray = finalPoints.ToArray();
		result.vertices = varray;
		result.triangles = triangles.ToArray();
		result.uv = finalUvs.ToArray();
		
	}
}

