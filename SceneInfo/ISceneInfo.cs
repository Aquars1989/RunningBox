using System;
namespace RunningBox
{
    public interface ISceneInfo
    {
        string SceneName { get; }
        int CountOfChallenge { get; }
        int CountOfComplete { get; }
        SceneGaming CreateScene(int level);
        int HighScore { get; }
        int MaxLevel { get; }
        string SceneID { get; }
        void Settlement(int level, long timeOfChallenge, int score, bool complete);
        void SetValue(int level, int countOfChallenge, long timeOfChallenge, int highScore, bool complete);
        long TimeOfChallenge { get; }
        bool GetComplete(int level);
        int GetCountOfChallenge(int level);
        long GetTimeOfChallenge(int level);
        int GetHighScore(int level);
    }
}
