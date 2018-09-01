﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickImpulse : MonoBehaviour {

    [Tooltip("forca que é aplicada a qualquer vetor de pulo")]
    public float ImpulseForce = 1.0f;

    private Rigidbody2D rb;

	private void OnEnable()
	{
        rb = this.GetComponent<Rigidbody2D>();
	}

	// Faz o pulo do jogador
	public void CreateImpulse(Vector3 mousePosition){

        Debug.Log(mousePosition);
        rb.AddForce(new Vector3(mousePosition.x*-1 , mousePosition.y*-1 , mousePosition.z) * ImpulseForce);

    }





	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}