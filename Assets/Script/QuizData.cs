using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyCornect
{
    public class QuizData
    {
        public List<ConnectionQuestion> ConnectionRound { get; set; }
        public List<SequenceQuestion> SequencesRound { get; set; }
        public List<List<WallQuestion>> WallRound { get; set; }
        public List<MissingVowelsQuestion> MissingVowelsRound { get; set; }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public class Question
    {
        public string Connection { get; set; }
        public List<string> Clues { get; set; } 
    }

    public class PictureQuestion : Question
    {
        public List<string> Pictures { get; set; }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public class ConnectionQuestion : PictureQuestion
    {
    }

    public class SequenceQuestion : PictureQuestion
    {
    }

    public class WallQuestion : Question
    {
    }

    public class MissingVowelsQuestion : Question
    {
        public bool Tiebreaker;
    }
}
