using UnityEngine;
using System.Collections;

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

	int segment_number;
	//GameObject tempcube;

	/* Returns a point on a cubic Catmull-Rom/Blended Parabolas curve
	 * u is a scalar value from 0 to 1
	 * segment_number indicates which 4 points to use for interpolation
	 */
	Vector3 ComputePointOnCatmullRomCurve(double u, int segmentNumber)
	{
		Vector3 point = new Vector3();
		Vector3 p0;
		Vector3 p1;
		Vector3 p2;
		Vector3 p3;
		
		if (segmentNumber == controlPoints.Length - 1)
		{
			p0 = controlPoints[segmentNumber-1];
			p1 = controlPoints[segmentNumber];
			p2 = controlPoints[0];
			p3 = controlPoints[1];
		}
        else if(segmentNumber == controlPoints.Length - 2)
        {
			p0 = controlPoints[segmentNumber-1];
			p1 = controlPoints[segmentNumber];
			p2 = controlPoints[segmentNumber + 1];
			p3 = controlPoints[0];
		}
		else if (segmentNumber == controlPoints.Length - 3)
		{
			p0 = controlPoints[segmentNumber - 1];
			p1 = controlPoints[segmentNumber];
			p2 = controlPoints[segmentNumber + 1];
			p3 = controlPoints[segmentNumber + 2];
        }
        else if(segmentNumber == 0)
        {
			p0 = controlPoints[controlPoints.Length - 1];
			p1 = controlPoints[segmentNumber];
			p2 = controlPoints[segmentNumber + 1];
			p3 = controlPoints[segmentNumber + 2];
        }
        else
        {
			p0 = controlPoints[segmentNumber - 1];
			p1 = controlPoints[segmentNumber];
			p2 = controlPoints[segmentNumber + 1];
			p3 = controlPoints[segmentNumber + 2];
		}


		//The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
		Vector3 a = 0.5f * 2f * p1;
		Vector3 b = 0.5f * p2 - p0;
		Vector3 c = 0.5f * 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 d = 0.5f * -p0 + 3f * p1 - 3f * p2 + p3;

		//The cubic polynomial: a + b * u + c * u^2 + d * u^3
		point = (a + (b * (float)u) + (c * (float)u * (float)u) + (d * (float)u * (float)u * (float)u));


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
	
	// Use this for initialization
	void Start () {

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
	}
	
	// Update is called once per frame
	void Update () {

		time += DT;
        if (Vector3.Distance(transform.position, controlPoints[segment_number]) > 0.1)
        {
            Vector3 temp = ComputePointOnCatmullRomCurve(DT, segment_number);
            transform.position = temp;
        }
        else
        {
			Debug.Log(segment_number);
            time = 0;
            if (segment_number < 7)
            {
				segment_number += 1;
            }
            else
            {
				segment_number = 0;
            }
        }

        // TODO - use time to determine values for u and segment_number in this function call


    }
}
