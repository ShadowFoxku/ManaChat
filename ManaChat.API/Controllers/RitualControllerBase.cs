using ManaFox.Core.Flow;
using Microsoft.AspNetCore.Mvc;

namespace ManaChat.API.Controllers
{
    public class RitualControllerBase : ControllerBase
    {
        public static bool IsRitualValid<T>(Ritual<T> ritual, out string message)
        {
            if (ritual == null)
            {
                message = "Rital was null - this indicates server errors";
                return false;
            }

            if (ritual.IsTorn)
            {   
                var tear = ritual.GetTear()!;
                if (tear.IsInternalTear)
                {   // bad state, we should handle this before we reach the API validation layer
                    if (tear.InnerException != null)
                        throw tear.InnerException;
                    throw new Exception($"An unhandled tear occured. Message: {tear.Message}");
                }

                message = tear.Message;
                return false;
            }

            message = string.Empty;
            return true;
        }
    }
}
