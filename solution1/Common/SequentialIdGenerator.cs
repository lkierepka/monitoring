using System;
using SequentialGuid;

namespace Common
{
    public class SequentialIdGenerator : IIdGenerator
    {
        public Guid NewId() => SequentialGuidGenerator.Instance.NewGuid();
    }
}