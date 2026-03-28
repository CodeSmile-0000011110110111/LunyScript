using Luny;
namespace LunyScript
{
        public struct ComponentApi
        {
                private readonly Script _script;
                private readonly StackTrace _trace;
                internal ComponentApi(Script script, StackTrace trace)
                {
                        _script = script;
                        _trace = trace;
                }
        }
}
