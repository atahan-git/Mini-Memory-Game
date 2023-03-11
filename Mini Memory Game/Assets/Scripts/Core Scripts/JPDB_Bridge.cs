using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
public class JPDB_Bridge : MonoBehaviour {

    public static JPDB_Bridge s;
    
    public const int newWordFetchCount = 20;


    public TMP_Text currentWords;
    public GameObject needMoreWords;
    
    private void Awake() {
        if(s != null)
            s = this;
    }

    void Start()
    {
        //StartCoroutine(postRequest("http:///www.yoururl.com", "your json"));
        //StartCoroutine(PostWithTask());
        
        //DataSaver.s.CallOnCloudLoad(CheckFetch);
    }

    /*void CheckFetch() {
        var wordPack = DataSaver.s.GetCurrentSave().mainWordPack;
        if (wordPack.wordPairs.Count <= 0) {
            FetchWordsFromJPDB();
        }

        currentWords.text = wordPack.wordPairs.Count.ToString();
    }*/

    private void OnDestroy() {
        //DataSaver.s.RemoveFromCloudLoadCall(CheckFetch);
    }


    private bool isFetching = false;
    private int countToFetch;
    public void FetchWordsFromJPDB() {
        /*if (!isFetching) {
            countToFetch = newWordFetchCount;
            BeginFetch();
        }*/
        
        needMoreWords.SetActive(true);
    }
    
    [Button]
    public void FetchWordsFromJPDBEditor() {
        if (!isFetching) {
            countToFetch = newWordFetchCount;
            BeginFetch();
        }
    }

    void BeginFetch() {
        isFetching = true;
        /*var activeSave = DataSaver.s.GetCurrentSave();

        if (activeSave.mainWordPack.wordPackName == "unset") {
            activeSave.mainWordPack = new WordPack();
            activeSave.mainWordPack.wordPackName = "JPDB word pack";
        }*/


        Debug.Log("Fetching words from JPDB");
        FetchOneWord();
    }

    void FetchOneWord() {
        StartCoroutine(PostRequest());
    }

    public async Task<string> GetResource()
    {
        using (var httpClient = new HttpClient())
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://jpdb.io/review#a"),
                Headers =
                {
                    { "Cookie", "sid=521196e310556a9309f5f6bb42779d23" },
                },
            };
            
            var response = await httpClient.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return "yeet";
            var resourceJson = await response.Content.ReadAsStringAsync();
            return resourceJson;
        }
    }
    
    IEnumerator PostRequest() { 
        WWWForm form = new WWWForm();
        //string formData = "{\"fields\": [\"id\"]}";
        
        var uwr = UnityWebRequest.Post("https://jpdb.io/review#a", "");
        
        uwr.SetRequestHeader("Cookie", "sid=521196e310556a9309f5f6bb42779d23");
        
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            //Debug.Log("Received: " + uwr.downloadHandler.text);
            ProcessHTML(uwr.downloadHandler.text);
        }
    }

    
    void ProcessHTML(string html) {
        //Debug.Log("Processing html");
        var index = html.IndexOf("<div style=\"width: 2.5rem\"></div><a class=\"plain\"", StringComparison.Ordinal);

        if (index != -1) {
            //Debug.Log("First Type match");

            var chopped = html.Substring(index);
            chopped = chopped.Substring(chopped.IndexOf("<a", StringComparison.Ordinal));
            chopped = chopped.Substring(chopped.IndexOf(">", StringComparison.Ordinal) + 1);

            var endIndex = chopped.IndexOf("</a>", StringComparison.Ordinal);

            var word = chopped.Substring(0, endIndex);

            var rubies = word.Split(new string[] { "<ruby>" }, StringSplitOptions.None);
            if (rubies.Length > 1) {
                //Debug.Log($"word has rubies {word}");
                var rubyProcessesed = "";
                var miniLength = 0f;
                for (int i = 1; i < rubies.Length; i++) {
                    rubies[i] = rubies[i].Replace("</ruby>", "");
                    var rubyChunk = rubies[i];
                    //Debug.Log($"Ruby processing {rubies[i]}");

                    var rtChunkIndex = rubyChunk.IndexOf("<rt>", StringComparison.Ordinal);
                    if (rtChunkIndex != -1) {
                        var rtChunk = rubyChunk.Substring(rtChunkIndex + 4);
                        rtChunk = rtChunk.Replace("</rt>", "");
                        rubyChunk = rubyChunk.Substring(0, rtChunkIndex);

                        var prevMiniLength = miniLength;
                        miniLength += Mathf.Max(rubyChunk.Length, rtChunk.Length/2f * 3);
                        rubyProcessesed += $"{rubyChunk}<pos={prevMiniLength}em><sup>{rtChunk}</sup><pos={miniLength}em>";
                        Debug.Log("Check ruby processor");
                    } else {
                        rubyProcessesed += $"{rubyChunk}";
                    }
                }

                word = rubyProcessesed;
            }



            var meaningIndex = html.IndexOf("\"description\"",StringComparison.Ordinal);
            var meaningChopped = html.Substring(meaningIndex+17);

            var meaning = meaningChopped.Substring(0, meaningChopped.IndexOf("<", StringComparison.Ordinal));

            //Debug.Log(meaning);
            meaning = SanitizeMeaning(meaning);


            if (word.Length > 0 && meaning.Length > 0) {
                RegisterWord(word, meaning);
                SendReply(html);
                return;
            }
        }

        index = html.IndexOf("<div class=\"hbox\" style=\"justify-content: center; min-height: 15.5rem;\">", StringComparison.Ordinal);

        if (index != -1) {
            //Debug.Log("Second type match");

            var startIndex = index + 108;

            var myWord = html.Substring(startIndex);
            var endIndex = myWord.IndexOf("#", StringComparison.Ordinal);
            myWord = myWord.Substring(0, endIndex);
            
            //Debug.Log(myWord);

            var meaningIndex = html.IndexOf("<h6 class=\"subsection-label\">Keyword&nbsp;", StringComparison.Ordinal);
            var chopped = html.Substring(meaningIndex);

            meaningIndex = chopped.IndexOf("<div class=\"subsection\">", StringComparison.Ordinal);
            meaningIndex += 24;

            var meaning = chopped.Substring(meaningIndex);
            var meaningEnd = meaning.IndexOf("<", StringComparison.Ordinal);

            meaning = meaning.Substring(0, meaningEnd);
            
            meaning = SanitizeMeaning(meaning);
            //Debug.Log(meaning);

            if (myWord.Length > 0 && meaning.Length > 0) {
                RegisterWord(myWord, meaning);
                SendReply(html);
            }

            return;
        }
        
        Debug.LogError($"Unknown HTML type! {html}");
        isFetching = false;
    }

    string SanitizeMeaning(string meaning) {
        var firstMeaningEnd = meaning.IndexOf(";", StringComparison.Ordinal);
        if (firstMeaningEnd != -1)
            meaning = meaning.Substring(0, firstMeaningEnd);

        var paranthesisStart = meaning.IndexOf(" (", StringComparison.Ordinal);
        while (paranthesisStart != -1) {
            var paranthesisEnd = meaning.IndexOf(")",StringComparison.Ordinal);

            meaning = meaning.Substring(0, paranthesisStart) + meaning.Substring(paranthesisEnd+1);
            paranthesisStart = meaning.IndexOf(" (", StringComparison.Ordinal);
        }

        meaning = meaning.TrimStart(' ');
        meaning = meaning.TrimEnd(' ');

        return meaning;
    }


    void SendReply(string html) {
        //Debug.Log("Sending reply");
        var index = html.IndexOf("I know this, but may forget", StringComparison.Ordinal);

        if (index != -1) {
            //Debug.Log("first type reply");

            var chopped = html.Substring(index - 1000, 1000);

            var cIndex = chopped.IndexOf("\"c\"", StringComparison.Ordinal);
            var cValue = chopped.Substring(cIndex + 11);
            var end = cValue.IndexOf("\"", StringComparison.Ordinal);
            cValue = cValue.Substring(0, end);


            StartCoroutine(PostResponse(cValue));
            return;
        }
    }
    
    IEnumerator PostResponse(string cVal) { 
        //Debug.Log($"sending reply with cval {cVal}");
        WWWForm form = new WWWForm();
        form.AddField("c", cVal);
        form.AddField("r", 0);
        form.AddField("g", -1);
        //string formData = "{\"fields\": [\"id\"]}";
        
        var uwr = UnityWebRequest.Post("https://jpdb.io/review#a", form);
        
        uwr.SetRequestHeader("Cookie", "sid=521196e310556a9309f5f6bb42779d23");
        
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            //Debug.Log("Received: " + uwr.downloadHandler.text);
            AfterPostResponse();
        }
    }

    void AfterPostResponse() {
        if (countToFetch > 0) {
            FetchOneWord();
        } else {
            Debug.Log("Word fetch complete!");
            isFetching = false;
        }
        countToFetch -= 1;
    }


    void RegisterWord(string word, string meaning) {
        //var wordPack = DataSaver.s.GetCurrentSave().mainWordPack;
        var wordPack = PlayerLoadoutController.s.GetMainWordPack();

        for (int i = 0; i < wordPack.wordPairs.Count; i++) {
            if(wordPack.wordPairs[i].word == word)
                return;
        }
        
        wordPack.wordPairs.Add(new WordPair() {
            id = wordPack.wordPairs.Count,
            word = word,
            meaning = meaning
        });
        
        currentWords.text = wordPack.wordPairs.Count.ToString();
        Debug.Log($"Word registered from jpdb : '{word}' - '{meaning}'");
        DataSaver.s.SaveActiveGame();
    }

    /*IEnumerator PostWithTask() {
        var t = Task.Run(async () => await GetResource());
        yield return new WaitUntil(() => t.IsCompleted);
        Debug.Log(t.Result);
    }

    IEnumerator postRequest(string url, string json) { 
        WWWForm form = new WWWForm();
        form.AddField("fields", "[id]");
        //string formData = "{\"fields\": [\"id\"]}";
        
        var uwr = UnityWebRequest.Post("https://jpdb.io/api/v1/list-user-decks", "fields: [id]");
        
        uwr.SetRequestHeader("Authorization", "Bearer d0400395fc16493d423f7f8b23e8ca69");
        
        Debug.Log(uwr);
        Debug.Log(uwr.url);
        Debug.Log(uwr.uri);
        Debug.Log(uwr.method);
        /*uwr.SetRequestHeader("Content-Type", "application/json");#1#
        /*byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");#1#

        
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }
    public async Task<string> GetResource()
    {
        using (var httpClient = new HttpClient())
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://jpdb.io/api/v1/list-user-decks"),
                Headers =
                {
                    { "Authorization", "Bearer d0400395fc16493d423f7f8b23e8ca69" },
                },
                Content = new StringContent("{\n" +
                                            "\"fields\": [\n" +
                                            "\"id\",\n" +
                                            "\"name\"\n" +
                                            "]\n" +
                                            "}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            
            var response = await httpClient.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return "yeet";
            var resourceJson = await response.Content.ReadAsStringAsync();
            return resourceJson;
        }
    }*/
}
