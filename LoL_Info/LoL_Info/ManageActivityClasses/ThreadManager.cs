using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace LoL_Info.Classes
{
    class ThreadManager
    {
        private static Thread mainAdditionalThread;
        
        public static bool IsActive
        {
            get { return mainAdditionalThread.IsAlive; }
        }

        public static void UsualThread(ThreadStart threadDelegate, bool isMainProcess = false, bool isBackground = true,
            ApartmentState state = ApartmentState.STA)
        {
            if (isMainProcess)
            {
                mainAdditionalThread = new Thread(threadDelegate)
                {
                    Name = "MainAdditionalThread",
                    IsBackground = isBackground
                };
                mainAdditionalThread.TrySetApartmentState(state);
                mainAdditionalThread.Start();
            }
            else
            {
                var thread = new Thread(threadDelegate)
                {
                    Name = "AdditionalThread",
                    IsBackground = isBackground
                };
                thread.TrySetApartmentState(state);
                thread.Start();
            }
        }

        public static void ParamThread(ParameterizedThreadStart threadDelegate, object param,
            bool isMainProcess = false, bool isBackground = true, ApartmentState state = ApartmentState.STA)
        {
            if (isMainProcess)
            {
                mainAdditionalThread = new Thread(threadDelegate)
                {
                    Name = "MainAdditionalThread",
                    IsBackground = isBackground
                };
                mainAdditionalThread.TrySetApartmentState(state);
                mainAdditionalThread.Start(param);
            }
            else
            {
                var thread = new Thread(threadDelegate)
                {
                    Name = "AdditionalThread",
                    IsBackground = isBackground
                };
                thread.TrySetApartmentState(state);
                thread.Start(param);
            }
        }

        public static void WriteMessage(string text)
        {
            UsualThread(() => new Windows.MessageBoxControl(text).ShowDialog());
        }
    }
}
