using System.Collections;
using System.IO;
using UnityEngine;

public class NPCImageGenerator : MonoBehaviour
{
    public Mesh[] npcModels; // Список моделей
    public Material[] Materials;  // Список текстур
    public Camera captureCamera;  // Камера для скриншотов
    public string savePath = "Assets/Resources/NPCPhotos"; // Путь сохранения

    private SkinnedMeshRenderer _skinnedMeshRenderer => GetComponent<SkinnedMeshRenderer>();
    private Renderer _renderer => GetComponent<Renderer>();
    
    private void Start()
    {
        StartCoroutine(GeneratePhotos());
    }

    private IEnumerator GeneratePhotos()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        int modelIndex = 5;
        int matIndex = 0;

        foreach (var model in npcModels)
        {
            matIndex = 0;
            foreach (var material in Materials)
            {
                // Применяем текстуру
                _skinnedMeshRenderer.sharedMesh = model;
                _skinnedMeshRenderer.SetMaterials(new(){material,material});
                
                    

                // Ждём кадр, чтобы камера обновила отображение
                yield return new WaitForEndOfFrame();

                // Делаем скриншот
                string fileName = $"{savePath}/{modelIndex}_{matIndex}.png";
                TakeScreenshot(fileName);
                matIndex++;
            }

            modelIndex++;
        }

        print($"Генерация завершена. Сохранено {modelIndex}_{matIndex} фотографий.");
    }

    private void TakeScreenshot(string filePath)
    {
        RenderTexture renderTexture = captureCamera.targetTexture;
        Texture2D screenShot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        // Сохраняем содержимое камеры в текстуру
        RenderTexture.active = renderTexture;
        captureCamera.Render();
        screenShot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        RenderTexture.active = null;

        // Сохраняем текстуру в файл
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }
}