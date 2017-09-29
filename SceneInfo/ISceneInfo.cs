using System;
namespace RunningBox
{
    public interface ISceneInfo
    {
        string SceneName { get; }
        int CountOfChallenge { get; }
        int CountOfComplete { get; }
        SceneGaming CreateScene(int level);
        int HighPlayingTime { get; }
        int HighScore { get; }
        int MaxLevel { get; }
        string SceneID { get; }
        void Settlement(int level, int surviveTime, int score, bool complete);
        void SetValue(int level, int countOfChallenge, long timeOfChallenge, int highSurviveTime, int highScore, bool complete);
        void Settlement(ScenePlayingInfo playInfo);
        long TimeOfChallenge { get; }
        bool GetComplete(int level);
        int GetCountOfChallenge(int level);
        long GetTimeOfChallenge(int level);
        int GetPlayingTimeLimit(int level);
        int GetHighPlayingTime(int level);
        int GetHighScore(int level);
        void WriteRegistry(int level);
        void ReadRegistry(int level);
        void AllWriteRegistry();
        void AllReadRegistry();
    }
}
