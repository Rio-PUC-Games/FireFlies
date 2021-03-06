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

    private Vector3 impulseVector = Vector3.zero;

    public Animator playerAnim;
    public Animator cameraAnim;
    public GameObject feedbackParticles;

    public AudioClip shootSound;
    public AudioClip aimSound;

    private AudioSource source;
    private float volLowRange = .5f;
    private float volHighRange = 1.0f;

    [HideInInspector]
    public bool isAbleToJump = true;
    [HideInInspector]
    public bool isPerfectJump = true;
    [HideInInspector]
    public float MaxImpulseRadius = 3;
    [HideInInspector]
    public float MinImpulseRadius = 1.5f;

    public GameObject pauseCanvas;

    private bool oneCheck = true;
    private bool oneCheck2 = true;
    private int pauseState = 0;
    private bool cancelledJump = false;

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

        if (Input.GetButtonDown("Cancel") && pauseState == 0)
        {
            Debug.Log("Pausou");
            pauseState = 1;
            pauseCanvas.SetActive(true);
            Time.timeScale = 0;
        }
        if(Input.GetButtonUp("Cancel") && pauseState == 1)
        {
            pauseState = 2;
        }
        if (Input.GetButtonDown("Cancel") && pauseState == 2)
        {
            Debug.Log("Despausou");
            pauseState = 3;
            pauseCanvas.SetActive(false);
            Time.timeScale = 1;
        }
        if(Input.GetButtonUp("Cancel") && pauseState == 3)
        {
            pauseState = 0;
        }
    }
    public void PauseGame()
    {
        Debug.Log("Pausou");
        pauseState = 2;
        pauseCanvas.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeGameFromPause()
    {
        Debug.Log("Despausou");
        pauseState = 0;
        pauseCanvas.SetActive(false);
        Time.timeScale = 1;
    }

	private void OnEnable()
	{
        source = GetComponent<AudioSource>();
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
        initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (isAbleToJump && pauseState == 0)
        {
        	float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(aimSound, vol);
        }
    }

    // Está clicando
    private void OnMouseDrag()
    {
        if (isAbleToJump && pauseState == 0 && !cancelledJump)
        {
            // Diminui o tempo
            time.slowTime();

            // ajeita a posição do mouse e pega o tamanho do impulso para quando ele soltar o player
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickImpulsePlayerComponent.CreateImpulse(initialMousePosition - currentMousePosition);

            // calcula a distancia maxima que o vetor pode ter
            if(Vector3.Distance(initialMousePosition, currentMousePosition) <= MaxImpulseRadius){
                line.SetPosition(0, player.transform.position + (initialMousePosition - currentMousePosition));
                line.SetPosition(1, player.transform.position + (initialMousePosition - currentMousePosition) * -1);
                impulseVector = initialMousePosition - currentMousePosition;
            } else {
                Vector3 outsideVector = (initialMousePosition - currentMousePosition).normalized * MaxImpulseRadius;
                line.SetPosition(0, player.transform.position + outsideVector);
                line.SetPosition(1, player.transform.position + outsideVector * -1);
                impulseVector = outsideVector;
            }
            
            //evita de força zero (game breaking)
            if(impulseVector.magnitude > MinImpulseRadius){
                // Desenha a linha do impulso
                line.enabled = true;

                // ajeita a ponta do vetor
                Arrow.SetActive(true);
                Arrow.transform.position = line.GetPosition(0);

                Vector3 dir = line.GetPosition(0) - player.transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else{
                // apaga a linha de impulso
                line.enabled = false;
                Arrow.SetActive(false);
            }
        }
    }

    // Soltou o clique
	private void OnMouseUp()
	{
        if (isAbleToJump && pauseState == 0 && !cancelledJump)
        {
            // libera o tempo a ser normal
            time.normalTime();

            // apaga a linha de impulso
            line.enabled = false;
            Arrow.SetActive(false);

            //evita pulos de força zero (game breaking)
            if(impulseVector.magnitude > MinImpulseRadius){
                // Zera a velocidade do player antes de dar um novo impulso para não ter soma de vetores
                GameObject.Find("Player").GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                clickImpulsePlayerComponent.CreateImpulse(impulseVector);
                clickImpulsePlayerComponent.Jump(isPerfectJump);

                // Solta o som do impulso
                float vol = Random.Range(volLowRange, volHighRange);
                source.PlayOneShot(shootSound, vol);

                // coloca como falso até que o player toque numa plataforma novamente
                isAbleToJump = false;
                isPerfectJump = false;
            }

            // zera todos os efeitos para recalculagem
            currentMousePosition = Vector3.zero;
            initialMousePosition = Vector3.zero;
            impulseVector = Vector3.zero;
        }
        cancelledJump = false;

	}
    public void CancelJump(){
        if(isAbleToJump && Input.GetMouseButton(0)){
            //garante que mouseDrag e mouseUp não realizam funções de pulo
            cancelledJump = true;

            // libera o tempo a ser normal
            time.normalTime();

            // apaga a linha de impulso
            line.enabled = false;
            Arrow.SetActive(false);

            // zera todos os efeitos para recalculagem
            currentMousePosition = Vector3.zero;
            initialMousePosition = Vector3.zero;
            impulseVector = Vector3.zero;
        }
    }
}
