using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using System.Reflection;

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
    public float stiffness_traccion;
    public float stiffness_flexion;
    //public List<int[]> aristas;       // Array de arrays de 3 ints
    struct Edges {  // Estructura de una arista
        // Arista base definida por dos vertices, que se ordenan de menor a mayor
        int vertex1;
        int vertex2;
        //public Edges(int v1, int v2) {
        //    if (v1 < v2)
        //    {
        //        vertex1 = v2;
        //        vertex1 = v1;
        //    }
        //    else {
        //        vertex1 = v2;
        //        vertex2 = v1;
        //    }
        //}
    
    
    }
    #endregion

    #region OtherVariables
    Vector3 pos;
    #endregion

    #region MonoBehaviour
    public void Awake()
    {
        nodeList = new List<Node>();
        springList = new List<Spring>();
        //aristas = new List<int[]>();  // Guarda vertices asociados a un triangulo ordenados de menor a mayor
        
        Paused = false;
        // Malla asociada al game Object
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;

        // Vertices y triangulos de la malla
        Vector3[] vertices = mesh.vertices;     // Guarda la coordenada local del vertice
        int[] triangles = mesh.triangles;       // Guarda los indices de los vertices de un triangulo bajo un identificador de triangulo
        //tr_ord = new int[triangles.Length*3];

        // Transformar la posición del primer vértice a coordenadas globales

        foreach (Vector3 vertex in vertices)
        {

            // Crear un nuevo nodo con la posición del vértice
            Node nodo = new Node(transform.TransformPoint(vertex), this);       // Utiliza un constructor que usa la referencia vertex y mspc

            // Agregar el nodo a la lista de nodos
            nodeList.Add(nodo);
            //print("creado nodo en la pos "+nodo.pos);

        }
        //for (int i = 0; i <= 20; i++) {
        //    nodeList[i]._fixed = true;
        //}

        //int ansindex1 = triangles[0];
        //int ansindex2 = triangles[1];
        //int ansindex3 = triangles[2];
        //int[] ansidxs = { ansindex1, ansindex2, ansindex3 };
        //Array.Sort(ansidxs);
        for (int index = 0; index < triangles.Length - 1; index += 3)
        {
            //Debug.Log(triangles[index] + " " + triangles[index + 1] + " " + triangles[index + 1]);
            int index1 = triangles[index];
            int index2 = triangles[index + 1];
            int index3 = triangles[index + 2];
            springList.Add(new Spring(nodeList[index1], nodeList[index2], this));
            springList.Add(new Spring(nodeList[index1], nodeList[index3], this));
            springList.Add(new Spring(nodeList[index2], nodeList[index3], this));

        }
        // Ordenar la lista de triangulos
        for (int i = 0; i < triangles.Length; i = i + 3)   // Recorro triangulos
        {
            //for (int j = 0; j < 3; j++)                     // Recorrer sus aristas
            //{
            //    // Edge auxiliar 
            //    auxEdge = new Edge(triangulos[i + indices[j]], triangulos[i + indices[j + 1]]);
            //    if (!opuesto.ContainsKey(auxEdge))
            //    {
            //        // Añadir spring 
            //        // Añadir spring opuesto
            //        springs.Add(new Spring(nodes[triangulos[i + indices[j]]], nodes[triangulos[i + indices[j + 1]]], StiffnessTraccion));
            //        opuesto.Add(auxEdge, triangulos[i + indices[j + 2]]);

            //    }
            //    else
            //    {
            //        springs.Add(new Spring(nodes[opuesto[auxEdge]], nodes[triangulos[i + indices[j + 2]]], StiffnessFlexion));
            //    }
            //}
        }



        //        for (int index = 0; index < triangles.Length-1; index+=3)
        //{
        //    //Debug.Log(triangles[index] + " " + triangles[index+1] + " " + triangles[index+1]);

        //}
                   
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
//if (a[0] == idxs[0] & a[1] == idxs[1])
//{
//    int[] arista = { a[2], idxs[2] };
//    Array.Sort(arista);
//    // Introducimos una arista con los vertices opuestos
//    springList.Add(new Spring(nodeList[arista[0]], nodeList[arista[1]], this));
//    existe = true;
//}

//int index1 = triangles[index];
//int index2 = triangles[index + 1];
//int index3 = triangles[index + 2];
//Debug.Log(index1 + " " + index2 + " " + index3);
//// Indices de nodo
//int[] idxs = { index1, index2, index3 };
//Array.Sort(idxs);        // Ordenar los indices de menor a mayor
//                         //Vector3 i_ord = new Vector3(indexes[0], indexes[1], indexes[2]);
//                         // Anyadir a los muelles cada par de nodos ya en coordenadas globales

//bool existe = false;
//// Si existe un triangulo que este usando la misma arista que la que queríamos introducir..    

//if (existe)
//{
//    springList.Add(new Spring(nodeList[idxs[]], nodeList[index2], this));
//    springList.Add(new Spring(nodeList[index1], nodeList[index3], this));
//}
//else
//{
//    springList.Add(new Spring(nodeList[index1], nodeList[index2], this));
//    springList.Add(new Spring(nodeList[index1], nodeList[index3], this));
//    springList.Add(new Spring(nodeList[index2], nodeList[index3], this));
//}



//springList.Add(new Spring(nodeList[index2], nodeList[index1], this));
//springList.Add(new Spring(nodeList[index2], nodeList[index3], this));

//springList.Add(new Spring(nodeList[index3], nodeList[index1], this));
//springList.Add(new Spring(nodeList[index3], nodeList[index2], this));

//print("El primer nodo del spring es " + spring.nodeA.pos);
//print("El segundo nodo del spring es " + spring.nodeB.pos);