using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Util.Interfaces
{
    public interface IConfigurationService
    {
        T GetSetting<T>(string settingName);
        IConfigurationService WithConfiguration(IConfiguration configuration);
    }
}
