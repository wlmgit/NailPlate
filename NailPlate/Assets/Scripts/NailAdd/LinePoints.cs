using UnityEngine;
using System.Collections;

public class LinePoints: MonoBehaviour{


	private static LinePoints mLine;
	public string point1;
	public string point2;
	public bool IsFirstLine=false;
	public Color lineColor;
	public static LinePoints LineInstance
	{
		get
		{
			if(mLine==null)
			{
				mLine=new LinePoints();
			}
			return mLine;
		}
	}
	void Awake()
	{
		mLine = this;
	}

	public void setLinePoint(string Lpoint1,string Lpoint2)
	{
		point1=Lpoint1;
		point2 =Lpoint2;
	}
	public bool ExistPoint(GameObject _point)
	{
		if (point1 .Equals( _point.name) || point2.Equals ( _point.name))
			return true;
		else
			return false;
	}
}
