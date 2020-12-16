using UnityEngine;
using System.Collections.Generic;


public class Triangulator
{
	private List<Vector2> pointsList = new List<Vector2>();

	public Triangulator(Vector2[] points)
	{
		pointsList = new List<Vector2>(points);
	}

	public int[] Triangulate()
	{
		List<int> triangulatedPoints = new List<int>();

		int numOfPoints = pointsList.Count;
		if (numOfPoints < 3)
			return triangulatedPoints.ToArray();

		int[] VertexSequence = new int[numOfPoints];
		if (AreaOfPolygon() > 0)                                         //for sequencing all points in anti clockwise(area will be +ve)
		{
			for (int v = 0; v < numOfPoints; v++)
				VertexSequence[v] = v;
		}
		else
		{
			for (int v = 0; v < numOfPoints; v++)
				VertexSequence[v] = numOfPoints - 1 - v;
		}

		int realTimePointsCount = numOfPoints;

		for (int middle = 0; realTimePointsCount > 2;)
		{
			int first = middle;                                                 //for choosing 3 points
			if (realTimePointsCount <= first)
				first = 0;
			middle = first + 1;
			if (realTimePointsCount <= middle)
				middle = 0;
			int last = middle + 1;
			if (realTimePointsCount <= last)
				last = 0;

			if (CheckTriangulation(first, middle, last, realTimePointsCount, VertexSequence))
			{

				int a = VertexSequence[first];
				int b = VertexSequence[middle];
				int c = VertexSequence[last];
				triangulatedPoints.Add(a);
				triangulatedPoints.Add(b);
				triangulatedPoints.Add(c);
				for (int p = middle, q = middle + 1; q < realTimePointsCount; p++, q++)
					VertexSequence[p] = VertexSequence[q];
				realTimePointsCount--;

			}
		}

		triangulatedPoints.Reverse();                                           //solve plane's normal direction
		return triangulatedPoints.ToArray();
	}

	private float AreaOfPolygon()
	{
		int n = pointsList.Count;
		float A = 0.0f;
		for (int a = n - 1, b = 0; b < n; a = b++)
		{
			Vector2 pval = pointsList[a];
			Vector2 qval = pointsList[b];
			A += pval.x * qval.y - qval.x * pval.y;
		}
		return (A * 0.5f);
	}
	private float AreaOfTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		float A = ((p2.x - p1.x) * (p3.y - p1.y)) - ((p2.y - p1.y) * (p3.x - p1.x));

		return (A * 0.5f);
	}

	private bool CheckTriangulation(int u, int v, int w, int n, int[] V)
	{
		int p;
		Vector2 A = pointsList[V[u]];
		Vector2 B = pointsList[V[v]];
		Vector2 C = pointsList[V[w]];
		if (0 > AreaOfTriangle(A, B, C))                              //diagonal outside of polygon
			return false;
		for (p = 0; p < n; p++)
		{
			if ((p == u) || (p == v) || (p == w))
				continue;
			Vector2 P = pointsList[V[p]];
			if (PointInsideTriangle(A, B, C, P))                 // digonal crossing other sides
				return false;
		}
		return true;
	}


	private bool PointInsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)         //by checking area +ve
	{
		float Area1 = AreaOfTriangle(A, B, P);
		float Area2 = AreaOfTriangle(B, C, P);
		float Area3 = AreaOfTriangle(C, A, P);

		return ((Area1 >= 0.0f) && (Area2 >= 0.0f) && (Area3 >= 0.0f));
	}
}