using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;
using UnityEditor.Experimental.GraphView;

public class MassSpringCloth : MonoBehaviour 
{
	/// <summary>
	/// Default constructor. Zero all. 
	/// </summary>
	public MassSpringCloth()
	{
		this.Paused = true;
		this.TimeStep = 0.01f;
		this.Gravity = new Vector3 (0.0f, -9.81f, 0.0f);
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
	public Integration IntegrationMethod;
    public List<Node> nodes = new List<Node>();        // Lista de nodos
    public List<Spring> springList; // Lista de muelles

    #endregion

    #region OtherVariables
    Vector3 pos;
    #endregion

    #region MonoBehaviour
    public void Awake()
    {

    }
    public void Start()
    {
        Paused = false;
        // Malla asociada al game Object
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;

        // Vertices y triangulos de la malla
        Vector3[] vertices = mesh.vertices;     // Guarda la coordenada local del vertice
        int[] triangles = mesh.triangles;       // Guarda los indices de los vertices de un triangulo bajo un identificador de triangulo
       

        // Transformar la posición del primer vértice a coordenadas globales
        int i = 0;                      // Índice del primer vértice
        pos = transform.TransformPoint(vertices[i]);

        foreach (Vector3 vertex in vertices)
        {
            // Crear un nuevo nodo con la posición del vértice
            Node nodo = new Node(vertex);       // Creo que tengo que implementar un constructor


            // Agregar el nodo a la lista de nodos
            nodes.Add(nodo);
            print("creado nodo en la pos "+nodo.pos);
            
        }
        nodes[0]._fixed = true;
        
    }

    public void Update()
	{
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = new Vector3[mesh.vertexCount];

        int i = 0;
        // Convertir el vertice a coordenadas locales
        vertices[i] = transform.InverseTransformPoint(pos);

        // Refrescar la geometria igualando al array
        mesh.vertices = vertices;
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
        foreach (Node n in nodes)
        {     // Recorremos los nodos existentes en la lista
            if (!n._fixed)
            {
                // Calcular la fuerza sobre los nodos
                n.force = Vector3.zero;       // Resetea la fuerza de la particula
                n.ComputeForces();            // Calcula su propia nueva fuerza
            }
        }
        foreach (Spring spring in springList)
        {
            spring.ComputeForces();     // Cada uno de los objetos que conoce unas ciertas cualidades es capaz de calcular las fuerzas
        }
        foreach (Node n in nodes)
        {
            if (!n._fixed)
            {
                // Calcular la aceleracion
                n.vel = n.vel + ((TimeStep / n.mass) * n.force);     // Calcular velocidades
                n.pos += TimeStep * n.vel;                      // Calcular posicion
            }
        }
        foreach (Spring spring in springList)
        {
            spring.UpdateLength();
        }
        print("nodo en la pos " + nodes[1].pos);
    }
		
}
