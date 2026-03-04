using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AlgorithmDeveloper.UI.Elements.Controls
{
    /// <summary>
    /// Внутренний класс для работы с нативными методами Windows API
    /// </summary>
    internal sealed class NativeUIUtils
    {
        #region Константы Windows

        /// <summary>
        /// Сообщение для получения прямоугольника вкладки
        /// </summary>
        public const int WM_GETTABRECT = 0x130a;
        
        /// <summary>
        /// Стиль окна для прозрачности
        /// </summary>
        public const int WS_EX_TRANSPARENT = 0x20;
        
        /// <summary>
        /// Сообщение для установки шрифта
        /// </summary>
        public const int WM_SETFONT = 0x30;
        
        /// <summary>
        /// Сообщение об изменении шрифта
        /// </summary>
        public const int WM_FONTCHANGE = 0x1d;
        
        /// <summary>
        /// Сообщение горизонтальной прокрутки
        /// </summary>
        public const int WM_HSCROLL = 0x114;
        
        /// <summary>
        /// Сообщение для проверки попадания в область вкладки
        /// </summary>
        public const int TCM_HITTEST = 0x130D;
        
        /// <summary>
        /// Сообщение перерисовки
        /// </summary>
        public const int WM_PAINT = 0xf;
        
        /// <summary>
        /// Стиль окна для RTL макета
        /// </summary>
        public const int WS_EX_LAYOUTRTL = 0x400000;
        
        /// <summary>
        /// Стиль окна для отключения наследования макета
        /// </summary>
        public const int WS_EX_NOINHERITLAYOUT = 0x100000;

        #endregion Контанты Windows





        #region Выравнивание содержимого

        /// <summary>
        /// Любое выравнивание по правому краю
        /// </summary>
        public static readonly ContentAlignment AnyRightAlign = ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight;
        
        /// <summary>
        /// Любое выравнивание по левому краю
        /// </summary>
        public static readonly ContentAlignment AnyLeftAlign = ContentAlignment.BottomLeft | ContentAlignment.MiddleLeft | ContentAlignment.TopLeft;
        
        /// <summary>
        /// Любое выравнивание по верхнему краю
        /// </summary>
        public static readonly ContentAlignment AnyTopAlign = ContentAlignment.TopRight | ContentAlignment.TopCenter | ContentAlignment.TopLeft;
        
        /// <summary>
        /// Любое выравнивание по нижнему краю
        /// </summary>
        public static readonly ContentAlignment AnyBottomAlign = ContentAlignment.BottomRight | ContentAlignment.BottomCenter | ContentAlignment.BottomLeft;
        
        /// <summary>
        /// Любое выравнивание по центру по вертикали
        /// </summary>
        public static readonly ContentAlignment AnyMiddleAlign = ContentAlignment.MiddleRight | ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft;
        
        /// <summary>
        /// Любое выравнивание по центру по горизонтали
        /// </summary>
        public static readonly ContentAlignment AnyCenterAlign = ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter;

        #endregion Выравнивание содержимого





        #region User32.dll

        /// <summary>
        /// Отправляет сообщение управляемому контролу
        /// </summary>
        /// <param name="hWnd">Дескриптор окна</param>
        /// <param name="msg">Код сообщения</param>
        /// <param name="wParam">Первый параметр сообщения</param>
        /// <param name="lParam">Второй параметр сообщения</param>
        /// <returns>Результат обработки сообщения</returns>
        public static nint SendMessage(nint hWnd, int msg, nint wParam, nint lParam)
        {
            // Этот метод заменяет User32 метод SendMessage, но работает только для отправки сообщений управляемым контролам.
            var control = Control.FromHandle(hWnd);

            if (control == null)
                return nint.Zero;

            var message = new Message
            {
                HWnd = hWnd,
                LParam = lParam,
                WParam = wParam,
                Msg = msg
            };

            MethodInfo? wproc = control.GetType().GetMethod("WndProc",
                BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance);

            if (wproc != null)
            {
                object[] args = [message];
                wproc.Invoke(control, args);
                return ((Message)args[0]).Result;
            }

            return nint.Zero;
        }

        #endregion User32.dll





        #region GDI32.dll

        /// <summary>
        /// Удаляет логическое перо, кисть, шрифт, растровое изображение, регион или палитру, освобождая все системные ресурсы, связанные с объектом.
        /// </summary>
        /// <param name="hObject">Дескриптор графического объекта</param>
        /// <returns>Если функция выполняется успешно, возвращаемое значение не равно нулю.</returns>
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        #endregion GDI32.dll

        #region Вспомогательные функции

        /// <summary>
        /// Извлекает младшее слово из IntPtr
        /// </summary>
        /// <param name="dWord">Значение для обработки</param>
        /// <returns>Младшее слово</returns>
        public static int LoWord(nint dWord)
        {
            return dWord.ToInt32() & 0xffff;
        }

        /// <summary>
        /// Извлекает старшее слово из IntPtr
        /// </summary>
        /// <param name="dWord">Значение для обработки</param>
        /// <returns>Старшее слово</returns>
        public static int HiWord(nint dWord)
        {
            if ((dWord.ToInt32() & 0x80000000) == 0x80000000)
                return dWord.ToInt32() >> 16;
            else
                return dWord.ToInt32() >> 16 & 0xffff;
        }

        /// <summary>
        /// Преобразует структуру в IntPtr
        /// </summary>
        /// <param name="structure">Структура для преобразования</param>
        /// <returns>Указатель на структуру</returns>
        public static nint ToIntPtr(object structure)
        {
            nint lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(structure));
            Marshal.StructureToPtr(structure, lparam, false);

            return lparam;
        }

        #endregion Вспомогательные функции





        #region Структуры и перечисления Windows

        /// <summary>
        /// Флаги для проверки попадания в область вкладки
        /// </summary>
        [Flags]
        public enum TCHITTESTFLAGS
        {
            /// <summary>
            /// Попадание в пустую область
            /// </summary>
            TCHT_NOWHERE = 1,
            
            /// <summary>
            /// Попадание в иконку элемента
            /// </summary>
            TCHT_ONITEMICON = 2,
            
            /// <summary>
            /// Попадание в текст элемента
            /// </summary>
            TCHT_ONITEMLABEL = 4,
            
            /// <summary>
            /// Попадание в элемент (иконка или текст)
            /// </summary>
            TCHT_ONITEM = TCHT_ONITEMICON | TCHT_ONITEMLABEL
        }



        /// <summary>
        /// Структура для проверки попадания в область вкладки
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TCHITTESTINFO
        {
            /// <summary>
            /// Инициализирует новый экземпляр структуры TCHITTESTINFO
            /// </summary>
            /// <param name="location">Координаты для проверки</param>
            public TCHITTESTINFO(Point location)
            {
                pt = location;
                flags = TCHITTESTFLAGS.TCHT_ONITEM;
            }

            /// <summary>
            /// Точка для проверки попадания
            /// </summary>
            public Point pt;
            
            /// <summary>
            /// Флаги проверки попадания
            /// </summary>
            public TCHITTESTFLAGS flags;
        }



        /// <summary>
        /// Структура для информации о перерисовке
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct PAINTSTRUCT
        {
            /// <summary>
            /// Дескриптор контекста устройства
            /// </summary>
            public nint hdc;
            
            /// <summary>
            /// Флаг стирания фона
            /// </summary>
            public int fErase;
            
            /// <summary>
            /// Прямоугольник для перерисовки
            /// </summary>
            public RECT rcPaint;
            
            /// <summary>
            /// Флаг восстановления
            /// </summary>
            public int fRestore;
            
            /// <summary>
            /// Флаг инкрементального обновления
            /// </summary>
            public int fIncUpdate;

            /// <summary>
            /// Зарезервированные байты
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }



        /// <summary>
        /// Структура прямоугольника для Windows API
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// Левая координата
            /// </summary>
            public int Left;
            
            /// <summary>
            /// Верхняя координата
            /// </summary>
            public int Top;
            
            /// <summary>
            /// Правая координата
            /// </summary>
            public int Right;
            
            /// <summary>
            /// Нижняя координата
            /// </summary>
            public int Bottom;

            /// <summary>
            /// Инициализирует новый экземпляр структуры RECT
            /// </summary>
            /// <param name="left">Левая координата</param>
            /// <param name="top">Верхняя координата</param>
            /// <param name="right">Правая координата</param>
            /// <param name="bottom">Нижняя координата</param>
            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }

            /// <summary>
            /// Инициализирует новый экземпляр структуры RECT из Rectangle
            /// </summary>
            /// <param name="r">Прямоугольник для преобразования</param>
            public RECT(Rectangle r)
            {
                Left = r.Left;
                Top = r.Top;
                Right = r.Right;
                Bottom = r.Bottom;
            }

            /// <summary>
            /// Создает RECT из координат и размеров
            /// </summary>
            /// <param name="x">X координата</param>
            /// <param name="y">Y координата</param>
            /// <param name="width">Ширина</param>
            /// <param name="height">Высота</param>
            /// <returns>Новая структура RECT</returns>
            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }

            /// <summary>
            /// Создает RECT из указателя
            /// </summary>
            /// <param name="ptr">Указатель на структуру RECT</param>
            /// <returns>Структура RECT</returns>
            public static RECT FromIntPtr(nint ptr)
            {
                return Marshal.PtrToStructure<RECT>(ptr);
            }

            /// <summary>
            /// Получает размер прямоугольника
            /// </summary>
            public Size Size
            {
                get { return new Size(Right - Left, Bottom - Top); }
            }
        }

        #endregion Структуры и перечисления Windows
    }
}
