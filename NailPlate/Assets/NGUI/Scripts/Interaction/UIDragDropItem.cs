//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VectorList;

/// <summary>
/// UIDragDropItem is a base script for your own Drag & Drop operations.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Drag and Drop Item")]
public class UIDragDropItem : MonoBehaviour
{
	public enum Restriction
	{
		None,
		Horizontal,
		Vertical,
		PressAndHold,
	}

	/// <summary>
	/// What kind of restriction is applied to the drag & drop logic before dragging is made possible.
	/// </summary>

	public Restriction restriction = Restriction.None;

	/// <summary>
	/// Whether a copy of the item will be dragged instead of the item itself.
	/// </summary>

	public bool cloneOnDrag = false;

	/// <summary>
	/// How long the user has to press on an item before the drag action activates.
	/// </summary>

	[HideInInspector]
	public float pressAndHoldDelay = 1f;
	[SerializeField]
	public GameObject NailPoints;

#region Common functionality

	protected Transform mTrans;
	protected Transform mParent;
	protected Collider mCollider;
	protected Collider2D mCollider2D;
	protected UIButton mButton;
	protected UIRoot mRoot;
	protected UIGrid mGrid;
	protected UITable mTable;
	protected int mTouchID = int.MinValue;
	protected float mDragStartTime = 0f;
	protected UIDragScrollView mDragScrollView = null;
	protected bool mPressed = false;
	protected bool mDragging = false;

	/// <summary>
	/// Cache the transform.
	/// </summary>

	protected virtual void Start ()
	{
		mTrans = transform;
		mCollider = GetComponent<Collider>();
		mCollider2D = GetComponent<Collider2D>();
		mButton = GetComponent<UIButton>();
		mDragScrollView = GetComponent<UIDragScrollView>();
	}

	/// <summary>
	/// Record the time the item was pressed on.
	/// </summary>

	protected void OnPress (bool isPressed)
	{
		if (isPressed)
		{
			mDragStartTime = RealTime.time + pressAndHoldDelay;
			mPressed = true;
		}
		else mPressed = false;
	}

	/// <summary>
	/// Start the dragging operation after the item was held for a while.
	/// </summary>

	protected virtual void Update ()
	{
		if (restriction == Restriction.PressAndHold)
		{
			if (mPressed && !mDragging && mDragStartTime < RealTime.time)
				StartDragging();
		}
	}

	/// <summary>
	/// Start the dragging operation.
	/// </summary>

	protected void OnDragStart ()
	{
		if (!enabled || mTouchID != int.MinValue) return;

		// If we have a restriction, check to see if its condition has been met first
		if (restriction != Restriction.None)
		{
			if (restriction == Restriction.Horizontal)
			{
				Vector2 delta = UICamera.currentTouch.totalDelta;
				if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y)) return;
			}
			else if (restriction == Restriction.Vertical)
			{
				Vector2 delta = UICamera.currentTouch.totalDelta;
				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) return;
			}
			else if (restriction == Restriction.PressAndHold)
			{
				// Checked in Update instead
				return;
			}
		}
		StartDragging();
	}

	/// <summary>
	/// Start the dragging operation.
	/// </summary>

	protected virtual void StartDragging ()
	{
		if (!mDragging)
		{
			if (cloneOnDrag)
			{
				GameObject clone = NGUITools.AddChild(transform.parent.gameObject, gameObject);
				clone.transform.localPosition = transform.localPosition;
				clone.transform.localRotation = transform.localRotation;
				clone.transform.localScale = transform.localScale;
//				clone.transform.parent=NailPoints.transform;
				UIButtonColor bc = clone.GetComponent<UIButtonColor>();
				if (bc != null) bc.defaultColor = GetComponent<UIButtonColor>().defaultColor;

				UICamera.currentTouch.dragged = clone;

				UIDragDropItem item = clone.GetComponent<UIDragDropItem>();
				item.mDragging = true;
				item.Start();
				item.OnDragDropStart();
			}
			else
			{
				mDragging = true;
				OnDragDropStart();
			}
		}
	}

	/// <summary>
	/// Perform the dragging.
	/// </summary>

	protected void OnDrag (Vector2 delta)
	{
		if (!mDragging || !enabled || mTouchID != UICamera.currentTouchID) return;
		OnDragDropMove(delta * mRoot.pixelSizeAdjustment);
	}

	/// <summary>
	/// Notification sent when the drag event has ended.
	/// </summary>

	protected void OnDragEnd ()
	{
		if (!enabled || mTouchID != UICamera.currentTouchID) return;
		StopDragging(UICamera.hoveredObject);
		getNailPoint ();
	}

	/// <summary>
	/// Drop the dragged item.
	/// </summary>

	public void StopDragging (GameObject go)
	{
		if (mDragging)
		{
			mDragging = false;
			OnDragDropRelease(go);
		}
	}

#endregion

	/// <summary>
	/// Perform any logic related to starting the drag & drop operation.
	/// </summary>

	protected virtual void OnDragDropStart ()
	{
		// Automatically disable the scroll view
		if (mDragScrollView != null) mDragScrollView.enabled = false;

		// Disable the collider so that it doesn't intercept events
		if (mButton != null) mButton.isEnabled = false;
		else if (mCollider != null) mCollider.enabled = false;
		else if (mCollider2D != null) mCollider2D.enabled = false;

		mTouchID = UICamera.currentTouchID;
		mParent = mTrans.parent;
		mRoot = NGUITools.FindInParents<UIRoot>(mParent);
		mGrid = NGUITools.FindInParents<UIGrid>(mParent);
		mTable = NGUITools.FindInParents<UITable>(mParent);

		// Re-parent the item
		if (UIDragDropRoot.root != null)
			mTrans.parent = UIDragDropRoot.root;

		Vector3 pos = mTrans.localPosition;
		pos.z = 0f;
		mTrans.localPosition = pos;

		TweenPosition tp = GetComponent<TweenPosition>();
		if (tp != null) tp.enabled = false;

		SpringPosition sp = GetComponent<SpringPosition>();
		if (sp != null) sp.enabled = false;

		// Notify the widgets that the parent has changed
		NGUITools.MarkParentAsChanged(gameObject);

		if (mTable != null) mTable.repositionNow = true;
		if (mGrid != null) mGrid.repositionNow = true;
	}

	/// <summary>
	/// Adjust the dragged object's position.
	/// </summary>

	protected virtual void OnDragDropMove (Vector2 delta)
	{
		mTrans.localPosition += (Vector3)delta;
	}

	/// <summary>
	/// Drop the item onto the specified object.
	/// </summary>

	protected virtual void OnDragDropRelease (GameObject surface)
	{
		if (!cloneOnDrag)
		{
			mTouchID = int.MinValue;

			// Re-enable the collider
			if (mButton != null) mButton.isEnabled = true;
			else if (mCollider != null) mCollider.enabled = true;
			else if (mCollider2D != null) mCollider2D.enabled = true;

			// Is there a droppable container?
			UIDragDropContainer container = surface ? NGUITools.FindInParents<UIDragDropContainer>(surface) : null;

			if (container != null)
			{
				// Container found -- parent this object to the container
				mTrans.parent = (container.reparentTarget != null) ? container.reparentTarget : container.transform;

				Vector3 pos = mTrans.localPosition;
				pos.z = 0f;
				mTrans.localPosition = pos;
			}
			else
			{
				// No valid container under the mouse -- revert the item's parent
				mTrans.parent = mParent;
			}

			// Update the grid and table references
			mParent = mTrans.parent;
			mGrid = NGUITools.FindInParents<UIGrid>(mParent);
			mTable = NGUITools.FindInParents<UITable>(mParent);

			// Re-enable the drag scroll view script
			if (mDragScrollView != null)
				StartCoroutine(EnableDragScrollView());

			// Notify the widgets that the parent has changed
			NGUITools.MarkParentAsChanged(gameObject);

			if (mTable != null) mTable.repositionNow = true;
			if (mGrid != null) mGrid.repositionNow = true;

			// We're now done
			OnDragDropEnd();
		}
		else NGUITools.Destroy(gameObject);
	}

	/// <summary>
	/// Function called when the object gets reparented after the drop operation finishes.
	/// </summary>

	protected virtual void OnDragDropEnd () { }

	/// <summary>
	/// Re-enable the drag scroll view script at the end of the frame.
	/// Reason: http://www.tasharen.com/forum/index.php?topic=10203.0
	/// </summary>

	protected IEnumerator EnableDragScrollView ()
	{
		yield return new WaitForEndOfFrame();
		if (mDragScrollView != null) mDragScrollView.enabled = true;
	}
	/// <summary>
	/// Gets the nail point.
	/// </summary>
	struct ObjDis
	{
		public float dis;
		public GameObject points;
	}
	protected void getNailPoint()
	{
		float stander=Vector2.Distance(Camera.main.WorldToScreenPoint (GameObject.Find("NailPointL_U").transform.position),Camera.main.WorldToScreenPoint(GameObject.Find("NailPointL_D").transform.position))/GameControl.GameControlInstance.NailStyle;
		List<ObjDis> nailPointDis = new List<ObjDis> ();
		foreach (Transform child in NailPoints.transform) 
		{
			ObjDis point1=new ObjDis();
			point1.dis=Vector2.Distance(Camera.main.WorldToScreenPoint( transform.position),Camera.main.WorldToScreenPoint( child.position));
			point1.points=child.gameObject;
			nailPointDis.Add(point1);
		}
		for(int j=0;j<=nailPointDis.Count-1;j++) 
		{ for (int i=0;i<nailPointDis.Count-j-1;i++) 
			if (nailPointDis [i].dis>nailPointDis [i+1].dis) 
			{ 
				ObjDis temp=nailPointDis [i]; 
				nailPointDis [i]=nailPointDis [i+1]; 
				nailPointDis [i+1]=temp;
			} 
		}
		if (nailPointDis [0].dis <= stander || nailPointDis [1].dis <= stander) {
			setFirstLine(nailPointDis [0].points,nailPointDis [1].points);
		}
	}
	protected void setFirstLine(GameObject _point1,GameObject _point2)
	{
		setColor ();
		MLine line1 = new MLine (_point1,_point2, _point1.transform);
		line1.setColor (GameControl.getColor);
		MLine line2 = new MLine (_point2,_point1, _point2.transform);
		line2.setColor (GameControl.getColor);
		addCompent (_point1);
		addCompent (_point2);
		line1.obj.AddComponent<LinePoints> ().setLinePoint (_point1.name, _point2.name); 
		line2.obj.AddComponent<LinePoints> ().setLinePoint (_point2.name, _point1.name); 
		GameControl.mLines=new List<Line> ();
		GameControl.mLinks=new LinkList();
		GameControl.GameControlInstance.addLinePoint (_point1.name, _point2.name);
		GameControl.GameControlInstance.addLinePoint (_point2.name, _point1.name);
		GameControl.mLinks.Insert (0,_point1.name);
		GameControl.mLinks.Insert (1,_point2.name);
		GameControl.mLinks.Insert (2,_point1.name);
		GameControl.nailControl.mPoints.Add (GameControl.mLinks);
		GameControl.nailControl.mColor.Add (GameControl.getColor);
		GameControl.GameControlInstance.setNailing ();
	}
	protected void addCompent(GameObject obj)
	{
		if(obj.GetComponent<AddNailPoint>()==null)
		obj.AddComponent<AddNailPoint> ();
	}
	public void setColor()
	{
		string targetName = transform.name;
		switch (targetName) 
		{
		case "color1(Clone)":
			GameControl.getColor=Color.red;
			break;
		case "color2(Clone)":
			GameControl.getColor=Color.yellow;
			break;
		}
	}
}
