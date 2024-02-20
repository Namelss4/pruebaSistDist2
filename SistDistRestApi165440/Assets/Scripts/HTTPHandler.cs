using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class HttpHandler : MonoBehaviour
{
    private string FakeApiUrl = "https://my-json-server.typicode.com/Namelss4/pruebaSistDist2";
    private string RickYMortyApiUrl = "https://rickandmortyapi.com/api";
    Coroutine sendRequest_GetCharacters;

    [SerializeField]
    private RawImage[] portraits = new RawImage[5];
    [SerializeField]
    private TextMeshProUGUI[] names = new TextMeshProUGUI[5];

    [SerializeField]
    private TextMeshProUGUI btn1, btn2, btn3;

    private int imgCounter;

    //private bool isGettingUsernames;

    public void SendRequest(int id)
    {
        if (sendRequest_GetCharacters == null)
            imgCounter = 0;
        sendRequest_GetCharacters = StartCoroutine(GetUserData(id, false));
    }

    IEnumerator GetUserData(int uid, bool isGettingNames)
    {
        UnityWebRequest request = UnityWebRequest.Get(FakeApiUrl + "/users/" + uid);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                // Debug.Log(request.downloadHandler.text);
                UserData user = JsonUtility.FromJson<UserData>(request.downloadHandler.text);

                Debug.Log(user.username);

                if (!isGettingNames)
                {
                    foreach (int cardID in user.deck)
                    {
                        StartCoroutine(GetCharacter(cardID, imgCounter));
                        imgCounter++;                   
                    }
                }
                else
                {
                    switch (uid)
                    {
                        case 1:
                            btn1.text = user.username;
                            break;
                        case 2:
                            btn2.text = user.username;
                            break;
                        case 3:
                            btn3.text = user.username;
                            break;
                        default:
                            break;
                    }
                }

            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }

        }
        sendRequest_GetCharacters = null;
    }

    IEnumerator GetCharacter(int id, int nameCounter)
    {
        UnityWebRequest request = UnityWebRequest.Get(RickYMortyApiUrl + "/character/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                //Debug.Log(request.downloadHandler.text);
                Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);

                names[nameCounter].text = character.name;
                StartCoroutine(DownloadImage(character.image, nameCounter));
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }

        }
        sendRequest_GetCharacters = null;
    }

    IEnumerator DownloadImage(string url, int imgCounter)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            portraits[imgCounter].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    IEnumerator GetCharacters()
    {
        UnityWebRequest request = UnityWebRequest.Get(RickYMortyApiUrl + "/character");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                //Debug.Log(request.downloadHandler.text);
                CharactersList characters = JsonUtility.FromJson<CharactersList>(request.downloadHandler.text);

                Debug.Log("TOTAL:" + characters.info.count);
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
        sendRequest_GetCharacters = null;
    } 

    private void Start()
    {
        StartCoroutine(GetUserData(1, true));
        StartCoroutine(GetUserData(2, true));
        StartCoroutine(GetUserData(3, true));
    }
}

[System.Serializable]
public class UserData
{
    public int id;
    public string username;
    public int[] deck;
}

[System.Serializable]
public class CharactersList
{
    public CharactersInfo info;
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
[System.Serializable]
public class CharactersInfo
{
    public int count;
    public int pages;
    public string prev;
    public string next;
}
