using NoteOne_Utility.Attributes;

namespace NoteOne_Core.Common
{
    public enum ResponseTypes
    {
        Xml = 0,
        Json = 1,
        String = 2,
        Html = 3,
        Binary = 4,
        Object = 5
    }

    public enum NotificationItemStatusCode
    {
        NotStart = 0,
        Running = 1,
        Stopped = 2,
        Error = 3
    }

    public enum ServiceChannelGroupID
    {
        [Description("ServiceChannelGroupID_PublicService")]
        PublicService = 0,
        [Description("ServiceChannelGroupID_GenernalSetting")]
        GenernalSetting = 1,
        [Description("ServiceChannelGroupID_OnlineService")]
        OnlineService = 2,
        [Description("ServiceChannelGroupID_OfflineService")]
        OfflineService = 3,
        [Description("ServiceChannelGroupID_LocalService")]
        LocalService = 4,
        [Description("ServiceChannelGroupID_OnlinePictures")]
        OnlinePictures = 5,
        [Description("ServiceChannelGroupID_LocalPictures")]
        LocalPictures = 99,
        [Description("ServiceChannelGroupID_CustomSearchPictures")]
        CustomSearchPictures = 7,
        [Description("ServiceChannelGroupID_RecommendedApps")]
        RecommendedApps = 100
    }

    public enum QueryResultTypes
    {
        Single = 0,
        Multi = 1
    }

    public enum ServiceTypes
    {
        Online = 0,
        Local = 1
    }
}