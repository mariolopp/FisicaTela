using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class Spring {

    public Node nodeA, nodeB;

    public float Length0;
    public float Length;
    public Vector3 position;
    public float stiffness;
    public float mass;
    public float d;
    public float amortigua_factor = 0.005f;
    public Quaternion rotation;

    public Spring(Node nodeA, Node nodeB, tetraEdroGenerator mspc)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
        stiffness = mspc.stiffness;
        mass = mspc.mass;
        d = stiffness / mass;
        // Guardar una longitud inicial para tener como referencia
        UpdateLength();
        Length0 = Length;
    }
    // Use this for initialization
    //public void Start () {
    //    UpdateLength();
    //    Length0 = Length;
    //}

    // Update is called once per frame
    //public void Update () {
    //       // Queremos coger el vector unityario en la direccion y y queremos ponerlo en la posicion u
    //       localScale = new Vector3(localScale.x, Length / 2.0f, localScale.z);  // Escalando el cilindro en funcion de la longitud que tiene / 2 para que se pueda pinter con mas longitud mas tarde
    //       position = 0.5f * (nodeA.pos + nodeB.pos);    // Porque para pintar el cilindro se hace justo entre los dos nodos


    //       Vector3 u = nodeA.pos - nodeB.pos;
    //       u.Normalize();
    //       Quaternion rotation = Quaternion.FromToRotation(Vector3.up, u); // Esta funcion nos permite rotar el vector en direccion 'y' a la direccion 'u'
    //   }

    public void UpdateLength ()
    {
        Length = (nodeA.pos - nodeB.pos).magnitude; // Tamaño del vector que une las posiciones de los dos nodos
    }

    public void ComputeForces()
    {
        Vector3 u = nodeA.pos - nodeB.pos;      // Vector que une las posiciones de los dos nodos
        u.Normalize();
        
        // Tamanyo del vector en la dirección del vector unnitario
        Vector3 amortiguamiento = -(stiffness*amortigua_factor  ) * (Vector3.Dot(nodeA.vel - nodeB.vel, u))*u;  // −𝑑 𝑢 ⋅ 𝑣𝑎 − 𝑣𝑏 𝑢 
        
        Vector3 force = (-stiffness) * (Length - Length0) * u + amortiguamiento;   // Formula de la fuerza elástica
        nodeA.force += force;
        nodeB.force -= force;

    }
}
