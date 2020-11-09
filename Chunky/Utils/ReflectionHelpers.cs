using System.Linq.Expressions;
using System.Reflection;

namespace Chunky.Utils
{
    /// <summary>
    ///     Invokes a constructor of type <typeparamref name="T" /> with the given arguments,
    ///     and returns the result.
    /// </summary>
    /// <param name="args">The arguments to pass to the constructor</param>
    /// <typeparam name="T">The object type.</typeparam>
    public delegate T ObjectActivator<out T>(params object[] args);

    /// <summary>
    ///     Exposes reflection-related helper functions.
    /// </summary>
    public static class ReflectionHelpers
    {
        /// <summary>
        ///     Constructs a new <see cref="ObjectActivator{T}" /> for the given type constructor.
        /// </summary>
        /// <param name="ctor">The constructor to build an activator for.</param>
        /// <typeparam name="T">The desired activator return type.</typeparam>
        /// <returns>A new activator in the form of a delegate function.</returns>
        public static ObjectActivator<T> GetActivator<T>
            (ConstructorInfo ctor)
        {
            var paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            var param =
                Expression.Parameter(typeof(object[]), "args");

            var argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (var i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            var newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            var lambda =
                Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            //compile it
            var compiled = (ObjectActivator<T>) lambda.Compile();
            return compiled;
        }
    }
}