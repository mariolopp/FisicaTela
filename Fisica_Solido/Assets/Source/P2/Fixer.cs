using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Fixer : MonoBehaviour
{
    // Start is called before the first frame update
    //GameObject[] objectsWithTag;
    GameObject[] cloths;
    public tetraEdroGenerator cloth;
    List<Node> nodes;
    Vector3[] localPos;     // Coordenadas locales de los vertices respecto al fixer
    bool[] nodesInside;        // Guarda si el node estaba inicialmente dentro del fixer
    private void Awake()
    {
        
    }
    void Start()
    {
        cloths = GameObject.FindGameObjectsWithTag("Cloth");
        if (cloths != null)
        {
            cloth = cloths[0].GetComponent<tetraEdroGenerator>();
            
            nodes = cloth.nodeList;      // Referencia a la lista de nodos de la prenda
            localPos = new Vector3[nodes.Count];  // inicializamos a la cantidad de nodos
            int a = 0;
            foreach (Node n in nodes)
            {
                localPos[a] = transform.InverseTransformPoint(n.pos); // transformar a coordenadas locales del fixer la coord del vertice
                a++;
            }
        }
        nodesInside = new bool[nodes.Count];       
        Bounds bounds = GetComponent<Collider>().bounds;    // Obtener el collider del objeto fixed
        
        int i = 0;
        foreach (Node n in nodes)
        {
            bool isInside = bounds.Contains(n.pos);     // Comprobar si una posición de vertice en coord globales está dentro del collider
            if (isInside)
            {
                n._fixed = true;        // Si el vertice está agarrado las fisicas no le afectan

                Vector3 globalPos = transform.TransformPoint(localPos[i]);    // aplicar la transformación que haya tenido el fixer al vertice
                //print(n.pos);
                n.pos = globalPos;                                            // Aplicar las coordenadas globales aplicadas a dicho vertice
                //print("new " + n.pos);
                isInside = true;
                nodesInside[i] = true;
            }
            else
            {
                n._fixed = false;       // Si el vertice no esta agarrado las fisicas le afectan
            }
            i++;
        }

    }

    // Update is called once per frame
    void Update()
    {

        Bounds bounds = GetComponent<Collider>().bounds;    // Obtener el collider del objeto fixed

        int i = 0;
        foreach (Node n in nodes)
        {
            if (nodesInside[i])             // Si los vertices estaban en su posicion inicial dentro del fixer, estos se moveran con el
            {
                //n._fixed = true;        // Si el vertice está agarrado las fisicas no le afectan
                
                Vector3 globalPos = transform.TransformPoint(localPos[i]);    // aplicar la transformación que haya tenido el fixer al vertice
                //print(n.pos);
                n.pos = globalPos;                                         // Aplicar las coordenadas globales aplicadas a dicho vertice
                //print("new " + n.pos);
            }
            else
            {
                //n._fixed = false; // Si el vertice no esta agarrado las fisicas le afectan
            }
            i++;
        }
    }
}
