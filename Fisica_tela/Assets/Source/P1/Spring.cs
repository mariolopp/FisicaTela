﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spring {

    public Node nodeA, nodeB;

    public float Length0;
    public float Length;
    public Vector3 localScale;
    public Vector3 position;
    public float stiffness;
    public Quaternion rotation;

    public Spring(Node nodeA, Node nodeB, MassSpringCloth mspc)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
        stiffness = mspc.stiffness;
        localScale.Set(1f, 1f, 1f);
    }
    // Use this for initialization
    public void Start () {
        UpdateLength();
        Length0 = Length;
    }
	
	// Update is called once per frame
	public void Update () {
        // Queremos coger el vector unityario en la direccion y y queremos ponerlo en la posicion u
        localScale = new Vector3(localScale.x, Length / 2.0f, localScale.z);  // Escalando el cilindro en funcion de la longitud que tiene / 2 para que se pueda pinter con mas longitud mas tarde
        position = 0.5f * (nodeA.pos + nodeB.pos);    // Porque para pintar el cilindro se hace justo entre los dos nodos


        Vector3 u = nodeA.pos - nodeB.pos;
        u.Normalize();
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, u); // Esta funcion nos permite rotar el vector en direccion 'y' a la direccion 'u'
    }

    public void UpdateLength ()
    {
        Length = (nodeA.pos - nodeB.pos).magnitude; // Tamaño del vector que une las posiciones de los dos nodos
    }

    public void ComputeForces()
    {
        Vector3 u = nodeA.pos - nodeB.pos;      // Vector que une las posiciones de los dos nodos
        u.Normalize();
        Vector3 force = (-stiffness) * (Length - Length0) * u;   // Formula de la fuerza elástica
        nodeA.force += force;
        nodeB.force -= force;
    }
}
