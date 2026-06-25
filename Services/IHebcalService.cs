using BeitKnesetDisplay.Models;

namespace BeitKnesetDisplay.Services
{
    public interface IHebcalService
    {
        Task<DisplayData> GetDisplayDataAsync();
    }
}