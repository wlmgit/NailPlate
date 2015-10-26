using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISingleton : MonoBehaviour {

	#region
	private static UISingleton mUIInstance;

	private int NailCount;

	public GameObject panel_nail;

	public static UISingleton UIInstance
	{
		get
		{
			if(mUIInstance==null)
			{
				mUIInstance=new UISingleton();
			}
			return mUIInstance;
		}
	}
	    
	void Awake()
	{
		mUIInstance = this;	
	}
	#endregion

	public GameObject mNailPanel;
	public GameObject mElashPanel;
	public GameObject mSymmPanel;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void Nail_click()
	{
		mNailPanel.SetActive (true);
		mElashPanel.SetActive (false);
		mSymmPanel.SetActive (false);
	}
	void Elash_click()
	{
		mNailPanel.SetActive (false);
		mElashPanel.SetActive (true);
		mSymmPanel.SetActive (false);
	}
	void Symm_click()
	{
		mNailPanel.SetActive (false);
		mElashPanel.SetActive (false);
		mSymmPanel.SetActive (true);
	}
	void setNail5()
	{
		NailCount = 5;
		panel_nail.SetActive (true);
	}
	void setNail6()
	{
		NailCount = 6;
		panel_nail.SetActive (true);
	}
	void setNail8()
	{
		NailCount = 8;
		panel_nail.SetActive (true);
	}
	void setNail12()
	{
		NailCount = 12;
		panel_nail.SetActive (true);
	}
	void setHorizSymm()
	{
		GameControl.isReflect = true;
//		SymmetricBase mH
		horizSymmetric mHorizSymm = new horizSymmetric ();
		mHorizSymm.drawSymmetric ();
		mHorizSymm.reflectPlane (GameControl.mLinks);
		mHorizSymm._planeColor = GameControl.getColor;
		mHorizSymm.drawRefPlane (mHorizSymm.reflectPos);
	}
	void setVerticalSymm()
	{
		//		SymmetricBase mH
		VerticalSymm mVerticSymm = new VerticalSymm ();
		mVerticSymm.drawSymmetric ();
		mVerticSymm.reflectPlane (GameControl.mLinks);
	}
	void setHoriz_vetric()
	{
		horiz_verticSymm mHoriz_vertric = new horiz_verticSymm ();
		mHoriz_vertric.drawSymmetric ();
	}
	void setLU_RDSymm()
	{
		LU_RDSymm mLU_RD = new LU_RDSymm ();
		mLU_RD.drawSymmetric ();
		mLU_RD.reflectPlane (GameControl.mLinks);
	}
	void setLD_RUSymm()
	{
		LD_RUSymm mLD_RU = new LD_RUSymm ();
		mLD_RU.drawSymmetric ();
		mLD_RU.reflectPlane (GameControl.mLinks);
	}
	void setXSymm()
	{
		XSymm mXSymm = new XSymm ();
		mXSymm.drawSymmetric ();
	}
	void setNail(int nailCount)
	{
		GameControl.GameControlInstance.NailStyle=nailCount;
		GameControl.GameControlInstance.InitNail();
		GameControl.GameControlInstance.set_Nail (nailCount);
		GameControl.GameControlInstance.setParent ();
		GameControl.nailControl.mPoints = new List<VectorList.LinkList> ();
		GameControl.mLinks=new VectorList.LinkList ();
		GameControl.GameControlInstance.updaLine ();
	}
	void removeGameControl(int count)
	{
		int maxCount = GameControl.mLines.Count - 1;
		for (int i=0; i<count; i++) 
		{
			GameControl.mLines.RemoveAt(maxCount);
			maxCount--;
		}
	}
	void btn_qx()
	{
		panel_nail.SetActive (false);
	}
	void changeNail()
	{
		setNail(NailCount);
		panel_nail.SetActive (false);

	}
	void drawPlane()
	{
		for (int i=0; i<GameControl.nailControl.mPoints.Count; i++) {
			if(GameObject.Find(GameControl.GameControlInstance.getPlaneName(GameControl.nailControl.mPoints[i]))==null)
			{
				ScaningLine mScan = new ScaningLine ();
				mScan._color=GameControl.nailControl.mColor[i];
				mScan.getPoint (GameControl.nailControl.mPoints[i]);
				mScan.ScanLinePolygonFill ();
			}
		}
	}
	void reset()
	{
		GameControl.nailControl=new Nails();
		for (int i=1; i<=GameControl.mLinks.Length(); i++) {
			GameObject.Find(GameControl.mLinks.get_Listnode(i)).GetComponent<Renderer>().material.color = Color.black;
			Destroy(GameObject.Find(GameControl.mLinks.get_Listnode(i)).GetComponent<AddNailPoint>());
		}
		GameControl.mLinks = new VectorList.LinkList ();
		GameControl.getColor = new Color ();
		destroryAllRef ();
		destroyAllLine ();
		destroyAllPlane ();
		GameControl.isReflect = false;
	}
	void destroryAllRef()
	{
		for (int i=0; i<GameObject.FindGameObjectsWithTag("reflectObj").Length; i++) {
			Destroy(GameObject.FindGameObjectsWithTag("reflectObj")[i]);
		}
	}
	void destroyAllLine()
	{
		for (int i=0; i<GameObject.FindGameObjectsWithTag("line").Length; i++) {
			Destroy(GameObject.FindGameObjectsWithTag("line")[i]);
		}
	}
	void destroyAllPlane()
	{
		for(int i=0;i<GameObject.FindGameObjectsWithTag("plane").Length;i++)
			Destroy (GameObject.FindGameObjectsWithTag("plane")[i]);
	}
	void btn_cancel()
	{
		trash_menu.SetActive (false);
		save_menu.SetActive (false);
	}
	void btn_draw()
	{
//		Debug.Log ("click");
//		btn_right ();
//		CallOS.callIOSPaint ();
	}
	bool Is_right=false;
	public GameObject right_mune;
	public GameObject trash_menu;
	public GameObject save_menu;
	void btn_trash()
	{
		trash_menu.SetActive(true);
	}
	void btn_save()
	{
		save_menu.SetActive (true);
	}
	void btn_right()
	{
		Hashtable right_args = new Hashtable();
		Hashtable left_args = new Hashtable();
		float move_x;
		if (!Is_right) {
			Is_right=true;
			right_mune.transform.GetComponent<UIWidget>().leftAnchor.Set(1,5253);
			return;
		}
		if (Is_right) 
		{
			Is_right=false;
			right_mune.transform.GetComponent<UIWidget>().leftAnchor.Set(1,4258);
			return;
		}
	}
	public static bool IsMouseOverUI  
	{  
		get  
		{  
			Vector3 mousePostion=Input.mousePosition;  
			//			GameObject hoverobject = UICamera.Raycast(mousePostion, out UICamera.lastHit) ? UICamera.lastHit.collider.gameObject : null;  
			UICamera.hoveredObject = UICamera.Raycast(mousePostion) ? UICamera.lastHit.collider.gameObject : null;
			if (UICamera.hoveredObject  != null)  
			{  
				//				Debug.Log(UICamera.hoveredObject.transform.name);
				return true;  
			}  
			else  
			{  
				return false;  
			}  
		}  
	}
}
