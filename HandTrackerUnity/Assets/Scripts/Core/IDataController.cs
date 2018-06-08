using Scripts.Core.Config;

namespace Scripts.Core
{
    public interface IDataController
    {
		GameConfig GetConfig { get; }
    }
}