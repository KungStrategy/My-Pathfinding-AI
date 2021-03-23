using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject rallyPoint;
    public GameObject marker;
    GameObject obstacle;
    //CircleCollider2D circ;
    CapsuleCollider col;
    Vector3 position;
    Vector3 directionToRallyPoint;
    Vector3 directionOfAim;
    Vector3 obstacleHitPoint;
    Vector3 directionToObstacle;
    Vector3 directionToCenter;
    Vector3 positionPointSaver;
    Vector3 exitCirclePoint;
    float distanceToRallyPoint;
    float distanceToObstacle;
    float distanceToCenter;
    float speed = 2f;
    //float angularSpeedConverter;
    float ratioX;
    float ratioY;
    float ratioZ;
    float angleRight;
    float angleLeft;
    float timeCounter;
    float startAngle;
    //float exitLoopDistance;
    bool pathClear = false;
    bool obstacleDetected = false;
    bool walkingAroundObstacle = false;
    string directionOfTravel;

    void Start() => position = transform.position;

    void Update()
    {
        //directionToRallyPoint = rallyPoint.transform.position - transform.position;
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
                    directionToCenter = obstacle.transform.position - transform.position;
                    distanceToCenter = Vector3.Distance(obstacle.transform.position, transform.position);
                    
                    if (directionToCenter.x <= 0)
                    {
                        startAngle = Mathf.Atan(directionToCenter.z / directionToCenter.x);
                    }
                    else
                    {
                        startAngle = Mathf.Atan(directionToCenter.z / directionToCenter.x) + Mathf.PI;
                    }
                    
                    positionPointSaver = transform.position;
                    Debug.Log("Intersection Point: " + positionPointSaver);
                    Debug.Log("radius: " + distanceToCenter);
                    //Debug.Log("Rally Point Vector" + directionToRallyPoint);
                    //Debug.Log("AIM Vector" + directionOfAim);
                    timeCounter = startAngle;
                    //Debug.DrawRay(positionPointSaver, directionToRallyPoint, Color.blue, 1000f);
                    ChooseRightOrLeft();
                }
            }

        }

        if (walkingAroundObstacle == true)
        {
            if (directionOfTravel == "CounterClockwise")
            {
                timeCounter += Time.deltaTime * (speed / distanceToCenter);
            }
            else
            {
                timeCounter -= Time.deltaTime * (speed / distanceToCenter); // * speed;
            }

            position.x = (Mathf.Cos(timeCounter) * distanceToCenter) + obstacle.transform.position.x;
            position.y = transform.position.y;
            position.z = (Mathf.Sin(timeCounter) * distanceToCenter) + obstacle.transform.position.z;

            transform.position = position;
            /*if(distanceToRallyPoint <= exitLoopDistance)
            {
                walkingAroundObstacle = false;
            }*/
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
        Debug.Log("Rally Point Vector" + directionToRallyPoint);
        Debug.Log("AIM Vector" + directionOfAim);
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
                obstacle = hitObstacle.transform.gameObject;
                //Debug.Log("Obstacle Center: " + obstacle.transform.position);
            }
        }
    }

    void ChooseRightOrLeft()
    {
        directionToObstacle = obstacleHitPoint - transform.position;
        CheckRight();
        CheckLeft();
        if (angleRight < -angleLeft)
        {
            walkingAroundObstacle = true;
            directionOfTravel = "CounterClockwise";
            //CalculateExitPoint();
        }
        else
        {
            walkingAroundObstacle = true;
            directionOfTravel = "Clockwise";
            //CalculateExitPoint();
        }
        CalculateExitPoint();
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
            CheckLeft();
        }
    }

    void CalculateExitPoint()
    {
        Debug.Log("CalculateExitPoint");
        col = obstacle.GetComponent<CapsuleCollider>();
        col.radius = distanceToCenter;
        Debug.DrawRay(positionPointSaver, directionOfAim, Color.blue, 1000f);
        RaycastHit hit;
        if (Physics.Raycast(positionPointSaver, directionOfAim, out hit, 4))
        {
            if (hit.collider)
            {
                exitCirclePoint = hit.point;
                Debug.Log("Exit Point: " + exitCirclePoint);
            }
        }
        //float temporaryVariable = Vector3.Distance(rallyPoint.transform.position, positionPointSaver);
        //exitLoopDistance = temporaryVariable - (2 * distanceToCenter);
    }
}
