using UnityEngine;

namespace Scripts.Utils
{
    public class ObjectPersister : MonoBehaviour
    {

		private void Awake()
		{
            DontDestroyOnLoad(this);
		}
	}
}