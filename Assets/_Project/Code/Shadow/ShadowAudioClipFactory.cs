using UnityEngine;

namespace ADoorInsideTheDark.Shadow
{
    public static class ShadowAudioClipFactory
    {
        private const int SampleRate = 44100;

        public static AudioClip CreatePerceptionEnterCue()
        {
            return CreateOneShotClip(
                "ShadowPerceptionEnterCue",
                0.11f,
                sampleTime =>
                {
                    float baseTone = Mathf.Sin(2f * Mathf.PI * 392f * sampleTime) * 0.18f;
                    float harmonic = Mathf.Sin(2f * Mathf.PI * 587.33f * sampleTime) * 0.08f;
                    return baseTone + harmonic;
                });
        }

        public static AudioClip CreatePerceptionExitCue()
        {
            return CreateOneShotClip(
                "ShadowPerceptionExitCue",
                0.09f,
                sampleTime =>
                {
                    float baseTone = Mathf.Sin(2f * Mathf.PI * 293.66f * sampleTime) * 0.14f;
                    float harmonic = Mathf.Sin(2f * Mathf.PI * 220f * sampleTime) * 0.06f;
                    return baseTone + harmonic;
                });
        }

        public static AudioClip CreateRevealLoop()
        {
            int sampleCount = Mathf.CeilToInt(SampleRate * 0.5f);
            AudioClip clip = AudioClip.Create("ShadowRevealLoop", sampleCount, 1, SampleRate, false);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float sampleTime = (float)i / SampleRate;
                float baseTone = Mathf.Sin(2f * Mathf.PI * 240f * sampleTime) * 0.10f;
                float shimmer = Mathf.Sin(2f * Mathf.PI * 480f * sampleTime) * 0.035f;
                samples[i] = baseTone + shimmer;
            }

            clip.SetData(samples, 0);
            return clip;
        }

        public static AudioClip CreatePerceptionBlockedCue()
        {
            return CreateOneShotClip(
                "ShadowPerceptionBlockedCue",
                0.12f,
                sampleTime =>
                {
                    float baseTone = Mathf.Sin(2f * Mathf.PI * 196f * sampleTime) * 0.16f;
                    float overtone = Mathf.Sin(2f * Mathf.PI * 233.08f * sampleTime) * 0.06f;
                    return baseTone + overtone;
                });
        }

        public static AudioClip CreateDestabilizedGuidanceCue()
        {
            return CreateOneShotClip(
                "ShadowDestabilizedGuidanceCue",
                0.22f,
                sampleTime =>
                {
                    float baseTone = Mathf.Sin(2f * Mathf.PI * 261.63f * sampleTime) * 0.12f;
                    float shimmer = Mathf.Sin(2f * Mathf.PI * 392f * sampleTime) * 0.08f;
                    return baseTone + shimmer;
                });
        }

        private static AudioClip CreateOneShotClip(string clipName, float durationSeconds, System.Func<float, float> sampleProvider)
        {
            int sampleCount = Mathf.CeilToInt(SampleRate * durationSeconds);
            AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, SampleRate, false);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float normalized = sampleCount > 1 ? (float)i / (sampleCount - 1) : 0f;
                float envelope = Mathf.Sin(normalized * Mathf.PI);
                float sampleTime = (float)i / SampleRate;
                samples[i] = sampleProvider(sampleTime) * envelope;
            }

            clip.SetData(samples, 0);
            return clip;
        }
    }
}
