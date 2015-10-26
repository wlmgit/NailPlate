using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using VectorList;



public class ScaningLine  {

	public List<Vector3> points = new List<Vector3> ();
	public GameObject mPlane = new GameObject ();
	public int SCFcount=0;
	public Color _color;

	public ScaningLine()
	{
	}

	public void ScanLinePolygonFill()
	{
		float ymax = 0;
		float ymin = 0;
		GetPolygonMinMax (out ymax,out ymin);
		this.SCFcount = (int)((ymax - ymin) * 100);
		GameControl.area = 0;
		for (int i=0; i<=SCFcount; i++) {
			getCrossPos(ymin+0.01f*i);
		}
		GameControl.area =GameControl.area/GameControl.EdgeLenth/GameControl.EdgeLenth;
	}
//	public float area;
	public void getCrossPos(float scfLine)
	{
		List<Vector3> crossPoints = new List<Vector3> ();
		for (int i = 0; i < points.Count; i++) 
		{
			Vector3 ps = points [i];
			Vector3 pe = points [(i + 1) % points.Count];
			if(ps.y!=pe.y)
			{
				if(scfLine>=pe.y&&scfLine<ps.y)
					crossPoints.Add(new Vector3(getPosX(ps,pe,scfLine),scfLine,20));
				if(scfLine<pe.y&&scfLine>=ps.y)
					crossPoints.Add(new Vector3(getPosX(ps,pe,scfLine),scfLine,20));
			}
		}
		sortPoints(crossPoints);
		for (int i=0; i<crossPoints.Count-1; i=i+2) 
		{
			setMash(crossPoints[i],crossPoints[i+1]);
		}
//		GameControl.area=GameControl.area/(GameControl.EdgeLenth*GameControl.EdgeLenth);
//		combMesh ();
	}
	public void setMash(Vector3 pos1,Vector3 pos2)
	{
		GameObject obj = new GameObject ("plane");
		obj.transform.position = new Vector3 (0,0,-1);
		obj.transform.parent = mPlane.transform;
		obj.AddComponent<MeshFilter> ();
		obj.AddComponent<MeshRenderer> ();
		MeshFilter meshFilter = obj.GetComponent<MeshFilter> ();
		Mesh mesh = meshFilter.mesh;
		Vector3[] vertices=new Vector3[4];
		int[] triangles=new int[6];
		//三角形三个定点坐标，为了显示清楚忽略Z轴
		vertices[0] = new Vector3(pos1.x,pos1.y-0.005f,pos1.z);
		vertices[1] = new Vector3(pos2.x,pos2.y-0.005f,pos2.z);
		vertices[2] = new Vector3(pos1.x,pos1.y+0.005f,pos1.z);
		vertices[3] = new Vector3(pos2.x,pos2.y+0.005f,pos2.z);
		GameControl.area += (pos1.x - pos2.x) * 0.01f;
		//三角形绘制顶点的数组
		triangles[0] =0;
		triangles[1] =1;
		triangles[2] =2;
		triangles[3] =1;
		triangles[4] =3;
		triangles[5] =2;
		//绘制三角形
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		obj.GetComponent<MeshRenderer>().material=MatTools.getPlaneMat(_color); 
		_color.a = 0.5f;
		obj.GetComponent<MeshRenderer> ().material.color = _color;
	}
	public void drawSCF(Vector3 pos1,Vector3 pos2)
	{

	}
	public void sortPoints(List<Vector3> _crossPoints)
	{
		for(int j=0;j<=_crossPoints.Count-1;j++) 
		{ 
			for (int i=0;i<_crossPoints.Count-1-j;i++) 
			if (_crossPoints[i].x<_crossPoints[i+1].x) 
			{ 
				Vector3 temp=_crossPoints[i]; 
				_crossPoints[i]=_crossPoints[i+1]; 
				_crossPoints[i+1]=temp;
			} 
		} 
	}
	public float getPosX(Vector3 a,Vector3 b,float y)
	{
		float x = 0;
		if (a.x != b.x) 
		{
			float dx = (b.y - a.y) / (b.x - a.x);
			x = (y - a.y + dx * a.x) / dx;
		} 
		else
			x = a.x;
		return x;
	}
	public void GetPolygonMinMax (out float ymax,out float ymin)
	{
		ymax = points [0].y;
		ymin = points [0].y;
		for (int i=0; i<points.Count; i++) 
		{
			if(ymax<points[i].y)
				ymax=points[i].y;
			if(ymin>points[i].y)
				ymin=points[i].y;
		}
	}
	//Vector3s
	public List<Vector3> getPoint(LinkList _points)
	{
		mPlane.name = "";
		points = new List<Vector3> ();
		for (int i=1; i<_points.Length(); i++) 
		{
			points.Add(GameObject.Find(_points.get_Listnode(i)).transform.position);
			mPlane.name+=_points.get_Listnode(i);
		}
		mPlane.AddComponent<MeshFilter> ();
		mPlane.AddComponent<MeshRenderer> ();
		mPlane.transform.position=new Vector3(0,0,-0.5f);
		mPlane.tag = "plane";
		return points;
	}
	public void combMesh()
	{
		//---------------- 先获取材质 -------------------------  
		//获取自身和所有子物体中所有MeshRenderer组件  
		MeshRenderer[] meshRenderers =mPlane.GetComponentsInChildren<MeshRenderer>();    
		//新建材质球数组  
		Material[] mats = new Material[meshRenderers.Length];    
		for (int i = 0; i < meshRenderers.Length; i++) {  
			//生成材质球数组   
			mats[i] = meshRenderers[i].sharedMaterial;     
		}  
		//---------------- 合并 Mesh -------------------------  
		//获取自身和所有子物体中所有MeshFilter组件  
		MeshFilter[] meshFilters =mPlane.GetComponentsInChildren<MeshFilter>();    
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];     
		for(int i = 0; i < meshFilters.Length; i++)
		{  
			combine[i].mesh = meshFilters[i].sharedMesh;  
			//矩阵(Matrix)自身空间坐标的点转换成世界空间坐标的点   
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;  
			meshFilters[i].gameObject.SetActive(false);
		}   
		//为新的整体新建一个mesh  
		mPlane.GetComponent<MeshFilter>().mesh = new Mesh();   
		//合并Mesh. 第二个false参数, 表示并不合并为一个网格, 而是一个子网格列表  
		mPlane.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
		mPlane.gameObject.SetActive(true);  
		//为合并后的新Mesh指定材质 ------------------------------  
		mPlane.GetComponent<MeshRenderer>().material=MatTools.getPlaneMat(_color); 
		_color.a = 0.5f;
		mPlane.GetComponent<MeshRenderer> ().material.color = _color;
	}
}
