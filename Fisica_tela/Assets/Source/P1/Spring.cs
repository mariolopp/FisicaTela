using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MassSpringCloth {

    public Node nodeA, nodeB;

    public float Length0;
    public float Length;

    public float stiffness;

    public Spring(Node nodeA, Node nodeB)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
    }
    // Use this for initialization
    void Start () {
        UpdateLength();
        Length0 = Length;
    }
	
	// Update is called once per frame
	void Update () {
        // Queremos coger el vector unityario en la direccion y y queremos ponerlo en la posicion u
        transform.localScale = new Vector3(transform.localScale.x, Length / 2.0f, transform.localScale.z);  // Escalando el cilindro en funcion de la longitud que tiene / 2 para que se pueda pinter con mas longitud mas tarde
        transform.position = 0.5f * (nodeA.pos + nodeB.pos);    // Porque para pintar el cilindro se hace justo entre los dos nodos


        Vector3 u = nodeA.pos - nodeB.pos;
        u.Normalize();
        transform.rotation = Quaternion.FromToRotation(Vector3.up, u); // Esta funcion nos permite rotar el vector en direccion 'y' a la direccion 'u'
    }

    public void UpdateLength ()
    {
        Length = (nodeA.pos - nodeB.pos).magnitude; // Tamaño del vector que une las posiciones de los dos nodos
    }

    public void ComputeForces()
    {
        Vector3 u = nodeA.pos - nodeB.pos;      // Vector que une las posiciones de los dos nodos
        u.Normalize();
        Vector3 force = - stiffness * (Length - Length0) * u;   // Formula de la fuerza elástica
        nodeA.force += force;
        nodeB.force -= force;
    }
}
