using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Torshify.Radio.Utilities
{
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        #region Fields

        private const string Kernel32DllName = "kernel32.dll";

        #endregion Fields

        #region Properties

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
        }

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        [DllImport(Kernel32DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32DllName)]
        private static extern IntPtr GetConsoleWindow();

        private static void InvalidateOutAndError()
        {
            Type type = typeof(Console);

            System.Reflection.FieldInfo @out = type.GetField(
                "_out",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.FieldInfo error = type.GetField(
                "_error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.MethodInfo initializeStdOutError = type.GetMethod(
                "InitializeStdOutError",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(@out != null, "out can't be null");
            Debug.Assert(error != null, "error can't be null");
            Debug.Assert(initializeStdOutError != null, "stdout can't be null");

            @out.SetValue(null, null);
            error.SetValue(null, null);

            initializeStdOutError.Invoke(null, new object[] { true });
        }

        private static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }

        #endregion Methods
    }
}