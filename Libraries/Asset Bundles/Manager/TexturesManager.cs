using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TexturesManager : TPRLSingleton<TexturesManager>
{
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private Sprite[] comicSprites;
    private Dictionary<string, Sprite> dicSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> dicComicSprites = new Dictionary<string, Sprite>();

    protected override void Awake()
    {
        dontDestroyOnLoad = true;
        base.Awake();
    }

    void Start()
    {
        //AddSprites();
    }

    private void AddSprites()
    {
        if (sprites == null || sprites.Length == 0) return;
        int length = sprites.Length;
        for (int i = 0; i < length; i++)
        {
            if (sprites[i] == null) continue;
            if (!dicSprites.ContainsKey(sprites[i].name))
                dicSprites.Add(sprites[i].name, sprites[i]);
        }

        length = comicSprites.Length;
        for (int i = 0; i < length; i++)
        {
            if (comicSprites[i] == null) continue;
            if (!dicComicSprites.ContainsKey(comicSprites[i].name))
                dicComicSprites.Add(comicSprites[i].name, comicSprites[i]);
        }
    }

    public Sprite GetSprites(string sprite_name)
    {
        if (sprite_name.Contains("PNG/") || sprite_name.Contains("png/"))
        {
            sprite_name = sprite_name.Substring(sprite_name.LastIndexOf("/") + 1);
        }
        if (dicSprites.Count == 0 || !dicSprites.ContainsKey(sprite_name))
        {
            Sprite sprite = AssetBundleDownloader.GetAsset<Sprite>(BundleName.SPRITE, sprite_name);
            if (!dicSprites.ContainsKey(sprite.name))
                dicSprites.Add(sprite.name, sprite);
            return sprite;
        }
        return dicSprites[sprite_name];
    }

    public void GetSpritesAsync(string sprite_name, UnityAction<Sprite> action)
    {
        if (sprite_name.Contains("PNG/") || sprite_name.Contains("png/"))
        {
            sprite_name = sprite_name.Substring(sprite_name.LastIndexOf("/") + 1);
        }
        if (dicSprites.Count == 0 || !dicSprites.ContainsKey(sprite_name))
        {
            AssetBundleDownloader.GetAssetAsync<Sprite>(BundleName.SPRITE, sprite_name, (sprite) => {
                if (sprite != null)
                {
                    if (!dicSprites.ContainsKey(sprite.name))
                        dicSprites.Add(sprite.name, (Sprite)sprite);
                    action?.Invoke((Sprite)sprite);
                }
            });
            return;
        }
        action?.Invoke(dicSprites[sprite_name]);
    }

    public Sprite GetComicSprites(string sprite_name)
    {
        if (sprite_name.Contains("PNG/") || sprite_name.Contains("png/"))
        {
            sprite_name = sprite_name.Substring(sprite_name.LastIndexOf("/") + 1);
        }
        if (dicComicSprites.Count == 0 || !dicComicSprites.ContainsKey(sprite_name))
        {
            Sprite sprite = AssetBundleDownloader.GetAsset<Sprite>(BundleName.SPRITE, sprite_name);
            if (!dicComicSprites.ContainsKey(sprite.name))
                dicComicSprites.Add(sprite.name, sprite);
            return sprite;
        }
        return dicComicSprites[sprite_name];
    }

    public void GetComicSpritesAsync(string sprite_name, UnityAction<Sprite> action)
    {
        if (dicComicSprites.Count == 0 || !dicComicSprites.ContainsKey(sprite_name))
        {
            AssetBundleDownloader.GetAssetAsync<Sprite>(BundleName.SPRITE, sprite_name, (sprite) => {
                if (sprite != null)
                {
                    if (!dicComicSprites.ContainsKey(sprite.name))
                        dicComicSprites.Add(sprite.name, (Sprite)sprite);
                    action?.Invoke((Sprite)sprite);
                }
            });
            return;
        }
        action?.Invoke(dicComicSprites[sprite_name]);
    }

    public Texture2D GetTexutres(string texture_name)
    {
        if (texture_name.Contains("PNG/") || texture_name.Contains("png/"))
        {
            texture_name = texture_name.Substring(texture_name.LastIndexOf("/") + 1);
        }
        if (dicSprites.Count == 0 || !dicSprites.ContainsKey(texture_name))
        {
            Debug.LogError("Khong tim thay texture : " + texture_name);
            return null;
        }
        return dicSprites[texture_name].texture;
    }

    protected override void OnDestroy()
    {
        List<string> keys = new List<string>(dicSprites.Keys);
        int length = keys.Count;
        for (int i = 0; i < length; i++)
        {
            dicSprites[keys[i]] = null;
        }
        dicSprites.Clear();

        keys = new List<string>(dicComicSprites.Keys);
        length = keys.Count;
        for (int i = 0; i < length; i++)
        {
            dicComicSprites[keys[i]] = null;
        }
        dicComicSprites.Clear();
        base.OnDestroy();
    }
}