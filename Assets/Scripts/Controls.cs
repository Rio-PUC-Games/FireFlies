﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

    public GameObject Arrow;
    public GameObject Main;

    private GameObject player;
    private ClickImpulse clickImpulsePlayerComponent;
    private LineRenderer line;
    private TimeManager time;

    private Vector3 currentMousePosition = Vector3.zero;
    private Vector3 initialMousePosition = Vector3.zero;

    [HideInInspector]
    public bool isAbleToJump = true;
    [HideInInspector]
    public bool isPerfectJump = true;
    [HideInInspector]
    public float MaxImpulseRadius = 3;

	private void LateUpdate()
	{
        if(Input.GetMouseButtonDown(0)){
            OnMouseDown();
        }
        
        if(Input.GetMouseButton(0)){
            OnMouseDrag();
        } 

        if (Input.GetMouseButtonUp(0)){
            OnMouseUp();
        }
	}

	private void OnEnable()
	{
        time = Main.GetComponent<TimeManager>();
        player = GameObject.Find("Player");
        line = player.GetComponent<LineRenderer>();
        clickImpulsePlayerComponent = player.GetComponent<ClickImpulse>();
        lineSetup();
	}

    private void lineSetup(){

        // line
        line.material = new Material(Shader.Find("Particles/Additive"));
        line.widthMultiplier = 0.2f;
        line.positionCount = 2;

    } 

    // Clicou
	private void OnMouseDown()
	{
        initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
    }

    // Está clicando
    private void OnMouseDrag()
    {
        if (isAbleToJump)
        {
            time.slowTime();
            line.enabled = true;
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
            clickImpulsePlayerComponent.CreateImpulse(initialMousePosition - currentMousePosition);

            if(Vector3.Distance(initialMousePosition, currentMousePosition) <= MaxImpulseRadius){
                line.SetPosition(0, player.transform.position + (initialMousePosition - currentMousePosition));
                line.SetPosition(1, player.transform.position + (initialMousePosition - currentMousePosition) * -1);
            } else {

                Vector3 outsideVector = (initialMousePosition - currentMousePosition).normalized * MaxImpulseRadius;

                line.SetPosition(0, player.transform.position + outsideVector);
                line.SetPosition(1, player.transform.position + outsideVector * -1);
            }

            Arrow.SetActive(true);
            Arrow.transform.position = line.GetPosition(0);
        }
    }

    // Soltou o clique
	private void OnMouseUp()
	{
        if (isAbleToJump)
        {
            time.normalTime();
            line.enabled = false;
            GameObject.Find("Player").GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            clickImpulsePlayerComponent.CreateImpulse(initialMousePosition - currentMousePosition);
            clickImpulsePlayerComponent.Jump(isPerfectJump);
            isAbleToJump = false;
            currentMousePosition = Vector3.zero;
            initialMousePosition = Vector3.zero;
            isPerfectJump = false;
            Arrow.SetActive(false);
        }
	}

}
