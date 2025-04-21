using Newtonsoft.Json;

namespace Minesweeper.Utilities;

public static class JsonUtility {
    private static readonly JsonSerializerSettings _options
        = new() {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };


    public static void Read<T>(string fileName, out T? obj) {
        using var sr = new StreamReader(fileName);
        obj = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
    }

    public static bool ReadSafe<T>(string fileName, ref T? obj) {
        if (File.Exists(fileName)) {
            Read(fileName, out obj);
            return true;
        }

        return false;
    }

    public static void Write<T>(string fileName, T data) {
        var json = JsonConvert.SerializeObject(data, _options);
        File.WriteAllText(fileName, json);
    }

    public static void Clear(string fileName) {
        if (File.Exists(fileName))
            File.Delete(fileName);
        File.Create(fileName).Close();
    }

    public static void WriteSafe<T>(string fileName, T data) {
        Clear(fileName);
        Write(fileName, data);
    }
}