﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

    private ClickImpulse clickImpulsePlayerComponent;
    private LineRenderer line;

    private Vector3 currentMousePosition = Vector3.zero;

	private void OnEnable()
	{
        line = this.GetComponent<LineRenderer>();
        clickImpulsePlayerComponent = this.GetComponent<ClickImpulse>();
        lineSetup();
	}

    private void lineSetup(){

        // line
        line.material = new Material(Shader.Find("Particles/Additive"));
        line.widthMultiplier = 0.2f;
        line.positionCount = 2;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.cyan, 0.0f), new GradientColorKey(Color.green, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        line.colorGradient = gradient;
    } 

    // Clicou
	private void OnMouseDown()
	{

    }

    // Está clicando
    private void OnMouseDrag()
    {
        line.enabled = true;
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        line.SetPosition(0, this.transform.position + currentMousePosition);
        line.SetPosition(1, this.transform.position + currentMousePosition*-1);
    }

    // Soltou o clique
	private void OnMouseUp()
	{
        line.enabled = false;
        clickImpulsePlayerComponent.CreateImpulse(currentMousePosition);
        currentMousePosition = Vector3.zero;
	}

}