using LibraryBotUtn.Common.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Services.BotConfig
{
    public interface IApiServices<T> where T : new()
    {
        Task<FracesEntiti> Frace(string intencion, string token);
    }
    public interface IFracesRepositori : IApiServices<FracesEntiti> { }

    public interface IAutentication<R, U> where U : new()
    {
        Task<R> Auth(string email);
        Task<U> GetUser(TokenEntity entity);
        Task<R> NewUser(ClienteEntity cliente, string token);
    }
    public interface IAutenticationRepositori : IAutentication<ResultadoEntity, UsuarioEntity> { }
    public interface IChat<T> where T : new()
    {
        Task<T> Interaction(InteractionEntity interaction, string token);
    }
    public interface IChatRepositori : IChat<ChatEntity> { }

    public interface ICliente<T, C> where T : new()
    {
        Task<T> GetUser(TokenEntity entity);
        Task<C> Auth(string email);
    }
    public interface IClienteRepositori : ICliente<ClienteEntity, ClienteVerificado> { }
    public interface IStore
    {
        Task<(JObject content, string etag)> LoadAsync(string key);

        Task<bool> SaveAsync(string key, JObject content, string etag);
    }
    public interface IStoreRepositori : IStore { }

}
