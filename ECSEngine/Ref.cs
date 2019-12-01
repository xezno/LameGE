using System.Reflection;

// thinking about this class alone makes me hate myself 10x more
namespace ECSEngine
{
    public class Ref<T>
    {
        private readonly FieldInfo fieldInfo;
        private readonly object origin;

        public T value
        {
            get => (T)fieldInfo.GetValue(origin);
            set => fieldInfo.SetValue(origin, value);
        }

        public Ref(FieldInfo fieldInfo, object origin)
        {
            this.fieldInfo = fieldInfo;
            this.origin = origin;
        }
    }
}
