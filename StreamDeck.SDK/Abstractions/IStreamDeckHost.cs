using System.Threading.Tasks;

namespace StreamDeck.SDK.Abstractions
{
    public interface IStreamDeckHost
    {
        Task<int> RunAsync();
    }
}
