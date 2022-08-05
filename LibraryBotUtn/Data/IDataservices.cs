using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Services.BotConfig
{
    public interface IDataservices
    {
        IFracesRepositori FracesRepositori { get; }
        IAutenticationRepositori AuthRepositori { get; }
        IChatRepositori ChatRepositori { get; }
        IClienteRepositori ClienteRepositori { get; }
        IStoreRepositori StoreRepositori { get; }
        ISemilleroRepositori SemilleroRepositori { get; }
    }
}
