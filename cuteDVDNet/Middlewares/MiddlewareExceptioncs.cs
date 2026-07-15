using cuteDVDCore.Exceptions;
using cuteDVDNet.Middlewares.MiddlewareReturns;
using cuteDVDNet.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace cuteDVDNet.Middlewares
{
    public class MiddlewareExceptioncs(RequestDelegate request, ILogger<object> logger, IHubContext<HubDiskErr> hub)
    {
        private readonly RequestDelegate request = request;
        private readonly ILogger<object> logger = logger;
        private readonly IHubContext<HubDiskErr> hub = hub;

        public async Task InvokeAsync(HttpContext context) {
            try
            {
                await request(context);
            }
            catch (Exception ex) {
                ExceptionReturn exceptionReturn = ex switch
                {
                    ExceptionDriveFuncError => new ExceptionReturn("Function get error when try read disk", StatusCodes.Status503ServiceUnavailable),
                    ExceptionDriveNotReady => new ExceptionReturn("Drive not have DVD or CD Disk", StatusCodes.Status503ServiceUnavailable),
                    NotImplementedException => new ExceptionReturn("Method not work now <3", StatusCodes.Status405MethodNotAllowed),
                    NullReferenceException => new ExceptionReturn ("Not found Cd Rom or not files in cd, NULL", StatusCodes.Status503ServiceUnavailable),
                    _ => new ExceptionReturn("Unknown error", StatusCodes.Status503ServiceUnavailable)
                };

                logger.LogError(ex,ex.Message);
                if (exceptionReturn.StatusCode == StatusCodes.Status503ServiceUnavailable)
                    await hub.Clients.All.SendAsync(
                    "DiskError",
                    "DVD Not Ready");
                if (exceptionReturn.StatusCode == StatusCodes.Status405MethodNotAllowed) { }

                else await hub.Clients.All.SendAsync(
                    "DiskError",
                    "Unknown error on disk");
                await context.Response.WriteAsJsonAsync(exceptionReturn);
                context.Response.StatusCode = exceptionReturn.StatusCode;

            }
        }
    }
}
