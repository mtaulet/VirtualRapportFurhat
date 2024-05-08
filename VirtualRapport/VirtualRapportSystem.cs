using System.Text;
using Microsoft.Psi;
using HeadGestureData = OpenSense.Components.OpenFace.Gesture;
using AcousticFeatures = OpenSense.Components.OpenSmile.Vector<float>;
using System.Web;


namespace OpenSense.Components.VirtualRapport {
    public enum FurhatGesture {
        Nod,
        Shake,
        GazeAway
    }
    public sealed class VirtualRapportSystem : IProducer<FurhatGesture> {
        public Receiver<HeadGestureData> HeadGestureIn { get; }
        public Receiver<bool> SpeechActivityIn { get; }
        public Receiver<AcousticFeatures> AcousticFeaturesIn { get; }
        public Emitter<FurhatGesture> Out { get; private set; }


        public VirtualRapportSystem(Pipeline pipeline) {
            HeadGestureIn = pipeline.CreateReceiver<HeadGestureData>(this, ProcessHeadGesture, nameof(HeadGestureIn));
            SpeechActivityIn = pipeline.CreateReceiver<bool>(this, ProcessSpeechActivity, nameof(SpeechActivityIn));
            AcousticFeaturesIn = pipeline.CreateReceiver<AcousticFeatures>(this, ProcessSound, nameof(AcousticFeaturesIn));

            Out = pipeline.CreateEmitter<FurhatGesture>(this, nameof(Out));
        }


        public static string furhatUri = "http://172.16.31.15:54321/furhat";
        private int nods = 0, tilts = 0, shakes = 0, none = 0;
        private double lastLoudness = 0;
        public const double LoudnessSlopeThreshold = 0.2;
        public const double PitchSlopeThreshold = 0.2;
        public DateTime lastGestureTimestamp = new DateTime(DateTime.Now.Ticks);
        public DateTime lastSpeechActivity = new DateTime(DateTime.Now.Ticks);
        public bool speechPaused = false;

        private void ProcessSound(AcousticFeatures data, Envelope envelope) {
            float pitch = data.Fields[461].Data[0];        // F0_sma_amean
            float pitchSlope = data.Fields[462].Data[0];   // F0_sma_linregc1
            float loudness = data.Fields[24].Data[0];      // pcm_loudness_sma_amean    
            float loudnessSlope = data.Fields[25].Data[0]; // pcm_loudness_sma_linregc1

            // RAISED LOUDNESS
            if (loudness > 0 && loudnessSlope > LoudnessSlopeThreshold) {
                Out.Post(FurhatGesture.Nod, DateTime.Now);
            }
            // LOWERING OF PITCH
            else if (pitch > 90 && pitchSlope < -PitchSlopeThreshold) {
                Out.Post(FurhatGesture.Nod, DateTime.Now);
            }
        }


        private void ProcessHeadGesture(HeadGestureData data, Envelope envelope) {
            if (data == HeadGestureData.Shake) {
                Out.Post(FurhatGesture.Shake, DateTime.Now);
            }
            else if (data == HeadGestureData.Nod) {
                Out.Post(FurhatGesture.Nod, DateTime.Now);
            }
        }
        private void ProcessSpeechActivity(bool data, Envelope envelope) {
            if ((data == false) && ((DateTime.Now - lastSpeechActivity).TotalSeconds > 3)) {
                Out.Post(FurhatGesture.GazeAway, DateTime.Now);
                lastSpeechActivity = DateTime.Now;
            }
            if (data == true) {
                lastSpeechActivity = DateTime.Now;
            }

        }
    }
}
