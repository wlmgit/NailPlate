using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VectorList;

public class Line{
	public string  Lpoint1;
	public string  Lpoint2;
}
//public enum colorlState
//{
//	mRed,
//	mBlue,
//	mYellow,
//}
public class LineStr
{
	public string Lpoint1;
	public string Lpoint2;
	public LineStr(string point1,string point2)
	{
		Lpoint1 = point1;
		Lpoint2 = point2;
	}
}

public class Nails
{
	public List<LinkList> mPoints=new List<LinkList>();
	public List<Color> mColor = new List<Color> ();
}
public class GameControl : MonoBehaviour {
	private static GameControl mGameControl;
    //ing Nail
	public static List<Line> mLines = new List<Line> ();
	public static LinkList mLinks = new LinkList ();
	public static LinkList tempLink = new LinkList ();
	//Nails
	public static Nails nailControl=new Nails();
	public static bool isReflect=false;
	public static bool isdoubleClick=false;
	public Vector3[] pointPos;
	//color
	public static Color getColor; 
	public static GameControl GameControlInstance
	{
		get
		{
			if(mGameControl==null)
			{
				mGameControl=new GameControl ();
			}
			return mGameControl;
		}
	}

	void Awake()
	{
		mGameControl = this;
		setNailPosition ();
	}

	// Use this for initialization
	public GameObject NailL_U;
	public GameObject NailL_D;
	public GameObject NailR_U;
	public GameObject NailR_D;
	public Transform NailPoint;
	public Transform objParent;
	public int NailStyle;
	public bool NailStyleChange;
	public static float EdgeLenth;
	public static float area;
	public Area_Lenth lenth = new Area_Lenth ();
	void Start () {
		NailStyle = 5;
		set_Nail (NailStyle);
	}
	
	// Update is called once per frame
	void Update () {
		setEdge ();
		setPointPos (mLinks);
		if (GameObject.Find ("lenth") != null && GameObject.Find ("area") != null) 
		{
			GameObject.Find ("lenth").GetComponent<UILabel> ().text = lenth.getLenth (pointPos).ToString ();
			GameObject.Find ("area").GetComponent<UILabel> ().text = area.ToString ("0.0");
		}
	}
	public void setEdge()
	{
		EdgeLenth = (NailR_U.transform.position.x - NailL_U.transform.position.x) /( NailStyle-1);
	}
	public bool checkPlane()
	{
		return GameObject.Find (GameControl.GameControlInstance.getPlaneName(GameControl.mLinks))!=null;
	}
	public void set_Nail(int nailFlag)
	{
		float Pos_x = (NailR_U.transform.position.x - NailL_U.transform.position.x) /(nailFlag-1);
		float Pos_y = (NailL_U.transform.position.y - NailL_D.transform.position.y) /(nailFlag-1);
		for (int i=1; i<nailFlag-1; i++)
		{
			Instantiate (NailPoint, new Vector3(NailL_U.transform.position.x+Pos_x*i, NailL_U.transform.position.y,19), Quaternion.identity);
			GameObject.Find("NailPoint(Clone)").transform.name="Nail0"+i.ToString()+"r0";
			GameObject.Find("Nail0"+i.ToString()+"r0").transform.parent=objParent;
		}
		for (int i=1;i < nailFlag-1; i++) {
			for(int j=0;j<nailFlag;j++)
			{
				Instantiate (NailPoint, new Vector3(NailL_U.transform.position.x+Pos_x*j, NailL_U.transform.position.y-Pos_y*i,19), Quaternion.identity);
				GameObject.Find("NailPoint(Clone)").transform.name="Nail"+i.ToString()+j.ToString()+"r"+i.ToString();
				GameObject.Find("Nail"+i.ToString()+j.ToString()+"r"+i.ToString()).transform.parent=objParent;
		    }
	}
		for (int i=1; i<nailFlag-1; i++)
		{
			Instantiate (NailPoint, new Vector3(NailL_D.transform.position.x+Pos_x*i, NailL_D.transform.position.y,19), Quaternion.identity);
			GameObject.Find("NailPoint(Clone)").transform.name="Nail"+(nailFlag-1).ToString()+i.ToString()+"r"+(nailFlag-1).ToString();
			GameObject.Find("Nail"+(nailFlag-1).ToString()+i.ToString()+"r"+(nailFlag-1).ToString()).transform.parent=objParent;
		}
		NailStyleChange = false;
   }
	public void setPointPos(LinkList pointStr)
	{
		pointPos=new Vector3[pointStr.Length()];
		for (int i=0; i<pointStr.Length(); i++)
			pointPos [i] =GameObject.Find( pointStr.get_Listnode (i+1)).transform.position;
	}
	public float getArea(Vector3[] pointPos)
	{
		float x = Mathf.Abs(pointPos [1].x - pointPos [0].x);
		float y = Mathf.Abs(pointPos [2].y - pointPos [0].y);
		return x * y;
	}
	public void setNailPosition()
	{
		NailL_U = GameObject.Find ("NailPointL_U");
		NailL_D = GameObject.Find ("NailPointL_D");
		NailR_U = GameObject.Find ("NailPointR_U");
		NailR_D = GameObject.Find ("NailPointR_D");
	}
	public void addLinePoint(string _point1,string _point2)
	{
		Line _Line=new Line();
		_Line.Lpoint1 = _point1;
		_Line.Lpoint2 = _point2;
		mLines.Remove(_Line);
	}
	public void removePoint(string point1,string point2)
	{
		Line _Line=new Line();
		_Line.Lpoint1 = point1;
		_Line.Lpoint2 = point2;
		mLines.Add (_Line);
	}
	public void remove(int lineIndex)
	{
		mLines.RemoveAt (lineIndex);
	}
	public int getLinePoint()
	{
		return mLines.Count;
	}
	public int pointlinks;
	public int PointgetNailIndex(string _pName)
	{
		pointlinks = 0;
		int nailIndex=-1;
		if (GameObject.Find (_pName).tag == "point") {
			for (int j=0; j<GameControl.nailControl.mPoints.Count; j++) {
				int temp = nailIndex;
				for (int k=1; k<=GameControl.nailControl.mPoints[j].Length(); k++) {
					if (GameControl.nailControl.mPoints [j].get_Listnode (k).Equals (_pName)) {
						GameControl.mLinks = GameControl.nailControl.mPoints [j];
//						GameControl.getColor=GameControl.nailControl.mColor[j];
						nailIndex = j;
					}
				}
				if (temp != nailIndex)
					pointlinks++;
			}
		} else 
		{
			string point1=GameObject.Find(_pName).GetComponent<LinePoints>().point1;
			string point2=GameObject.Find(_pName).GetComponent<LinePoints>().point2;
			for (int j=0; j<GameControl.nailControl.mPoints.Count; j++) {
				int temp = nailIndex;
				for (int k=1; k<=GameControl.nailControl.mPoints[j].Length(); k++) {
					if (GameControl.nailControl.mPoints [j].get_Listnode (k).Equals (point1)&&GameControl.nailControl.mPoints [j].get_Listnode (k+1).Equals (point2)) {
						GameControl.mLinks = GameControl.nailControl.mPoints [j];
						nailIndex = j;
					}
				}
				if (temp != nailIndex)
					pointlinks++;
			}
		}
		GameControl.getColor=GameControl.nailControl.mColor[nailIndex];
		return nailIndex;
	}
	public void updaLine()
	{
		for(int i=0;i<GameObject.FindGameObjectsWithTag("line").Length;i++)
		Destroy (GameObject.FindGameObjectsWithTag("line")[i]);
		for (int i=0; i<nailControl.mPoints.Count; i++)
		{
			for(int j=1;j<nailControl.mPoints[i].Length();j++)
			{
				GameObject link1=GameObject.Find(nailControl.mPoints[i].get_Listnode(j));
				GameObject link2=GameObject.Find(nailControl.mPoints[i].get_Listnode(j+1));
				MLine line=new MLine(link1,link2,link1.transform);
				line.setColor(GameControl.nailControl.mColor[i]);
				line.obj.AddComponent<LinePoints>().setLinePoint(link1.name,link2.name);
//				if(i==nailControl.mPoints.Count-1)
//				{
//					line.obj.transform.position=new Vector3(0,0,-1);
//				}
			}
		}
	}
	public void drawReflect(LinkList _pointPos,bool isFill)
	{
		Destroy (GameObject.Find("horizPlane"));
		Destroy (GameObject.Find ("refPlane"));
		horizSymmetric mHorizSymm = new horizSymmetric ();
		mHorizSymm._planeColor = GameControl.getColor;
		mHorizSymm.reflectPlane (_pointPos);
		if (isFill) 
		{
			mHorizSymm.drawRefPlane(mHorizSymm.reflectPos);
		}
	}
	public int check(string point)
	{
		int count = 0;
		for (int i=1; i<GameControl.mLinks.Length(); i++)
		{
			if(mLinks.get_Listnode(i).Equals(point))
				count++;
		}
		return count;
	}
	public Line getLine(int lineIndex)
	{
		return mLines[lineIndex];
	}
	public bool IsExist(GameObject _point1)
	{
		bool isExist = false;
		for (int i=0; i<nailControl.mPoints.Count; i++) 
		{
			for(int j=1;j<=nailControl.mPoints[i].Length();j++)
			{
				if(nailControl.mPoints[i].get_Listnode(j).Equals(_point1.name))
					isExist=true;
			}
		}
		return isExist;
	}
	public int getMlineIndex(GameObject _point1,GameObject _point2)
	{
		int index=0;
		for (int i=0; i<mLines.Count; i++) 
		{
			if(mLines[i].Lpoint1==_point1.name&&mLines[i].Lpoint2==_point2.name)
				index=i;
		}
		return index;
	}
	public void InitNail()
	{
		for (int i = 0; i<GameObject.FindGameObjectsWithTag("point").Length; i++) {
			GameObject go = GameObject.FindGameObjectsWithTag("point")[i];
			if (go != NailL_U && go != NailL_D && go != NailR_U && go != NailR_D) 
			Destroy(go);
		}
	}
	public void setParent()
	{
		for (int i=0; i<GameObject.FindGameObjectsWithTag("point").Length; i++)
		{
			if(GameObject.FindGameObjectsWithTag("point")[i].transform.parent!=objParent)
				GameObject.FindGameObjectsWithTag("point")[i].transform.parent=objParent;
		}
	}
	public static int temp1=0;
	public static int temp2=0;
	public void destroyTemp(string flag,string point)
	{
		int count_links = GameControl.mLinks.Length ();
		int pointIndex=GameControl.mLinks.get_Index(flag);

		if(pointIndex==1)
		{
//			if(GameObject.Find("line"+point+GameControl.mLinks.get_Listnode(2))!=null&&temp1==0)
//			{
				Destroy(GameObject.Find ("line"+point+mLinks.get_Listnode(2)));
			checkColor(point,mLinks.get_Listnode(2));
//				temp1=1;
//			}
//			if(GameObject.Find("line"+GameControl.mLinks.get_Listnode(count_links-1)+point)!=null&&temp2==0)
//			{
				Destroy(GameObject.Find ("line"+mLinks.get_Listnode(count_links-1)+point));
			checkColor(mLinks.get_Listnode(count_links-1),point);
//				temp2=1;
//			}
			if(GameObject.Find("linedragObj"+GameControl.mLinks.get_Listnode(2))!=null)
				Destroy (GameObject.Find ("line" +"dragObj"+GameControl.mLinks.get_Listnode(2)));
			if(GameObject.Find("line"+GameControl.mLinks.get_Listnode(count_links-1)+"dragObj")!=null)
				Destroy (GameObject.Find ("line" + GameControl.mLinks.get_Listnode(count_links-1)+"dragObj"));
		}
		else
		{
			string obj="line"+point+GameControl.mLinks.get_Listnode(pointIndex+1);
			string obj1="line"+GameControl.mLinks.get_Listnode(pointIndex-1)+point;
//			if(GameObject.Find(obj)!=null&&temp1==0)
//			{
				Destroy(GameObject.Find (obj));
			checkColor(point,GameControl.mLinks.get_Listnode(pointIndex+1));
//				temp1=1;
//			}
//			if(GameObject.Find(obj1)!=null&&temp2==0)
//			{
				Destroy(GameObject.Find (obj1));
			checkColor(GameControl.mLinks.get_Listnode(pointIndex-1),point);
//				temp2=1;
//			}
			if(GameObject.Find("line"+GameControl.mLinks.get_Listnode(pointIndex-1)+"dragObj")!=null)
				Destroy (GameObject.Find ("line" + GameControl.mLinks.get_Listnode(pointIndex-1)+"dragObj"));
			if(GameObject.Find("linedragObj"+GameControl.mLinks.get_Listnode(pointIndex+1))!=null)
				Destroy (GameObject.Find ("line" +"dragObj"+ GameControl.mLinks.get_Listnode(pointIndex+1)));
		}
	}
	public bool checkNext(string point1,string point2)
	{
		bool single = false;
		for (int i=0; i<nailControl.mPoints.Count; i++)
		{
			if(nailControl.mPoints[i]!=GameControl.mLinks)
			{
				for(int j=1;j<nailControl.mPoints[i].Length();j++)
				{
					if(nailControl.mPoints[i].get_Listnode(j).Equals(point1)&&nailControl.mPoints[i].get_Listnode(j+1).Equals(point2))
					{
						single=true;
					}
				}
			}
		}
		return single;
	}
	public void updateLines(string flag,GameObject point,int nailIndex)
	{
		int count_links= GameControl.mLinks.Length ();
		int pointIndex = GameControl.mLinks.get_Index (point.name);
		destroyTemp (flag, point.name);
		GameObject link1;
		GameObject link2;
		if (pointIndex == 1) 
		{
			link1 = GameObject.Find (GameControl.mLinks.get_Listnode (count_links - 1));
			link2 = GameObject.Find (GameControl.mLinks.get_Listnode (pointIndex + 1));
			MLine line1 = new MLine (link1, point, link1.transform);
			line1.setColor(GameControl.getColor);
			line1.obj.AddComponent<LinePoints> ().setLinePoint (link1.name, point.name);
			setComplent (link1, point);
			MLine line2 = new MLine (point, link2, point.transform);
			line2.obj.AddComponent<LinePoints> ().setLinePoint (point.name, link2.name);
			line2.setColor(GameControl.getColor);
			setComplent (link2, point);
		}
		else 
		{
			link1 = GameObject.Find (GameControl.mLinks.get_Listnode (pointIndex - 1));
			link2 = GameObject.Find (GameControl.mLinks.get_Listnode (pointIndex + 1));
			MLine line1 = new MLine (link1, point, link1.transform);
			line1.setColor(GameControl.getColor);
			line1.obj.AddComponent<LinePoints> ().setLinePoint (link1.name, point.name);
			setComplent (link1, point);
			MLine line2 = new MLine (point, link2, point.transform);
			line2.setColor(GameControl.getColor);
			line2.obj.AddComponent<LinePoints> ().setLinePoint (point.name, link2.name);
			setComplent (link2, point);
		}

		GameControl.mLines=new List<Line> ();
		GameControl.nailControl.mPoints.RemoveAt(nailIndex);
		GameControl.nailControl.mColor.RemoveAt (nailIndex);
		GameControl.nailControl.mPoints.Add(GameControl.mLinks);
		GameControl.nailControl.mColor.Add (GameControl.getColor);
//		GameControl.mLinks=new VectorList.LinkList();
		GameControl.mLinks=GameControl.nailControl.mPoints[GameControl.nailControl.mPoints.Count-1];
	}
	public void drawLine(int flag,int index,GameObject point)
	{
		if (flag == 1) 
		{
			if(index==1)
			{
				GameObject link1=GameObject .Find(GameControl.mLinks.get_Listnode(GameControl.mLinks.Length()-1));
				GameObject link2=GameObject.Find(GameControl.mLinks.get_Listnode(2));
				MLine line1 = new MLine (link1,point,link1.transform);
				line1.obj.AddComponent<LinePoints> ().setLinePoint (link1.name, point.name);
				line1.setColor(GameControl.getColor);
				MLine line2 = new MLine (point,link2 ,point.transform);
				line2.obj.AddComponent<LinePoints> ().setLinePoint (point.name,link2.name);
				line2.setColor(GameControl.getColor);
			}
			else
			{
				GameObject link1=GameObject .Find(GameControl.mLinks.get_Listnode(index-1));
				GameObject link2=GameObject.Find(GameControl.mLinks.get_Listnode(index+1));
				MLine line1 = new MLine (link1, point,link1.transform);
				line1.setColor(GameControl.getColor);
				line1.obj.AddComponent<LinePoints> ().setLinePoint (link1.name, point.name);
				MLine line2 = new MLine (point,link2 ,point.transform);
				line2.obj.AddComponent<LinePoints> ().setLinePoint (point.name,link2.name);
				line2.setColor(GameControl.getColor);
			}
		}
		if (flag == 2) 
		{
			if(index==1)
			{
				GameObject link1=GameObject .Find(GameControl.mLinks.get_Listnode(GameControl.mLinks.Length()-1));
				GameObject link2=GameObject.Find(GameControl.mLinks.get_Listnode(2));
				MLine line1 = new MLine (link1, point,link1.transform);
				line1.obj.AddComponent<LinePoints> ().setLinePoint (link1.name, point.name);
				line1.setColor(GameControl.getColor);
				MLine line2 = new MLine (point,link2 ,point.transform);
				line2.obj.AddComponent<LinePoints> ().setLinePoint (point.name,link2.name);
				line2.setColor(GameControl.getColor);
			}
			else
			{
				GameObject link1=GameObject .Find(GameControl.mLinks.get_Listnode(index-1));
				GameObject link2=GameObject.Find(GameControl.mLinks.get_Listnode(index+1));
				MLine line1 = new MLine (link1, point,link1.transform);
				line1.obj.AddComponent<LinePoints> ().setLinePoint (link1.name, point.name);
				line1.setColor(GameControl.getColor);
				MLine line2 = new MLine (point,link2 ,point.transform);
				line2.obj.AddComponent<LinePoints> ().setLinePoint (point.name,link2.name);
				line2.setColor(GameControl.getColor);
			}
		}
	}
	public string getPlaneName(LinkList _points)
	{
		string planeName = "";
		for (int i=1; i<_points.Length(); i++) {
			planeName+=_points.get_Listnode(i);
		}
		return planeName;
	}
	public void setNailing()
	{
		for (int i=0; i<GameObject.FindGameObjectsWithTag("point").Length; i++)
			GameObject.FindGameObjectsWithTag ("point") [i].GetComponent<Renderer> ().material.color = Color.black;
		for (int i=1; i<=GameControl.mLinks.Length(); i++) 
		{
			GameObject.Find(GameControl.mLinks.get_Listnode(i)).GetComponent<Renderer> ().material.color = Color.red;
		}
		if(GameObject.Find("dragObj")!=null)
			GameObject.Find("dragObj").GetComponent<Renderer> ().material .color= Color.red;
	}
	public void setComplent(GameObject _point1,GameObject _point2)
	{
		if(_point1.GetComponent<AddNailPoint>()==null)
			_point1.AddComponent<AddNailPoint> ();
		if(_point2.GetComponent<AddNailPoint>()==null)
			_point2.AddComponent<AddNailPoint> ();
	}
	public void checkColor(string point1,string point2)
	{
		for (int i=0; i<nailControl.mPoints.Count; i++) 
		{
			if(nailControl.mPoints[i]!=mLinks)
			{
				for(int j=1;j<nailControl.mPoints[i].Length();j++)
				{
					if(nailControl.mPoints[i].get_Listnode(j).Equals(point1)&&nailControl.mPoints[i].get_Listnode(j+1).Equals(point2))
					{
						MLine line2 = new MLine (GameObject.Find( point1),GameObject.Find( point2) ,GameObject.Find( point1).transform);
						line2.obj.AddComponent<LinePoints> ().setLinePoint (point1,point2);
						line2.setColor(nailControl.mColor[i]);
					}
				}
			}
			else
			{
				int count=0;
				for(int j=1;j<nailControl.mPoints[i].Length();j++)
				{
					if(nailControl.mPoints[i].get_Listnode(j).Equals(point1)&&nailControl.mPoints[i].get_Listnode(j+1).Equals(point2))
					{
						count++;
						if(count>1)
						{
							MLine line2 = new MLine (GameObject.Find( point1),GameObject.Find( point2) ,GameObject.Find( point1).transform);
							line2.obj.AddComponent<LinePoints> ().setLinePoint (point1,point2);
							line2.setColor(nailControl.mColor[i]);
						}
					}
				}
			}
		}
	}
}
