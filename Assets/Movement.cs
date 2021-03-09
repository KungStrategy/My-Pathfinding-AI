using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject rallyPoint;
    Vector3 position;
    Vector3 directionToRallyPoint;
    Vector3 directionOfAim;
    Vector3 obstacle;
    Vector3 directionToObstacle;
    Vector3 decisionPoint;
    Vector3 directionToDecisionPoint;
    float distanceToRallyPoint;
    float distanceToObstacle;
    float distanceToDecisionPoint;
    float speed = 2f;
    float ratioX;
    float ratioY;
    float ratioZ;
    float angleRight;
    float angleLeft;
    bool pathClear = true;
    bool chooseLeftOrRightActivated = false;

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
        else
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
                if (chooseLeftOrRightActivated == false)
                {
                    ChooseRightOrLeft();
                }
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
            Debug.Log(hitObstacle.transform.gameObject.tag);
            if (hitObstacle.transform.gameObject.tag == "Avoid")
            {
                pathClear = false;
                Debug.Log("Obstacle: " + hitObstacle.point);
                obstacle = hitObstacle.point;
                CalculateDecisionPoint();
            }
        }
    }

    void CalculateDecisionPoint()
    {
        directionToObstacle = obstacle - transform.position;
        distanceToObstacle = Vector3.Distance(obstacle, transform.position);
        float ratioXDecision = directionToObstacle.x / distanceToObstacle;
        float ratioYDecision = directionToObstacle.y / distanceToObstacle;
        float ratioZDecision = directionToObstacle.z / distanceToObstacle;
        decisionPoint.x = obstacle.x - (ratioXDecision * 1f);
        decisionPoint.y = obstacle.y - (ratioYDecision * 1f);
        decisionPoint.z = obstacle.z - (ratioZDecision * 1f);
        Debug.Log("decision point: " + decisionPoint);
    }

    void ChooseRightOrLeft()
    {
        chooseLeftOrRightActivated = true;
        directionToObstacle = obstacle - transform.position;
        CheckRight();
        CheckLeft();
    }

    void CheckRight()
    {
        angleRight += 5;
        Debug.Log("Angle: " + angleRight);
        //Debug.Log("Direction to Obstacle: " + directionToObstacle);
        Vector3 newVector = Quaternion.Euler(0, angleRight, 0) * directionToObstacle;
        Debug.Log("New Vector: " + newVector);
        RaycastHit hitCheck;
        if (Physics.Raycast(transform.position, newVector, out hitCheck, 5))
        {
            Debug.Log("Hit point: " + hitCheck.point);
            CheckRight();
        }
        else
        {
            Debug.Log("done");
        }
    }

    void CheckLeft()
    {
        angleLeft -= 5;
        Debug.Log("Angle: " + angleLeft);
        //Debug.Log("Direction to Obstacle: " + directionToObstacle);
        Vector3 newVector = Quaternion.Euler(0, angleLeft, 0) * directionToObstacle;
        Debug.Log("New Vector: " + newVector);
        RaycastHit hitCheck;
        if (Physics.Raycast(transform.position, newVector, out hitCheck, 5))
        {
            Debug.Log("Hit point: " + hitCheck.point);
            CheckLeft();
        }
        else
        {
            Debug.Log("done");
        }
    }
}
