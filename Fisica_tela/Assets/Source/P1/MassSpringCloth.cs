using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;
using UnityEditor.Experimental.GraphView;
using System;

public class MassSpringCloth : MonoBehaviour
{
    /// <summary>
    /// Default constructor. Zero all. 
    /// </summary>
    public MassSpringCloth()
    {
        this.Paused = true;
        this.TimeStep = 0.01f;
        this.Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        this.IntegrationMethod = Integration.Symplectic;
    }

    /// <summary>
    /// Integration method.
    /// </summary>
    public enum Integration
    {
        Explicit = 0,
        Symplectic = 1,
    };

    #region InEditorVariables

    public bool Paused;
    public float TimeStep;
    public Vector3 Gravity;
    public float stiffness = 50f; //asignar a los muelles
    public float mass = 1.0f;
    public Integration IntegrationMethod;
    public List<Node> nodeList;        // Lista de nodos
    public List<Spring> springList; // Lista de muelles
    

    #endregion

    #region OtherVariables
    Vector3 pos;
    #endregion

    #region MonoBehaviour
    public void Awake()
    {
        nodeList = new List<Node>();
        springList = new List<Spring>();
        Paused = false;
        // Malla asociada al game Object
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;

        // Vertices y triangulos de la malla
        Vector3[] vertices = mesh.vertices;     // Guarda la coordenada local del vertice
        int[] triangles = mesh.triangles;       // Guarda los indices de los vertices de un triangulo bajo un identificador de triangulo

        
        // Transformar la posición del primer vértice a coordenadas globales

        foreach (Vector3 vertex in vertices)
        {

            // Crear un nuevo nodo con la posición del vértice
            Node nodo = new Node(transform.TransformPoint(vertex), this);       // Utiliza un constructor que usa la referencia vertex y mspc

            // Agregar el nodo a la lista de nodos
            nodeList.Add(nodo);
            //print("creado nodo en la pos "+nodo.pos);

        }
        for (int i = 0; i <= 20; i++) {
            nodeList[i]._fixed = true;
        }
        

        for (int index = 0; index < triangles.Length-1; index+=3)
        {

            int index1 = triangles[index];
            int index2 = triangles[index+1];
            int index3 = triangles[index+2];

            // Anyadir a los muelles cada par de nodos ya en coordenadas globales
            springList.Add(new Spring(nodeList[index1], nodeList[index2], this));
            springList.Add(new Spring(nodeList[index1], nodeList[index3], this));
            springList.Add(new Spring(nodeList[index2], nodeList[index3], this));

            //springList.Add(new Spring(nodeList[index2], nodeList[index1], this));
            //springList.Add(new Spring(nodeList[index2], nodeList[index3], this));

            //springList.Add(new Spring(nodeList[index3], nodeList[index1], this));
            //springList.Add(new Spring(nodeList[index3], nodeList[index2], this));

            //print("El primer nodo del spring es " + spring.nodeA.pos);
            //print("El segundo nodo del spring es " + spring.nodeB.pos);
        }
                   
    }

    public void Update()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = new Vector3[mesh.vertexCount];


        // Refrescar la geometria igualando al array
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            // Convertir el vertice a coordenadas locales para devolver la actualización a la malla
            vertices[i] = transform.InverseTransformPoint(nodeList[i].pos);

        }
        //foreach (Node n in nodeList) { 
        //    transform.TransformPoint(n.pos);
        //}


        mesh.vertices = vertices;



        

        //print("El vertice de la malla se ha movido a " + mesh.vertices[1]);
    }

    public void FixedUpdate()
    {
        if (this.Paused)
            return; // Not simulating

        // Select integration method
        switch (this.IntegrationMethod)
        {
            case Integration.Explicit: this.stepExplicit(); break;
            case Integration.Symplectic: this.stepSymplectic(); break;
            default:
                throw new System.Exception("[ERROR] Should never happen!");
        }

    }

    #endregion

    /// <summary>
    /// Performs a simulation step in 1D using Explicit integration.
    /// </summary>
    private void stepExplicit()
    {
    }

    /// <summary>
    /// Performs a simulation step in 1D using Symplectic integration.
    /// </summary>
    private void stepSymplectic()       // Metodo de integracion
    {
        foreach (Node n in nodeList)
        {     // Recorremos los nodos existentes en la lista
            if (!n._fixed)
            {
                // Calcular la fuerza sobre los nodos
                n.force = Vector3.zero;       // Resetea la fuerza de la particula
                n.ComputeForces();            // Calcula su propia nueva fuerza
            }
            //else if (n.pos){ 
                // Si un nodo esta dentro de un objeto de tipo Fixed,
                // este actuacizará su posición acorde a como se mueva este objeto


            //}
        }
        foreach (Spring spring in springList)
        {
            spring.ComputeForces();     // Cada uno de los objetos que conoce unas ciertas cualidades es capaz de calcular las fuerzas
        }
        foreach (Node n in nodeList)
        {
            if (!n._fixed)
            {
                // Calcular la aceleracion
                n.vel = n.vel + ((TimeStep / mass) * n.force);     // Calcular velocidades
                n.pos += TimeStep * n.vel;                      // Calcular posicion
            }
        }
        foreach (Spring spring in springList)
        {
            spring.UpdateLength();
        }
        //print("nodo en la pos " + nodes[1].pos);
    }

}
