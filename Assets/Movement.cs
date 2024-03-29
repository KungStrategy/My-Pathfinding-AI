﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject rallyPoint;
    GameObject obstacle;
    Vector3 position;
    Vector3 directionToRallyPoint;
    Vector3 directionOfAim;
    Vector3 directionToCenter;
    Vector3 enterCirclePoint;
    Vector3 exitCirclePoint;
    float distanceToRallyPoint;
    float radius;
    float speed = 2f;
    float angleRight = 25f;
    float angleLeft = -25f;
    float radians;
    float exitCircleDistance;
    bool pathClear = false;
    bool obstacleDetected = false;
    bool walkingAroundObstacle = false;
    string directionOfTravel;

    void Start()
    {
        position = transform.position;
        //makes colliders bigger so rally points don't get to close to obstacles
        EnlargeCapsuleColliderOfObstacles();
    }

    void Update()
    {
        //checks for input from player
        if (Input.touchCount > 0)
        {
            //makes sure the functions are only called once
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ReasignRallyPoint();
                CheckPath();
            }
        }

        //normal walking script
        if (pathClear == true)
        {
            directionToRallyPoint = rallyPoint.transform.position - transform.position;
            distanceToRallyPoint = Vector3.Distance(rallyPoint.transform.position, transform.position);
            //calculates x and y components of direction
            float ratioX = directionToRallyPoint.x / distanceToRallyPoint;
            float ratioZ = directionToRallyPoint.z / distanceToRallyPoint;
            //aplies movement over time
            position.x += ratioX * speed * Time.deltaTime;
            position.y = transform.position.y;
            position.z += ratioZ * speed * Time.deltaTime;
            //moves the soldier
            transform.position = position;

            //what to do if CheckPath() spots an obstacle
            if (obstacleDetected == true)
            {
                radius = Vector3.Distance(obstacle.transform.position, transform.position);
                //stay on path untill close to obstacle
                if (radius <= ((obstacle.transform.localScale.x/2) + (transform.localScale.x/2) + 0.01))
                {
                    pathClear = false;
                    directionToCenter = obstacle.transform.position - transform.position;
                    radius = Vector3.Distance(obstacle.transform.position, transform.position);
                    //calculate the angle between the center of the obstacle and the soldier
                    if (directionToCenter.x <= 0)
                    {
                        radians = Mathf.Atan(directionToCenter.z / directionToCenter.x);
                    }
                    //adds 180 degs because unity angles start over at 180
                    else
                    {
                        radians = Mathf.Atan(directionToCenter.z / directionToCenter.x) + Mathf.PI;
                    }
                    enterCirclePoint = transform.position;
                    ChooseRightOrLeft();
                }
            }
        }

        //send the soldier walking in a cirle around obstacle
        if (walkingAroundObstacle == true)
        {
            //Debug.Log("walking");
            if (directionOfTravel == "CounterClockwise")
            {
                radians += Time.deltaTime * (speed / radius);
            }
            else
            {
                radians -= Time.deltaTime * (speed / radius);
            }
            //cos and sin make the circle, must add obstacle position to adjust center
            position.x = (Mathf.Cos(radians) * radius) + obstacle.transform.position.x;
            position.y = transform.position.y;
            position.z = (Mathf.Sin(radians) * radius) + obstacle.transform.position.z;
            transform.position = position;
            //checks to when the soldier should stop circling
            exitCircleDistance = Vector3.Distance(exitCirclePoint, transform.position);
            if(exitCircleDistance < 0.05)
            {
                walkingAroundObstacle = false;
                obstacleDetected = false;
                pathClear = true;
                // check to see if there is another obstacle
                CheckPath();
            }
        }
    }

    //moves the rally point to where ever the player touches
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
            //checks to see if point touched is the ground
            if (hit.transform.gameObject.tag == "Ground")
            {
                rallyPoint.transform.position = newPosition;
            }
            //sets a modified rally point so that soldier does not run into obstacle
            else
            {
                GameObject touchedObstacle = hit.transform.gameObject;
                Vector3 directionToTouchedObstacle = touchedObstacle.transform.position - newPosition;
                // finds the angle from center of object touched so soldier can move to that side of the obstacle
                if (directionToTouchedObstacle.x <= 0)
                {
                    radians = Mathf.Atan(directionToTouchedObstacle.z / directionToTouchedObstacle.x);
                }
                //adds 180 degs because unity angles start over at 180
                else
                {
                    radians = Mathf.Atan(directionToTouchedObstacle.z / directionToTouchedObstacle.x) + Mathf.PI;
                }
                Vector3 modifiedRallyPoint = new Vector3(0, 0, 0);
                //makes sure the rally point is proper distance from obstacle
                float colliderRadius = (touchedObstacle.transform.localScale.x / 2) + (transform.localScale.x / 2) + 0.1f;
                //trig to calculate exact point
                modifiedRallyPoint.x = (Mathf.Cos(radians) * colliderRadius) + touchedObstacle.transform.position.x;
                modifiedRallyPoint.y = 0.5f;
                modifiedRallyPoint.z = (Mathf.Sin(radians) * colliderRadius) + touchedObstacle.transform.position.z;
                rallyPoint.transform.position = modifiedRallyPoint;
            }
        }
    }

    //checks to see if there is an obstacle between the soldier and the rally point
    void CheckPath()
    {
        directionToRallyPoint = rallyPoint.transform.position - transform.position;
        directionOfAim = directionToRallyPoint;
        //adjusts the vector so that it is not aiming at the ground
        directionOfAim.y += 1;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionOfAim, out hit, Mathf.Infinity))
        {
            //verifies that what it hit is an obstacle
            if (hit.transform.gameObject.tag == "Avoid")
            {
                obstacleDetected = true;
                obstacle = hit.transform.gameObject;
            }
        }
    }

    //sees what direction is faster to get around the obstacle
    void ChooseRightOrLeft()
    {
        //sets colliders so that CheckRight() and CheckLeft() can see the obstacle
        ResetCapsuleColliderOfObstacles();
        CheckRight();
        CheckLeft();
        //chooses direction
        if (angleRight < -angleLeft)
        {
            directionOfTravel = "CounterClockwise";
        }
        else
        {
            directionOfTravel = "Clockwise";
        }
        walkingAroundObstacle = true;
        //reset angles so they can evaluate other obstacles
        angleRight = 25f;
        angleLeft = -25f;
        CalculateExitPoint();
        //makes colliders bigger so other rally points don't get to close to obstacles
        EnlargeCapsuleColliderOfObstacles();
    }

    //keeps checking 5 more degrees to the right until it clears the obstacle
    void CheckRight()
    {
        angleRight += 5;
        Vector3 newVector = Quaternion.Euler(0, angleRight, 0) * directionOfAim;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, newVector, out hit, (2 * obstacle.transform.localScale.x)))
        {
            CheckRight();
        }
    }

    //keeps checking 5 more degrees to the left until it clears the obstacle
    void CheckLeft()
    {
        angleLeft -= 5;
        Vector3 newVector = Quaternion.Euler(0, angleLeft, 0) * directionOfAim;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, newVector, out hit, (2 * obstacle.transform.localScale.x)))
        {
            CheckLeft();
        }
    }

    // calculates the point that the soldier should stop circling
    void CalculateExitPoint()
    {
        //long equation to find the point that a vector intersects a circle
        float a, b, c;
        float quadratic;
        a = directionToRallyPoint.x * directionToRallyPoint.x + directionToRallyPoint.z * directionToRallyPoint.z;
        b = 2 * (directionToRallyPoint.x * (enterCirclePoint.x - obstacle.transform.position.x) + directionToRallyPoint.z * (enterCirclePoint.z - obstacle.transform.position.z));
        c = obstacle.transform.position.x * obstacle.transform.position.x + obstacle.transform.position.z * obstacle.transform.position.z;
        c += enterCirclePoint.x * enterCirclePoint.x + enterCirclePoint.z * enterCirclePoint.z;
        c -= 2 * (obstacle.transform.position.x * enterCirclePoint.x + obstacle.transform.position.z * enterCirclePoint.z);
        c -= radius * radius;
        quadratic = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
        exitCirclePoint = new Vector3(enterCirclePoint.x + quadratic * directionToRallyPoint.x, transform.position.y, enterCirclePoint.z + quadratic * directionToRallyPoint.z);
    }
    
    //makes all colliders slightly bigger than the radius of the walk around object loop
    void EnlargeCapsuleColliderOfObstacles()
    {
        GameObject[] avoid = GameObject.FindGameObjectsWithTag("Avoid");
        for (int i = 0; i < avoid.Length; i++)
        {
            CapsuleCollider col = avoid[i].GetComponent<CapsuleCollider>();
            col.radius = ((avoid[i].transform.localScale.x / 2) + (transform.localScale.x / 2) + 0.1f) / avoid[i].transform.localScale.x;
        }
    }

    //makes all colliders the same size as the obstacles
    void ResetCapsuleColliderOfObstacles()
    {
        GameObject[] avoid = GameObject.FindGameObjectsWithTag("Avoid");
        for (int i = 0; i < avoid.Length; i++)
        {
            CapsuleCollider col = avoid[i].GetComponent<CapsuleCollider>();
            col.radius = 0.5f;
        }
    }
}
