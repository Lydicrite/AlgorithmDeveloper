using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl
{
    /// <summary>
    /// Связка FlowLayoutPanel с CustomTabControl для синхронизации списка вкладок и управляющих элементов
    /// </summary>
    public class TabFlowBinder
    {
        private readonly FlowLayoutPanel _panel;
        private readonly CustomTabControl _tabControl;

        private TabPage? _protectedPage; // вкладка, которую нельзя скрывать/закрывать

        // Карта: TabPage -> соответствующий TabEntryControl
        private readonly Dictionary<TabPage, TabEntryControl> _map = new();
        // Отслеживание обработчиков TextChanged для корректной отписки
        private readonly Dictionary<TabPage, EventHandler> _textChangedHandlers = new();
        // Отслеживание обработчиков Disposed для корректного удаления элементов
        private readonly Dictionary<TabPage, EventHandler> _disposedHandlers = new();

        public TabFlowBinder(FlowLayoutPanel panel, CustomTabControl tabControl)
        {
            _panel = panel ?? throw new ArgumentNullException(nameof(panel));
            _tabControl = tabControl ?? throw new ArgumentNullException(nameof(tabControl));

            // В качестве защищённой вкладки выбираем ту, которая содержит панель _panel в своём дереве
            _protectedPage = FindOwnerTabPageForPanel();

            ConfigurePanel();
            WireTabControlEvents();
            RebuildEntries();
        }

        /// <summary>
        /// Удобный способ привязки панели к контролу вкладок
        /// </summary>
        public static TabFlowBinder Attach(FlowLayoutPanel panel, CustomTabControl tabControl)
        {
            return new TabFlowBinder(panel, tabControl);
        }

        private void ConfigurePanel()
        {
            _panel.FlowDirection = FlowDirection.TopDown;
            _panel.WrapContents = false;
            _panel.AutoScroll = true;
            _panel.AutoSize = false;
        }

        private void WireTabControlEvents()
        {
            _tabControl.ControlAdded += TabControl_ControlAdded;
            _tabControl.ControlRemoved += TabControl_ControlRemoved;
            _tabControl.TabClosing += TabControl_TabClosing;
            _tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            _tabControl.TabHidden += TabControl_TabHidden;
            _tabControl.TabShown += TabControl_TabShown;
            _tabControl.TabEntryPermissionChanged += TabControl_TabEntryPermissionChanged;
        }

        private void UnwireTabControlEvents()
        {
            _tabControl.ControlAdded -= TabControl_ControlAdded;
            _tabControl.ControlRemoved -= TabControl_ControlRemoved;
            _tabControl.TabClosing -= TabControl_TabClosing;
            _tabControl.SelectedIndexChanged -= TabControl_SelectedIndexChanged;
            _tabControl.TabHidden -= TabControl_TabHidden;
            _tabControl.TabShown -= TabControl_TabShown;
            _tabControl.TabEntryPermissionChanged -= TabControl_TabEntryPermissionChanged;
        }

        public void Dispose()
        {
            UnwireTabControlEvents();
            foreach (var kv in _map.ToList())
            {
                // Отписываем TextChanged
                if (_textChangedHandlers.TryGetValue(kv.Key, out var th))
                {
                    kv.Key.TextChanged -= th;
                    _textChangedHandlers.Remove(kv.Key);
                }

                // Отписываем Disposed
                if (_disposedHandlers.TryGetValue(kv.Key, out var dh))
                {
                    kv.Key.Disposed -= dh;
                    _disposedHandlers.Remove(kv.Key);
                }
                kv.Value.Dispose();
            }
            _map.Clear();
        }

        private void TabControl_ControlAdded(object? sender, ControlEventArgs e)
        {
            if (e.Control is TabPage page)
            {
                // При добавлении новой вкладки создаем элемент
                AddEntryFor(page);
            }
        }

        private void TabControl_ControlRemoved(object? sender, ControlEventArgs e)
        {
            if (e.Control is TabPage page)
            {
                // Если вкладка скрыта (не Dispose), просто обновим состояние, не удаляя элемент
                if (!page.IsDisposed)
                {
                    if (_map.TryGetValue(page, out var entry))
                    {
                        entry.UpdateState();
                    }
                    return;
                }

                // Если вкладка действительно закрыта и диспознута — удалим элемент
                RemoveEntryFor(page);
            }
        }

        private void TabControl_TabClosing(object? sender, TabControlCancelEventArgs e)
        {
            // После закрытия вкладки ее элемент должен быть удален
            // Событие возникает до удаления, поэтому подписаны также на ControlRemoved
            // Здесь можно обновить состояние
            if (e.TabPage != null && _map.TryGetValue(e.TabPage, out var entry))
            {
                // Можем отобразить предзакрытое состояние
                entry.UpdateState();
            }
        }

        private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Обновить заголовки/состояние при смене активной вкладки
            foreach (var entry in _map.Values)
            {
                entry.UpdateTitle();
                entry.UpdateState();
            }
        }

        private void TabControl_TabEntryPermissionChanged(object? sender, TabControlEventArgs e)
        {
            // Реагируем на изменение запрета создания управляющего элемента для конкретной вкладки
            if (e.TabPage == null) return;

            if (_tabControl.GetForbidEntry(e.TabPage))
            {
                // Удаляем элемент, если он существует
                RemoveEntryFor(e.TabPage);
            }
            else
            {
                // Создаём элемент, если он разрешён
                AddEntryFor(e.TabPage);
            }
        }
        private void TabControl_TabHidden(object? sender, TabControlEventArgs e)
        {
            if (e.TabPage != null && _map.TryGetValue(e.TabPage, out var entry))
            {
                entry.UpdateState();
            }
        }

        private void TabControl_TabShown(object? sender, TabControlEventArgs e)
        {
            if (e.TabPage != null && _map.TryGetValue(e.TabPage, out var entry))
            {
                entry.UpdateState();
            }
        }

        /// <summary>
        /// Полностью перестроить список элементов согласно текущему состоянию TabControl.
        /// </summary>
        public void RebuildEntries()
        {
            // Сохраняем старые, чтобы переиспользовать
            var existing = new Dictionary<TabPage, TabEntryControl>(_map);
            _map.Clear();
            _panel.SuspendLayout();
            try
            {
                _panel.Controls.Clear();

                // Собираем полное множество вкладок: видимые + скрытые
                var allPages = GetAllKnownPages();
                foreach (var page in allPages)
                {
                    if (page.IsDisposed) continue;
                    if (_tabControl.GetForbidEntry(page)) continue; // пропускаем запрещённые
                    TabEntryControl entry;
                    if (existing.TryGetValue(page, out var oldEntry) && !oldEntry.IsDisposed)
                        entry = oldEntry;
                    else
                        entry = new TabEntryControl(_tabControl, page);

                    // Проставляем защиту, если это главная вкладка
                    entry.IsProtected = _protectedPage != null && ReferenceEquals(page, _protectedPage);

                    _map[page] = entry;
                    _panel.Controls.Add(entry);

                    // Подписка на обновление заголовка
                    EnsureTextChangedSubscription(page);
                }

                // Удаляем и очищаем все элементы, которые больше не присутствуют (например, запрещены ForbidEntry)
                foreach (var kv in existing)
                {
                    if (!_map.ContainsKey(kv.Key))
                    {
                        RemoveEntryFor(kv.Key);
                    }
                }
            }
            finally
            {
                _panel.ResumeLayout();
            }
        }

        private IEnumerable<TabPage> GetAllKnownPages()
        {
            // Видимые страницы из TabPages
            var visible = _tabControl.TabPages.Cast<TabPage>().ToList();

            // Попробуем получить скрытые страницы через отражение приватного поля _TabPages, если доступно
            var all = new List<TabPage>(visible);
            try
            {
                var field = typeof(CustomTabControl).GetField("_TabPages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var backup = field?.GetValue(_tabControl) as List<TabPage>;
                if (backup != null)
                {
                    foreach (var page in backup)
                    {
                        if (!all.Contains(page))
                            all.Add(page);
                    }
                }
            }
            catch { }

            return all;
        }

        private void AddEntryFor(TabPage page)
        {
            if (page.IsDisposed) return;
            if (_tabControl.GetForbidEntry(page)) return; // не создаём элемент для запрещённой вкладки
            if (_map.ContainsKey(page))
            {
                _map[page].UpdateTitle();
                _map[page].UpdateState();
                EnsureTextChangedSubscription(page);
                EnsureDisposedSubscription(page);
                return;
            }

            var entry = new TabEntryControl(_tabControl, page);

            // Отмечаем защиту
            entry.IsProtected = _protectedPage != null && ReferenceEquals(page, _protectedPage);

            _map[page] = entry;
            _panel.Controls.Add(entry);

            EnsureTextChangedSubscription(page);
            EnsureDisposedSubscription(page);
        }

        private void RemoveEntryFor(TabPage page)
        {
            if (_map.TryGetValue(page, out var entry))
            {
                _panel.Controls.Remove(entry);
                entry.Dispose();
                _map.Remove(page);
            }

            // Отписка от TextChanged
            if (_textChangedHandlers.TryGetValue(page, out var th))
            {
                page.TextChanged -= th;
                _textChangedHandlers.Remove(page);
            }
            // Отписка от Disposed
            if (_disposedHandlers.TryGetValue(page, out var dh))
            {
                page.Disposed -= dh;
                _disposedHandlers.Remove(page);
            }
        }

        private void EnsureDisposedSubscription(TabPage page)
        {
            if (_disposedHandlers.ContainsKey(page)) return;
            EventHandler handler = (s, e) =>
            {
                // На случай, если закрытие произошло без события ControlRemoved/TabClosing
                RemoveEntryFor(page);
            };
            _disposedHandlers[page] = handler;
            page.Disposed += handler;
        }
        private void EnsureTextChangedSubscription(TabPage page)
        {
            if (_textChangedHandlers.ContainsKey(page)) return;
            EventHandler handler = (s, e) =>
            {
                if (_map.TryGetValue(page, out var entry))
                {
                    entry.UpdateTitle();
                }
            };
            _textChangedHandlers[page] = handler;
            page.TextChanged += handler;
        }
        private TabPage? FindOwnerTabPageForPanel()
        {
            // Идём вверх по дереву родителей от _panel, пока не найдём TabPage
            Control? c = _panel;
            while (c != null)
            {
                if (c is TabPage tp)
                    return tp;
                c = c.Parent;
            }
            return null;
        }
    }
}