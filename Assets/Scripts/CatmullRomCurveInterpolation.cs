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
		int lastPoint = controlPoints.Length - 1;
		// cant multipy vectors by doubles
		float t = 0.5f;

		//pi-2, pi-1, pi, pi+1
		Vector3 pim2;
		Vector3 pim1;
		Vector3 pi;
		Vector3 pip1;



		//if (segmentNumber == lastPoint)
		//{
		//	p0 = controlPoints[segmentNumber-1];
		//	p1 = controlPoints[segmentNumber];
		//	p0p = t * (controlPoints[segmentNumber] - controlPoints[segmentNumber-2]);
		//	p1p = t * (controlPoints[0] - controlPoints[segmentNumber-1]);
		//}
		//      else if(segmentNumber == 0)
		//      {
		//	p0 = controlPoints[lastPoint];
		//	p1 = controlPoints[segmentNumber];
		//	p0p = t * (controlPoints[segmentNumber] - controlPoints[lastPoint - 1]);
		//	p1p = t * (controlPoints[segmentNumber + 1] - controlPoints[lastPoint]);
		//}
		//      else if(segmentNumber == 1)
		//      {
		//	p0 = controlPoints[segmentNumber - 1];
		//	p1 = controlPoints[segmentNumber];
		//	p0p = t * (controlPoints[segmentNumber] - controlPoints[lastPoint]);
		//	p1p = t * (controlPoints[segmentNumber + 1] - controlPoints[segmentNumber - 1]);
		//}
		//      else
		//      {
		//	p0 = controlPoints[segmentNumber - 1];
		//	p1 = controlPoints[segmentNumber];
		//	p0p = t * (controlPoints[segmentNumber] - controlPoints[lastPoint]);
		//	p1p = t * (controlPoints[segmentNumber + 1] - controlPoints[segmentNumber - 1]);
		//}

		if (segmentNumber == lastPoint)
		{
			pim2 = controlPoints[segmentNumber-2];
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


		//The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
		//Vector3 a = 0.5f * 2f * p1;
		//Vector3 b = 0.5f * p0p - p0;
		//Vector3 c = 0.5f * 2f * p0 - 5f * p1 + 4f * p0p - p1p;
		//Vector3 d = 0.5f * -p0 + 3f * p1 - 3f * p0p + p1p;

		Vector3 c0 = pim1;
		Vector3 c1 = (-t) * pim2 + (t) * pi;
		Vector3 c2 = (2*t * pim2) + ((t-3) * pim1) + ((3 - 2*t) * pi) + ((-t) *  pip1);
		Vector3 c3 = ((-t) * pim2) + ((2 - t) * pim1) + ((t - 2) * pi) + (t * pip1);

		//The cubic polynomial: a + b * u + c * u^2 + d * u^3
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

		int num = 2;

		time += DT;
        //if (Vector3.Distance(transform.position, controlPoints[segment_number]) > 1)
		if(time/num < 1)
		{
            Vector3 temp = ComputePointOnCatmullRomCurve(time/num, segment_number);
            transform.position = temp;
        }
        else
        {
			Debug.Log(segment_number);
            time = 0;
            if (segment_number < NumberOfPoints - 1)
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
