using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System;
using Unity.VisualScripting;

public class tetraEdroGenerator : MonoBehaviour
{
    #region InEditorVariables

    public bool Paused;
    public float TimeStep;
    public Vector3 Gravity;
    public float stiffness = 50f; //asignar a los muelles
    public float mass = 5.0f;
    public Integration IntegrationMethod;
    public List<Node> nodeList;         // Lista de nodos
    public List<Spring> springList;     // Lista de muelles
    public float stiffness_traccion;
    public float stiffness_flexion;
    //public List<int[]> aristas;       // Array de arrays de 3 ints
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

    #region meshVariables
    public TextAsset fileName;
    public TextAsset text_tetraedros;
    public Mesh tetra;
    public MeshFilter tetraFilter;
    Vector3[] vertices;             // Array de vertices de la malla low poly
    public GameObject submarine;
    Vector3[] sub_vertex;           // Array de vertices de la malla high poly
    public int[] hPolyList;         // Guarda en que tetraedro esta cada vertice del modelo,
                                    // siendo en indice el vertice y el contenido el n� de tetraedro
    public int[][] tetraedros;      // Array de arrays de integers [indexTetraedro][0-3 id vertices]
    public Vector4[] bar_coords;    // Coordenadas baricentricas de cada vertice high poly
    Mesh sub;
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
        // Malla del objeto high poly
        sub = submarine.GetComponentInChildren<MeshFilter>().mesh;


        string[] textString = fileName.text.Split(new string[] { " ", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        string[] tetraString = text_tetraedros.text.Split(new string[] { " ", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

        int numNodes = int.Parse(textString[0]);
        //int numCoord = int.Parse(textString[1]);

        int numTetraedros = int.Parse(tetraString[0]);

        tetraedros = new int[numTetraedros][];
        hPolyList = new int[sub.vertexCount];
        bar_coords = new Vector4[sub.vertexCount];

        // Para que distinga el .decimal
        CultureInfo locale = new CultureInfo("en-US");
        float valor = float.Parse("1.425", locale);

        tetra = new Mesh();
        // Vertices y triangulos de la malla
        vertices = new Vector3[numNodes];
        int[] triangles = new int[numTetraedros * 12];       // Guarda los indices de los vertices de un triangulo bajo un identificador de triangulo


        // Parser de los vertices
        int index = 0;
        for (int i = 4; i <= (numNodes * 4); i += 4)
        {
            index = int.Parse(textString[i]);
            float vx = float.Parse(textString[i + 1], locale);
            float vy = float.Parse(textString[i + 2], locale);
            float vz = float.Parse(textString[i + 3], locale);
            //print(index);
            vertices[index - 1].x = vx;
            vertices[index - 1].y = vy;
            vertices[index - 1].z = vz;

            //print("vx es " + vx);
            //print(vertices[index-1].y);
            //print(vertices[index-1].z);

        }
        tetra.vertices = vertices;      // Vertices a la malla en coordenadas locales

        // Parser de triangulos de cada tetraedro
        int numTetra = 0;
        for (int i = 3; i <= (numTetraedros * 5); i += 5)
        {
            index = int.Parse(tetraString[i]);                  // Indice de tetraedro
            int t1 = int.Parse(tetraString[i + 1], locale);     // Indices de nodos del tetraedro
            int t2 = int.Parse(tetraString[i + 2], locale);
            int t3 = int.Parse(tetraString[i + 3], locale);
            int t4 = int.Parse(tetraString[i + 4], locale);

            triangles[numTetra] = t1 - 1;
            triangles[numTetra + 1] = t2 - 1;
            triangles[numTetra + 2] = t3 - 1;

            numTetra += 3;
            triangles[numTetra] = t1 - 1;
            triangles[numTetra + 1] = t2 - 1;
            triangles[numTetra + 2] = t4 - 1;

            numTetra += 3;
            triangles[numTetra] = t1 - 1;
            triangles[numTetra + 1] = t3 - 1;
            triangles[numTetra + 2] = t4 - 1;

            numTetra += 3;
            triangles[numTetra] = t2 - 1;
            triangles[numTetra + 1] = t3 - 1;
            triangles[numTetra + 2] = t4 - 1;

            // Inicializar array de tetraedros formado de indices que contienen un array 
            // con los ids de los vertices de los que el tetraedro esta formado
            //print("triangulo " + index);
            //print(t1);
            //print(t2);
            //print(t3);
            //print(t4);

            Vector3 p1 = vertices[t1-1];
            Vector3 p2 = vertices[t2 - 1];
            Vector3 p3 = vertices[t3 - 1];
            Vector3 p4 = vertices[t4 - 1];

            //print("t1: "+p1);
            //print("t2: " + p2);
            //print("t3: " + p3);
            //print("t4: " + p4);


            //// Calcular el producto vectorial
            Vector3 crossProduct = Vector3.Cross(p2 - p1, p3 - p1);

            //// Calcular el producto escalar con (p4 - p1)
            float dotProduct = Vector3.Dot(crossProduct, p4 - p1);

            //print(dotProduct);

            //// Si el producto escalar es negativo, intercambiar dos v�rtices para cambiar la orientaci�n
            if (dotProduct < 0)
            {
                print("Vertices intercambiados");
                print(t2 + t3);
                // Intercambiar p2 y p3
                int temp = t2;
                t2 = t3;
                t3 = temp;
                print(t2 + t3);
            }

            // PUEDE QUE TENGA QUE MODIFICAR EL ORDEN EN QUE SE GUARDAN
            tetraedros[index - 1] = new int[] { t1 - 1, t2 - 1, t3 - 1, t4 - 1 };
            //Debug.Log(tetraedros[index - 1][0]);
            //if ((t1 < 0) || (t2 < 0) || (t3 < 0) || (t4 < 0)) { print("ERROR"); }
        }
        tetra.triangles = triangles;    // Lista de indices de vertices para que la malla sepa unir los vertices

        tetraFilter = gameObject.AddComponent<MeshFilter>();
        tetraFilter.mesh = tetra;

        tetra.RecalculateNormals();     // Calcula las normales basandose en la lista de indices triangles
        tetra.RecalculateBounds();      // Calcula los limites del objeto

        // Crear los nodos
        foreach (Vector3 vertex in vertices)
        {

            // Crear un nuevo nodo con la posici�n del v�rtice
            Node nodo = new Node(transform.TransformPoint(vertex), this);       // Utiliza un constructor que usa la referencia vertex y mspc

            // Agregar el nodo a la lista de nodos
            nodeList.Add(nodo);
            //print("creado nodo en la pos "+nodo.pos);

        }
        //for (int i = 0; i <= 20; i++) {
        //    nodeList[i]._fixed = true;
        //}

        // Crear los muelles
        for (index = 0; index < triangles.Length - 1; index += 3)
        {
            //Debug.Log(triangles[index] + " " + triangles[index + 1] + " " + triangles[index + 1]);
            int index1 = triangles[index];
            int index2 = triangles[index + 1];
            int index3 = triangles[index + 2];
            springList.Add(new Spring(nodeList[index1], nodeList[index2], this));
            springList.Add(new Spring(nodeList[index1], nodeList[index3], this));
            springList.Add(new Spring(nodeList[index2], nodeList[index3], this));

        }
        


        // Una vez tenemos todos los vertices y aristas establecidos procedemos a comprobar que vertices del obj
        // pertenecen a cada tetraedro

        // Recorrer la malla de vertices high poly
        sub_vertex = sub.vertices;
        print("Se buscan " + sub.vertexCount);
        for (int i = 0; i < sub_vertex.Length; i++)       // Recorre todos los vertices high poly
        {
            // Vertice buscado
            Vector3 pSearch = sub_vertex[i];
            for (int j = 0; j < tetraedros.Length; j++) {   // Comprueba para cada uno a que tetraedro pertenece

                // Vertices del tetraedro
                Vector3 v0 = vertices[tetraedros[j][0]];
                Vector3 v1 = vertices[tetraedros[j][1]];
                Vector3 v2 = vertices[tetraedros[j][2]];
                Vector3 v3 = vertices[tetraedros[j][3]];

                // Volumenes
                float volumenTotal = volumenTetra(v0, v1, v2, v3);  // Volumen del tetraedro
                float vol3 = volumenTetra(v0, v1, v2, pSearch);
                float vol2 = volumenTetra(v0, v1, pSearch, v3);
                float vol1 = volumenTetra(v0, pSearch, v2, v3);
                float vol0 = volumenTetra(pSearch, v1, v2, v3);

                //float vol3 = volumenTetra(pSearch, v0, v1, v2);
                //float vol2 = volumenTetra(pSearch, v0, v1, v3);
                //float vol1 = volumenTetra(pSearch, v0, v2, v3);
                //float vol0 = volumenTetra(pSearch, v1, v2, v3);

                // return Vector3.Dot(Vector3.Cross((p2 - p1), (p3 - p1)), (p4 - p1));
                
                float peso0 = vol0 / volumenTotal;
                float peso1 = vol1 / volumenTotal;
                float peso2 = vol2 / volumenTotal;
                float peso3 = vol3 / volumenTotal;

                if ((peso0 > 0) && (peso0 <= 1) &&
                    (peso1 > 0) && (peso1 <= 1) &&
                    (peso2 > 0) && (peso2 <= 1) &&
                    (peso3 > 0) && (peso3 <= 1))   // Si no hay volumenes negativos
                {
                    // El punto esta dentro de dicho tetraedro
                    hPolyList[i] = j; // Guarda el indice de tetraedro que contiene al vertice
                    bar_coords[i] = new Vector4(peso0, peso1, peso2, peso3);
                    print(bar_coords[i]);
                    break;
                }
                else {
                    //print("no"+peso0+peso1+peso2+peso3);
                }
                

                
            }

        }


        //for (int i = 0; i < sub_vertex.Length; i++) {
        //    sub_vertex[i] += new Vector3(1.0f, 1f, 1f);
        //}
        //sub.vertices = sub_vertex;
        //sub.RecalculateNormals();     // Calcula las normales basandose en la lista de indices triangles
        //sub.RecalculateBounds();      // Calcula los limites del objeto

    }
    // Update is called once per frame
    public void Update()
    {
        // vertices = new Vector3[tetra.vertexCount];

        //print(tetra.vertexCount);
        // Refrescar la geometria igualando al array
        for (int i = 0; i < tetra.vertexCount; i++)
        {
            // Convertir el vertice a coordenadas locales para devolver la actualizaci�n a la malla
            vertices[i] = transform.InverseTransformPoint(nodeList[i].pos);
            //print("nodo " + i + nodeList[i].pos);

        }
        //foreach (Node n in nodeList) { 
        //    transform.TransformPoint(n.pos);
        //}
        tetra.vertices = vertices;
        //print("El vertice de la malla se ha movido a " + mesh.vertices[1]);

        for (int i = 0; i < sub_vertex.Length; i++) {       // Recorrer todos los vertices malla high poly
            // Para cada vertice del high poly, aplico la modificaci�n que haya sufrido su tetraedro
            int tIndex = hPolyList[i];          // indice de tetraedro asociado al vertice
            Vector4 bc = bar_coords[i];         // Coordenadas baricentricas del vertice

            // Ids de los vertices del tetraedro
            int id0 = tetraedros[tIndex][0];
            int id1 = tetraedros[tIndex][1];
            int id2 = tetraedros[tIndex][2];
            int id3 = tetraedros[tIndex][3];

            // Coordenadas de los vertices del tetraedro
            Vector3 v0 = vertices[id0];
            Vector3 v1 = vertices[id1];
            Vector3 v2 = vertices[id2];
            Vector3 v3 = vertices[id3];

            //print(bc.x);
            //print(bc.y);
            //print(bc.z);
            //print(bc.w);

            Vector3 newCoord = bc.x * v0 + bc.y * v1 + bc.z * v2 + bc.w * v3;
            //print(newCoord);
            sub_vertex[i] = newCoord;   // Vertices que nos interesa cambiar
            
        }
        sub.vertices = sub_vertex;
        
    }
    private float volumenTetra(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
        return Vector3.Dot(Vector3.Cross((p2 - p1), (p3 - p1)), (p4-p1));
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
            // este actuacizar� su posici�n acorde a como se mueva este objeto
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
    private void stepExplicit()
    {
        print("explicit");
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
