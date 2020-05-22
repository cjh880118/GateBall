#define LOAD_FROM_ASSETBUNDLE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JHchoi.Common;
using DG.Tweening;
using JHchoi.Models;

namespace JHchoi
{
    public enum SoundType
    {
        None = 0,
        Title_BGM,
        Button_Input,
        Title_Voice1,
        Title_Voice2,
        Title_Voice3,
        Title_Voice4,
        LobbyMode_Select,
        Mission_Level_Select,
        Bet_Bird_Effect,
        Missoin_Bird_Effect,
        Mission_Player_White,
        Mission_Player_Red,
        Mission_Gate1,
        Mission_Gate2,
        Mission_Gate3,
        Mission_Pole,
        Mission_Spark,
        Mission_Spark_Red,
        Mission_Spark_White,
        Mission_Success1,
        Mission_Success2,
        Mission_Success3,
        Mission_Success4,
        Mission_Success5,
        Mission_Success_Effect,
        Mission_Popup,
        Gate_Touch,
        Ball_Touch1,
        Ball_Touch2,
        Pole_Touch,
        Game_Result1,
        Game_Result2,
        Wave_Effect1,
        Wave_Effect2,
        Player_Order,
        Mission_Gate,
        Mission_Touch,
        Player_Ready,
        Crowd_Effect,
        Mission_Fail,
        Score_Result,
        Mission_BGM,
    }

    public class SoundManager : MonoSingleton<SoundManager>
    {
        public float volume = 1.0f;
        public float fadeDuration = 1.0f;

        BT_Sound _table;

        bool _loadComplete = false;
        public bool IsComplete { get { return _loadComplete; } }

        class ClipCache
        {
            public string resourceName;
            public BT_Sound.Param data;
            public AudioClip clip;
            public bool instant;
            public bool bgm;
        }

        readonly Dictionary<int, ClipCache> _caches = new Dictionary<int, ClipCache>();

        readonly ObjectPool<AudioSource> _audioSourcePool = new ObjectPool<AudioSource>();

        class PlayingAudio
        {
            public ClipCache clipCache;
            public AudioSource audioSource;
        }
        readonly LinkedList<PlayingAudio> _playingAudio = new LinkedList<PlayingAudio>();

        public IEnumerator Setup()
        {
            _loadComplete = false;
            _table = TableManager.Instance.GetTableClass<BT_Sound>();

            for (int i = 0; i < _table.sheets[0].list.Count; i++)
                yield return StartCoroutine(Load(_table.sheets[0].list[i].Index));

            _loadComplete = true;
        }

        public IEnumerator Load(int id, bool instant = true, bool bgm = false)
        {
            if (id == 0)
                yield break;

            if (_caches.ContainsKey(id))
                yield break;

            if (_table == null)
            {
                _table = TableManager.Instance.GetTableClass<BT_Sound>();
                if (_table == null)
                {
                    //yield return StartCoroutine(TableManager.Instance.Load());
                    _table = TableManager.Instance.GetTableClass<BT_Sound>();

                    if (_table == null)
                        yield break;
                }
            }

            //var data = _table.Rows.Find(x => x.SoundID == id);
            var data = _table.sheets[0].list.Find(x => x.Index == id);
            if (data == null)
            {
                Debug.LogErrorFormat("Could not found 'BT_SoundRow' : {0} of {1}", id, gameObject.name);
                yield break;
            }

            string path = Model.First<SettingModel>().GetLocalizingPath();

            string fullpath = string.Format("Sound/{0}/{1}", path, data.FileName);
            yield return StartCoroutine(ResourceLoader.Instance.Load<AudioClip>(fullpath,
                o => OnPostLoadProcess(o, fullpath, id, data, instant, bgm)));
        }

        void OnPostLoadProcess(Object o, string name, int id, BT_Sound.Param data, bool instant, bool bgm)
        {
            if (!_caches.ContainsKey(id))
            {
                var sound = bgm ? o as AudioClip : Instantiate(o) as AudioClip;
                _caches.Add(id, new ClipCache { resourceName = name, data = data, clip = sound, instant = instant, bgm = bgm });
            }
        }

        public void PlaySound(int id, bool fade = false)
        {
            //Debug.LogFormat("PlaySound - {0}", id);

            ClipCache cache;
            if (_caches.TryGetValue(id, out cache))
            {
                var source = _audioSourcePool.GetObject() ?? gameObject.AddComponent<AudioSource>();
                _playingAudio.AddLast(new PlayingAudio { clipCache = cache, audioSource = source });

                source.clip = cache.clip;
                source.loop = cache.data.Loop;
                source.volume = fade ? 0.0f : cache.data.Volum;

                source.Play();
                //Debug.LogFormat("PlaySound - {0} - {1} - OK", id, source.clip.name);

                if (fade)
                    source.DOFade(cache.data.Volum, fadeDuration);
            }
        }

        public void SoundVolumeReset(int id)
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id)
                {
                    ClipCache cache;
                    if (_caches.TryGetValue(id, out cache))
                    {
                        audio.audioSource.volume =  cache.data.Volum;
                    }
                }
                break;
            }
            node = node.Next;
        }

        public void SoundVolume(int id, float volume)
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id)
                {
                    audio.audioSource.volume = volume;
                }
                break;
            }
            node = node.Next;
        }


        public void StopSound(int id, bool fade = false)
        {
            //Debug.LogFormat("StopSound - {0}", id);

            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id)
                {
                    if (fade)
                    {
                        audio.audioSource.DOFade(0.0f, fadeDuration).OnComplete(
                            () =>
                            {
                                audio.audioSource.Stop();
                                audio.audioSource.clip = null;

                                _audioSourcePool.PoolObject(audio.audioSource);
                                _playingAudio.Remove(node);
                            });
                    }
                    else
                    {
                        audio.audioSource.Stop();
                        audio.audioSource.clip = null;

                        _audioSourcePool.PoolObject(audio.audioSource);
                        _playingAudio.Remove(node);
                    }

                    //Debug.LogFormat("StopSound - {0} - {1} - OK", id, audio.audioSource.clip.name);
                    break;
                }

                node = node.Next;
            }
        }

        public bool IsPlaySound(int id)
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id)
                {
                    return audio.audioSource.isPlaying;
                }
                node = node.Next;
            }
            return false;
        }

        public void StopAllSound()
        {
            //Debug.LogFormat("StopAllSound");

            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                audio.audioSource.Stop();
                audio.audioSource.clip = null;

                _audioSourcePool.PoolObject(audio.audioSource);

                node = node.Next;
            }

            _playingAudio.Clear();
        }

        void LateUpdate()
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                //if (audio.clipCache.instant && !audio.audioSource.isPlaying)
                if (!audio.audioSource.isPlaying)
                {
                    //if (!audio.audioSource.loop)
                    {
                        //Debug.LogFormat("LateUpdate - {0} - End", audio.audioSource.clip.name);

                        audio.audioSource.Stop();
                        audio.audioSource.clip = null;

                        _audioSourcePool.PoolObject(audio.audioSource);
                        _playingAudio.Remove(node);
                    }
                }

                node = node.Next;
            }
        }

        protected override void Release()
        {
            StopAllSound();
            UnloadAllLoadCaches();
        }

        public void UnloadAllInstantCaches()
        {
            var unloadList = new List<int>();

            foreach (var cache in _caches)
            {
                if (cache.Value.instant)
                {
                    Debug.LogFormat("UnloadAllInstantCaches - {0} - {1} - OK", cache.Value.data.Index, cache.Value.clip.name);

                    Destroy(cache.Value.clip);
                    cache.Value.clip = null;

                    unloadList.Add(cache.Value.data.Index);
                }
                else
                    Debug.LogFormat("UnloadAllInstantCaches - {0} - {1} - NO", cache.Value.data.Index, cache.Value.clip.name);
            }

            for (int i = 0; i < unloadList.Count; ++i)
            {
                _caches.Remove(unloadList[i]);
            }
        }

        public void UnloadAllLoadCaches()
        {
            foreach (var cache in _caches)
            {
                ResourceLoader.Instance.Unload(cache.Value.resourceName);

                if (!cache.Value.bgm)
                    Destroy(cache.Value.clip);

                cache.Value.clip = null;
            }

            _caches.Clear();
        }
    }
}
