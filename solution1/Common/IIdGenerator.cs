using System;

namespace Common
{
    public interface IIdGenerator
    {
        Guid NewId();
    }
}