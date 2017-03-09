﻿using StructureMap;
using System.Collections.Generic;

namespace DigiBot
{
    public interface IUser
    {
        string Name { get; }
        string Id { get; }
        IEnumerable<string> Roles { get; }
        bool IsAdmin { get; }
        bool IsBot { get; }
        //IContainer Scope { get; }
    }
}