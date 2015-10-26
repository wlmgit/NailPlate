using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VectorList;

public class dragLine : MonoBehaviour {

	private Vector3 _vec3TargetScreenSpace;// 目标物体的屏幕空间坐标  
	private Vector3 _vec3TargetWorldSpace;// 目标物体的世界空间坐标  
	private Transform _trans;// 目标物体的空间变换组件  
	private Vector3 _vec3MouseScreenSpace;// 鼠标的屏幕空间坐标  
	private Vector3 _vec3Offset;// 偏移 
	
	public bool Is_drag;
	// Use this for initialization
	public GameObject link1;
	public GameObject link2;
	public GameObject line;
	private bool isFirstLine;
	void Start () 
	{
		Is_drag = false;
	}
	void Awake( ) 
	{ 
		_trans = transform; 
	}   
	// Update is called once per frame
	void Update () 
	{
		drawDragLine ();
		if (hasPlane&Is_drag)
		{
			Destroy (GameObject.Find(GameControl.GameControlInstance.getPlaneName(GameControl.mLinks)));
			Destroy (GameObject.Find(GameControl.GameControlInstance.getPlaneName(changeDragPoint(DragNailName))));
//			ScaningLine mScan = new ScaningLine ();
//			mScan._color=GameControl.nailControl.mColor[getNailIndex];
//			mScan.getPoint (changeDragPoint(DragNailName));
//			mScan.ScanLinePolygonFill ();
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
		for (int i=1; i<newList.Length(); i++) {
			if(newList.get_Listnode(i).Equals(GameObject.Find(point).GetComponent<LinePoints>().point1)&&newList.get_Listnode(i+1).Equals(GameObject.Find(point).GetComponent<LinePoints>().point2)&&count==0)
			{
				newList.Insert(i,"dragObj");
				count++;
			}
		}
		return newList;
	}
//	void OnMouseEnter () {
//		if (GameObject.Find ("dragObj") == null) {
////			GetComponent<LineRenderer> ().material.color = Color.green;//当鼠标滑过时改变物体颜色为蓝色 
//			setObjlineDrag();
//		}
//	}
	
	void OnMouseExit () 
	{
	}
	void OnMouseUp()
	{
		DestroylineDrag ();
		temp = 0;
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
//		GameControl.GameControlInstance.updaLine ();
	}
	IEnumerator OnMouseDown()   	
	{ 
		if (!UISingleton.IsMouseOverUI) {
			setObjlineDrag ();
			if (Is_drag) 
			{
				creatDraglineObj();  
				_vec3TargetScreenSpace = Camera.main.WorldToScreenPoint(_trans.position);     
				_vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);   
				_vec3Offset = _trans.position - Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace);  	  
				while ( Input.GetMouseButton(0) ) {  
					// 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标）  
					_vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y,_vec3TargetScreenSpace.z);  
					// 把鼠标的屏幕空间坐标转换到世界空间坐标（Z值使用目标物体的屏幕空间坐标），加上偏移量，以此作为目标物体的世界空间坐
					_vec3TargetWorldSpace = Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace ) + _vec3Offset;             	
					// 更新目标物体的世界空间坐标   
					_trans.position = new Vector3(_vec3TargetWorldSpace.x,_vec3TargetWorldSpace.y,_trans.position.z); 
					// 等待固定更新
					yield return new WaitForFixedUpdate(); 
				}   
			}
		}
		yield return null;
	}  
	public string DragNailName;
	public bool isLink=false;
	bool hasPlane=false;
	public int getNailIndex;
	void setObjlineDrag()
	{
		Ray ray=Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit mHit;
		if(Physics.Raycast (ray, out mHit) && mHit.transform.parent.parent==GameControl.GameControlInstance.objParent.parent)
		{
			DragNailName=mHit.transform.name;
			_trans=mHit.transform;
			if(!Is_drag)
			{
			  getNailIndex=GameControl.GameControlInstance.PointgetNailIndex(_trans.name);
				GameControl.GameControlInstance.setNailing ();
			}
			Is_drag=true;
		}
		hasPlane = GameControl.GameControlInstance.checkPlane ();
		isFirstLine = transform.GetComponent<LinePoints> ().IsFirstLine;
	}


	void creatDraglineObj()
	{
		Instantiate (GameControl.GameControlInstance.NailPoint,new Vector3 (Camera.main.ScreenToWorldPoint(Input.mousePosition).x,Camera.main.ScreenToWorldPoint(Input.mousePosition).y,19), Quaternion.identity);
		GameObject.Find("NailPoint(Clone)").transform.parent=GameControl.GameControlInstance.objParent;
		GameObject.Find("NailPoint(Clone)").transform.name="dragObj";
		link1 =GameObject.Find( _trans.GetComponent<LinePoints> ().point1);
		link2 =GameObject.Find( _trans.GetComponent<LinePoints> ().point2);
		line = _trans.gameObject;
		_trans.GetComponent<LineRenderer> ().enabled = false;
		GameObject.Find ("dragObj").transform.parent = _trans;
		_trans = GameObject.Find ("dragObj").transform;
		_trans.tag = "dragpoint";
	
		_trans.gameObject.AddComponent<dragLinePont> ();
//		_trans.GetComponent<AddNailPoint> ().enabled = false;
		points.Add (_trans.gameObject);
//		_trans.gameObject.SetActive (false);
//		_trans.name="dragObj";
		_trans.gameObject.AddComponent <Rigidbody> ().useGravity = false;
		_trans.gameObject.GetComponent <Rigidbody> ().isKinematic = true;
	}
	void DestroylineDrag()
	{
		if(hasPlane)
			Destroy (GameObject.Find(GameControl.GameControlInstance.getPlaneName(changeDragPoint(DragNailName))));
		Is_drag = false;
		//fuyuan
		if (points.Count == 1)
		{
			destroyDragLine();
			MLine lineOri=new MLine(link1,link2,link1.transform);
			lineOri.setColor(GameControl.getColor);
			lineOri.obj.AddComponent<LinePoints> ().setLinePoint (link1.name,link2.name);
		}
		//add links
		if (points.Count == 2) 
		{
			points[1].GetComponent<Renderer>().material.color=Color.red;
			if(points[1].GetComponent<AddNailPoint>()==null)
			points[1].AddComponent<AddNailPoint>();
//			GameControl.nailControl.mPoints.Remove(GameControl.mLinks);
////			GameControl.nailControl.mColor.Remove(GameControl.getColor);
			GameControl.nailControl.mPoints.RemoveAt(getNailIndex);
			GameControl.nailControl.mColor.RemoveAt(getNailIndex);
				for(int i=1;i<GameControl.mLinks.Length();i++)
				{
					if(GameControl.mLinks.get_Listnode(i).Equals(link1.name)&&GameControl.mLinks.get_Listnode(i+1).Equals(link2.name))
					{
						GameControl.mLinks.Insert(i,points[1].name);
					}
				}
			GameControl.nailControl.mPoints.Add(GameControl.mLinks);
			GameControl.nailControl.mColor.Add(GameControl.getColor);
		}
		GameControl.GameControlInstance.setNailing ();
		lineIndexs.Clear ();
		pointLinks.Clear ();
		Destroy(GameObject.Find("dragObj"));
		Destroy (line);
		points.Clear ();
//		GameControl.mLinks = new VectorList.LinkList ();
		GameControl.mLinks = GameControl.nailControl.mPoints[GameControl.nailControl.mPoints.Count-1];
	}
	public List<GameObject> points=new List<GameObject>();

	public void drawDragLine()
	{
		destroyDragLine ();
		if (points.Count == 1) 
		{
			MLine line1 = new MLine (link1, points[0], link1.transform);
			line1.setColor(GameControl.getColor);
			MLine line2 = new MLine ( points[0],link2, points[0].transform);
			line2.setColor(GameControl.getColor);
		}
		if (points.Count == 2) 
		{
			MLine line1 = new MLine (link1, points[1], link1.transform);
			line1.setColor(GameControl.getColor);
			line1.obj.AddComponent<LinePoints> ().setLinePoint (link1.name, points [1].name);
			MLine line2 = new MLine (points[1],link2, link2.transform);
			line2.setColor(GameControl.getColor);
			line2.obj.AddComponent<LinePoints> ().setLinePoint (points [1].name,link2.name );
		}
		temp1 = 0;
		temp2 = 0;
	}
	private static List<GameObject> pointLinks=new List<GameObject>();
	private List<int> lineIndexs=new List<int>();
	
	int temp1=0;
	int temp2=0;
	int temp=0;
	void destroyDragLine()
	{
			if (points.Count == 1) {
				if (GameObject.Find ("line" + link1.name +points[0].name) != null)
				Destroy (GameObject.Find ("line" + link1.name +points[0].name));
			    if (GameObject.Find ("line"+points[0].name+link2.name) != null)
				Destroy (GameObject.Find ("line" +points[0].name+ link2.name ));
//			GameControl.nailControl.mPoints.Remove(GameControl.mLinks);
//			GameControl.nailControl.mPoints.Add(GameControl.mLinks);
			}
			if (points.Count == 2) {
			if (GameObject.Find ("line" + link1.name +points[0].name) != null)
				Destroy (GameObject.Find ("line" + link1.name +points[0].name));
			if (GameObject.Find ("line"  +points[0].name+ link2.name) != null)
				Destroy (GameObject.Find ("line" +points[0].name+ link2.name ));

			if (GameObject.Find ("line" + link1.name +points[1].name) != null&&temp1==0&&temp>0)
			{
				Destroy (GameObject.Find ("line" + link1.name +points[1].name));
				temp1=1;
			}
			if (GameObject.Find ("line" +points[1].name+ link2.name ) != null&&temp2==0&&temp>0)
			{
				Destroy (GameObject.Find ("line"  +points[1].name+ link2.name));
				temp2=1;
			}
			temp=1;
			}
	}
}
