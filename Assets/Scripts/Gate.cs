using UnityEngine;

public enum OpeningStateEnum
{
	Stopped = 0,
	Opening = 1,
	Closing = 2,
}

[ExecuteInEditMode]
public class Gate : MonoBehaviour
{
    public GateDoor door1;
	public GateDoor door2;

	public float width = 2f;
    public float height = 2f;

	public float openingAngle = 90f;
	public float openingTime = 2f;

	private Mesh columnMesh;

    private Mesh doorMesh;

	private float angleDelta = 0;
	private float angleDirection = 1;

	public OpeningStateEnum openingState = OpeningStateEnum.Stopped;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		door1.width = width / 2f;
		door1.height = height;
		door2.width = width / 2f;
		door2.height = height;

		door1.gameObject.transform.localPosition = new Vector3(width / 2f, 0, 0)			;
		door1.gameObject.transform.localRotation = Quaternion.AngleAxis(180f,new Vector3(0,1,0));
		door1.leftHanded = false;

		door2.gameObject.transform.localPosition = new Vector3(-width / 2f, 0, 0);
		door2.gameObject.transform.localRotation = Quaternion.AngleAxis(0f, new Vector3(0, 1, 0));
		door2.leftHanded = true;
	}

    // Update is called once per frame
    void Update()
    {

		if (door1.width != width / 2f || door1.height != height) {
			door1.gameObject.transform.localPosition = new Vector3(width / 2f, 0, 0);
			door2.gameObject.transform.localPosition = new Vector3(-width / 2f, 0, 0);
			door1.width = width / 2f;
			door1.height = height;
			door2.width = width / 2f;
			door2.height = height;
		}


		switch (openingState)
		{
			case OpeningStateEnum.Stopped:
				return;
			case OpeningStateEnum.Opening:
				angleDirection = 1f;
				break;
			case OpeningStateEnum.Closing:
				angleDirection = -1f;
				break;
		}


		
		if (angleDirection > 0 && angleDelta > openingAngle)
		{
			angleDelta = openingAngle;
			openingState = OpeningStateEnum.Stopped;
			return;
		}
		else
		if (angleDirection < 0 && angleDelta < 0)
		{
			angleDelta = 0;
			openingState = OpeningStateEnum.Stopped;
			return;
		}

		angleDelta +=  Time.deltaTime / openingTime* openingAngle * angleDirection;

		door1.gameObject.transform.localRotation = Quaternion.AngleAxis(180f + angleDelta, new Vector3(0, 1, 0));
		door2.gameObject.transform.localRotation = Quaternion.AngleAxis(-angleDelta, new Vector3(0, 1, 0));


	}
}
