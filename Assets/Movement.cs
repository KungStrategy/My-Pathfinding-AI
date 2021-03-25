using System.Collections;
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
    float angleRight;
    float angleLeft;
    float radians;
    float exitCircleDistance;
    bool pathClear = false;
    bool obstacleDetected = false;
    bool walkingAroundObstacle = false;
    string directionOfTravel;

    void Start() => position = transform.position;

    void Update()
    {
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
            directionToRallyPoint = rallyPoint.transform.position - transform.position;
            distanceToRallyPoint = Vector3.Distance(rallyPoint.transform.position, transform.position);
            float ratioX;
            float ratioZ;
            ratioX = directionToRallyPoint.x / distanceToRallyPoint;
            ratioZ = directionToRallyPoint.z / distanceToRallyPoint;
            position.x += ratioX * speed * Time.deltaTime;
            position.y = transform.position.y;
            position.z += ratioZ * speed * Time.deltaTime;
            transform.position = position;

            if (obstacleDetected == true)
            {
                radius = Vector3.Distance(obstacle.transform.position, transform.position);

                if (radius <= ((obstacle.transform.localScale.x/2) +1))
                {
                    pathClear = false;
                    directionToCenter = obstacle.transform.position - transform.position;
                    radius = Vector3.Distance(obstacle.transform.position, transform.position);
                    if (directionToCenter.x <= 0)
                    {
                        radians = Mathf.Atan(directionToCenter.z / directionToCenter.x);
                    }
                    else
                    {
                        radians = Mathf.Atan(directionToCenter.z / directionToCenter.x) + Mathf.PI;
                    }
                    enterCirclePoint = transform.position;
                    ChooseRightOrLeft();
                }
            }
        }

        if (walkingAroundObstacle == true)
        {
            if (directionOfTravel == "CounterClockwise")
            {
                radians += Time.deltaTime * (speed / radius);
            }
            else
            {
                radians -= Time.deltaTime * (speed / radius);
            }
            position.x = (Mathf.Cos(radians) * radius) + obstacle.transform.position.x;
            position.y = transform.position.y;
            position.z = (Mathf.Sin(radians) * radius) + obstacle.transform.position.z;
            transform.position = position;
            exitCircleDistance = Vector3.Distance(exitCirclePoint, transform.position);
            if(exitCircleDistance < 0.01)
            {
                walkingAroundObstacle = false;
                obstacleDetected = false;
                pathClear = true;
                CheckPath();
            }
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
        directionOfAim = directionToRallyPoint;
        directionOfAim.y += 1;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionOfAim, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.tag == "Avoid")
            {
                obstacleDetected = true;
                obstacle = hit.transform.gameObject;
            }
        }
    }

    void ChooseRightOrLeft()
    {
        CheckRight();
        CheckLeft();
        if (angleRight < -angleLeft)
        {
            directionOfTravel = "CounterClockwise";
        }
        else
        {
            directionOfTravel = "Clockwise";
        }
        walkingAroundObstacle = true;
        CalculateExitPoint();
    }

    void CheckRight()
    {
        angleRight += 5;
        Vector3 newVector = Quaternion.Euler(0, angleRight, 0) * directionToRallyPoint;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, newVector, out hit, (2 * obstacle.transform.localScale.x)))
        {
            CheckRight();
        }
    }

    void CheckLeft()
    {
        angleLeft -= 5;
        Vector3 newVector = Quaternion.Euler(0, angleLeft, 0) * directionToRallyPoint;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, newVector, out hit, (2 * obstacle.transform.localScale.x)))
        {
            CheckLeft();
        }
    }

    void CalculateExitPoint()
    {
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
}
