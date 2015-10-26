using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using VectorList;

public class AddNailPoint : MonoBehaviour {
	private static float time1=0f;
	private Vector3 _vec3TargetScreenSpace;// 目标物体的屏幕空间坐标  
	private Vector3 _vec3TargetWorldSpace;// 目标物体的世界空间坐标  
	private Transform _trans;// 目标物体的空间变换组件  
	private Vector3 _vec3MouseScreenSpace;// 鼠标的屏幕空间坐标  
	private Vector3 _vec3Offset;// 偏移 
    //	private bool linkLine;

	public bool Is_drag;
	// Use this for initialization

	void Start () 
	{
		Is_drag = false;
	}
	void Awake( ) { _trans = transform; }   
	// Update is called once per frame
	void Update () 
	{
		GameControl.GameControlInstance.setNailPosition ();
		drawChecked(); 
		if(hasPlane&&Is_drag)
		{
			Destroy (GameObject.Find(GameControl.GameControlInstance.getPlaneName(GameControl.mLinks)));
			Destroy (GameObject.Find(GameControl.GameControlInstance.getPlaneName(changeDragPoint(DragNailName))));
		}
		if (GameControl.isReflect && Is_drag) {
			GameControl.GameControlInstance.drawReflect (changeDragPoint(DragNailName),false);
		}
	}
	LinkList changeDragPoint(string point)
	{
		int count = 0;
		LinkList newList = new LinkList ();
		for(int i=0;i<GameControl.mLinks.Length();i++)
		newList.Insert(i,GameControl.mLinks.get_Listnode(i+1));
		for (int i=1; i<=newList.Length(); i++) {
			if(newList.get_Listnode(i).Equals(point)&&count==0)
			{
				newList.Delete(i);
				newList.Insert(i-1,"dragObj");
				count=1;
			}
		}
		if (newList.get_Listnode (1).Equals ("dragObj")) {
			newList.Delete (newList.Length());
			newList.Insert(newList.Length(),"dragObj");
		}
			
		return newList;
	}
	void OnMouseUp()
	{
		if (!isdoubleClick) 
		{
			DestroyDrag ();
		}
		else 
		{
			isdoubleClick = false;
		}
		GameControl.GameControlInstance.updaLine ();
	}
	void DoubleClick(string point)
	{
		if (hasPlane)
			Destroy (GameObject.Find(GameControl.GameControlInstance.getPlaneName(GameControl.mLinks)));
		GameControl.GameControlInstance.PointgetNailIndex (point);
		int pointIndex = GameControl.mLinks.get_Index (point);
		if (GameControl.mLinks.Length () > 3)
		{
			if(pointIndex==1)
			{
					Destroy(GameObject.Find("line"+point+GameControl.mLinks.get_Listnode(2)));
					Destroy(GameObject.Find("line"+GameControl.mLinks.get_Listnode(GameControl.mLinks.Length()-1)+point));
					GameControl.nailControl.mPoints.Remove(GameControl.mLinks);
					GameControl.mLinks.Delete(pointIndex);
					GameControl.mLinks.Delete(GameControl.mLinks.Length());
					GameControl.mLinks.Insert(GameControl.mLinks.Length(),GameControl.mLinks.get_Listnode(1));
					string point1=GameControl.mLinks.get_Listnode(GameControl.mLinks.Length()-1);
					string point2=GameControl.mLinks.get_Listnode(GameControl.mLinks.Length());
					MLine line=new MLine (GameObject.Find(point1),GameObject.Find(point2),GameObject.Find(point1).transform);
					line.obj.AddComponent<LinePoints>().setLinePoint(point1,point2);
					GameControl.nailControl.mPoints.Add(GameControl.mLinks);
			}
			else
			{
				string str="line"+point+GameControl.mLinks.get_Listnode(pointIndex+1);
				Destroy(GameObject.Find("line"+point+GameControl.mLinks.get_Listnode(pointIndex+1)));
				Destroy(GameObject.Find("line"+GameControl.mLinks.get_Listnode(pointIndex-1)+point));
				GameControl.nailControl.mPoints.Remove(GameControl.mLinks);
				GameControl.mLinks.Delete(pointIndex);
				GameControl.nailControl.mPoints.Add(GameControl.mLinks);
			}
			if(!GameControl.GameControlInstance.IsExist(GameObject.Find( point)))
				Destroy(GameObject.Find(point).GetComponent<AddNailPoint>());
		}
		if(hasPlane)
		{
			ScaningLine mScan = new ScaningLine ();
			mScan._color=GameControl.getColor;
			mScan.getPoint (GameControl.mLinks);
			mScan.ScanLinePolygonFill ();
		}
		if (GameControl.isReflect) {
			GameControl.GameControlInstance.drawReflect (GameControl.mLinks,true);
		}
		GameControl.GameControlInstance.updaLine ();
		GameControl.GameControlInstance.setNailing ();
	}
	static int count=0;
	bool isdoubleClick=false;
	void OnMouseDown()
	{
		if (!UISingleton.IsMouseOverUI) {
			if(count==0)
				time1 = Time.time;
			count++;
			if (count == 2) 
			{
				hasPlane=GameControl.GameControlInstance.checkPlane();
				if(Time.time-time1<0.5f)
				{
					DoubleClick(transform.name);
					isdoubleClick=true;
				}
				else
					StartCoroutine ("setDrag");
				count=0;
				time1=0; 
			}
			
			else
				StartCoroutine ("setDrag");
		}
	}

    IEnumerator setDrag()   	
	{  
		setObjDrag ();
		if (Is_drag) 
		{
			creatDragObj ();  
			_vec3TargetScreenSpace = Camera.main.WorldToScreenPoint (_trans.position); 
			_vec3MouseScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);  
			_vec3Offset = _trans.position - Camera.main.ScreenToWorldPoint (_vec3MouseScreenSpace);  
			while (Input.GetMouseButton(0)) 
			{  
				_vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y,_vec3TargetScreenSpace.z);  
				_vec3TargetWorldSpace = Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace ) + _vec3Offset;                
				_trans.position = new Vector3(_vec3TargetWorldSpace.x,_vec3TargetWorldSpace.y,_trans.position.z); 
				yield return new WaitForFixedUpdate(); 
			}   
			}
			yield return null;
	}

    public string DragNailName;
    public bool isLink=false;
    public int getNailIndex;
    public int getPointIndex;
	public bool hasPlane=false;
void setObjDrag()
{
	GameControl.temp1 = 0;
	GameControl.temp2 = 0;
	Ray ray=Camera.main.ScreenPointToRay (Input.mousePosition);
	RaycastHit mHit;
	if(Physics.Raycast (ray, out mHit) && mHit.transform.parent==GameControl.GameControlInstance.objParent)
	{
		DragNailName=mHit.transform.name;
		_trans=mHit.transform;
		if(!Is_drag)
		{
			getNailIndex=GameControl.GameControlInstance.PointgetNailIndex(_trans.gameObject.name);
//			GameControl.getColor=GameControl.nailControl.mColor[getNailIndex];
			getPointIndex=GameControl.mLinks.get_Index(_trans.gameObject.name);
			GameControl.GameControlInstance.setNailing ();
		}
		Is_drag=true;
	}
	hasPlane =GameControl.GameControlInstance.checkPlane ();
	changeParent (_trans);
}
	void changeParent(Transform target)
	{
		if (target.childCount > 0)
		{
			for(int i=0;i<target.childCount;i++)
			{
				if(!Existcharge(target.GetChild(i).GetComponent<LinePoints>().point2))
				{
					target.GetChild(i).parent=GameObject.Find(target.GetChild(i).GetComponent<LinePoints>().point2).transform;
				}
			}
		}
	}
	bool Existcharge(string point)
	{
		bool charge = false;
		for (int i=0; i<pointLinks.Count; i++) 
		{
			if(pointLinks[i].Equals(point))
				charge=true;
		}
		return charge;
	}
	void creatDragObj()
	{
		Instantiate (GameControl.GameControlInstance.NailPoint,_trans.position, Quaternion.identity);
		GameObject.Find("NailPoint(Clone)").transform.parent=GameControl.GameControlInstance.objParent;
		GameObject.Find("NailPoint(Clone)").transform.name=_trans.name;
		string temp = _trans.name;
		_trans.name="dragObj";
		_trans.gameObject.AddComponent <Rigidbody> ().useGravity = false;
		_trans.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		if (GameControl.GameControlInstance.check(temp)>1) 
		{
			GameObject.Find(temp).GetComponent<Renderer>().material.color=Color.red;
		}
	}
	void DestroyDrag()
	{
		if(hasPlane)
		Destroy (GameObject.Find(GameControl.GameControlInstance.getPlaneName(changeDragPoint(DragNailName))));
		//fuyuan
		if (points.Count == 1){
			points[0].GetComponent<Renderer>().material.color=Color.red;
			GameControl.GameControlInstance.updateLines(points[0].name ,points[0],getNailIndex);
		}
		//add links
		if (points.Count == 2) 
		{
			if(points[1].GetComponent<AddNailPoint>()==null)
			points[1].AddComponent<AddNailPoint>();
			points [1].GetComponent<Renderer> ().material.color = Color.red;
			changeLinkPoint();
		}
		if (hasPlane) 
		{
			ScaningLine mScan = new ScaningLine ();
			mScan._color=GameControl.getColor;
			mScan.getPoint (GameControl.mLinks);
			mScan.ScanLinePolygonFill ();
		}
		if (GameControl.isReflect) {
			GameControl.GameControlInstance.drawReflect (GameControl.mLinks,true);
		}
		points=new List<GameObject> ();
		lineIndexs=new List<int>();
		pointLinks=new List<string>();
		Destroy(GameObject.Find("dragObj"));
	}
	public List<GameObject> points=new List<GameObject>();
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.transform.parent == GameControl.GameControlInstance.objParent && other.gameObject.name != "dragObj"&&other.tag!="line") 
		{
			if(points.Count==1&&points[0].name!=other.gameObject.name)
			{
				points.Add(other.gameObject);
			}
			if(points.Count==2)
			{
				points.Remove(points[1]);
				points.Add(other.gameObject);
			}
			if(points.Count==0)
			{
				points.Add(other.gameObject);
			}
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.transform.parent == GameControl.GameControlInstance.objParent && other.name != "dragObj") 
		{
			if(points.Count==2)
			{
				GameControl.temp1=0;
				GameControl.temp2=0;
			    GameControl.GameControlInstance.destroyTemp(points[0].name,points[1].name);
				points.Remove(points[1]);
			}
		}
	}

    private static List<string> pointLinks=new List<string>();
	private List<int> lineIndexs=new List<int>();
	bool checkPoint()
	{
		pointLinks=new List<string> ();
		lineIndexs=new List<int> ();
		bool Ischecked = false;
		if (GameControl.GameControlInstance.getLinePoint () > 0) {
			for (int i=0; i<GameControl.GameControlInstance.getLinePoint(); i++) 
			{
				if (transform.name == GameControl.GameControlInstance.getLine (i).Lpoint1)
				{
					pointLinks.Add(GameControl.GameControlInstance.getLine (i).Lpoint2);
					lineIndexs.Add(i);
					Ischecked= true;
				}
				if (transform.name == GameControl.GameControlInstance.getLine (i).Lpoint2)
				{
					pointLinks.Add(GameControl.GameControlInstance.getLine (i).Lpoint1);
					lineIndexs.Add(i);
					Ischecked= true;
				}
			}
		} 
		return Ischecked;
	}

	void drawChecked()
	{
		destroyLine ();
		//nedd check
		if (points.Count == 1) 
		{
			GameControl.GameControlInstance.drawLine (1, getPointIndex,GameObject.Find("dragObj"));
		} 
		else 
		{
			if(points.Count==2)
			{
				GameControl.GameControlInstance.drawLine(2,getPointIndex,GameObject.Find("dragObj"));
			}
		}
	}
	void changeLinkPoint()
	{
		for (int i=0; i<GameControl.nailControl.mPoints.Count; i++) 
		{
			for(int j=1;j<=GameControl.nailControl.mPoints[i].Length();j++)
			{
				if(GameControl.nailControl.mPoints[i].get_Listnode(j).Equals(points[0].name))
				{
					if(points[0].GetComponent<AddNailPoint>()==null)
					points[0].AddComponent<AddNailPoint>();
				}
			}
		}
//		GameControl.nailControl.mPoints.Remove (GameControl.mLinks);
		GameControl.nailControl.mPoints.RemoveAt (getNailIndex);
		GameControl.nailControl.mColor.RemoveAt(getNailIndex);
		if (getPointIndex == 1) 
		{
			GameControl.mLinks.Delete (1);
			GameControl.mLinks.Delete (GameControl.mLinks.Length ());
			GameControl.mLinks.Insert (0, points [1].name);
			GameControl.mLinks.Insert (GameControl.mLinks.Length (), points [1].name);
		} 
		else 
		{
			GameControl.mLinks.Delete (getPointIndex);
			GameControl.mLinks.Insert (getPointIndex-1, points [1].name);
		}
		GameControl.nailControl.mPoints.Add(GameControl.mLinks);
		GameControl.nailControl.mColor.Add(GameControl.getColor);
//		GameControl.mLinks = new VectorList.LinkList ();
		GameControl.mLinks = GameControl.nailControl.mPoints[GameControl.nailControl.mPoints.Count-1];
		GameControl.mLines = new List<Line> ();
		if (!GameControl.GameControlInstance.IsExist (points [0]))
		{
			Destroy(points[0].GetComponent<AddNailPoint>());
		}

	}
	void setTwoLine(GameObject _point1,GameObject _point2)
	{
		Destroy (GameObject.Find ("line" + _point1.name));
		Destroy (GameObject.Find ("line" + _point2.name));
		MLine line1 = new MLine (points[0], points[1],_point1.transform);
		MLine line2 = new MLine (points[0], points[1],_point2.transform);
	}
	bool checkLine(GameObject _line,GameObject _point)
	{
		if (_line.GetComponent<LinePoints> ().point1.Equals(_point.name) || _line.GetComponent<LinePoints> ().point2.Equals( _point.name))
			return true;
		else
			return false;
	}
	void destroyLine()
	{
		if (points.Count == 1)
		{
			GameControl.GameControlInstance.destroyTemp(points[0].name,points[0].name);
		}
		if (points.Count == 2) 
		{
//			GameControl.GameControlInstance.destroyTemp(points[0].name,points[0].name);
//			if(GameControl.GameControlInstance.checkNext(po))
			GameControl.GameControlInstance.destroyTemp(points[0].name,points[1].name);
			GameControl.temp1=0;
			GameControl.temp2=0;
		}
	}
	void addDragLine(GameObject point1,GameObject point2)
	{
		GameObject dragLine = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
	}
}
