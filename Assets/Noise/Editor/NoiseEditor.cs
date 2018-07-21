using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class NoiseEditor : EditorWindow
{
    private int width = 512;
    private int height = 512;
    private float xOrg = 0f;
    private float yOrg = 0f;
    private float scale = 15f;

    private static string path = "";
    private static GameObject panel;

    [MenuItem("NoiseTool/Create PrelinNoise Texture")]
    private static void Init()
	{
        NoiseEditor window = (NoiseEditor)GetWindow(typeof(NoiseEditor));
        window.Show();
        path = Application.dataPath + "/Textures/Noise.png";
    }

	private void OnGUI()
	{
        EditorGUILayout.BeginVertical();

        width = EditorGUILayout.IntField("贴图宽度", width);
        height = EditorGUILayout.IntField("贴图高度", height);
        xOrg = EditorGUILayout.FloatField("宽度偏移起点", xOrg);
        yOrg = EditorGUILayout.FloatField("高度偏移起点", yOrg);
        scale = EditorGUILayout.FloatField("重复周期", scale);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("显示噪声贴图"))
		{
            ShowTextrue(CreatePanel(), CreateTexture());
        }

		if (GUILayout.Button("生成噪声贴图"))
		{
			SaveTexture(CreateTexture(), path);
		}

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("说明：噪声贴图显示在Panel的Renderer上");
        EditorGUILayout.LabelField("贴图生成位置为: " + path);

        EditorGUILayout.EndVertical();
	}

    private GameObject CreatePanel()
    {
        if (panel != null)
        {
            return panel;
        }

        Transform CamTra = Camera.main.transform;

        panel = GameObject.CreatePrimitive(PrimitiveType.Plane);
        panel.transform.position = CamTra.forward.normalized * 10 + CamTra.position;
        Vector3 rotation = CamTra.rotation.eulerAngles + new Vector3(-90, 0, 0);
        panel.transform.rotation = Quaternion.Euler(rotation);

        return panel;
    }

    private void ShowTextrue(GameObject panel, Texture2D tex)
    {
        if (panel == null)
        {
            Debug.LogWarning("panel为空");
            return;
        }

        Renderer rend = panel.GetComponent<Renderer>();

        if (rend == null)
        {
            Debug.LogWarning("Renderer为空");
            return;
        }

        rend.sharedMaterial.mainTexture = tex;
    }

	private Texture2D CreateTexture()
	{
        Texture2D tex = new Texture2D(width, height);
        Color[] pix = new Color[width * height];

        float y = 0f;
        while (y < height)
		{
            float x = 0f;
            while (x < width)
			{
                float xCoord = xOrg + x / width * scale;
				float yCoord = yOrg + y / height * scale;
                float sample = PrelinNoise.prelin_noise(new Vector2(xCoord, yCoord));
                pix[(int)y * width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        tex.SetPixels(pix);
        tex.Apply();

        return tex;
    }

	private bool SaveTexture(Texture2D tex, string path)
	{
        if (File.Exists(path))
        {
            Debug.LogWarning("已有文件");
            return false;
        }

        if (tex == null)
        {
            Debug.LogWarning("贴图为空");
            return false;
        }

        // 贴图转换为PNG图片
        byte[] texData = tex.EncodeToPNG();

        // 如果没有目录则创建目录
        int index = path.LastIndexOf('/');
        string dir = path.Remove(index);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        // 贴图存储
        File.WriteAllBytes(path, texData);

        return true;
    }
}
