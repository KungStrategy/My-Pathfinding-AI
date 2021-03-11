using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject rallyPoint;
    public GameObject marker;
    GameObject obstacle;
    Vector3 position;
    Vector3 directionToRallyPoint;
    Vector3 directionOfAim;
    Vector3 obstacleHitPoint;
    Vector3 directionToObstacle;
    Vector3 decisionPoint;
    Vector3 directionToDecisionPoint;
    Vector3 scanPointRight;
    Vector3 scanPointLeft;
    Vector3 avoidPoint;
    Vector3 directionToAvoidPoint;
    float distanceToRallyPoint;
    float distanceToObstacle;
    float distanceToDecisionPoint;
    float distanceToCenter;
    float speed = 2f;
    float ratioX;
    float ratioY;
    float ratioZ;
    float angleRight;
    float angleLeft;
    string obstacleName;
    bool pathClear = false;
    bool obstacleDetected = false;
    //bool pathDecisionPoint = false;
    bool pathAvoidPoint = false;
    //bool chooseLeftOrRightActivated = false;

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

            if (obstacleDetected == true)
            {
                distanceToCenter = Vector3.Distance(obstacle.transform.position, transform.position);

                if (distanceToCenter <= 2)
                {
                    pathClear = false;
                    Debug.Log("Distance to center: " + distanceToCenter);
                }
            }

        }
        
        /*if (pathDecisionPoint == true)
        {
            directionToDecisionPoint = decisionPoint - transform.position;
            distanceToDecisionPoint = Vector3.Distance(decisionPoint, transform.position);
            ratioX = directionToDecisionPoint.x / distanceToDecisionPoint;
            ratioY = directionToDecisionPoint.y / distanceToDecisionPoint;
            ratioZ = directionToDecisionPoint.z / distanceToDecisionPoint;
            position.x += ratioX * speed * Time.deltaTime;
            position.y = 1.5f;
            position.z += ratioZ * speed * Time.deltaTime;
            transform.position = position;
            if (distanceToDecisionPoint <= 0.001)
            {
                
                //ChooseRightOrLeft();
                pathDecisionPoint = false;
            }
        }*/

        if (pathAvoidPoint == true)
        {

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
            pathClear = true;
            Vector3 newPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            rallyPoint.transform.position = newPosition;
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
            //Debug.Log(hitObstacle.transform.gameObject.tag);
            if (hitObstacle.transform.gameObject.tag == "Avoid")
            {
                //pathClear = false;
                obstacleDetected = true;
                Debug.Log("Obstacle: " + hitObstacle.point);
                obstacleHitPoint = hitObstacle.point;
                obstacleName = hitObstacle.transform.gameObject.name;
                Debug.Log("Name: " + obstacleName);
                obstacle = hitObstacle.transform.gameObject;
                Debug.Log("Obstacle Center: " + obstacle.transform.position);
                //CalculateDecisionPoint();
            }
        }
    }

    /*void CalculateDecisionPoint()
    {
        directionToObstacle = obstacleHitPoint - transform.position;
        distanceToObstacle = Vector3.Distance(obstacleHitPoint, transform.position);
        float ratioXDecision = directionToObstacle.x / distanceToObstacle;
        float ratioYDecision = directionToObstacle.y / distanceToObstacle;
        float ratioZDecision = directionToObstacle.z / distanceToObstacle;
        decisionPoint.x = obstacleHitPoint.x - (ratioXDecision * 1f);
        decisionPoint.y = obstacleHitPoint.y - (ratioYDecision * 1f);
        decisionPoint.z = obstacleHitPoint.z - (ratioZDecision * 1f);
        pathDecisionPoint = true;
        Debug.Log("decision point: " + decisionPoint);
        
    }*/

    void ChooseRightOrLeft()
    {
        directionToObstacle = obstacleHitPoint - transform.position;
        CheckRight();
        CheckLeft();
        if (angleRight < -angleLeft)
        {
            Debug.Log("choose right");
            Debug.Log("Scan Point: " + scanPointRight);
            //directionToAvoidPoint = Quaternion.Euler(0, (angleRight + 10), 0) * directionToObstacle;
            //calculateAvoidPoint();
        }
        else
        {
            Debug.Log("choose left");
            Debug.Log("Scan Point: " + scanPointLeft);
            //directionToAvoidPoint = Quaternion.Euler(0, (angleLeft - 10), 0) * directionToObstacle;
            //calculateAvoidPoint();
        }
    }

    void CheckRight()
    {
        angleRight += 5;
        //Debug.Log("Angle: " + angleRight);
        Vector3 newVector = Quaternion.Euler(0, angleRight, 0) * directionToObstacle;
        //Debug.Log("New Vector: " + newVector);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, newVector, out hit, 5))
        {
            scanPointRight = hit.point;
            CheckRight();
        }
    }

    void CheckLeft()
    {
        angleLeft -= 5;
        //Debug.Log("Angle: " + angleLeft);
        Vector3 newVector = Quaternion.Euler(0, angleLeft, 0) * directionToObstacle;
        //Debug.Log("New Vector: " + newVector);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, newVector, out hit, 5))
        {
            scanPointLeft = hit.point;
            CheckLeft();
        }
    }

    void calculateAvoidPoint()
    {
        avoidPoint = (directionToAvoidPoint + obstacleHitPoint);
        Debug.Log("Avoid Point: " + avoidPoint);
        marker.transform.position = avoidPoint;
    }
}
