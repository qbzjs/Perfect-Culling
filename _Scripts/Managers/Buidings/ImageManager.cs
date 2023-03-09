using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ImageManager : SingletonMonoDontDestroy<ImageManager>
{
    private Queue<string> lstDownload = new Queue<string>();
    public Dictionary<string, UnityAction<string, Texture2D>> queue_Image = new Dictionary<string, UnityAction<string, Texture2D>>();
    public Dictionary<string, Texture2D> lstTextures = new Dictionary<string, Texture2D>();
    public Dictionary<string, bool> lstDownloadCheckDestroy = new Dictionary<string, bool>();
    private bool isStartDownload = false;

    private void Start()
    {
        lstDownload.Enqueue("https://cdn.tgdd.vn//GameApp/1343776//48-800x1422.jpg");
        lstDownload.Enqueue("https://tienthuattoan.com/wp-content/uploads/2022/01/Copyright-%C2%A9-tienthuattoan.com_-1.png");
        lstDownload.Enqueue("https://binancegate.com/wp-content/uploads/2021/11/The-Parallel-thumb.jpg");
        lstDownload.Enqueue("https://cafefcdn.com/thumb_w/650/pr/2021/photo2021-11-1817-36-04-16372325109501637846214-0-0-778-1246-crop-1637232676751-63772857402763.jpg");
        lstDownload.Enqueue("https://ucbb0151e4654c23bd45b53b63d0.previews.dropboxusercontent.com/p/thumb/ABqFFxmddYWtHNqLSF4f76kJ5a6l9gMj1ADVqr-vXztYLbllhPycofD_vvemk-5q5JizEedRJyL_Rh7mvmLbvLAEhXUK8rbb_j2weIbb174tGAMO87nG1pbUKEJJxcnl2xjMWvWngMcu35FHcPYFpLIqcfzOlu8fO261tNffAvU3dJOAk9dzhjoXV4aDm5VmKiB8CYINJOeQEX24L5mnLurcT_o1eEEbzWKmS33Q-nWOs5NEs-DS5Xi_-GEOKJebylu5_fM8bUJdG_u7WKl_ksBrA-KdHUy1fHOybWLUWTBRcz8k8Ae1nX9u2DM7LkBrb_cq7fIBmDKKhudi5rSCoiaOjFSe4wdXxGje09D1l_o8lwUKyph78jDuf71vfl8VYyA/p.jpeg");
        lstDownload.Enqueue("https://ucc8b6eea1f5f1b59bbe5d41b59b.previews.dropboxusercontent.com/p/thumb/ABoQ2kouerFJcEmnfM-_866pwKYWPA7aJDpCva-P6ljNjjb1tM6-YYGIuYjDEMbzt8fegyAvLRaDxVgQqoQzR3vAWbc9kYZfc72cEySh_xTJyW3J50tWhhi6eOtpoBmDudq_rgfp4ESopjgudf1KafqfDh8CpngAkYPmSe8oxQ4XUS632squr8NrqJw9ZRoGGrXPGBPmHr9jeaU7LEPRWiNS7qwqvXvGw0h72CBjGVz78HI1JtYUl1VYcGfcn0dFy_caupSxfrUpWVpjQplxwsnm-uGHKumsPlIw9fjl3sndeLcO9kYSUf_4NlYbOxEeBiOaEi_822RKKd3--kYOpPgKO6ORzRtZdTyABRdJ8x-qLJ8TBS9AHCHFAMea02hK4qc/p.jpeg");
        lstDownload.Enqueue("https://uc9069cafe5d2aa776d1edd699a7.previews.dropboxusercontent.com/p/thumb/ABrQOuoCfY6yFODceN1Mi3yvXaeJK_Rte8uW463oFyFrsde2a6Oq33Cl6l3_mX2CvnwttBf_7AXjIK7J-4q0Gm7gnEAZBMnefR5z-OPdGU-TmnTHX4TkWa1l8U-ICnWoni2m-1Hh6PWB31TVmrrqElA3FaA1C0agRzg0UMGBOQ-enSAWOHf3NzfIuvgJqqxEaYz32Vp80c8bmYtIf9MHHrcFe0PKX0LO1p9Oloqp0kkz6gYafetUiTg4iBGqinN5I1EhHK81zF4ghlXenXf5Np_dubRb5fTdgmXi3bsOPlnqXjTPy9xb0fOPJ-NqzoyTHbw1bRljoJrKpu-3oInIisxb9sO5l_Da5ziQNhvhsCZ2_9hYaKxdyfs2zGLMF9qux7M/p.jpeg");
        lstDownload.Enqueue("https://uc317f8b0f54fcbea5e48581ddda.previews.dropboxusercontent.com/p/thumb/ABocdl7-L17PFIyp1LD4Yq46cglzM-xA4ydps_JF8HZ3Tjy4HKSy4G_xmOVuW4ZiFY21HvqqeCl0b5pKl5EYNJ-K_WJ5BKCV3pFKBUx9aNnQq3Ptmd0iKxuLhl4zOKHSEZAZc7D8H7M2vXchOIsxULFcaMXy436s3jDItUsMjcIAKCmBscKZwmdjElGFFi6vU9gQIFe43bY07UiQeVMaerrHv2GJtkyBalhhhwjFJxGUhJEtq-eG95ddHMowyK0m_ieYZy7p2frqOLuUazAC_zzb_7I51gngd6mLawq-5uDHq87hLUqbjTjLAnhjT0nq2GpivRbpC4mCDb7VxW8UFKjLBF81ZQxDGVd23f-BfyGajhZKELdDZ5xdTWxL07rVexI/p.jpeg");
        lstDownload.Enqueue("https://uca1c42fb5694e20e49e8101dfcd.previews.dropboxusercontent.com/p/thumb/ABpTTpO27ysieddT8CQLJ0DJX8csf6pUCxczqz3PwAMQHA1UGq8kt8UhFCE-58kxrwI_Klb60gdeVmh3Hyoqf6RZe7G02lHKKgA3nd-aBmeXcSYdosRE_b0r5TSm-HzTRTRiV1uI3JWARGWxDczNEoAshz1HIsp2yoqPtz5TRJOfeEeRBoumwQ1LpDNAdcCw3XHNmfoNQlKKwIpX-p_IR9atfyhH_Dac6jCNPvqVw8gh6amTx0VxNou9Mbk_XhFEB5-Q8MLTwvhc-osrAPYrkBaz-h_d25M6TopHg7_oBJZoTps10Fl6FiAZGiVEYbRj25s948Hw0vnKaTBr1zf_9h0fGwlOByS5d5trvf_q0G0gS1BB8-0blNBAPCokDCkiVdA/p.jpeg");
        lstDownload.Enqueue("https://uc453df8830a98904625add1e93d.previews.dropboxusercontent.com/p/thumb/ABpOoo8TKH6QtDmSXYNdDzEr7kJQPoVkxywB1rlWYaz13cRa_cA6q_yZIGdA58H_2pZCAZ4_KpaEdVuWrfogNeRZL2_Y93US0oZ7mqJuRGSuKSpq_4USZYLYcqgGTcfeMULrPEjmjelT7Qn-uSng0r59J7XCKiBaeiTd0PC6Lryz7QL6uy7Js-rtPrClQ-o1bLIFKtKRsQs8CcKuO45giJcIaLDt8PTZHY81mqGEOlUevnoeOquyxRpuSpkK1fXxSTcuZuClIDlvdLks1AcXdwColVELB3_EUbvvpZFDmtgekud00of_sKX_RDP4I_kG8V7ZWBI1qeKHfU9Ska0lPP5FlSsOwDo0Afsso62ASfLyCwONIT-wBkfFE0I6Ugtmgn0/p.jpeg");
    }

    private void LateUpdate()
    {
        if (isStartDownload == false) return;
        if (lstDownload.Count == 0)
        {
            isStartDownload = false;
            return;
        }
        string url = lstDownload.Dequeue();
        if (string.IsNullOrEmpty(url)) return;
        StartCoroutine(DownLoadImage(url));
    }

    public void StartDownloadImage()
    {
        isStartDownload = true;
    }
    IEnumerator DownLoadImage(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();
            yield return www.downloadProgress >= 1.0f && www.isDone;
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                yield break;
            }
            //string type_url = url.Substring(url.LastIndexOf('/'), url.Length - url.LastIndexOf('/'));
            var texture = DownloadHandlerTexture.GetContent(www);
            if (lstDownloadCheckDestroy.ContainsKey(url))
            {
                if (texture != null && lstDownloadCheckDestroy[url])
                    texture.name += "_destroy";
                lstDownloadCheckDestroy.Remove(url);
            }

            if (!lstTextures.ContainsKey(url))
            {
                lstTextures.Add(url, texture);
            }


            if (queue_Image.ContainsKey(url))
            {
                queue_Image[url]?.Invoke(url, texture);
                queue_Image.Remove(url);
            }
        }
    }

    public void RegisterImage(string url, UnityAction<string, Texture2D> action, bool is_destroy_after_unuse = false)
    {
        //string type_url = url.Substring(url.LastIndexOf('/'), url.Length - url.LastIndexOf('/'));
        if (!string.IsNullOrEmpty(url))
        {
            if (lstTextures.ContainsKey(url))
            {
                if (lstTextures[url] != null)
                {
                    action?.Invoke(url, lstTextures[url]);
                    return;
                }
                else
                    lstTextures.Remove(url);
            }
            if (!lstDownload.Contains(url))
                lstDownload.Enqueue(url);
            if (!lstDownloadCheckDestroy.ContainsKey(url))
                lstDownloadCheckDestroy.Add(url, is_destroy_after_unuse);
            if (!queue_Image.ContainsKey(url))
            {
                queue_Image.Add(url, action);
            }
            else
            {
                queue_Image[url] += action;
            }
        }
    }

    public void UnregisterAll(string name)
    {
        if (queue_Image.ContainsKey(name))
        {
            queue_Image.Remove(name);
        }
    }

    public void UnregisterImage(string name, UnityAction<string, Texture2D> action)
    {
        if (queue_Image.ContainsKey(name))
        {
            queue_Image[name] -= action;
        }
    }

    public void ClearQueue()
    {
        queue_Image.Clear();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void DestroyTexturesUnuse(bool is_destroy_all)
    {
        Texture2D[] texture2Ds = FindObjectsOfType<Texture2D>();
        if (texture2Ds == null || texture2Ds.Length == 0) return;
        int length = texture2Ds.Length;
        for (int i = 0; i < length; i++)
        {
            if (texture2Ds[i] == null)
            {
                GameObject.Destroy(texture2Ds[i]);
                continue;
            }
            if (is_destroy_all == false)
            {
                if (texture2Ds[i].name.EndsWith("_destroy"))
                {
                    GameObject.Destroy(texture2Ds[i]);
                }
            }
            else
                GameObject.Destroy(texture2Ds[i]);
        }
    }
}
