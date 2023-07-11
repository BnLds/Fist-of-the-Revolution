using UnityEngine;
 using System.Collections.Generic;
 
public static class Leaderboard 
{
    private const int ENTRY_COUNT = 20;
    private const string PLAYER_PREFS_LEADERBOARD = "leaderboard";
 
    public struct ScoreEntry 
    {
        public string Name;
        public int Score;
 
        public ScoreEntry(string name, int score) 
        {
            this.Name = name;
            this.Score = score;
        }
    }
 
    private static List<ScoreEntry> _Entries;
 
    private static List<ScoreEntry> Entries 
    {
        get {
            if (_Entries == null) 
            {
                _Entries = new List<ScoreEntry>();
                LoadScores();
            }
            return _Entries;
        }
    }
 
 
    private static void SortScores() 
    {
        _Entries.Sort((a, b) => b.Score.CompareTo(a.Score));
    }
 
    private static void LoadScores() 
    {
        _Entries.Clear();
 
        for (int i = 0; i < ENTRY_COUNT; ++i) 
        {
            ScoreEntry entry;
            entry.Name = PlayerPrefs.GetString(PLAYER_PREFS_LEADERBOARD + "[" + i + "].name", "");
            entry.Score = PlayerPrefs.GetInt(PLAYER_PREFS_LEADERBOARD + "[" + i + "].score", 0);
            _Entries.Add(entry);
        }
 
        SortScores();
    }
 
    private static void SaveScores() 
    {
        for (int i = 0; i < ENTRY_COUNT; ++i) 
        {
            var entry = _Entries[i];
            PlayerPrefs.SetString(PLAYER_PREFS_LEADERBOARD + "[" + i + "].name", entry.Name);
            PlayerPrefs.SetInt(PLAYER_PREFS_LEADERBOARD + "[" + i + "].score", entry.Score);
        }
    }
 
    public static ScoreEntry GetEntry(int index) 
    {
        return Entries[index];
    }

    public static int GetEntriesCount()
    {
        int count = 0;
        for(int i = 0; i < ENTRY_COUNT; i++)
        {
            if(Entries[i].Name == "" && Entries[i].Score == 0) return count;
            count++;
        }

        return ENTRY_COUNT;
    }
 
    public static void Record(string name, int score) 
    {
        Entries.Add(new ScoreEntry(name, score));
        SortScores();
        Entries.RemoveAt(Entries.Count - 1);
        SaveScores();
    }

    public static bool CheckScore(int currentScore) 
    {
        for (int i = 0; i < ENTRY_COUNT; ++i) {
            if(Entries[i].Score < currentScore)
                {
                    return true;
                }
        }
        return false;
    }
}
