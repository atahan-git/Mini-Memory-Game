using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using FullSerializer;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using CloudLoginUnity;
using TMPro;
using UnityEngine.Events;

[Serializable]
public class DataSaver {

	public static DataSaver s;

	[SerializeField]
	private SaveFile activeSave;
	public const string saveName = "save.data";

	public bool loadingComplete = false;
	
	private static readonly fsSerializer _serializer = new fsSerializer();

	public TMP_Text status;

	void SetStatus(string message) {
		if(status != null)
			status.text = message;
	}

	public string GetSaveFilePathAndFileName () {
		return Application.persistentDataPath + "/" + saveName;
	}

	public static UnityEvent OnLoadComplete = new UnityEvent();
	public static UnityEvent OnCloudLoadComplete = new UnityEvent();
	public static UnityEvent OnLoggedIn = new UnityEvent();


	public void CallOnLoad(UnityAction action) {
		OnLoadComplete.AddListener(action);
		if (loadingComplete) {
			action.Invoke();
		}
	}

	public bool cloudLoadComplete = false;
	public void CallOnCloudLoad(UnityAction action) {
		OnCloudLoadComplete.AddListener(action);
		if (cloudLoadComplete) {
			action.Invoke();
		}
	}
	
	public void CallOnLogin(UnityAction action) {
		OnLoggedIn.AddListener(action);
		if (isLoggedIn) {
			action.Invoke();
		}
	}

	public void RemoveFromLoadCall(UnityAction action) {
		OnLoadComplete.RemoveListener(action);
	}
	
	public void RemoveFromCloudLoadCall(UnityAction action) {
		OnLoadComplete.RemoveListener(action);
	}
	
	public void RemoveFromLoginCall(UnityAction action) {
		OnLoggedIn.RemoveListener(action);
	}
	
	


	public SaveFile GetCurrentSave() {
		return activeSave;
	}

	public void ClearCurrentSave() {
		Debug.Log("Clearing Save");
		activeSave = MakeNewSaveFile();
	}
	
	public SaveFile MakeNewSaveFile() {
		var file = new SaveFile();
		PlayerLoadoutController.SetDefaultLoadoutWordPacks(file);
		file.isRealSaveFile = true;
		file.lastSaveTimeUTC = DateTime.Now.ToFileTimeUtc();
		file.gameVersion = VersionDisplay.versionTextValue;
		return file;
	}


	public bool isLoggedIn = false;
	public bool dontSave = false;
	private bool saveInNextCycle = false;
	[Button]
	public void SaveActiveGame () {
		if (!dontSave) {
			DiskSave();
			saveInNextCycle = true;
		}
	}

	private float saveTimer = 5;
	private float curSaveTime;
	public void Update() {
		curSaveTime += Time.deltaTime;

		if (curSaveTime > saveTimer && saveInNextCycle && cloudLoadComplete) { // dont save too many times in a row as we are syncing with cloud too.
			// also only save if we did cloud load before so we dont accidently override the cloud data.
			curSaveTime = 0;
			CloudSave();
			saveInNextCycle = false;
		}
	}

	[Button]
	void DiskSave() {
		var path = GetSaveFilePathAndFileName();
		
		SaveFile data = PrepSaveFile();
		
		WriteFile(path, data);
		Debug.Log("disk save complete!");
	}

	SaveFile PrepSaveFile() {
		activeSave.isRealSaveFile = true;
		activeSave.lastSaveTimeUTC = DateTime.Now.ToFileTimeUtc();
		activeSave.gameVersion = VersionDisplay.versionTextValue;
		return activeSave;
	}

	[Button]
	void CloudSave() {
		SaveFile data = PrepSaveFile();
		CloudLoginUser.CurrentUser.SetAttribute(saveName, Zip(ConvertToJson(data)), CloudSaveCallback);
	}
	void CloudSaveCallback (string message, bool hasError)
	{
		if (hasError) {
			Debug.Log("error saving");
			Debug.Log(message);
		} else {
			Debug.Log("cloud save complete!");
		}
	}

	public static void WriteFile(string path, object file) {
		Directory.CreateDirectory(Path.GetDirectoryName(path));
		
		StreamWriter writer = new StreamWriter(path);
		
		var json = ConvertToJson(file, true);

		writer.Write(json);
		writer.Close();

		Debug.Log($"IO OP: file \"{file.GetType()}\" saved to \"{path}\"");
	}
	
	public static string ConvertToJson<T>(T file, bool isPretty = false) where T : class, new() {
		fsData serialized;
		_serializer.TrySerialize(file, out serialized);
		if (isPretty)
			return fsJsonPrinter.PrettyJson(serialized);
		else 
			return fsJsonPrinter.CompressedJson(serialized);
		
	}


	const string gameToken = "0bef4af0-6231-403e-b488-a37d3e5f7f96";
	const string gameID = "235";
	public void Login() {
		CloudLogin.SetUpGame(gameID, gameToken, ApplicationSetUp, true);
	}
	
	
	public string userName = "";
	public string userEmail = "";
	public string password = "";
	void ApplicationSetUp(string message, bool hasError)
	{
		if (hasError)
		{
			Debug.Log("error setting aplication");
			Debug.Log(message);
		}
		else {

			SetStatus("<color=yellow>GAME CONNECTED!!" + CloudLogin.GetGameId() + "</color>");
			Debug.Log("<color=yellow>GAME CONNECTED!!" + CloudLogin.GetGameId() + "</color>");
			Debug.Log("*****************************************");

			userEmail = PlayerPrefs.GetString("userEmail");
			if (userEmail.Length > 0) {
				userName = userEmail.Substring(0, userEmail.IndexOf("@", StringComparison.Ordinal));
				password = PlayerPrefs.GetString("password");

				Debug.Log("Signing Up");
					CloudLogin.SignUp(userEmail, password, password, userName, SignedUp);
			}
		}
	}

	public void SignUp(string _email, string _password) {
		PlayerPrefs.SetString("userEmail", _email);
		PlayerPrefs.SetString("password", _password);
		
		
		userEmail = PlayerPrefs.GetString("userEmail");
		userName = userEmail.Substring(0, userEmail.IndexOf("@", StringComparison.Ordinal));
		password = PlayerPrefs.GetString("password");
		
		CloudLogin.SignUp(userEmail, password, password, userName, SignedUp);
	}
	
	void SignedUp(string message, bool hasError)
	{

		if (hasError)
		{
			Debug.Log("Error signign up: "+message);
		}
		else {
			SetStatus("signed up!");
			Debug.Log("signed up!");
		}
		
		SetStatus("signing in");
		Debug.Log("signing in");
		CloudLogin.SignIn(userEmail, password, SignedIn);
	}

	void SignedIn(string message, bool hasError) {
		if (hasError)
		{
			SetStatus("error signing in");
			Debug.Log("Error signign in: "+message);
		}
		else {
			SetStatus("signed in!");
			Debug.Log("signed in!");
			OnLoggedIn?.Invoke();
			isLoggedIn = true;
			CloudLoad();
		}
	}

	private long diskLoadTime;
	[Button]
	public void DiskLoad() {
		SaveFile diskSave;

		var path = GetSaveFilePathAndFileName();
		try {
			if (File.Exists(path)) {
				Debug.Log($"File read from disk");
				diskSave = ReadFile<SaveFile>(path);
			} else {
				Debug.Log($"No Save Data Found");
				diskSave = MakeNewSaveFile();
			}
		} catch {
			File.Delete(path);
			Debug.Log("Corrupt Data Deleted");
			diskSave = MakeNewSaveFile();
		}

		if (diskSave.lastSaveTimeUTC > activeSave.lastSaveTimeUTC) {
			Debug.Log("using disk save before logging in");
			SetStatus("using disk save before logging in");
			activeSave = diskSave;
		}

		diskLoadTime = diskSave.lastSaveTimeUTC;
		
		OnLoadComplete?.Invoke();
		loadingComplete = true;
		Debug.Log("disk load complete!");
	}
	
	[Button]
	public void CloudLoad () {
		var save = Unzip(CloudLoginUser.CurrentUser.GetAttributeValue(saveName));
		SaveFile cloudSave;

		if (save != null && save.Length > 0) {
			cloudSave = ParseJson<SaveFile>(save);
		} else {
			cloudSave = MakeNewSaveFile();
		}

		if (cloudSave.lastSaveTimeUTC > diskLoadTime) {
			Debug.Log("Using cloud save as it is more recent");
			SetStatus("using cloud save as it is more recent");
			activeSave = cloudSave;
		} else {
			Debug.Log("Using disk save as it is more recent");
			SetStatus("using disk save as it is more recent");
		}
		

		OnLoadComplete?.Invoke();
		OnCloudLoadComplete?.Invoke();
		cloudLoadComplete = true;
		loadingComplete = true;
		Debug.Log("cloud load complete!");
		SaveActiveGame();
	}
	

	public static T ReadFile<T>(string path) where T : class, new() {
		StreamReader reader = new StreamReader(path);
		var json = reader.ReadToEnd();
		reader.Close();

		T file = ParseJson<T>(json);

		Debug.Log($"IO OP: file \"{file.GetType()}\" read from \"{path}\"");
		return file;
	}
	
	public static T ParseJson<T>(string json) where T : class, new() {
		fsData serialized = fsJsonParser.Parse(json);

		T file = new T();
		_serializer.TryDeserialize(serialized, ref file).AssertSuccessWithoutWarnings();
		
		return file;
	}


	[Serializable]
	public class SaveFile {
		public bool isRealSaveFile = false;

		public string[] loadoutWordPackNames = new string[3];

		public string gameVersion = "unset";

		public long lastSaveTimeUTC;

		[SerializeField]
		private List<UserWordPackProgress> wordPackData = new List<UserWordPackProgress>();
		
		//public WordPack mainWordPack = new WordPack();

		public int coinCount;

		public List<string> unlockedUpgrades = new List<string>();
		
		public UserWordPackProgress GetProgress(WordPack wordPack) {
			if (wordPackData == null)
				wordPackData = new List<UserWordPackProgress>();
			var index = wordPackData.FindIndex((progress => progress.wordPackName == wordPack.wordPackName));
			if (index != -1) {
				return wordPackData[index];
			} else {
				var progress = new UserWordPackProgress();
				progress.wordPackName = wordPack.wordPackName;
				wordPackData.Add(progress);
				return progress;
			}
		}
	}
	
	
	[Serializable]
	public class UserWordPackProgress {
		public string wordPackName;
		public List<UserWordPairProgress> wordPairData = new List<UserWordPairProgress>();

		public UserWordPairProgress GetWordPairData (WordPair wordPair) {
			return GetWordPairData(wordPair.id);
		}
		UserWordPairProgress GetWordPairData (int wordPairId) {
			if (wordPairData.Count > wordPairId) {
				return wordPairData[wordPairId];
			} else {
				while (wordPairData.Count <= wordPairId) {
					wordPairData.Add(new UserWordPairProgress());
				}
				return wordPairData[wordPairId];
			}
		}
		
		public List<UserWordPairProgress> GetWordPairData () {
			return wordPairData;
		}
	}
	
	[Serializable]
	public class UserWordPairProgress {
		public WordPairProgressType learningMeaningSideType = WordPairProgressType.newWord; // 0=new, 1=learning
		//public WordPairProgressType learningForeignSideType = WordPairProgressType.newWord; // 0=new, 1=learning
		
		public long learningMeaningSide_lastRecallUtcFiletime;
		public long learningForeignSide_lastRecallUtcFiletime;
		public int learningMeaningSide_correctRecallCount;
		public int learningForeignSide_correctRecallCount;
		public int learningMeaningSide_wrongRecallCount;
		public int learningForeignSide_wrongRecallCount;

		public long GetLastRecallUtcFileTime(bool isLearningMeaningSide) {
			Assert.IsTrue(isLearningMeaningSide);
			return isLearningMeaningSide ? learningMeaningSide_lastRecallUtcFiletime : learningForeignSide_lastRecallUtcFiletime;
		}

		public int GetCorrect(bool isLearningMeaningSide) {
			Assert.IsTrue(isLearningMeaningSide);
			return isLearningMeaningSide ? learningMeaningSide_correctRecallCount : learningForeignSide_correctRecallCount;
		}

		public int GetWrong(bool isLearningMeaningSide) {
			Assert.IsTrue(isLearningMeaningSide);
			return isLearningMeaningSide ? learningMeaningSide_wrongRecallCount : learningForeignSide_wrongRecallCount;
		}
		
		public void SetLastRecallUtcFileTime(bool isLearningMeaningSide, long value) {
			Assert.IsTrue(isLearningMeaningSide);
			if (isLearningMeaningSide) {
				learningMeaningSide_lastRecallUtcFiletime = value;
			} else {
				learningForeignSide_lastRecallUtcFiletime = value;
			}
		}

		public void Increment(bool isLearningMeaningSide, bool isCorrect) {
			Assert.IsTrue(isLearningMeaningSide);
			if (isCorrect) {
				if (isLearningMeaningSide) {
					learningMeaningSide_correctRecallCount += 1;
				} else {
					learningForeignSide_correctRecallCount += 1;
				}
			} else {
				if (isLearningMeaningSide) {
					learningMeaningSide_wrongRecallCount += 1;
				} else {
					learningForeignSide_wrongRecallCount += 1;
				}
			}
			
			SetLastRecallUtcFileTime(isLearningMeaningSide, DateTime.Now.ToFileTimeUtc());
		}
	}
	
	
	public static void CopyTo(Stream src, Stream dest) {
		byte[] bytes = new byte[4096];

		int cnt;

		while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
			dest.Write(bytes, 0, cnt);
		}
	}

	public static string Zip(string str) {
		var bytes = Encoding.UTF8.GetBytes(str);

		using (var msi = new MemoryStream(bytes))
		using (var mso = new MemoryStream()) {
			using (var gs = new GZipStream(mso, CompressionMode.Compress)) {
				//msi.CopyTo(gs);
				CopyTo(msi, gs);
			}

			return Convert.ToBase64String(mso.ToArray());
		}
	}

	public static string Unzip(string zip) {
		var bytes = Convert.FromBase64String(zip);
		using (var msi = new MemoryStream(bytes))
		using (var mso = new MemoryStream()) {
			using (var gs = new GZipStream(msi, CompressionMode.Decompress)) {
				//gs.CopyTo(mso);
				CopyTo(gs, mso);
			}

			return Encoding.UTF8.GetString(mso.ToArray());
		}
	}
}

public enum WordPairProgressType {
	newWord = 0, learning = 1 
}
