using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class Node {

    public Vector3 pos;
    public Vector3 vel;
    public Vector3 force;
    public bool _fixed;
    public float mass;
    private MassSpringCloth massSprClth;

    // Use this for initialization
    // Primer paso de inicializacion es el awake 
    // Lo que se tiene que inicializar primero lo pongo aquí, y lo que depende de que esto esté inicializado lo pongo en el start
    private void Awake()
    {
        //pos = transform.position;
    }


    public Node(Vector3 v, MassSpringCloth massSpringCloth) { // Constructor dado un vertice (Vec3) y la referencia de la clase mspc
        pos = v;
        vel = Vector3.zero;
        force = Vector3.zero;
        mass = 1f;
        _fixed = false;
        massSprClth = massSpringCloth;
    }
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position = pos;
	}

    public void ComputeForces()
    {
        // Calcular la fuerza del nodo padre sobre este nodo
        force += massSprClth.Gravity;
    }

    public static implicit operator Vector3(Node v)
    {
        throw new NotImplementedException();
    }
}
