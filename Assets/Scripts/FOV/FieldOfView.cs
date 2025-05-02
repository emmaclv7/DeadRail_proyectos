using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
 ############################################################################################################################
 * Estimado programador valiente que osas aventurarte en estas líneas de código:
 
 * Cuando escribí este código, sólo dos entidades en este vasto universo comprendían su propósito y funcionamiento:
   Dios… y yo.
 
 * Hoy, sólo Dios conserva ese conocimiento.
 
 * Esta función, este fragmento de lógica arcana, es el vestigio de una batalla olvidada entre hombre y máquina.
 * Fue escrita en medio del caos, entre cafés fríos, noches sin luna y commits sin sentido.
 * Su estructura desafía la razón, su lógica se burla de toda convención.
 * Sin embargo, funcionaba. Y eso bastaba.
 
 * Si, por alguna extraña razón, has decidido optimizar esto (o peor aún, comprenderlo) te ruego: ten piedad de ti mismo.
 * Si fallas (y es muy probable que lo hagas), no te sientas derrotado.
 * No eres el primero, ni serás el último en caer ante esta bestia indomable.
 
 * Simplemente, suma tu sacrificio a los que vinieron antes. Deja una advertencia. Añade tu nombre al panteón de los caídos.
 
 * total_horas_perdidas_aqui = 12; // Aumenta este número con orgullo si has sufrido.
 ############################################################################################################################
 */


public class FieldOfView : MonoBehaviour
{
    public float viewAngle = 90f; //El angulo de vision (en cono) por grados
    public float viewDistance = 8f; //Distancia de la vision
    public int rayCount = 60; //Número de rallos lanzados para formar lo que se ve en el campo de vision
    public LayerMask obstacleMask; //Máscara de capas
    public LayerMask groundMask; //Máscara de capas

    private Mesh viewMesh; //Malla que se encarga de representar el campo de visión visualmente
    private Vector3 origin; //Posición donde se va a mover el rango de visión
    private float startingAngle; //Angulo incial que empieza el rango de vision en un inicio

    void Start()
    {
        viewMesh = new Mesh(); //Se crea una nueva malla
        GetComponent<MeshFilter>().mesh = viewMesh; //Se asigna la nueva malla al componente MeshFilter
    }

    void Update()
    {
        DrawFieldOfView(); //Se actualiza la malla del FOV en cada frame
    }

    //Método donde se actualiza la Posición de donde esta el rango de visi´on
    public void SetOrigin(Vector3 newOrigin)
    {
        origin = newOrigin;
    }

    //Método donde se acttualiza la dirección de donde se mira
    public void SetAimDirection(Vector3 direction)
    {
        direction.Normalize(); //Normalizamos el vector de dirección
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //Formúla donde se convierte la dirección del ángulo en grados
        startingAngle = angle + (viewAngle / 2f);  //Cálculo de los ángulos desde que se inicializan los rayos
    }

    //Método donde se genera la visualización de la malla en el campo de visión
    void DrawFieldOfView()
    {
        float angleStep = viewAngle / rayCount; //Cálculo el paso del ángulo por cada rayo que hay
        Vector3[] vertices = new Vector3[rayCount + 1]; //Creamos una array de vértices dónde el primero es el origen y todo lo demas son las puntas de cada rayo
        int[] triangles = new int[rayCount * 3]; //Creamos un array de triángulos para dibujar el cono que formará el rango de visión 

        vertices[0] = origin; //Vértice inicial donde es el origen del field of view

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i < rayCount; i++) //Se lanzan cada uno de los rayos
        {
            float angle = startingAngle - angleStep * i; //Se calculo el ángulo del rayo
            Vector3 dir = GetDirectionFromAngle(angle); //Ese ángulo se convierte en una dirección
            Vector3 vertex; //el punto final del rayo

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, viewDistance, obstacleMask | groundMask); //Lanzamos un raycast por si se choca con algo
            vertex = (hit.collider != null) ? hit.point : origin + dir * viewDistance; // si colisiona usamos el punto de impacto y sino el punto más lejano de la dirección

            vertices[vertexIndex] = vertex; //Guardamos el vértice en el array

            if (i > 0) //Empezamos a formar tríangulos
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = vertexIndex - 1;
                triangles[triangleIndex++] = vertexIndex;
            }

            vertexIndex++;
        }

        //conectamos el anterior triangulo con el primer vértice para cerrar el rango de visión
        triangles[triangleIndex++] = 0;
        triangles[triangleIndex++] = rayCount;
        triangles[triangleIndex++] = 1;

        //Limpiamos y actualizamos la malla con los nuevos triángulos y vertices generados
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    //Método donde convierte el ángulo en grados hasta un vector de dirección
    Vector3 GetDirectionFromAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}