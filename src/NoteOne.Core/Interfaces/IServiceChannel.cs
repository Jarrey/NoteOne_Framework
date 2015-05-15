using System.Collections.ObjectModel;
using NoteOne_Core.Common.Models;

namespace NoteOne_Core
{
    public interface IServiceChannel
    {
        ServiceChannelModel Model { get; }
        ObservableCollection<ServiceChannelModel> Models { get; }
        void RegisterService(Service service);
        void InitializeLogo();
    }
}