using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using Newtonsoft.Json.Linq;
using System.Linq;

public class SpeechToTextTranscriber
{
    public static async Task Main()
    {
        // Set the path to your Google Cloud JSON credentials file
        string credentialsFilePath = "C:\\Users\\hxt\\source\\repos\\Marriot Solution\\VoiceRecognition\\VoiceRecognition\\key.json";
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsFilePath);

        string inputAudioFilePath = "C:\\Users\\hxt\\source\\repos\\Marriot Solution\\VoiceRecognition\\VoiceRecognition\\test10.mp3";
        await TranscribeAudioAsync(inputAudioFilePath);
    }

    private static async Task TranscribeAudioAsync(string inputAudioFilePath)
    {
        var speechClient = SpeechClient.Create();
        var diariConfig = new SpeakerDiarizationConfig
        {
            EnableSpeakerDiarization = true,
            MinSpeakerCount = 2,
            MaxSpeakerCount = 6
        };

        var config = new RecognitionConfig
        {
            SampleRateHertz = 16000, // Set the appropriate language code
            EnableAutomaticPunctuation = true,
            LanguageCode = LanguageCodes.Vietnamese.Vietnam,
            Model = "latest_long",
            DiarizationConfig = diariConfig,
            UseEnhanced = true,
        };

        var audio = RecognitionAudio.FromFile(inputAudioFilePath);
        var response = await speechClient.LongRunningRecognizeAsync(config, audio);

        // Poll for the completed response (or check the operation's status)
        var completedResponse = response.PollUntilCompleted();
        if (!completedResponse.IsCompleted)
        {
            Console.WriteLine("Error: " + completedResponse.Exception);
            return;
        }

        /*var lastResult = completedResponse.Result.Results[completedResponse.Result.Results.Count - 1];
        var wordsInfo = lastResult.Alternatives[0].Words;

        int tag = 1;
        string speaker = "";
        string transcript = "";

        foreach (var wordInfo in wordsInfo)
        {
            if (wordInfo.SpeakerTag == tag)
            {
                speaker += " " + wordInfo.Word;
            }
            else
            {
                transcript += $"speaker {tag}: {speaker}\n";
                tag = wordInfo.SpeakerTag;
                speaker = wordInfo.Word;

                
            }
        }

        transcript += $"speaker {tag}: {speaker}";
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine(transcript);*/

        var speakersTranscripts = new Dictionary<int, List<string>>();
        Console.OutputEncoding = Encoding.UTF8;
        foreach (var wordInfo in completedResponse.Result.Results[completedResponse.Result.Results.Count - 1].Alternatives[0].Words)
            {
                if (!speakersTranscripts.ContainsKey(wordInfo.SpeakerTag))
                {
                    speakersTranscripts[wordInfo.SpeakerTag] = new List<string>();
                }
               // Console.WriteLine(wordInfo.Word + "  " + wordInfo.SpeakerTag);
                speakersTranscripts[wordInfo.SpeakerTag].Add(wordInfo.Word);
            }

        string[] myCombination = { "vật", "chất"};
        Console.WriteLine($"Tổ hợp từ cần tìm  {string.Join(" ", myCombination)}");
        Console.WriteLine("\n");


        foreach (var speaker in speakersTranscripts.Keys)
        {
            Console.WriteLine($"Speaker {speaker}: {string.Join(" ", speakersTranscripts[speaker])}");
            if (ContainsExactCombination(speakersTranscripts[speaker].ToArray(), myCombination.ToArray()))
            {
                Console.WriteLine("Found the exact combination of items");

                Console.WriteLine("\n");
                Console.WriteLine("\n");
            }
            else
            {
                Console.WriteLine("Did not find the exact combination of items");

                Console.WriteLine("\n");

                Console.WriteLine("\n");
            }
        } 
    }
    public static bool ContainsExactCombination(string[] array, string[] combination)
    {
        for (int i = 0; i <= array.Length - combination.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < combination.Length; j++)
            {
                if (array[i + j] != combination[j])
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                return true;
            }
        }
        return false;
    }
}
