using Google.Cloud.Speech.V1;
using Google.Apis.Auth.OAuth2;
using System;

namespace SpeechToTextApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\Users\\hxt\\source\\repos\\Marriot Solution\\VoiceRecognition\\VoiceRecognition\\key.json");
            var speech = SpeechClient.Create();
            var config = new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                SampleRateHertz = 16000,
                LanguageCode = LanguageCodes.English.UnitedStates
            };
            var audio = RecognitionAudio.FromStorageUri("gs://cloud-samples-tests/speech/brooklyn.flac");

            var response = speech.Recognize(config, audio);

            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine(alternative.Transcript);
                }
            }
        }
    }
}