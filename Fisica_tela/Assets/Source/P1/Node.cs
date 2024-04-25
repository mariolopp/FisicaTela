using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Node {

    public Vector3 pos;
    public Vector3 vel;
    public Vector3 force;
    public bool _fixed;
    public float mass;

    // Use this for initialization
    // Primer paso de inicializacion es el awake 
    // Lo que se tiene que inicializar primero lo pongo aquí, y lo que depende de que esto esté inicializado lo pongo en el start
    private void Awake()
    {
        //pos = transform.position;
        
    }
    public Node(Vector3 v) { // Constructor dado un vertice (Vec3)
        pos = v;
        vel = Vector3.zero;
        force = Vector3.zero;
        mass = 1f;
        _fixed = false;
    }
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position = pos;
	}

    public void ComputeForces()
    {
        //force += mass * transform.parent.GetComponent<MassSpringCloth>().Gravity;
    }
}
