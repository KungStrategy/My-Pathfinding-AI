using System.Collections;
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
    bool pathClear = true;

    void Start() => position = transform.position;

    void Update()
    {
        directionToRallyPoint = rallyPoint.transform.position - transform.position;
        distanceToRallyPoint = Vector3.Distance(rallyPoint.transform.position, transform.position);

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ReasignRallyPoint();
                CheckPath();
            }
        }

        if (pathClear == true)
        {
            
            ratioX = directionToRallyPoint.x / distanceToRallyPoint;
            ratioY = directionToRallyPoint.y / distanceToRallyPoint;
            ratioZ = directionToRallyPoint.z / distanceToRallyPoint;
            position.x += ratioX * speed * Time.deltaTime;
            position.y = 1.5f;
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
            //Debug.Log("Rally Point: " + hit.point);
        }
    }

    void CheckPath()
    {
        directionToRallyPoint = rallyPoint.transform.position - transform.position;
        distanceToRallyPoint = Vector3.Distance(rallyPoint.transform.position, transform.position);
        directionOfAim = directionToRallyPoint;
        directionOfAim.y += 1f;
        RaycastHit hitObstacle;
        if (Physics.Raycast(transform.position, directionOfAim, out hitObstacle, distanceToRallyPoint))
        {
            Debug.Log(hitObstacle.transform.gameObject.tag);
            if (hitObstacle.transform.gameObject.tag == "Avoid")
            {
                pathClear = false;
                Debug.Log("Obstacle: " + hitObstacle.point);
                Debug.Log("Direction to Rally Point: " + directionToRallyPoint);
                //Debug.Log("Direction of Aim: " + directionOfAim);
                obstacle = hitObstacle.point;
                CalculateDecisionPoint();
            }
            
        }
        //Debug.Log("Soldier Position: " + transform.position);
        //Debug.Log("Direction to Rally Point: " + directionToRallyPoint);
        //Debug.Log("Distance to Rally Point: " + distanceToRallyPoint);
        //Debug.Log("Obstacle: " + hitObstacle.point);
    }

    void CalculateDecisionPoint()
    {
        directionToObstacle = obstacle - transform.position;
        distanceToObstacle = Vector3.Distance(obstacle, transform.position);
        //float distanceToDecisionPoint = distanceToObstacle - .5f;
        ratioX = directionToObstacle.x / distanceToObstacle;
        ratioY = directionToObstacle.y / distanceToObstacle;
        ratioZ = directionToObstacle.z / distanceToObstacle;
        //decisionPoint.x = distanceToDecisionPoint * obstacle.x / distanceToObstacle;
        //decisionPoint.y = distanceToDecisionPoint * obstacle.y / distanceToObstacle;
        //decisionPoint.z = distanceToDecisionPoint * obstacle.z / distanceToObstacle;
        //decisionPoint.x = distanceToDecisionPoint * directionToObstacle.x / distanceToObstacle * obstacle.x;
        //decisionPoint.y = distanceToDecisionPoint * directionToObstacle.y / distanceToObstacle * obstacle.y;
        //decisionPoint.z = distanceToDecisionPoint * directionToObstacle.z / distanceToObstacle * obstacle.z;
        decisionPoint.x = (ratioX * obstacle.x) - (ratioX * .5f);
        decisionPoint.y = (ratioY * obstacle.y) - (ratioY * .5f);
        decisionPoint.z = (ratioZ * obstacle.z) - (ratioZ * .5f);
        marker.transform.position = decisionPoint;
        Debug.Log("decision point: " + decisionPoint);
        Debug.Log("direction to obstacle: " + directionToObstacle);
        Debug.Log("Distance To Obstacle: " + distanceToObstacle);
    }
}
