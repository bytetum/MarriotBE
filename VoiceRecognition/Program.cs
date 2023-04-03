using Google.Cloud.Speech.V1;
using Google.Apis.Auth.OAuth2;
using System;
using System.Text;

namespace SpeechToTextApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\Users\\hxt\\source\\repos\\Marriot Solution\\VoiceRecognition\\VoiceRecognition\\key.json");
            var speech = SpeechClient.Create();
            var DiarizationConfig1 = new SpeakerDiarizationConfig
            {
                EnableSpeakerDiarization = true,
                MinSpeakerCount = 1,
                MaxSpeakerCount = 6,
            };
            var config = new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.EncodingUnspecified,
                SampleRateHertz = 16000,
                EnableSpokenPunctuation = true,
                EnableAutomaticPunctuation = true,
                DiarizationConfig = DiarizationConfig1,
                EnableWordConfidence = true,
                LanguageCode = LanguageCodes.English.UnitedKingdom
            };
            var audio = RecognitionAudio.FromFile("C:\\Users\\hxt\\source\\repos\\Marriot Solution\\VoiceRecognition\\VoiceRecognition\\short-test.mp3");

            var response = speech.Recognize(config, audio);

            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    foreach (var work in alternative.Words)
                    {
                        Console.OutputEncoding = Encoding.UTF8;
                        Console.WriteLine("Work: " + work.Word + "   |   "+ "Confident: " + work.Confidence);
                    }

                    Console.WriteLine(alternative.Transcript);
                    Console.WriteLine("End a sentence \n");
                }
                    
            }
        }
    }
}