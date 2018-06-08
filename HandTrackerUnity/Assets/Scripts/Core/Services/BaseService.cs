namespace Scripts.Core.Services
{
    public abstract class BaseService
    {
        protected IDataController _dataController;

        public BaseService(IDataController dataController)
        {
            _dataController = dataController;
        }
    }
}