﻿using System;
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
        _fixed = false;
        massSprClth = massSpringCloth;
    }
    public void ComputeForces()
    {
        // Aplicar la fuerza de la gravedad sobre el nodo
        force += massSprClth.mass * massSprClth.Gravity;
        Vector3 viento = -(massSprClth.mass*1) * vel;
        force += viento;
    }

}
