using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ServiceContract
{
    public interface IGetConfiguration
    {
        Task GetConfigAsync(string configNameWithExt);
    }
}
