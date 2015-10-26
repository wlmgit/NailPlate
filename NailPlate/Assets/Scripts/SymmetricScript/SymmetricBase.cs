using UnityEngine;
using System.Collections;
using VectorList;
using System.Collections.Generic;

public class SymmetricBase{

	public LinkList mPoints;
	public Color _planeColor;
	public Vector3[] trigles=new Vector3[4];

	public SymmetricBase()
	{
		Init ();
	}
//	public SymmetricBase(LinkList point)
//	{
//		setPointStr (point);
//		setPointPos ();
//	}
	public virtual void drawSymmetric()
	{
	}
	public virtual void reflectPlane(LinkList pointStr)
	{
	}
	public void setPointStr(LinkList point)
	{
		mPoints = point;
	}
	public void Init()
	{
//		this.mPoints = _points;
		trigles [0] = GameObject.Find ("NailPointL_U").transform.position;
		trigles [1] = GameObject.Find ("NailPointR_U").transform.position;
		trigles [2] = GameObject.Find ("NailPointL_D").transform.position;
		trigles [3] = GameObject.Find ("NailPointR_D").transform.position;
	}
	public Vector3[] allPointPos;
	public void setPointPos()
	{
		int count = mPoints.Length ();
		allPointPos=new Vector3[count];
		for (int i=0; i<count; i++) {
			allPointPos[i]=GameObject.Find(mPoints.get_Listnode(i+1)).transform.position;
		}
	}
	public void drawRefPlane(Vector3[] refPos)
	{
		ScaningLine mScan = new ScaningLine ();
		mScan.mPlane.name ="refPlane";
		mScan.mPlane.tag = "reflectObj";
		mScan._color = _planeColor;
		mScan.mPlane.AddComponent<MeshFilter> ();
		mScan.mPlane.AddComponent<MeshRenderer> ();
		for (int i=0; i<refPos.Length; i++)
			mScan.points.Add (refPos[i]);
		//				mScan.getPoint (GameControl.nailControl.mPoints[i]);
		mScan.ScanLinePolygonFill ();
	}
}

public class horizSymmetric:SymmetricBase
{
	public Vector3 point1=new Vector3();
	public Vector3 point2=new Vector3();
	public horizSymmetric()
	{
		point1 = new Vector3 (trigles[0].x+(trigles[1].x-trigles[0].x)/2,trigles[0].y,trigles[0].z);
		point2 = new Vector3 (trigles[2].x+(trigles[3].x-trigles[2].x)/2,trigles[2].y,trigles[2].z);
	}
	public override void drawSymmetric ()
	{
		GameObject horizSymm = new GameObject ("horizSymm");
		horizSymm.tag = "reflectObj";
		float dy = (point1.y - point2.y) / 8;
		for (int i=0; i<8; i++) 
		{
			Vector3 ps=new Vector3(point1.x,point2.y+dy*i,point1.z);
			Vector3 pe=new Vector3(point1.x,point2.y+dy*i+dy-dy/7,point1.z);
			MLine line = new MLine (ps,pe,horizSymm.transform,"horiz");
			line.obj.tag="symLine";
			GameObject.Destroy(line.obj.GetComponent<dragLine>());
		}
	}
	public Vector3[] reflectPos;
	public override void reflectPlane (LinkList pointStr)
	{
		reflectPos=new Vector3[pointStr.Length()];
		base.setPointStr(pointStr);
		base.setPointPos ();
		for (int i=0; i<allPointPos.Length; i++) 
		{
			float x=allPointPos[i].x+(point1.x-allPointPos[i].x)*2;
			reflectPos[i]=new Vector3(x,allPointPos[i].y,allPointPos[i].z);
		}
		GameObject refPlane = new GameObject ("horizPlane");
		refPlane.tag = "reflectObj";
		for (int i=0; i<reflectPos.Length-1; i++) 
		{
			MLine line = new MLine (reflectPos[i],reflectPos[i+1],refPlane.transform,"reflectline");
			GameObject.Destroy(line.obj.GetComponent<dragLine>());
			line.obj.tag="refLine";
		}
	}
}
public class VerticalSymm:SymmetricBase
{
	public Vector3 point1=new Vector3();
	public Vector3 point2=new Vector3();
	public VerticalSymm()
	{
		point1 = new Vector3 (trigles[0].x,trigles[2].y+(trigles[0].y-trigles[2].y)/2,trigles[0].z);
		point2 = new Vector3 (trigles[1].x,trigles[2].y+(trigles[0].y-trigles[2].y)/2,trigles[2].z);
		
	}
	public override void drawSymmetric ()
	{
		GameObject horizSymm= new GameObject ("verticalSymm");
		float dx = (point2.x - point1.x) / 8;
		for (int i=0; i<8; i++) 
		{
			Vector3 ps=new Vector3(point1.x+dx*i,point2.y,point1.z);
			Vector3 pe=new Vector3(point1.x+dx*i+dx-dx/7,point2.y,point1.z);
			MLine line = new MLine (ps,pe,horizSymm.transform,"vertical");
			GameObject.Destroy(line.obj.GetComponent<dragLine>());
		}
	}
	public Vector3[] reflectPos;
	public override void reflectPlane (LinkList pointStr)
	{
		reflectPos=new Vector3[pointStr.Length()];
		base.setPointStr(pointStr);
		base.setPointPos ();
		for (int i=0; i<allPointPos.Length; i++) 
		{
			float y=allPointPos[i].y+(point1.y-allPointPos[i].y)*2;
			reflectPos[i]=new Vector3(allPointPos[i].x,y,allPointPos[i].z);
		}
		GameObject refPlane = new GameObject ("reflectPlane");
		for (int i=0; i<reflectPos.Length-1; i++) 
		{
			MLine line = new MLine (reflectPos[i],reflectPos[i+1],refPlane.transform,"reflectline");
			GameObject.Destroy(line.obj.GetComponent<dragLine>());
		}
	}
}
public class horiz_verticSymm:SymmetricBase
{
	public override void drawSymmetric ()
	{
		horizSymmetric mHorizSymm = new horizSymmetric ();
		mHorizSymm.drawSymmetric ();
		VerticalSymm mVerticSymm = new VerticalSymm ();
		mVerticSymm.drawSymmetric ();
	}
}
public class LU_RDSymm:SymmetricBase
{
	public Vector3 point1=new Vector3();
	public Vector3 point2=new Vector3();
	public LU_RDSymm()
	{
		point1 = trigles [0];
		point2 = trigles [3];
	}
	public override void drawSymmetric()
	{
		GameObject horizSymm= new GameObject ("LU_RDSymm");
		float dx = (point2.x - point1.x) / 12;
		for (int i=0; i<12; i++) 
		{
			Vector3 ps=new Vector3(trigles[2].x+dx*i,trigles[1].y-dx*i,point1.z);
			Vector3 pe=new Vector3(trigles[2].x+dx*i+dx-dx/10,trigles[1].y-dx*i-dx+dx/10,point1.z);
			MLine line = new MLine (ps,pe,horizSymm.transform,"LU_RD");
			GameObject.Destroy(line.obj.GetComponent<dragLine>());
		}
	}
	public Vector3[] reflectPos;
	public override void reflectPlane (LinkList pointStr)
	{
		reflectPos=new Vector3[pointStr.Length()];
		base.setPointStr(pointStr);
		base.setPointPos ();
		for (int i=0; i<allPointPos.Length; i++) 
		{
			float x=point1.y+point1.x-allPointPos[i].y;
			float y=point1.y+point1.x-allPointPos[i].x;
			reflectPos[i]=new Vector3(x,y,allPointPos[i].z);
		}
		GameObject refPlane = new GameObject ("reflectPlane");
		for (int i=0; i<reflectPos.Length-1; i++) 
		{
			MLine line = new MLine (reflectPos[i],reflectPos[i+1],refPlane.transform,"reflectline");
			GameObject.Destroy(line.obj.GetComponent<dragLine>());
		}
	}
}
public class LD_RUSymm:SymmetricBase
{
	public Vector3 point1=new Vector3();
	public Vector3 point2=new Vector3();
	public LD_RUSymm()
	{
		point1 = trigles [2];
		point2 = trigles [1];
	}
	public override void drawSymmetric()
	{
		GameObject horizSymm= new GameObject ("LD_RUSymm");
		float dx = (point2.x - point1.x) / 12;
		for (int i=0; i<12; i++) 
		{
			Vector3 ps=new Vector3(trigles[0].x+dx*i,trigles[3].y+dx*i,point1.z);
			Vector3 pe=new Vector3(trigles[0].x+dx*i+dx-dx/10,trigles[3].y+dx*i+dx-dx/10,point1.z);
			MLine line = new MLine (ps,pe,horizSymm.transform,"LD_RU");
			GameObject.Destroy(line.obj.GetComponent<dragLine>());
		}
	}
	public Vector3[] reflectPos;
	public override void reflectPlane (LinkList pointStr)
	{
		reflectPos=new Vector3[pointStr.Length()];
		base.setPointStr(pointStr);
		base.setPointPos ();
		for (int i=0; i<allPointPos.Length; i++) 
		{
			float x=allPointPos[i].y-point1.y+point1.x;
			float y=point1.y-point1.x+allPointPos[i].x;
			reflectPos[i]=new Vector3(x,y,allPointPos[i].z);
		}
		GameObject refPlane = new GameObject ("reflectPlane");
		for (int i=0; i<reflectPos.Length-1; i++) 
		{
			MLine line = new MLine (reflectPos[i],reflectPos[i+1],refPlane.transform,"reflectline");
			GameObject.Destroy(line.obj.GetComponent<dragLine>());
		}
	}
}
public class XSymm:SymmetricBase
{
	public override void drawSymmetric ()
	{
		LU_RDSymm mLU_RDSymm = new LU_RDSymm();
		mLU_RDSymm.drawSymmetric ();
		LD_RUSymm mLD_RUSymm = new LD_RUSymm();
		mLD_RUSymm.drawSymmetric ();
	}
}
