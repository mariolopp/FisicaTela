using UnityEngine;
using System.Collections.Generic;


public class tetraEdroGenerator : MonoBehaviour
{
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
    public Mesh tetra;
    public MeshFilter tetraFilter;
    struct Edges
    {  // Estructura de una arista
        // Arista base definida por dos vertices, que se ordenan de menor a mayor
        int vertex1;
        int vertex2;
    }
    public enum Integration
    {
        Explicit = 0,
        Symplectic = 1,
    };
    #endregion

    public void Awake() {
        nodeList = new List<Node>();
        springList = new List<Spring>();
        //aristas = new List<int[]>();  // Guarda vertices asociados a un triangulo ordenados de menor a mayor

        Paused = false;
        TimeStep = 0.01f;
        Gravity = new Vector3(0.0f, -9.81f, 0.0f);

        Paused = false;
        // Malla asociada al game Object
        tetra = new Mesh();
        


        // Vertices y triangulos de la malla
        Vector3[] vertices = new Vector3[] {
            new Vector3(1, 1, 1),   // Vértice 0
            new Vector3(-1, -1, 1), // Vértice 1
            new Vector3(-1, 1, -1), // Vértice 2
            new Vector3(1, -1, -1)  // Vértice 3
        };     // Guarda la coordenada local del vertice


        int[] triangles = new int[]
        {
            0, 1, 2,  // Triángulo 1 (Vértices 0, 1, 2)
            0, 3, 1,  // Triángulo 2 (Vértices 0, 3, 1)
            0, 2, 3,  // Triángulo 3 (Vértices 0, 2, 3)
            1, 3, 2   // Triángulo 4 (Vértices 1, 3, 2)

        };       // Guarda los indices de los vertices de un triangulo bajo un identificador de triangulo

        tetra.vertices = vertices;      // Vertices a la malla en coordenadas locales
        tetra.triangles = triangles;    // Lista de indices de vertices para que la malla sepa unir los vertices

        tetraFilter = gameObject.AddComponent<MeshFilter>();
        tetraFilter.mesh = tetra;

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
        
        tetra.RecalculateNormals();     // Calcula las normales basandose en la lista de indices triangles
        tetra.RecalculateBounds();      // Calcula los limites del objeto
    }
    // Update is called once per frame
    public void Update()
    {
        Vector3[] vertices = new Vector3[tetra.vertexCount];

        //print(tetra.vertexCount);
        // Refrescar la geometria igualando al array
        for (int i = 0; i < tetra.vertexCount; i++)
        {
            // Convertir el vertice a coordenadas locales para devolver la actualización a la malla
            vertices[i] = transform.InverseTransformPoint(nodeList[i].pos);
            //print("nodo " + i + nodeList[i].pos);

        }
        //foreach (Node n in nodeList) { 
        //    transform.TransformPoint(n.pos);
        //}
        tetra.vertices = vertices;

        //print("El vertice de la malla se ha movido a " + mesh.vertices[1]);
    }

    public void FixedUpdate()
    {
        if (this.Paused)
            return; // Not simulating
        // Select integration method
        this.stepSymplectic();

    }
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
        //print("nodo en la pos " + nodeList[1].pos);

    }
    void OnDrawGizmos()
    {
        if (tetra != null)
        {
            Gizmos.color = Color.blue;

            foreach (Vector3 vertex in tetra.vertices)
            {
                Gizmos.DrawSphere(transform.TransformPoint(vertex), 0.2f);
            }

            Gizmos.color = Color.red;
            for (int index = 0; index < tetra.triangles.Length - 1; index += 3)
            {
                int v1 = tetra.triangles[index];
                int v2 = tetra.triangles[index + 1];
                int v3 = tetra.triangles[index + 2];
                Gizmos.DrawLine(transform.TransformPoint(tetra.vertices[v1]), transform.TransformPoint(tetra.vertices[v2]));
                Gizmos.DrawLine(transform.TransformPoint(tetra.vertices[v2]), transform.TransformPoint(tetra.vertices[v3]));
                Gizmos.DrawLine(transform.TransformPoint(tetra.vertices[v1]), transform.TransformPoint(tetra.vertices[v3]));
            }

        }
    }
}
