﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerJumpAgain : MonoBehaviour {

    private Controls ctrl;



	private void OnEnable()
	{
        ctrl = GameObject.Find("Main Camera").GetComponent<Controls>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
        if(collision.gameObject.layer == 8){
            //Debug.Log("Faz coisas perfeitamente");
            ctrl.isPerfectJump = true;
        }
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
        
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
        ctrl.isPerfectJump = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.gameObject.layer == 8)
        {
            ctrl.isAbleToJump = true;

            if (ctrl.feedbackParticles != null)
            {
                ctrl.feedbackParticles.SetActive(false);
                ctrl.feedbackParticles.SetActive(true);
            }
        }
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

	private void OnTriggerExit2D(Collider2D collision)
	{
      
	}

}
