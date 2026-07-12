using System.Text;

namespace cuteDVDNet.Middlewares
{
    public class InformationMiddleware(RequestDelegate rdelegate, ILogger<object> logger)
    {
        private readonly RequestDelegate rdelegate = rdelegate;
        private readonly ILogger<object> logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            // Включаем буферизацию, чтобы тело можно было прочитать несколько раз
            context.Request.EnableBuffering();

            // --- Логирование ЗАПРОСА ---
            logger.LogInformation("=== Request ===");
            logger.LogInformation("Method: {Method}", context.Request.Method);
            logger.LogInformation("Path: {Path}", context.Request.Path);
            logger.LogInformation("Query: {Query}", context.Request.QueryString);

            // Читаем тело через StreamReader
            using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();
            logger.LogInformation("Body: {Body}", body);

            // Возвращаем поток в начало, чтобы контроллер мог его прочитать
            context.Request.Body.Position = 0;

            // --- Вызов следующего middleware / контроллера ---
            await rdelegate(context);

            // --- Логирование ОТВЕТА (теперь статус корректный) ---
            logger.LogInformation("=== Response ===");
            logger.LogInformation("StatusCode: {StatusCode}", context.Response.StatusCode);
            logger.LogInformation("Headers: {Headers}", context.Response.Headers);
        }
    }
}
