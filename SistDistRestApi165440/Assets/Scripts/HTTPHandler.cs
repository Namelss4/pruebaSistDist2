using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpHandler : MonoBehaviour
{

    private string url = "https://rickandmortyapi.com/api";
    public void SendRequest()
    {
        StartCoroutine("GetCharacter");
    }

    IEnumerator GetCharacter()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/character");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                Debug.Log(request.downloadHandler.text);
                CharactersList characters = JsonUtility.FromJson<CharactersList>(request.downloadHandler.text);

                foreach (Character character in characters.results)
                {
                    Debug.Log(character.name + " is a " + character.species);
                }

            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }

        }
    }
}

[System.Serializable]
public class CharactersList
{
    public Character[] results;
}
[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;

}
