// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AnchorStoreManager
{
    string filename = "anchorKeys";
    string savePath;
    //private List<string> anchorkeys = new List<string>();

    public AnchorStoreManager()
    {
        savePath = string.Format("{0}/{1}.txt", Application.persistentDataPath, filename);
        //baseAddress = exchangerUrl;
        //Task.Factory.StartNew(async () =>
        //{
        //    string previousKey = string.Empty;
        //    while (true)
        //    {
        //        string currentKey = await RetrieveLastAnchorKey();
        //        if (!string.IsNullOrWhiteSpace(currentKey) && currentKey != previousKey)
        //        {
        //            Debug.Log("Found key " + currentKey);
        //            lock (anchorkeys)
        //            {
        //                anchorkeys.Add(currentKey);
        //            }
        //            previousKey = currentKey;
        //        }
        //        await Task.Delay(500);
        //    }
        //}, TaskCreationOptions.LongRunning);
    }

    //public async Task<string> RetrieveAnchorKey(long anchorNumber)
    //{
    //    try
    //    {
    //        HttpClient client = new HttpClient();
    //        return await client.GetStringAsync(baseAddress + "/" + anchorNumber.ToString());
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogException(ex);
    //        Debug.LogError($"Failed to retrieve anchor key for anchor number: {anchorNumber}.");
    //        return null;
    //    }
    //}

    public async Task<List<string>> RetrieveAnchorKeys()
    {
        //try
        //{
        //    HttpClient client = new HttpClient();
        //    return await client.GetStringAsync(baseAddress + "/last");
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogException(ex);
        //    Debug.LogError("Failed to retrieve last anchor key.");
        //    return null;
        //}
        List<string> anchorKeys = new List<string>();
        using (StreamReader sr = new StreamReader(savePath))
        {
            while (sr.Peek() >= 0)
            {
                anchorKeys.Add(sr.ReadLine());
            }
        }
        return anchorKeys;
    }

    public async Task StoreAnchorKey(string anchorKey)
    {
        if (string.IsNullOrWhiteSpace(anchorKey))
        {
            return;
        }


        StreamWriter writer = new StreamWriter(savePath, true);
        writer.WriteLine(anchorKey);
        writer.Close();


        //try
        //{
        //    HttpClient client = new HttpClient();
        //    var response = await client.PostAsync(baseAddress, new StringContent(anchorKey));
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        long ret;
        //        if (long.TryParse(responseBody, out ret))
        //        {
        //            Debug.Log("Key " + ret.ToString());
        //            return ret;
        //        }
        //        else
        //        {
        //            Debug.LogError($"Failed to store the anchor key. Failed to parse the response body to a long: {responseBody}.");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError($"Failed to store the anchor key: {response.StatusCode} {response.ReasonPhrase}.");
        //    }

        //    Debug.LogError($"Failed to store the anchor key: {anchorKey}.");
        //    return -1;
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogException(ex);
        //    Debug.LogError($"Failed to store the anchor key: {anchorKey}.");
        //    return -1;
        //}
    }
}
