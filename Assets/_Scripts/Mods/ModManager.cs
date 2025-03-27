using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using ModLibrary;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Zenject;

public class ModManager : MonoBehaviour
{
    private List<ModInfo> _loadedMods = new();
    private List<IModPlugin> _activeModScripts = new();

    private EventBus _eventBus;
    private bool _isLoaded;

    [Inject]
    private void Construct(EventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    private async void Start()
    {
        await LoadMods();

        InitializeAllModsScripts();
    }

    private void Update()
    {
        if (_isLoaded)
            UpdateAllModsScripts();
    }

    private void OnApplicationQuit()
    {
        DisableAllModsScripts();
    }

    private void UpdateAllModsScripts()
    {
        foreach (var plugin in _activeModScripts)
        {
            if (plugin != null)
                plugin.Update();
        }
    }

    private void InitializeAllModsScripts()
    {
        foreach (var plugin in _activeModScripts)
        {
            Debug.Log($"Initializing mod {plugin.Id}");
            var modInfo = GetCurrentModInfo(plugin.Id);
            if (modInfo != null)
                plugin.Initialize(modInfo.FolderPath);
        }

        _isLoaded = true;
    }

    private ModInfo GetCurrentModInfo(string id)
    {
        foreach (var info in _loadedMods)
        {
            if (info.Id == id)
                return info;
        }
        
        return null;
    }

    private void DisableAllModsScripts()
    {
        foreach (var plugin in _activeModScripts)
        {
            plugin.Unload();
        }
    }

    private async Task LoadMods()
    {
        var modsPath = $"C:/Users/{Environment.UserName}/Desktop/mods";

        if (!Directory.Exists(modsPath))
            Directory.CreateDirectory(modsPath);

        foreach (var mod in Directory.GetDirectories(modsPath))
        {
            var path = mod.Replace("\\", "/");
            var modInfo = GetModInfo(path);

            if (modInfo != null)
            {
                await LoadModScripts(modInfo);
                LoadModTextures(modInfo);
                LoadModSprites(modInfo);
                LoadModAudio(modInfo);
                LoadModConfigs(modInfo);
                _loadedMods.Add(modInfo);
            }
        }
    }

    private ModInfo GetModInfo(string modFolder)
    {
        var manifestPath = modFolder + "/manifest.json";

        if (File.Exists(manifestPath))
        {
            var manifestJson = File.ReadAllText(manifestPath);
            var modInfo = JsonUtility.FromJson<ModInfo>(manifestJson);
            modInfo.FolderPath = modFolder;
            return modInfo;
        }

        Debug.LogError($"Mod manifest not found in {modFolder}");
        return null;
    }
    
    private Task LoadModScripts(ModInfo modInfo)
    {
        foreach (var dll in modInfo.DllsPath)
        {
            var path = modInfo.FolderPath + dll;
            if (File.Exists(path))
            {
                var assembly = Assembly.LoadFile(path);
                FindAndRegisterModPlugins(assembly, modInfo.Id);
            }
            else
                Debug.LogError($"Mod dll path not found. Path: {path}");
        }

        return Task.CompletedTask;
    }

    private void FindAndRegisterModPlugins(Assembly assembly, string modeId)
    {
        foreach (var type in assembly.GetTypes())
        {
            try
            {
                var plugin = (IModPlugin)Activator.CreateInstance(type);
                if (plugin.Id == modeId)
                {
                    Debug.Log($"Registering mod {plugin}");
                    _activeModScripts.Add(plugin);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to cast {type} to IModPlugin");
            }
        }
    }
    
    private void LoadModTextures(ModInfo modInfo)
    {
        var texturesPath = modInfo.FolderPath + "/Textures";
        if (Directory.Exists(texturesPath))
        {
            var textureFiles = Directory.GetFiles(texturesPath, "*.png");
            foreach (var file in textureFiles)
            {
                var textureName = Path.GetFileNameWithoutExtension(file);
                var texture = LoadTextureFromFile(file);
                if (texture != null)
                {
                    ReplaceGameTexture(textureName, texture);
                }
            }
        }
    }

    private Texture2D LoadTextureFromFile(string filePath)
    {
        var fileData = File.ReadAllBytes(filePath);
        var texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        else
        {
            Debug.LogError($"Не удалось загрузить текстуру из {filePath}");
            return null;
        }
    }
    
    private void ReplaceGameTexture(string textureName, Texture2D newTexture)
    {
        var materials = Resources.LoadAll<Material>("");

        foreach (var material in materials)
        {
            if (material.mainTexture.name == textureName)
                material.mainTexture = newTexture;
        }
    }

    private void LoadModSprites(ModInfo modInfo)
    {
        var spritesPath = modInfo.FolderPath + "/Sprites";
        if (Directory.Exists(spritesPath))
        {
            var spriteFiles = Directory.GetFiles(spritesPath, "*.png");
            foreach (var file in spriteFiles)
            {
                var spriteName = Path.GetFileNameWithoutExtension(file);
                var texture = LoadTextureFromFile(file);
                if (texture != null)
                {
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    ReplaceGameSprite(spriteName, sprite);
                }
            }
        }
    }

    private void ReplaceGameSprite(string spriteName, Sprite newSprite)
    {
        var spriteRenderers = Resources.FindObjectsOfTypeAll<SpriteRenderer>();
        foreach (var renderer in spriteRenderers)
        {
            if (renderer.sprite != null && renderer.sprite.name == spriteName)
            {
                renderer.sprite = newSprite;
            }
        }

        var uiImages = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Image>();
        foreach (var image in uiImages)
        {
            if (image.sprite != null && image.sprite.name == spriteName)
            {
                image.sprite = newSprite;
            }
        }
    }
    
    private void LoadModAudio(ModInfo modInfo)
    {
        var audioPath = modInfo.FolderPath + "/Audio";
        if (Directory.Exists(audioPath))
        {
            var audioFiles = Directory.GetFiles(audioPath, "*.wav");
            foreach (var file in audioFiles)
            {
                var audioName = Path.GetFileNameWithoutExtension(file);
                StartCoroutine(LoadAudioClip(file, audioName));
            }
        }
    }

    private IEnumerator LoadAudioClip(string filePath, string audioName)
    {
        using (var www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                ReplaceGameAudio(audioName, clip);
            }
            else
            {
                Debug.LogError($"Не удалось загрузить аудио из {filePath}: {www.error}");
            }
        }
    }

    private void ReplaceGameAudio(string audioName, AudioClip newClip)
    {
        var audioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
        foreach (var source in audioSources)
        {
            if (source.clip != null && source.clip.name == audioName)
            {
                source.clip = newClip;
            }
        }
    }

    private void LoadModConfigs(ModInfo modInfo)
    {
        var playerConfigPath = modInfo.FolderPath + "/Configs/PlayerConfig.json";
        if (File.Exists(playerConfigPath))
        {
            var playerConfig = JsonConvert.DeserializeObject<PlayerConfig>(File.ReadAllText(playerConfigPath));
            _eventBus.Invoke(new PlayerModConfigLoaded(playerConfig));
        }
    }
}