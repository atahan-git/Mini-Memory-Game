using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomVerticalLayoutGroup : MonoBehaviour {


    public List<ObjectHolder> myObjects = new List<ObjectHolder>();

    public float startHeight = 700;
    public float gravity = 100;

    public void AddNewObject(GameObject myObject) {
	    var obj = new ObjectHolder() {
		    item = myObject.GetComponent<RectTransform>(), 
		    speed = 0
	    };

	    myObject.transform.SetParent(transform);
	    
	    obj.item.transform.localPosition = Vector3.zero;
	    obj.item.transform.localScale = Vector3.one;
	    obj.item.anchoredPosition = new Vector2(0, lastHeight + startHeight);
	    myObjects.Add(obj);
    }

    public void RemoveObject(GameObject myObject) {
	    ObjectHolder myObj = null;
	    for (int i = 0; i < myObjects.Count; i++) {
		    if (myObjects[i].item.gameObject == myObject) {
			    myObj = myObjects[i];
			    break;
		    }
	    }

	    if (myObj != null) {
		    StartCoroutine(RemoveObject(myObj));
	    }
    }

    IEnumerator RemoveObject(ObjectHolder myObject) {
	    var timer = 0.5f;
	    var speed = 0f;
	    while (timer >= 0f) {

		    var pos = myObject.item.anchoredPosition;
		    pos.x -= speed;
		    myObject.item.anchoredPosition = pos;

		    speed += gravity * Time.deltaTime;
		    
		    timer -= Time.deltaTime;
		    yield return null;
	    }

	    myObjects.Remove(myObject);
	    Destroy(myObject.item.gameObject);
    }


    private float lastHeight;
    private void Update() {
	    lastHeight = 0f;
        for (int i = 0; i < myObjects.Count; i++) {
	        var curPos = myObjects[i].item.anchoredPosition;
	        var targetPos = curPos;
	        targetPos.y = lastHeight;
             myObjects[i].item.anchoredPosition = Vector2.MoveTowards(curPos, targetPos, myObjects[i].speed);
             myObjects[i].speed += gravity * Time.deltaTime;

             myObjects[i].speed = Mathf.Clamp(myObjects[i].speed, -300, 300);

             lastHeight = myObjects[i].item.anchoredPosition.y + myObjects[i].item.rect.height;
        }
    }

    [System.Serializable]
    public class ObjectHolder {
	    public RectTransform item;
	    public float speed;
    }
}
