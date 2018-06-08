using UnityEngine;

namespace Scripts.Core.Config
{
	public partial class GameConfig
	{
		public string CONFIG_STORAGE_DIRECTORY
		{
			get
			{
				if (string.IsNullOrEmpty(_cachedConfigPath))
				{
#if UNITY_EDITOR
					_cachedConfigPath = Application.streamingAssetsPath + "/Config/";
#else
                    _cachedConfigPath = Application.persistentDataPath + "/Config/";
#endif
				}
				return _cachedConfigPath;
			}
		}

		private string _cachedConfigPath;

		public string LOCAL_CONFIG_STORAGE_DIRECTORY
        {
            get
            {
                if (string.IsNullOrEmpty(_cachedLocalConfigPath))
                {
					_cachedLocalConfigPath = Application.streamingAssetsPath + "/Config/";
                }
				return _cachedLocalConfigPath;
            }
        }

		private string _cachedLocalConfigPath;
	}
}