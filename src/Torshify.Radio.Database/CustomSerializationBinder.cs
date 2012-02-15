using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace Torshify.Radio.Database
{
    public class CustomSerializationBinder : SerializationBinder
    {
        #region Fields

        internal static readonly CustomSerializationBinder Instance = new CustomSerializationBinder();

        // Fields
        private readonly ThreadSafeStore<TypeNameKey, Type> _typeCache = new ThreadSafeStore<TypeNameKey, Type>(new Func<TypeNameKey, Type>(GetTypeFromTypeNameKey));

        #endregion Fields

        #region Methods

        // Methods
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.FullName;
            typeName = serializedType.FullName;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            return this._typeCache.Get(new TypeNameKey(assemblyName, typeName));
        }

        private static Type GetTypeFromTypeNameKey(TypeNameKey typeNameKey)
        {
            string assemblyName = typeNameKey.AssemblyName;
            string typeName = typeNameKey.TypeName;
            if (assemblyName == null)
            {
                return Type.GetType(typeName);
            }

            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyName);
            //Assembly assembly = Assembly.Load(assemblyName);
            if (assembly == null)
            {
                throw new JsonSerializationException(string.Format("Could not load assembly '{0}'.",assemblyName));
            }
            Type type = assembly.GetType(typeName);
            if (type == null)
            {
                throw new JsonSerializationException(string.Format("Could not find type '{0}' in assembly '{1}'.", typeName, assembly.FullName));
            }
            return type;
        }

        #endregion Methods

        #region Nested Types

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        internal struct TypeNameKey : IEquatable<CustomSerializationBinder.TypeNameKey>
        {
            internal readonly string AssemblyName;
            internal readonly string TypeName;

            #region Constructors

            public TypeNameKey(string assemblyName, string typeName)
            {
                this.AssemblyName = assemblyName;
                this.TypeName = typeName;
            }

            #endregion Constructors

            #region Methods

            public override int GetHashCode()
            {
                return (((this.AssemblyName != null) ? this.AssemblyName.GetHashCode() : 0) ^ ((this.TypeName != null) ? this.TypeName.GetHashCode() : 0));
            }

            public override bool Equals(object obj)
            {
                return ((obj is CustomSerializationBinder.TypeNameKey) && this.Equals((CustomSerializationBinder.TypeNameKey)obj));
            }

            public bool Equals(CustomSerializationBinder.TypeNameKey other)
            {
                return ((this.AssemblyName == other.AssemblyName) && (this.TypeName == other.TypeName));
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}