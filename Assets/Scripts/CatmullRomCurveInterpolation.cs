using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomCurveInterpolation : MonoBehaviour {
	
	const int NumberOfPoints = 8;
	Vector3[] controlPoints;
	
	const int MinX = -5;
	const int MinY = -5;
	const int MinZ = 0;

	const int MaxX = 5;
	const int MaxY = 5;
	const int MaxZ = 5;
	
	double time = 0;
	const double DT = 0.01;

	Vector3[] arcPoints;
	int arcPos;
	int numPoints;

	public float interval;
	IList<GameObject> dots = new List<GameObject>();

	//GameObject tempcube;

	/* Returns a point on a cubic Catmull-Rom/Blended Parabolas curve
	 * u is a scalar value from 0 to 1
	 * segment_number indicates which 4 points to use for interpolation
	 */
	Vector3 ComputePointOnCatmullRomCurve(double u, int segmentNumber)
	{
		Vector3 point;
		int lastPoint = controlPoints.Length - 1;
		float t = 0.5f;

		//pi-2, pi-1, pi, pi+1
		Vector3 pim2;
		Vector3 pim1;
		Vector3 pi;
		Vector3 pip1;

		if (segmentNumber == lastPoint)
		{
			pim2 = controlPoints[segmentNumber - 2];
			pim1 = controlPoints[segmentNumber - 1];
			pi = controlPoints[segmentNumber];
			pip1 = controlPoints[0];
		}
		else if (segmentNumber == 0)
		{
			pim2 = controlPoints[lastPoint - 1];
			pim1 = controlPoints[lastPoint];
			pi = controlPoints[segmentNumber];
			pip1 = controlPoints[segmentNumber + 1];
		}
		else if (segmentNumber == 1)
		{
			pim2 = controlPoints[lastPoint];
			pim1 = controlPoints[segmentNumber - 1];
			pi = controlPoints[segmentNumber];
			pip1 = controlPoints[segmentNumber + 1];
		}
		else
		{
			pim2 = controlPoints[segmentNumber - 2];
			pim1 = controlPoints[segmentNumber - 1];
			pi = controlPoints[segmentNumber];
			pip1 = controlPoints[segmentNumber + 1];
		}


		Vector3 c0 = pim1;
		Vector3 c1 = (-t) * pim2 + (t) * pi;
		Vector3 c2 = (2*t * pim2) + ((t-3) * pim1) + ((3 - 2*t) * pi) + ((-t) *  pip1);
		Vector3 c3 = ((-t) * pim2) + ((2 - t) * pim1) + ((t - 2) * pi) + (t * pip1);

		//The cubic polynomial: c0 + c1 * u + c2 * u^2 + c3 * u^3
		point = (c0 + (c1 * (float)u) + (c2 * (float)u * (float)u) + (c3 * (float)u * (float)u * (float)u));


		// TODO - compute and return a point as a Vector3		
		// Hint: Points on segment number 0 start at controlPoints[0] and end at controlPoints[1]
		//		 Points on segment number 1 start at controlPoints[1] and end at controlPoints[2]
		//		 etc...

		return point;
	}
	
	void GenerateControlPointGeometry()
	{
		for(int i = 0; i < NumberOfPoints; i++)
		{
			GameObject tempcube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			tempcube.transform.localScale -= new Vector3(0.8f,0.8f,0.8f);
			tempcube.transform.position = controlPoints[i];
		}	
	}

    Vector3[] CalculateArcs()
	{
		arcPoints = new Vector3[1000];
		int arrPos = 0;
		for(int i = 0; i < NumberOfPoints; i++)
        {
			//number of points between two control points
			float resolution = 0.1f;
			//determines where on the curve to put the points based off resolution
			int stops = Mathf.FloorToInt(1f / resolution);
			for (int j = 1; j <= stops; j++)
			{
				//Which t position are we at?
				float u = j * resolution;
				//Find the coordinate between the end points with a Catmull-Rom spline
				arcPoints[arrPos] = ComputePointOnCatmullRomCurve(u, i);
				arrPos += 1;
				numPoints += 1;
			}
			arcPoints[arrPos] = controlPoints[i];
			arrPos += 1;
			numPoints += 1;
		}
		return arcPoints;
	}
	
	// Use this for initialization
	void Start () {

		arcPos = 0;
		numPoints = 0;
		interval = 0.1f;

		controlPoints = new Vector3[NumberOfPoints];
		
		// set points randomly...
		controlPoints[0] = new Vector3(0,0,0);
		for(int i = 1; i < NumberOfPoints; i++)
		{
			controlPoints[i] = new Vector3(Random.Range(MinX,MaxX),Random.Range(MinY,MaxY),Random.Range(MinZ,MaxZ));
		}
		/*...or hard code them for testing
		controlPoints[0] = new Vector3(0,0,0);
		controlPoints[1] = new Vector3(0,0,0);
		controlPoints[2] = new Vector3(0,0,0);
		controlPoints[3] = new Vector3(0,0,0);
		controlPoints[4] = new Vector3(0,0,0);
		controlPoints[5] = new Vector3(0,0,0);
		controlPoints[6] = new Vector3(0,0,0);
		controlPoints[7] = new Vector3(0,0,0);
		*/
		
		GenerateControlPointGeometry();
		CalculateArcs();

		//Handles.color = Color.yellow;
		//for (int i = 0; i < arcPoints.Length - 1; i++)
		//{
		//	Handles.DrawLine(arcPoints[i], arcPoints[i + 1]);
		//}

	}
	void OnDrawGizmosSelected()
	{
		for (int i = 0; i < NumberOfPoints - 1; i++)
		{
			Debug.Log("Here");
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(arcPoints[i], arcPoints[i + 1]);
		}
	}
	// Update is called once per frame
	void Update () {
		time += DT;
        if (time > interval)
        {
			if(arcPos > (numPoints / 2))
			{
				interval += 0.005f;
				Debug.Log("decelerating");
            }
			else
            {
				if(interval > 0.01f)
                {
					interval -= 0.005f;
				}

				Debug.Log("accerlating");
            }
			transform.position = arcPoints[arcPos];

			if (arcPos > 0)
            {
				GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				dot.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
				var renderer = dot.transform.GetComponent<Renderer>();
				renderer.material.SetColor("_Color", Color.yellow);
				dot.transform.position = arcPoints[arcPos - 1];
			}
			
			arcPos += 1;
			time = 0;
        }
        // TODO - use time to determine values for u and segment_number in this function call

    }
}
