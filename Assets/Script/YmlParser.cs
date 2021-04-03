﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OnlyCornect
{
    class YmlParser : MonoBehaviour
    {
        public static QuizData ParseQuiz()
        {
            string filepath = Application.streamingAssetsPath + "/Questions.yml";
            QuizData quiz = null;

            using (StreamReader reader = new StreamReader(filepath))
            {
                var deserializer = new DeserializerBuilder()
                    .Build();

                quiz = deserializer.Deserialize<QuizData>(reader);
            }

            return quiz;
        }
    }
}
