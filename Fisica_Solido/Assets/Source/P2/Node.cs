using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class Node {

    public Vector3 pos;
    public Vector3 vel;
    public Vector3 force;
    public bool _fixed;
    public bool g_enabled = true;
    public float coef;
    public float wind_factor = 3f;
    private tetraEdroGenerator massSprClth;

    // Use this for initialization
    // Primer paso de inicializacion es el awake 
    // Lo que se tiene que inicializar primero lo pongo aquí, y lo que depende de que esto esté inicializado lo pongo en el start
    private void Awake()
    {
        //pos = transform.position;
    }


    public Node(Vector3 v, tetraEdroGenerator massSpringCloth) { // Constructor dado un vertice (Vec3) y la referencia de la clase mspc
        pos = v;
        vel = Vector3.zero;
        force = Vector3.zero;
        _fixed = false;
        massSprClth = massSpringCloth;
    }
    public void ComputeForces()
    {
        // Aplicar la fuerza de la gravedad sobre el nodo
        Debug.Log("Gravedad es "+massSprClth.Gravity);
        force += massSprClth.mass * massSprClth.Gravity;
        Vector3 roz_viento = -(massSprClth.mass*wind_factor) * vel;
        force += roz_viento;
        if (coef>0) {    // Si esta colisionando la fuerza hacia abajo se setea a 0
            force = (-force* coef);
            vel = (-vel * coef);
            // Habria que buscar la normal al objeto con el que colisiona y ejercer una fuerza contraria
            // parecida a la que traiga el nodo
        }
        
    }

}
