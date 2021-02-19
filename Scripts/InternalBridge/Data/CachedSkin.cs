using UnityEditor;
using UnityEngine;

namespace UniSkin
{
    [FilePath("cachedSkin", FilePathAttribute.Location.PreferencesFolder)]
    internal class CachedSkin : ScriptableSingleton<CachedSkin>
    {
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

        public static bool Update(Skin skin)
        {
            if (instance._skin == skin)
            {
                return false;
            }
            else
            {
                instance._skin = skin;
                EditorUtility.SetDirty(instance);
                _dirty = true;

                return true;
            }
        }

        public static void Save()
        {
            if (!_dirty) return;

            instance.Save(saveAsText: true);
            _dirty = false;
        }
    }
}
