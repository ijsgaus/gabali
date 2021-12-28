namespace RabbitRelink.Middlewares;

public delegate DoPublish<T> ProducerMiddleware<T>(DoPublish<T> next);
