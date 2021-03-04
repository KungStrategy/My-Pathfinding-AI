﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject rallyPoint;
    public GameObject marker;
    Vector3 position;
    Vector3 directionToRallyPoint;
    Vector3 directionOfAim;
    Vector3 obstacle;
    Vector3 directionToObstacle;
    Vector3 decisionPoint;
    float distanceToRallyPoint;
    float distanceToObstacle;
    float speed = 2f;
    float ratioX;
    float ratioY;
    float ratioZ;
    bool beingTouched = false;
    bool pathClear = true;

    void Start() => position = transform.position;

    void Update()
    {
        directionToRallyPoint = rallyPoint.transform.position - transform.position;
        distanceToRallyPoint = Vector3.Distance(rallyPoint.transform.position, transform.position);
      
        if (Input.touchCount > 0)
        {
            if (beingTouched == false)
            {
                ReasignRallyPoint();
                CheckPath();
                beingTouched = true;
            }
        }

        if (Input.touchCount == 0)
        {
            beingTouched = false;
        }

        if (pathClear == true)
        {
            
            ratioX = directionToRallyPoint.x / distanceToRallyPoint;
            ratioY = directionToRallyPoint.y / distanceToRallyPoint;
            ratioZ = directionToRallyPoint.z / distanceToRallyPoint;
            position.x += ratioX * speed * Time.deltaTime;
            position.y += ratioY * speed * Time.deltaTime;
            position.z += ratioZ * speed * Time.deltaTime;
            transform.position = position;
        }
    }

    void ReasignRallyPoint()
    {
        RaycastHit hit;
        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = touch.position;
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 newPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            rallyPoint.transform.position = newPosition;
            Debug.Log("Rally Point: " + hit.point);
        }
    }

    void CheckPath()
    {
        //Debug.Log("path checked");
        directionOfAim = directionToRallyPoint;
        directionOfAim.y += 1f;
        RaycastHit hitObstacle;
        //Ray checkPathRay = new Ray(transform.position, directionToRallyPoint);
        //Debug.DrawRay(transform.position, directionToRallyPoint * distanceToRallyPoint, Color.green);
        if (Physics.Raycast(transform.position, directionOfAim, out hitObstacle, distanceToRallyPoint))
        {
            Debug.Log("made it thru the loop");
            //Debug.Log(hitObstacle.transform.gameObject.tag);
            /*if (hitObstacle.transform.gameObject.tag == "Avoid")
            {
                pathClear = false;
                Debug.Log("Obstacle: " + hitObstacle.point);
                //Debug.Log("Direction to Rally Point: " + directionToRallyPoint);
                //Debug.Log("Direction of Aim: " + directionOfAim);
                obstacle = hitObstacle.point;
                //CalculateDecisionPoint();
            }*/
            
        }
        Debug.Log("Soldier Position: " + transform.position);
        Debug.Log("Direction to Rally Point: " + directionToRallyPoint);
        //Debug.Log("Distance to Rally Point: " + distanceToRallyPoint);
        Debug.Log("Obstacle: " + hitObstacle.point);
    }

    void CalculateDecisionPoint()
    {
        directionToObstacle = obstacle - transform.position;
        distanceToObstacle = Vector3.Distance(obstacle, transform.position);
        float distanceToDecisionPoint = distanceToObstacle - .5f;
        decisionPoint.x = distanceToDecisionPoint * obstacle.x / distanceToObstacle;
        decisionPoint.y = distanceToDecisionPoint * obstacle.y / distanceToObstacle;
        decisionPoint.z = distanceToDecisionPoint * obstacle.z / distanceToObstacle;
        marker.transform.position = decisionPoint;
        Debug.Log("decision point: " + decisionPoint);
    }
}
