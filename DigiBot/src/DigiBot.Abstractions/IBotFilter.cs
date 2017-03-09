using System;

namespace DigiBot
{
    public interface IBotFilter
    {
        Action<IBotAppBuilder> Configure(Action<IBotAppBuilder> next);
    }
}