using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;

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

        var lastResult = completedResponse.Result.Results[completedResponse.Result.Results.Count - 1];
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
        Console.WriteLine(transcript);

        /*var speakersTranscripts = new Dictionary<int, List<string>>();
        
        foreach (var result in completedResponse.Result.Results)
        {
            foreach (var wordInfo in result.Alternatives[0].Words)
            {
                if (!speakersTranscripts.ContainsKey(wordInfo.SpeakerTag))
                {
                    speakersTranscripts[wordInfo.SpeakerTag] = new List<string>();
                }

                speakersTranscripts[wordInfo.SpeakerTag].Add(wordInfo.Word);
            }
        }

        foreach (var speaker in speakersTranscripts.Keys)
        {
            Console.WriteLine($"Speaker {speaker}: {string.Join(" ", speakersTranscripts[speaker])}");
        } */
    }
}
