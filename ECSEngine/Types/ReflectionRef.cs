using System.Reflection;

namespace ECSEngine.Types
{
    public class ReflectionRef<T>
    {
        private readonly FieldInfo fieldInfo;
        private readonly object origin;

        public T Value
        {
            get => (T)fieldInfo.GetValue(origin);
            set => fieldInfo.SetValue(origin, value);
        }

        public ReflectionRef(FieldInfo fieldInfo, object origin)
        {
            this.fieldInfo = fieldInfo;
            this.origin = origin;
        }
    }
}
