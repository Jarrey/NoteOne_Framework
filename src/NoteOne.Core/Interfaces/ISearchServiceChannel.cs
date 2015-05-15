using System.Threading.Tasks;
using NoteOne_Core.Common.Models;

namespace NoteOne_Core.Interfaces
{
    public interface ISearchServiceChannel
    {
        void InitializeServiceChannelModels();

        void AddModel(ServiceChannel serviceChannel, string title, string subTitle);
        Task<bool> RemoveModel(string key);
        void SaveModels();
        void CleanModels();
        bool ContainsModel(string key);

        ServiceChannelModel GetModel(string key);
    }
}