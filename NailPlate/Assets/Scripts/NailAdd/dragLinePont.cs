using UnityEngine;
using System.Collections;

public class dragLinePont : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "point"&&other.name!=transform.parent.GetComponent<dragLine>().link1.name&&other.name!=transform.parent.GetComponent<dragLine>().link2.name) 
		{
//			if(GameControl.GameControlInstance.check(other.gameObject.name))
			transform.parent.GetComponent<dragLine>().points.Add(other.gameObject);
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "point"&&transform.parent.GetComponent<dragLine>().points.Count==2) 
		{
			destroyMidDrag();
			transform.parent.GetComponent<dragLine>().points.Remove(other.gameObject);
		}
	}
	void destroyMidDrag()
	{
		if (GameObject.Find ("line" + transform.parent.GetComponent<dragLine>().link1.name +transform.parent.GetComponent<dragLine>().points[1].name) != null)
			Destroy (GameObject.Find ("line" + transform.parent.GetComponent<dragLine>().link1.name +transform.parent.GetComponent<dragLine>().points[1].name));
		if (GameObject.Find ("line" +transform.parent.GetComponent<dragLine>().points[1].name+transform.parent.GetComponent<dragLine>().link2.name) != null)
			Destroy (GameObject.Find ("line" +transform.parent.GetComponent<dragLine>().points[1].name+ transform.parent.GetComponent<dragLine>().link2.name ));
	}
}
