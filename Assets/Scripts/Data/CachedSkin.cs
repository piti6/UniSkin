using System;
using UnityEngine;

namespace UniSkin
{
    [UnityEditor.FilePath("cachedSkin.skn", UnityEditor.FilePathAttribute.Location.PreferencesFolder)]
    public class CachedSkin : UnityEditor.ScriptableSingleton<CachedSkin>
    {
        public static Action OnUpdated = () => { };
        public static Skin Skin
        {
            get
            {
                if (instance._skin is null)
                {
                    Update(Skin.Default);
                    Save();
                }

                return instance._skin;
            }
        }

        [SerializeField]
        private Skin _skin;

        private static bool _dirty = false;

        public static void Update(Skin skin)
        {
            instance._skin = skin;
            _dirty = true;
            OnUpdated.Invoke();
        }

        public static void Save()
        {
            if (!_dirty) return;

            instance.Save(saveAsText: true);
            _dirty = false;
        }
    }
}
