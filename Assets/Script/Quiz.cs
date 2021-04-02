using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyCornect
{
    [System.Serializable]
    public class QuizData
    {
        public List<ConnectionQuestion> ConnectionRound { get; set; }
        public List<SequenceQuestion> SequencesRound { get; set; }
        public List<List<WallQuestion>> WallRound { get; set; }
        public List<MissingVowelsQuestion> MissingVowelsRound { get; set; }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class Question
    {
        public string Connection { get; set; }
        public List<string> Clues { get; set; }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class ConnectionQuestion : Question
    {

    }

    [System.Serializable]
    public class SequenceQuestion : Question
    {

    }

    [System.Serializable]
    public class WallQuestion : Question
    {

    }

    [System.Serializable]
    public class MissingVowelsQuestion : Question
    {

    }

}
