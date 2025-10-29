using Services.Core;

namespace Services.Audio.Core
{
    public interface IAudioService : IService
    {
        public void PlayAudioFX(AudioFX audioFX);
    }
}
