using System.Net;

namespace MWSMan.Core.Model.Utilities;

public class AppDynamicSetting
{
    /// <summary>
    /// ایپی ارسال کننده 
    /// </summary>
    public string SenderIp { get; set; }

    /// <summary>
    /// نام سرور ارسال کننده
    /// </summary>
    public string SenderServerName { get; set; }
    /// <summary>
    /// دامین کلی و پابلیک شبکه
    /// </summary>
    public string localDomain { get; set; }
    //public AppDynamicSetting()
    //{
    //   
    //}
}

