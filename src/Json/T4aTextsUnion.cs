namespace TeleVisionGuide.Json
{
    public class T4aTextsUnion
    {
        public object[]? AnythingArray;
        public T4aTextsClass? TextsClass;

        public static implicit operator T4aTextsUnion(object[] AnythingArray) => new T4aTextsUnion { AnythingArray = AnythingArray };
        public static implicit operator T4aTextsUnion(T4aTextsClass TextsClass) => new T4aTextsUnion { TextsClass = TextsClass };
    }
}
