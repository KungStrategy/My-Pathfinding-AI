using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject rallyPoint;
    Vector3 position;
    Vector3 directionToRallyPoint;
    Vector3 obstacle;
    Vector3 directionToObstacle;
    Vector3 decisionPoint;
    float distanceToRallyPoint;
    float distanceToObstacle;
    float speed = 2f;
    float ratioX;
    float ratioZ;
    bool pathClear = true;

    void Start() => position = transform.position;

    void Update()
    {
        if (Input.touchCount > 0)
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



        RaycastHit hitObstacle;
        if (Physics.Raycast(transform.position, directionToRallyPoint, out hitObstacle, distanceToRallyPoint))
        {
            pathClear = false;
            Debug.Log("Obstacle: " + hitObstacle.point);
            //distanceToObstacle = hitObstacle.distance;
            obstacle = hitObstacle.point;

        }


        if (pathClear == true)
        {
            directionToRallyPoint = rallyPoint.transform.position - transform.position;
            distanceToRallyPoint = Vector3.Distance(rallyPoint.transform.position, transform.position);
            ratioX = directionToRallyPoint.x / distanceToRallyPoint;
            ratioZ = directionToRallyPoint.z / distanceToRallyPoint;
            position.x += ratioX * speed * Time.deltaTime;
            position.z += ratioZ * speed * Time.deltaTime;
            transform.position = position;
        }

        if (pathClear == false)
        {
            CalculateDecisionPoint();
        }
        /*else
        {
            directionToObstacle = obstacle - transform.position;
            distanceToObstacle = Vector3.Distance(obstacle, transform.position);
            if (distanceToObstacle > 0.5)
            {
                ratioX = directionToObstacle.x / distanceToObstacle;
                ratioZ = directionToObstacle.z / distanceToObstacle;
                position.x += ratioX * speed * Time.deltaTime;
                position.z += ratioZ * speed * Time.deltaTime;
                transform.position = position;
            }
        }*/

    }

    void CalculateDecisionPoint()
    {
        directionToObstacle = obstacle - transform.position;
        distanceToObstacle = Vector3.Distance(obstacle, transform.position);
        float distanceToDecisionPoint = distanceToObstacle - 0.5f;
        decisionPoint.x = distanceToDecisionPoint / directionToObstacle.x;
        decisionPoint.y = distanceToDecisionPoint / directionToObstacle.y;
        decisionPoint.z = distanceToDecisionPoint / directionToObstacle.z;
        Debug.Log("decision point: " + decisionPoint);
    }
}
