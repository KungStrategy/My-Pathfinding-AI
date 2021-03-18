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
    Vector3 directionToCenter;
    Vector3 positionPointSaver;
    Vector3 positionAdjuster;
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
    float timeCounter;
    float startAngle;
    string obstacleName;
    bool pathClear = false;
    bool obstacleDetected = false;
    //bool pathDecisionPoint = false;
    bool walkingAroundObstacle = false;
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
            position.y = transform.position.y;
            position.z += ratioZ * speed * Time.deltaTime;
            transform.position = position;

            if (obstacleDetected == true)
            {
                distanceToCenter = Vector3.Distance(obstacle.transform.position, transform.position);

                if (distanceToCenter <= 2)
                {
                    pathClear = false;
                    Debug.Log("Distance to center: " + distanceToCenter);
                    directionToCenter = obstacle.transform.position - transform.position;

                    Vector3 directionFromObstacleToSoldier = transform.position - obstacle.transform.position;

                    distanceToCenter = Vector3.Distance(obstacle.transform.position, transform.position);
                    //startAngle = Vector3.Angle(obstacle.transform.position, transform.position);

                    startAngle = Vector3.Distance(transform.position, directionFromObstacleToSoldier);
                    Debug.Log("Distance to Center: " + distanceToCenter);
                    Debug.Log("position at start of circle: " + transform.position);
                    positionPointSaver = transform.position;
                    
                    //positionAdjuster.x = (2 * (obstacle.transform.position.x - positionPointSaver.x));
                    //positionAdjuster.y = positionPointSaver.y;
                    //positionAdjuster.z = (2 * (obstacle.transform.position.z - positionPointSaver.z));
                    //Debug.Log("Center of circle: " + positionAdjuster);
                    //Debug.Log("Direction to center: " + directionToCenter);
                    //Debug.DrawRay(transform.position, directionToCenter, Color.blue);
                    //timeCounter = startAngle;
                    ChooseRightOrLeft();
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

        if (walkingAroundObstacle == true)
        {
            timeCounter += Time.deltaTime * speed;
            float ratioXCenter = directionToCenter.x / distanceToCenter;
            float ratioZCenter = directionToCenter.z / distanceToCenter;
            //position.x = (Mathf.Cos(timeCounter) * distanceToCenter) + (2 * distanceToCenter * ratioXCenter);
            //position.y = 0;
            //position.z = (Mathf.Sin(timeCounter) * distanceToCenter) + (2 * distanceToCenter * ratioZCenter);

            //position.x = (Mathf.Cos(timeCounter) * distanceToCenter) + (distanceToCenter * directionToCenter.x);
            //position.y = 0;
            //position.z = (Mathf.Sin(timeCounter) * distanceToCenter) + (distanceToCenter * directionToCenter.z);

            //position.x = (Mathf.Cos(timeCounter) * distanceToCenter) + (2 * ratioXCenter);
            //position.y = 0;
            //position.z = (Mathf.Sin(timeCounter) * distanceToCenter) + (2 * ratioZCenter);


            //position.x = (Mathf.Cos(timeCounter) * distanceToCenter) + (2 * (obstacle.transform.position.x - positionPointSaver.x));
            //position.y = transform.position.y;
            //position.z = (Mathf.Sin(timeCounter) * distanceToCenter) + (2 * (obstacle.transform.position.z - positionPointSaver.z));

            position.x = (Mathf.Cos(timeCounter) * distanceToCenter) + obstacle.transform.position.x;
            position.y = transform.position.y;
            position.z = (Mathf.Sin(timeCounter) * distanceToCenter) + obstacle.transform.position.z;

            transform.position = position;
            //Debug.Log("Angle: " + startAngle);
            //Debug.Log("Direction to Center: " + directionToCenter);
            
            //Debug.Log("ratio X: " + ratioXCenter);
            //Debug.Log("ratio Z: " + ratioZCenter);
            //transform.RotateAround(obstacle.transform.position, Vector3.right, speed * Time.deltaTime);
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
                //Debug.Log("Obstacle: " + hitObstacle.point);
                obstacleHitPoint = hitObstacle.point;
                obstacleName = hitObstacle.transform.gameObject.name;
                //Debug.Log("Name: " + obstacleName);
                obstacle = hitObstacle.transform.gameObject;
                //Debug.Log("Obstacle Center: " + obstacle.transform.position);
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
            //Debug.Log("Scan Point: " + scanPointRight);
            //directionToAvoidPoint = Quaternion.Euler(0, (angleRight + 10), 0) * directionToObstacle;
            //calculateAvoidPoint();
            walkingAroundObstacle = true;
        }
        else
        {
            Debug.Log("choose left");
            //Debug.Log("Scan Point: " + scanPointLeft);
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
            //scanPointRight = hit.point;
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
            //scanPointLeft = hit.point;
            CheckLeft();
        }
    }
}
