using System;
using System.Linq;
using SystemBase.CommonSystems.Audio.Actions;
using SystemBase.CommonSystems.Audio.Helper;
using SystemBase.Core;
using SystemBase.Utils;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SystemBase.CommonSystems.Audio
{
    [GameSystem]
    public class AudioSystem : GameSystem<SFXComponent, BackgroundMusicComponent>
    {
        private readonly BoolReactiveProperty _musicIsMuted = new BoolReactiveProperty(false);
        private readonly FloatReactiveProperty _musicVolume = new FloatReactiveProperty(.8f);
        private readonly BoolReactiveProperty _sfxIsMuted = new BoolReactiveProperty(false);
        private readonly FloatReactiveProperty _sfxVolume = new FloatReactiveProperty(1f);

        private BackgroundMusicComponent _currentMusic;
        private BackgroundMusicComponent _lastMusic;

        public override void Init()
        {
            base.Init();

            MessageBroker.Default
                .Receive<AudioActSFXSetMute>()
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Select(mute => mute.IsMuted)
                .Subscribe(b => _sfxIsMuted.Value = b);
             
            MessageBroker.Default
                .Receive<AudioActMusicSetMute>()
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Select(mute => mute.IsMuted)
                .Subscribe(b => _musicIsMuted.Value = b);

            MessageBroker.Default
                .Receive<AudioActMusicSetVolume>()
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Select(volume => volume.Volume)
                .Subscribe(newVolume => _musicVolume.Value = newVolume);

            MessageBroker.Default
                .Receive<AudioActSFXSetVolume>()
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Select(volume => volume.Volume)
                .Subscribe(newVolume => _sfxVolume.Value = newVolume);
        }

        public override void Register(SFXComponent component)
        {
            MessageBroker.Default
                .Receive<AudioActSFXPlay>()
                .Where(_ => !_sfxIsMuted.Value)
                .DistinctUntilChanged(IoC.Resolve<ISFXComparer>())
                .Subscribe(play => PlaySFX(component, play.Parameters)(play.Name))
                .AddTo(component);
        }

        public override void Register(BackgroundMusicComponent component)
        {
            MessageBroker.Default
                .Receive<AudioActMusicStart>()
                .Subscribe(start =>
                {
                    if (start.CrossFadeTime == TimeSpan.Zero)
                    {
                        _currentMusic.Source.Stop();
                        _currentMusic = component;
                        _currentMusic.Source.Play();
                    }
                    else
                    {
                        _lastMusic = _currentMusic;
                        _currentMusic = component;
                        _currentMusic.Source.volume = 0;
                        _currentMusic.Source.Play();
                        FadeIn(_currentMusic, start.CrossFadeTime, 0f);// TODO
                        FadeOut(_lastMusic, start.CrossFadeTime);
                    }
                })
                .AddTo(component);

            MessageBroker.Default
                .Receive<AudioActMusicStop>()
                .Subscribe(stop => { })
                .AddTo(component);

            _musicIsMuted
                .Subscribe(b => component.Source.mute = b)
                .AddTo(component);

            _musicVolume
                .Subscribe(f => component.Source.volume = f)
                .AddTo(component);
        }

        private static void FadeOut(BackgroundMusicComponent bgMusic, TimeSpan time)
        {
            var stepPerSecond = bgMusic.Source.volume / time.TotalSeconds;
            var fadeOut = Observable.EveryUpdate()
                .Subscribe(deltaTime => bgMusic.Source.volume -= (float) stepPerSecond * deltaTime);
            Observable.Timer(time).Subscribe(l => fadeOut.Dispose());
        }

        private static void FadeIn(BackgroundMusicComponent bgMusic, TimeSpan time, float targetVolume)
        {
            var stepPerSecond = targetVolume / time.TotalSeconds;
            var fadeIn = Observable.EveryUpdate()
                .Subscribe(dettaTime => bgMusic.Source.volume += (float) stepPerSecond * dettaTime);
            Observable.Timer(time).Subscribe(l => fadeIn.Dispose());
        }

        private void RemoveSourceAfterStopped(AudioSource source, SoundFile file)
        {
            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .TakeWhile(_ => source.isPlaying)
                .Subscribe(_ =>
                {
                    source.volume = file.Volume * _sfxVolume.Value;
                }, () => Object.Destroy(source));
        }

        private Action<string> PlaySFX(SFXComponent component, PlaySFXParameters parameters)
        {
            return name =>
            {
                var soundFile = component.Sounds.FirstOrDefault(file => file.Name == name);
                if (soundFile != null)
                {
                    var source = component.gameObject.AddComponent<AudioSource>();
                    source.pitch = 1 + (UnityEngine.Random.value - 0.5f) * 2f * component.MaxPitchChange;
                    source.loop = parameters?.Loop ?? false;
                    source.clip = soundFile.File;
                    source.volume = soundFile.Volume * _sfxVolume.Value;
                    if(source.loop) source.Play();
                    else source.PlayOneShot(soundFile.File, soundFile.Volume * _sfxVolume.Value);
                    RemoveSourceAfterStopped(source, soundFile);
                }
                else
                {
                    Debug.Log("Can't find Sound with Name: " + name);
                }
            };
        }
    }
}