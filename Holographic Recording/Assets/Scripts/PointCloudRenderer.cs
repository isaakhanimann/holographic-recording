using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PointCloudRenderer : MonoBehaviour
{
    Mesh mesh;
    //public MeshRenderer meshRenderer;
    MeshFilter mf;

    //public Transform offset; // Put any gameobject that faciliatates adjusting the origin of the pointcloud in VR. 
    private bool showRecording = true;
    private int runningFrame = -1;
    private string[] files;

    // Start is called before the first frame update
    void Start()
    {
        mf = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh
        {
            // Use 32 bit integer values for the mesh, allows for stupid amount of vertices (2,147,483,647 I think?)
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        mf.mesh = mesh;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    void UpdateMesh(int runningFrame)
    {
        //runningFrame++;
        Debug.Log("Update Mesh");

        Vector3[] positions = readFile(files[runningFrame]);

        Debug.Log("Renderer: points received");
        if (positions == null || positions.Length == 0)
        {
            Debug.Log("Empty array");
            return;
        }
        int size = positions.Length;

        Color[] colours = new Color[size];
        Debug.Log(size);

        for (int n = 0; n < size; n++)
        {
            colours[n] = new Color(1, 1, 1, 1);
        }

        mf.mesh.Clear();

        Mesh mesh2 = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        mesh2.vertices = positions;
        mesh2.colors = colours;
        int[] indices = new int[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            indices[i] = i;
        }

        mesh2.SetIndices(indices, MeshTopology.Points, 0);
        mesh2.RecalculateNormals();
        mf.mesh = mesh2;
        Debug.Log("Renderer: points updated");
    }

    public IEnumerator Play(string recordingName)
    {
        //float timeStart = Time.time;
        string recording_num = recordingName.Replace("AnimationClip", "");
        Debug.Log(string.Format("points_{0}", recording_num));

        files = Directory.GetFiles(Application.persistentDataPath, "*.dat", SearchOption.AllDirectories)
                        .Where(s => s.Contains(string.Format("points_{0}", recording_num)))
                        .OrderBy(s => s).ToArray();
        Debug.Log("Point Cloud files:" + files.Length);

        showRecording = true;

        for (int i = 0; i < files.Length; i++)
        {
            UpdateMesh(i);
            yield return new WaitForSeconds(0.05f);
        }

        mf.mesh.Clear();
    }

    private Vector3[] readFile(string str)
    {
        try
        {
            byte[] bHex = File.ReadAllBytes(str);
            float[] points = new float[bHex.Length / 4];
            Buffer.BlockCopy(bHex, 0, points, 0, points.Length);

            int size = points.Length / 3;
            Vector3[] pcl = new Vector3[size];

            for (int n = 0; n < size; n++)
            {
                pcl[n] = new Vector3(points[n * 3], points[n * 3 + 1], points[n * 3 + 2]);
            }
            return pcl;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Exception caught");
            return new Vector3[0];
        }
    }
}
