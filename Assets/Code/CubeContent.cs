using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class CubeContent : MonoBehaviour
{
    private GameObject container;
    public GameObject[,,] content = new GameObject[3,3,3];

    public float width
    {
        get { return rows*size + Mathf.Max((rows - 1) * delta, 0); }
    }

    [Range(1,3)]
    public int rows = 3;
    public float size = 1.0f;
    public float delta = 0.2f;
    public Color color = Color.white;
    
    // Start is called before the first frame update
    void Start()
    {

        if (!Application.isPlaying)
            return;

        container = new GameObject("container");
        container.transform.parent = transform;
        container.transform.localPosition = new Vector3(-width/2, -width/2, -width/2);
        Vector3 start_pos = transform.position;
        start_pos.y = width / 2;
        transform.position = start_pos;

        ForeachElements(delegate(int row_x, int row_y, int row_z)
        {
            SetCubeColor(row_x, row_y, row_z, color, false);
        });

        //UpdateInterface();
    }
    
    // Update is called once per frame
    void Update()
    {
        GameObject preview = transform.Find("preview") != null ? transform.Find("preview").gameObject : null;
        if (Application.isPlaying)
        {
            if(preview != null)
                Destroy(preview);
            
            return;
        }

        if (preview == null)
        {
            preview = GameObject.CreatePrimitive(PrimitiveType.Cube);
            preview.name = "preview";
            preview.transform.parent = transform;
        }
        
        preview.transform.localScale = new Vector3(width, width, width);
        preview.transform.localPosition = new Vector3(0, width/2, 0);
        
        var tempMaterial = new Material(preview.GetComponent<Renderer>().sharedMaterial);
        tempMaterial.color = color;
        preview.GetComponent<Renderer>().sharedMaterial = tempMaterial;
    }
    
    public void ForeachElements(Action<int, int, int> cb)
    {
        int row_x, row_y, row_z;
        for (row_x = 0; row_x < rows; row_x++)
        {
            for (row_y = 0; row_y < rows; row_y++)
            {
                for (row_z = 0; row_z < rows; row_z++)
                {
                    cb(row_x, row_y, row_z);
                }
            }  
        }
    }
    
    public int GetCubesCount(List<Color> colors)
    {
        int count = 0;
        ForeachElements(delegate(int cube_x, int cube_y, int cube_z)
        {
            if (colors.Contains(GetCubeColor(cube_x, cube_y, cube_z)))
                count++;
        });
        return count;
    }

    List<Vector3> GetListOfCubes(List<Color> colors)
    {
        List<Vector3> cubes = new List<Vector3>();
        ForeachElements(delegate(int cube_x, int cube_y, int cube_z)
        {
            if (colors.Contains(GetCubeColor(cube_x, cube_y, cube_z)))
                cubes.Add(new Vector3(cube_x, cube_y, cube_z));
        });
        return cubes;
    }

    bool GetNextCubePosition(List<Color> colors, out int row_x, out int row_y, out int row_z)
    {
        List<Vector3> cubes = GetListOfCubes(colors);
        if (cubes.Count == 0)
        {
            row_x = -1;
            row_y = -1;
            row_z = -1;
            return false;
        }

        Vector3 next_cube = cubes[0];
        row_x = (int) next_cube.x;
        row_y = (int) next_cube.y;
        row_z = (int) next_cube.z;
        return true;
    }
    
    bool GetRandomCubePosition(List<Color> colors, out int row_x, out int row_y, out int row_z)
    {
        List<Vector3> cubes = GetListOfCubes(colors);
        if (cubes.Count == 0)
        {
            row_x = -1;
            row_y = -1;
            row_z = -1;
            return false;
        }

        Vector3 random_cube = cubes[Random.Range(0, cubes.Count)];
        row_x = (int) random_cube.x;
        row_y = (int) random_cube.y;
        row_z = (int) random_cube.z;
        return true;
    }
    
    GameObject GetOrCreateCube(int row_x, int row_y, int row_z)
    {
        GameObject cube = content[row_x, row_y, row_z];
        Rigidbody rb = GetComponent<Rigidbody>();

        if (cube == null)
        {

            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "element"+ row_x +"_"+ row_y+"_"+ row_z;
            cube.transform.parent = container.transform;
            cube.transform.localScale = new Vector3(size, size, size);
            cube.transform.localPosition =
                new Vector3(row_x * size + row_x * delta, row_y * size + row_y * delta,
                    row_z * size + row_z * delta) + Vector3.one * size / 2;

            rb = cube.AddComponent<Rigidbody>();
            rb.mass = 0f;
            rb.isKinematic = true;

            content[row_x, row_y, row_z] = cube;
        }
        return cube;
    }
    
    Color GetCubeColor(int row_x, int row_y, int row_z)
    {
        GameObject cube = GetOrCreateCube(row_x, row_y, row_z);
        return cube.GetComponent<Renderer>().sharedMaterial.color;
    }

    void SetCubeColor(int row_x, int row_y, int row_z, Color new_color, bool is_anim)
    {
        Debug.Log("SetCubeColor - " + is_anim);
        if (is_anim == false)
        {
            Debug.Log("is_anim == false");
            GameObject cube = GetOrCreateCube(row_x, row_y, row_z);
            var tempMaterial = new Material(cube.GetComponent<Renderer>().sharedMaterial);
            tempMaterial.color = new_color;
            cube.GetComponent<Renderer>().sharedMaterial = tempMaterial;
        }
        else
        {
            Debug.Log("is_anim == true");
            AnimChangeCubeColor(row_x, row_y, row_z, new_color);
        }

    }

    void AnimChangeCubeColor(int row_x, int row_y, int row_z, Color new_color)
    {
        GameObject cube = GetOrCreateCube(row_x, row_y, row_z);
        var tempMaterial = new Material(cube.GetComponent<Renderer>().sharedMaterial);

        MiniCubeClone(cube, new_color);
        
        tempMaterial.color = new_color;
        cube.GetComponent<Renderer>().sharedMaterial = tempMaterial;
    }

    void MiniCubeClone(GameObject cube_original, Color new_color)
    {
        GameObject cube = Instantiate(cube_original.gameObject, cube_original.transform.position, cube_original.transform.rotation);

        var tempMaterial = new Material(cube.GetComponent<Renderer>().sharedMaterial);
        tempMaterial.color = tempMaterial.color;
        cube.GetComponent<Renderer>().sharedMaterial = tempMaterial;
        var rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = 1f;
            rb.isKinematic = false;
            rb.AddForce(transform.forward * 5);
        }
        Destroy(cube, 1f);
    }

    public void CubeClone(GameObject cube_original)
    {
        GameObject cube = Instantiate(cube_original.gameObject, cube_original.transform.position, cube_original.transform.rotation);

        //var tempMaterial = new Material(cube.GetComponent<Renderer>().sharedMaterial);
        //tempMaterial.color = tempMaterial.color;
        //cube.GetComponent<Renderer>().sharedMaterial = tempMaterial;
        //var rb = cube.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.mass = 1f;
        //    rb.isKinematic = false;
        //    rb.AddForce(transform.forward * 5);
        //}
        Destroy(cube, 1f);
    }

    public void GenerateColored(float percent)
    {
        while (percent >= (float)GetCubesCount(World.colors)/(float)content.Length)
        {
            int row_x, row_y, row_z = 0;
            GetRandomCubePosition(new List<Color>{Color.white}, out row_x, out row_y, out row_z);
            SetCubeColor(row_x, row_y, row_z, World.GetRandomColor(), false);
        }
    }
    
    public void AddColored(int count, Color new_color)
    {
        int new_colored_count = Mathf.Min(GetCubesCount(World.colors) + count, content.Length);
        Debug.Log("AddColored - " + new_colored_count);
        while (GetCubesCount(World.colors) < new_colored_count)
        {
            int row_x, row_y, row_z = 0;
            GetNextCubePosition(new List<Color>{Color.white}, out row_x, out row_y, out row_z);
            SetCubeColor(row_x, row_y, row_z, new_color, true);
        }

        if (IsFull())
        {
            World.IsGameWin(this.GetComponent<Cube>());
        }

    }
    
    public void RemoveColored(int count)
    {
        int new_colored_count = Mathf.Max(GetCubesCount(World.colors) - count, 0);
        Debug.Log("GetCubesCount(World.colors) - " + GetCubesCount(World.colors));
        Debug.Log("RemoveColored - " + new_colored_count);

        while (GetCubesCount(World.colors) > new_colored_count)
        {
            int row_x, row_y, row_z = 0;
            GetRandomCubePosition(World.colors, out row_x, out row_y, out row_z);
            SetCubeColor(row_x, row_y, row_z, Color.white, true);
        }

        if (IsEmpty())
        {
            World.IsGameOver(this.GetComponent<Cube>());
        }
    }

    public bool IsEmpty()
    {
        int color_count = GetCubesCount(World.colors);
        if (color_count <= 0)
        {
            return true;
        }
        return false;
    }

    public bool IsFull()
    {
        int color_count = GetCubesCount(World.colors);
        if (color_count >= content.Length)
        {
            return true;
        }
        return false;
    }

}
