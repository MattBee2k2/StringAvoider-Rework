﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class StringLogic : MonoBehaviour
{
    [SerializeField] int stringResolution;
    [SerializeField] float segmentLength = 0.025f;
    [SerializeField] float radius = 0.1f;

    [SerializeField] Vector2 mousePosition, previousMousePosition, mouseDelta;

    float radians;

    [SerializeField] GameObject stringPointPrefab;
    [SerializeField] List<GameObject> stringPointsList;
    [SerializeField] List<Rigidbody2D> stringPointsRB;
    [SerializeField] List<Vector2> stringPointsData;

    [SerializeField] Transform spawnPoint;

    [SerializeField] bool canDrag;
    

    // Start is called before the first frame update
    void Awake()
    {
        stringPointsList = new List<GameObject>();
        stringPointsRB = new List<Rigidbody2D>();
        stringPointsData = new List<Vector2>();

        for (int i = 0; i < stringResolution; i++)
        {
            radians = 12 * Mathf.PI * i / stringResolution + Mathf.PI / 4;

            Vector2 stringPointPosition = new Vector2((spawnPoint.position.x + radius * Mathf.Cos(radians)), spawnPoint.position.y + radius * Mathf.Sin(radians));

            stringPointsList.Add(Instantiate(stringPointPrefab, stringPointPosition, Quaternion.identity, this.transform));
            stringPointsRB.Add(stringPointsList[i].GetComponent<Rigidbody2D>());
         }
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mouseDelta = mousePosition - previousMousePosition;

        if(Input.GetMouseButtonDown(0))
        {
            canDrag = true;
            previousMousePosition = mousePosition;
            mouseDelta = Vector2.zero; 
        }


        if (Input.GetMouseButton(0) && canDrag)
        {
            previousMousePosition = mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            canDrag = false;
        }
    }

    void FixedUpdate()
    {
        if(canDrag)
        {
            MoveString(mouseDelta.x, mouseDelta.y);
        }
    }

    void MoveString(float x, float y)
    {
        //stringPointsList[0].transform.position = new Vector2(x + stringPointsList[0].transform.position.x, y + stringPointsList[0].transform.position.y);
        stringPointsRB[0].MovePosition(new Vector2(x + stringPointsList[0].transform.position.x, y + stringPointsList[0].transform.position.y));

        for (int i = 1; i < stringResolution ; i++)
        {
            float nodeAngle = Mathf.Atan2(stringPointsList[i].transform.position.y - stringPointsList[i - 1].transform.position.y, stringPointsList[i].transform.position.x - stringPointsList[i - 1].transform.position.x);

            //stringPointsList[i].transform.position = new Vector2(stringPointsList[i - 1].transform.position.x + segmentLength * Mathf.Cos(nodeAngle), stringPointsList[i - 1].transform.position.y + segmentLength * Mathf.Sin(nodeAngle));
            stringPointsRB[i].MovePosition(new Vector2(stringPointsList[i - 1].transform.position.x + segmentLength * Mathf.Cos(nodeAngle), stringPointsList[i - 1].transform.position.y + segmentLength * Mathf.Sin(nodeAngle)));
        }
    }
}
